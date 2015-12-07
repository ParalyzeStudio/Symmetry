using UnityEngine;

public class StripAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        StripMesh stripMesh = this.GetComponent<StripMesh>();
        stripMesh.SetColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        StripMesh stripMesh = this.GetComponent<StripMesh>();
        stripMesh.SetColor(color);
    }
}