using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ValueAnimator : MonoBehaviour 
{
    //Variables to handle fading
    protected bool m_fading;
    public float m_opacity;
    protected float m_fromOpacity;
    protected float m_toOpacity;
    protected float m_fadingDuration;
    protected float m_fadingDelay;
    protected float m_fadingElapsedTime;

    //Variables to handle scaling
    protected bool m_scaling;
    protected Vector3 m_scale;
    protected Vector3 m_fromScale;
    protected Vector3 m_toScale;
    protected float m_scalingDuration;
    protected float m_scalingDelay;
    protected float m_scalingElapsedTime;

    //Variables to handle translating
    protected bool m_translating;
    protected Vector3 m_position;
    protected Vector3 m_fromPosition;
    protected Vector3 m_toPosition;
    protected float m_translatingDuration;
    protected float m_translatingDelay;
    protected float m_translatingElapsedTime;

    //Variables to handle rotating
    protected bool m_rotating;
    protected Vector3 m_rotationAxis;
    protected float m_angle;
    protected float m_fromAngle;
    protected float m_toAngle;
    protected float m_rotatingDuration;
    protected float m_rotatingDelay;
    protected float m_rotatingElapsedTime;

    //Store previous values to change them dynamically in inspector
    private float m_prevOpacity;


    public void FadeFromTo(float fromOpacity, float toOpacity, float duration, float delay = 0.0f)
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
    }

    public void ScaleFromTo(Vector3 fromScale, Vector3 toScale, float duration, float delay = 0.0f)
    {
        m_scaling = true;
        m_scale = fromScale;
        m_fromScale = fromScale;
        m_toScale = toScale;
        m_scalingDuration = duration;
        m_scalingDelay = delay;
        m_scalingElapsedTime = 0;
    }

    public void TranslateFromTo(Vector3 fromPosition, Vector3 toPosition, float duration, float delay = 0.0f)
    {
        m_translating = true;
        m_position = fromPosition;
        m_fromPosition = fromPosition;
        m_toPosition = toPosition;
        m_translatingDuration = duration;
        m_translatingDelay = delay;
        m_translatingElapsedTime = 0;
    }

    public void RotateFromToAroundAxis(float fromAngle, float toAngle, Vector3 axis, float duration, float delay = 0.0f)
    {
        m_rotating = true;
        m_angle = fromAngle;
        m_fromAngle = fromAngle;
        m_toAngle = toAngle;
        m_rotationAxis = axis;
        m_rotatingDuration = duration;
        m_rotatingDelay = delay;
        m_rotatingElapsedTime = 0;
    }

    protected virtual void UpdateOpacity(float dt)
    {
        if (m_fading)
        {
            m_fadingElapsedTime += dt;
            if (m_fadingElapsedTime > m_fadingDelay)
            {
                float deltaOpacity = dt / m_fadingDuration * (m_toOpacity - m_fromOpacity);
                if (deltaOpacity < 0 && (m_opacity + deltaOpacity) < m_toOpacity
                ||
                deltaOpacity > 0 && (m_opacity + deltaOpacity) > m_toOpacity)
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
            m_translatingElapsedTime += dt;
            if (m_translatingElapsedTime > m_translatingDelay)
            {
                Vector3 deltaPosition = dt / m_translatingDuration * (m_toPosition - m_fromPosition);
                m_position += deltaPosition;
                float sqrCoveredDistance = (m_position - m_fromPosition).sqrMagnitude;
                float sqrTotalDistance = (m_toPosition - m_fromPosition).sqrMagnitude;

                if (sqrCoveredDistance > sqrTotalDistance)
                {
                    m_position = m_toPosition;
                    m_translating = false;
                    OnFinishTranslating();
                }

                OnPositionChanged(m_position);
            }
        }
    }

    protected void UpdateRotation(float dt)
    {
        if (m_rotating)
        {
            m_rotatingElapsedTime += dt;
            if (m_rotatingElapsedTime > m_rotatingDelay)
            {
                float deltaAngle = dt / m_rotatingDuration * (m_toAngle - m_fromAngle);
                m_angle += deltaAngle;
                if (deltaAngle < 0 && (m_angle + deltaAngle) < m_toAngle
                    ||
                    deltaAngle > 0 && (m_angle + deltaAngle) > m_toAngle)
                {
                    m_angle = m_toAngle;
                    m_rotating = false;
                    OnFinishRotating();
                }

                OnRotationChanged(m_angle, m_rotationAxis);
            }
        }
    }

    protected virtual void UpdateScale(float dt)
    {
        if (m_scaling)
        {
            m_scalingElapsedTime += dt;
            if (m_scalingElapsedTime > m_scalingDelay)
            {
                Vector3 deltaScale = dt / m_scalingDuration * (m_toScale - m_fromScale);
                m_scale += deltaScale;
                float sqrCoveredScale = (m_scale - m_fromScale).sqrMagnitude;
                float sqrTotalScale = (m_toScale - m_fromScale).sqrMagnitude;
                if (sqrCoveredScale > sqrTotalScale)
                {
                    m_scale = m_toScale;
                    m_scaling = false;
                    OnFinishScaling();
                }

                OnScaleChanged(m_scale);
            }
        }
    }

	protected virtual void Update () 
    {
        float dt = Time.deltaTime;

        if (m_prevOpacity != m_opacity)
            OnOpacityChanged(m_opacity);

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
