using UnityEngine;
using System.Collections.Generic;

public class Counter : MonoBehaviour
{
    public GameObject m_counterElementPfb;
    public List<CounterElement> m_elements { get; set; }

    public int m_activeCounterElementNumber { get; set; }

    public void Init()
    {
        m_elements = new List<CounterElement>();
    }

    public void Build()
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        float elementSpacing = 92.0f;
        int maxActions = levelManager.m_currentLevel.m_maxActions;
        for (int i = 0; i != maxActions; i++)
        {
            GameObject clonedCounterElement = (GameObject) Instantiate(m_counterElementPfb);
            clonedCounterElement.transform.parent = this.gameObject.transform;

            float counterElementPositionX;
            if (maxActions % 2 == 0) //even number of actions
                counterElementPositionX = ((i + 1 - maxActions / 2 - 0.5f) * elementSpacing);
            else //odd number of actions
                counterElementPositionX = ((i - maxActions / 2) * elementSpacing);

            GameObjectAnimator counterElementAnimator = clonedCounterElement.gameObject.GetComponent<GameObjectAnimator>();
            counterElementAnimator.SetPosition(new Vector3(counterElementPositionX, 0, 0));

            CounterElement counterElementComponent = clonedCounterElement.gameObject.GetComponent<CounterElement>();
            counterElementComponent.Init();

            m_elements.Add(counterElementComponent);
        }

        m_activeCounterElementNumber = 1;
    }

    /**
     * Add one more action to the counter
     * **/
    public void IncrementCounter()
    {
        int elementIdx = m_activeCounterElementNumber - 1;

        m_elements[elementIdx].SetStatus(CounterElement.CounterElementStatus.CURRENT);

        if (elementIdx > 0) //set the previous element as DONE
            m_elements[elementIdx - 1].SetStatus(CounterElement.CounterElementStatus.DONE);

        m_activeCounterElementNumber++;
    }
}

