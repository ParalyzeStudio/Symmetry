using UnityEngine;
using System.Collections.Generic;

/**
 * Class to hold data for a triangle edge (2 GridPoints)
 * **/
public class GridEdge
{
    public GridPoint m_pointA { get; set; }
    public GridPoint m_pointB { get; set; }

    public const int EDGES_OVERLAP = 1;
    public const int EDGES_INTERSECTION_IS_ENDPOINT = 2;
    public const int EDGES_STRICT_INTERSECTION = 4;

    public GridEdge(GridPoint pointA, GridPoint pointB)
    {
        m_pointA = pointA;
        m_pointB = pointB;
    }

    public bool Equals(GridEdge other)
    {
        return (this.m_pointA == other.m_pointA && this.m_pointB == other.m_pointB)
               ||
               (this.m_pointA == other.m_pointB && this.m_pointA == other.m_pointB);
    }

    /**
     * Tells if this edge intersects the edge passed as parameter without giving the intersection
     * **/
    public bool IntersectsEdge(GridEdge edge, int bFilters = EDGES_OVERLAP | EDGES_INTERSECTION_IS_ENDPOINT | EDGES_STRICT_INTERSECTION)
    {
        GridEdge edge1 = this;
        GridEdge edge2 = edge;

        bool bSeg1Pt1InSeg2 = edge2.ContainsPoint(edge1.m_pointA);
        bool bSeg1Pt2InSeg2 = edge2.ContainsPoint(edge1.m_pointB);
        bool bSeg2Pt1InSeg1 = edge1.ContainsPoint(edge2.m_pointA);
        bool bSeg2Pt2InSeg1 = edge1.ContainsPoint(edge2.m_pointB);

        long det1 = MathUtils.Determinant(edge1.m_pointA, edge1.m_pointB, edge2.m_pointA);
        long det2 = MathUtils.Determinant(edge1.m_pointA, edge1.m_pointB, edge2.m_pointB);
        if (det1 == 0 && det2 == 0) //4 endpoints are aligned, segments are collinear and on the same line
        {
            //one segment is contained in the other
            if (bSeg1Pt1InSeg2 && bSeg1Pt2InSeg2 || bSeg2Pt1InSeg1 && bSeg2Pt2InSeg1)
                if ((bFilters & EDGES_OVERLAP) > 0)
                    return true;

            //check if segments share at least 1 endpoint
            long dotProduct = 0;
            if (MathUtils.AreVec2PointsEqual(edge1.m_pointB, edge2.m_pointA))
                dotProduct = MathUtils.DotProduct(edge1.m_pointA - edge1.m_pointB, edge2.m_pointB - edge2.m_pointA);
            else if (MathUtils.AreVec2PointsEqual(edge1.m_pointB, edge2.m_pointB))
                dotProduct = MathUtils.DotProduct(edge1.m_pointA - edge1.m_pointB, edge2.m_pointA - edge2.m_pointB);
            else if (MathUtils.AreVec2PointsEqual(edge1.m_pointA, edge2.m_pointA))
                dotProduct = MathUtils.DotProduct(edge1.m_pointB - edge1.m_pointA, edge2.m_pointB - edge2.m_pointA);
            else if (MathUtils.AreVec2PointsEqual(edge1.m_pointA, edge2.m_pointB))
                dotProduct = MathUtils.DotProduct(edge1.m_pointB - edge1.m_pointA, edge2.m_pointA - edge2.m_pointB);

            if (dotProduct != 0)
            {
                if (dotProduct > 0)
                {
                    if ((bFilters & EDGES_OVERLAP) > 0)
                        return true;
                }
                else
                {
                    if ((bFilters & EDGES_INTERSECTION_IS_ENDPOINT) > 0)
                        return true;
                }
            }

            //check if one segment endpoint is strictly contained in the other segment
            if (bSeg1Pt1InSeg2 || bSeg1Pt2InSeg2)
                if ((bFilters & EDGES_OVERLAP) > 0)
                    return true;

            //all other cases = no intersection
            return false;
        }
        else if (det1 == 0 || det2 == 0) //3 points are aligned
        {
            if (bSeg1Pt1InSeg2 || bSeg1Pt2InSeg2 || bSeg2Pt1InSeg1 || bSeg2Pt2InSeg1)
                if ((bFilters & EDGES_INTERSECTION_IS_ENDPOINT) > 0)
                    return true;

            return false;
        }
        else if ((det1 < 0 && det2 > 0) || (det1 > 0 && det2 < 0)) //check if det1 and det2 are of opposite signs
        {
            long det3 = MathUtils.Determinant(edge2.m_pointA, edge2.m_pointB, edge1.m_pointA);
            long det4 = MathUtils.Determinant(edge2.m_pointA, edge2.m_pointB, edge1.m_pointB);

            if ((det3 < 0 && det4 > 0) || (det3 > 0 && det4 < 0)) //check if det3 and det4 are of opposite signs
            {
                return (bFilters & EDGES_STRICT_INTERSECTION) > 0; //we only check the strict intersection here
            }
            else
            {
                return false;
            }
        }
        else /** (det1 * det2) > 0 **/
        {
            return false;
        }
    }

    /**
     * Return the intersection (if it exists) between this edge and the line defined by linePoint and lineDirection
     * **/
    public void IntersectionWithLine(GridPoint linePoint, GridPoint lineDirection, out bool intersects, out GridPoint intersection)
    {
        //Store global values in temporary values for the purpose of this calculation
        GridPoint pointA = m_pointA;
        GridPoint pointB = m_pointB;

        //order points by ascending x for the segment only
        if (pointA.X > pointB.X)
        {
            GridPoint tmpPoint = pointB;
            pointB = pointA;
            pointA = tmpPoint;
        }
        else if (pointA.X == pointB.X) // if x-coordinate is the same for both points, order them by ascending y
        {
            if (pointA.Y > pointB.Y)
            {
                GridPoint tmpPoint = pointB;
                pointB = pointA;
                pointA = tmpPoint;
            }
        }

        //Both lines equation
        float x, y;
        if (lineDirection.X != 0 && pointA.X != pointB.X) //y = a1x + b1 && y = a2x + b2
        {
            float a1 = lineDirection.Y / (float) lineDirection.X;
            float b1 = linePoint.Y - a1 * linePoint.X;
            float a2 = (pointB.Y - pointA.Y) / (pointB.X - pointA.X);
            float b2 = pointA.Y - a2 * pointA.X;

            if (a1 == a2) //parallel lines
            {
                intersects = false;
                intersection = GridPoint.zero;
                return;
            }
            else
            {
                x = (b2 - b1) / (a1 - a2);
                y = a1 * x + b1;
            }
        }
        else if (lineDirection.X != 0 && pointA.X == pointB.X) //y = a1x + b1 && x = a2
        {
            float a1 = lineDirection.Y / (float) lineDirection.X;
            float b1 = linePoint.Y - a1 * linePoint.X;
            float a2 = pointA.X;

            x = a2;
            y = a1 * a2 + b1;
        }
        else if (lineDirection.X == 0 && pointA.X != pointB.X) //x = a1 && y = a2x + b2
        {
            float a1 = linePoint.X;
            float a2 = (pointB.Y - pointA.Y) / (pointB.X - pointA.X);
            float b2 = pointA.Y - a2 * pointA.X;

            x = a1;
            y = a2 * a1 + b2;
        }
        else //parallel vertical lines, no intersection or infinite intersections. In both cases return no intersection
        {
            intersects = false;
            intersection = GridPoint.zero;
            return;
        }

        //Round (x,y)
        int X = Mathf.RoundToInt(x);
        int Y = Mathf.RoundToInt(y);

        //Check if ((x, y) point is contained in the segment
        if (pointA.X == pointB.X)
        {
            if (Y >= pointA.Y && Y <= pointB.Y)
            {
                intersects = true;
                intersection = new GridPoint(X, Y, false);
                return;
            }
            else
            {
                intersects = false;
                intersection = GridPoint.zero;
                return;
            }
        }
        else
        {
            if (X >= pointA.X && X <= pointB.X)
            {
                intersects = true;
                intersection = new GridPoint(X, Y, false);
                return;
            }
            else
            {
                intersects = false;
                intersection = GridPoint.zero;
                return;
            }
        }
    }

    /**
     * Tells if this edge contains the point passed as parameter
     * **/
    public bool ContainsPoint(GridPoint point, bool bIncludeEndpoints = true)
    {
        if (point == m_pointA || point == m_pointB)
            return bIncludeEndpoints;

        long det = MathUtils.Determinant(m_pointA, m_pointB, point);
        if (det != 0) //points are not aligned, thus point can never be on the segment AB
            return false;

        //points are aligned, just test if points is inside the strip defined by A and B
        GridPoint u = point - m_pointA; //AM vector
        GridPoint v = m_pointB - m_pointA; //AB vector

        long dotProduct = MathUtils.DotProduct(u, v); //calculate the dot product AM.AB
        if (dotProduct > 0)
            return dotProduct < v.sqrMagnitude; //AM length should be majored by AB length so dot product AM.AB should be majored by AB squared length
        else //AM and AB are of opposite sign
            return false;
    }

