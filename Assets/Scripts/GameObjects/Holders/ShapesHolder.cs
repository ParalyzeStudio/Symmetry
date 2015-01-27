using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShapesHolder : MonoBehaviour
{
    public List<GameObject> m_shapes { get; set; }

    public ShapesHolder()
    {
        m_shapes = new List<GameObject>();
    }

    public void AddShape(GameObject shape)
    {
        m_shapes.Add(shape);
    }

    public void RemoveShape(GameObject shape)
    {
        for (int shapeIndex = 0; shapeIndex != m_shapes.Count; shapeIndex++)
        {
            if (m_shapes[shapeIndex] == shape)
            {
                m_shapes.Remove(shape);
                return;
            }
        }
    }

    public void ClearShapes()
    {
        m_shapes.Clear();
    }
}
