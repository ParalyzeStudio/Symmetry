using UnityEngine;
using System.Collections.Generic;

public class Shape
{
    public List<Vector2> m_contour;
    public List<GridTriangle> m_gridTriangles { get; set; } //the list of triangles that will serve as mesh triangles to render this shape
    public Color m_color { get; set; }

    public Shape()
    {
        m_contour = new List<Vector2>();
        m_gridTriangles = new List<GridTriangle>();
    }

    public void Triangulate()
    {
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
        }

        //GameObject debugTriangles = new GameObject("debugTriangles");
        //debugTriangles.transform.position = new Vector3(0, 0, -100.0f);
        //for (int iTriangleIndex = 0; iTriangleIndex != triangles.Count; iTriangleIndex += 3)
        //{
        //    GameObject edge1 = new GameObject("triangleEdge");
        //    GridSegment gridSegment1 = edge1.AddComponent<GridSegment>();
        //    edge1.AddComponent<MeshFilter>();
        //    edge1.AddComponent<MeshRenderer>();
        //    gridSegment1.m_startPointGrid = triangles[iTriangleIndex];
        //    gridSegment1.m_endPointGrid = triangles[iTriangleIndex + 1];
        //    edge1.transform.parent = debugTriangles.transform;
        //    edge1.transform.localPosition = MathUtils.BuildVector3FromVector2(edge1.transform.position, 0);

        //    GameObject edge2 = new GameObject("triangleEdge");
        //    GridSegment gridSegment2 = edge2.AddComponent<GridSegment>();
        //    edge2.AddComponent<MeshFilter>();
        //    edge2.AddComponent<MeshRenderer>();
        //    gridSegment2.m_startPointGrid = triangles[iTriangleIndex + 1];
        //    gridSegment2.m_endPointGrid = triangles[iTriangleIndex + 2];
        //    edge2.transform.parent = debugTriangles.transform;
        //    edge2.transform.localPosition = MathUtils.BuildVector3FromVector2(edge2.transform.position, 0);

        //    GameObject edge3 = new GameObject("triangleEdge");
        //    GridSegment gridSegment3 = edge3.AddComponent<GridSegment>();
        //    edge3.AddComponent<MeshFilter>();
        //    edge3.AddComponent<MeshRenderer>();
        //    gridSegment3.m_startPointGrid = triangles[iTriangleIndex + 2];
        //    gridSegment3.m_endPointGrid = triangles[iTriangleIndex];
        //    edge3.transform.parent = debugTriangles.transform;
        //    edge3.transform.localPosition = MathUtils.BuildVector3FromVector2(edge3.transform.position, 0);
        //}
    }
}