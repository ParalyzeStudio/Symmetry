using UnityEngine;

public class ShapeCellAnimator : GameObjectAnimator
{
    public override void SetOpacity(float opacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(opacity, bPassOnChildren);

        ShapeCell shapeCell = this.gameObject.GetComponent<ShapeCell>();
        shapeCell.SetOpacity(opacity);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        ShapeCell shapeCell = this.gameObject.GetComponent<ShapeCell>();
        shapeCell.SetColor(m_color);
    }
}

