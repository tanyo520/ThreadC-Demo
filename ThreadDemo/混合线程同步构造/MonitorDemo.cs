using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    class MonitorDemo
    {
        public static void Demo() {
            /*
              最常用的混合型线程同步构造就是Monitor,他提供了自旋，线程所有权和递归的互斥锁之所以常用是因为他资格最老。
              c#内建关键字支持它,JIT编译器对它知之甚详，而且CLR自己也再代表你的应用程序使用它。 存在很多问题。很容易造成代码中出现BUG

            同步块
            它为内核对象.拥有线程的ID，递归计数，以及等待线程计数提供相应的字段，Monitor是静态类，它接收对任何堆对象的引用
            Enter(Object obj);
            Exit(Object obj);
            TryEnter(Object obj,Int32 millisecondesTimeout);
            Enter(Object obj,ref Boolean lockTaken);

            为每个对象都关联一个同步数据结构显得很浪费，尤其是考虑到大多数对象的同步块从不使用，为节省内存，CLR团队采用一种
            更经济的方式提供刚才描述的功能。
            CLR初始化时在堆中分配一个同步块数组，每当对象创建时，有两个额外的开销字段与它关联，
            1.类型对象指针。包含类型的“类型对象”的内存地址
            2.同步块索引，包含同步块数组中的一个整数索引。
            对象在构造时，它的同步块索引为-1，表面不引用任何同步块
            调用Monitor.Enter时，CLR再数组中找到一个空白同步块。并设置对象的同步块索引。让它引用该同步块。
            同步块和对象是动态关联。调用Exit时，会检查是否有其他线程正在等待使用对象的同步块。如果没有线程等待它。同步
            块就自由了。Exit将对象同步块索引设置为-1.它就可以和其它对象关联了。

            问题点：
            1.变量能引用一个代理对象。前提是变量引用的那个对象的类型派生自System.MarshalByRefObject.调用Monitor时，
            传递对代理对象的引用，锁定的是代理对象而不是代理对象实际的对象。
            2.如果线程调用Enter，向它传递的式类型对象的引用，而这个类型对象式以AppDomain中立的方式加载。线程就会跨越
            进程中所有的AppDomain在那个类型上获取锁，这是CLR的bug.它破坏了AppDomain本应提供的隔离性。这个bug很难再高性能的情况下修护
            所以一直没有修复。建议永远不要向Monitor传递类型对象引用
            3.由于字符串可以留用。所以两个完全独立的代码可能再不知情的情况下获取对内存中的一个String对象的引用。如果将
            String对象引用传递给Monitor方法。两个独立的代码段现在就会在不知情的情况下以同步方式执行。
            4.跨越AppDomain边界传递字符串时，CLR不创建字符串的副本，相反。它只是将对字符串的一个引用传递给他的AppDomain
            这增强了性能，理论上式可行的，因为String对象本来就是不可变。但和其他对象一样,Sting对象关联了一个同步块索引。
            这个索引是可变的。使不同AppDomain中的线程在不知情的情况下开始同步。这是CLR的AppDomain隔离存在的另一个Bug。
            我的建议是永远不要将String引用传递给Monitor的方法。
            5.由于Monitor方法要获取一个Object。所以传递值类型会导致值类型被装箱。造成线程在已装箱对象上获取锁。每次调用
            Monitor.Enter都回在一个完全不同的对象上获取锁，造成完全无法实现线程同步。
            6.向方法应用[MethodImpl(MethodImplOptions.Synchronized)]特性。会造成JIT编译器用Monitor.Enter和Monitor.Exit调用包围方法的本机代码。
            如果方法是实例方法会将this传给Monitor的方法。锁定隐式公共的锁。如果方法是静态的，对类型的类型对象的引用传递
            给这些方法。造成锁定“AppDomain中立”的类型。我的建议是永远不要使用这个特性。
            7.调用类型的类型构造器时，CLR要获取类型对象上的一个锁，确保只有一个线程初始化对象及其静态字段，同样的，这个
            类型可能以“AppDomain中立”的方式加载。所以会出问题，例如，假定类型构造器代码进去死循环，进程中的所有AppDomain
            都无法使用该类型。我的建议式避免使用类型构造器（静态构造器），或者至少保持他们的短小和简单。
            除了上面说的，可能还有更糟糕的情况。C#提供lock关键字来简化语法。
             lock (this) {
            }
            等价于
            Boolean lockTaken =false;
            try{
             Monitor.Enter(this,ref lockTaken);
            }finally{
                 if(lockTaken){
                 Monitor.Exit(this);
                }
            }
            1.C#团队认为他们再Finally块中调用Monitor.Exit是帮了你一个大忙，这样就确保锁总是得以释放。无论try块中发生了什么。但这是他们一厢情愿的想法
            在try块中，如果更改状态时，发生异常，这个状态就会处于损坏状态，锁在finally块中退出时，另一个线程可能开始操作损坏的状态。
            显然，更好的解决方法是让应用程序挂起，而不是让它带着损坏的状态继续运行。这样不仅结果很难预料，还有可能引发安全隐患。
            2.进入和离开try块回影响方法性能，有的JIT编译器不会内联含有try块的方法。造成性能进一步下降。结果是不仅代码速度变慢了
            ，还会造成线程访问损坏的状态。我的建议是杜绝使用C#的lock语句。

            讨论Boolean lockTaken变量了，下面是这个变量试图解决的问题。假定一个线程进入try块。但在调用Monitor.Enter之前退出。
            finally块会得到调用，但它的代码不应退出锁。locakTaken变量就是为了解决这个问题而设计的。SpinLock结构也支持这个模式。
             */


        }
    }
}