    /**
     * Does the strip defined by this axis contains the parameter 'point'
     * **/
    public bool ContainsPointInStrip(GridPoint point)
    {
        GridPoint u = point - m_pointA; //AM vector
        GridPoint v = m_pointB - m_pointA; //AB vector

        long dotProduct = MathUtils.DotProduct(u, v); //calculate the dot product AM.AB

        if (dotProduct >= 0)
            return dotProduct <= v.sqrMagnitude; //AM length should be majored by AB length so dot product AM.AB should be majored by AB squared length
        else //AM and AB are of opposite sign
            return false;
    }
}

/**
 * Class that holds data to represent a triangle where points are in ccw order
 * **/
//public class BaseTriangle
//{
//    public Vector2[] m_points { get; set; }

//    public BaseTriangle()
//    {
//        m_points = new Vector2[3];
//    }

//    public BaseTriangle(BaseTriangle other) : this()
//    {
//        for (int i = 0; i != 3; i++)
//        {
//            m_points[i] = other.m_points[i];
//        }
//    }

//    public Vector2 GetCenter()
//    {
//        return (m_points[0] + m_points[1] + m_points[2]) / 3.0f;
//    }

//    public bool HasVertex(Vector2 vertex)
//    {
//        return MathUtils.AreVec2PointsEqual(m_points[0], vertex) || 
//               MathUtils.AreVec2PointsEqual(m_points[1], vertex) || 
//               MathUtils.AreVec2PointsEqual(m_points[2], vertex);
//    }

//    public bool ContainsPoint(Vector2 point)
//    {
//        return GeometryUtils.IsInsideTriangle(point, m_points[0], m_points[1], m_points[2]);
//    }

//    /**
//     * Return true if this triangle intersects the parameter 'triangle'
//     * This intersection can have a null area (triangles just share points or portions of edges for instance)
//     * To test if this intersection is effectively null, use the method IntersectTriangleWithNonNullIntersection()
//     * **/
//    public bool IntersectsTriangle(BaseTriangle triangle)
//    {
//        bool bIntersectEdges = IntersectsEdge(triangle.m_points[0], triangle.m_points[1]) ||
//                               IntersectsEdge(triangle.m_points[1], triangle.m_points[2]) ||
//                               IntersectsEdge(triangle.m_points[2], triangle.m_points[0]);

//        //Test if the triangle is contained inside the other
//        if (!bIntersectEdges)
//            return this.ContainsPoint(triangle.GetCenter());
//        else 
//            return true;
//    }

//    public bool IntersectsTriangleWithNonNullIntersection(BaseTriangle triangle)
//    {
//        if (!IntersectsTriangle(triangle)) //no intersection at all, return false
//            return false;

//        return IsIntersectionWithTriangleNull(triangle);
//    }

//    private bool IsIntersectionWithTriangleNull(BaseTriangle triangle)
//    {
//        int[] point1OnContourEdgeIndices = ContourContainsPoint(triangle.m_points[0]);
//        int[] point2OnContourEdgeIndices = ContourContainsPoint(triangle.m_points[1]);
//        int[] point3OnContourEdgeIndices = ContourContainsPoint(triangle.m_points[2]);

//        int pointsOnContourCount = 0;
//        if (point1OnContourEdgeIndices != null) pointsOnContourCount++;
//        if (point2OnContourEdgeIndices != null) pointsOnContourCount++;
//        if (point3OnContourEdgeIndices != null) pointsOnContourCount++;            

//        if (pointsOnContourCount == 0) 
//        {
//            //if the second triangle does not contain any of the first triangle vertices on its contour then there is a non-null intersection
//            return (triangle.ContourContainsPoint(m_points[0]) == null &&
//                    triangle.ContourContainsPoint(m_points[1]) == null &&
//                    triangle.ContourContainsPoint(m_points[2]) == null);

//        }
//        else if (pointsOnContourCount == 3) //all points are on the contour of the other triangle so the first triangle is contained in the second triangle
//            return true;
//        else if (pointsOnContourCount == 2)
//        {
//            //test if points are equal to one of the triangle vertices
//            bool bOnVertex1 = false;
//            bool bOnVertex2 = false;
//            bool bOnVertex3 = false;
//            if (point1OnContourEdgeIndices != null && point1OnContourEdgeIndices.Length == 2)
//            {
//                if (point1OnContourEdgeIndices[0] == 2 && point1OnContourEdgeIndices[1] == 0)
//                    bOnVertex1 = true;
//                else if (point1OnContourEdgeIndices[0] == 0 && point1OnContourEdgeIndices[1] == 1)
//                    bOnVertex2 = true;
//                else if (point1OnContourEdgeIndices[0] == 1 && point1OnContourEdgeIndices[1] == 2)
//                    bOnVertex3 = true;
//            }
//            if (point2OnContourEdgeIndices != null && point2OnContourEdgeIndices.Length == 2)
//            {
//                if (point2OnContourEdgeIndices[0] == 2 && point2OnContourEdgeIndices[1] == 0)
//                    bOnVertex1 = true;
//                else if (point2OnContourEdgeIndices[0] == 0 && point2OnContourEdgeIndices[1] == 1)
//                    bOnVertex2 = true;
//                else if (point2OnContourEdgeIndices[0] == 1 && point2OnContourEdgeIndices[1] == 2)
//                    bOnVertex3 = true;
//            }
//            if (point3OnContourEdgeIndices != null && point3OnContourEdgeIndices.Length == 2)
//            {
//                if (point3OnContourEdgeIndices[0] == 2 && point3OnContourEdgeIndices[1] == 0)
//                    bOnVertex1 = true;
//                else if (point3OnContourEdgeIndices[0] == 0 && point3OnContourEdgeIndices[1] == 1)
//                    bOnVertex2 = true;
//                else if (point3OnContourEdgeIndices[0] == 1 && point3OnContourEdgeIndices[1] == 2)
//                    bOnVertex3 = true;
//            }
//            //bool bOnVertex1 = (point1OnContourEdgeIndices != null && point1OnContourEdgeIndices.Length == 2);
//            //bool bOnVertex2 = (point2OnContourEdgeIndices != null && point2OnContourEdgeIndices.Length == 2);
//            //bool bOnVertex3 = (point3OnContourEdgeIndices != null && point3OnContourEdgeIndices.Length == 2);

//            //Count points that are strictly on an edge
//            int bOnEdge1Count = 0;
//            int bOnEdge2Count = 0;
//            int bOnEdge3Count = 0;
//            if (point1OnContourEdgeIndices != null && point1OnContourEdgeIndices.Length == 1)
//            {
//                if (point1OnContourEdgeIndices[0] == 0)
//                    bOnEdge1Count++;
//                else if (point1OnContourEdgeIndices[0] == 1)
//                    bOnEdge2Count++;
//                else if (point1OnContourEdgeIndices[0] == 2)
//                    bOnEdge3Count++;
//            }
//            if (point2OnContourEdgeIndices != null && point2OnContourEdgeIndices.Length == 1)
//            {
//                if (point2OnContourEdgeIndices[0] == 0)
//                    bOnEdge1Count++;
//                else if (point2OnContourEdgeIndices[0] == 1)
//                    bOnEdge2Count++;
//                else if (point2OnContourEdgeIndices[0] == 2)
//                    bOnEdge3Count++;
//            }
//            if (point3OnContourEdgeIndices != null && point3OnContourEdgeIndices.Length == 1)
//            {
//                if (point3OnContourEdgeIndices[0] == 0)
//                    bOnEdge1Count++;
//                else if (point3OnContourEdgeIndices[0] == 1)
//                    bOnEdge2Count++;
//                else if (point3OnContourEdgeIndices[0] == 2)
//                    bOnEdge3Count++;
//            }

//            //Find the index of the edge the two points share. If they are on different edges there is a non-null intersection
//            int sharedEdgeIndex;
//            if (bOnVertex1 && bOnVertex2)
//                sharedEdgeIndex = 0;
//            else if (bOnVertex2 && bOnVertex3)
//                sharedEdgeIndex = 1;
//            else if (bOnVertex1 && bOnVertex3)
//                sharedEdgeIndex = 2;
//            else if ((bOnVertex1 || bOnVertex2) && bOnEdge1Count == 1)
//                sharedEdgeIndex = 0;
//            else if ((bOnVertex2 || bOnVertex3) && bOnEdge2Count == 1)
//                sharedEdgeIndex = 1;
//            else if ((bOnVertex3 || bOnVertex1) && bOnEdge3Count == 1)
//                sharedEdgeIndex = 2;
//            else if (bOnEdge1Count == 2)
//                sharedEdgeIndex = 0;
//            else if (bOnEdge2Count == 2)
//                sharedEdgeIndex = 1;
//            else if (bOnEdge3Count == 2)
//                sharedEdgeIndex = 2;
//            else //one point on a different edge each time ==> non-null intersection
//                return true;

//            //Find the point that is not on the contour
//            Vector2 offContourPoint;
//            if (point1OnContourEdgeIndices == null)
//                offContourPoint = triangle.m_points[0];
//            else if (point2OnContourEdgeIndices == null)
//                offContourPoint = triangle.m_points[1];
//            else
//                offContourPoint = triangle.m_points[2];

