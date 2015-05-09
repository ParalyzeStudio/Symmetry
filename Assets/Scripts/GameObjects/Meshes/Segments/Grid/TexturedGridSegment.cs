using UnityEngine;

public class TexturedGridSegment : GridSegment
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