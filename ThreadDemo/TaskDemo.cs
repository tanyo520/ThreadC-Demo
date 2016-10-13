using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
   public class TaskDemo
    {
        /*
           ThreadPool的QueueUserWorkItem方法发起一次异步的计算限制操作。但这技术有太多限制。
           最大问题：
            没有内建的机制。让你知道这个操作什么时候完成。
            也没有机制在操作完成后可以有返回值。
            因此，引入Task的概念。

          System.Threading.Tasks

         */

        public void InvokeDemo()
        {
            //貌似公用线程池 可以查阅下源码。
            ThreadPool.QueueUserWorkItem((s) => { Thread.Sleep(1000); Console.WriteLine("QueueUserWorkItem:{0} ThreadId:{1}", s, System.Threading.Thread.CurrentThread.ManagedThreadId); }, 11);
            new Task((s)=> Console.WriteLine("new Task {0} ThreadId:{1}",s, System.Threading.Thread.CurrentThread.ManagedThreadId),11).Start();
            Task.Run(() => Console.WriteLine("Task.Run {0} ThreadId:{1}", 11, System.Threading.Thread.CurrentThread.ManagedThreadId));

            Console.ReadKey();

        }

        /// <summary>
        /// task取消
        /// </summary>
        public void InvokeDemo1() {
            //TaskCreationOptions.AttachedToParent 该提议总是被采纳，将一个task和他的父task关联
            //TaskCreationOptions.DenyChildAttach 该提议总是被采纳,如果一个任务视图和这个父任务关联,他就是一个普通任务，而不是子任务
            //TaskCreationOptions.HideScheduler 该提议总是被采纳,强迫子任务采用默认调度器而不是父任务的调度器
            //TaskCreationOptions.LongRunning 提议TaskScheduler应该尽可能的创建线程池线程
            //TaskCreationOptions.PreferFairness 提议TaskScheduler你希望该任务尽快执行
            //TaskCreationOptions.None //默认
            /*
              有标准提议的 在调度的时候，可能会采纳,可能不会，总是采纳的，总会采纳，因为他们和TaskSchduler本身无关  
           */

            //等待任务并获取结果
            //Task<Int32> task = new Task<Int32>(()=> { Console.WriteLine("1");int sum = 0; for (int i = 0; i < 99999999999999999; i--) { Console.WriteLine(sum); sum += i; } return sum; });
            //task.Start();

            //task.Wait();
            //try {
            //    Console.WriteLine(task.Result);
            //} catch (AggregateException e) {
            //    Console.WriteLine(e);
            //}

            //取消任务
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<Int32> task = Task.Run<Int32>(()=> Sum(cts.Token, 99999999),cts.Token);
            cts.Cancel();
            try
            {
                //task.Wait();
                Console.WriteLine(task.Result);
            }
            catch (AggregateException e) {
                e.Handle(s => s is OperationCanceledException);
                Console.WriteLine("aaaaaaaaaaaaaaaa");
                Console.WriteLine(e);
            } 
            
          

        }

        /// <summary>
        /// task不阻塞执行，返回结果
        /// </summary>
        public void InvokeDemo2()
        {
            /*伸缩性好的应用不应该出现线程阻塞，调用wait，或者任务尚未完成时的Result属性。极有可能造成线程池创建新线程。
             * 这增大了资源的消耗。也不利于性能和伸缩性。
             * 幸好，有更好的办法知道任务在什么时候结束运行。任务完成时可启动另一个任务。
             * 重写demo1的代码。
             * 
             * 如果一个任务未启动就取消他会爆InvalidOperationExcepttion
             * 调用task静态Run方法会立即调用start
             * Restul属性内部会调用Wait
             * TaskContinuationOptions类型定义:
             *      TaskContinuationOptions.None|| 默认
                    TaskContinuationOptions.PreferFairness|| 提议TaskScheduler你希望该任务尽快执行
                    TaskContinuationOptions.LongRunning ||议TaskScheduler应该尽可能的创建线程池线程
                    TaskContinuationOptions.AttachedToParent ||该提议总是被采纳，将一个task和他的父task关联
                    TaskContinuationOptions.DenyChildAttach ||任务试图和这个父任务连接将抛出InvalidOperationException
                    TaskContinuationOptions.HideScheduler ||强迫子任务采用默认调度器而不是父任务的调度器
                    TaskContinuationOptions.LazyCancellation ||除非前置任务(antecedent task)完成，否则禁止延续任务完成(取消)
                    TaskContinuationOptions.ExecuteSynchronously || 这个标志指出你希望由执行第一个任务的线程执行。 ContinueWith任务第一个任务完成后调用。ContinueWith的线程接着执行ContinueWith的任务,是指同步执行。在同一个线程中，按先后顺序执行，就是同步。
                    这些标志指出在什么情况下运行ContinueWith任务
                    TaskContinuationOptions.NotOnRanToCompletion ||
                    TaskContinuationOptions.NotOnFaulted ||
                    TaskContinuationOptions.NotOnCanceled ||
                    这些标志是以上3个标志的便利组合
                    TaskContinuationOptions.OnlyOnCanceled|| 第一个任务取消时执行
                    TaskContinuationOptions.OnlyOnFaulted || 第一个任务抛出未处理异常时执行
                    TaskContinuationOptions.OnlyOnRanToCompletion 第一个任务顺利完成时执行
             * 
            */

            //Task<Int32> task=new Task<Int32>(()=> { Thread.Sleep(1000); Console.WriteLine("threadId:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId); return 11; });
            //task.Start();
            //task.ContinueWith(t =>Console.WriteLine("threadId:{0},result:{1}",System.Threading.Thread.CurrentThread.ManagedThreadId,t.Result));
            Task<Int32> task = new Task<Int32>(() => { Thread.Sleep(1000); Console.WriteLine("threadId:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId); return 11; });
            task.ContinueWith(t => Console.WriteLine("threadId:{0},result:{1}", System.Threading.Thread.CurrentThread.ManagedThreadId, t.Result),TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => Console.WriteLine("threadId:{0},result:{1}", System.Threading.Thread.CurrentThread.ManagedThreadId, t.Result), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => Console.WriteLine("threadId:{0},result:{1}", System.Threading.Thread.CurrentThread.ManagedThreadId, t.Result), TaskContinuationOptions.OnlyOnCanceled);
        }

        /// <summary>
        /// task可以启动子任务
        /// </summary>
        public void InvokeDemo3() {
            Task<Int32[]> tasks = new Task<Int32[]>(()=> {
                var result = new Int32[3];
                new Task(() => result[0] = 0, TaskCreationOptions.AttachedToParent).Start();
                new Task(() => result[1] = 111, TaskCreationOptions.AttachedToParent).Start();
                new Task(() => result[2] = 222, TaskCreationOptions.AttachedToParent).Start();
                return result;
            });
            tasks.ContinueWith(ps => Array.ForEach(ps.Result, Console.WriteLine),TaskContinuationOptions.AttachedToParent);
            tasks.Start();
        }
        /// <summary>
        /// 任务内部揭秘
        /// </summary>
        public void InvokeDemo4()
        {
            /*
                每个任务对象都有一组字段，这些字段构成了任务的状态。其中包括一个Int32 I的（Task的只读属性ID）.代表task执行状态的
                一个Int32.对父任务的引用。对Task创建时指定的TaskScheduler的引用。对回调方法的引用。对要传给回调方法的对象的引用（可通过
                task的只读AsyncState属性查询。），对ExecutionContext的引用以及对ManualResetEventSlim对象的引用。另外
                每个task对象都有根据需要创建状态的引用。补充状态包含一个CancellationToken，一个ContinueWithTask对象集合。为
                抛出未处理异常的子任务准备的一个task对象集合等。任务很有用，但并不是没有代价，必须为这些状态分配内存。如果不需任务的附加
                功能，那么使用ThreadPool.QueueUserWorkItem能更好的资源利用。

               Task和Task<TResult>实现了IDisposable接口，允许任务在用完Task对象后调用Dispose，如今Dispose方法所做的都是关闭ManualResetEventSlim对象。
               但可以定义从Task和Task<TResult>派生的类。在这些类中分配他们自己的资源。并从写Dispose方法,释放这些资源。建议不要再代码中为Task
               对象显式调用那个Dispose，相反应该让垃圾回收器自己清理任何不需要的资源。

               在一个Task对象存在期间,可查询Task的只读Status属性了解它再其生存期的什么位置，该属性返回一个TaskStatus值
                TaskStatus.Created| //任务已经显式创建,可以手动Start
                //任务最终的状态以下三个
                TaskStatus.Canceled| //任务被取消
                TaskStatus.Faulted| //任务报错
                TaskStatus.RanToCompletion|// //任务顺利完成

                TaskStatus.Running|//任务正在运行
                TaskStatus.WaitingForActivation| //任务已经隐式创建,会自动开始 ContinueWith,ContinueWhenAl..和TaskCompletionSource<TResult>对象来创建的Task都处于当前状态。
                 。
                TaskStatus.WaitingForChildrenToComplete|//任务正在等待他的子任务完成，子任务完成后他才完成。
                TaskStatus.WaitingToRun|//任务已经调度，但是还未运行
             */



        }
        /// <summary>
        /// 任务工厂
        /// </summary>
        public void InvokeDemo5() {
            /*
               有时候需要创建一组共享相同配置的Task对象，为了避免机械的将相同的参数传递给每个Task的构造器。可创建一个任务工厂
               来封装通用配置。
               TaskFactory。和TaskFactory<TResult> 平级关系
             
             */

            Task<Int32[]> parent = new Task<int[]>(() => {
                Int32[] results = new Int32[3];
                CancellationTokenSource cts = new CancellationTokenSource();
                TaskFactory<Int32> taskFactory = new TaskFactory<int>(cts.Token, TaskCreationOptions.AttachedToParent, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                var childTasks = new[] { taskFactory.StartNew(() => results[0] = 111),
                taskFactory.StartNew(() => results[1] = 222),
                taskFactory.StartNew(() => results[2] = 333) };

                for (int i = 0; i < childTasks.Length; i++) {
                    childTasks[i].ContinueWith(t => cts.Cancel(), TaskContinuationOptions.OnlyOnFaulted);
                }
                taskFactory.ContinueWhenAll(childTasks, comTask => comTask.Where(t => !t.IsFaulted && !t.IsCanceled).Max(t => t.Result), CancellationToken.None).
                ContinueWith(t => Console.WriteLine("the max is" + t.Result), TaskContinuationOptions.ExecuteSynchronously);
                return results;
               
            });

            parent.ContinueWith(p =>
            {
                StringBuilder buf = new StringBuilder("the follwing exception(s) occurred:"+Environment.NewLine);
                foreach (var e in p.Exception.Flatten().InnerExceptions) {
                    buf.AppendLine(" " + e.GetType().ToString());
                }
                Console.WriteLine(buf.ToString());
            },TaskContinuationOptions.OnlyOnFaulted);
            parent.Start();
        }
        /// <summary>
        /// 任务调度器 TaskScheduler
        /// </summary>
        public void InvokeDemo6()
        {
            /*
             FCL提供了2个调度器类型
             1.线程池任务调度器(thread poll task scheduler)
             2.同步上下文任务调度器(synchronization context task scheduler)
             在默认情况下应用程序都是使用的线程池任务调度器TaskScheduler.Default
             同步上下文任务调度器适合提供了图形界面的应用程序。winform,wpf,silerlight,windows store。
             他将所有任务都调度给应用程序的GUI线程,所有任务代码都能成功更新UI组件(按钮，菜单等)，该调度器不能使用线程池。
             可执行TaskSheduler的静态FormCurrentSynchronizationContext方法来获取对同步上下文任务调度器的引用。

            Prarallel Extensions Extras提供大量任务有关的代码包括多个调度器源码:http://code.msdn.microsoft.com/ParExtSamples
             IOTaskScheduler 将任务排队给线程池的I/O线程而不是工作者线程。
             LimitedConcurrencyLeveITaskSchedule 任务调度器不允许超过n(一个构造器参数)个任务同时运行。
             OrderedTaskScheduler只允许一个任务运行，继承自LimitedConcurrencyLeveITaskSchedule 构造器函数N为1.
             PrioritizingTaskScheduler 将任务送入CLR线程池队列，之后，可调用Prioritize指出一个Task应该再所有普通任务之前
             处理(如果他还没有处理的话)。可以调用Deprioritize使一个Task再所有普通任务之后处理。
             ThreadPerTaskScheduler 每个任务创建并启动一个单独的线程，完全不使用线程池。
             */

        }
        public  Int32 Sum(CancellationToken token, Int32 num) {
            var j = 0;
            for (var i = 0; i < num; i++) {
                token.ThrowIfCancellationRequested();
                j += i;
            }
            return j;
        }
    }
}