//            //Determine the position (left or right) of the off contour point about the edge set previously
//            Vector2 edgePoint1 = m_points[sharedEdgeIndex];
//            Vector2 edgePoint2 = m_points[(sharedEdgeIndex == 2) ? 0 : sharedEdgeIndex + 1];
//            float det = MathUtils.Determinant(edgePoint1, edgePoint2, offContourPoint);
//            return (det > 0); //'left'
//        }
//        else //(pointsOnContourCount == 1)
//        {
//            //Find the index of the point that is on this triangle contour
//            Vector2 pointOnContour;
//            Vector2[] pointsOffContour = new Vector2[2];
//            int[] edgeIndices;
//            if (point1OnContourEdgeIndices != null)
//            {
//                pointOnContour = triangle.m_points[0];
//                pointsOffContour[0] = triangle.m_points[1];
//                pointsOffContour[1] = triangle.m_points[2];
//                edgeIndices = point1OnContourEdgeIndices;
//            }
//            else if (point2OnContourEdgeIndices != null)
//            {
//                pointOnContour = triangle.m_points[1];
//                pointsOffContour[0] = triangle.m_points[0];
//                pointsOffContour[1] = triangle.m_points[2];
//                edgeIndices = point2OnContourEdgeIndices;
//            }
//            else
//            {
//                pointOnContour = triangle.m_points[2];
//                pointsOffContour[0] = triangle.m_points[0];
//                pointsOffContour[1] = triangle.m_points[1];
//                edgeIndices = point3OnContourEdgeIndices;
//            }

//            //test if the point is equal to one of the triangle vertices
//            if (edgeIndices.Length == 1)
//            {
//                int edgeIndex = edgeIndices[0];
//                Vector2 edgePoint1 = m_points[edgeIndex];
//                Vector2 edgePoint2 = m_points[edgeIndex == 2 ? 0 : edgeIndex + 1];

//                //Check if at least one of the two points off contour is on the 'left' of the edge
//                float det = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[0]);
//                if (det > 0) return true; //'left'
//                det = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[1]);
//                return (det > 0); //'left'
//            }
//            else // edgeIndices.Length == 2 (point is equal to one of the triangle vertices)
//            {
//                //check if both off contour points are on the left or the right of the first edge
//                int edgeIndex = edgeIndices[0];
//                Vector2 edgePoint1 = m_points[edgeIndex];
//                Vector2 edgePoint2 = m_points[edgeIndex == 2 ? 0 : edgeIndex + 1];
//                float det1 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[0]);
//                float det2 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[1]);

//                if (det1 < 0 && det2 < 0) //both points are on the 'right' of the edge, no intersection
//                    return false;

//                //check if both off contour points are on the left or the right of the second edge
//                edgeIndex = edgeIndices[1];
//                edgePoint1 = m_points[edgeIndex];
//                edgePoint2 = m_points[edgeIndex == 2 ? 0 : edgeIndex + 1];
//                det1 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[0]);
//                det2 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[1]);

//                if (det1 < 0 && det2 < 0) //both points are on the 'right' of the edge, no intersection
//                    return false;

//                //check if both off contour points are on the left or the right of line directed by the sum of first and second edges
//                Vector2 firstEdgePoint1 = m_points[edgeIndices[0]];
//                Vector2 firstEdgePoint2 = m_points[edgeIndices[0] == 2 ? 0 : edgeIndices[0] + 1];
//                Vector2 secondEdgePoint1 = m_points[edgeIndices[1]];
//                Vector2 secondEdgePoint2 = m_points[edgeIndices[1] == 2 ? 0 : edgeIndices[1] + 1];
//                Vector2 firstEdge = firstEdgePoint2 - firstEdgePoint1;
//                Vector2 secondEdge = secondEdgePoint2 - secondEdgePoint1;

//                Vector2 sumResultingEdge = firstEdge + secondEdge;

//                det1 = MathUtils.Determinant(pointOnContour, pointOnContour + sumResultingEdge, pointsOffContour[0]);
//                det2 = MathUtils.Determinant(pointOnContour, pointOnContour + sumResultingEdge, pointsOffContour[1]);

//                if (det1 < 0 && det2 < 0) //both points are on the 'right' of the resulting edge, no intersection
//                    return false;

//                return true; //all other cases lead to a non-null intersection
//            }           
//        }
//    }

//    /**
//     * Does this triangle intersects the edge defined by edgePoint1 and and edgePoint2
//     * **/
//    public bool IntersectsEdge(Vector2 edgePoint1, Vector2 edgePoint2, int bFilters = GeometryUtils.SEGMENTS_OVERLAP | GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT | GeometryUtils.SEGMENTS_STRICT_INTERSECTION)
//    {
//        for (int i = 0; i != 3; i++)
//        {
//            Vector2 point1 = m_points[i];
//            Vector2 point2 = m_points[(i == 2) ? 0 : i + 1];

//            if (GeometryUtils.TwoSegmentsIntersect(point1, point2, edgePoint1, edgePoint2, bFilters))
//                return true;
//        }

//        return false;
//    }

//    /**
//    * Tells if one of the edges of this triangle intersects the contour passed as parameter
//    **/
//    public bool IntersectsOutline(DottedOutline outline, int bFilters = GeometryUtils.SEGMENTS_OVERLAP | GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT | GeometryUtils.SEGMENTS_STRICT_INTERSECTION)
//    {
//        //check if triangle intersect outline main contour
//        Contour contourPoints = outline.m_contour;
//        for (int iContourPointIndex = 0; iContourPointIndex != contourPoints.Count; iContourPointIndex++)
//        {
//            Vector2 contourSegmentPoint1 = contourPoints[iContourPointIndex];
//            Vector2 contourSegmentPoint2 = (iContourPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iContourPointIndex + 1];

//            if (IntersectsEdge(contourSegmentPoint1, contourSegmentPoint2, bFilters))
//                return true;

//            //for (int iTriangleEdgeIndex = 0; iTriangleEdgeIndex != 3; iTriangleEdgeIndex++)
//            //{
//            //    Vector2 triangleEdgePoint1 = m_points[iTriangleEdgeIndex];
//            //    Vector2 triangleEdgePoint2 = (iTriangleEdgeIndex == 2) ? m_points[0] : m_points[iTriangleEdgeIndex + 1];

//            //    if (GeometryUtils.TwoSegmentsIntersect(contourSegmentPoint1, contourSegmentPoint2, triangleEdgePoint1, triangleEdgePoint2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION))
//            //        return true;
//            //}
//        }

//        //check if triangle intersect outline holes
//        List<Contour> outlineHoles = outline.m_holes;
//        for (int i = 0; i != outlineHoles.Count; i++)
//        {
//            Contour hole = outlineHoles[i];
//            Vector2 holeSegmentPoint1 = hole[i];
//            Vector2 holeSegmentPoint2 = (i == hole.Count - 1) ? hole[0] : hole[i + 1];

//            if (IntersectsEdge(holeSegmentPoint1, holeSegmentPoint2, bFilters))
//                return true;
//        }

//        return false;
//    }

//    /**
//    * Tells if one of the edges of this triangle intersects the segment defined by segmentPoint1 and segmentPoint2
//    * **/
//    public bool IntersectsSegment(Vector2 segmentPoint1, Vector2 segmentPoint2)
//    {
//        return GeometryUtils.TwoSegmentsIntersect(m_points[0], m_points[1], segmentPoint1, segmentPoint2) ||
//               GeometryUtils.TwoSegmentsIntersect(m_points[1], m_points[2], segmentPoint1, segmentPoint2) ||
//               GeometryUtils.TwoSegmentsIntersect(m_points[2], m_points[0], segmentPoint1, segmentPoint2);
//    }

//    /**
//    * Tells if one of the edges of this triangle intersects the line defined by linePoint1 and linePoint2
//    * **/
//    public bool IntersectsLine(Vector2 linePoint1, Vector2 linePoint2)
//    {
//        return GeometryUtils.SegmentIntersectsLine(m_points[0], m_points[1], linePoint1, linePoint2) ||
//               GeometryUtils.SegmentIntersectsLine(m_points[1], m_points[2], linePoint1, linePoint2) ||
//               GeometryUtils.SegmentIntersectsLine(m_points[2], m_points[0], linePoint1, linePoint2);
//    }

//    /**
//     * Tells if this triangle contains the triangle passed as parameter
//     * **/
//    public bool ContainsTriangle(BaseTriangle triangle)
//    {
//        return this.ContainsPoint(triangle.m_points[0]) &&
//               this.ContainsPoint(triangle.m_points[1]) &&
//               this.ContainsPoint(triangle.m_points[2]);
//    }

//    /**
//     * Tells if this triangle contains a whole shape
//     * **/
//    public bool ContainsShape(Shape shape)
//    {
//        for (int iTriangleIndex = 0; iTriangleIndex != shape.m_triangles.Count; iTriangleIndex++)
//        {
//            BaseTriangle triangle = shape.m_triangles[iTriangleIndex];
//            if (!this.ContainsTriangle(triangle)) //this triangle does not contains at least one of the shape triangles
//                return false;
//        }

