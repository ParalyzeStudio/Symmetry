using UnityEngine;

[ExecuteInEditMode]
public class ChapterSlot : MonoBehaviour
{
    public Color m_color;
    private Color m_prevColor;
    public int m_number;
    private int m_prevNumber;

    private TintColorMaterialAssignment m_skin;
    private TextMesh m_numberText;

    public void Start()
    {
        m_skin = this.gameObject.GetComponentInChildren<TintColorMaterialAssignment>();
        m_numberText = this.gameObject.GetComponentInChildren<TextMesh>();
    }

    public void Update()
    {
        if (m_color != m_prevColor)
        {
            m_prevColor = m_color;
            m_skin.m_tintColor = m_color;
        }

        if (m_number != m_prevNumber)
        {
            m_prevNumber = m_number;
            m_numberText.text = m_number.ToString();
        }
    }
}

