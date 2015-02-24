using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShapesHolder : MonoBehaviour
{
    public List<GameObject> m_shapesObj { get; set; }

    public void Awake()
    {
        m_shapesObj = new List<GameObject>();
    }

    public void AddShapeObject(GameObject shape)
    {
        m_shapesObj.Add(shape);
    }

    public void RemoveShapeObject(GameObject shape)
    {
        for (int shapeIndex = 0; shapeIndex != m_shapesObj.Count; shapeIndex++)
        {
            if (m_shapesObj[shapeIndex] == shape)
            {
                m_shapesObj.Remove(shape);
                return;
            }
        }
    }

    public void ClearShapeObjects()
    {
        m_shapesObj.Clear();
    }

    //public void AddShape(Shape shape)
    //{
    //    m_shapes.Add(shape);
    //}

    //public void RemoveShapeObject(Shape shape)
    //{
    //    for (int shapeIndex = 0; shapeIndex != m_shapesObj.Count; shapeIndex++)
    //    {
    //        if (m_shapes[shapeIndex] == shape)
    //        {
    //            m_shapes.Remove(shape);
    //            return;
    //        }
    //    }
    //}

    //public void ClearShapes()
    //{
    //    m_shapes.Clear();
    //}
}
