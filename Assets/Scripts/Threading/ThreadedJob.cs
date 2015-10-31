using UnityEngine;
//using System.ComponentModel;
using System.Threading;

public class ThreadedJob
{
    public delegate void ThreadFunction();
    private ThreadFunction functionToCallOnPreRun;
    private ThreadFunction functionToRun;
    private ThreadFunction functionToCallOnFinish;

    private bool m_IsDone = false;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;
    public bool IsDone
    {
        get
        {
            bool tmp;
            lock (m_Handle)
            {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set
        {
            lock (m_Handle)
            {
                m_IsDone = value;
            }
        }
    }

    public ThreadedJob(ThreadFunction functionToRun, ThreadFunction functionToCallOnPreRun = null, ThreadFunction functionToCallOnFinish = null)
    {
        this.functionToRun = functionToRun;
        this.functionToCallOnPreRun = functionToCallOnPreRun;
        this.functionToCallOnFinish = functionToCallOnFinish;
    }

    public virtual void Start()
    {
        //call there the function that makes some job before the thread is actually ran
        if (this.functionToCallOnPreRun != null)
            functionToCallOnPreRun();

        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }

    public virtual void Abort()
    {
        m_Thread.Abort();
    }

    protected virtual void OnFinished() 
    {
        if (functionToCallOnFinish != null)
            functionToCallOnFinish();
    }

    public virtual bool Update()
    {
        if (IsDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    private void Run()
    {
        functionToRun();
        IsDone = true;
    }
}

//public class ThreadedJob
//{
//    public void Init(ThreadStart threadStart)
//    {
//        Thread thread = new Thread(threadStart);
//        thread.Start();
//    }

//    //public void Init()
//    //{
//    //    this.DoWork += new DoWorkEventHandler(threadedJob_DoWork);
//    //    this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(threadedJob_RunWorkerCompleted);
//    //    int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
//    //    Debug.Log("INIT threadID:" + threadID);
//    //}

//    //protected virtual void threadedJob_DoWork(object sender, DoWorkEventArgs args)
//    //{
        
//    //}

//    //protected virtual void threadedJob_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
//    //{
//    //    int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
//    //    Debug.Log("RunWorkerCompleted threadID:" + threadID);
//    //}

//    ////long ComputeFibonacci(int n)
//    ////{
//    ////    // The parameter n must be >= 0 and <= 91.
//    ////    // Fib(n), with n > 91, overflows a long.
//    ////    if ((n < 0) || (n > 91))
//    ////    {
//    ////        throw new System.ArgumentException(
//    ////            "value must be >= 0 and <= 91", "n");
//    ////    }

//    ////    long result = 0;

//    ////    if (n < 2)
//    ////    {
//    ////        result = 1;
//    ////    }
//    ////    else
//    ////    {
//    ////        result = ComputeFibonacci(n - 1) +
//    ////                 ComputeFibonacci(n - 2);
//    ////    }

//    ////    return result;
//    ////}
//}
