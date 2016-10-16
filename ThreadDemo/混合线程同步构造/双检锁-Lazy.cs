using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
   public class 双检锁_Lazy
    {

        public static void Demo() {
            /*
              FCL有两个封装了双检锁Demo模式。泛型System.Lazy类。
              Lazy(Func<T> valueFactory,LazyThreadSafetyMode mode)
              Boolean IsValueCreated{get;}
              T Value{get;}
             */
            Lazy<string> lazy = new Lazy<string>(() => DateTime.Now.ToLongTimeString(), true);
            Console.WriteLine(lazy.IsValueCreated);
            Console.WriteLine(lazy.Value);
            Console.WriteLine(lazy.IsValueCreated);
            Thread.Sleep(10000);
            Console.WriteLine(lazy.Value);//值还是一样
            /*
              LazyThreadSafetyMode.PublicationOnly 使用Interlocked.CompareExchange技术
              LazyThreadSafetyMode.None 完全没有线程安全的支持，（只适合GUI应用程序）
              LazyThreadSafetyMode.ExecutionAndPublication 使用双锁技术。
             */
        }

        public static void Demo1()
        {
            /*
             内存有限时（理论不必担心这个小对象）可能不像创建Lazy类的实例，这时可以调用System.Threading.LazyInitializer类的静态方法。
            //这两个方法内部使用Interlocked.CompareExchange技术
            public static T EnsureInitialized<T>(ref T target) 
            public static T EnsureInitialized<T>(ref T target, Func<T> valueFactory)
            //这两个方法内部使用双锁技术。
            public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock)
            public static T EnsureInitialized<T>(ref T target, ref bool initialized, ref object syncLock, Func<T> valueFactory)
            syncLock参数指定同步对象，可以用同一个锁保护对象保护多个初始化函数和字段。
             */
            String name = null;
            LazyInitializer.EnsureInitialized(ref name, () => "tanyo");
            Console.WriteLine(name);
            LazyInitializer.EnsureInitialized(ref name, () => "tianyong");
            Console.WriteLine(name);//还是tanyo
        }
    }
}
