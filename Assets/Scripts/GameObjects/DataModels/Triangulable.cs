using UnityEngine;
using System.Collections.Generic;

public class Triangulable
{
    public List<GridTriangle> m_gridTriangles { get; set; }
    public List<Vector2> m_contour { get; set; } //the points surrounding the triangulable shape
    public List<List<Vector2>> m_holes { get; set; } //the holes inside the triangulable shape
    public float m_area { get;set; }

    public Triangulable()
    {
        m_gridTriangles = new List<GridTriangle>();
        m_contour = new List<Vector2>();
        m_holes = new List<List<Vector2>>();
    }

    public Triangulable(List<Vector2> contour)
    {
        m_contour = contour;
        m_gridTriangles = new List<GridTriangle>();
        m_holes = new List<List<Vector2>>();
    }

    public Triangulable(List<Vector2> contour, List<List<Vector2>> holes)
    {
        m_contour = contour;
        m_gridTriangles = new List<GridTriangle>();
        m_holes = new List<List<Vector2>>();
    }

    public Triangulable(Triangulable other)
    {
        //deep copy triangles
        m_gridTriangles = new List<GridTriangle>();
        m_gridTriangles.Capacity = other.m_gridTriangles.Count;
        for (int i = 0; i != other.m_gridTriangles.Count; i++)
        {
            m_gridTriangles.Add(new GridTriangle(other.m_gridTriangles[i]));
        }

        //deep copy contour
        m_contour = new List<Vector2>();
        m_contour.Capacity = other.m_contour.Count;
        m_contour.AddRange(other.m_contour); //Vector2 is a value type so no need to clone them deeply

        //deep copy holes
        m_holes = new List<List<Vector2>>();
        m_holes.Capacity = other.m_holes.Count;
        for (int i = 0; i != other.m_holes.Count; i++)
        {
            List<Vector2> holesVec = new List<Vector2>();
            holesVec.Capacity = other.m_holes[i].Count;
            holesVec.AddRange(other.m_holes[i]); //Vector2 is a value type so no need to clone them deeply
            m_holes.Add(holesVec);
        } 
      
        //copy area
        m_area = other.m_area;
    }

    public virtual void Triangulate()
    {
        List<Vector2> triangles = new List<Vector2>();

        Triangulation.Process(m_contour, ref triangles);

        m_area = 0;
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

    /**
     * Calculate contour from triangles 
     * **/
    public void CalculateContour()
    {
        m_contour.Clear(); //clear any previous contour

        //Retrieve all edges contained in this shape
        TriangleEdge[] edges = new TriangleEdge[m_gridTriangles.Count * 3];
        for (int iTriangleIndex = 0; iTriangleIndex != m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_gridTriangles[iTriangleIndex];
            for (int iVertexIndex = 0; iVertexIndex != triangle.m_points.Length; iVertexIndex++)
            {
                TriangleEdge edge = new TriangleEdge(triangle.m_points[iVertexIndex], (iVertexIndex < 2) ? triangle.m_points[iVertexIndex + 1] : triangle.m_points[0]);
                edges[iTriangleIndex * 3 + iVertexIndex] = edge;
            }
        }

        //Recreate a list with edges that don't repeat when looping
        List<TriangleEdge> unrepeatEdges = new List<TriangleEdge>();
        for (int iEdgeIndex = 0; iEdgeIndex != edges.Length; iEdgeIndex++)
        {
            bool bPushEdge = true;
            TriangleEdge edge = edges[iEdgeIndex];
            if (edge == null)
                continue;
            for (int k = iEdgeIndex + 1; k != edges.Length; k++)
            {
                if (edges[k] == null)
                    continue;

                if (edges[k].Equals(edge))
                {
                    edges[k] = null;
                    edges[iEdgeIndex] = null;
                    bPushEdge = false;
                    break;
                }
            }

            if (bPushEdge)
                unrepeatEdges.Add(edge);
        }

        //Finally build the contour by assembling edges
        TriangleEdge peakEdge = unrepeatEdges[0];
        m_contour.Add(peakEdge.m_pointA);
        m_contour.Add(peakEdge.m_pointB);
        Vector2 firstVertex = peakEdge.m_pointA;
        Vector2 peakVertex = peakEdge.m_pointB;
        
        while (!MathUtils.AreVec2PointsEqual(peakVertex, firstVertex))
        {
            TriangleEdge nextEdge = FindEdgeContainingVertex(unrepeatEdges, peakVertex, peakEdge);
            if (MathUtils.AreVec2PointsEqual(nextEdge.m_pointA, peakVertex))
            {
                peakVertex = nextEdge.m_pointB;
                if (peakVertex == firstVertex) //end of the loop
                    break;
                m_contour.Add(nextEdge.m_pointB);
                peakEdge = nextEdge;
            }
            else if (MathUtils.AreVec2PointsEqual(nextEdge.m_pointB, peakVertex))
            {
                peakVertex = nextEdge.m_pointA;
                if (peakVertex == firstVertex) //end of the loop
                    break;
                m_contour.Add(nextEdge.m_pointA);
                peakEdge = nextEdge;
            }
        }
    }

    /**
     * Find the edge containing the vertex passed as parameter
     * In a closed polygon two edges share this vertex so exclude the one which is the current peakEdge
     * **/
    private TriangleEdge FindEdgeContainingVertex(List<TriangleEdge> allEdges, Vector2 vertex, TriangleEdge peakEdge)
    {
        for (int iEdgeIndex = 0; iEdgeIndex != allEdges.Count; iEdgeIndex++)
        {
            TriangleEdge edge = allEdges[iEdgeIndex];
            if (edge == peakEdge) //next edge cannot be the current peak edge
                continue;

            if (MathUtils.AreVec2PointsEqual(edge.m_pointA, vertex) || MathUtils.AreVec2PointsEqual(edge.m_pointB, vertex))
            {
                return edge;
            }
        }

        return null;
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