//        return true;
//    }

//    /**
//     * Tells if the parameter point is on this triangle contour.
//     * Return the indices of edges the point is on (max 2 if point is equal to one of the triangle vertices)
//     * **/
//    public int[] ContourContainsPoint(Vector2 point)
//    {
//        int[] edgeIndices;

//        //First check if point equals to one of the triangle vertices
//        if (MathUtils.AreVec2PointsEqual(point, m_points[0]))
//        {
//            edgeIndices = new int[2];
//            edgeIndices[0] = 2;
//            edgeIndices[1] = 0;
//            return edgeIndices;
//        }
//        else if (MathUtils.AreVec2PointsEqual(point, m_points[1]))
//        {
//            edgeIndices = new int[2];
//            edgeIndices[0] = 0;
//            edgeIndices[1] = 1;
//            return edgeIndices;
//        }
//        else if (MathUtils.AreVec2PointsEqual(point, m_points[2]))
//        {
//            edgeIndices = new int[2];
//            edgeIndices[0] = 1;
//            edgeIndices[1] = 2;
//            return edgeIndices;
//        }
        
//        for (int i = 0; i != 3; i++)
//        {
//            Vector2 edgePoint1 = m_points[i];
//            Vector2 edgePoint2 = m_points[(i == 2) ? 0 : i + 1];

//            if (GeometryUtils.IsPointContainedInSegment(point, edgePoint1, edgePoint2))
//            {
//                edgeIndices = new int[1];
//                edgeIndices[0] = i;
//                return edgeIndices;
//            }
//        }

//        return null;
//    }

//    /**
//     * Returns the index of one of the three triangle vertex if the point passed as parameter is equal to it 
//     * Otherwise returns -1
//     * **/
//    public int PointEqualsVertex(Vector2 point)
//    {
//        if (MathUtils.AreVec2PointsEqual(point, m_points[0]))
//            return 0;
//        else if (MathUtils.AreVec2PointsEqual(point, m_points[1]))
//            return 1;
//        else if (MathUtils.AreVec2PointsEqual(point, m_points[2]))
//            return 2;

//        return -1;
//    }

//    /**
//     * Calculates the area of the triangle
//     * **/
//    public float GetArea()
//    {
//        return 0.5f * Mathf.Abs(MathUtils.Determinant(m_points[0], m_points[1], m_points[2]));
//    }

//    /**
//     * Finds intersections between a line and the edges of this triangle
//     * **/
//    public List<Vector2> FindIntersectionsWithLine(Vector2 linePoint1, Vector2 linePoint2)
//    {
//        List<Vector2> intersections = new List<Vector2>();
//        intersections.Capacity = 2;

//        Vector2 lineDirection = linePoint2 - linePoint1;

//        Vector2 intersection;
//        bool intersects;
//        GeometryUtils.SegmentLineIntersection(m_points[0], m_points[1], linePoint1, lineDirection, out intersection, out intersects);
//        if (intersects)
//            intersections.Add(intersection);

//        GeometryUtils.SegmentLineIntersection(m_points[1], m_points[2], linePoint1, lineDirection, out intersection, out intersects);
//        if (intersects)
//        {
//            if (intersections.Count == 0
//                ||
//                intersections.Count == 1 && !MathUtils.AreVec2PointsEqual(intersection, intersections[0]))
//                intersections.Add(intersection);
//        }


//        if (intersections.Count < 2)
//        {
//            GeometryUtils.SegmentLineIntersection(m_points[2], m_points[0], linePoint1, lineDirection, out intersection, out intersects);
//            if (intersects)
//                intersections.Add(intersection);
//        }

//        return intersections;
//    }
//}


/**
 * A triangle with GridPoint vertices
 * **/
public class GridTriangle
{
    public GridPoint[] m_points { get; set; }

    public GridTriangle()
    {
        m_points = new GridPoint[3];
    }

    public GridTriangle(GridTriangle other)
        : this()
    {
        for (int i = 0; i != 3; i++)
        {
            m_points[i] = other.m_points[i];
        }
    }

    public GridPoint GetCenter()
    {
        return (m_points[0] + m_points[1] + m_points[2]) / 3.0f;
    }

    public bool HasVertex(GridPoint vertex)
    {
        return m_points[0] == vertex || m_points[1] == vertex || m_points[2] == vertex;
    }

    /**
     * Determines if a point is inside this triangle
     * Depending on the vertices order (clockwise or counter-clockwise) the signs of det1, det2 and det3 can be positive or negative
     * so we just check if they are of the same sign
     * **/
    public bool ContainsPoint(GridPoint point)
    {
        //test if point is one of the 3 triangle vertices
        if (HasVertex(point))
            return true;

        long det1 = MathUtils.Determinant(m_points[0], m_points[1], point);
        long det2 = MathUtils.Determinant(m_points[1], m_points[2], point);
        long det3 = MathUtils.Determinant(m_points[2], m_points[0], point);

        return (det1 <= 0 && det2 <= 0 && det3 <= 0) || (det1 >= 0 && det2 >= 0 && det3 >= 0);
    }

    /**
     * Return true if this triangle intersects the parameter 'triangle'
     * This intersection can have a null area (triangles just share points or portions of edges for instance)
     * To test if this intersection is effectively null, use the method IntersectTriangleWithNonNullIntersection()
     * **/
    public bool IntersectsTriangle(GridTriangle triangle)
    {
        GridEdge edge1 = new GridEdge(triangle.m_points[0], triangle.m_points[1]);
        GridEdge edge2 = new GridEdge(triangle.m_points[1], triangle.m_points[2]);
        GridEdge edge3 = new GridEdge(triangle.m_points[2], triangle.m_points[0]);

        bool bIntersectEdges = IntersectsEdge(edge1) || IntersectsEdge(edge2) || IntersectsEdge(edge3);

        //Test if the triangle is contained inside the other
        if (!bIntersectEdges)
            return this.ContainsPoint(triangle.GetCenter());
        else 
            return true;
    }

    public bool IntersectsTriangleWithNonNullIntersection(GridTriangle triangle)
    {
        if (!IntersectsTriangle(triangle)) //no intersection at all, return false
            return false;

        return IsIntersectionWithTriangleNull(triangle);
    }

