using UnityEngine;

public class HexagonMeshAnimator : GameObjectAnimator
{
    public float m_thickness { get; set; }

    //Radius animation
    protected bool m_animatingRadius;
    public float m_innerRadius { get; set; }
    protected float m_fromRadius;
    protected float m_toRadius;
    protected float m_radiusAnimationDuration;
    protected float m_radiusAnimationDelay;
    protected float m_radiusAnimationElapsedTime;
    protected InterpolationType m_radiusAnimationInterpolationType;

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

    public void SetThickness(float thickness, bool bRenderCircle = true)
    {
        m_thickness = thickness;
        if (bRenderCircle)
            RenderHexagon();
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
        hexaMesh.Render(m_innerRadius, m_thickness, m_color);
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
                }
                else
                    IncInnerRadius(deltaRadius);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;
        UpdateRadius(dt);
    }
}
