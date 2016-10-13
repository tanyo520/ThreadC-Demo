using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
   public class ThreadPoolDemo
    {

        public void InworkDemo() {
            Console.WriteLine("main ThreadId:{0}", Thread.CurrentThread.ManagedThreadId);
            ThreadPool.QueueUserWorkItem(Complete, 222);
            Console.WriteLine("this  is end");
        }

        public void Complete(object state) {
            Console.WriteLine("ThreadId:{0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("state is :{0}", state);
        }
        /// <summary>
        /// 线程池如何管理线程
        /// </summary>
        public void Demo() {
             /*
               建议使用CLR自带的线程池,因为很难搞出一个更好的。
               
            1. 设置线程池的限制。
             CLR允许开发人员设置线程池创建最大线程数。但实践证明不应该设置线程池的上限。因为可能发生饥饿或死锁，
             由于饥饿和死锁的问题，CLR团队一直再稳步的增加线程池默认拥有的最大线程数。目前默认大约1000个线程。
            这个基本可以看成不限数量。32位操作系统2G内存，每个线程准备1M 最多能1360线程。
            
            64位提供8TB地址空间，可以创建千百万个线程。。但分配这么多纯属浪费。
            ThreadPool提供几个静态方法，可以调用他们设置和查询线程池的线程数。GetMaxThreads,SetMaxThreads,GetMinThreads,
            SetMinThreads.和GetAvailableThreads.强烈建议不要嗲用上述任何方法。 
             */
        }
        /// <summary>
        /// 如何管理工作者线程
        /// </summary>
        public void Demo1() {
             /*
              * ThreadPool.QueueUserWorkItem方法和Timer类总是将工作项放到全局队列中。工作者线程采用先入先出(FIFO)算法将工作项从
              * 全局队列中取出，并处理他们。由于多个工作者线程可能同时从全局队列中拿走工作项。所以所有工作者
              * 线程都竞争一个线程同步锁，以保证两个或者多个线程不能获取同一个工作项。这个线程同步锁再某些应用中可能成为瓶颈
              * 对伸缩性和性能造成限制。
              * 
              * 
              * 默认TaskScheduler（TaskScheduler.Default）来调度task。非工作者线程调度一个task时，该task被添加到全局队列。
              * 但每个工作者线程都有自己的本地队列，工作者线程调度一个Task时,该task被添加到调用的线程的本地队列。
              * 工作者线程准备好处理工作项时，他总是先检查本地队列来查找一个task，存在一个task工作者线程就会从本地队列移除task并处理工作项。
              * 工作者采用后入先出(LIFO)的算法把任务从本地队列中取出，由于工作者线程是唯一一个允许他访问本地队列头的线程。
              * 所以无需同步锁。而且再队列中添加和删除task的速度非常快，这个行为的副作用是task按照和进入队列时的相反顺序执行。
              * 
              * 如果工作者线程发现它本地队列变空了，会尝试从另一个工作者线程的本地队列中偷一个task，这个task是从本地队列的尾部偷走的，并要求获取一个线程同步锁
              * 这对性能有少许影响，这种偷的行为很少发生，很少需要获取锁，如果所有本地队列都为空，那么工作者线程会从全局队列
              * 提取工作项到本地队列。(FIFO算法)(获取它的锁)，如果全局队列也是空，那么工作者线程进入睡眠状态。等待事情的发生。
              * 如果时间太长他会醒来，并销毁自身，允许系统回收线程使用的资源(内核对象，栈,TEB)
              * 
              * 注意：线程池从来不保证排队中的工作项的处理顺序。尤其是考虑到多个线程可能同时处理工作项，但上述副作用使这个问题变的
              * 恶化了，你必须保证自己的应用程序对于工作项或task的执行顺序不作任何预设。
              * 非工作者线程调用=> CLR线程池=>全局队列And工作者线程|工作者线程有自己的本地队列。
              * 
              * 
              * 线程池快速创建工作者线程，使工作者线程的数量等于传给ThreadPool的SetMinThreads方法的值，如果从不调用这个，那么默认
              * 等于你的进程允许使用的CPU数量，这个由进程的affnity mask 关联掩码决定。通常你的进程允许使用机器上所有的cpu。
              * 所以线程池创建的工作者线程数量很快就回达到机器的CPU数，创建那么多线程后，线程池回监视工作项的完成速度。
              * 如果工作项完成的时间太长线程池会创建更多的工作者线程。如果工作项的完成速度开始变快，工作者线程会被销毁。
              * 
              */
        }
    }
}
