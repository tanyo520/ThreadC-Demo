using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
 public   class ThreadContext
    {
        /*
           执行上下文
            

           每个线程都关联了一个执行上下文数据结构
           包括安全设置（压缩栈，Thread的principal属性和windows身份）
           宿主设置（参见System.Threading.HostExecutionContextManager）
           逻辑调用上下文数据(system.Runtime.Remoting.Messageing.CallContext)的LogicalSetData和LogicalGetData
           线程执行代码时，一些操作会受到线程执行上下文设置(尤其安全设置)的影响
           每个线程(初始化线程)使用另一个线程(辅助线程)执行任务时，前者执行上下文应该流向(复制到)辅助线程。这就确保辅助线程在执行任何操作使用的是相同的安全设置
           和宿主设置。还确保了在初始线程的逻辑调用上下文中存储的数据适用于辅助线程

         默认情况下,clr自动造成初始化线程的执行上下文流向任何辅助线程。这造成上下文信息传递给辅助线程。
         但这会造成性能问题。这是因为执行上下文包含大量信息而收集所有的信息，再复制给辅助线程，要耗费不少时间。如果辅助线程又采用了更多辅助线程
         还必须创建和初始化更多的执行上下文数据结构


         System.Threading命名空间有一个ExecutionContext类，他允许你控制线程的执行上下文如果从一个线程流向另一个。
         对于服务器应用程序 性能的提升非常显著。但客户端应用程序性能提升不了读书，由于SupperssFlow方法用[SecuiryCritical]特性进行标识。所以
         某些客户端(silverlight)应用程序是无法调用的.
         当然只有在辅助线程不需要访问上下文信息时，才应该阻止傻瓜下文的流动
         如果初始线程的执行上下文不流向辅助线程。辅助线程会使用上一次和他关联的任意上下文。
         在这种情况下不应该执行任何以依赖于执行上下文状态(比如用户的windows身份)的代码

         
         */


        public void InworkDemo() {
            CallContext.LogicalSetData("Name", "tanyo");
            //线程池访问逻辑调用上下文
            ThreadPool.QueueUserWorkItem(state => Console.WriteLine("name:{0}", CallContext.LogicalGetData("Name")));
            //阻止main线程执行上下文的流动
            ExecutionContext.SuppressFlow();
            //初始化由线程池工作
            //线程池线程不能访问逻辑调用上下文数据
            ThreadPool.QueueUserWorkItem(state => Console.WriteLine("name:{0}", CallContext.LogicalGetData("Name")));
            //恢复main线程的执行上下文的流动
            //以免将来使用更多的线程池线程。
            ExecutionContext.RestoreFlow();

            Console.ReadLine();

        }
        /*
         
           虽然我们讨论了是在调用线程池时组织上下文的流动。但使用task对象以及发起异步IO操作时，这个技术同样有用。 

          添加到逻辑上下文的 项必须是可序列化的。包含 了逻辑调用上下文数据项的执行上下文，让他流动起来可能严重损害性能。因为获取执行上下文
          需要对数据进行序列化和反序列化操作。
         */

    }
}
