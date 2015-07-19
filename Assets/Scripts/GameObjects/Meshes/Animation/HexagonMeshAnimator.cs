using UnityEngine;

public class HexagonMeshAnimator : GameObjectAnimator
{
    //public float m_thickness { get; set; }
    public float m_innerRadius { get; set; }
    public float m_outerRadius { get; set; }

    //thickness animation
    protected bool m_animatingThickness;
    protected float m_fromThickness;
    protected float m_toThickness;
    protected float m_thicknessAnimationDuration;
    protected float m_thicknessAnimationDelay;
    protected float m_thicknessAnimationElapsedTime;
    protected InterpolationType m_thicknessAnimationInterpolationType;

    //inner radius animation
    protected bool m_animatingInnerRadius;
    //protected bool m_maintainThicknessOnInnerRadiusAnimation;
    protected float m_fromInnerRadius;
    protected float m_toInnerRadius;
    protected float m_innerRadiusAnimationDuration;
    protected float m_innerRadiusAnimationDelay;
    protected float m_innerRadiusAnimationElapsedTime;
    protected InterpolationType m_innerRadiusAnimationInterpolationType;

    //outer radius animation
    protected bool m_animatingOuterRadius;
    protected float m_fromOuterRadius;
    protected float m_toOuterRadius;
    protected float m_outerRadiusAnimationDuration;
    protected float m_outerRadiusAnimationDelay;
    protected float m_outerRadiusAnimationElapsedTime;
    protected InterpolationType m_outerRadiusAnimationInterpolationType;

    public void SetInnerRadius(float radius, bool bRenderCircle = true)
    {
        m_innerRadius = radius;
        if (bRenderCircle)
            RenderHexagon();
    }

    public virtual void IncInnerRadius(float deltaRadius)
    {
        float fRadius = m_innerRadius + deltaRadius;
        SetInnerRadius(fRadius);
    }

    public void SetOuterRadius(float radius, bool bRenderCircle = true)
    {
        m_outerRadius = radius;
        if (bRenderCircle)
            RenderHexagon();
    }

