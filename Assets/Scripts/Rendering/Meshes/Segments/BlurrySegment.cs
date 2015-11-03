using UnityEngine;

public class BlurrySegment : MonoBehaviour
{
    public GameObject m_sharpSegmentObject;
    public GameObject m_blurrySegmentObject;
    private SimplifiedRoundedSegment m_sharpSegment;
    private SimplifiedRoundedSegment m_blurrySegment;

    //parent segment tree if this segment is part of a group and child node
    public SegmentTree m_parentTree { get; set; }
    public SegmentTreeNode m_endTreeNode { get; set; }

    public void Build(Vector3 pointA, 
                      Vector3 pointB, 
                      float sharpSegmentThickness, 
                      float blurrySegmentThickness,
                      Material sharpSegmentMaterial,
                      Material blurrySegmentMaterial,
                      Color sharpSegmentColor, 
                      Color blurrySegmentColor)
    {
        //sharp segment
        m_sharpSegment = m_sharpSegmentObject.GetComponent<SimplifiedRoundedSegment>();
        m_sharpSegment.Build(pointA, pointB, sharpSegmentThickness, sharpSegmentMaterial, sharpSegmentColor);
        
        //blurry segment
        m_blurrySegment = m_blurrySegmentObject.GetComponent<SimplifiedRoundedSegment>();
        m_blurrySegment.Build(pointA, pointB, blurrySegmentThickness, blurrySegmentMaterial, blurrySegmentColor);

        SegmentAnimator segmentAnimator = this.GetComponent<SegmentAnimator>();
        segmentAnimator.SetPointAPosition(pointA);
        segmentAnimator.SetPointBPosition(pointB);
    }

    /**
     * Set new coordinates for pointA. Set bGridPoint to true is the passed pointB is in grid coordinates
     * **/
    public virtual void SetPointA(Vector2 pointA, bool bRenderSegment = true)
    {
        m_sharpSegment.SetPointA(pointA, bRenderSegment);
        m_blurrySegment.SetPointA(pointA, bRenderSegment);
    }

    /**
     * Set new coordinates for pointB. Set bGridPoint to true is the passed pointB is in grid coordinates
     * **/
    public virtual void SetPointB(Vector2 pointB, bool bRenderSegment = true)
    {
        m_sharpSegment.SetPointB(pointB, bRenderSegment);
        m_blurrySegment.SetPointB(pointB, bRenderSegment);
    }
}