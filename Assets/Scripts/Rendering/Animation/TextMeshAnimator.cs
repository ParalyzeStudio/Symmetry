using UnityEngine;

[ExecuteInEditMode]
public class TextMeshAnimator : GameObjectAnimator
{
    public float m_fontHeight;
    private float m_prevFontHeight;

    public override void Awake()
    {
        base.Awake();
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

    protected override void Update()
    {
        base.Update();

        if (m_prevFontHeight != m_fontHeight)
        {
            SetFontHeight(m_fontHeight);
        }
    }
}
