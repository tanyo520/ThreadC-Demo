using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.基元线程同步构造
{
  public  class 用户模式构造
    {
        private  static Boolean s_stopWorker = false;
        public static void Demo() {
            /*
              CLR保证对以下数据类型变量读写是原子性的。Boolean,Char，（S）Byte,(U)Int16,(U)Int32,(U)IntPtr,Single以及引用类型。
              这些意味着变量中的所有字节都一次性读取或者写入。
              虽然对变量原子访问保证读取和写入操作一次性完成，但是由于编译器或者cpu优化，不保证操作什么时候发生。。

              易变构造
              在特定的时间，他在包含一个简单数据类型的变量上执行原子性读或写操作。
              互锁构造
              在特定的时间，他在包含一个简单数据类型的变量上执行原子性读或写操作。
              所有易变和互锁构造都要求传递对包含简单数据类型的一个变量的引用（内存地址）
             */
        }
        /// <summary>
        /// 易变构造
        /// </summary>
        public static void Demo1() {
            /*
             早期软件用汇编写，非常繁琐，要将这个cpu寄存器用于这个，分支到哪里，通过这个间接调用等。后来人们发明了高级语言。
             一系列的if else switch/case。各种循环，局部便利。实参，虚方法。操作符重载等。最终要通过编译器转换成低级构造。使计算机能真正做你想做的事情
             c#转换成il，jit将il转换成本机cpu指令。然后由cpu亲自处理这些指令。除c#编译器，jit编译器外，cpu本身都可能优化你的代码。
             例如 下面value会直接为0 for循环里面的代码会被优化掉。
              Int32 value = (1 * 100) - (50 * 20);
            for (Int32 x = 0; x < value; x++) {
                Console.WriteLine(x);
            }
             */

            /*
             * c#编译器，jit编译器外，cpu优化都是从单线程的角度去优化，但是有些我们多线程下的逻辑可能会被优化掉。
             */
            Console.WriteLine("main for 5 seconds");
            Thread t = new Thread(Worker);
            t.Start();
            Thread.Sleep(5000);
            s_stopWorker = true;
            Console.WriteLine("Main:waiting for worker to stop");
            t.Join();
        }
        /// <summary>
        /// 两个线程同时访问 易变构造
        /// </summary>
        public static void Demo2() {
            ThreadSharingData d = new ThreadSharingData();
            d.Go();
        }
        /// <summary>
        /// 易变构造
        /// </summary>
        public static void Demo3() {
            /*
             如何保证正确调用Volatile.Read和Volatile.Write方法。是程序员最头疼问题之一。
             很难记住这些规则。并搞清楚线程会再后台对数据共享进行什么操作。 为了简化编程c#提供volatile关键字
             可用用于任何静态和实例字段。类型为。Boolean,Char，（S）Byte,(U)Int16,(U)Int32,(U)IntPtr,Single以及引用类型类型为S）Byte,(U)Int16,(U)Int32,等的枚举类型
             JIT编译器确保对易变字段所有以易变读取或者写入的方式执行。不必显示调用Volatile静态Read或Write方法。还会告知C#和JIT编译器不将字段缓存到CPU寄存器
             确保字段所有的读写都在RAM中进行。

            将m_amout定义为volatile字段
            m_amout=m_amout+m_amout
            当前代码会有性能问题。不执行代码优化。编译器必须生成代码将m_amount读入到一个寄存器。再把他读入到另一个寄存器。将两个寄存器相加再一起。再写入到
            m_amout字段。如果再一个循环内，将会造成更大的悲剧。
            c#不支持以引用传递的形式将字段传给方法。会导致编译器生成一条警告信息
              Boolean.TryParse("1", out m_amount);
            //对Volatile字段的引用不被视为volatile
            */

        }
        /// <summary>
        /// 互锁构造
        /// </summary>
        public static void Demo4() {
            /*
              System.Threading.InterLocked类提供的方法，InterLocked类中每个方法都执行一次院子读取以及写入的操作。
              InterLocked的所有方法建立了完整的内存栅栏，调用某个InterLocked方法之前的任何变量写入都再这个InterLocked方法调用之前执行，
              而调用之后的任何变量读取都再这个调用之后读取。
              Increment(ref Int32 i) ++i
              Decrement(ref Int32 i) --i
              Add(ref Int32 i)  i+=value
              Exchange(ref Int32 i,Int32 value) int32 old=i;i=value,return old
              CompareExchange(ref Int32 i,Int32 value,Int32 comparand) old=i; if(i==Comparend)i=value;return old;

             Exchange和CompareExchange能接受Object，IntPtr，Single和Double等类型的参数。这俩个方法各自还有一个泛型版本，类型被约束为Class(任意引用类型)

            InterLocked，他们相当快，而且能做不少事情。
             */
            MultiWebRequests de = new MultiWebRequests(1);
        }
        public static void Worker(Object o) {
            Int32 x = 0;
            while (!Volatile.Read(ref s_stopWorker)) x++;
            Console.WriteLine("worker:stop when x={0}", x);

        }
    }
    /// <summary>
    /// 易变Demo
    /// </summary>
    class ThreadSharingData {
        private Int32 m_flag = 0;
        private Int32 m_value = 0;

        public void Thread1()
        {
            //m_flag = 1;
            //m_value = 5;
            
            m_value = 5;
            // m_flag最后一个写入
           Volatile.Write(ref m_flag,1);
        }

        public void Thread2() {
            //注意可能m_value优于m_falg读取。
            //if (m_flag == 1) {
            //    Console.WriteLine(m_value);
            //}
            //m_value必然再读取了m_flag之后读取
            if (Volatile.Read(ref m_flag) == 1)
            {
                Console.WriteLine(m_value);
            }
        }

        public void Go() {
            Thread t = new Thread(Thread1);
            t.Start();
            Thread t1 = new Thread(Thread2);
            t1.Start();
        }
    }
    /// <summary>
    /// 互锁Demo
    /// </summary>
    class MultiWebRequests {
        private AyncCoordinator m_ac = new AyncCoordinator();
        private Dictionary<string, string> m_servers = new Dictionary<string, string>() {
            { "http://www.baidu.com",null },
            { "http://www.hao123.com",null },
            { "http://www.youku.com",null }
        };
        public MultiWebRequests(Int32 timeOut = Timeout.Infinite) {
            var httpClient = new HttpClient();
            foreach (var server in m_servers.Keys) {
                m_ac.AboutToBengin(1);
                httpClient.GetByteArrayAsync(server).ContinueWith(task =>ComputeResult(server,task));

            }
            m_ac.AllBegin(AllDone, timeOut);
        }
        private void ComputeResult(String server, Task<Byte[]> task) {
            Object result;
            if (task.Exception != null)
            {
                result = task.Exception.InnerException;
            }
            else {
                result = task.Result.Length;
            }
            m_servers[server] = result.ToString();
            m_ac.JustEnded();
        }
        public void Cancel() { m_ac.Cancel(); }
        private void AllDone(CoordinationStatus status) {
            switch (status) {
                case CoordinationStatus.Cancel:
                    Console.WriteLine("cancel");
                    break;
                case CoordinationStatus.TimeOut:
                    Console.WriteLine("time out");
                    break;
                case CoordinationStatus.AllDone:
                    Console.WriteLine("option completed");
                    foreach (var st in m_servers) {
                        Console.WriteLine("keys:{0}", st.Key);
                        Object result = st.Value;
                        if (result is Exception)
                        {
                            Console.WriteLine("failed due to {0}.", result.GetType().Name);
                        }
                        else {
                            Console.WriteLine("value:{0}", result);
                        }
                        
                    }
                    break;
            }
        }
    }
    enum CoordinationStatus {
         AllDone,
         TimeOut,
         Cancel
    }
    class AyncCoordinator {
        private Int32 m_opCount = 1;
        private Int32 m_statusReported = 0;
        private Action<CoordinationStatus> m_callback;
        private Timer m_timer;
        public void AboutToBengin(Int32 opsToAdd = 1) {
            Interlocked.Add(ref m_opCount, opsToAdd);
        }
        public void JustEnded() {
            if (Interlocked.Decrement(ref m_opCount) == 0) {
                ReportStatus(CoordinationStatus.AllDone);
            }
        }

        public void AllBegin(Action<CoordinationStatus> callback,Int32 timerOut=Timeout.Infinite) {
            m_callback = callback;
            if (timerOut != Timeout.Infinite) {
                m_timer = new Timer(TimeExpired, null, timerOut, Timeout.Infinite);
            }
            JustEnded();
        }
        private void TimeExpired(Object o) { ReportStatus(CoordinationStatus.TimeOut); }

        public void Cancel() { ReportStatus(CoordinationStatus.Cancel); }

        private void ReportStatus(CoordinationStatus status) {
            if (Interlocked.Exchange(ref m_statusReported, 1) == 0) {
                m_callback(status);
            }
        }
    }
}