    public virtual void IncOuterRadius(float deltaRadius)
    {
        float fRadius = m_outerRadius + deltaRadius;
        SetOuterRadius(fRadius);
    }
    
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        HexagonMesh hexaMesh = this.GetComponent<HexagonMesh>();
        hexaMesh.SetColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        HexagonMesh hexaMesh = this.GetComponent<HexagonMesh>();
        hexaMesh.SetColor(color);
    }

    public void RenderHexagon()
    {
        HexagonMesh hexaMesh = this.GetComponent<HexagonMesh>();
        hexaMesh.Render(m_innerRadius, m_outerRadius, m_color);
    }

    public void AnimateInnerRadiusTo(float toRadius, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        if (m_innerRadius == toRadius)
            return;

        m_animatingInnerRadius = true;
        m_fromInnerRadius = m_innerRadius;
        m_toInnerRadius = toRadius;
        m_innerRadiusAnimationDuration = duration;
        m_innerRadiusAnimationDelay = delay;
        m_innerRadiusAnimationElapsedTime = 0;
        m_innerRadiusAnimationInterpolationType = interpolType;
    }

    public void AnimateOuterRadiusTo(float toRadius, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        if (m_outerRadius == toRadius)
            return;

        m_animatingOuterRadius = true;
        m_fromOuterRadius = m_outerRadius;
        m_toOuterRadius = toRadius;
        m_outerRadiusAnimationDuration = duration;
        m_outerRadiusAnimationDelay = delay;
        m_outerRadiusAnimationElapsedTime = 0;
        m_outerRadiusAnimationInterpolationType = interpolType;
    }

    //public void AnimateThicknessTo(float toThickness, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    //{
    //    if (m_thickness == toThickness)
    //        return;

    //    m_animatingThickness = true;
    //    m_fromThickness = m_thickness;
    //    m_toThickness = toThickness;
    //    m_thicknessAnimationDuration = duration;
    //    m_thicknessAnimationDelay = delay;
    //    m_thicknessAnimationElapsedTime = 0;
    //    m_thicknessAnimationInterpolationType = interpolType;
    //}

    protected void UpdateInnerRadius(float dt)
    {
        if (m_animatingInnerRadius)
        {
            bool inDelay = (m_innerRadiusAnimationElapsedTime < m_innerRadiusAnimationDelay);
            m_innerRadiusAnimationElapsedTime += dt;
            if (m_innerRadiusAnimationElapsedTime >= m_innerRadiusAnimationDelay)
            {
                if (inDelay) //we were in delay previously
                {
                    dt = m_innerRadiusAnimationElapsedTime - m_innerRadiusAnimationDelay;
                }
                float effectiveElapsedTime = m_innerRadiusAnimationElapsedTime - m_innerRadiusAnimationDelay;
                float deltaRadius = 0;
                float radiusVariation = m_toInnerRadius - m_fromInnerRadius;
                if (m_innerRadiusAnimationInterpolationType == InterpolationType.LINEAR)
                    deltaRadius = dt / m_innerRadiusAnimationDuration * radiusVariation;
                else if (m_innerRadiusAnimationInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaRadius = radiusVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_innerRadiusAnimationDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_fadingDuration)));

                if (effectiveElapsedTime > m_innerRadiusAnimationDuration)
                {
                    SetInnerRadius(m_toInnerRadius);
                    m_animatingInnerRadius = false;
                }
                else
                    IncInnerRadius(deltaRadius);
            }
        }
    }

    protected void UpdateOuterRadius(float dt)
    {
        if (m_animatingOuterRadius)
        {
            bool inDelay = (m_outerRadiusAnimationElapsedTime < m_outerRadiusAnimationDelay);
            m_outerRadiusAnimationElapsedTime += dt;
            if (m_outerRadiusAnimationElapsedTime >= m_outerRadiusAnimationDelay)
            {
                if (inDelay) //we were in delay previously
                {
                    dt = m_outerRadiusAnimationElapsedTime - m_outerRadiusAnimationDelay;
                }
                float effectiveElapsedTime = m_outerRadiusAnimationElapsedTime - m_outerRadiusAnimationDelay;
                float deltaRadius = 0;
                float radiusVariation = m_toOuterRadius - m_fromOuterRadius;
                if (m_outerRadiusAnimationInterpolationType == InterpolationType.LINEAR)
                    deltaRadius = dt / m_outerRadiusAnimationDuration * radiusVariation;
                else if (m_outerRadiusAnimationInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaRadius = radiusVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_outerRadiusAnimationDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_fadingDuration)));

                if (effectiveElapsedTime > m_outerRadiusAnimationDuration)
                {
                    SetOuterRadius(m_toOuterRadius);
                    m_animatingInnerRadius = false;
                }
                else
                    IncOuterRadius(deltaRadius);
            }
        }
    }

    //protected void UpdateThickness(float dt)
    //{
    //    if (m_animatingThickness)
    //    {
    //        bool inDelay = (m_thicknessAnimationElapsedTime < m_thicknessAnimationDelay);
    //        m_thicknessAnimationElapsedTime += dt;
    //        if (m_thicknessAnimationElapsedTime >= m_thicknessAnimationDelay)
    //        {
    //            if (inDelay) //we were in delay previously
    //            {
    //                dt = m_thicknessAnimationElapsedTime - m_thicknessAnimationDelay;
    //            }
    //            float effectiveElapsedTime = m_thicknessAnimationElapsedTime - m_thicknessAnimationDelay;
    //            float deltaThickness = 0;
    //            float thicknessVariation = m_toThickness - m_fromThickness;
    //            if (m_thicknessAnimationInterpolationType == InterpolationType.LINEAR)
    //                deltaThickness = dt / m_thicknessAnimationDuration * thicknessVariation;
    //            else if (m_thicknessAnimationInterpolationType == InterpolationType.SINUSOIDAL)
    //                deltaThickness = thicknessVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_thicknessAnimationDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_fadingDuration)));

    //            if (effectiveElapsedTime > m_thicknessAnimationDuration)
    //            {
    //                SetThickness(m_toThickness);
    //                m_animatingThickness = false;
    //            }
    //            else
    //                IncThickness(deltaThickness);
    //        }
    //    }
    //}

    protected override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;
        UpdateInnerRadius(dt);
        UpdateOuterRadius(dt);
    }
}
