using UnityEngine;

/**
 * A polygon made by assembling background triangles and serving as a clickable element
 * **/
public abstract class GUISlot : Object
{
    public const float BLEND_COLOR_DEFAULT_PROPORTION = 0.25f;

    public BackgroundTriangle[] m_triangles { get; set; }

    //how the background triangles inside that slot will be blended
    public Color m_blendColor { get; set; }
    public float m_blendColorProportion { get; set; }

    /**
     * Show the slot with fading in background + information that appears on top of it
     * **/
    public abstract void Show(Color blendColor, float blendColorProportion, bool bAnimated = true, float fDuration = 1.0f, float fDelay = 0.0f);

    /**
     * Dismiss a previously shown slot with the option to dismiss the background or only the information contained in that slot
     * **/
    public abstract void Dismiss(bool bDismissBackground = true, bool bAnimated = true, float fDuration = 1.0f, float fDelay = 0.0f);

    /**
     * Tell if this slot contains the point passed as parameter
     * **/
    public bool ContainsPoint(Vector2 point)
    {
        for (int i = 0; i != m_triangles.Length; i++)
        {
            if (m_triangles[i].ContainsPoint(point))
                return true;
        }

        return false;
    }

    /**
     * Returns the center of this slot
     * **/
    public Vector2 GetCenter()
    {
        Vector2 accumulationCenter = Vector2.zero;

        for (int i = 0; i != m_triangles.Length; i++)
        {
            accumulationCenter += m_triangles[i].GetCenter();
        }

        return accumulationCenter /= m_triangles.Length;
    }

    /**
     * Show the background by blending triangles
     * **/
    protected void ShowBackground(Color blendColor, float blendColorProportion, bool bAnimated, float fDuration, float fDelay)
    {
        if (m_triangles == null)
            throw new System.Exception("Slot triangles have to be built first");

        m_blendColor = blendColor;
        m_blendColorProportion = blendColorProportion;

        //blend all triangles
        for (int i = 0; i != m_triangles.Length; i++)
        {
            BackgroundTriangle triangle = m_triangles[i];
            Color toColor = Color.Lerp(triangle.m_color, blendColor, blendColorProportion);
            if (bAnimated)
                triangle.StartColorAnimation(toColor, fDuration, fDelay);
            else
            {
                triangle.m_color = toColor;
            }
        }
    }

    /**
     * Dismiss the background by reverting the processus of blending
     * **/
    protected void DismissBackground(bool bAnimated, float fDuration, float fDelay)
    {
        for (int i = 0; i != m_triangles.Length; i++)
        {
            BackgroundTriangle triangle = m_triangles[i];

            Color toColor;
            if (m_blendColorProportion < 1)
                toColor = (triangle.m_color - m_blendColorProportion * m_blendColor) / (1 - m_blendColorProportion);
            else
                toColor = m_blendColor;

            if (bAnimated)
                triangle.StartColorAnimation(toColor, fDuration, fDelay);
            else
            {
                triangle.m_color = toColor;
            }
        }
    }
}
