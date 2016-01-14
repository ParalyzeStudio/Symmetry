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

    /**
     * Triangulate this object
     * **/
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
            //use a copy to prevent the current shape contour and holes to be modified when separating shared points
            GridTriangulable triangulableCopy = new GridTriangulable(this);
            List<BiasedPoint> biasedPoints = triangulableCopy.SeparateHolesSharedPoints();           

            Vector2[] triangles = Triangulation.P2tTriangulate(triangulableCopy);
            GridPoint[] gridTriangles = FillTrianglesWithExactValues(triangles, biasedPoints);         

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

    public struct BiasedPoint
    {
        public GridPoint m_biasedValue { get; set; }
        public GridPoint m_exactValue { get; set; }

        public BiasedPoint(GridPoint biasedValue, GridPoint exactValue)
            : this()
        {
            m_biasedValue = biasedValue;
            m_exactValue = exactValue;
        }
    }

    /**
     * In case of 2 holes (or more) sharing the same point, we have to separate them otherwise the triangulation will fail
     * When we encounter a hole point that is also on the contour of another hole, we move it in the direction of its 2 neighbouring vertices
     * We return a set of pair values each containing the biased GridPoint with its relevant exact GridPoint
     * **/
    private List<BiasedPoint> SeparateHolesSharedPoints()
    {
        List<BiasedPoint> biasedPoints = new List<BiasedPoint>();

        for (int iHoleIdx = 0; iHoleIdx != m_holes.Count; iHoleIdx++)
        {
            Contour holeContour = m_holes[iHoleIdx];            
            for (int i = 0; i != holeContour.Count; i++)
            {
                GridPoint holePoint = holeContour[i];

                //search if contour contains this point
                if (m_contour.ContainsPoint(holePoint))
                {
                    GridPoint leftNeighbor = (i > 0) ? holeContour[i - 1] : holeContour[holeContour.Count - 1];
                    GridPoint rightNeighbor = (i < holeContour.Count - 1) ? holeContour[i + 1] : holeContour[0];
                    //Determine the position of the hole point relatively to its 2 neighbors
                    long det = MathUtils.Determinant(leftNeighbor, rightNeighbor, holePoint);
                    GridPoint translationDirection = (leftNeighbor - holePoint) + (rightNeighbor - holePoint);
                    Vector2 normalizedTranslationDirection = translationDirection.NormalizeAsVector2();
                    int translationLength = 1; //set an approximate length to move the shared point
                    if (det > 0) //'left'
                        translationDirection = -translationDirection;

                    GridPoint translation = new GridPoint(Mathf.RoundToInt(normalizedTranslationDirection.x * translationLength),
                                                          Mathf.RoundToInt(normalizedTranslationDirection.y * translationLength));

                    //Add the biased point to the set
                    GridPoint exactGridPoint = holeContour[i];
                    GridPoint biasedGridPoint = exactGridPoint + translation;
                    BiasedPoint biasedPoint = new BiasedPoint(biasedGridPoint, exactGridPoint);
                    biasedPoints.Add(biasedPoint);

                    holeContour[i] = biasedGridPoint;
                }

                //search among other holes if they contain this point
                for (int j = iHoleIdx + 1; j != m_holes.Count; j++) //search among other holes if they contain this point
                {
                    if (m_holes[j].ContainsPoint(holePoint))
                    {
                        GridPoint leftNeighbor = (i > 0) ? holeContour[i - 1] : holeContour[holeContour.Count - 1];
                        GridPoint rightNeighbor = (i < holeContour.Count - 1) ? holeContour[i + 1] : holeContour[0]; 
                        //Determine the position of the hole point relatively to its 2 neighbors
                        long det = MathUtils.Determinant(leftNeighbor, rightNeighbor, holePoint);
                        GridPoint translationDirection = (leftNeighbor - holePoint) + (rightNeighbor - holePoint);
                        Vector2 normalizedTranslationDirection = translationDirection.NormalizeAsVector2();
                        int translationLength = 1; //set an approximate length to move the shared point
                        if (det > 0) //'left'
                            translationDirection = -translationDirection;

                        GridPoint translation = new GridPoint(Mathf.RoundToInt(normalizedTranslationDirection.x * translationLength),
                                                              Mathf.RoundToInt(normalizedTranslationDirection.y * translationLength));

                        //Add the biased point to the set
                        GridPoint exactGridPoint = holeContour[i];
                        GridPoint biasedGridPoint = exactGridPoint + translation;
                        BiasedPoint biasedPoint = new BiasedPoint(biasedGridPoint, exactGridPoint);
                        biasedPoints.Add(biasedPoint);

                        holeContour[i] = biasedGridPoint;
                    }
                }
            }
        }

        return biasedPoints;
    }

    private GridPoint[] FillTrianglesWithExactValues(Vector2[] triangles, List<BiasedPoint> biasedValues)
    {
        GridPoint[] gridTriangles = new GridPoint[triangles.Length];

        for (int i = 0; i != triangles.Length; i++)
        {
            Vector2 triangleVertex = triangles[i];
            GridPoint gridTriangleVertex = new GridPoint((int)triangleVertex.x, (int)triangleVertex.y, false);

            //check if the current vertex had its coordinates biased
            for (int j = 0; j != biasedValues.Count; j++)
            {
                BiasedPoint biasedValue = biasedValues[j];
                if (biasedValue.m_biasedValue.Equals(gridTriangleVertex)) //if so replace it with the exact value
                    gridTriangleVertex = biasedValue.m_exactValue;
            }

            gridTriangles[i] = gridTriangleVertex;
        }

        return gridTriangles;
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
            if (this is DottedOutline)
                Debug.Log("triangleArea" + iTriangleIndex + ":" + triangle.GetArea());
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


