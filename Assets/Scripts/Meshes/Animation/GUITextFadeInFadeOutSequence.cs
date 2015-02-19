using UnityEngine;
using UnityEngine.UI;

/**
 * Sequence to make a GUIText fade in and fade out for ever
 * **/
public class GUITextFadeInFadeOutSequence : ValueAnimator
{
    public float m_cycleDuration; //The duration of one cycle (e.g going from opacity 1 to opacity 0 and going back to opacity 1)
    private bool m_ascending; //is the opacity in an ascending phase (0->1)
    private bool m_paused;

    protected void Awake()
    {
        m_ascending = false;
        m_opacity = 1.0f;
        m_paused = false;
    }

    public void SetPaused(bool bPaused)
    {
        m_paused = bPaused;
    }

    protected override void Update()
    {
        if (m_paused)
            return;

        float dt = Time.deltaTime;

        float phaseDuration = 0.5f * m_cycleDuration;
        float absDiffOpacity = dt / phaseDuration;
        float dOpacity;
        if (m_ascending)
        {
            dOpacity = absDiffOpacity;
            m_opacity += dOpacity;
            if (m_opacity > 1)
            {
                m_opacity = 1;
                m_ascending = false;
            }
        }
        else
        {
            dOpacity = -absDiffOpacity;
            m_opacity += dOpacity;
            if (m_opacity < 0)
            {
                m_opacity = 0;
                m_ascending = true;
            }
        }

        OnOpacityChanged(m_opacity);
    }

    public override void OnOpacityChanged(float fNewOpacity)
    {
        TextMesh textMesh = this.GetComponent<TextMesh>();
        Color textColor = textMesh.color;
        textColor.a = fNewOpacity;
        textMesh.color = textColor;
    }
}

