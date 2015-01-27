using UnityEngine;

/**
 * Class that holds data for a triangle in world coordinates
 * **/
public class WorldTriangle
{
    public Vector2[] m_points { get; set; }

    public WorldTriangle()
    {
        m_points = new Vector2[3];
    }
}

/**
 * Class that holds data for a triangle in grid coordinates (line, column)
 * **/
public class GridTriangle
{
    public Vector2[] m_points { get; set; }

    public GridTriangle()
    {
        m_points = new Vector2[3];
    }
}
