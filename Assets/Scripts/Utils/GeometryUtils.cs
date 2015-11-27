﻿using UnityEngine;
using System;
using ClipperLib;

public class GeometryUtils
{
    public const double CONVERSION_FLOAT_PRECISION = 1.0E1;

    static public Vector3 BuildVector3FromVector2(Vector2 vector, float zValue)
    {
        return new Vector3(vector.x, vector.y, zValue);
    }

    static public Vector2 BuildVector2FromVector3(Vector2 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    //public static IntPoint ConvertVector2ToIntPoint(Vector2 point)
    //{
    //    return new IntPoint((long)Math.Round(point.x * CONVERSION_FLOAT_PRECISION), (long)Math.Round(point.y * CONVERSION_FLOAT_PRECISION));
    //}

    //public static Vector2 ConvertIntPointToVector2(IntPoint point)
    //{
    //    return new Vector2(point.X / (float)CONVERSION_FLOAT_PRECISION, point.Y / (float)CONVERSION_FLOAT_PRECISION);
    //}

    /**
     * Simply tells if a segment intersects a line define by 2 points without giving the coordinates of the intersection point
     * **/
    static public bool SegmentIntersectsLine(Vector2 segmentPoint1, Vector2 segmentPoint2, Vector2 linePoint1, Vector2 linePoint2)
    {
        float det1 = MathUtils.Determinant(linePoint1, linePoint2, segmentPoint1);
        float det2 = MathUtils.Determinant(linePoint1, linePoint2, segmentPoint2);

        if (MathUtils.AreFloatsEqual(det1, 0) || MathUtils.AreFloatsEqual(det2, 0))
            return false;

        return (det1 * det2) < 0;
    }

    /**
     * Find the intersection of a segment and a line if it exists
     * **/

    //static public void SegmentLineIntersection(Vector2 segmentPoint1, Vector2 segmentPoint2, Vector2 linePoint, Vector2 lineDirection, out Vector2 intersection, out bool intersects)
    //{
    //    //normalize the line direction if not
    //    lineDirection.Normalize();

    //    //order points by ascending x for the segment only
    //    if (segmentPoint1.x > segmentPoint2.x)
    //    {
    //        Vector2 tmpPoint = segmentPoint2;
    //        segmentPoint2 = segmentPoint1;
    //        segmentPoint1 = tmpPoint;
    //    }
    //    else if (segmentPoint1.x == segmentPoint2.x) // if x-coordinate is the same for both points, order them by ascending y
    //    {
    //        if (segmentPoint1.y > segmentPoint2.y)
    //        {
    //            Vector2 tmpPoint = segmentPoint2;
    //            segmentPoint2 = segmentPoint1;
    //            segmentPoint1 = tmpPoint;
    //        }
    //    }

    //    //Both lines equation
    //    float x, y;
    //    if (!MathUtils.AreFloatsEqual(lineDirection.x, 0) && !MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x)) //y = a1x + b1 && y = a2x + b2
    //    {
    //        float a1 = lineDirection.y / lineDirection.x;
    //        float b1 = linePoint.y - a1 * linePoint.x;
    //        float a2 = (segmentPoint2.y - segmentPoint1.y) / (segmentPoint2.x - segmentPoint1.x);
    //        float b2 = segmentPoint1.y - a2 * segmentPoint1.x;

    //        if (a1 == a2) //parallel lines
    //        {
    //            intersects = false;
    //            intersection = Vector2.zero;
    //            return;
    //        }
    //        else
    //        {
    //            x = (b2 - b1) / (a1 - a2);
    //            y = a1 * x + b1;
    //        }
    //    }
    //    else if (!MathUtils.AreFloatsEqual(lineDirection.x, 0) && MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x)) //y = a1x + b1 && x = a2
    //    {
    //        float a1 = lineDirection.y / lineDirection.x;
    //        float b1 = linePoint.y - a1 * linePoint.x;
    //        float a2 = segmentPoint1.x;

    //        x = a2;
    //        y = a1 * a2 + b1;
    //    }
    //    else if (MathUtils.AreFloatsEqual(lineDirection.x, 0) && !MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x)) //x = a1 && y = a2x + b2
    //    {
    //        float a1 = linePoint.x;
    //        float a2 = (segmentPoint2.y - segmentPoint1.y) / (segmentPoint2.x - segmentPoint1.x);
    //        float b2 = segmentPoint1.y - a2 * segmentPoint1.x;

    //        x = a1;
    //        y = a2 * a1 + b2;
    //    }
    //    else //parallel vertical lines, no intersection or infinite intersections. In both cases return no intersection
    //    {
    //        intersects = false;
    //        intersection = Vector2.zero;
    //        return;
    //    }


    //    //Check if ((x, y) point is contained in the segment
    //    if (MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x))
    //    {
    //        if (MathUtils.isValueInInterval(y, segmentPoint1.y, segmentPoint2.y, 0))
    //        {
    //            intersects = true;
    //            intersection = new Vector2(x, y);
    //            return;
    //        }
    //        else
    //        {
    //            intersects = false;
    //            intersection = Vector2.zero;
    //            return;
    //        }
    //    }
    //    else
    //    {
    //        if (MathUtils.isValueInInterval(x, segmentPoint1.x, segmentPoint2.x, 0))
    //        {
    //            intersects = true;
    //            intersection = new Vector2(x, y);
    //            return;
    //        }
    //        else
    //        {
    //            intersects = false;
    //            intersection = Vector2.zero;
    //            return;
    //        }
    //    }
    //}

    public const int SEGMENTS_OVERLAP = 1; //segments share more than 1 vertex (i.e intersection is a segment)
    public const int SEGMENTS_INTERSECTION_IS_ENDPOINT = 2; //segments share one vertex and this vertex is a segment endpoint
    public const int SEGMENTS_STRICT_INTERSECTION = 4; //segments share one vertex and this is not a segment endpoint

    /**
     * Simply tells if two segments intersect without giving the coordinates of the intersection point
     * Set bStrictIntersection to true if we must check if the intersection is a single point that differs from the 4 segment endpoints
     * **/
    //static public bool TwoSegmentsIntersect(GridPoint segment1Point1, GridPoint segment1Point2, GridPoint segment2Point1, GridPoint segment2Point2, int bFilters = SEGMENTS_OVERLAP | SEGMENTS_INTERSECTION_IS_ENDPOINT | SEGMENTS_STRICT_INTERSECTION)
    //{
    //    bool bSeg1Pt1InSeg2 = IsPointContainedInSegment(segment1Point1, segment2Point1, segment2Point2);
    //    bool bSeg1Pt2InSeg2 = IsPointContainedInSegment(segment1Point2, segment2Point1, segment2Point2);
    //    bool bSeg2Pt1InSeg1 = IsPointContainedInSegment(segment2Point1, segment1Point1, segment1Point2);
    //    bool bSeg2Pt2InSeg1 = IsPointContainedInSegment(segment2Point2, segment1Point1, segment1Point2);

    //    float det1 = MathUtils.Determinant(segment1Point1, segment1Point2, segment2Point1);
    //    float det2 = MathUtils.Determinant(segment1Point1, segment1Point2, segment2Point2);
    //    if (det1 == 0 && det2 == 0) //4 endpoints are aligned, segments are collinear and on the same line
    //    {
    //        //one segment is contained in the other
    //        if (bSeg1Pt1InSeg2 && bSeg1Pt2InSeg2 || bSeg2Pt1InSeg1 && bSeg2Pt2InSeg1)
    //            if ((bFilters & SEGMENTS_OVERLAP) > 0)
    //                return true;

    //        //check if segments share at least 1 endpoint
    //        int dotProduct = 0;
    //        if (MathUtils.AreVec2PointsEqual(segment1Point2, segment2Point1))
    //            dotProduct = MathUtils.DotProduct(segment1Point1 - segment1Point2, segment2Point2 - segment2Point1);
    //        else if (MathUtils.AreVec2PointsEqual(segment1Point2, segment2Point2))
    //            dotProduct = MathUtils.DotProduct(segment1Point1 - segment1Point2, segment2Point1 - segment2Point2);
    //        else if (MathUtils.AreVec2PointsEqual(segment1Point1, segment2Point1))
    //            dotProduct = MathUtils.DotProduct(segment1Point2 - segment1Point1, segment2Point2 - segment2Point1);
    //        else if (MathUtils.AreVec2PointsEqual(segment1Point1, segment2Point2))
    //            dotProduct = MathUtils.DotProduct(segment1Point2 - segment1Point1, segment2Point1 - segment2Point2);

    //        if (dotProduct != 0)
    //        {
    //            if (dotProduct > 0)
    //            {
    //                if ((bFilters & SEGMENTS_OVERLAP) > 0)
    //                    return true;
    //            }
    //            else
    //            {
    //                if ((bFilters & SEGMENTS_INTERSECTION_IS_ENDPOINT) > 0)
    //                    return true;
    //            }
    //        }

    //        //check if one segment endpoint is strictly contained in the other segment
    //        if (bSeg1Pt1InSeg2 || bSeg1Pt2InSeg2)
    //            if ((bFilters & SEGMENTS_OVERLAP) > 0)
    //                return true;

    //        //all other cases = no intersection
    //        return false;
    //    }
    //    else if (det1 == 0 || det2 == 0) //3 points are aligned
    //    {
    //        if (bSeg1Pt1InSeg2 || bSeg1Pt2InSeg2 || bSeg2Pt1InSeg1 || bSeg2Pt2InSeg1)
    //            if ((bFilters & SEGMENTS_INTERSECTION_IS_ENDPOINT) > 0)
    //                return true;

    //        return false;
    //    }        
    //    else if ((det1 * det2) < 0) //check if det1 and det2 are of opposite signs
    //    {
    //        float det3 = MathUtils.Determinant(segment2Point1, segment2Point2, segment1Point1);
    //        float det4 = MathUtils.Determinant(segment2Point1, segment2Point2, segment1Point2);

    //        if ((det3 * det4) < 0) //check if det3 and det4 are of opposite signs
    //        {
    //            return (bFilters & SEGMENTS_STRICT_INTERSECTION) > 0; //we only check the strict intersection here
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    else /** (det1 * det2) > 0 **/
    //    {
    //        return false;
    //    }
    //}


    /**
     * Find the intersection of two segments if it exists
     * **/
    //static public void TwoSegmentsIntersection(Vector2 segment1Point1, Vector2 segment1Point2, Vector2 segment2Point1, Vector2 segment2Point2, out bool intersects, out Vector2 intersection)
    //{
    //    //order points by ascending x
    //    if (segment1Point1.x > segment1Point2.x)
    //    {
    //        Vector2 tmpPoint = segment1Point2;
    //        segment1Point2 = segment1Point1;
    //        segment1Point1 = tmpPoint;
    //    }
    //    if (segment2Point1.x > segment2Point2.x)
    //    {
    //        Vector2 tmpPoint = segment2Point2;
    //        segment2Point2 = segment2Point1;
    //        segment2Point1 = tmpPoint;
    //    }

    //    //Both lines equation
    //    float x, y;
    //    if (!MathUtils.AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !MathUtils.AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && y = a2x + b2
    //    {
    //        float a1 = (segment1Point2.y - segment1Point1.y) / (segment1Point2.x - segment1Point1.x);
    //        float b1 = segment1Point1.y - a1 * segment1Point1.x;
    //        float a2 = (segment2Point2.y - segment2Point1.y) / (segment2Point2.x - segment2Point1.x);
    //        float b2 = segment2Point1.y - a2 * segment2Point1.x;

    //        if (a1 == a2) //parallel lines
    //        {
    //            intersects = false;
    //            intersection = Vector2.zero;
    //            return;
    //        }
    //        else
    //        {
    //            x = (b2 - b1) / (a1 - a2);
    //            y = a1 * x + b1;
    //        }
    //    }
    //    else if (!MathUtils.AreFloatsEqual(segment1Point1.x, segment1Point2.x) && MathUtils.AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && x = a2
    //    {
    //        float a1 = (segment1Point2.y - segment1Point1.y) / (segment1Point2.x - segment1Point1.x);
    //        float b1 = segment1Point1.y - a1 * segment1Point1.x;
    //        float a2 = segment2Point1.x;

    //        x = a2;
    //        y = a1 * a2 + b1;
    //    }
    //    else if (MathUtils.AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !MathUtils.AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //x = a1 && y = a2x + b2
    //    {
    //        float a1 = segment1Point1.x;
    //        float a2 = (segment2Point2.y - segment2Point1.y) / (segment2Point2.x - segment2Point1.x);
    //        float b2 = segment2Point1.y - a2 * segment2Point1.x;

    //        x = a1;
    //        y = a2 * a1 + b2;
    //    }
    //    else //parallel vertical lines, no intersection or infinite intersections. In both cases return no intersection
    //    {
    //        intersects = false;
    //        intersection = Vector2.zero;
    //        return;
    //    }


    //    //Check if (x, y) point is contained in both segments
    //    if (MathUtils.isValueInInterval(x, segment1Point1.x, segment1Point2.x)
    //        &&
    //        MathUtils.isValueInInterval(x, segment2Point1.x, segment2Point2.x))
    //    {
    //        intersects = true;
    //        intersection = new Vector2(x, y);
    //    }
    //    else
    //    {
    //        intersects = false;
    //        intersection = Vector2.zero;
    //    }
    //}

    /**
     * Tells if 2 segments are equal
     * **/
    static public bool AreSegmentsEqual(Vector2 segment1Point1, Vector2 segment1Point2, Vector2 segment2Point1, Vector2 segment2Point2)
    {
        return (MathUtils.AreVec2PointsEqual(segment1Point1, segment2Point1) && MathUtils.AreVec2PointsEqual(segment1Point2, segment2Point2))
               ||
               (MathUtils.AreVec2PointsEqual(segment1Point1, segment2Point2) && MathUtils.AreVec2PointsEqual(segment1Point2, segment2Point1));
    }

    /**
     * Checks if a point is contained in the segment [pointA ; pointB] or ]pointA; pointB[ if we chose to exclude endpoints
     * **/
    //static public bool IsPointContainedInSegment(Vector2 point, Vector2 segmentPointA, Vector2 segmentPointB, bool bIncludeEndpoints = true)
    //{
    //    if (MathUtils.AreVec2PointsEqual(point, segmentPointA) || MathUtils.AreVec2PointsEqual(point, segmentPointB))
    //        return bIncludeEndpoints;

    //    float det = MathUtils.Determinant(segmentPointA, segmentPointB, point);
    //    if (!MathUtils.AreFloatsEqual(det, 0)) //points are not aligned, thus point can never be on the segment AB
    //        return false;

    //    //points are aligned, just test if points is inside the ribbon defined by A and B
    //    return IsPointContainedInStrip(point, segmentPointA, segmentPointB, bIncludeEndpoints);
    //}

    /**
     * Test if a point is contained in a strip whose width is defined by segment AB
     * **/
    static public bool IsPointContainedInStrip(Vector2 point, Vector2 ribbonPointA, Vector2 ribbonPointB, bool bIncludeEndpoints = true)
    {
        Vector2 u = point - ribbonPointA; //AM vector
        Vector2 v = ribbonPointB - ribbonPointA; //AB vector

        float dotProduct = MathUtils.DotProduct(u, v); //calculate the dot product AM.AB
        if (dotProduct > 0)
            return dotProduct < v.sqrMagnitude; //AM length should be majored by AB length so dot product AM.AB should be majored by AB squared length
        else //AM and AB are of opposite sign
            return false;
    }

    /**
     * Checks if a point that we know is on a line holding pointA and pointB is also contained in the segment [pointA ; pointB]
     * **/
    static public bool IsLinePointContainedInSegment(Vector2 linePoint, Vector2 segmentPointA, Vector2 segmentPointB)
    {
        float minX = Mathf.Min(segmentPointA.x, segmentPointB.x);
        float maxX = Mathf.Max(segmentPointA.x, segmentPointB.x);
        float minY = Mathf.Min(segmentPointA.y, segmentPointB.y);
        float maxY = Mathf.Max(segmentPointA.y, segmentPointB.y);

        return (MathUtils.isValueInInterval(linePoint.x, minX, maxX)
                &&
                MathUtils.isValueInInterval(linePoint.y, minY, maxY));
    }

    /**
     * Determines if a point is inside the triangle defined by pointA, pointB and pointC 
     * Depending on the vertices order (clockwise or counter-clockwise) the signs of det1, det2 and det3 can be positive or negative
     * so we just check if they are of the same sign
     * **/
    static public bool IsInsideTriangle(Vector2 testPoint, Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        if (MathUtils.AreVec2PointsEqual(testPoint, pointA) ||
            MathUtils.AreVec2PointsEqual(testPoint, pointB) ||
            MathUtils.AreVec2PointsEqual(testPoint, pointC))
            return true;

        float det1 = MathUtils.Determinant(pointA, pointB, testPoint);
        float det2 = MathUtils.Determinant(pointB, pointC, testPoint);
        float det3 = MathUtils.Determinant(pointC, pointA, testPoint);

        if (MathUtils.AreFloatsEqual(det1, 0)) det1 = 0;
        if (MathUtils.AreFloatsEqual(det2, 0)) det2 = 0;
        if (MathUtils.AreFloatsEqual(det3, 0)) det3 = 0;

        return (det1 <= 0 && det2 <= 0 && det3 <= 0) || (det1 >= 0 && det2 >= 0 && det3 >= 0);
    }

    /**
     * Calculate the distance from a point to a line
     * **/
    static public float DistanceToLine(Vector2 point, Vector2 linePoint, Vector2 lineDirection)
    {
        //equation of the line ax + by + c = 0
        float a = -lineDirection.y;
        float b = lineDirection.x;
        float c = -a * linePoint.x - b * linePoint.y;

        float distance = Mathf.Abs(a * point.x + b * point.y + c) / Mathf.Sqrt(a * a + b * b);
        return distance;
    }

    /**
     * Tells if two segment overlap
     * **/
    //static public bool TwoSegmentsOverlap(Vector2 segment1Point1, Vector2 segment1Point2, Vector2 segment2Point1, Vector2 segment2Point2)
    //{
    //    //same segments
    //    if ((MathUtils.AreVec2PointsEqual(segment1Point1, segment2Point1) && MathUtils.AreVec2PointsEqual(segment1Point2, segment2Point2))
    //        ||
    //        (MathUtils.AreVec2PointsEqual(segment1Point2, segment2Point1) && MathUtils.AreVec2PointsEqual(segment1Point1, segment2Point2)))
    //    {
    //        return true;
    //    }

    //    Vector2 segment1Direction = segment1Point2 - segment1Point1;
    //    Vector2 segment2Direction = segment2Point2 - segment2Point1;
    //    bool bCollinearVectors = Mathf.Abs(MathUtils.Determinant(segment1Direction, segment2Direction)) < MathUtils.DEFAULT_EPSILON;

    //    if (bCollinearVectors)
    //    {
    //        if (IsPointContainedInSegment(segment1Point1, segment2Point1, segment2Point2, false)
    //            ||
    //            IsPointContainedInSegment(segment1Point2, segment2Point1, segment2Point2, false)
    //            ||
    //            IsPointContainedInSegment(segment2Point1, segment1Point1, segment1Point2, false)
    //            ||
    //            IsPointContainedInSegment(segment2Point2, segment1Point1, segment1Point2, false))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}
}
