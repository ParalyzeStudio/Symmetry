using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Triangle
{
    public Vector2 pointA;
    public Vector2 pointB;
    public Vector2 pointC;
}

public class Shape
{
    public List<Vector2> m_points { get; set; } //the list of points forming this shape in GridAnchor coordinates (line, column) in CCW order
    public List<Triangle> m_triangles { get; set; } //the list of triangles that will serve as mesh triangles to render this shape

    public Shape()
    {
        m_points = new List<Vector2>();
        m_triangles = new List<Triangle>();
    }
}
