using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.IO限制异步操作
{
    public sealed class EventAwaiter<TEventArgs> : INotifyCompletion
    {
        private ConcurrentQueue<TEventArgs> m_events = new ConcurrentQueue<TEventArgs>();
        private Action m_Continuation;
        public EventAwaiter<TEventArgs> GetAwaiter() {
            return this;
        }
        public bool IsCompleted { get { return this.m_events.Count > 0; } }
        public void OnCompleted(Action continuation)
        {
            Volatile.Write(ref m_Continuation, continuation);
        }
        public TEventArgs GetResult() {
            TEventArgs e;
            m_events.TryDequeue(out e);
            return e;
        }
        public void EventRaised(object sender, TEventArgs eventArgs) {
            m_events.Enqueue(eventArgs);
            Action configurn = Interlocked.Exchange(ref m_Continuation, null);
            if (configurn != null) {
                configurn();
            }
        }
      
    }
}
