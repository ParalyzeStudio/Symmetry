using UnityEngine;
using System.Collections;

public class ValueAnimator : MonoBehaviour 
{
    //Variables to handle fading
    protected bool m_fading;
    protected float m_opacity;
    protected float m_fromOpacity;
    protected float m_toOpacity;
    protected float m_fadingDuration;
    protected float m_fadingDelay;
    protected float m_fadingElapsedTime;

    //Variables to handle scaling
    protected bool m_scaling;
    protected Vector2 m_scale;
    protected Vector2 m_fromScale;
    protected Vector2 m_toScale;
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

    //TODO rotation

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

    public void ScaleFromTo(Vector2 fromScale, Vector2 toScale, float duration, float delay)
    {
        m_scaling = true;
        m_scale = fromScale;
        m_fromScale = fromScale;
        m_toScale = toScale;
        m_scalingDuration = duration;
        m_scalingDelay = delay;
        m_scalingElapsedTime = 0;
    }

    public void TranslateFromTo(Vector3 fromPosition, Vector3 toPosition, float duration, float delay)
    {
        m_translating = true;
        m_position = fromPosition;
        m_fromPosition = fromPosition;
        m_toPosition = toPosition;
        m_translatingDuration = duration;
        m_translatingDelay = delay;
        m_translatingElapsedTime = 0;
    }

	protected virtual void Update () 
    {
        float dt = Time.deltaTime;

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

        if (m_scaling)
        {
            m_scalingElapsedTime += dt;
            if (m_scalingElapsedTime > m_scalingDelay)
            {
                Vector2 deltaScale = dt / m_scalingDuration * (m_toScale - m_fromScale);
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

    public virtual void OnOpacityChanged(float fNewOpacity)
    {

    }

    public virtual void OnPositionChanged(Vector3 newPosition)
    {
        
    }

    public virtual void OnScaleChanged(Vector2 newScale)
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
}
