﻿using UnityEngine;

public class MathUtils
{
    public const float DEFAULT_EPSILON = 0.1f;

    static public Vector3 BuildVector3FromVector2(Vector2 vector, float zValue)
    {
        return new Vector3(vector.x, vector.y, zValue);
    }

    static public Vector2 BuildVector2FromVector3(Vector2 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    static public bool AreFloatsEqual(float floatA, float floatB, float epsilon = DEFAULT_EPSILON)
    {
        return Mathf.Abs(floatA - floatB) < epsilon;
    }

    static public bool AreVec2PointsEqual(Vector2 pointA, Vector2 pointB, float epsilon = DEFAULT_EPSILON)
    {
        return (pointB - pointA).sqrMagnitude < epsilon;
    }

    static public bool AreVec3PointsEqual(Vector3 pointA, Vector3 pointB, float epsilon = DEFAULT_EPSILON)
    {
        return (pointB - pointA).sqrMagnitude < epsilon;
    }

    /**
     * Simply tells if two segments intersect without giving the coordinates of the intersection point
     * **/
    static public bool TwoSegmentsIntersect(Vector2 segment1Point1, Vector2 segment1Point2, Vector2 segment2Point1, Vector2 segment2Point2)
    {
        float det1 = Determinant(segment1Point1, segment1Point2, segment2Point1, false);
        float det2 = Determinant(segment1Point1, segment1Point2, segment2Point2, false);

        if ((det1 * det2) < 0) //check if det1 and det2 are of opposite signs
        {
            float det3 = Determinant(segment2Point1, segment2Point2, segment1Point1, false);
            float det4 = Determinant(segment2Point1, segment2Point2, segment1Point2, false);

            if ((det3 * det4) < 0) //check if det3 and det4 are of opposite signs
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (det1 == 0 || det2 == 0)
        {
            if (det1 == 0 && det2 == 0) //two segments are collinear and on the same line
            {
                return false;
            }
            else //one point of the second segment is contained into the first segment but not the other point
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
        if (!AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && y = a2x + b2
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
        else if (!AreFloatsEqual(segment1Point1.x, segment1Point2.x) && AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //y = a1x + b1 && x = a2
        {
            float a1 = (segment1Point2.y - segment1Point1.y) / (segment1Point2.x - segment1Point1.x);
            float b1 = segment1Point1.y - a1 * segment1Point1.x;
            float a2 = segment2Point1.x;

            x = a2;
            y = a1 * a2 + b1;
        }
        else if (AreFloatsEqual(segment1Point1.x, segment1Point2.x) && !AreFloatsEqual(segment2Point1.x, segment2Point2.x)) //x = a1 && y = a2x + b2
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
        if (isValueInInterval(x, segment1Point1.x, segment1Point2.x)
            &&
            isValueInInterval(x, segment2Point1.x, segment2Point2.x))
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
     * Checks if a point that we know is on a line is also contained in a segment defined by pointA and pointB
     * **/
    static public bool isLinePointContainedInSegment(Vector2 point, Vector2 pointA, Vector2 pointB)
    {
        float minX = Mathf.Min(pointA.x, pointB.x);
        float maxX = Mathf.Max(pointA.x, pointB.x);
        float minY = Mathf.Min(pointA.y, pointB.y);
        float maxY = Mathf.Max(pointA.y, pointB.y);

        return (isValueInInterval(point.x, minX, maxX)
                &&
                isValueInInterval(point.y, minY, maxY));
    }

    /**
     * Returns true if valueToTest is in the interval [valueA - epsilon, valueB + epsilon]
     * **/
    static public bool isValueInInterval(float testValue, float valueA, float valueB, float epsilon = DEFAULT_EPSILON)
    {
        if (valueA > valueB) //reorder values in ascending order
        {
            float tmpValue = valueA;
            valueA = valueB;
            valueB = tmpValue;
        }

        return (testValue >= (valueA - epsilon) && testValue <= (valueB + epsilon));
    }

    /**
     * Calculate the determinant of 3 points
     * **/
    static public float Determinant(Vector2 u, Vector2 v, Vector2 w, bool normalize)
    {
        if (normalize)
        {
            Vector2 vec1 = u - w;
            Vector2 vec2 = v - w;
            float determinant = u.x * v.y + v.x * w.y + w.x * u.y - u.x * w.y - v.x * u.y - w.x * v.y;
            return determinant / (vec1.magnitude * vec2.magnitude);
        }
        else
            return u.x * v.y + v.x * w.y + w.x * u.y - u.x * w.y - v.x * u.y - w.x * v.y;
    }

    /**
     * Determines if a point is inside the triangle defined by pointA, pointB and pointC 
     * Depending on the vertices order (clockwise or counter-clockwise) the signs of det1, det2 and det3 can be positive or negative
     * so we just check if they are of the same sign
     * **/
    static public bool IsInsideTriangle(Vector2 testPoint, Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        float det1 = Determinant(pointA, pointB, testPoint, false);
        float det2 = Determinant(pointB, pointC, testPoint, false);
        float det3 = Determinant(pointC, pointA, testPoint, false);

        return (det1 <= 0 && det2 <= 0 && det3 <= 0) || (det1 >= 0 && det2 >= 0 && det3 >= 0);
    }
}
