using UnityEngine;

public class TexturedSegmentAnimator : SegmentAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        TexturedSegment texturedSegment = this.GetComponent<TexturedSegment>();
        texturedSegment.SetTintColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        TexturedSegment texturedSegment = this.GetComponent<TexturedSegment>();
        texturedSegment.SetTintColor(color);
    }
}