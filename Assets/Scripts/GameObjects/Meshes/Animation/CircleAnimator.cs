using UnityEngine;

public class CircleAnimator : TranspQuadOpacityAnimator
{
    //thickness of the circle
    public float m_thickness;

    //number of segments of the circle
    public int m_numSegments;

    //Radius animation
    protected bool m_animatingRadius;
    public float m_innerRadius;
    protected float m_fromRadius;
    protected float m_toRadius;
    protected float m_radiusAnimationDuration;
    protected float m_radiusAnimationDelay;
    protected float m_radiusAnimationElapsedTime;
    protected InterpolationType m_radiusAnimationInterpolationType;
    public bool m_radiusAnimationDestroyOnFinish;

    //store previous values to update circle from inspector
    private float m_prevInnerRadius;
    private int m_prevNumSegments;
    private float m_prevThickness;

    public void SetInnerRadius(float radius, bool bRenderCircle = true)
    {
        m_innerRadius = radius;
        m_prevInnerRadius = radius;
        if (bRenderCircle)
            RenderCircle();
    }

    public virtual void IncInnerRadius(float deltaRadius)
    {
        float fRadius = m_innerRadius + deltaRadius;
        SetInnerRadius(fRadius);
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
        CircleMesh circle = this.GetComponent<CircleMesh>();
        circle.Render(m_innerRadius, m_thickness, m_color, m_numSegments);
    }

    public void AnimateRadiusTo(float toRadius, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyOnFinish = false)
    {
        if (m_innerRadius == toRadius)
            return;

        m_animatingRadius = true;
        m_fromRadius = m_innerRadius;
        m_toRadius = toRadius;
        m_radiusAnimationDuration = duration;
        m_radiusAnimationDelay = delay;
        m_radiusAnimationElapsedTime = 0;
        m_radiusAnimationInterpolationType = interpolType;
        m_radiusAnimationDestroyOnFinish = bDestroyOnFinish;
    }

    protected void UpdateRadius(float dt)
    {
        if (m_animatingRadius)
        {
            bool inDelay = (m_radiusAnimationElapsedTime < m_radiusAnimationDelay);
            m_radiusAnimationElapsedTime += dt;
            if (m_radiusAnimationElapsedTime >= m_radiusAnimationDelay)
            {
                if (inDelay) //we were in delay previously
                {
                    dt = m_radiusAnimationElapsedTime - m_radiusAnimationDelay;
                }
                float effectiveElapsedTime = m_radiusAnimationElapsedTime - m_radiusAnimationDelay;
                float deltaRadius = 0;
                float radiusVariation = m_toRadius - m_fromRadius;
                if (m_radiusAnimationInterpolationType == InterpolationType.LINEAR)
                    deltaRadius = dt / m_radiusAnimationDuration * radiusVariation;
                else if (m_radiusAnimationInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaRadius = radiusVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_radiusAnimationDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_fadingDuration)));

                if (effectiveElapsedTime > m_radiusAnimationDuration)
                {
                    SetInnerRadius(m_toRadius);
                    m_animatingRadius = false;
                    OnFinishAnimatingRadius();
                }
                else
                    IncInnerRadius(deltaRadius);
            }
        }
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

        float dt = Time.deltaTime;
        UpdateRadius(dt);
    }

    public virtual void OnFinishAnimatingRadius()
    {
        if (m_radiusAnimationDestroyOnFinish)
            Destroy(this.gameObject);
    }
}