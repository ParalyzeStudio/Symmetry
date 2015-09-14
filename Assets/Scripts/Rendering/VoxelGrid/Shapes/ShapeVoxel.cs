using UnityEngine;

public class ShapeVoxel
{
    private Shape m_overlappingShape;
    public Vector2 m_position;

    public ShapeVoxel(Vector2 position)
    {
        m_position = position;
    }

    public void SetOverlappingShape(Shape shape)
    {
        m_overlappingShape = shape;
    }
}