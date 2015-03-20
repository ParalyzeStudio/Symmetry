using UnityEngine;

public class SlotAnimator : GameObjectAnimator
{
    //public override void OnOpacityChanged(float fNewOpacity)
    //{
    //    base.OnOpacityChanged(fNewOpacity);

    //    ColorNumberSlot slotProperties = this.gameObject.GetComponent<ColorNumberSlot>();
    //    Color slotColor = slotProperties.m_color;
    //    slotColor.a = fNewOpacity;

    //    slotProperties.SetColor(slotColor);
    //}

    public override void SetOpacity(float fNewOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fNewOpacity, bPassOnChildren);

        ColorNumberSlot slotProperties = this.gameObject.GetComponent<ColorNumberSlot>();
        Color slotColor = slotProperties.m_color;
        slotColor.a = fNewOpacity;

        slotProperties.SetColor(slotColor);
    }
}
