using UnityEngine;
using System.Collections.Generic;

public class ShapeVoxel : MonoBehaviour
{
    public List<Shape> m_overlappingShapes { get; set; } //the list of shapes that overlap this voxel
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