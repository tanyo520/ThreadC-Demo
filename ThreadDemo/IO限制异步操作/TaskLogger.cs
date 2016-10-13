using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ThreadDemo.IO限制异步操作
{
    public static class TaskLogger
    {
        public enum TaskLoggerLevel { None, Pending }
        public static TaskLoggerLevel LogLevel { get; set; }

        public sealed class TaskLogEntry
        {
            public Task Task { get; internal set; }
            public string Tag { get; internal set; }
            public DateTime LogTime { get; internal set; }
            public string CallerMemberName { get; internal set; }
            public string CallerFilePath { get; internal set; }
            public Int32 CallerLineNumber { get; internal set; }
            public override string ToString()
            {
                return string.Format("LogTime={0},Tag={1},Member={2},File={3}({4})", LogLevel, Tag ?? "None", CallerMemberName, CallerFilePath, CallerLineNumber);
            }
        }
        private static readonly ConcurrentDictionary<Task, TaskLogEntry> s_log = new ConcurrentDictionary<Task, TaskLogEntry>();
        public static IEnumerable<TaskLogEntry> GetLogEntries() { return s_log.Values; }

        public static Task<TResult> Log<TResult>(this Task<TResult> task, string tag = null, [CallerMemberName] string callerMemberName = null, [CallerFilePath]string callerFilePath = null, [CallerLineNumber]Int32 callerLineNumber = -1)
        {
            return (Task<TResult>) Log(task, tag, callerMemberName, callerFilePath, callerLineNumber);
        }
      
        public static Task Log(this Task task, string tag = null, [CallerMemberName] string callerMemberName = null, [CallerFilePath]string callerFilePath = null, [CallerLineNumber]Int32 callerLineNumber = -1)
        {
            if (LogLevel == TaskLoggerLevel.None)
            {
                return task;
            }
            var log = new TaskLogEntry()
            {
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber,
                CallerMemberName = callerMemberName,
                LogTime = DateTime.Now,
                Tag = tag,
                Task = task
            };
            s_log[task] = log;
            task.ContinueWith(t =>
            {
                TaskLogEntry entry;
                s_log.TryRemove(t, out entry);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }
        
    }

    public class De
    {

        
    }
}
