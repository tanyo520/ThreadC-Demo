using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
   public class TimerDemo
    {
        private  Timer s_Timer;
        /// <summary>
        /// 执行定时计算限制操作
        /// </summary>
        public void Demo() {
            // new System.Threading.Timer() 4个重载构造。
            // period参数 指定以后每次调用回调方法之前要等待多少毫秒，如果为这个参数传递Timeout.Infinite(-1)，线程池只调用回调方法一次

            //在内部 timer只使用一个线程，这个线程知道下一个timer对象什么时候到期，计时器还有多久触发。下一个timer对象到期时，线程回唤醒。
            //再内部调用ThreadPool的QueueUserWorkItem，将一个工作项添加到线程池的队列中使你的回调方法得到调用。如果回调方法执行时间很长
            //计时器可能再上个回调还没处理完再次触发。这可能造成多个线程池线程同时调用回调方法。
            //为了解决这个问题，建议period参数指定Timeout.Infinite。这样计时器只触发一次，，然后，再你的回调方法中Change方法指定一个新的dutime。，并将period参数
            //继续指定Timeout.Infinite.

            //timer 类还提供一个Dispose方法，允许完全取消计时器，并可再当时处于pending状态的所有回调方法之后
            //向notifyObject参数标识内核对象发出信号
            /*
             *  timer对象被垃圾回收时，他的终结代码告诉线程池取消计时器，使他不再触发，所以使用timer对象时，要确定有一个变量
             *  再保持timer对象的存活，否则对你的回调方法的调用就会停止。
             */
             //创建但不启动计时器，确保s_timer在线程池调用status之前引用该计时器
            s_Timer = new Timer(Status, 11, Timeout.Infinite, Timeout.Infinite);
            //现在s_timer以及赋值，可以启动
            //现在再status调用change，保证不会抛出nullReferenceException
            s_Timer.Change(0, Timeout.Infinite);
            Console.ReadLine();
        }
        private void Status(object state) {
            Console.WriteLine("In Status at {0}", DateTime.Now);
            Thread.Sleep(1000);
            s_Timer.Change(2000, Timeout.Infinite);
            //这个方法返回后 ，线程回归池中，等待下一个工作项。
        }
        /// <summary>
        /// 利用Task.Delay和async和await关键字实现当时操作。
        /// </summary>
        public async void Demo1()
        {
            while (true) {
                Console.WriteLine("In  at {0}", DateTime.Now);
                Thread.Sleep(1000);
                Console.WriteLine("threadid:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                await Task.Delay(2000);
            }
        }
        /// <summary>
        /// 计时器太多，时间太少。
        /// </summary>
        public void Demo2() {
            /*
                1.System.Threading.Timer
                2.System.Windows.Form.Timer
                3.System.Windows.Threading.DispatcherTimer
                4.Windows.UI.Xaml.DispatcherTimer
                5.System.Timers.Timer.
            */
        }
    }
}
