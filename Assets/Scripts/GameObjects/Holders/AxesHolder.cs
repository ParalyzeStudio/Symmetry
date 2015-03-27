﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AxesHolder : MonoBehaviour 
{
    public List<GameObject> m_axes { get; set; }
    public GameObject m_axisPfb;

    public void Awake()
    {
        m_axes = new List<GameObject>();
    }

    public void BuildAxis(Vector2 gridStartPosition)
    {
        GameObject newAxis = (GameObject)Instantiate(m_axisPfb);
        newAxis.GetComponent<AxisRenderer>().BuildEndpointAtGridPosition(gridStartPosition);
        newAxis.transform.parent = this.gameObject.transform;
        newAxis.transform.localPosition = Vector3.zero;
        AddAxis(newAxis);
    }

    public void AddAxis(GameObject axis)
    {
        m_axes.Add(axis);
    }

    public void RemoveAxis(GameObject axis)
    {
        for (int axisIndex = 0; axisIndex != m_axes.Count; axisIndex++)
        {
            if (m_axes[axisIndex] == axis)
            {
                m_axes.Remove(axis);
                return;
            }
        }
    }

    public void ClearAxes()
    {
        m_axes.Clear();
    }

    public GameObject GetAxisBeingBuilt()
    {
        for (int iAxisIndex = 0; iAxisIndex != m_axes.Count; iAxisIndex++)
        {
            GameObject axis = m_axes[iAxisIndex];
            if (axis.GetComponent<AxisRenderer>().m_buildStatus == AxisRenderer.BuildStatus.FIRST_ENDPOINT_SET)
                return axis;
        }

        return null;
    }
}