    private bool IsIntersectionWithTriangleNull(GridTriangle triangle)
    {
        int[] point1OnContourEdgeIndices = ContourContainsPoint(triangle.m_points[0]);
        int[] point2OnContourEdgeIndices = ContourContainsPoint(triangle.m_points[1]);
        int[] point3OnContourEdgeIndices = ContourContainsPoint(triangle.m_points[2]);

        int pointsOnContourCount = 0;
        if (point1OnContourEdgeIndices != null) pointsOnContourCount++;
        if (point2OnContourEdgeIndices != null) pointsOnContourCount++;
        if (point3OnContourEdgeIndices != null) pointsOnContourCount++;            

        if (pointsOnContourCount == 0) 
        {
            //if the second triangle does not contain any of the first triangle vertices on its contour then there is a non-null intersection
            return (triangle.ContourContainsPoint(m_points[0]) == null &&
                    triangle.ContourContainsPoint(m_points[1]) == null &&
                    triangle.ContourContainsPoint(m_points[2]) == null);

        }
        else if (pointsOnContourCount == 3) //all points are on the contour of the other triangle so the first triangle is contained in the second triangle
            return true;
        else if (pointsOnContourCount == 2)
        {
            //test if points are equal to one of the triangle vertices
            bool bOnVertex1 = false;
            bool bOnVertex2 = false;
            bool bOnVertex3 = false;
            if (point1OnContourEdgeIndices != null && point1OnContourEdgeIndices.Length == 2)
            {
                if (point1OnContourEdgeIndices[0] == 2 && point1OnContourEdgeIndices[1] == 0)
                    bOnVertex1 = true;
                else if (point1OnContourEdgeIndices[0] == 0 && point1OnContourEdgeIndices[1] == 1)
                    bOnVertex2 = true;
                else if (point1OnContourEdgeIndices[0] == 1 && point1OnContourEdgeIndices[1] == 2)
                    bOnVertex3 = true;
            }
            if (point2OnContourEdgeIndices != null && point2OnContourEdgeIndices.Length == 2)
            {
                if (point2OnContourEdgeIndices[0] == 2 && point2OnContourEdgeIndices[1] == 0)
                    bOnVertex1 = true;
                else if (point2OnContourEdgeIndices[0] == 0 && point2OnContourEdgeIndices[1] == 1)
                    bOnVertex2 = true;
                else if (point2OnContourEdgeIndices[0] == 1 && point2OnContourEdgeIndices[1] == 2)
                    bOnVertex3 = true;
            }
            if (point3OnContourEdgeIndices != null && point3OnContourEdgeIndices.Length == 2)
            {
                if (point3OnContourEdgeIndices[0] == 2 && point3OnContourEdgeIndices[1] == 0)
                    bOnVertex1 = true;
                else if (point3OnContourEdgeIndices[0] == 0 && point3OnContourEdgeIndices[1] == 1)
                    bOnVertex2 = true;
                else if (point3OnContourEdgeIndices[0] == 1 && point3OnContourEdgeIndices[1] == 2)
                    bOnVertex3 = true;
            }

            //Count points that are strictly on an edge
            int bOnEdge1Count = 0;
            int bOnEdge2Count = 0;
            int bOnEdge3Count = 0;
            if (point1OnContourEdgeIndices != null && point1OnContourEdgeIndices.Length == 1)
            {
                if (point1OnContourEdgeIndices[0] == 0)
                    bOnEdge1Count++;
                else if (point1OnContourEdgeIndices[0] == 1)
                    bOnEdge2Count++;
                else if (point1OnContourEdgeIndices[0] == 2)
                    bOnEdge3Count++;
            }
            if (point2OnContourEdgeIndices != null && point2OnContourEdgeIndices.Length == 1)
            {
                if (point2OnContourEdgeIndices[0] == 0)
                    bOnEdge1Count++;
                else if (point2OnContourEdgeIndices[0] == 1)
                    bOnEdge2Count++;
                else if (point2OnContourEdgeIndices[0] == 2)
                    bOnEdge3Count++;
            }
            if (point3OnContourEdgeIndices != null && point3OnContourEdgeIndices.Length == 1)
            {
                if (point3OnContourEdgeIndices[0] == 0)
                    bOnEdge1Count++;
                else if (point3OnContourEdgeIndices[0] == 1)
                    bOnEdge2Count++;
                else if (point3OnContourEdgeIndices[0] == 2)
                    bOnEdge3Count++;
            }

            //Find the index of the edge the two points share. If they are on different edges there is a non-null intersection
            int sharedEdgeIndex;
            if (bOnVertex1 && bOnVertex2)
                sharedEdgeIndex = 0;
            else if (bOnVertex2 && bOnVertex3)
                sharedEdgeIndex = 1;
            else if (bOnVertex1 && bOnVertex3)
                sharedEdgeIndex = 2;
            else if ((bOnVertex1 || bOnVertex2) && bOnEdge1Count == 1)
                sharedEdgeIndex = 0;
            else if ((bOnVertex2 || bOnVertex3) && bOnEdge2Count == 1)
                sharedEdgeIndex = 1;
            else if ((bOnVertex3 || bOnVertex1) && bOnEdge3Count == 1)
                sharedEdgeIndex = 2;
            else if (bOnEdge1Count == 2)
                sharedEdgeIndex = 0;
            else if (bOnEdge2Count == 2)
                sharedEdgeIndex = 1;
            else if (bOnEdge3Count == 2)
                sharedEdgeIndex = 2;
            else //one point on a different edge each time ==> non-null intersection
                return true;

            //Find the point that is not on the contour
            GridPoint offContourPoint;
            if (point1OnContourEdgeIndices == null)
                offContourPoint = triangle.m_points[0];
            else if (point2OnContourEdgeIndices == null)
                offContourPoint = triangle.m_points[1];
            else
                offContourPoint = triangle.m_points[2];

            //Determine the position (left or right) of the off contour point about the edge set previously
            GridPoint edgePoint1 = m_points[sharedEdgeIndex];
            GridPoint edgePoint2 = m_points[(sharedEdgeIndex == 2) ? 0 : sharedEdgeIndex + 1];
            float det = MathUtils.Determinant(edgePoint1, edgePoint2, offContourPoint);
            return (det > 0); //'left'
        }
        else //(pointsOnContourCount == 1)
        {
            //Find the index of the point that is on this triangle contour
            GridPoint pointOnContour;
            GridPoint[] pointsOffContour = new GridPoint[2];
            int[] edgeIndices;
            if (point1OnContourEdgeIndices != null)
            {
                pointOnContour = triangle.m_points[0];
                pointsOffContour[0] = triangle.m_points[1];
                pointsOffContour[1] = triangle.m_points[2];
                edgeIndices = point1OnContourEdgeIndices;
            }
            else if (point2OnContourEdgeIndices != null)
            {
                pointOnContour = triangle.m_points[1];
                pointsOffContour[0] = triangle.m_points[0];
                pointsOffContour[1] = triangle.m_points[2];
                edgeIndices = point2OnContourEdgeIndices;
            }
            else
            {
                pointOnContour = triangle.m_points[2];
                pointsOffContour[0] = triangle.m_points[0];
                pointsOffContour[1] = triangle.m_points[1];
                edgeIndices = point3OnContourEdgeIndices;
            }

            //test if the point is equal to one of the triangle vertices
            if (edgeIndices.Length == 1)
            {
                int edgeIndex = edgeIndices[0];
                GridPoint edgePoint1 = m_points[edgeIndex];
                GridPoint edgePoint2 = m_points[edgeIndex == 2 ? 0 : edgeIndex + 1];

                //Check if at least one of the two points off contour is on the 'left' of the edge
                float det = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[0]);
                if (det > 0) return true; //'left'
                det = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[1]);
                return (det > 0); //'left'
            }
            else // edgeIndices.Length == 2 (point is equal to one of the triangle vertices)
            {
                //check if both off contour points are on the left or the right of the first edge
                int edgeIndex = edgeIndices[0];
                GridPoint edgePoint1 = m_points[edgeIndex];
                GridPoint edgePoint2 = m_points[edgeIndex == 2 ? 0 : edgeIndex + 1];
                float det1 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[0]);
                float det2 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[1]);

                if (det1 < 0 && det2 < 0) //both points are on the 'right' of the edge, no intersection
                    return false;

                //check if both off contour points are on the left or the right of the second edge
                edgeIndex = edgeIndices[1];
                edgePoint1 = m_points[edgeIndex];
                edgePoint2 = m_points[edgeIndex == 2 ? 0 : edgeIndex + 1];
                det1 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[0]);
                det2 = MathUtils.Determinant(edgePoint1, edgePoint2, pointsOffContour[1]);

                if (det1 < 0 && det2 < 0) //both points are on the 'right' of the edge, no intersection
                    return false;

                //check if both off contour points are on the left or the right of line directed by the sum of first and second edges
                GridPoint firstEdgePoint1 = m_points[edgeIndices[0]];
                GridPoint firstEdgePoint2 = m_points[edgeIndices[0] == 2 ? 0 : edgeIndices[0] + 1];
                GridPoint secondEdgePoint1 = m_points[edgeIndices[1]];
                GridPoint secondEdgePoint2 = m_points[edgeIndices[1] == 2 ? 0 : edgeIndices[1] + 1];
                GridPoint firstEdge = firstEdgePoint2 - firstEdgePoint1;
                GridPoint secondEdge = secondEdgePoint2 - secondEdgePoint1;

                GridPoint sumResultingEdge = firstEdge + secondEdge;

                det1 = MathUtils.Determinant(pointOnContour, pointOnContour + sumResultingEdge, pointsOffContour[0]);
                det2 = MathUtils.Determinant(pointOnContour, pointOnContour + sumResultingEdge, pointsOffContour[1]);

                if (det1 < 0 && det2 < 0) //both points are on the 'right' of the resulting edge, no intersection
                    return false;

                return true; //all other cases lead to a non-null intersection
            }           
        }
    }

    /**
     * Does this triangle intersects the edge defined by edgePoint1 and and edgePoint2
     * **/
    public bool IntersectsEdge(GridEdge edge, int bFilters = GeometryUtils.SEGMENTS_OVERLAP | GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT | GeometryUtils.SEGMENTS_STRICT_INTERSECTION)
    {
        for (int i = 0; i != 3; i++)
        {
            GridPoint point1 = m_points[i];
            GridPoint point2 = m_points[(i == 2) ? 0 : i + 1];
            GridEdge triangleEdge = new GridEdge(point1, point2);

            if (edge.IntersectsEdge(triangleEdge, bFilters))
                return true;
        }

        return false;
    }

    /**
    * Tells if one of the edges of this triangle intersects the contour passed as parameter
    **/
    public bool IntersectsOutline(DottedOutline outline, int bFilters = GeometryUtils.SEGMENTS_OVERLAP | GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT | GeometryUtils.SEGMENTS_STRICT_INTERSECTION)
    {
        //check if triangle intersect outline main contour
        Contour contourPoints = outline.m_contour;
        for (int iContourPointIndex = 0; iContourPointIndex != contourPoints.Count; iContourPointIndex++)
        {
            GridPoint contourSegmentPoint1 = contourPoints[iContourPointIndex];
            GridPoint contourSegmentPoint2 = (iContourPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iContourPointIndex + 1];

            if (IntersectsEdge(new GridEdge(contourSegmentPoint1, contourSegmentPoint2), bFilters))
                return true;
        }

        //check if triangle intersect outline holes
        List<Contour> outlineHoles = outline.m_holes;
        for (int i = 0; i != outlineHoles.Count; i++)
        {
            Contour hole = outlineHoles[i];
            GridPoint holeSegmentPoint1 = hole[i];
            GridPoint holeSegmentPoint2 = (i == hole.Count - 1) ? hole[0] : hole[i + 1];

            if (IntersectsEdge(new GridEdge(holeSegmentPoint1, holeSegmentPoint2), bFilters))
                return true;
        }

        return false;
    }

    /**
     * Tells if this triangle contains the triangle passed as parameter
     * **/
    //public bool ContainsTriangle(GridTriangle triangle)
    //{
    //    return this.ContainsPoint(triangle.m_points[0]) &&
    //           this.ContainsPoint(triangle.m_points[1]) &&
    //           this.ContainsPoint(triangle.m_points[2]);
    //}

    /**
     * Tells if this triangle contains a whole shape
     * **/
    //public bool ContainsShape(Shape shape)
    //{
    //    for (int iTriangleIndex = 0; iTriangleIndex != shape.m_triangles.Count; iTriangleIndex++)
    //    {
    //        GridTriangle triangle = shape.m_triangles[iTriangleIndex];
    //        if (!this.ContainsTriangle(triangle)) //this triangle does not contains at least one of the shape triangles
    //            return false;
    //    }

    //    return true;
    //}

    /**
     * Tells if the parameter point is on this triangle contour.
     * Return the indices of edges the point is on (max 2 if point is equal to one of the triangle vertices)
     * **/
    public int[] ContourContainsPoint(GridPoint point)
    {
        int[] edgeIndices;

        //First check if point equals to one of the triangle vertices
        if (MathUtils.AreVec2PointsEqual(point, m_points[0]))
        {
            edgeIndices = new int[2];
            edgeIndices[0] = 2;
            edgeIndices[1] = 0;
            return edgeIndices;
        }
        else if (MathUtils.AreVec2PointsEqual(point, m_points[1]))
        {
            edgeIndices = new int[2];
            edgeIndices[0] = 0;
            edgeIndices[1] = 1;
            return edgeIndices;
        }
        else if (MathUtils.AreVec2PointsEqual(point, m_points[2]))
        {
            edgeIndices = new int[2];
            edgeIndices[0] = 1;
            edgeIndices[1] = 2;
            return edgeIndices;
        }
        
        for (int i = 0; i != 3; i++)
        {
            GridPoint edgePoint1 = m_points[i];
            GridPoint edgePoint2 = m_points[(i == 2) ? 0 : i + 1];
            GridEdge edge = new GridEdge(edgePoint1, edgePoint2);

            if (edge.ContainsPoint(point))
            {
                edgeIndices = new int[1];
                edgeIndices[0] = i;
                return edgeIndices;
            }
        }

        return null;
    }

    /**
     * Calculates the area of the triangle
     * **/
    public float GetArea()
    {
        return 0.5f * Mathf.Abs(MathUtils.Determinant(m_points[0], m_points[1], m_points[2]));
    }

    /**
     * Translate this triangle
     * **/
    public void Translate(GridPoint translation)
    {
        m_points[0] += translation;
        m_points[1] += translation;
        m_points[2] += translation;
    }

    /**
     * Finds intersections between a line and the edges of this triangle
     * **/
    //public List<Vector2> FindIntersectionsWithLine(Vector2 linePoint1, Vector2 linePoint2)
    //{
    //    List<Vector2> intersections = new List<Vector2>();
    //    intersections.Capacity = 2;

    //    Vector2 lineDirection = linePoint2 - linePoint1;

    //    Vector2 intersection;
    //    bool intersects;
    //    GeometryUtils.SegmentLineIntersection(m_points[0], m_points[1], linePoint1, lineDirection, out intersection, out intersects);
    //    if (intersects)
    //        intersections.Add(intersection);

    //    GeometryUtils.SegmentLineIntersection(m_points[1], m_points[2], linePoint1, lineDirection, out intersection, out intersects);
    //    if (intersects)
    //    {
    //        if (intersections.Count == 0
    //            ||
    //            intersections.Count == 1 && !MathUtils.AreVec2PointsEqual(intersection, intersections[0]))
    //            intersections.Add(intersection);
    //    }


    //    if (intersections.Count < 2)
    //    {
    //        GeometryUtils.SegmentLineIntersection(m_points[2], m_points[0], linePoint1, lineDirection, out intersection, out intersects);
    //        if (intersects)
    //            intersections.Add(intersection);
    //    }

    //    return intersections;
    //}
}

