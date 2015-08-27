using UnityEngine;

public class Gradient
{
    //variables related to linear gradients
    public Vector2 m_linearGradientStartPoint { get; set; } //World coordinates of the start point of this gradient
    public Vector2 m_linearGradientEndPoint { get; set; } //World coordinates of the start point of this gradient
    public Color m_linearGradientStartColor { get; set; } //the color of the gradient start point
    public Color m_linearGradientEndColor { get; set; } //the color of the gradient end point

    //variables related to radial gradients
    public Vector2 m_radialGradientCenter { get; set; }
    public float m_radialGradientRadius { get; set; }
    public Color m_radialGradientInnerColor { get; set; } //the color at the center of the gradient
    public Color m_radialGradientOuterColor { get; set; } //the color on the outer circle of the gradient

    //use this to box the gradient
    public Vector2 m_gradientBoxMin; //coordinates of the bottom left point of this box
    public Vector2 m_gradientBoxMax; //coordinates of the top right point of this box

    public enum GradientType
    {
        LINEAR = 1,
        RADIAL
    }

    public GradientType m_type { get; set; }

    public Gradient()
    {

    }

    /**
     * Creates a linear gradient between startPoint and endPoint
     * **/
    public void CreateLinear(Vector2 startPoint, Vector2 endPoint, Color startColor, Color endColor)
    {
        m_type = GradientType.LINEAR;
        m_linearGradientStartPoint = startPoint;
        m_linearGradientEndPoint = endPoint;
        m_linearGradientStartColor = startColor;
        m_linearGradientEndColor = endColor;
    }

    /**
     * Creates a radial gradient with center and radius
     * **/
    public void CreateRadial(Vector2 center, float radius, Color innerColor, Color outerColor)
    {
        m_type = GradientType.RADIAL;
        m_radialGradientCenter = center;
        m_radialGradientRadius = radius;
        m_radialGradientInnerColor = innerColor;
        m_radialGradientOuterColor = outerColor;
    }

    /**
     * Creates a elliptic radial gradient with center, axes and rotation angle
     * **/
    public void CreateElliptic(Vector2 center, float smallAxis, float bigAxis, float rotationAngle, Color startColor, Color EndColor)
    {

    }

    /**
     * Get the value of this color for this gradient at specific position
     * **/
    public Color GetColorAtPosition(Vector2 position)
    {
        if (m_type == GradientType.LINEAR)
        {
            //First check if the projection of the position onto the gradient segment is inside that segment
            Vector2 e1 = position - m_linearGradientStartPoint;
            Vector2 e2 = m_linearGradientEndPoint - m_linearGradientStartPoint;
            float projectionLength = MathUtils.DotProduct(e1, e2) / e2.magnitude;
            float gradientLength = (m_linearGradientEndPoint - m_linearGradientStartPoint).magnitude;

            if (projectionLength > 0 && projectionLength < gradientLength) //interpolate the value between m_linearGradientStartColor and m_linearGradientEndColor
            {
                float t = projectionLength / gradientLength;
                return Color.Lerp(m_linearGradientStartColor, m_linearGradientEndColor, t);
            }
            else if (projectionLength <= 0)
                return m_linearGradientStartColor;
            else if (projectionLength >= gradientLength)
                return m_linearGradientEndColor;
        }
        else if (m_type == GradientType.RADIAL)
        {
            float distanceToCenter = (position - m_radialGradientCenter).magnitude;

            if (distanceToCenter < m_radialGradientRadius)
            {               
                float t = distanceToCenter / m_radialGradientRadius;
                return Color.Lerp(m_radialGradientInnerColor, m_radialGradientOuterColor, t);
            }

            return m_radialGradientOuterColor;
        }

        return Color.black;
    }
}
