using UnityEngine;

public class CircleAnimator : TranspQuadOpacityAnimator
{
    public float m_innerRadius;
    private float m_prevInnerRadius;
    public float m_thickness;
    private float m_prevThickness;
    public int m_numSegments;
    private int m_prevNumSegments;

    public void SetInnerRadius(float radius, bool bRenderCircle = true)
    {
        m_innerRadius = radius;
        m_prevInnerRadius = radius;
        if (bRenderCircle)
            RenderCircle();
    }

    public void SetThickness(float thickness, bool bRenderCircle = true)
    {
        m_thickness = thickness;
        m_prevThickness = thickness;
        if (bRenderCircle)
            RenderCircle();
    }

    public void SetNumSegments(int numSegments, bool bRenderCircle = true)
    {
        m_numSegments = numSegments;
        m_prevNumSegments = numSegments;
        if (bRenderCircle)
            RenderCircle();
    }

    public void RenderCircle()
    {
        Circle circle = this.GetComponent<Circle>();
        circle.Render(m_innerRadius, m_thickness, m_color, m_numSegments);
    }

    protected override void Update()
    {
        base.Update();

        if (m_innerRadius != m_prevInnerRadius ||
            m_thickness != m_prevThickness ||
            m_numSegments != m_prevNumSegments)
        {
            SetInnerRadius(m_innerRadius, false);
            SetThickness(m_thickness, false);
            SetNumSegments(m_numSegments, false);

            RenderCircle();
        }
    }
}

