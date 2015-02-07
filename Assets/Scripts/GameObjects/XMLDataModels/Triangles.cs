using UnityEngine;
using System.Collections.Generic;

/**
 * Class that holds data for a triangle in grid coordinates (column, line)
 * **/
public class GridTriangle
{
    public Vector2[] m_points { get; set; }

    public GridTriangle()
    {
        m_points = new Vector2[3];
    }

    /**
    * Tells if one of the edges of this triangle intersects the contour passed as parameter
    **/
    public bool IntersectsContour(Contour contour)
    {
        List<Vector2> contourPoints = contour.m_contour;
        for (int iContourPointIndex = 0; iContourPointIndex != contourPoints.Count; iContourPointIndex++)
        {
            Vector2 contourSegmentPoint1 = contourPoints[iContourPointIndex];
            Vector2 contourSegmentPoint2 = (iContourPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iContourPointIndex + 1];

            for (int iTriangleEdgeIndex = 0; iTriangleEdgeIndex != 3; iTriangleEdgeIndex++)
            {
                Vector2 triangleEdgePoint1 = m_points[iTriangleEdgeIndex];
                Vector2 triangleEdgePoint2 = (iTriangleEdgeIndex == 2) ? m_points[0] : m_points[iTriangleEdgeIndex + 1];

                if (GeometryUtils.TwoSegmentsIntersect(contourSegmentPoint1, contourSegmentPoint2, triangleEdgePoint1, triangleEdgePoint2))
                    return true;
            }
        }
        return false;
    }

    /**
    * Tells if one of the edges of this triangle intersects the segment defined by segmentPoint1 and segmentPoint2
    * **/
    public bool IntersectsSegment(Vector2 segmentPoint1, Vector2 segmentPoint2)
    {
        return GeometryUtils.TwoSegmentsIntersect(m_points[0], m_points[1], segmentPoint1, segmentPoint2) ||
               GeometryUtils.TwoSegmentsIntersect(m_points[1], m_points[2], segmentPoint1, segmentPoint2) ||
               GeometryUtils.TwoSegmentsIntersect(m_points[2], m_points[0], segmentPoint1, segmentPoint2);
    }

    /**
    * Tells if one of the edges of this triangle intersects the line defined by linePoint1 and linePoint2
    * **/
    public bool IntersectsLine(Vector2 linePoint1, Vector2 linePoint2)
    {
        return GeometryUtils.SegmentIntersectsLine(m_points[0], m_points[1], linePoint1, linePoint2) ||
               GeometryUtils.SegmentIntersectsLine(m_points[1], m_points[2], linePoint1, linePoint2) ||
               GeometryUtils.SegmentIntersectsLine(m_points[2], m_points[0], linePoint1, linePoint2);
    }

    /**
    * Calculates the barycentre of this triangle
    * **/
    public Vector2 GetBarycentre()
    {
        return (m_points[0] + m_points[1] + m_points[2]) / 3.0f;
    }

    /**
     * Tells if this triangle contains a point
     * **/
    public bool ContainsGridPoint(Vector2 gridPoint)
    {
        return GeometryUtils.IsInsideTriangle(gridPoint, m_points[0], m_points[1], m_points[2]);
    }

    /**
     * Returns the index of one of the three triangle vertex if the point passed as parameter is equal to it 
     * Otherwise returns -1
     * **/
    public int PointEqualsVertex(Vector2 point)
    {
        if (MathUtils.AreVec2PointsEqual(point, m_points[0]))
            return 0;
        else if (MathUtils.AreVec2PointsEqual(point, m_points[1]))
            return 1;
        else if (MathUtils.AreVec2PointsEqual(point, m_points[2]))
            return 2;

        return -1;
    }

    /**
     * Calculates the area of the triangle
     * **/
    public float GetArea()
    {
        return 0.5f * Mathf.Abs(MathUtils.Determinant(m_points[0], m_points[1], m_points[2], false));
    }
    
