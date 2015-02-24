using UnityEngine;

public class TextMeshAnimator : GameObjectAnimator
{
    private TextMesh m_textMesh;

    //Variables handling fade in/fade out sequence
    private bool m_opacityCycling;
    public float m_cycleDuration; //The duration of one cycle (e.g going from opacity 1 to opacity 0 and going back to opacity 1)
    private bool m_ascending; //is the opacity in an ascending phase (0->1)
    private bool m_cyclingPaused;

    public override void Awake()
    {
        base.Awake();
        m_textMesh = null;
        m_opacityCycling = false;
        m_cyclingPaused = false;
    }

    public void SetTextMeshOpacityCycling(float fCycleDuration, bool bAscending, float fDelay = 0.0f, bool paused = false)
    {
        m_opacityCycling = true;
        m_cycleDuration = fCycleDuration;
        m_ascending = bAscending;
        m_cyclingPaused = paused;
    }

    public void SetCyclingPaused(bool bPaused)
    {
        m_cyclingPaused = bPaused;
    }

    public override void OnOpacityChanged(float fNewOpacity)
    {
        if (m_textMesh == null)
            m_textMesh = this.gameObject.GetComponent<TextMesh>();

        Color newTextColor = new Color(m_textMesh.color.r, m_textMesh.color.g, m_textMesh.color.b, fNewOpacity);
        m_textMesh.color = newTextColor;
    }

    /**
     * Update the opacity using fade-in/fade-out cycles and not calling the base implementation
     * **/
    protected override void UpdateOpacity(float dt)
    {
        if (m_opacityCycling)
        {
            if (m_cyclingPaused)
                return;

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
        else
            base.UpdateOpacity(dt);
    }
}
