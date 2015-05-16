using UnityEngine;

/**
 * Data structure to hold information on a slot (level/chapter) with a color and a number
 * **/
[ExecuteInEditMode]
public class ColorNumberSlot : MonoBehaviour
{
    private ColorQuad m_skin;
    private TextMesh m_numberText;

    public Material m_skinMaterial;

    public void Init()
    {
        //Init the skin
        m_skin = this.gameObject.GetComponentInChildren<ColorQuad>();
        m_skin.InitQuadMesh();
        Material clonedMaterial = (Material)Instantiate(m_skinMaterial);
        m_skin.SetMaterial(clonedMaterial);

        m_numberText = this.gameObject.GetComponentInChildren<TextMesh>();
    }

    public void SetColor(Color color)
    {
        ColorQuadAnimator skinAnimator = m_skin.gameObject.GetComponent<ColorQuadAnimator>();
        skinAnimator.SetColor(color);
    }

    public void SetNumber(int number)
    {
        m_numberText.text = number.ToString();
        if (number >= 10 && number <= 99) //2-digits number
        {
            Vector3 numberTextPosition = m_numberText.transform.localPosition;
            float horizontalOffset = (number == 11) ? -2.0f : -8.0f;
            m_numberText.transform.localPosition = new Vector3(numberTextPosition.x + horizontalOffset , numberTextPosition.y, numberTextPosition.z);
        }
    }
}

