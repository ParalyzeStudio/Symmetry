using UnityEngine;

public class GeometryUtils
{
    static public Vector3 BuildVector3FromVector2(Vector2 vector, float zValue)
    {
        return new Vector3(vector.x, vector.y, zValue);
    }

    static public Vector2 BuildVector2FromVector3(Vector2 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    /**
     * Simply tells if a segment intersects a line define by 2 points without giving the coordinates of the intersection point
     * **/
    static public bool SegmentIntersectsLine(Vector2 segmentPoint1, Vector2 segmentPoint2, Vector2 linePoint1, Vector2 linePoint2)
    {
        float det1 = MathUtils.Determinant(linePoint1, linePoint2, segmentPoint1, false);
        float det2 = MathUtils.Determinant(linePoint1, linePoint2, segmentPoint2, false);

        if (MathUtils.AreFloatsEqual(det1, 0) || MathUtils.AreFloatsEqual(det2, 0))
            return false;

        return (det1 * det2) < 0;
    }

    /**
     * Find the intersection of a segment and a line if it exists
     * **/
    static public void SegmentLineIntersection(Vector2 segmentPoint1, Vector2 segmentPoint2, Vector2 linePoint, Vector2 lineDirection, out Vector2 intersection, out bool intersects)
    {
        //normalize the line direction if not
        lineDirection.Normalize();

        //order points by ascending x for the segment only
        if (segmentPoint1.x > segmentPoint2.x)
        {
            Vector2 tmpPoint = segmentPoint2;
            segmentPoint2 = segmentPoint1;
            segmentPoint1 = tmpPoint;
        }
        else if (segmentPoint1.x == segmentPoint2.x) // if x-coordinate is the same for both points, order them by ascending y
        {
            if (segmentPoint1.y > segmentPoint2.y)
            {
                Vector2 tmpPoint = segmentPoint2;
                segmentPoint2 = segmentPoint1;
                segmentPoint1 = tmpPoint;
            }
        }

        //Both lines equation
        float x, y;
        if (!MathUtils.AreFloatsEqual(lineDirection.x, 0) && !MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x)) //y = a1x + b1 && y = a2x + b2
        {
            float a1 = lineDirection.y / lineDirection.x;
            float b1 = linePoint.y - a1 * linePoint.x;
            float a2 = (segmentPoint2.y - segmentPoint1.y) / (segmentPoint2.x - segmentPoint1.x);
            float b2 = segmentPoint1.y - a2 * segmentPoint1.x;

            if (a1 == a2) //parallel lines
            {
                intersects = false;
                intersection = Vector2.zero;
                return;
            }
            else
            {
                x = (b2 - b1) / (a1 - a2);
                y = a1 * x + b1;
            }
        }
        else if (!MathUtils.AreFloatsEqual(lineDirection.x, 0) && MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x)) //y = a1x + b1 && x = a2
        {
            float a1 = lineDirection.y / lineDirection.x;
            float b1 = linePoint.y - a1 * linePoint.x;
            float a2 = segmentPoint1.x;

            x = a2;
            y = a1 * a2 + b1;
        }
        else if (MathUtils.AreFloatsEqual(lineDirection.x, 0) && !MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x)) //x = a1 && y = a2x + b2
        {
            float a1 = linePoint.x;
            float a2 = (segmentPoint2.y - segmentPoint1.y) / (segmentPoint2.x - segmentPoint1.x);
            float b2 = segmentPoint1.y - a2 * segmentPoint1.x;

            x = a1;
            y = a2 * a1 + b2;
        }
        else //parallel vertical lines, no intersection or infinite intersections. In both cases return no intersection
        {
            intersects = false;
            intersection = Vector2.zero;
            return;
        }


        //Check if ((x, y) point is contained in the segment
        if (MathUtils.AreFloatsEqual(segmentPoint1.x, segmentPoint2.x))
        {
            if (MathUtils.isValueInInterval(y, segmentPoint1.y, segmentPoint2.y))
            {
                intersects = true;
                intersection = new Vector2(x, y);
                return;
            }
            else
            {
                intersects = false;
                intersection = Vector2.zero;
                return;
            }
        }
        else
        {
            if (MathUtils.isValueInInterval(x, segmentPoint1.x, segmentPoint2.x))
            {
                intersects = true;
                intersection = new Vector2(x, y);
                return;
            }
            else
            {
                intersects = false;
                intersection = Vector2.zero;
                return;
            }
        }
    }

    /**
     * Simply tells if two segments intersect without giving the coordinates of the intersection point
     * **/
    static public bool TwoSegmentsIntersect(Vector2 segment1Point1, Vector2 segment1Point2, Vector2 segment2Point1, Vector2 segment2Point2)
    {
        float det1 = MathUtils.Determinant(segment1Point1, segment1Point2, segment2Point1, false);
        float det2 = MathUtils.Determinant(segment1Point1, segment1Point2, segment2Point2, false);
        
        if (MathUtils.AreFloatsEqual(det1, 0) || MathUtils.AreFloatsEqual(det2, 0))
        {
            //two segments are collinear and on the same line OR one point of the second segment is contained into the first segment but not the other point
            return false;       
        }
        else if ((det1 * det2) < 0) //check if det1 and det2 are of opposite signs (product can't be zero)
        {
            float det3 = MathUtils.Determinant(segment2Point1, segment2Point2, segment1Point1, false);
            float det4 = MathUtils.Determinant(segment2Point1, segment2Point2, segment1Point2, false);

            if ((det3 * det4) < 0) //check if det3 and det4 are of opposite signs
            {
                return true;
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
     * Find the intersection of two segments if it exists
     * **/
    static public void TwoSegmentsIntersection(Vector2 segment1Point1, Vector2 segment1Point2, Vector2 segment2Point1, Vector2 segment2Point2, out bool intersects, out Vector2 intersection)
    {
        //order points by ascending x
        if (segment1Point1.x > segment1Point2.x)
        {
            Vector2 tmpPoint = segment1Point2;
            segment1Point2 = segment1Point1;
            segment1Point1 = tmpPoint;
        }
        if (segment2Point1.x > segment2Point2.x)
        {
            Vector2 tmpPoint = segment2Point2;
            segment2Point2 = segment2Point1;
            segment2Point1 = tmpPoint;
        }

        //Both lines equation
        float x, y;
        if (!MathUtils.AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !MathUtils.AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && y = a2x + b2
        {
            float a1 = (segment1Point2.y - segment1Point1.y) / (segment1Point2.x - segment1Point1.x);
            float b1 = segment1Point1.y - a1 * segment1Point1.x;
            float a2 = (segment2Point2.y - segment2Point1.y) / (segment2Point2.x - segment2Point1.x);
            float b2 = segment2Point1.y - a2 * segment2Point1.x;

            if (a1 == a2) //parallel lines
            {
                intersects = false;
                intersection = Vector2.zero;
                return;
            }
            else
            {
                x = (b2 - b1) / (a1 - a2);
                y = a1 * x + b1;
            }
        }
        else if (!MathUtils.AreFloatsEqual(segment1Point1.x, segment1Point2.x) && MathUtils.AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && x = a2
        {
            float a1 = (segment1Point2.y - segment1Point1.y) / (segment1Point2.x - segment1Point1.x);
            float b1 = segment1Point1.y - a1 * segment1Point1.x;
            float a2 = segment2Point1.x;

            x = a2;
            y = a1 * a2 + b1;
        }
        else if (MathUtils.AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !MathUtils.AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //x = a1 && y = a2x + b2
        {
            float a1 = segment1Point1.x;
            float a2 = (segment2Point2.y - segment2Point1.y) / (segment2Point2.x - segment2Point1.x);
            float b2 = segment2Point1.y - a2 * segment2Point1.x;

            x = a1;
            y = a2 * a1 + b2;
        }
        else //parallel vertical lines, no intersection or infinite intersections. In both cases return no intersection
        {
            intersects = false;
            intersection = Vector2.zero;
            return;
        }


        //Check if (x, y) point is contained in both segments
        if (MathUtils.isValueInInterval(x, segment1Point1.x, segment1Point2.x)
            &&
            MathUtils.isValueInInterval(x, segment2Point1.x, segment2Point2.x))
        {
            intersects = true;
            intersection = new Vector2(x, y);
        }
        else
        {
            intersects = false;
            intersection = Vector2.zero;
        }
    }

    /**
     * Checks if a point is contained in the segment [pointA ; pointB]
     * **/
    static public bool IsPointContainedInSegment(Vector2 point, Vector2 segmentPointA, Vector2 segmentPointB)
    {
        if (MathUtils.AreVec2PointsEqual(point, segmentPointA) || MathUtils.AreVec2PointsEqual(point, segmentPointB))
            return true;

        float det = MathUtils.Determinant(segmentPointA, segmentPointB, point, false);
        if (!MathUtils.AreFloatsEqual(det, 0)) //points are not aligned, thus point can never be on the segment AB
            return false;

        Vector2 u = point - segmentPointA; //AM vector
        Vector2 v = segmentPointB - segmentPointA; //AB vector

        float dotProduct = MathUtils.DotProduct(u, v); //calculate the dot product AM.AB
        if (dotProduct > 0)
            return dotProduct < v.sqrMagnitude; //AM length should be majored by AB length so fot product AM.AB should be majored by AB squared length
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
        float det1 = MathUtils.Determinant(pointA, pointB, testPoint, false);
        float det2 = MathUtils.Determinant(pointB, pointC, testPoint, false);
        float det3 = MathUtils.Determinant(pointC, pointA, testPoint, false);

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
}
