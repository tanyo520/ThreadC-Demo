using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    class 条件变量模式
    {

        public static void Demo() {
            /*
              假定一个线程希望在一个复合条件为true时执行一些代码。一个选项是让线程连续自旋。反复测试条件。
              但这会浪费cpu时间，也不可能造成复合条件的多个变量进行原子性的测试，有个模式允许线程根据一个复合条件来同步
              它们的操作。而不会浪费资源。这个模式称为便利模式。

             */
        }
    }
    class ConditionVariablePattern {
        private readonly Object m_lock = new object();
        private Boolean m_condition = false;
        public void Thread1() {
            Monitor.Enter(m_lock); //获取一个互斥锁
            while (!m_condition) {
                //条件不满足时，就等待另一个线程更改条件
                 Monitor.Wait(m_lock);//临时释放锁，其他线程获得锁
            }
            Monitor.Exit(m_lock);
        }

        public void Thread2() {
            Monitor.Enter(m_lock);
            m_condition = true;
            //Monitor.Pulse(m_lock);//释放锁之后唤醒一个正在等待的线程
            Monitor.PulseAll(m_lock);//释放锁之后唤醒所有正在等待的线程。
            Monitor.Exit(m_lock);
            /*
              可以使用简单的同步逻辑（只是一个锁）来测试构成一个复合条件的几个变量，而且多一个正在等待的线程可以完全解除
              阻塞，而不会造成任何逻辑错误，唯一的缺点就是解除线程的阻塞可能浪费一些cpu时间。
             */
        }
    }

    class SynchronizedQueue<T> {
        private readonly Object m_lock = new object();
        private readonly Queue<T> m_queue = new Queue<T>();
        public void Enqueue(T item)
        {
            Monitor.Enter(m_lock);
            m_queue.Enqueue(item);
            Monitor.PulseAll(m_lock);
            Monitor.Exit(m_lock);
        }
        public T Dequeue() {
            Monitor.Enter(m_lock);
            while (m_queue.Count == 0) {
                Monitor.Wait(m_lock);
            }
            T item = m_queue.Dequeue();
            Monitor.Exit(m_lock);
            Console.Write(item.ToString());
            return item;
        }
    }
}