/**
 * Triangle that belongs to a Shape
 * **/
//public class ShapeTriangle : BaseTriangle
//{
//    public Shape m_parentShape { get; set; }
//    public Color m_color { get; set; }

//    public ShapeTriangle(Shape parentShape = null) : base()
//    {
//        m_parentShape = parentShape;       
//    }

//    public ShapeTriangle(Shape parentShape, Color color) : base()
//    {
//        m_parentShape = parentShape;
//        m_color = color;
//    }

//    /**
//     * Splits this triangle intersected by a line
//     * **/
//    public void Split(Vector2 linePoint1, Vector2 linePoint2, out ShapeTriangle[] splitTriangles, out int splitTrianglesCount)
//    {
//        splitTriangles = new ShapeTriangle[3];
//        splitTrianglesCount = 0;

//        List<Vector2> intersections = FindIntersectionsWithLine(linePoint1, linePoint2);

//        if (intersections.Count != 2)
//            return;

//        int intersection1IsTriangleVertex = PointEqualsVertex(intersections[0]);
//        int intersection2IsTriangleVertex = PointEqualsVertex(intersections[1]);
//        if (intersection1IsTriangleVertex >= 0 || intersection2IsTriangleVertex >= 0) //one of the intersection is equal to a triangle vertex
//        {
//            splitTriangles[0] = new ShapeTriangle(this.m_parentShape);
//            splitTriangles[1] = new ShapeTriangle(this.m_parentShape);
//            splitTrianglesCount = 2;

//            if (intersection1IsTriangleVertex >= 0)
//            {
//                if (intersection1IsTriangleVertex == 0)
//                {
//                    splitTriangles[0].m_points[0] = intersections[0];
//                    splitTriangles[0].m_points[1] = m_points[1];
//                    splitTriangles[0].m_points[2] = intersections[1];

//                    splitTriangles[1].m_points[0] = intersections[0];
//                    splitTriangles[1].m_points[1] = intersections[1];
//                    splitTriangles[1].m_points[2] = m_points[2];
//                }
//                else if (intersection1IsTriangleVertex == 1)
//                {
//                    splitTriangles[0].m_points[0] = intersections[0];
//                    splitTriangles[0].m_points[1] = m_points[2];
//                    splitTriangles[0].m_points[2] = intersections[1];

//                    splitTriangles[1].m_points[0] = m_points[0];
//                    splitTriangles[1].m_points[1] = intersections[0];
//                    splitTriangles[1].m_points[2] = intersections[1];
//                }
//                else if (intersection1IsTriangleVertex == 2)
//                {
//                    splitTriangles[0].m_points[0] = intersections[0];
//                    splitTriangles[0].m_points[1] = m_points[0];
//                    splitTriangles[0].m_points[2] = intersections[1];

//                    splitTriangles[1].m_points[0] = intersections[0];
//                    splitTriangles[1].m_points[1] = intersections[1];
//                    splitTriangles[1].m_points[2] = m_points[1];
//                }
//            }
//            else
//            {
//                if (intersection2IsTriangleVertex == 0)
//                {
//                    splitTriangles[0].m_points[0] = intersections[1];
//                    splitTriangles[0].m_points[1] = m_points[1];
//                    splitTriangles[0].m_points[2] = intersections[0];

//                    splitTriangles[1].m_points[0] = intersections[1];
//                    splitTriangles[1].m_points[1] = intersections[0];
//                    splitTriangles[1].m_points[2] = m_points[2];
//                }
//                else if (intersection2IsTriangleVertex == 1)
//                {
//                    splitTriangles[0].m_points[0] = intersections[1];
//                    splitTriangles[0].m_points[1] = m_points[2];
//                    splitTriangles[0].m_points[2] = intersections[0];

//                    splitTriangles[1].m_points[0] = m_points[0];
//                    splitTriangles[1].m_points[1] = intersections[1];
//                    splitTriangles[1].m_points[2] = intersections[0];
//                }
//                else if (intersection2IsTriangleVertex == 2)
//                {
//                    splitTriangles[0].m_points[0] = intersections[1];
//                    splitTriangles[0].m_points[1] = m_points[0];
//                    splitTriangles[0].m_points[2] = intersections[0];

//                    splitTriangles[1].m_points[0] = intersections[1];
//                    splitTriangles[1].m_points[1] = intersections[0];
//                    splitTriangles[1].m_points[2] = m_points[1];
//                }
//            }
//        }
//        else //intersections are strictly inside edges
//        {
//            splitTriangles[0] = new ShapeTriangle(this.m_parentShape);
//            splitTriangles[1] = new ShapeTriangle(this.m_parentShape);
//            splitTriangles[2] = new ShapeTriangle(this.m_parentShape);
//            splitTrianglesCount = 3;

//            //find edges on which intersection points are on
//            int[] intersectionEdgesNumber = new int[2];

