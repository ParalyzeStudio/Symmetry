using UnityEngine;

[ExecuteInEditMode]
public class TintColorMaterialAssignment : MaterialAssignment
{
    public Color m_tintColor;
    private Color m_prevTintColor;

    public override void Update()
    {
        base.Update();
        if (m_tintColor != m_prevTintColor)
        {
            m_prevTintColor = m_tintColor;
            this.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", m_tintColor);
        }
    }
}