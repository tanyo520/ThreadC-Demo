using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    /// <summary>
    /// FCL中混合构造
    /// </summary>
  public  class FclSlimDemocs
    {
        public static void Demo() {
            /*
              FCL自带很多混合构造，他们通过一些别致的逻辑将你的线程保持再用户模式，从而增强了应用程序的性能。
              有的混合构造知道首次有的线程再一个构造上发生竞争时，才会创建内核模式的构造。如果线程一直不在构造上发生竞争。
              应用程序就可以避免因为创建对象而产生的性能损失，同避免为对象分配内存，很多构造还支持使用一个CancellationToken参数。
              使线程强迫解除可能正在构造上等待的其他线程的阻塞。

            ManualResetEventSlim和SemaphoreSlim类
            这个两个构造的工作方式对应内核模式构造完全一致。只是它们都在用户模式中自旋。而且都推迟到发生第一次竞争时，才创建内核模式的构造。
            它们的Wait方法允许传递一个超时值，和CancellationToken.

             */
        }
    }
}
