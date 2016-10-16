using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    class 并发集合类
    {
        public static void Demo() {
            /*
             FCL自带4个线程安全集合类，全部在System.Collections.Concurrent命名空间中定义。它们是
             ConcurrentQueue,以先入先出（FIFO）的顺序处理数据
             ConcurrentStack,以后入先出（LOFI）的顺序处理数据
             ConcurrentDictionary,一个无序key/value对集合
             ConcurrentBag 一个无序数据项集合，允许重复

            这些集合类都是非阻塞的，如果一个线程试图提取一个不存在的元素，线程会立即返回，线程不会在那里阻塞，等待着一个元素
            的出现，正是这个原因，所以如果获取了一个数据项，像TryDequeue，TryPop，TryTake和TryGetValue这样的方法全部返回
            true,否则返回false.
            一个集合非阻塞并不意味着他们不需要要锁了，ConcurrentDictionary内部使用了Monitor，但是对集合中项操作时，
            锁只被占极短时间，ConcurrentQueue和ConcurrentStack确实不需要锁，他们两个再内部使用Interlocked的方法来操作集合
            一个ConcurrentBag对象由大量迷你集合对象构成，每个线程一个，线程将一个项添加到bag中时，就用InterLocked方法将
            这个项添加到调用线程的迷你集合中，一个线程试图从Bag中提取一个元素时，bag就检查调用线程的迷你集合，试图从中取出
            数据项，如果数据项在那里，就用InerLocked方法提取，如果不在，就在内部获取一个Monitor，以便另一个线程的迷你集合提取
            这个项，称为一个线程从另一个线程中窃取一个数据项。

            注意所有并发集合类都提供了GetEnumerator方法，它一般用于C#foreach语句，也可以用于Linq,对于ConcurrentQueue,
           ConcurrentStack, ConcurrentBag集合
            GetEnumerator方法获取集合内容的一个快照，并从这个快照中返回元素，实际集合的内容可能使用这个快照枚举时
            发生改变，ConcurrentDictionary的GetEnumerator方法不获取它的内容的快照。因此在枚举字典期间，字典的内容可能改变
            这点务必注意。还要注意的是Count属性返回的式查询时集合中的元素数量。如其他线程正在增删元素，这个计数就不对了。
             */
        }

        public static void Demo1() {
            /*
             *  ConcurrentQueue, ConcurrentStack, ConcurrentBag 接口都实现了IProducerCOnsumerConllection<T>接口
             *  实现了这个接口的任何类都能转变成一个阻塞集合，如果集合已满，那么负责生产数据项的线程会阻塞，如果集合已空，那么负责消费的线程
             *  会阻塞，当然我们尽量不使用这种集合，将非阻塞集合转换成阻塞集合需要构造一个System.Collectons.Concurrent.BlockingCollection类
             *  向它传递非阻塞集合引用。
             *  
             *  BLockingCollection(IProducerCOnsumerConllection<T> collection,Int32 boundeCapacity);            
              boundeCapacity参数指出你想在集合中容纳多少个数据项。在基础集合已满时，如果一个线程调用Add,生产线程会阻塞，
              生产线程可调用TryAdd，传递一个超时值，或者CancellationToken使线程一直阻塞，直到数据添加成功，超时时间到期，获取被取消。

              BLockingCollection实现了IDisposable接口，会调用集合的Dispose，它还会对类内部用于阻塞生成者和消费者的两个
              SemaphoreSlim对象进行清理。

            生产者不在向集合中添加更多项时，可调用CompleteAdding方法，这会向消费者发出信号。让其知道不会再生产更多项了
            具体地说，这会造成正在使用GetConsumingEnumerable的一个foreach循环终止。

            此类还提供一些静态方法
            AddToAny,TryAddToAny
            TakeFromAny,TryTakeFromAny
            所以这些方法都获取一个BlockingCollection<T>[]
            以及一个数据项，一个超时值，和一个CAncellationToken，
            AddToAny方法遍历数组中所有的集合，直到发现因为容量还没满，而能够接收数据项的一个集合，
            TakeFromAny方法则便利数组中所有的集合，知道发现一个能从中移除一个数据项的集合
             */
            var bl = new BlockingCollection<Int32>(new ConcurrentQueue<Int32>());
            ThreadPool.QueueUserWorkItem(ConsumeIems, bl);
            ThreadPool.QueueUserWorkItem(ConsumeIems, bl);
            for (int i = 0; i < 5; i++) {
                Console.WriteLine("producing:" + i);
                bl.Add(i);
            }
            bl.CompleteAdding();
            Console.ReadLine();
        }
        private static void ConsumeIems(Object o) {
            var bl = (BlockingCollection<Int32>)o;
            foreach (var item in bl.GetConsumingEnumerable()) {
                Console.WriteLine("consuming:" + item);
            }
            Console.WriteLine("all items hae been consmed");
        }
    }
}
