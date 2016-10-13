using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.基元线程同步构造
{
 public   class 内核构造模式
    {
        public static void Demo()
        {
            /*
              windows提供了几个内核模式的构造来同步线程。内核模式的构造比用户模式的构造慢得多，一个原因是他们要求windows操作系统自身配合，
              另一个原因是内核对象上调用的每个方法都造成调用线程托管代码转换成本机（native）用户模式代码，再转换为本机内核代码，然后朝相反方向返回。
              这些需要大量cpu时间，经常执行会对应用程序总体性能造成负面影响。

            优点:
             内核模式的构造检测到一个资源上的竞争时，windwos会阻塞输掉的线程。使它不占着一个cpu自旋。不浪费处理器资源。
             内核模式的构造可以实现本机和托管线程相互间的同步
             内核模式的构造可同步再同一台机器的不同进程中运行的线程。
             内核模式构造可应用安全性设置，防止未经授权账户访问他们。
             线程可一直阻塞，直到集合中的所有内核模式构造都可以用，或直到集合中的任何内核模式可构造可用。
             内核模式的构造上阻塞的线程可以指定超时值。指定时间内访问不到希望的资源，线程就可以解除阻塞并执行其他任务。
             事件和信号量是两种基元内核模式线程同步构造。互斥体构造是基于他们2个之上的构建。

             C#提供一个名为WaitHandle的抽象基类，他作用就是包装一个windows内核对象句柄，FCL提供了几个从WaitHanlde派生的类
             所有类都在System.Threading命名空间定义。
             WaitHandle
               EventWaitHandle
                  AutoResetEvent
                  ManualResetEvent
             Semaphore
             Mutex
             WaitHandle 内部有个SafeWaitHandle字段。他容纳了一个Win32内核对象句柄。
             这个字段是再构造一个具体的WaitHandle派生类时初始化的。
             在内核模式上构造调用的每个方法都代表一个完整的内存删栏。（删栏的意思是：表明调用这个方法之前的任何变量写入都必须再这个方法调用之前发生，而这个调用
             之后的任何变量读取都必须再这个调用之后发生。）
             */
            //bool blnIsRunning;
            //Mutex mutexApp = new Mutex(false, Assembly.GetExecutingAssembly().FullName, out blnIsRunning);
            //if (!blnIsRunning)
            //{
            //    Console.WriteLine("已经运行了");
            //}
            //else {
            //    Console.WriteLine("第一次运行");
            //}


        }
        /// <summary>
        /// 信号量
        /// </summary>
        public static void Demo1()
        {
            /*
             信号量是由内核维护的Int32变量，信号量为0时，在信号量上等待的线程会阻塞，信号量大于0时会解除阻塞。
             在信号量等待的线程解除阻塞时，内核自动从信号量计数中减1.信号量还关联了最大Int32的值，当前计数绝不允许超过最大计数。
             class Semaphore:WaitHandle{
               public Semaphore(Ini32 initalCount,Int32 maximunCount);
               public Int32 Relese();调用release(1)返回上一个计数
               public Int2 Relese(Int32 releaseCount)返回上一个计数
             }

            三种内核模式基元
            多个线程在一个自动重置事件上等待时，设置事件只导致一个线程被解除阻塞。
            多个线程再一个手动重置事件等待时，设置事件导致所有线程解除阻塞。
            多个线程再一个信号量上等待时，释放信号量导致releaseCount个线程被解除阻塞。（releaseCount是传给Semaphore的 Relese方法的实参
            因此，自动重置事件再行为上和最大计数为1的信号量非常相似。两者区别在于，可以在一个自动重置事件上连续多次调用set。同时仍然只有一个线程解除阻塞
            相反再一个信号量上多次调用Release，会是它的内部计算一直递增，这可能解除大量线程的阻塞。
            如果在一个信号量上多次调用Release会导致它计数超过最大计数，这时会抛出SemaphoreFullException异常。
             *  */
            Boolean createNew;
            //正确
            var s = new Semaphore(1, 1, Assembly.GetExecutingAssembly().FullName, out createNew);
            if (createNew)
            {
                Console.WriteLine("第一次创建");
            }
            else
            {
                Console.WriteLine("第2次创建");
            }
            //错误 不能释放
            //using (new Semaphore(1, 1, Assembly.GetExecutingAssembly().FullName, out createNew))
            //{
            //    if (createNew)
            //    {
            //        Console.WriteLine("第一次创建");
            //    }
            //    else
            //    {
            //        Console.WriteLine("第2次创建");
            //    }

            //}

        }
        /// <summary>
        /// 信号量
        /// </summary>
        public static void Demo3() {

            SimpleSemaphoreWaitLock lock1 = new SimpleSemaphoreWaitLock(4);


        }
        /// <summary>
        /// 事件构造
        /// </summary>
        public static void Demo2() {
            /*
             * 事件其实只是由内核维护的Boolean变量，事件为false 在事件上等待的线程就阻塞，事件为true就解除阻塞。
             * 有两种事件，自动重置事件和手动重置事件。
             * 当自动重置事件设置为true时，他只唤醒一个阻塞的线程。因为再解除第一个线程阻塞后，内核将事件自动重置回false。造成其他线程继续阻塞
             * 手动重置事件为true时，它解除正在等待他的所有线程阻塞，因为内核不将事件重置回false。现在，你的代码必须将事件手动重置回false.
             * 
             * class EventWaitHandle：WaitHandle{
             *  public Boolean Set(); //将设置为True总是返回True
             *  public Boolean Reset();//将设置为Flase总是返回True
             * }
             * //自动
             * class AutoResetEvent:EventWaitHandle{
             *  
             * }
             * //手动
             * class ManulResetEvent:EventWaitHandle{
             * }
             */
            Int32 x = 0;
            const Int32 interations = 10000000;
            Stopwatch sw = Stopwatch.StartNew();
            for (Int32 i = 0; i < interations; i++) {
                x++;
            }
            Console.WriteLine("inc :{0}", sw.ElapsedMilliseconds);
            sw.Restart();
            for (Int32 i = 0; i < interations; i++)
            {
                M();
                x++;
                M();
            }
            Console.WriteLine("M inc :{0}", sw.ElapsedMilliseconds);
            SpinLock sl = new SpinLock(false);
            sw.Restart();
            for (Int32 i = 0; i < interations; i++)
            {
                Boolean taken = false;
                sl.Enter(ref taken);
                x++;
                sl.Exit();
            }
            Console.WriteLine("SpinLock inc :{0}", sw.ElapsedMilliseconds);

            using (var si = new SimpleWaitLock()) {
                sw.Restart();
                for (Int32 i = 0; i < interations; i++)
                {
                    si.Enter();
                    x++;
                    si.Leave();
                }
                Console.WriteLine("SimpleWaitLock inc :{0}", sw.ElapsedMilliseconds);
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void M() {

        }

        /// <summary>
        /// 互斥锁Mutex
        /// </summary>
        public static void Demo5()
        {
            /*
              Mutex对象回查询调用线程的Int32ID，记录是哪个线程获得了它。一个线程调用ReleaseMutex时，Mutex确保调用线程
              就是获取Mutex的那个线程。如诺不然，Mutex的状态不会改变，而ReleaseMutex回抛出System.ApplicationException异常
              而拥有Mutex的线程因为任何原因终止，在Mutex上等待的某个线程会因为抛出System.Threading.AbandonedMutexException异常而被唤醒。
              该异常通常会成为未处理异常，而终止整个进程。这是好事，因为线程获取了一个Mutex后，可能再更新完Mutex所保护的数据之前终止，如果
              其他线程捕捉了AbandonedMutexException，就可能试图访问损坏的数据，造成无法预料的结果和安全隐患。
              其次，Mutex对象维护着一个递归计数。。指出拥有该Mutex的线程拥有了它多少次，如果一个线程当前拥有一个Mutex，而后该线程再次再Mutex
              上等待。只有计数变成0，另外一个线程才能成为该Mutex的所有者。
             */
            SomeClass cl = new SomeClass();
            cl.Method1();
        }
        /// <summary>
        /// 实现互斥锁
        /// </summary>
        public static void Demo6() {
            Thread t = new Thread(T);
            Thread t1 = new Thread(T);
            t.Start();
            t1.Start();

        }
       static RecursiveAutoResetEvent r = new RecursiveAutoResetEvent();
        public static void T()
        {
            r.Enter();
            Thread.Sleep(5000);
            Console.WriteLine("1111111");
            r.Leave();

        }
    }
    class RecursiveAutoResetEvent : IDisposable{
        private readonly AutoResetEvent au = new AutoResetEvent(true);
        private Int32 m_currentThreadId=0;
        private Int32 m_recursionCount = 0;

        public void Dispose()
        {
            au.Dispose();
        }

        public void Enter()
        {
             var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            if (threadId == m_currentThreadId) {
                m_recursionCount++;return;
            }
            au.WaitOne();

            m_currentThreadId = threadId;
            m_recursionCount = 1;
        }

        public void Leave() {
            if (m_currentThreadId != System.Threading.Thread.CurrentThread.ManagedThreadId) {
                throw new InvalidOperationException();
            }
            if (--m_recursionCount == 0) {
                m_currentThreadId = 0;
                au.Set();
            }
        }

    }

    class SomeClass : IDisposable
    {
        private readonly Mutex m_lock = new Mutex();
        public void Method1() {
            m_lock.WaitOne();
            Console.WriteLine("Method1");
            Method2();
            m_lock.ReleaseMutex();
        }

        public void Method2()
        {
            m_lock.WaitOne();
            Console.WriteLine("Method2");
            m_lock.ReleaseMutex();

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    class SimpleWaitLock : IDisposable {
        private readonly AutoResetEvent reset;
        public SimpleWaitLock() {
            reset  = new AutoResetEvent(true);//最开始可以自由使用 阻塞还是不组赛
        }

        public void Enter()
        {
            reset.WaitOne();
        }
        public void Leave() {
            //让另一个线程访问。
            reset.Set();
        }
        public void Dispose()
        {
            reset.Dispose();
        }
    }

    class SimpleSemaphoreWaitLock : IDisposable
    {
        private readonly Semaphore reset;
        public SimpleSemaphoreWaitLock(Int32 maxCount)
        {
            reset = new Semaphore(maxCount, maxCount);
        }

        public void Enter()
        {
            reset.WaitOne();
        }
        public void Leave()
        {
            //让另一个线程访问。
            reset.Release(1);
        }
        public void Dispose()
        {
            reset.Dispose();
        }
    }
}
