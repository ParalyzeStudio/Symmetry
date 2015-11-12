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
     * Add a job to the queue or executes it directly if queue is empty
     * **/
    public void AddJob(ThreadedJob job)
    {
        Debug.Log("AddJob");
        if (m_pendingJobs.Count == 0)
        {
            Debug.Log("AddJob--Queue is empty");
            if (m_currentJob != null) //a job is currently running
            {
                Debug.Log("AddJob--A job is running");
                m_pendingJobs.Enqueue(job);
            }
            else
            {
                Debug.Log("AddJob--No job is running");
                m_currentJob = job;
                job.Start();
            }
        }
        else
        {
            Debug.Log("AddJob--Queue contains " + m_pendingJobs.Count + " elements");
            m_pendingJobs.Enqueue(job);
        }
    }

    public void Update()
    {
        if (m_currentJob != null && m_currentJob.Update()) //a job just finished executing
        {
            if (m_pendingJobs.Count > 0) //execute the next one in the queue
            {
                Debug.Log("Update--Dequeue next job");
                m_currentJob = m_pendingJobs.Dequeue();
                m_currentJob.Start();
            }
            else
            {
                Debug.Log("Update--resetting currentJob to null");
                m_currentJob = null;
            }
        }
    }
}
