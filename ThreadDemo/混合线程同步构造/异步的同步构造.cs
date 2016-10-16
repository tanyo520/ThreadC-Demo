using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    class 异步的同步构造
    {
        public static void Demo() {
            /*
             任何使用了内核模式的线程同步构造，我都不喜欢，因为所有这些基元都会阻塞一个线程的运行。创建线程的代价很大，
             创建了不用，
             例如：假定客户端向网站发起请求，客户端请求到达时，一个线程池线程开始处理客户端请求。假定这个客户端想
             以线程安全的方式修改服务器中的数据，所以它请求一个reader-writer锁来进行写入（这使线程成为一个writer线程）
             假定这个锁长时间占有，再锁占用期间，另一个客户端请求到达。所以线程池为这个请求创建一个新的线程。随着请求
             越来越多。线程池创建的线程也会越来越多。所有这些线程都傻傻的再锁上面阻塞。服务器把它的所有时间都发在了线程
             的创建上，而目的仅仅是让它们停止运行。这样的服务器完全没有伸缩性可言。更糟糕的是当writer线程锁被释放时，
             所有的reader线程都会解除阻塞并开始执行。现在又变成了大量线程试图再相对数量很少的cpu上运行。所以，windows
             开始再线程之间不停的进行上下文切换，上下文切换产生了大量开销。所以真正工作的反正没有得到很快的处理。

            可以用task类来完成，拿Barrier类来说，可以生成几个task对象来处理一个阶段。然后，当所有这些任务完成后。可以用
            另外的一个或多个任务对象继续。
            任务优势。
            1.任务使用的内存比线程芍的多，创建和销毁所需的时间也少很多。
            2.线程池根据可用CPU数量自动伸缩任务规模
            3.每个任务完成一个阶段后，运行的任务的线程回到线程池，在那里能接收新任务
            4.线程池是站在整个进程高度观察任务。所以它能更好的调度这些任务。减少进程中的线程数，并减少上下文切换。

            锁很流行，但长时间拥有会造成巨大的伸缩性问题。如果代码能通过异步的同步构造指出它想要个锁，那么非常有用。
            这种情况下，如果线程得不到锁。可直接返回并执行其他工作。而不必再那傻傻的阻塞。以后锁可用时，代码可恢复
            执行并访问锁所保护的资源。

            SemaphoreSlim类通过WaitAsync方法实现这个思路。
            Task<Boolean> WaitAsync(Int32 millisecondsTimeOut,CancellationToken cancellationToken);
             */

           
        }
        /// <summary>
        /// SemaphoreSlim WaitAsync
        /// </summary>
        /// <param name="semaohoreSlim"></param>
        /// <returns></returns>
        public static async Task AccessResourceViaAsyncSynchronization(SemaphoreSlim semaohoreSlim) {
            await semaohoreSlim.WaitAsync();//请求获取锁来获得对资源的独占访问
            //执行到这里表面没有别的线程正在方法资源
            //独占访问资源
            semaohoreSlim.Release();//释放锁，使其他线程能访问资源

            /*
             WaitAsync提供的是信号量语义，一般创建最大计数为1的SemaphoreSlim。从而对SemaphoreSlim保护的资源进行互斥访问
             所以这使得和Monitor时的行为相似，只是SemaphoreSlim不支持线程所有权和递归语义（这是好事）。

             */
        }
        /// <summary>
        /// Reder-Writer。 ConcurrentExclusiveSchedulerPair
        /// </summary>
        public static void ConcurrentExclusiveSchedulerDemo() {
            /*
               public TaskScheduler ConcurrentScheduler { get; } //同时运行（一次运行多个）
               public TaskScheduler ExclusiveScheduler { get; }//任何任务将独占式的运行，一次只能运行一个。
              */
            var ceps = new ConcurrentExclusiveSchedulerPair();
            TaskScheduler concurrent = ceps.ConcurrentScheduler;
            TaskScheduler exclusive = ceps.ExclusiveScheduler;
            var tConcurrent = new TaskFactory(concurrent);
            var tExclusive = new TaskFactory(exclusive);
            for (int i = 0; i < 5; i++) {
                var exclusiveB = i < 2;
                (exclusiveB ? tExclusive : tConcurrent).StartNew(() =>
                {
                    Console.WriteLine("{0} access", exclusiveB ? "exclusive":"concurrent");
                    //进行独占写和并发读操作。
                 });
            }
            /*
             遗憾的是.net 没有Reader-Writer语义的异步锁。可以自己构建个 参考AsyncOneManyLock
             */
        }
    }
}