//            bool isEdge1Intersected = false;
//            bool isEdge2Intersected = false;
//            //find where the first intersection is
//            isEdge1Intersected = GeometryUtils.IsPointContainedInSegment(intersections[0], m_points[0], m_points[1]);
//            if (isEdge1Intersected)
//                intersectionEdgesNumber[0] = 1; //intersection is on the first edge of the triangle
//            else
//            {
//                isEdge2Intersected = GeometryUtils.IsPointContainedInSegment(intersections[0], m_points[1], m_points[2]);
//                if (isEdge2Intersected)
//                    intersectionEdgesNumber[0] = 2; //intersection is on the second edge of the triangle
//                else
//                    intersectionEdgesNumber[0] = 3; //intersection is on the third edge of the triangle
//            }

//            //find where the second intersection is
//            if (!isEdge1Intersected)
//            {
//                isEdge1Intersected = GeometryUtils.IsPointContainedInSegment(intersections[1], m_points[0], m_points[1]);
//                if (isEdge1Intersected)
//                    intersectionEdgesNumber[1] = 1; //intersection is on the first edge of the triangle
//                else
//                {
//                    if (isEdge2Intersected)
//                        intersectionEdgesNumber[1] = 3; //intersection is on the third edge of the triangle
//                    else
//                        intersectionEdgesNumber[1] = 2; //intersection is on the second edge of the triangle
//                }
//            }
//            else
//            {
//                isEdge2Intersected = GeometryUtils.IsPointContainedInSegment(intersections[1], m_points[1], m_points[2]);
//                if (isEdge2Intersected)
//                    intersectionEdgesNumber[1] = 2; //intersection is on the second edge of the triangle
//                else
//                    intersectionEdgesNumber[1] = 3; //intersection is on the third edge of the triangle
//            }

//            if (intersectionEdgesNumber[1] < intersectionEdgesNumber[0])
//            {
//                Vector2 tmpIntersection = intersections[0];
//                intersections[0] = intersections[1];
//                intersections[1] = tmpIntersection;

//                int tmpEdgeNumber = intersectionEdgesNumber[0];
//                intersectionEdgesNumber[1] = intersectionEdgesNumber[0];
//                intersectionEdgesNumber[0] = tmpEdgeNumber;
//            }

            
//            if (intersectionEdgesNumber[0] == 1 && intersectionEdgesNumber[1] == 2)
//            {
//                splitTriangles[0].m_points[0] = intersections[0];
//                splitTriangles[0].m_points[1] = m_points[1];
//                splitTriangles[0].m_points[2] = intersections[1];

//                splitTriangles[1].m_points[0] = intersections[0];
//                splitTriangles[1].m_points[1] = intersections[1];
//                splitTriangles[1].m_points[2] = m_points[2];

//                splitTriangles[2].m_points[0] = intersections[0];
//                splitTriangles[2].m_points[1] = m_points[2];
//                splitTriangles[2].m_points[2] = m_points[0];
//            }
//            else if (intersectionEdgesNumber[0] == 1 && intersectionEdgesNumber[1] == 3)
//            {
//                splitTriangles[0].m_points[0] = m_points[0];
//                splitTriangles[0].m_points[1] = intersections[0];
//                splitTriangles[0].m_points[2] = intersections[1];

//                splitTriangles[1].m_points[0] = intersections[0];
//                splitTriangles[1].m_points[1] = m_points[1];
//                splitTriangles[1].m_points[2] = m_points[2];

//                splitTriangles[2].m_points[0] = intersections[0];
//                splitTriangles[2].m_points[1] = m_points[2];
//                splitTriangles[2].m_points[2] = intersections[1];
//            }
//            else if (intersectionEdgesNumber[0] == 2 && intersectionEdgesNumber[1] == 3)
//            {
//                splitTriangles[0].m_points[0] = m_points[0];
//                splitTriangles[0].m_points[1] = m_points[1];
//                splitTriangles[0].m_points[2] = intersections[0];

//                splitTriangles[1].m_points[0] = intersections[0];
//                splitTriangles[1].m_points[1] = intersections[1];
//                splitTriangles[1].m_points[2] = m_points[0];

//                splitTriangles[2].m_points[0] = intersections[0];
//                splitTriangles[2].m_points[1] = m_points[2];
//                splitTriangles[2].m_points[2] = intersections[1];
//            }
//        }
//    }
//}

/**
 * Triangle used for drawing fancy backgrounds
 * It has a front and a back face with 2 different colors
 * **/
public class BackgroundTriangle
{
    public Vector2[] m_points;

    public Color m_color { get; set; } //the color of the front face of this triangle (the 3 vertices share the same color)
    public Color m_originalColor { get; set; } //the color that this triangle face should have before being offset to m_frontColor
    //private Vector3 m_flipAxis; //the axis the triangle is rotating around
    //private float m_flipAngle; //the angle the triangle is rotated along its flip axis
    public int m_indexInColumn { get; set; }
    public BackgroundTriangleColumn m_parentColumn { get; set; }
    public float m_edgeLength { get; set; }
    public float m_angle { get; set; } //the angle this triangle is rotated
    public float m_contourThickness { get; set; }

    //neighbors of this triangle;
    private BackgroundTriangle m_edge1Neighbor;
    private BackgroundTriangle m_edge2Neighbor;
    private BackgroundTriangle m_edge3Neighbor;

    //Variables to handle color animation of this triangle
    private Color m_animationStartColor; //the color when the animation starts
    private Color m_animationEndColor; //the color when the animation ends
    private bool m_colorAnimating;//is this triangle currently animating its own color when switching its status
    private float m_colorAnimationElapsedTime;
    private float m_colorAnimationDuration;
    private float m_colorAnimationDelay;

    public const float EDGE_COLOR_INTENSIFY_FACTOR = 20.0f;

    /**
     * Build an equilateral triangle with the given orientation passed through the angle variable
     * The triangle can have an inner contour, in this case set the contourThickness to a strictly positive value
     * **/
    public BackgroundTriangle(Vector2 position, float edgeLength, float angle, Color color, float contourThickness = 0)
    {
        m_points = new Vector2[3];

        m_edgeLength = edgeLength;
        m_angle = angle;
        m_contourThickness = contourThickness;

        //the 3 bisectors of this triangle
        Vector2 bisector0 = new Vector2(1, 0); //angle 0
        Vector2 bisector1 = new Vector2(-0.5f, -Mathf.Sqrt(3) / 2); //angle -2 * PI / 3
        Vector2 bisector2 = new Vector2(-0.5f, Mathf.Sqrt(3) / 2); //angle 2 * PI / 3

        float H = Mathf.Sqrt(3) / 2 * edgeLength;
        float maxContourThickness = 1 / 3.0f * H;
        if (contourThickness > maxContourThickness) //contour so large that this triangle is formed by 3 colored triangles, remove this case
        {
            throw new System.Exception("Contour is bigger than 1/3 of the bisector");
        }
        else
        {
            if (contourThickness == 0) //no contour, this case should not happen but deal with it anyway
            {
                m_points[0] = 2 / 3.0f * H * bisector0;
                m_points[1] = 2 / 3.0f * H * bisector1;
                m_points[2] = 2 / 3.0f * H * bisector2;
            }
            else //we have a valid inner contour
            {
                m_points = new Vector2[6];

                //outer vertices
                m_points[0] = 2 / 3.0f * H * bisector0;
                m_points[1] = 2 / 3.0f * H * bisector1;
                m_points[2] = 2 / 3.0f * H * bisector2;

                //inner vertices
                m_points[3] = m_points[0] - 2 * contourThickness / Mathf.Sqrt(3) * bisector0;
                m_points[4] = m_points[1] - 2 * contourThickness / Mathf.Sqrt(3) * bisector1;
                m_points[5] = m_points[2] - 2 * contourThickness / Mathf.Sqrt(3) * bisector2;
            }
        }              

        if (angle != 0)
        {
            for (int i = 0; i != m_points.Length; i++)
            {
                m_points[i] = Quaternion.AngleAxis(angle, Vector3.forward) * m_points[i];
            }
        }

        //Switch to global position
        for (int i = 0; i != m_points.Length; i++)
        {
            m_points[i] += position;
        }

        m_originalColor = color;
        m_color = color;
    }

    /**
     * Return the center of this triangle
     * **/
    public Vector2 GetCenter()
    {
        return (m_points[0] + m_points[1] + m_points[2]) / 3.0f;
    }

    /**
     * Set the color of this triangle and update neighbors edges colors
     * **/
    public void SetColor(Color color)
    {
        m_color = color;
        SetInnerTriangleColor(color);


        //recalculate the color of the edge separating the two neighbors
        this.InvalidateEdge1Color();
        if (m_edge1Neighbor != null)
            m_edge1Neighbor.InvalidateEdge1Color();

        //recalculate the color of the edge separating the two neighbors
        this.InvalidateEdge2Color();
        if (m_edge2Neighbor != null)
            m_edge2Neighbor.InvalidateEdge2Color();

        //recalculate the color of the edge separating the two neighbors
        this.InvalidateEdge3Color();
        if (m_edge3Neighbor != null)
            m_edge3Neighbor.InvalidateEdge3Color();

        m_parentColumn.m_parentRenderer.m_meshColorsDirty = true;
    }

    /**
     * Set the triangle that share this triangle edge 1 as a neighbor
     * **/
    public void SetEdge1Neighbor(BackgroundTriangle neighbor)
    {
        m_edge1Neighbor = neighbor;
        InvalidateEdge1Color();
    }

