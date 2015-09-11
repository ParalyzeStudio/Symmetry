using UnityEngine;

public class ShapeVoxel
{
    public enum ShapeVoxelState
    {
        EMPTY = 0,
        FILLED
    }

    public ShapeVoxelState m_state;
    public Vector2 m_position;

    public ShapeVoxel(Vector2 position)
    {
        m_position = position;
        m_state = ShapeVoxelState.EMPTY;
    }
}