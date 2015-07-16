using UnityEngine;

[ExecuteInEditMode]
public class TextMeshAnimator : GameObjectAnimator
{
    //Variables handling fade in/fade out sequence
    //private bool m_opacityCycling;
    //public float m_cycleDuration; //The duration of one cycle (e.g going from opacity 1 to opacity 0 and going back to opacity 1)
    //private bool m_ascending; //is the opacity in an ascending phase (0->1)
    //private float m_cyclingDelay;
    //private bool m_cyclingPaused;

    public float m_fontHeight;
    private float m_prevFontHeight;

    public override void Awake()
    {
        base.Awake();
        //m_opacityCycling = false;
        //m_cyclingDelay = 0.0f;
        //m_cyclingPaused = false;
    }

    /**
     * Set the font height in unity screen units on this TextMesh
     * **/
    public void SetFontHeight(float fontHeight)
    {
        m_fontHeight = fontHeight;
        m_prevFontHeight = fontHeight;

        float screenHeight = Screen.height; //take the physical resolution height of this screen

        //calculate the character size
        float characterSize = -0.019f * screenHeight + 27.238f;

        //Calulate the constant a for the equation characterSize = a / fontSize
        float a = 12.7f * fontHeight;

        //easily determine the fontSize
        int fontSize = Mathf.RoundToInt(a / characterSize);

        //Set those values to the TextMesh component
        TextMesh textMesh = this.gameObject.GetComponent<TextMesh>();
        textMesh.characterSize = characterSize;
        textMesh.fontSize = fontSize;
    }

    //public void SetTextMeshOpacityCycling(float fCycleDuration, bool bAscending, float fDelay = 0.0f, bool paused = false)
    //{
    //    m_opacityCycling = true;
    //    m_cycleDuration = fCycleDuration;
    //    m_ascending = bAscending;
    //    m_fadingElapsedTime = 0;
    //    m_cyclingDelay = fDelay;
    //    m_cyclingPaused = paused;
    //}

    //public void SetCyclingPaused(bool bPaused)
    //{
    //    m_cyclingPaused = bPaused;
    //}

    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);
            
        TextMesh textMesh = this.gameObject.GetComponent<TextMesh>();

        Color newTextColor = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, fOpacity);
        textMesh.color = newTextColor;
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        TextMesh textMesh = this.gameObject.GetComponent<TextMesh>();
        textMesh.color = color;
    }

    /**
     * Update the opacity using fade-in/fade-out cycles and not calling the base implementation
     * **/
    //protected override void UpdateOpacity(float dt)
    //{
    //    if (m_opacityCycling)
    //    {
    //        if (m_cyclingPaused)
    //            return;

    //        m_fadingElapsedTime += dt;
    //        if (m_fadingElapsedTime <= m_cyclingDelay)
    //            return;

    //        float phaseDuration = 0.5f * m_cycleDuration;
    //        float absDiffOpacity = dt / phaseDuration;
    //        float dOpacity;
    //        if (m_ascending)
    //        {
    //            dOpacity = absDiffOpacity;
    //            IncOpacity(dOpacity);
    //            if (m_opacity >= 1)
    //            {
    //                m_ascending = false;
    //            }
    //        }
    //        else
    //        {
    //            dOpacity = -absDiffOpacity;
    //            IncOpacity(dOpacity);
    //            if (m_opacity <= 0)
    //            {
    //                m_ascending = true;
    //            }
    //        }
    //    }
    //    else
    //        base.UpdateOpacity(dt);
    //}

    protected override void Update()
    {
        base.Update();

        if (m_prevFontHeight != m_fontHeight)
        {
            SetFontHeight(m_fontHeight);
        }
    }
}
