using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    class CountdownEventDemo
    {
        public static void Demo() {
            /*
             System.Threading.CountdownEcent.这个构造使用了ManualResetEventSlim对象。这个造成阻塞的一个线程，知道它的neibu计算器
             变成0，从某种角度说这个构造的行为和Semaphore的行为相反，（Semaphore计数为0时阻塞线程。）

            Reset(Int32 count);将CurrentCount设置为count
            AddCount(Int32 signalCount);将CurrentCount递增signalCount
            TryAddCount(Int32 signalCount);将CurrentCount递增signalCount
            Siganl(Int32 siganlCount);将CurrentCount递减siganlCount
            Wait(Int32 millisecondsTimeout,CancellationToken cancellationToken);
            Int32 CurrentCount;
            Boolean IsSet; 如果CurrentCount为0 就返回true。
            一旦一个CountdownEvent的CurrentCount变成0，它就不能更改了，CurrentCount为0时，AddCount方法会抛出一个InalidOperationException
            如果CurrentCount为0，TryAddCount直接返回false;
             */
        }
    }
}
