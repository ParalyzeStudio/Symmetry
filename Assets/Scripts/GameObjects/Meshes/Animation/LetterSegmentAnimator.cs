using UnityEngine;

public class LetterSegmentAnimator : SegmentAnimator
{
    public override void OnFinishTranslatingPointB()
    {
        TitleLetterSegment segment = this.gameObject.GetComponent<TitleLetterSegment>();
        TitleLetterVertex vertexB = segment.m_vertexB;
        vertexB.SpreadToNeighbors();
    }
}
