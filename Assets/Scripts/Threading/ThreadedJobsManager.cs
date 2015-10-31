using UnityEngine;
using System.Collections.Generic;

public class ThreadedJobsManager : MonoBehaviour
{
    private const int THREAD_CAPACITY = 4;
    private List<ThreadedJob> m_currentJobs;

    public void Awake()
    {
        m_currentJobs = new List<ThreadedJob>(THREAD_CAPACITY);
    }

    public void AddAndRunJob(ThreadedJob job)
    {
        m_currentJobs.Add(job);
        job.Start();
    }

    public void Update()
    {
        for (int i = 0; i != m_currentJobs.Count; i++)
        {
            if (m_currentJobs[i].Update()) //remove the thread from the active thread list
            {
                m_currentJobs.Remove(m_currentJobs[i]);
                i--;
            }
        }
    }
}
