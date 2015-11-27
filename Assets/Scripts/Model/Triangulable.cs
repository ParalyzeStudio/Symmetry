using UnityEngine;
using System.Collections.Generic;

public class GridTriangulable
{
    public List<GridTriangle> m_triangles { get; set; }
    public Contour m_contour { get; set; } //the points surrounding the triangulable shape
    public List<Contour> m_holes { get; set; } //the holes inside the triangulable shape
    public float m_area { get;set; }

    public GridTriangulable()
    {
        m_triangles = new List<GridTriangle>();
        m_contour = new Contour();
        m_holes = new List<Contour>();
    }

    public GridTriangulable(Contour contour)
    {
        m_contour = contour;
        m_triangles = new List<GridTriangle>();
        m_holes = new List<Contour>();
    }

    public GridTriangulable(GridPoint[] contour)
    {
        m_contour = new Contour();
        m_contour.Capacity = contour.Length;
        m_contour.AddRange(contour);
        m_triangles = new List<GridTriangle>();
        m_holes = new List<Contour>();
    }

    public GridTriangulable(Contour contour, List<Contour> holes)
    {
        m_contour = contour;
        m_holes = holes;
        m_triangles = new List<GridTriangle>();
    }

    public GridTriangulable(GridTriangulable other)
    {
        //deep copy triangles
        m_triangles = new List<GridTriangle>(other.m_triangles.Count);
        for (int i = 0; i != other.m_triangles.Count; i++)
        {
            m_triangles.Add(new GridTriangle(other.m_triangles[i]));
        }

        //deep copy contour
        m_contour = new Contour(other.m_contour.Count);
        m_contour.AddRange(other.m_contour); //Vector2 is a value type so no need to clone them deeply

        //deep copy holes
        m_holes = new List<Contour>(other.m_holes.Count);
        for (int i = 0; i != other.m_holes.Count; i++)
        {
            Contour holesVec = new Contour(other.m_holes[i].Count);
            holesVec.AddRange(other.m_holes[i]); //Vector2 is a value type so no need to clone them deeply
            m_holes.Add(holesVec);
        } 
      
        //copy area
        m_area = other.m_area;
    }

    public virtual void Triangulate()
    {
        m_triangles.Clear();

        if (m_contour.Count == 3 && m_holes.Count == 0) //only one triangle
        {
            m_triangles.Capacity = 1;

            GridTriangle triangle = new GridTriangle();
            triangle.m_points[0] = m_contour[0];
            triangle.m_points[1] = m_contour[1];
            triangle.m_points[2] = m_contour[2];

            m_triangles.Add(triangle);
            m_area = triangle.GetArea();
        }
        else
        {
            Vector2[] triangles = Triangulation.P2tTriangulate(this);
            GridPoint[] gridTriangles = new GridPoint[triangles.Length];
            for (int i = 0; i != triangles.Length; i++)
            {
                Vector2 triangleVertex = triangles[i];
                gridTriangles[i] = new GridPoint((int) triangleVertex.x, (int) triangleVertex.y);
                gridTriangles[i].m_scale = GridPoint.DEFAULT_SCALE_PRECISION;
            }

            for (int iVertexIndex = 0; iVertexIndex != gridTriangles.Length; iVertexIndex += 3)
            {
                m_triangles.Capacity = triangles.Length;

                GridTriangle triangle = new GridTriangle();
                triangle.m_points[0] = gridTriangles[iVertexIndex];
                triangle.m_points[1] = gridTriangles[iVertexIndex + 1];
                triangle.m_points[2] = gridTriangles[iVertexIndex + 2];

                m_triangles.Add(triangle);
                m_area += triangle.GetArea();
            }
        }       
    }

    /**
     * Does this object contains the parameter 'point'
     * **/
    public bool ContainsPoint(GridPoint point)
    {
        for (int i = 0; i != m_triangles.Count; i++)
        {
            if (m_triangles[i].ContainsPoint(point))
                return true;
        }

        return false;
    }

    /**
     * Find the edge containing the vertex passed as parameter
     * In a closed polygon two edges share this vertex so exclude the one which is the current peakEdge
     * **/
    //private GridTriangleEdge FindEdgeContainingVertex(List<GridTriangleEdge> allEdges, Vector2 vertex, GridTriangleEdge peakEdge)
    //{
    //    for (int iEdgeIndex = 0; iEdgeIndex != allEdges.Count; iEdgeIndex++)
    //    {
    //        GridTriangleEdge edge = allEdges[iEdgeIndex];
    //        if (edge == peakEdge) //next edge cannot be the current peak edge
    //            continue;

    //        if (MathUtils.AreVec2PointsEqual(edge.m_pointA, vertex) || MathUtils.AreVec2PointsEqual(edge.m_pointB, vertex))
    //        {
    //            return edge;
    //        }
    //    }

    //    return null;
    //}

    public void CalculateArea()
    {
        m_area = 0;
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_triangles[iTriangleIndex];
            m_area += triangle.GetArea();
        }
    }

    /**
    * Calculates the barycentre of this polygon
    * **/
    public Vector2 GetBarycentre()
    {
        Vector2 barycentre = Vector2.zero;
        for (int i = 0; i != m_contour.Count; i++)
        {
            barycentre += m_contour[i];
        }

        barycentre /= m_contour.Count;

        return barycentre;
    }

    /**
     * Approximate vertices coordinates (contour and holes) by rounding values to 'significantFiguresCount' after the decimal point
     * For instance passing 0 will round all values to the closest integer
     * passing 1 will maintain 1 digit after the after the decimal point
     * **/
    //public void ApproximateVertices(int significantFiguresCount)
    //{
    //    //contour
    //    m_contour.ApproximateVertices(significantFiguresCount);

    //    //holes
    //    for (int i = 0; i != m_holes.Count; i++)
    //    {
    //        m_holes[i].ApproximateVertices(significantFiguresCount);
    //    }
    //}
}


