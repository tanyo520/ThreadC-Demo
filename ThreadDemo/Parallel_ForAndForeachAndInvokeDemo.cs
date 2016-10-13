using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo
{
   public class Parallel_ForAndForeachAndInvokeDemo
    {
        private readonly object InterLocked;

        /// <summary>
        /// paralleDemo
        /// </summary>
        public void Demo()
        {
            /*
              静态System.Threading.Tasks.Parallel类封装了这些情形，他内部使用Task对象

             */
            //for (int i = 0; i < 1000; i++) {
            //    Console.WriteLine(i);
            //}
            Parallel.For(0, 1000, (i) =>
            {
                Console.WriteLine("index:{0},ThreadId:{1}", i, System.Threading.Thread.CurrentThread.ManagedThreadId);
            });

            //List<string> items = new List<string>() { "tanyo", "tanyo1", "tanyo2" };
            //Parallel.ForEach(items, (item) =>
            //{
            //    Console.WriteLine("Thread:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //    Console.WriteLine(item);
            //});

            //如果可以用for或者foreach 推荐用for 速度快。

            //如果要执行多个方法，并行执行，可以使用Invoke。
            //Parallel.Invoke(() =>
            //{
            //    Console.WriteLine("Thread:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //}, () =>
            //{
            //    Console.WriteLine("Thread:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //}, () =>
            //{
            //    Console.WriteLine("Thread:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //});
            //****************
            //var options = new ParallelOptions();
            //options.CancellationToken;//允许取消操作 默认为none
            //options.MaxDegreeOfParallelism//允许可以指定并发操作的最大工作项数目 默认为-1 可用cpu数
            // options.TaskScheduler //默认为Default.

            /*
              for和foreach允许传递3个委托
              1.任务局部初始化委托(localInit) ，为参与工作的每个任务都调用一次该委托，这个委托是再任务被要求处理一个
              工作项之前调用。
              2.主体委托(body)为参与工作的各个线程所处理的每一项都调用一次该委托。
              3.任务局部终结委托(localFinally) 为参与工作的每个任务都调用一次该委托,这个委托是再任务处理好派发给他的所有工作项之后调用。
              即使主题委托代码引发异常，也会调用。
             */

            var s=  DirectoryBytes("I:\\Code\\ThreadDemo", "*", SearchOption.AllDirectories);
            Console.WriteLine(s);


        }

        public Int64 DirectoryBytes(string path, string seachPatten, SearchOption searchOption) {
            var files = Directory.EnumerateFiles(path,seachPatten);
            Int64 masterTotal = 0;
            Parallel.ForEach<String, Int64>(files, () =>
            {
                return 0;
            }, (file,loopState,index,taskLocalTotal) =>
            {
                Int64 fileLeth = 0;
                FileStream fs = null;
                try
                {
                    fs = File.OpenRead(file);
                    fileLeth = fs.Length;
                }
                catch (Exception e)
                {
                }
                finally {
                    if (fs != null)
                        fs.Dispose();
                }
                return taskLocalTotal += fileLeth;
            }, taskLocalTotal =>
            {
                System.Threading.Interlocked.Add(ref masterTotal, taskLocalTotal);
            });
            return masterTotal;
        }
    }
}
