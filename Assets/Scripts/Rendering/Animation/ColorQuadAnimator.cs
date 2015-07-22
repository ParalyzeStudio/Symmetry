using UnityEngine;

public class ColorQuadAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        ColorQuad colorQuad = this.GetComponent<ColorQuad>();
        colorQuad.SetColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        ColorQuad colorQuad = this.GetComponent<ColorQuad>();
        colorQuad.SetColor(color);
    }
}