using UnityEngine;

public class ColorSegmentAnimator : SegmentAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        ColorSegment colorSegment = this.GetComponent<ColorSegment>();
        colorSegment.SetColor(m_color);       
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        ColorSegment colorSegment = this.GetComponent<ColorSegment>();
        colorSegment.SetColor(m_color);
    }
}

