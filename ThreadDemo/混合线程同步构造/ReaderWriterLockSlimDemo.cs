using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
   public class ReaderWriterLockSlimDemo
    {
        public static void Demo() {
            /*
             让一个线程简单的读取一些数据内容。如果这些数据被一个互斥锁保护。那么当那么多线程试图同时访问这些数据时候，只有一个线程才会运行。
             其他所有线程都会阻塞。。这样会造成应用程序伸缩性和吞吐能力的下降，如果所有线程都希望以只读方式访问数据。
             那么根本没必要阻塞它们。应该允许它们并发访问数据。另一方面。如果一个线程希望修改数据。这个线程就需要对数据独占的方式访问。
             ReaderWriterLockSlim封装了解决这个问题的逻辑。具体地说，这个构造像下面这样控制线程。

              1.一个线程向数据写入时，请求访问的其他所有线程都被阻塞。
              2.一个线程从数据读取时，请求读取的其他线程允许继续执行，但请求写入的线程仍被阻塞。
              3.向线程写入的线程结束后，要么解除一个写入线程的阻塞，使它能向叔写入要么解除所有读取线程的阻塞，使它们能
              并发读取数据，如果没有线程被阻塞，锁就可以自由使用状态，可供下一个Reader和Writer线程获取。
              4.从数据读取的所有线程结束后，一个Writer线程被解除阻塞，使它能写入数据，如果没有线程阻塞，锁就进入自由使用状态，
             可供下一个Reader和Writer线程获取。

             方法：
             EnterRadLock()
             TryEnterReadLock(Int32 millisecondsTimeOut)
             ExitReadLock()
             EnterWriterLock()
             TryEnterWriterLock(Int32 millisecondsTimeOut)
             ExitWriterLock()
             ReaderWriterLockSlim还提供了一些额外的方法，允许一个Reader线程升级为witer线程，以后，线程可以把自己降级回reader线程
             一个线程刚开始可能是读取数据，然后根据数据的内容线程可能想对数据进行修改，为此，线程要把它自己从reader升级为writer，锁如果支持这个行为，性能回大打折扣，
             而且我完全不觉得这是一个有用的功能。线程并不是直接从reader变成writer的，当时可能还有其他线程正在读取，这些线程必须完全退出锁，在此之后，
             尝试升级线程才允许成为writer。着相当于先让reader线程退出锁，再立即获取这个锁进行写入。
             FCL还提供了一个ReaderWriterLock
             在1.0的时候引入，这个构造存在很多问题。所以在3.5的时候引入ReaderWriterLockSlim
             问题点：
             1.即使不存在线程竞争，它的速度也非常慢。
             2.线程所有权和递归行为是这个构造强加的，完全取消不了。使锁变的非常慢。
             3.相比writer线程，它更看重于reader线程，所以writer线程可能排好长的队，却很少有机会获得服务。最终造成拒绝服务问题。

             */
        }
    }
    class Transaction : IDisposable
    {
        //LockRecursionPolicy.NoRecursion ,SupportsRecursion 支持线程所有权和递归行为。NoRecursion不支持
        /*
         reader-writer锁支持所有权和递归锁的代非常高昂，因为锁必须跟踪允许进入锁的所有Reader线程，同时为每个线程
         都单独维护递归计数，事实上，为了以线程安全的方式维护所有这些信息。它内部还要使用一个互斥的 自选锁。

       
             */
        private readonly ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private DateTime m_timeOflastTrans;
        public void PerformTranscation()
        {
            //对数据独占访问权
            m_lock.EnterWriteLock();
            m_timeOflastTrans = DateTime.Now;
            m_lock.ExitWriteLock();
        }

        public void Dispose()
        {
            m_lock.Dispose();
        }

        public DateTime LastTranscation{
            get {
                //拥有对数据的共享访问权
                m_lock.EnterReadLock();
                DateTime tmp = m_timeOflastTrans;
                m_lock.ExitWriteLock();
                return tmp;
            }
        }

    }
}
