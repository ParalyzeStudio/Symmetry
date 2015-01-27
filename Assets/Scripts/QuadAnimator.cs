using UnityEngine;
using System.Collections;

public class QuadAnimator : MonoBehaviour 
{
    //Variables to handle fading
    private bool m_fading;
    private float m_opacity;
    private float m_fromOpacity;
    private float m_toOpacity;
    private float m_fadingDuration;
    private float m_fadingElapsedTime;

    //Variables to handle scaling
    private bool m_scaling;
    private Vector2 m_scale;
    private Vector2 m_fromScale;
    private Vector2 m_toScale;
    private float m_scalingDuration;
    private float m_scalingElapsedTime;

    //Variables to handle translating
    private bool m_translating;
    private Vector2 m_position;
    private Vector2 m_fromPosition;
    private Vector2 m_toPosition;
    private float m_translatingDuration;
    private float m_translatingElapsedTime;

    public void FadeFromTo(float fromOpacity, float toOpacity, float duration, float delay)
    {
        if (fromOpacity == toOpacity)
            return;

        m_fading = true;
        m_toOpacity = toOpacity;
        m_fadingDuration = duration;
        m_fadingElapsedTime = 0;
    }

    public void ScaleFromTo(Vector2 fromScale, Vector2 toScale, float duration, float delay)
    {
        m_scaling = true;
        m_toScale = toScale;
        m_scalingDuration = duration;
        m_scalingElapsedTime = 0;
    }

    public void TranslateFromTo(Vector2 fromPosition, Vector2 toPosition, float duration, float delay)
    {
        m_translating = true;
        m_toPosition = toPosition;
        m_translatingDuration = duration;
        m_translatingElapsedTime = 0;
    }

	protected void Update () 
    {
        float dt = Time.deltaTime;

        if (m_fading)
        {
            float deltaOpacity = dt * (m_toOpacity - m_fromOpacity);
            if (deltaOpacity < 0 && (m_opacity + deltaOpacity) < m_toOpacity)
            {
                m_opacity = m_toOpacity;
                m_fading = false;
            }
            else if (deltaOpacity > 0 && (m_opacity + deltaOpacity) > m_toOpacity)
            {
                m_opacity = m_toOpacity;
                m_fading = false;
            }
        }

        if (m_translating)
        {
            Vector2 deltaPosition = dt * (m_toPosition - m_fromPosition);
            float sqrCoveredDistance = (m_position + deltaPosition - m_fromPosition).sqrMagnitude;
            float sqrTotalDistance = (m_toPosition - m_fromPosition).sqrMagnitude;
            if (sqrCoveredDistance > sqrTotalDistance)
            {
                m_position = m_toPosition;
                m_translating = false;
            }
        }

        if (m_scaling)
        {
            Vector2 deltaScale = dt * (m_toScale - m_fromScale);
            float sqrCoveredScale = (m_scale + deltaScale - m_fromScale).sqrMagnitude;
            float sqrTotalScale = (m_toScale - m_fromScale).sqrMagnitude;
            if (sqrCoveredScale > sqrTotalScale)
            {
                m_scale = m_toScale;
                m_scaling = false;
            }
        }
	}
}
