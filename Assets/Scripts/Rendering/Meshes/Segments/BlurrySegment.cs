using UnityEngine;

public class BlurrySegment : MonoBehaviour
{
    public GameObject m_sharpSegmentObject;
    public GameObject m_blurrySegmentObject;
    private ColorSegment m_sharpSegment;
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
        m_sharpSegment = m_sharpSegmentObject.GetComponent<ColorSegment>();
        m_sharpSegment.Build(pointA, pointB, sharpSegmentThickness, sharpSegmentMaterial, sharpSegmentColor, 4);
        
        //blurry segment
        m_blurrySegment = m_blurrySegmentObject.GetComponent<SimplifiedRoundedSegment>();
        m_blurrySegment.Build(pointA, pointB, blurrySegmentThickness, blurrySegmentMaterial, blurrySegmentColor);

        SegmentAnimator segmentAnimator = this.GetComponent<SegmentAnimator>();
        segmentAnimator.SetPosition(Vector3.zero);
    }

    /**
     * Set new coordinates for pointA. Set bGridPoint to true is the passed pointB is in grid coordinates
     * **/
    public virtual void SetPointA(Vector3 pointA, bool bRenderSegment = true)
    {
        m_sharpSegment.SetPointA(new Vector3(pointA.x, pointA.y, pointA.z - 1), bRenderSegment);
        m_blurrySegment.SetPointA(pointA, bRenderSegment);
    }

    /**
     * Set new coordinates for pointB. Set bGridPoint to true is the passed pointB is in grid coordinates
     * **/
    public virtual void SetPointB(Vector3 pointB, bool bRenderSegment = true)
    {
        m_sharpSegment.SetPointB(new Vector3(pointB.x, pointB.y, pointB.z - 1), bRenderSegment);
        m_blurrySegment.SetPointB(pointB, bRenderSegment);
    }
}