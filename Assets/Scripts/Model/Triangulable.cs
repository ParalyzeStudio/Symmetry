using UnityEngine;
using System.Collections.Generic;

public class Triangulable
{
    public List<BaseTriangle> m_triangles { get; set; }
    public Contour m_contour { get; set; } //the points surrounding the triangulable shape
    public List<Contour> m_holes { get; set; } //the holes inside the triangulable shape
    public float m_area { get;set; }

    public Triangulable()
    {
        m_triangles = new List<BaseTriangle>();
        m_contour = new Contour();
        m_holes = new List<Contour>();
    }

    public Triangulable(Contour contour)
    {
        m_contour = contour;
        m_triangles = new List<BaseTriangle>();
        m_holes = new List<Contour>();
    }

    public Triangulable(Vector2[] contour)
    {
        m_contour = new Contour();
        m_contour.Capacity = contour.Length;
        m_contour.AddRange(contour);
        m_triangles = new List<BaseTriangle>();
        m_holes = new List<Contour>();
    }

    public Triangulable(Contour contour, List<Contour> holes)
    {
        m_contour = contour;
        m_holes = holes;
        m_triangles = new List<BaseTriangle>();
    }

    public Triangulable(Triangulable other)
    {
        //deep copy triangles
        m_triangles = new List<BaseTriangle>();
        m_triangles.Capacity = other.m_triangles.Count;
        for (int i = 0; i != other.m_triangles.Count; i++)
        {
            m_triangles.Add(new BaseTriangle(other.m_triangles[i]));
        }

        //deep copy contour
        m_contour = new Contour();
        m_contour.Capacity = other.m_contour.Count;
        m_contour.AddRange(other.m_contour); //Vector2 is a value type so no need to clone them deeply

        //deep copy holes
        m_holes = new List<Contour>();
        m_holes.Capacity = other.m_holes.Count;
        for (int i = 0; i != other.m_holes.Count; i++)
        {
            Contour holesVec = new Contour();
            holesVec.Capacity = other.m_holes[i].Count;
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
            BaseTriangle triangle = new BaseTriangle();
            triangle.m_points[0] = m_contour[0];
            triangle.m_points[1] = m_contour[1];
            triangle.m_points[2] = m_contour[2];

            m_triangles.Add(triangle);
            m_area = triangle.GetArea();
        }
        else
        {
            Vector2[] triangles = Triangulation.P2tTriangulate(this);

            for (int iVertexIndex = 0; iVertexIndex != triangles.Length; iVertexIndex += 3)
            {
                BaseTriangle triangle = new BaseTriangle();
                triangle.m_points[0] = triangles[iVertexIndex];
                triangle.m_points[1] = triangles[iVertexIndex + 1];
                triangle.m_points[2] = triangles[iVertexIndex + 2];

                m_triangles.Add(triangle);
                m_area += triangle.GetArea();
            }
        }
    }

    /**
     * Does this object contains the parameter 'point'
     * **/
    public bool ContainsPoint(Vector2 point)
    {
        for (int i = 0; i != m_triangles.Count; i++)
        {
            if (m_triangles[i].ContainsPoint(point))
                return true;
        }

        return false;
    }

    /**
     * Calculate contour from triangles 
     * **/
    public void CalculateContour()
    {
        m_contour.Clear(); //clear any previous contour

        //Retrieve all edges contained in this shape
        TriangleEdge[] edges = new TriangleEdge[m_triangles.Count * 3];
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = m_triangles[iTriangleIndex];
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
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = m_triangles[iTriangleIndex];
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
}


