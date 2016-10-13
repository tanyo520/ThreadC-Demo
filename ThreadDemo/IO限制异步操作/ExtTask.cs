using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aa
{
   public static class ExtTask
    {
        private struct Void { };
        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> oroginalTask, CancellationToken ct)
        {
            var canTask = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), canTask))
            {
                Task any = await Task.WhenAny(oroginalTask, canTask.Task);
                if (any == canTask.Task) ct.ThrowIfCancellationRequested();
                return await oroginalTask;
            }
        }
        public static async Task<TResult> WithCancellation<TResult>(this Task oroginalTask, CancellationToken ct)
        {
            var canTask = new TaskCompletionSource<Void>();
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), canTask))
            {
                Task any =await Task.WhenAny(oroginalTask, canTask.Task);
                if (any == canTask.Task) ct.ThrowIfCancellationRequested();

                return await (Task<TResult>)oroginalTask;
            }
        }
       
    }
}
