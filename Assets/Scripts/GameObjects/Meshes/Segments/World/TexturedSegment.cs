using UnityEngine;

public class TexturedSegment : Segment
{

    protected virtual void UpdateUVs()
    {

    }

    public override void SetColor(Color color)
    {
        TexturedQuadAnimator segmentAnimator = this.GetComponent<TexturedQuadAnimator>();
        segmentAnimator.SetColor(color);
    }
}