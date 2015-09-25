using UnityEngine;

public class RibbonAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        RibbonMesh ribbonMesh = this.GetComponent<RibbonMesh>();
        ribbonMesh.SetColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        RibbonMesh ribbonMesh = this.GetComponent<RibbonMesh>();
        ribbonMesh.SetColor(color);
    }
}