using UnityEngine;

public class SlotAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fNewOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fNewOpacity, bPassOnChildren);

        ColorNumberSlot slot = this.gameObject.GetComponent<ColorNumberSlot>();
        m_color.a = fNewOpacity;

        slot.SetColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        ColorNumberSlot slot = this.gameObject.GetComponent<ColorNumberSlot>();
        slot.SetColor(color);
    }
}
