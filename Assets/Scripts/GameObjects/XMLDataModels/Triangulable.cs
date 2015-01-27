using UnityEngine;
using System.Collections.Generic;

public class Triangulable
{
    public List<GridTriangle> m_gridTriangles { get; set; }
    public List<Vector2> m_contour { get; set; }
    public float m_area { get;set; }

    public Triangulable()
    {
        m_gridTriangles = new List<GridTriangle>();
        m_contour = new List<Vector2>();
    }

    public void Triangulate()
    {
        m_area = 0;

        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        GridBuilder gridBuilder = gridObject.GetComponent<GridBuilder>();

        List<Vector2> triangles = new List<Vector2>();

        Triangulation.Process(m_contour, ref triangles);

        for (int iVertexIndex = 0; iVertexIndex != triangles.Count; iVertexIndex += 3)
        {
            GridTriangle gridTriangle = new GridTriangle();
            gridTriangle.m_points[0] = triangles[iVertexIndex];
            gridTriangle.m_points[1] = triangles[iVertexIndex + 1];
            gridTriangle.m_points[2] = triangles[iVertexIndex + 2];

            m_gridTriangles.Add(gridTriangle);
            m_area += gridTriangle.GetArea();
        }        
    }

    public void CalculateArea()
    {
        m_area = 0;
        for (int iTriangleIndex = 0; iTriangleIndex != m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_gridTriangles[iTriangleIndex];
            m_area += triangle.GetArea();
        }
    }

    public bool ContainsGridPoint(Vector2 gridPoint)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_gridTriangles.Count; iTriangleIndex++)
        {
            if (m_gridTriangles[iTriangleIndex].ContainsGridPoint(gridPoint))
                return true;
        }

        return false;
    }
}


