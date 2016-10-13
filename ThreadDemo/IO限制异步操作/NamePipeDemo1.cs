using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.IO限制异步操作
{
public   class NamePipeDemo1
    {
        public static async void ServerStart()
        {
            var pipe = new NamedPipeServerStream("test", PipeDirection.InOut, -1,
                PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
            await Task.Factory.FromAsync(pipe.BeginWaitForConnection, pipe.EndWaitForConnection, null);
            bool isRun = true;
            while (isRun) {
                StreamReader sr = new StreamReader(pipe);
                string str = "";
                while (pipe.CanRead && (null != (str = sr.ReadLine())))
                {
                    // ProcessMessage(str, pipe);
                    Console.Write(str);
                    if (!pipe.IsConnected)
                    {
                        isRun = false;
                        break;
                    }
                    else {
                        //ProcessMessage(str, pipe);
                    }
                }
                Thread.Sleep(50);

            }
        }
        protected static void ProcessMessage(string str, NamedPipeServerStream pipeServer)
        {
            // Read user input and send that to the client process.
            using (StreamWriter sw = new StreamWriter(pipeServer))
            {
                sw.AutoFlush = true;
                sw.WriteLine("hello world " + str);
            }
        }

        public static async void Go() {
            ServerStart();
            List<Task> request = new List<Task>(1);
            for (int i = 0; i < request.Capacity; i++) {
                request.Add(IssueClientRequst(i));
            }
            await Task.WhenAll(request);
            //for (int n = 0; n < responses.Length; n++) {
            //    Console.WriteLine(responses[n]);
            //}
        }
        public static async Task IssueClientRequst(int n)
        {
            var pipeCline = new NamedPipeClientStream(".", "test", PipeDirection.InOut);
            pipeCline.Connect();
            string ss = string.Format("hello world {0}", n);
            bool isRun = true;
            using (StreamWriter sw = new StreamWriter(pipeCline))
            {
                sw.AutoFlush = true;
                await sw.WriteLineAsync(ss);
                //sw.Flush();
            }
            while (isRun)
            {
                StreamReader sr = new StreamReader(pipeCline);
                string str = "";
                while (pipeCline.CanRead && (null != (str = sr.ReadLine())))
                {
                    // ProcessMessage(str, pipe);
                    Console.Write(str);
                    if (!pipeCline.IsConnected)
                    {
                        isRun = false;
                        break;
                    }
                    else
                    {
                      //  ProcessMessage(str, pipe);
                    }
                }
                Thread.Sleep(50);

            }
           
           

        }
    }
}
