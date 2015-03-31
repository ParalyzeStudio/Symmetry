using UnityEngine;
using System.Collections.Generic;

public class Counter : MonoBehaviour
{
    public GameObject m_counterElementPfb;
    public List<GameObject> m_elements { get; set; }

    public void Init()
    {
        m_elements = new List<GameObject>();
    }

    public void Build()
    {
        //LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        float elementSpacing = 92.0f;
        //int maxActions = levelManager.m_currentLevel.m_maxActions;
        int maxActions = 4;
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

            m_elements.Add(clonedCounterElement);
        }
    }
}

