using UnityEngine;
using System.Collections.Generic;

public class ShapeVoxel
{
    public List<Shape> m_overlappingShapes { get; set; } //the list of shapes that overlap this voxel
    public Vector3 m_position { get; set; }

    public float m_xEdge, m_yEdge; //distances from voxel to intersection points along x and y axes

    public Vector2 m_xNormal, m_yNormal; //normals (hermite data) at intersection points along x and y axes

    public GameObject m_debugQuad { get; set; }

    public ShapeVoxel(Vector3 position)
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