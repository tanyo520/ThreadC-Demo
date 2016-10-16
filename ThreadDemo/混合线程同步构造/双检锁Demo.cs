using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    class 双检锁Demo
    {
        /*
         * 单实例，对象构造推迟到应用程序首次请求该对象时进行。也称延迟初始化。
         * 如果多线程下 多个线程请求单实例可能会出问题。这时必须使用一些线程同步机制确保单实例对象只被构造一次。
         * CLR很好的支持双检锁。归功于CLR的内存模型及volatile字段访问。
         */
    }
    public sealed class Singleton {
        //s_lock对象是实现线程安全所需要的，定义这个对象时，我们假设创建单例对象的，代价高于创建一个System.Object对象。并假设可能根本不需要创建单例对象/
        //否则更经济，更简单的做法是再一个类构造中创建单实例对象。
        private static Object s_lock = new object();
        private static Singleton s_value = null;
        /// <summary>
        ///私有构造防止new对象
        /// </summary>
        private Singleton() {

        }
        public static Singleton GetSingleton() {

            if (s_value != null) return s_value;
            Monitor.Enter(s_lock);
            if (s_value == null) {
                Singleton temp = new Singleton();
                Volatile.Write(ref s_value, temp);
            }
            Monitor.Exit(s_lock);
            return s_value;

        }
    }

    /*
     这个模式在java中会出问题，java虚拟机再GetSingleton方法开始的时候将s_value的值读入CPU寄存器。然后，对第二个if语句
     求值时，他直接查询寄存。造成第二个if语句的总是为true，结果多个线程都会创建实例。只有在多线程恰好同时调用GetSingleton的情况下才会发生，概率小，所以
     很难发现。

    在CLR中，对任何锁方法的调用都构成了一个完整的内存删栏，在删栏之前写入的任何变量都必须再删栏之前完成。
    再删栏之后读取的变量必须都再删栏之后开始。意味着s_value字段的值必须再调用了Monitor.Enter之后重新读取。调用
    前缓存到寄存器的东西作不了数。
    用Volatile.Write写s_value的问题。你极有可能这样写s_value=new Singleton();
    你的想法是让编译器生成代码为一个Singleton分配内存，调用构造器来初始化字段，再将引用赋值给s_value字段。
    使一个值对其他线程可见称为发布。。但那只是你的一厢情愿的想法。编译器会这样做。
    为Singleton分配内存，将引用发布给s_value。再调用构造器，从单线程的角度出发，像这样改变顺序无关紧要，但在将引用发布给
    s_value之后，并再调用构造器之前，如果一个线程调用了GetSingleton方法，那么会发送啥，这个线程会发现s_value不为null，
    所以会开始使用singleton对象，但对象构造器还没有结束执行。这是一个很难追踪的Bug，尤其是他完全是由于计时而造成。
    用Volatile.Write解决了这个问题，它保证了temp构造器结束之后才发布到s_value。解决这个问题的另一个办法就是使用volatile关键字
    这使向s_value的写入变得具有易变性。同样构造器必须在写入发生前结束运行，但这会使所有读取操作都具有易变性。完全没必要。

     */

        /*
         不延迟加载完全不必要考虑多线程问题。
         */
    class Singleton1 {
        private static Singleton1 s = new Singleton1();
        private Singleton1() {
        }
        
        public static Singleton1 GetSingleton() {
            return s;
        }
    }
    class Singleton2 {
        private static Singleton2 sing = null;
        private Singleton2() { }
        public static Singleton2 GetSingleton() {
            if (sing != null) return sing;
            if (sing == null) {
                Singleton2 si = new Singleton2();
                Interlocked.CompareExchange(ref sing, si, null);
                //如果这个线程竞争失败，新建的si会被垃圾回收
                /*
             
                    如果一个线程池线程在一个Monitor或者任何内核模式的线程同步构造上阻塞，线程池就会创建另一个线程来保持CPU的饱和。
                    因此会分配并初始化更多的内存。而且所有DLL会受到一个线程连接的通知。  
                    使用  CompareExchange则永远不会发生这种情况，当然只有再构造器没有副作用的时候才能使用这个技术。
                  */
            }
            return sing;
        }
    }
}
