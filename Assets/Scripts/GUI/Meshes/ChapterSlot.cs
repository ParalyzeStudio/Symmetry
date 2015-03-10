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

    public void Init()
    {
        m_skin = this.gameObject.GetComponentInChildren<TintColorMaterialAssignment>();
        m_skin.InitMeshRendererMaterial();

        m_numberText = this.gameObject.GetComponentInChildren<TextMesh>();
    }

    public void SetColor(Color color)
    {
        m_color = color;
        m_prevColor = color;
        m_skin.SetTintColor(color);
    }

    public void SetNumber(int number)
    {
        m_number = number;
        m_prevNumber = m_number;
        m_numberText.text = m_number.ToString();
    }

    public void Update()
    {
        //if (m_color != m_prevColor)
        //{
        //    SetColor(m_color);
        //}

        //if (m_number != m_prevNumber)
        //{
        //    SetNumber(m_number);
        //}
    }
}

