using UnityEngine;
using System.Collections.Generic;

public class ShapeVoxel : MonoBehaviour
{
    private List<Shape> m_overlappingShapes; //the list of shapes that overlap this voxel
    public Vector3 m_position { get; set; }

    public void Init(Vector3 position)
    {
        m_position = position;
        m_overlappingShapes = new List<Shape>();
    }

    public void ClearOverlappingShapes()
    {
        m_overlappingShapes.Clear();
    }

    public bool AddOverlappingShape(Shape shape)
    {
        for (int i = 0; i != m_overlappingShapes.Count; i++)
        {
            if (m_overlappingShapes[i] == shape)
                return false;
        }

        m_overlappingShapes.Add(shape);
        return true;
    }

    public void RemoveOverlappingShape(Shape shape)
    {
        for (int i = 0; i != m_overlappingShapes.Count; i++)
        {
            if (m_overlappingShapes[i] == shape)
            {
                m_overlappingShapes.Remove(shape);
                return;
            }
        }
    }

    public bool IsOverlappedByShape(Shape shape)
    {
        for (int i = 0; i != m_overlappingShapes.Count; i++)
        {
            if (m_overlappingShapes[i] == shape)
                return true;
        }

        return false;
    }
}