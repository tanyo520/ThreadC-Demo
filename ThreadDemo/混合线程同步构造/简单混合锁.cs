using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
  public  class 简单混合锁
    {
        public static void Demo() {
            SimpleHybridLock simple = new SimpleHybridLock();
        }
    }

    class SimpleHybridLock : IDisposable {
        private Int32 m_waiters = 0;
        private AutoResetEvent m_waiterLock = new AutoResetEvent(false);

        public void Enter()
        {
            if (Interlocked.Increment(ref m_waiters) == 1)
            {
                return;
            }
            m_waiterLock.WaitOne();
        }
        public void Leave() {
            if (Interlocked.Decrement(ref m_waiters) == 0) {
                return;
            }
            m_waiterLock.Set();
        }

        public void Dispose()
        {
            m_waiterLock.Dispose();
        }
    }
}
