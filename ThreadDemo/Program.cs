using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadDemo.IO限制异步操作;
using aa;
using System.Runtime.ExceptionServices;
using ThreadDemo.基元线程同步构造;

namespace ThreadDemo
{
    class Program
    {
        static  void Main(string[] args)
        {
            //ThreadPoolDemo demo = new ThreadPoolDemo();
            //demo.InworkDemo();

            //ThreadContext con = new ThreadContext();
            //con.InworkDemo();
            //CanleThreadDemo demo = new CanleThreadDemo();
            //demo.InvokeDemo1();

            //TaskDemo taskDemo = new TaskDemo();
            ////taskDemo.InvokeDemo();
            ////taskDemo.InvokeDemo1();
            ////taskDemo.InvokeDemo2();
            //taskDemo.InvokeDemo5();

            //Parallel_ForAndForeachAndInvokeDemo demo = new Parallel_ForAndForeachAndInvokeDemo();
            //demo.Demo();
            //LinqParallelDemo demo = new LinqParallelDemo();
            //demo.Demo();
            //Go();

            //TimerDemo demo = new TimerDemo();
            //demo.Demo1();
            //  Go1();
            //Getss();

            // Task<string> task= NamePipeDemo.AwaitWebClient(new Uri("http://www.google.com"));
            //try
            //{
            //    Console.WriteLine(task.Result);
            //}
            //catch (AggregateException e) {
            //    Console.WriteLine(task.Exception.InnerException);
            //}
            //TaskRunAsync.Demo();
            //NamePipeDemo.StarServer();
            //  NamePipeDemo.ClientServer();
            //NamePipeDemo1.Go();
            //取消IO操作.Go();
            //用户模式构造.Demo4();
            // 自旋锁.Demo2();
            //内核构造模式.Demo();
            内核构造模式.Demo6();
            Console.ReadKey();
        }
        public static async void Go()
        {
            TaskLogger.LogLevel = TaskLogger.TaskLoggerLevel.Pending;
            var tasks = new List<Task>() {
                 Task.Delay(2000).Log("2s"),
                 Task.Delay(5000).Log("5s"),
                 Task.Delay(6000).Log("6s")
            };
            try
            {
                await Task.WhenAll(tasks).WithCancellation<int>(new CancellationTokenSource(3000).Token);
            }

            catch (Exception e)
            {
            }

            foreach (var op in TaskLogger.GetLogEntries().OrderBy(tle => tle.LogTime))
            {
                Console.WriteLine(op);
            }


        }

        private static async void ShowEcetions()
        {
            var eventAwaiter = new EventAwaiter<FirstChanceExceptionEventArgs>();
            AppDomain.CurrentDomain.FirstChanceException += eventAwaiter.EventRaised;
            while (true)
            {
                Console.WriteLine("appdomin exception {0}", (await eventAwaiter).Exception.GetType());
            }
        }

        public static void Go1()
        {
            ShowEcetions();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    switch (i)
                    {
                        case 0: throw new InvalidOperationException();
                        case 1: throw new ObjectDisposedException("");
                        case 2: throw new ArgumentOutOfRangeException();
                    }
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// 没有返回值的await
        /// </summary>
        public static async void Getss() {
             await Task.Delay(3000);
        }
    }
}
