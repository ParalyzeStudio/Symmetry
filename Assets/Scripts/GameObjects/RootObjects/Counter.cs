﻿using UnityEngine;
using System.Collections.Generic;

public class Counter : MonoBehaviour
{
    public GameObject m_counterElementPfb;
    public List<CounterElement> m_elements { get; set; }

    public int m_reachedCounterElementNumber { get; set; } //the number of the counter element that we reached and whose current status is marked as CURRENT
                                                           //if counter has just been built, set this number to 0 (no current element)

    public Material m_elementSkinOnMaterial;
    public Material m_elementSkinOffMaterial;
    public Material m_elementShadowMaterial;
    public Material m_elementOverlayMaterial;

    public void Init()
    {
        m_elements = new List<CounterElement>();
    }

    public void Build()
    {
        //First clone materials that will be passed to each counter element
        Material clonedShadowMaterial = (Material)Instantiate(m_elementShadowMaterial);
        Material clonedSkinOnMaterial = (Material)Instantiate(m_elementSkinOnMaterial);
        Material clonedSkinOffMaterial = (Material)Instantiate(m_elementSkinOffMaterial);
        Material clonedOverlayMaterial = (Material)Instantiate(m_elementOverlayMaterial);

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

            //Create the counter element
            CounterElement counterElementComponent = clonedCounterElement.gameObject.GetComponent<CounterElement>();
            counterElementComponent.Init(clonedShadowMaterial, clonedSkinOnMaterial, clonedSkinOffMaterial, clonedOverlayMaterial);

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