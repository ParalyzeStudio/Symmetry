using UnityEngine;
using System.Collections.Generic;

public class CallFuncHandler : MonoBehaviour
{
    private List<CallFuncObject> m_callFuncInstances;

    //Declare a delegate type for void return type and void parameters
    public delegate void CallFunc();

    public class CallFuncObject
    {
        public float m_timer { get; set; }
        public CallFunc m_callFunc { get; set; }

        public CallFuncObject(CallFunc callFunc, float timer)
        {
            m_callFunc = callFunc;
            m_timer = timer;
        }
    }

    public void Awake()
    {
        m_callFuncInstances = new List<CallFuncObject>();
        m_callFuncInstances.Capacity = 8; //set a big enough capacity to prevent reallocations
    }

    public void AddCallFuncInstance(CallFunc callFunc, float delay)
    {
        CallFuncObject callFuncObject = new CallFuncObject(callFunc, delay);
        m_callFuncInstances.Add(callFuncObject);
    }

    private void RemoveCallFuncInstance(CallFuncObject callFuncObject)
    {
        m_callFuncInstances.Remove(callFuncObject);
    }

    private void ProcessCallFunc(CallFunc callFunc)
    {
        callFunc();
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        for (int i = 0; i != m_callFuncInstances.Count; i++)
        {
            CallFuncObject callFuncObject = m_callFuncInstances[i];

            callFuncObject.m_timer -= dt;
            if (callFuncObject.m_timer < 0)
            {
                ProcessCallFunc(callFuncObject.m_callFunc);
                RemoveCallFuncInstance(callFuncObject);
                i--;
            }
        }
    }
}