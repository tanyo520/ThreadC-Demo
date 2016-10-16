using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    class BarrierDemo
    {

        public static void Demo() {
            /*
             Barrier控制的一系列线程需要并行工作，从而再一个算法的不同阶段推进，或许通过一个例子更容易理解
             当CLR使用它的垃圾回收器（GC）的服务器版本时，GC算法为每个内核都创建一个线程。这些线程在不同应用程序线程
             的栈中向上移动。并发标记堆中对象。每个线程完成了它自己的那一部分工作后，必须停下来等待其他线程完成。所有线程
             都标记好对象后，线程就可以并发地压缩堆中的不同部分。每个线程都完成了对他的那一部分堆的压缩之后，线程
             必须阻塞等待其他线程，所有线程都完成了对自己那一部分的堆压缩之后，所有线程都要再应用程序的线程的栈中上行
             对根进行修正，使之引用因为压缩而发生了移动的对象的新位置。只有所有线程都完成这个工作之后，垃圾回收器
             的工作才算真正完成。应用程序的线程限制可以恢复执行了。
             AddParticipants(Int32 participantCount) 添加参与者。
             RemoveParticipants(Int32 participantCount)移除参与者
             SignalAndWait(Int32 millisecondsTimeOut,CancellationToken cancellationToken);
             Int64 CurrentPhaseNumber{get;}//指出进行到哪一个阶段（从0开始）
             Int32 ParticipantCount{get;} 参与者数量
             Int32 ParticipantsRemaining{get;}需要调用SignalAndWait的线程数

            构造Barrier时要告诉它有多少个线程准备参与工作，还可传递一个Action<Barrier>委托来引用所有参与者完成一个阶段
            的工作后调用的代码。可以调用AddPariticipant和RemoveParticipant方法在Barrier中动态添加和删除参与线程。
            但实际应用中，人们很少这样做，每个线程完成它的阶段性工作后，应调用SignalAndWait，告诉Barrier线程，以及完成了一个阶段的工作，
            而Barrier会阻塞线程（使用一个ManualResetEventSlim）。所有参与者都调用了SignalAndWait后，Barrier将调用指定的委托（由最后一个调用SignalAndWait的线程调用）
            然后解除正在等待的所有线程的组塞，使他们开始下一个阶段。

             http://blog.csdn.net/wangzhiyu1980/article/details/45688075
             */
            // Barrier s=new Barrier()
        }

       public class BarrierTest
        {
            private static void Phase0Doing(int TaskID)
            {
                Console.WriteLine("Task : #{0}   =====  Phase 0", TaskID);
            }

            private static void Phase1Doing(int TaskID)
            {
                Console.WriteLine("Task : #{0}   *****  Phase 1", TaskID);
            }

            private static void Phase2Doing(int TaskID)
            {
                Console.WriteLine("Task : #{0}   ^^^^^  Phase 2", TaskID);
            }

            private static void Phase3Doing(int TaskID)
            {
                Console.WriteLine("Task : #{0}   $$$$$  Phase 3", TaskID);
            }

            private static int _TaskNum = 4;
            private static Task[] _Tasks;
            private static Barrier _Barrier;


          public  static void Go()
            {
                _Tasks = new Task[_TaskNum];
                _Barrier = new Barrier(_TaskNum, (barrier) =>
                {
                    Console.WriteLine("-------------------------- Current Phase:{0} --------------------------",
                                      _Barrier.CurrentPhaseNumber);
                });

                for (int i = 0; i < _TaskNum; i++)
                {
                    _Tasks[i] = Task.Factory.StartNew((num) =>
                    {
                        var taskid = (int)num;

                        Phase0Doing(taskid);
                        _Barrier.SignalAndWait();

                        Phase1Doing(taskid);
                        _Barrier.SignalAndWait();

                        Phase2Doing(taskid);
                        _Barrier.SignalAndWait();

                        Phase3Doing(taskid);
                        _Barrier.SignalAndWait();

                    }, i);
                }

                var finalTask = Task.Factory.ContinueWhenAll(_Tasks, (tasks) =>
                {
                    Task.WaitAll(_Tasks);
                    Console.WriteLine("========================================");
                    Console.WriteLine("All Phase is completed");

                    _Barrier.Dispose();
                });

                finalTask.Wait();

                Console.ReadLine();
            }
        }
    }
}
