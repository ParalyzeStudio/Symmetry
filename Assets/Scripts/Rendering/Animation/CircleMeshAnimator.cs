using UnityEngine;

[ExecuteInEditMode]
public class CircleMeshAnimator : GameObjectAnimator
{
    //inner and outer radiuses of the circle
    //public float m_innerRadius { get; set; }
    //public float m_outerRadius { get; set; }

    public float m_innerRadius;
    public float m_outerRadius;

    private float m_prevInnerRadius;
    private float m_prevOuterRadius;

    //number of segments in this circle
    public int m_numSegments { get; set; }

    //inner radius animation
    protected bool m_animatingInnerRadius;
    protected float m_fromInnerRadius;
    protected float m_toInnerRadius;
    protected float m_innerRadiusAnimationDuration;
    protected float m_innerRadiusAnimationDelay;
    protected float m_innerRadiusAnimationElapsedTime;
    protected InterpolationType m_innerRadiusAnimationInterpolationType;
    protected bool m_innerRadiusAnimationDestroyOnFinish;

    //outer radius animation
    protected bool m_animatingOuterRadius;
    protected float m_fromOuterRadius;
    protected float m_toOuterRadius;
    protected float m_outerRadiusAnimationDuration;
    protected float m_outerRadiusAnimationDelay;
    protected float m_outerRadiusAnimationElapsedTime;
    protected InterpolationType m_outerRadiusAnimationInterpolationType;
    protected bool m_outerRadiusAnimationDestroyOnFinish;

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

    public void SetOuterRadius(float radius, bool bRenderCircle = true)
    {
        m_outerRadius = radius;
        m_prevOuterRadius = radius;
        if (bRenderCircle)
            RenderCircle();
    }

    public virtual void IncOuterRadius(float deltaRadius)
    {
        float fRadius = m_outerRadius + deltaRadius;
        SetOuterRadius(fRadius);
    }

    public void SetNumSegments(int numSegments, bool bRenderCircle = true)
    {
        m_numSegments = numSegments;
        if (bRenderCircle)
            RenderCircle();
    }

    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        CircleMesh circleMesh = this.GetComponent<CircleMesh>();
        circleMesh.SetColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        CircleMesh circleMesh = this.GetComponent<CircleMesh>();
        circleMesh.SetColor(m_color);
    }

    public void RenderCircle()
    {
        CircleMesh circle = this.GetComponent<CircleMesh>();
        circle.Render(m_innerRadius, m_outerRadius, m_color, m_numSegments, m_angle);
    }

    public void AnimateInnerRadiusTo(float toRadius, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyOnFinish = false)
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
        m_innerRadiusAnimationDestroyOnFinish = bDestroyOnFinish;
    }

    public void AnimateOuterRadiusTo(float toRadius, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyOnFinish = false)
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
        m_outerRadiusAnimationDestroyOnFinish = bDestroyOnFinish;
    }

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
                    deltaRadius = radiusVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_innerRadiusAnimationDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_innerRadiusAnimationDuration)));

                if (effectiveElapsedTime > m_innerRadiusAnimationDuration)
                {
                    SetInnerRadius(m_toInnerRadius);
                    m_animatingInnerRadius = false;
                    OnFinishAnimatingInnerRadius();
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
                    deltaRadius = radiusVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_outerRadiusAnimationDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_outerRadiusAnimationDuration)));

                if (effectiveElapsedTime > m_outerRadiusAnimationDuration)
                {
                    SetOuterRadius(m_toOuterRadius);
                    m_animatingOuterRadius = false;
                    OnFinishAnimatingOuterRadius();
                }
                else
                    IncOuterRadius(deltaRadius);
            }
        }
    }

    /***
     * Callbacks
     * */

    public virtual void OnFinishAnimatingInnerRadius()
    {
        if (m_innerRadiusAnimationDestroyOnFinish)
            Destroy(this.gameObject);  
    }

    public virtual void OnFinishAnimatingOuterRadius()
    {
        if (m_outerRadiusAnimationDestroyOnFinish)
            Destroy(this.gameObject);
    }

    protected override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;
        UpdateInnerRadius(dt);
        UpdateOuterRadius(dt);

        if (m_prevInnerRadius != m_innerRadius)
        {
            SetInnerRadius(m_innerRadius);
        }

        if (m_prevOuterRadius != m_outerRadius)
        {
            SetOuterRadius(m_outerRadius);
        }
    }
}