using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo.混合线程同步构造
{
    public enum OneManyMode { Exclusive, Shared }
    public class AsyncOneManyLock
    {
        private SpinLock m_lock = new SpinLock(true);//自旋锁不要用readonly
        private void Lock() { Boolean taken = false; m_lock.Enter(ref taken); }
        private void UnLock() { m_lock.Exit(); }

        private Int32 m_state = 0;
        private Boolean IsFree { get { return m_state == 0; } }
        private Boolean IsOwnedByWriter { get { return m_state == -1; } }

        private Boolean IsOwnedByReaders { get { return m_state > 0; } }
        private Int32 AddReaders(Int32 count) { return m_state += count; }
        private Int32 SubtractReader() { return --m_state; }
        private void MakeWriter() { m_state = -1; }
        private void MakeFree() { m_state = 0; }

        //目的是在非竞态条件时增强性能和减少内存消耗
        private readonly Task m_noCntentionAccessGranter;
        //每个等待writer都通过它们在这里排队的TaskCompletionSource来唤醒
        private readonly Queue<TaskCompletionSource<Object>> m_qWaitingWriters = new Queue<TaskCompletionSource<object>>();
        //一个TaskCompletionSource收到信号，所有等待的Reader都唤醒
        private TaskCompletionSource<Object> m_waitingReadersSignal = new TaskCompletionSource<object>();
        private Int32 m_numWaitingReaders = 0;
        public AsyncOneManyLock()
        {
            m_noCntentionAccessGranter = Task.FromResult<Object>(null);
        }
        public Task WaitAsync(OneManyMode mode) {
            Task accessGranter = m_noCntentionAccessGranter;//假定无竞争
            Lock();
            switch (mode) {
                case OneManyMode.Exclusive:
                    if (IsFree)
                    {
                        MakeWriter();//无竞争
                    }
                    else {
                        //有竞争，新的writer任务进入队列，并返回使它writer等待
                        var cts = new TaskCompletionSource<Object>();
                        m_qWaitingWriters.Enqueue(cts);
                        accessGranter = cts.Task;
                    }
                    break;
                case OneManyMode.Shared:
                    if (IsFree || (IsOwnedByReaders && m_qWaitingWriters.Count == 0))
                    {
                        AddReaders(1);//无竞争
                    }
                    else {
                        //有竞争 递增等待的reader数量，并返回reader任务使reader等待
                        m_numWaitingReaders++;
                        accessGranter = m_waitingReadersSignal.Task.ContinueWith(t => t.Result);
                    }
                    break;
            }
            UnLock();
            return accessGranter;
        }
        public void Release() {
            TaskCompletionSource<Object> accessGranter = null;//假定没有代码被释放
            Lock();
            if (IsOwnedByWriter) MakeFree();//一个writer离开
            else SubtractReader();//一个reader离开
            if (IsFree)
            {
                if (m_qWaitingWriters.Count > 0)
                {
                    MakeWriter();
                    accessGranter = m_qWaitingWriters.Dequeue();
                }
                else if (m_numWaitingReaders > 0) {
                    AddReaders(m_numWaitingReaders);
                    m_numWaitingReaders = 0;
                    accessGranter = m_waitingReadersSignal;
                    //为将来需要等待readers创建一个新的tcs
                    m_waitingReadersSignal = new TaskCompletionSource<object>();
                }
               
            }
            UnLock();
            //唤醒锁外面的writer/reader,减少竞争机率提高性能。
            if (accessGranter != null) accessGranter.SetResult(null);
        }
    }

    public class AsyncOneManyLockDemo{

        public static void Demo() {

        }
        private static async Task AccessResourceViaAsyncSynchronization(AsyncOneManyLock asyncLock) {
            await asyncLock.WaitAsync(OneManyMode.Shared);
            //如果执行到这里，表面没有其他线程在向资源写入，可能有其他线程在读写
            //读完以后释放资源
            asyncLock.Release();
        }
    }
}
