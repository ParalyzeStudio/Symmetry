//#define DEBUG_ENABLED

using UnityEngine;
using System.Collections.Generic;

/**
 * A class that executes threads in sequence (one at a time)
 * It can be useful for portions of code that share a same global variable and need current thread to finish to be executed properly
 * In practice we will use this thread manager instead of the classic ThreadedJobsManager for clipping job because 
 * it requires a global Clipper instance to be shared
 * **/
public class QueuedThreadedJobsManager : MonoBehaviour
{
    private Queue<ThreadedJob> m_pendingJobs;
    private ThreadedJob m_currentJob;


    public void Awake()
    {
        m_pendingJobs = new Queue<ThreadedJob>(4); //set some capacity to the queue
    }

    /**
     * Add a job to the queue or executes it directly if queue is empty and no job is currently executing
     * **/
    public void AddJob(ThreadedJob job)
    {
#if DEBUG_ENABLED
        Debug.Log(">>>>AddJob");
#endif
        if (m_pendingJobs.Count == 0)
        {
#if DEBUG_ENABLED
            Debug.Log("AddJob--Queue is empty");
#endif
            if (m_currentJob != null) //a job is currently running
            {
#if DEBUG_ENABLED
                Debug.Log("AddJob--A job is running, Enqueue this job to run it later");
#endif
                m_pendingJobs.Enqueue(job);
            }
            else
            {
#if DEBUG_ENABLED
                Debug.Log("AddJob--No job is running, this job can start immediately");
#endif
                m_currentJob = job;
                job.Start();
            }
        }
        else
        {
#if DEBUG_ENABLED
            Debug.Log("AddJob--Queue contains " + m_pendingJobs.Count + " elements");
#endif
            m_pendingJobs.Enqueue(job);
        }
    }

    public void Update()
    {
        if (m_currentJob != null && m_currentJob.Update()) //a job just finished executing
        {
#if DEBUG_ENABLED
            Debug.Log("Update--Job just finished executing");
#endif

            if (m_pendingJobs.Count > 0) //execute the next one in the queue
            {
#if DEBUG_ENABLED
                Debug.Log("Update--Dequeue next job and run it");
#endif
                m_currentJob = m_pendingJobs.Dequeue();
                m_currentJob.Start();
            }
            else
            {
#if DEBUG_ENABLED
                Debug.Log("Update--no more job pending ==> resetting currentJob to null");
#endif
                m_currentJob = null;
            }
        }
    }
}
