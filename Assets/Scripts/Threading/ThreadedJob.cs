using UnityEngine;
//using System.ComponentModel;
using System.Threading;

public class ThreadedJob
{
    public string m_name { get; set; }

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