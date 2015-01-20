using UnityEngine;
using System.Collections.Generic;

public class Shape
{
    public List<GridTriangle> m_triangles { get; set; } //the list of triangles that will serve as mesh triangles to render this shape
    public Color m_color { get; set; }

    public Shape()
    {
        m_triangles = new List<GridTriangle>();
    }
}