    /**
     * Set the triangle that share this triangle edge 2 as a neighbor
     * **/
    public void SetEdge2Neighbor(BackgroundTriangle neighbor)
    {
        m_edge2Neighbor = neighbor;
        InvalidateEdge2Color();
    }

    /**
     * Set the triangle that share this triangle edge 3 as a neighbor
     * **/
    public void SetEdge3Neighbor(BackgroundTriangle neighbor)
    {
        m_edge3Neighbor = neighbor;
        InvalidateEdge3Color();
    }

    /**
     * Recalculate the color of the edge1 based on the color of the relevant neighbor
     * **/
    private void InvalidateEdge1Color()
    {
        Color edgeColor;
        if (m_edge1Neighbor != null)
        {
            float rDist = Mathf.Abs(m_edge1Neighbor.m_color.r - this.m_color.r);
            float gDist = Mathf.Abs(m_edge1Neighbor.m_color.g - this.m_color.g);
            float bDist = Mathf.Abs(m_edge1Neighbor.m_color.b - this.m_color.b);
            edgeColor = 0.5f * (m_edge1Neighbor.m_color + this.m_color);

            edgeColor = ColorUtils.IntensifyColorChannels(edgeColor,
                                                          true,
                                                          true,
                                                          true,
                                                          Mathf.Clamp(rDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.r, 0, 1),
                                                          Mathf.Clamp(gDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.g, 0, 1),
                                                          Mathf.Clamp(bDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.b, 0, 1));
        }
        else
            edgeColor = m_color; //set the edge color the same as the inner triangle

        SetEdge1Color(edgeColor);
    }

    /**
     * Recalculate the color of the edge2 based on the color of the relevant neighbor
     * **/
    private void InvalidateEdge2Color()
    {
        Color edgeColor;
        if (m_edge2Neighbor != null)
        {
            float rDist = Mathf.Abs(m_edge2Neighbor.m_color.r - this.m_color.r);
            float gDist = Mathf.Abs(m_edge2Neighbor.m_color.g - this.m_color.g);
            float bDist = Mathf.Abs(m_edge2Neighbor.m_color.b - this.m_color.b);
            edgeColor = 0.5f * (m_edge2Neighbor.m_color + this.m_color);

            edgeColor = ColorUtils.IntensifyColorChannels(edgeColor,
                                                          true,
                                                          true,
                                                          true,
                                                          Mathf.Clamp(rDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.r, 0, 1),
                                                          Mathf.Clamp(gDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.g, 0, 1),
                                                          Mathf.Clamp(bDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.b, 0, 1));
        }
        else
            edgeColor = m_color; //set the edge color the same as the inner triangle

        SetEdge2Color(edgeColor);
    }

    /**
     * Recalculate the color of the edge3 based on the color of the relevant neighbor
     * **/
    private void InvalidateEdge3Color()
    {
        Color edgeColor;
        if (m_edge3Neighbor != null)
        {
            float rDist = Mathf.Abs(m_edge3Neighbor.m_color.r - this.m_color.r);
            float gDist = Mathf.Abs(m_edge3Neighbor.m_color.g - this.m_color.g);
            float bDist = Mathf.Abs(m_edge3Neighbor.m_color.b - this.m_color.b);
            edgeColor = 0.5f * (m_edge3Neighbor.m_color + this.m_color);

            edgeColor = ColorUtils.IntensifyColorChannels(edgeColor,
                                                          true,
                                                          true,
                                                          true,
                                                          Mathf.Clamp(rDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.r, 0, 1),
                                                          Mathf.Clamp(gDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.g, 0, 1),
                                                          Mathf.Clamp(bDist * EDGE_COLOR_INTENSIFY_FACTOR * edgeColor.b, 0, 1));
        }
        else
            edgeColor = m_color; //set the edge color the same as the inner triangle

        SetEdge3Color(edgeColor);
    }

    /**
     * Modify the mesh colors arrays to apply the edge 1 color
     * **/
    public void SetEdge1Color(Color color)
    {
        BackgroundTrianglesRenderer parentRenderer = m_parentColumn.m_parentRenderer;

        int edge1FirstVerticesArrayIndex = parentRenderer.GetTriangleFirstVerticesArrayIndex(m_parentColumn.m_index, this.m_indexInColumn);
        parentRenderer.m_meshColors[edge1FirstVerticesArrayIndex] = color;
        parentRenderer.m_meshColors[edge1FirstVerticesArrayIndex + 1] = color;
        parentRenderer.m_meshColors[edge1FirstVerticesArrayIndex + 2] = color;
        parentRenderer.m_meshColors[edge1FirstVerticesArrayIndex + 3] = color;
    }

    /**
     * Modify the mesh colors arrays to apply the edge 2 color
     * **/
    public void SetEdge2Color(Color color)
    {
        BackgroundTrianglesRenderer parentRenderer = m_parentColumn.m_parentRenderer;

        int edge2FirstVerticesArrayIndex = parentRenderer.GetTriangleFirstVerticesArrayIndex(m_parentColumn.m_index, this.m_indexInColumn) + 4;
        parentRenderer.m_meshColors[edge2FirstVerticesArrayIndex] = color;
        parentRenderer.m_meshColors[edge2FirstVerticesArrayIndex + 1] = color;
        parentRenderer.m_meshColors[edge2FirstVerticesArrayIndex + 2] = color;
        parentRenderer.m_meshColors[edge2FirstVerticesArrayIndex + 3] = color;
    }

    /**
     * Modify the mesh colors arrays to apply the edge 3 color
     * **/
    public void SetEdge3Color(Color color)
    {
        BackgroundTrianglesRenderer parentRenderer = m_parentColumn.m_parentRenderer;

        int edge3FirstVerticesArrayIndex = parentRenderer.GetTriangleFirstVerticesArrayIndex(m_parentColumn.m_index, this.m_indexInColumn) + 8;
        parentRenderer.m_meshColors[edge3FirstVerticesArrayIndex] = color;
        parentRenderer.m_meshColors[edge3FirstVerticesArrayIndex + 1] = color;
        parentRenderer.m_meshColors[edge3FirstVerticesArrayIndex + 2] = color;
        parentRenderer.m_meshColors[edge3FirstVerticesArrayIndex + 3] = color;
    }

    /**
     * Modify the mesh colors arrays to apply the inner color of this triangle
     * **/
    private void SetInnerTriangleColor(Color color)
    {
        BackgroundTrianglesRenderer parentRenderer = m_parentColumn.m_parentRenderer;

        int innerTriangleFirstVerticesArrayIndex = parentRenderer.GetTriangleFirstVerticesArrayIndex(m_parentColumn.m_index, this.m_indexInColumn) + 12;
        parentRenderer.m_meshColors[innerTriangleFirstVerticesArrayIndex] = color;
        parentRenderer.m_meshColors[innerTriangleFirstVerticesArrayIndex + 1] = color;
        parentRenderer.m_meshColors[innerTriangleFirstVerticesArrayIndex + 2] = color;
    }

    /**
     * Returns the global index of this triangle inside the mesh
     * **/
    public int GetGlobalIndexInMesh()
    {
        BackgroundTrianglesRenderer parentRenderer = m_parentColumn.m_parentRenderer;

        int globalIndex = 0;
        for (int iColumnIdx = 0; iColumnIdx != m_parentColumn.m_index; iColumnIdx++)
        {
            globalIndex += parentRenderer.m_triangleColumns.Count;
        }

        globalIndex += m_indexInColumn;

        return globalIndex;
    }

    /**
     * Generate a color slightly different from original color
     * **/
    public void GenerateColorFromOriginalColor(float delta)
    {
        SetColor(ColorUtils.GetRandomNearColor(m_originalColor, delta));
    }

    /**
     * Start the color animation process
     * **/
    public void StartColorAnimation(Color toColor, float fDuration, float fDelay = 0.0f)
    {
        m_animationStartColor = m_color;
        m_animationEndColor = toColor;
        m_colorAnimationElapsedTime = 0;
        m_colorAnimationDuration = fDuration;
        m_colorAnimationDelay = fDelay;

        m_colorAnimating = true;
    }    

    /**
     * Animates the color. This function is called from the Update loop of the parent renderer (via the column)
     * **/
    public void AnimateColor(float dt)
    {
        if (m_colorAnimating)
        {
            m_colorAnimationElapsedTime += dt;

            if (m_colorAnimationElapsedTime < m_colorAnimationDelay)
                return;

            if (m_colorAnimationElapsedTime >= m_colorAnimationDuration + m_colorAnimationDelay)
            {
                m_colorAnimating = false;
                SetColor(m_animationEndColor);
            }
            else
            {
                float timeRatio = (m_colorAnimationElapsedTime - m_colorAnimationDelay) / m_colorAnimationDuration;
                SetColor(Color.Lerp(m_animationStartColor, m_animationEndColor, timeRatio));
            }
        }       
    }

    public void Offset(float dy)
    {
        Vector2 offset = new Vector2(0, dy);

        for (int i = 0; i != m_points.Length; i++)
        {
            m_points[i] += offset;
        }
    }    
}