using UnityEngine;

public class ShapeAnimator : QuadAnimator
{
    public override void OnOpacityChanged(float fNewOpacity)
    {
        base.OnOpacityChanged(fNewOpacity);

        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        Color oldColor = shapeRenderer.m_color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, fNewOpacity);
        shapeRenderer.m_color = newColor;
    }
}