    /**
     * Finds intersections between a line and the edges of this triangle
     * **/
    public List<Vector2> FindIntersectionsWithLine(Vector2 linePoint1, Vector2 linePoint2)
    {
        List<Vector2> intersections = new List<Vector2>();
        intersections.Capacity = 2;

        Vector2 lineDirection = linePoint2 - linePoint1;

        Vector2 intersection;
        bool intersects;
        GeometryUtils.SegmentLineIntersection(m_points[0], m_points[1], linePoint1, lineDirection, out intersection, out intersects);
        if (intersects)
            intersections.Add(intersection);

        GeometryUtils.SegmentLineIntersection(m_points[1], m_points[2], linePoint1, lineDirection, out intersection, out intersects);
        if (intersects)
        {
            if (intersections.Count == 0
                ||
                intersections.Count == 1 && !MathUtils.AreVec2PointsEqual(intersection, intersections[0]))
            intersections.Add(intersection);
        }
            

        if (intersections.Count < 2)
        {
            GeometryUtils.SegmentLineIntersection(m_points[2], m_points[0], linePoint1, lineDirection, out intersection, out intersects);
            if (intersects)
                intersections.Add(intersection);
        }

        return intersections;
    }

    /**
     * Splits this triangle intersected by a line
     * **/
    public void Split(Vector2 linePoint1, Vector2 linePoint2, out GridTriangle[] splitTriangles, out int splitTrianglesCount)
    {
        splitTriangles = new GridTriangle[3];
        splitTrianglesCount = 0;

        List<Vector2> intersections = FindIntersectionsWithLine(linePoint1, linePoint2);

        if (intersections.Count != 2)
            return;

        int intersection1IsTriangleVertex = PointEqualsVertex(intersections[0]);
        int intersection2IsTriangleVertex = PointEqualsVertex(intersections[1]);
        if (intersection1IsTriangleVertex >= 0 || intersection2IsTriangleVertex >= 0) //one of the intersection is equal to a triangle vertex
        {
            splitTriangles[0] = new GridTriangle();
            splitTriangles[1] = new GridTriangle();
            splitTrianglesCount = 2;

            if (intersection1IsTriangleVertex >= 0)
            {
                if (intersection1IsTriangleVertex == 0)
                {
                    splitTriangles[0].m_points[0] = intersections[0];
                    splitTriangles[0].m_points[1] = m_points[1];
                    splitTriangles[0].m_points[2] = intersections[1];

                    splitTriangles[1].m_points[0] = intersections[0];
                    splitTriangles[1].m_points[1] = intersections[1];
                    splitTriangles[1].m_points[2] = m_points[2];
                }
                else if (intersection1IsTriangleVertex == 1)
                {
                    splitTriangles[0].m_points[0] = intersections[0];
                    splitTriangles[0].m_points[1] = m_points[2];
                    splitTriangles[0].m_points[2] = intersections[1];

                    splitTriangles[1].m_points[0] = m_points[0];
                    splitTriangles[1].m_points[1] = intersections[0];
                    splitTriangles[1].m_points[2] = intersections[1];
                }
                else if (intersection1IsTriangleVertex == 2)
                {
                    splitTriangles[0].m_points[0] = intersections[0];
                    splitTriangles[0].m_points[1] = m_points[0];
                    splitTriangles[0].m_points[2] = intersections[1];

                    splitTriangles[1].m_points[0] = intersections[0];
                    splitTriangles[1].m_points[1] = intersections[1];
                    splitTriangles[1].m_points[2] = m_points[1];
                }
            }
            else
            {
                if (intersection2IsTriangleVertex == 0)
                {
                    splitTriangles[0].m_points[0] = intersections[1];
                    splitTriangles[0].m_points[1] = m_points[1];
                    splitTriangles[0].m_points[2] = intersections[0];

                    splitTriangles[1].m_points[0] = intersections[1];
                    splitTriangles[1].m_points[1] = intersections[0];
                    splitTriangles[1].m_points[2] = m_points[2];
                }
                else if (intersection2IsTriangleVertex == 1)
                {
                    splitTriangles[0].m_points[0] = intersections[1];
                    splitTriangles[0].m_points[1] = m_points[2];
                    splitTriangles[0].m_points[2] = intersections[0];

                    splitTriangles[1].m_points[0] = m_points[0];
                    splitTriangles[1].m_points[1] = intersections[1];
                    splitTriangles[1].m_points[2] = intersections[0];
                }
                else if (intersection2IsTriangleVertex == 2)
                {
                    splitTriangles[0].m_points[0] = intersections[1];
                    splitTriangles[0].m_points[1] = m_points[0];
                    splitTriangles[0].m_points[2] = intersections[0];

                    splitTriangles[1].m_points[0] = intersections[1];
                    splitTriangles[1].m_points[1] = intersections[0];
                    splitTriangles[1].m_points[2] = m_points[1];
                }
            }
        }
        else //intersections are strictly inside edges
        {
            splitTriangles[0] = new GridTriangle();
            splitTriangles[1] = new GridTriangle();
            splitTriangles[2] = new GridTriangle();
            splitTrianglesCount = 3;

            //find edges on which intersection points are on
            int[] intersectionEdgesNumber = new int[2];

            bool isEdge1Intersected = false;
            bool isEdge2Intersected = false;
            //find where the first intersection is
            isEdge1Intersected = GeometryUtils.IsPointContainedInSegment(intersections[0], m_points[0], m_points[1]);
            if (isEdge1Intersected)
                intersectionEdgesNumber[0] = 1; //intersection is on the first edge of the triangle
            else
            {
                isEdge2Intersected = GeometryUtils.IsPointContainedInSegment(intersections[0], m_points[1], m_points[2]);
                if (isEdge2Intersected)
                    intersectionEdgesNumber[0] = 2; //intersection is on the second edge of the triangle
                else
                    intersectionEdgesNumber[0] = 3; //intersection is on the third edge of the triangle
            }

            //find where the second intersection is
            if (!isEdge1Intersected)
            {
                isEdge1Intersected = GeometryUtils.IsPointContainedInSegment(intersections[1], m_points[0], m_points[1]);
                if (isEdge1Intersected)
                    intersectionEdgesNumber[1] = 1; //intersection is on the first edge of the triangle
                else
                {
                    if (isEdge2Intersected)
                        intersectionEdgesNumber[1] = 3; //intersection is on the third edge of the triangle
                    else
                        intersectionEdgesNumber[1] = 2; //intersection is on the second edge of the triangle
                }
            }
            else
            {
                isEdge2Intersected = GeometryUtils.IsPointContainedInSegment(intersections[1], m_points[1], m_points[2]);
                if (isEdge2Intersected)
                    intersectionEdgesNumber[1] = 2; //intersection is on the second edge of the triangle
                else
                    intersectionEdgesNumber[1] = 3; //intersection is on the third edge of the triangle
            }

            if (intersectionEdgesNumber[1] < intersectionEdgesNumber[0])
            {
                Vector2 tmpIntersection = intersections[0];
                intersections[0] = intersections[1];
                intersections[1] = tmpIntersection;

                int tmpEdgeNumber = intersectionEdgesNumber[0];
                intersectionEdgesNumber[1] = intersectionEdgesNumber[0];
                intersectionEdgesNumber[0] = tmpEdgeNumber;
            }

            
            if (intersectionEdgesNumber[0] == 1 && intersectionEdgesNumber[1] == 2)
            {
                splitTriangles[0].m_points[0] = intersections[0];
                splitTriangles[0].m_points[1] = m_points[1];
                splitTriangles[0].m_points[2] = intersections[1];

                splitTriangles[1].m_points[0] = intersections[0];
                splitTriangles[1].m_points[1] = intersections[1];
                splitTriangles[1].m_points[2] = m_points[2];

                splitTriangles[2].m_points[0] = intersections[0];
                splitTriangles[2].m_points[1] = m_points[2];
                splitTriangles[2].m_points[2] = m_points[0];
            }
            else if (intersectionEdgesNumber[0] == 1 && intersectionEdgesNumber[1] == 3)
            {
                splitTriangles[0].m_points[0] = m_points[0];
                splitTriangles[0].m_points[1] = intersections[0];
                splitTriangles[0].m_points[2] = intersections[1];

                splitTriangles[1].m_points[0] = intersections[0];
                splitTriangles[1].m_points[1] = m_points[1];
                splitTriangles[1].m_points[2] = m_points[2];

                splitTriangles[2].m_points[0] = intersections[0];
                splitTriangles[2].m_points[1] = m_points[2];
                splitTriangles[2].m_points[2] = intersections[1];
            }
            else if (intersectionEdgesNumber[0] == 2 && intersectionEdgesNumber[1] == 3)
            {
                splitTriangles[0].m_points[0] = m_points[0];
                splitTriangles[0].m_points[1] = m_points[1];
                splitTriangles[0].m_points[2] = intersections[0];

                splitTriangles[1].m_points[0] = intersections[0];
                splitTriangles[1].m_points[1] = intersections[1];
                splitTriangles[1].m_points[2] = m_points[0];

                splitTriangles[2].m_points[0] = intersections[0];
                splitTriangles[2].m_points[1] = m_points[2];
                splitTriangles[2].m_points[2] = intersections[1];
            }
        }
    }
}
