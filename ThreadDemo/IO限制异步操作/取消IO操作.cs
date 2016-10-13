using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.IO限制异步操作
{
  public  static  class 取消IO操作
    {
        private struct Void { }
        private static async Task<TResult> WithCancellation<TResult>(this Task<TResult> originalTask, CancellationToken ct) {
            var cancelTask = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), cancelTask))
            {
                Task any = await Task.WhenAny(originalTask, cancelTask.Task);
                if (any == cancelTask.Task)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
            return await originalTask;
        }
        public static async Task WithCancellation(this Task oroginalTask, CancellationToken ct)
        {
            var canTask = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), canTask))
            {
                Task any = await Task.WhenAny(oroginalTask, canTask.Task);
                if (any == canTask.Task) ct.ThrowIfCancellationRequested();
                 await oroginalTask;
            }
        }
        public static async Task Go() {
            var cts = new CancellationTokenSource(5000);//再#毫秒后取消。 //更快取消需调用Cancel();
            var ct = cts.Token;
            try
            {
                await Task.Delay(10000).WithCancellation(ct);
                Console.WriteLine("task completed");
            }
            catch (OperationCanceledException e) {
                Console.WriteLine("task Cancelled");
            }
        }
    }
}
