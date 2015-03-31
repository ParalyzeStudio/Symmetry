using UnityEngine;
using System.Collections.Generic;

public class Axes : MonoBehaviour
{
    public List<GameObject> m_childrenAxes { get; set; }
    public GameObject m_axisPfb;

    public void Awake()
    {
        m_childrenAxes = new List<GameObject>();
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
        m_childrenAxes.Add(axis);
    }

    public void RemoveAxis(GameObject axis)
    {
        for (int axisIndex = 0; axisIndex != m_childrenAxes.Count; axisIndex++)
        {
            if (m_childrenAxes[axisIndex] == axis)
            {
                m_childrenAxes.Remove(axis);
                return;
            }
        }
    }

    public void ClearAxes()
    {
        m_childrenAxes.Clear();
    }

    public GameObject GetAxisBeingBuilt()
    {
        for (int iAxisIndex = 0; iAxisIndex != m_childrenAxes.Count; iAxisIndex++)
        {
            GameObject axis = m_childrenAxes[iAxisIndex];
            if (axis.GetComponent<AxisRenderer>().m_buildStatus == AxisRenderer.BuildStatus.FIRST_ENDPOINT_SET)
                return axis;
        }

        return null;
    }
}
