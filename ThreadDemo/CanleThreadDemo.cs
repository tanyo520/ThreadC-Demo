using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
    public class CanleThreadDemo
    {
        /*
           协作式取消和超时
           MNF提供了标准的取消操作模式。这个模式是协作式
           意味着操作必须显示支持取消。无论执行操作的代码，还是试图取消操作的代码。都需要使用本次提到的类型。
           对于长时间运行的任务支持取消是一件很棒的事
           取消操作首先要创建一个
           System.Threading.CancellationTokenSoure对象。

         这个对象包含了管理取消有关的所有状态，构造好一个CancellationTokenSource（一个引用类型）之后。可从他的Token属性
         获得一个或多个CancellationToken（一个值类型）实例。并传给你的操作。使操作可以取消
         CancellationToken实例是轻量级值类型。包含单个私有字段。对其CancellationTokenSource对象的引用。
         在计算限制操作的循环中。可定时调用CancellationToken的IsCancellationRequested的属性。了解循环是否应该提前终止。
         从而终止计算限制的操作。提前终止的好处在于，CPU不需要把时间浪费在你不感兴趣的结果上。
         */

        public void InvokeDemo() {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.WriteLine("main Thread {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
           CancellationTokenRegistration t=  cts.Token.Register(() => Console.WriteLine("call 1:{0}",System.Threading.Thread.CurrentThread.ManagedThreadId),true);
            cts.Token.Register(() => Console.WriteLine("call 2:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId), false);
            t.Dispose();
            ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 1000));
            Console.WriteLine("press Endter to Cancel the opertion");
            Console.ReadKey();
            cts.Cancel();
            Console.ReadLine();
        }

        //链接
        public void InvokeDemo1()
        {
            var cts = new CancellationTokenSource();
            cts.Token.Register(() => Console.WriteLine("cts"));

            var cts1 = new CancellationTokenSource();
            cts1.Token.Register(() => Console.WriteLine("cts1"));

            var linkCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cts1.Token);
            linkCts.Token.Register(() => Console.WriteLine("linkcts"));
            cts1.Cancel();
            Console.WriteLine("ctsted:{0} |  cts1ted:{1} |  linkCts:{2}", cts.IsCancellationRequested, cts1.IsCancellationRequested, linkCts.IsCancellationRequested);
            Console.ReadKey();


        }
        private  void Count(CancellationToken token, Int32 countTO) {
            for (Int32 count = 0; count < countTO; count++) {
                if (token.IsCancellationRequested) {
                    Console.WriteLine("is Cancelled");
                    break;
                }
                Console.WriteLine("count is {0}", count);
                Thread.Sleep(200);
            }
            Console.WriteLine("count is done");
           
        }

        /*
          注意 要执行一个不允许被取消的操作，可以向该操作传递通过调用CancellationToken的静态None属性而返回的CancellationToken，
          该属性返回一个特殊的Cancellation实例。他不喝任何CancellatonTokenSource对象关联。（实例的私有字段为null）
        由于没有CancellationTokenSource 实例所以没法调用Cancel 一个操作查询IsCancellationRequested属性。总是返回false.
         */

        /*
           很多情况下我们取消操作立即取消会有问题，可能要在多久后取消。 CancellationTokenSource为我们提供了api
           CancelAfter()传递时间                            
         */
    }
}
