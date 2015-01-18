using UnityEngine;

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
