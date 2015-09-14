using UnityEngine;

public class MathUtils
{
    public const float DEFAULT_EPSILON = 0.0001f;

    static public bool AreFloatsEqual(float floatA, float floatB, float epsilon = DEFAULT_EPSILON)
    {
        return Mathf.Abs(floatA - floatB) <= epsilon;
    }

    static public bool AreVec2PointsEqual(Vector2 pointA, Vector2 pointB, float epsilon = DEFAULT_EPSILON)
    {
        return (pointB - pointA).sqrMagnitude <= epsilon;
    }

    static public bool AreVec3PointsEqual(Vector3 pointA, Vector3 pointB, float epsilon = DEFAULT_EPSILON)
    {
        return (pointB - pointA).sqrMagnitude <= epsilon;
    }

    /**
     * Returns true if valueToTest is in the interval [valueA - epsilon, valueB + epsilon]
     * **/
    static public bool isValueInInterval(float testValue, float valueA, float valueB, float epsilon = MathUtils.DEFAULT_EPSILON)
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
     * Calculates the determinant of 3 points
     * **/
    static public float Determinant(Vector2 u, Vector2 v, Vector2 w)
    {
        Vector2 vec1 = u - w;
        Vector2 vec2 = v - w;

        return Determinant(vec1, vec2);
    }

    /**
     * Calculates the determinant of 2 vectors
     * **/
    static public float Determinant(Vector2 u, Vector2 v)
    {
        return u.x * v.y - u.y * v.x;
    }

    /**
     * Calculates the dot product of 2 vectors
     * **/
    static public float DotProduct(Vector2 u, Vector2 v)
    {
        return u.x * v.x + u.y * v.y;
    }


    /**
     * Test if the float value has a fractional part or is a pure int and can be casted so
     * **/
    static public bool HasFractionalPart(float fValue)
    {
        int iValue = Mathf.FloorToInt(fValue);

        return (iValue != fValue);
    }
}
