﻿using UnityEngine;

/**
 * Data structure to hold information on a slot (level/chapter) with a color and a number
 * **/
[ExecuteInEditMode]
public class ColorNumberSlot : MonoBehaviour
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
        if (number >= 10 && number <= 99) //2-digits number
        {
            Vector3 numberTextPosition = m_numberText.transform.localPosition;
            float horizontalOffset = (number == 11) ? -2.0f : -8.0f;
            m_numberText.transform.localPosition = new Vector3(numberTextPosition.x + horizontalOffset , numberTextPosition.y, numberTextPosition.z);
        }
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
