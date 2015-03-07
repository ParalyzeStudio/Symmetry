using UnityEngine;

public class SegmentAnimator : TranspQuadOpacityAnimator
{
    /**
     * Resizes the segment to the new length passed as parameter
     * **/
    public void ResizeTo(float fNewLength, float fDuration, float fDelay = 0.0f)
    {
        Segment segment = this.gameObject.GetComponent<Segment>();
        Vector2 segmentFromScale = new Vector2(segment.m_length, segment.m_thickness);
        ScaleFromTo(segmentFromScale, new Vector2(fNewLength, segment.m_thickness), 1.0f, fDelay);
    }

    public override void OnScaleChanged(Vector3 newScale)
    {
        base.OnScaleChanged(newScale);

        Segment segment = this.gameObject.GetComponent<Segment>();
        segment.SetLength(newScale.x);
    }
}
