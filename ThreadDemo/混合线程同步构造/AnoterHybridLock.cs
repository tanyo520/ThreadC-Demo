using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    /// <summary>
    /// 自旋，线程所有权，递归
    /// </summary>
   public class AnoterHybridLock
    {
        private Int32 m_waiters = 0;
        private AutoResetEvent m_waiterLock = new AutoResetEvent(false);
        private Int32 m_spincont = 4000;
        private Int32 m_owingThreadId = 0, m_recursion = 0;
        public void Enter() {
            Int32 threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            if (threadId == m_owingThreadId) { m_recursion++;return; }
            SpinWait wait = new SpinWait();
            for (var i = 0; i < m_spincont; i++) {
                if (Interlocked.CompareExchange(ref m_waiters, 1, 0) == 0) { goto GotLock; }
                //黑科技 给其他线程运行的机会，希望锁会被释放
                wait.SpinOnce();
            }
            //自旋结束 锁仍未获取到 再试一次。
            if (Interlocked.Increment(ref m_waiters) > 1) {
                m_waiterLock.WaitOne();
            }
            GotLock: m_recursion = 1; m_owingThreadId = threadId;
        }

        public void Leave() {
            Int32 thread = System.Threading.Thread.CurrentThread.ManagedThreadId;
            if (thread != m_owingThreadId) throw new SynchronizationLockException("lock not owned by calling thread");
            if (--m_recursion > 0) return;
            m_owingThreadId = 0;
            if (Interlocked.Decrement(ref m_waiters) == 0) {
                return;
            }
            m_waiterLock.Set();
        }
    }
}
