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

    public GameObject BuildAxis(Vector2 gridStartPosition)
    {
        GameObject newAxis = (GameObject)Instantiate(m_axisPfb);
        newAxis.transform.parent = this.gameObject.transform;
        newAxis.transform.localPosition = Vector3.zero;

        //Build and render the axis once
        AxisRenderer axisRenderer = newAxis.GetComponent<AxisRenderer>();
        axisRenderer.BuildElements(gridStartPosition, true);
        axisRenderer.Render(gridStartPosition, gridStartPosition, true);

        AddAxis(newAxis);
        return newAxis;
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
        if (m_childrenAxes.Count > 0)
            return m_childrenAxes[m_childrenAxes.Count - 1];

        return null;
    }
}
