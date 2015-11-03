using UnityEngine;

public class BlurrySegmentAnimator : SegmentAnimator
{
    public override void SetPointAPosition(Vector3 position)
    {
        m_pointAPosition = position;
        BlurrySegment segment = this.gameObject.GetComponent<BlurrySegment>();
        Vector3 sharpSegmentPosition = new Vector3(position.x, position.y, position.z - 1); //set sharp segment above blurry segment
        segment.m_sharpSegmentObject.GetComponent<SegmentAnimator>().SetPointAPosition(sharpSegmentPosition);
        segment.m_blurrySegmentObject.GetComponent<SegmentAnimator>().SetPointAPosition(position);
    }

    public override void SetPointBPosition(Vector3 position)
    {
        m_pointBPosition = position;
        BlurrySegment segment = this.gameObject.GetComponent<BlurrySegment>();
        Vector3 sharpSegmentPosition = new Vector3(position.x, position.y, position.z - 1); //set sharp segment above blurry segment
        segment.m_sharpSegmentObject.GetComponent<SegmentAnimator>().SetPointBPosition(sharpSegmentPosition);
        segment.m_blurrySegmentObject.GetComponent<SegmentAnimator>().SetPointBPosition(position);
    }

    public override void OnFinishTranslatingPointB()
    {
        BlurrySegment segment = this.GetComponent<BlurrySegment>();
        SegmentTreeNode endNode = segment.m_endTreeNode;
        if (endNode != null && endNode.m_active)
        {
            segment.m_parentTree.BuildSegmentsForNode(endNode, true);
            endNode.m_active = false; //deactivate this node as it already started emitting its children segments
        }
    }
}