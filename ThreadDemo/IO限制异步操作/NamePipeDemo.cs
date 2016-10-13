using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadDemo.IO限制异步操作
{
  public  class NamePipeDemo
    {
        /// <summary>
        /// 将APM模式转换成task
        /// </summary>
        public static async void StarServer() {
            var pipe = new NamedPipeServerStream("test", PipeDirection.InOut, -1,
                PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
           await Task.Factory.FromAsync(pipe.BeginWaitForConnection, pipe.EndWaitForConnection, null);


            using (StreamWriter sw = new StreamWriter(pipe))
            {
                sw.AutoFlush = true;
                sw.WriteLine("hello world ");
            }

        }
        public static  void ClientServer()
        {
            var pipeClient = new NamedPipeClientStream(".", "test", PipeDirection.In);
            pipeClient.Connect();
            using (StreamReader sr = new StreamReader(pipeClient))
            {
                string temp;
                while ((temp = sr.ReadLine()) != null)
                {
                    Console.WriteLine(string.Format("Received from server: {0}", temp));
                }
            }

        }

        public static async Task<String> AwaitWebClient(Uri url) {
            var wc = new System.Net.WebClient();
            var tcs = new TaskCompletionSource<String>();
            wc.DownloadStringCompleted += (s, e) =>
            {
                if (e.Cancelled) tcs.SetCanceled();
                else if (e.Error != null) {
                    tcs.SetException(e.Error);
                 }
                else tcs.SetResult(e.Result);
            };
            wc.DownloadStringAsync(url);
            try
            {
                String result = await tcs.Task;
                //Console.WriteLine(result);
                return result;
            }
            catch (AggregateException e) {

                return null;
            }
        }
    }
}
