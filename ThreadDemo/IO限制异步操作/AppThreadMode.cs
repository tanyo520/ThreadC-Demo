using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo.IO限制异步操作
{
    /// <summary>
    /// 应用程序线程处理模型
    /// </summary>
   public class AppThreadMode
    {
        public static void Demo() {
            /*
               线程上下文再不通框架中不通 比如wpf winform 上下文线程是gui线程 后台处理以后要回到gui线程。
               再.net程序中上下文中获取到语言文化和身份标识需要流向新的线程。这样一来代表客户端执行的任何额外的工作才能使用客户端的语言文化和身份标识信息。
               FCL定义了一个名为System.Threading.SynchronizationContext的基类。他解决了这些问题。
               简单的来说SynchronizationContext派生对象将应用程序模型连接到他的线程处理模型。这些派生类不直接和你打交道。也没有公开或记录文档。

            开发人员通常不需要了解关于SynchronizationContext类的任何事情。等待一个task时回获取调用线程的SynchronizationContext对象。线程池线程完成task后会使用该
            SynchronizationContext对象。确保应用程序模型使用正确的线程处理模型。所以gui等待一个task时，await操作符后面的代码保证再gui线程上执行。
            对于.net应用程序 await后面的代码保证关联了客户端语言文化和身份标识信息的线程池线程上执行。

            让状态机使用应用程序模型的线程处理模型来恢复。这再大多数时候都很有用。也很方便。但偶尔回带来一些问题。死锁比如wpf下面代码
            private sealed class MyWpf:Window{
                 void Onactivated(EventArgs e){
                   String http= GetHttp().Result();
                }
                async Task<String> GetHttp(){
                   //发出Http请求，让线程从GetHttp返回
                     HttpResponseMessage msg=await new HttpClient().GetAsync("http://www.baidu.com");
                    //这里永远执行不到。Gui线程再等待这个方法结束 
                    //但这个方法结束不了,因为Gui线程一直再等待他的结束。死锁
                    return await msg.Content.ReadAsStringAsync();
               }
             }
             */
            /*
              为了解决上面问题可以有如下2种方案
             1. Task<TResult> 提供了ConfigureAwait方法。
              向方法传递true相当于没有调用方法。但传递false，await操作符就不查询调用线程的SynchronizationContext对象。当前线程池结束Task时回直接完成他
              await操作符后的代码通过线程池线程执行。 由于不知道哪个task操作忽略SynchronizationContext 所以都忽略
               //发出Http请求，让线程从GetHttp返回
                    HttpResponseMessage msg=await new HttpClient().GetAsync("http://www.baidu.com").ConfigureAwait(false);
                   //这里永远执行不到。Gui线程再等待这个方法结束 
                   //但这个方法结束不了,因为Gui线程一直再等待他的结束。死锁
                   return await msg.Content.ReadAsStringAsync().ConfigureAwait(false);

            2.用一个线程池线程操作所有操作 这个版本中不再是异步函数。方法签名中删除了async.但是传递给task.run的lambda表达式是异步函数。
            Task<String> GetHttp(){
              Taks.Run(async ()=>{
                 HttpResponseMessage msg=await new HttpClient().GetAsync("http://www.baidu.com");
                   //这里永远执行不到。Gui线程再等待这个方法结束 
                   //但这个方法结束不了,因为Gui线程一直再等待他的结束。死锁
                   return await msg.Content.ReadAsStringAsync();
              })
            }
            */
        }
    }
}
