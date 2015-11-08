using UnityEngine;
using System.Collections.Generic;

public class Counter : MonoBehaviour
{
    //Material shared across counter elements
    public Material m_filledElementMaterial;
    public Material m_emptyElementMaterial;
    public Material m_outerContourMaterial;

    public GameObject m_counterElementPfb;
    public List<CounterElement> m_elements { get; set; }

    public int m_reachedCounterElementNumber { get; set; } //the number of the counter element that we reached and whose current status is marked as CURRENT
                                                           //if counter has just been built, set this number to 0 (no current element)

    public void Init()
    {
        m_elements = new List<CounterElement>();
    }

    public void Build()
    {
        //First clone materials that will be passed to each counter element
        Material filledElementMaterial = Instantiate(m_filledElementMaterial);
        Material emptyElementMaterial = Instantiate(m_emptyElementMaterial);
        Material outerContourMaterial = Instantiate(m_outerContourMaterial);

        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

        float elementSpacing = 115.0f;
        int maxActions = levelManager.m_currentLevel.m_maxActions;
        maxActions = 4; //TODO remove this line
        for (int i = 0; i != maxActions; i++)
        {
            GameObject clonedCounterElement = (GameObject) Instantiate(m_counterElementPfb);

            float counterElementPositionX;
            if (maxActions % 2 == 0) //even number of actions
                counterElementPositionX = ((i + 1 - maxActions / 2 - 0.5f) * elementSpacing);
            else //odd number of actions
                counterElementPositionX = ((i - maxActions / 2) * elementSpacing);

            GameObjectAnimator counterElementAnimator = clonedCounterElement.gameObject.GetComponent<GameObjectAnimator>();
            counterElementAnimator.SetParentTransform(this.transform);
            counterElementAnimator.SetPosition(new Vector3(counterElementPositionX, 0, 0));

            //Create the counter element
            CounterElement counterElementComponent = clonedCounterElement.gameObject.GetComponent<CounterElement>();
            Color tintColor = levelManager.m_currentChapter.GetThemeColors()[4];
            counterElementComponent.Init(tintColor, filledElementMaterial, emptyElementMaterial, outerContourMaterial);
            counterElementComponent.SetStatus(i == 0 ? CounterElement.CounterElementStatus.CURRENT : CounterElement.CounterElementStatus.WAITING);

            m_elements.Add(counterElementComponent);
        }

        m_reachedCounterElementNumber = 0;
    }

    /**
     * Add one more action to the counter
     * **/
    public void IncrementCounter()
    {
        if (m_reachedCounterElementNumber >= m_elements.Count)
            return;

        m_reachedCounterElementNumber++;

        int elementIdx = m_reachedCounterElementNumber - 1;

        m_elements[elementIdx].SetStatus(CounterElement.CounterElementStatus.CURRENT);

        if (elementIdx > 0) //set the previous element as DONE
            m_elements[elementIdx - 1].SetStatus(CounterElement.CounterElementStatus.DONE);       
    }

    /**
     * Tells if we reached the last counter element
     * **/
    public bool isFull()
    {
        return (m_reachedCounterElementNumber == m_elements.Count);
    }
}