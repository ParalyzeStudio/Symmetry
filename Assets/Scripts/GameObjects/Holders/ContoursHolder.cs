using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContoursHolder : MonoBehaviour
{
    public List<GameObject> m_contoursObj { get; set; }
    public List<Contour> m_contours { get; set; }

    public ContoursHolder()
    {
        m_contoursObj = new List<GameObject>();
        m_contours = new List<Contour>();
    }

    public void AddContour(GameObject contour)
    {
        m_contoursObj.Add(contour);
    }

    public void RemoveContour(GameObject contour)
    {
        for (int contourIndex = 0; contourIndex != m_contoursObj.Count; contourIndex++)
        {
            if (m_contoursObj[contourIndex] == contour)
            {
                m_contoursObj.Remove(contour);
                return;
            }
        }
    }

    public void ClearContours()
    {
        m_contoursObj.Clear();
    }
}
