using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

/**
 * Class that holds data for a list of GridTriangle
 * **/
public class Shape
{
    public List<GridTriangle> m_triangles { get; set; } //the list of triangles that will serve as mesh triangles to render this shape

    public Shape()
    {
        m_triangles = new List<GridTriangle>();
    }
}
