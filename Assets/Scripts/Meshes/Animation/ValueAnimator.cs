using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ValueAnimator : MonoBehaviour 
{
    public enum InterpolationType
    {
        LINEAR = 1,
        SINUSOIDAL = 2
    }

    //Variables to handle fading
    protected bool m_fading;
    public float m_opacity = 1;
    protected float m_fromOpacity;
    protected float m_toOpacity;
    protected float m_fadingDuration;
    protected float m_fadingDelay;
    protected float m_fadingElapsedTime;
    protected InterpolationType m_fadingInterpolationType;

    //Variables to handle scaling
    protected bool m_scaling;
    protected Vector3 m_scale;
    protected Vector3 m_fromScale;
    protected Vector3 m_toScale;
    protected float m_scalingDuration;
    protected float m_scalingDelay;
    protected float m_scalingElapsedTime;
    protected InterpolationType m_scalingInterpolationType;

    //Variables to handle translating
    protected bool m_translating;
    protected Vector3 m_position;
    protected Vector3 m_fromPosition;
    protected Vector3 m_toPosition;
    protected float m_translatingDuration;
    protected float m_translatingDelay;
    protected float m_translatingElapsedTime;
    protected float m_translatingTimeOffset;
    protected InterpolationType m_translatingInterpolationType;

    //Variables to handle rotating
    protected bool m_rotating;
    protected Vector3 m_rotationAxis;
    protected float m_angle;
    protected float m_fromAngle;
    protected float m_toAngle;
    protected float m_rotatingDuration;
    protected float m_rotatingDelay;
    protected float m_rotatingElapsedTime;
    protected InterpolationType m_rotatingInterpolationType;

    //Store previous values to change them dynamically in inspector
    private float m_prevOpacity = -1;

    public void FadeFromTo(float fromOpacity, float toOpacity, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        if (fromOpacity == toOpacity)
            return;

        m_fading = true;
        m_opacity = fromOpacity;
        m_fromOpacity = fromOpacity;
        m_toOpacity = toOpacity;
        m_fadingDuration = duration;
        m_fadingDelay = delay;
        m_fadingElapsedTime = 0;
        m_fadingInterpolationType = interpolType;
    }

    public void ScaleFromTo(Vector3 fromScale, Vector3 toScale, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_scaling = true;
        m_scale = fromScale;
        m_fromScale = fromScale;
        m_toScale = toScale;
        m_scalingDuration = duration;
        m_scalingDelay = delay;
        m_scalingElapsedTime = 0;
        m_scalingInterpolationType = interpolType;
    }

    public void TranslateFromTo(Vector3 fromPosition, Vector3 toPosition, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_translating = true;
        m_position = fromPosition;
        m_fromPosition = fromPosition;
        m_toPosition = toPosition;
        m_translatingDuration = duration;
        m_translatingDelay = delay;
        m_translatingElapsedTime = 0;
        m_translatingInterpolationType = interpolType;
    }

    public void RotateFromToAroundAxis(float fromAngle, float toAngle, Vector3 axis, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_rotating = true;
        m_angle = fromAngle;
        m_fromAngle = fromAngle;
        m_toAngle = toAngle;
        m_rotationAxis = axis;
        m_rotatingDuration = duration;
        m_rotatingDelay = delay;
        m_rotatingElapsedTime = 0;
        m_rotatingInterpolationType = interpolType;
    }

    protected virtual void UpdateOpacity(float dt)
    {
        if (m_fading)
        {
            bool inDelay = (m_fadingElapsedTime < m_fadingDelay);
            m_fadingElapsedTime += dt;
            if (m_fadingElapsedTime >= m_fadingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_fadingElapsedTime - m_fadingDelay;
                float effectiveElapsedTime = m_fadingElapsedTime - m_fadingDelay;
                float deltaOpacity = 0;
                float opacityVariation = m_toOpacity - m_fromOpacity;
                if (m_fadingInterpolationType == InterpolationType.LINEAR)
                    deltaOpacity = dt / m_fadingDuration * opacityVariation;
                else if (m_fadingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaOpacity = opacityVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_fadingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_fadingDuration)));

                if (effectiveElapsedTime > m_fadingDuration)
                {
                    m_opacity = m_toOpacity;
                    m_fading = false;
                    OnFinishFading();
                }
                else
                    m_opacity += deltaOpacity;
                OnOpacityChanged(m_opacity);
            }
        }
    }

    protected virtual void UpdatePosition(float dt)
    {      
        if (m_translating)
        {
            bool inDelay = (m_translatingElapsedTime < m_translatingDelay);
            m_translatingElapsedTime += dt;
            if (m_translatingElapsedTime >= m_translatingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_translatingElapsedTime - m_translatingDelay;
                m_translatingTimeOffset = m_translatingElapsedTime - m_translatingDelay;
                float effectiveElapsedTime = m_translatingElapsedTime - m_translatingDelay;
                Vector3 deltaPosition = Vector3.zero;
                Vector3 positionVariation = m_toPosition - m_fromPosition;
                if (m_translatingInterpolationType == InterpolationType.LINEAR)
                    deltaPosition = dt / m_translatingDuration * positionVariation;
                else if (m_translatingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaPosition = positionVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_translatingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_translatingDuration)));

                if (effectiveElapsedTime > m_translatingDuration)
                {
                    m_position = m_toPosition;
                    m_translating = false;
                    OnFinishTranslating();
                }
                else
                    m_position += deltaPosition;

                OnPositionChanged(m_position);
            }
        }
    }

    protected void UpdateRotation(float dt)
    {
        if (m_rotating)
        {
            bool inDelay = (m_rotatingElapsedTime < m_rotatingDelay);
            m_rotatingElapsedTime += dt;
            if (m_rotatingElapsedTime > m_rotatingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_rotatingElapsedTime - m_rotatingDelay;
                float effectiveElapsedTime = m_rotatingElapsedTime - m_rotatingDelay;
                float deltaAngle = 0;
                float angleVariation = m_toAngle - m_fromAngle;
                if (m_rotatingInterpolationType == InterpolationType.LINEAR)
                    deltaAngle = dt / m_rotatingDuration * angleVariation;
                else if (m_rotatingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaAngle = angleVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_rotatingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_rotatingDuration)));

                if (effectiveElapsedTime > m_rotatingDuration)
                {
                    m_angle = m_toAngle;
                    m_rotating = false;
                    OnFinishRotating();
                }
                else
                    m_angle += deltaAngle;

                OnRotationChanged(m_angle, m_rotationAxis);
            }
        }
    }

    protected virtual void UpdateScale(float dt)
    {
        if (m_scaling)
        {
            bool inDelay = (m_scalingElapsedTime < m_scalingDelay);
            m_scalingElapsedTime += dt;
            if (m_scalingElapsedTime > m_scalingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_scalingElapsedTime - m_scalingDelay;
                float effectiveElapsedTime = m_scalingElapsedTime - m_scalingDelay;
                Vector3 deltaScale = Vector3.zero;
                Vector3 scaleVariation = m_toScale - m_fromScale;
                if (m_scalingInterpolationType == InterpolationType.LINEAR)
                    deltaScale = dt / m_scalingDuration * scaleVariation;
                else if (m_scalingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaScale = scaleVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_scalingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_scalingDuration)));

                
                if (effectiveElapsedTime > m_scalingDuration)
                {
                    m_scale = m_toScale;
                    m_scaling = false;
                    OnFinishScaling();
                }
                else
                    m_scale += deltaScale;

                OnScaleChanged(m_scale);
            }
        }
    }

	protected virtual void Update () 
    {
        float dt = Time.deltaTime;

        if (m_prevOpacity != m_opacity)
        {
            m_prevOpacity = m_opacity;
            OnOpacityChanged(m_opacity);
        }

        UpdateOpacity(dt);
        UpdatePosition(dt);
        UpdateRotation(dt);
        UpdateScale(dt);
	}

    public virtual void OnOpacityChanged(float fNewOpacity)
    {
        if (fNewOpacity > 1)
            fNewOpacity = 1;
        else if (fNewOpacity < 0)
            fNewOpacity = 0;

        m_opacity = fNewOpacity;
        m_prevOpacity = fNewOpacity;
    }

    public virtual void OnPositionChanged(Vector3 newPosition)
    {
        
    }

    public virtual void OnScaleChanged(Vector3 newScale)
    {
        
    }

    public virtual void OnRotationChanged(float newAngle, Vector3 axis)
    {

    }

    public virtual void OnFinishFading()
    {
        
    }

    public virtual void OnFinishTranslating()
    {

    }

    public virtual void OnFinishScaling()
    {

    }

    public virtual void OnFinishRotating()
    {

    }
}
