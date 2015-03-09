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
            ValueAnimator animator = this.GetComponent<ValueAnimator>();
            if (animator)
                m_tintColor.a = animator.m_opacity;
            this.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", m_tintColor);
            m_prevTintColor = m_tintColor;
        }
    }
}