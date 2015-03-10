using UnityEngine;

[ExecuteInEditMode]
public class TintColorMaterialAssignment : MaterialAssignment
{
    public Color m_tintColor;
    private Color m_prevTintColor;

    public void Awake()
    {
        m_tintColor = m_prevTintColor;
    }

    public void SetTintColor(Color tintColor)
    {
        m_tintColor = tintColor;
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.SetColor("_Color", m_tintColor);
        m_prevTintColor = m_tintColor;
    }

    public override void Update()
    {
        base.Update();
        if (m_tintColor != m_prevTintColor)
        {
            SetTintColor(m_tintColor);
        }
    }
}