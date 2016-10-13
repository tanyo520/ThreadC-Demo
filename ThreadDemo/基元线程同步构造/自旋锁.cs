using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.基元线程同步构造
{
   public class 自旋锁
    {
       private static SimpleSpinLock s = new SimpleSpinLock();
        private static bool token = false;
        private static System.Threading.SpinLock locks = new SpinLock();// spinlock提供超时支持，再黑客技的时候采用了SpinWait技术 提高性能。SpinWait封装了
        //黑科技的最新研究成果
        /*
          黑科技式指让希望获得资源的线程暂停执行。使当前拥有资源的线程能执行他的代码并让出资源。
          SpinWait内部调用Thread的静态Sleep，Yield和SpinWait方法。
          当休眠System.TimeOut.Infinite（中的值为-1），系统将永远不会调度线程。这样做没啥意义。更好的做法是让线程退出。
          回收他的栈和内核对象。可以向sleep传递0,告诉系统放弃当前时间片的剩余部分，强迫系统调度到另一个线程。
          但系统可能重新条都了刚才的sleep的线程（如果没有相同或更高优先级的其他可调度线程，就会发生这种情况。）
          线程要求windows在当前cpu上调度另一个线程，这通过Thread的Yield方法来实现的。如果windows发现另一个线程准备好再当前处理器上运行，yield就会返回true。
          调用yeild的线程回提前结束他的时间片，所选的线程得以运行一个时间片，然后调用yield的线程再次被调度。开始用一个全新的时间片运行。如果windows发现没有其他线程
          准备再当前处理器上运行。yield就会返回false调用yield的线程继续运行他的时间片。
             
             */
        public static void Aceess()
        {
            s.Enter();
            Console.WriteLine("111111111");
            s.Leave();
        }

        public static void Aceess1()
        {
            token = false;
            locks.Enter(ref token);
            Console.WriteLine("111111111");
            locks.Exit();
        }


        public static void Demo1()
        {
            Thread t = new Thread(Aceess);
            Thread t1 = new Thread(Aceess);
            Thread t2 = new Thread(Aceess);
            t.Start();
            t2.Start();
            t1.Start();

        }
        public static void Demo2()
        {
            Thread t = new Thread(Aceess1);
            Thread t1 = new Thread(Aceess1);
            Thread t2 = new Thread(Aceess1);
            t.Start();
            t2.Start();
            t1.Start();

        }
    }
    internal struct SimpleSpinLock {
        private Int32 m_ResourceInUse;//0为false 1为true
        public void Enter() {
            while (true) {
                if (Interlocked.Exchange(ref m_ResourceInUse, 1) == 0) return;
                //添加黑科技
            }
        }

        public void Leave() {
            Volatile.Write(ref m_ResourceInUse, 0);
        }
    }
}
