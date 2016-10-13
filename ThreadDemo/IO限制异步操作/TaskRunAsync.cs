using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo.IO限制异步操作
{
    public static class Tex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoWaring(this Task task) { }
    }
    public  class TaskRunAsync
    {
     
        public static void Demo() {
            Console.WriteLine("mmmmmm，threadId:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            Task.Run(async () =>
            {
                Console.WriteLine("aaaaaaaa，threadId:{0}",System.Threading.Thread.CurrentThread.ManagedThreadId);
                HttpResponseMessage res =  await new HttpClient().GetAsync("http://www.baidu.com");
                Console.WriteLine("hhhhhhhhhhhhh，threadId:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                string s=  await res.Content.ReadAsStringAsync();
                Console.WriteLine("eeeeeeeee，threadId:{0},body:{1}", System.Threading.Thread.CurrentThread.ManagedThreadId, s);
            });
        }

       
        public static async Task OuerAsyncFunc() {
            //如果不赋值给一个变量 会提升要插入await,
            var a = InnerAsyncFunc();
            //当然还有其他的办法 [MethodImpl(MethodImplOptions.AggressiveInlining)] NoWaring方法
            InnerAsyncFunc().NoWaring();
        }

        public static async Task InnerAsyncFunc() {
            await new Task(() => { Console.Write("11"); });
        }
    }
}
