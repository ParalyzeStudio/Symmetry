using UnityEngine;
using System.Collections.Generic;

public class SegmentTreeNode
{
    public Vector2 m_position { get; set; }
    public List<SegmentTreeNode> m_children { get; set; }
    public bool m_animationStartNode { get; set; }
    public bool m_active { get; set; }

    public SegmentTreeNode(Vector2 position)
    {
        m_position = position;
        m_children = new List<SegmentTreeNode>();
        m_animationStartNode = false;
        m_active = true;
    }

    public void AddChild(SegmentTreeNode child)
    {
        m_children.Add(child);
    }

    public void SetAnimationStartNode(bool startNode)
    {
        m_animationStartNode = startNode;
    }
}

/**
 * A structure that holds data to represent multiple segments
 * **/
public class SegmentTree : MonoBehaviour
{
    public enum SegmentType
    {
        COLOR = 1,
        TEXTURED,
        BLURRY
    }

    private SegmentType m_segmentType;

    public List<SegmentTreeNode> m_nodes { get; set; }
    private GameObject m_parentHolder;
    private GameObject m_segmentObjectPfb;
    private float m_segmentThickness1;
    private float m_segmentThickness2; //a second thickness is added in case of a BlurrySegment
    private Material m_segmentMaterial1;
    private Material m_segmentMaterial2; //a second material is added in case of a BlurrySegment
    private Color m_tintColor1;
    private Color m_tintColor2; //a second color is added in case of a BlurrySegment
 
    public void Awake()
    {
        m_nodes = new List<SegmentTreeNode>();
    }

    /**
     * Use this Init method in case we are dealing with simple color or textured segments inside the tree
     * **/
    public void Init(SegmentType segmentType,
                     GameObject parentHolder, 
                     GameObject segmentObjectPfb, 
                     float segmentThickness, 
                     Material segmentMaterial, 
                     Color tintColor)
    {
        if (segmentType != SegmentType.COLOR && segmentType != SegmentType.TEXTURED)
            throw new System.Exception("Segment type has to be either SegmentType.COLOR or SegmentType.TEXTURED");

        m_segmentType = segmentType;
        m_parentHolder = parentHolder;
        m_segmentObjectPfb = segmentObjectPfb;
        m_segmentThickness1 = segmentThickness;
        m_segmentMaterial1 = segmentMaterial;
        m_tintColor1 = tintColor;
    }

    /**
     * Use this Init method in case we are dealing with a BlurrySegment
     * **/
    public void Init(SegmentType segmentType,
                     GameObject parentHolder,
                     GameObject blurrySegmentObjectPfb,
                     float sharpSegmentThickness,
                     float blurThickness,
                     Material sharpSegmentMaterial,
                     Material blurMaterial,
                     Color sharpSegmentColor,
                     Color blurColor)
    {
        if (segmentType != SegmentType.BLURRY)
            throw new System.Exception("Segment type has to be SegmentType.BLURRY");

        m_segmentType = segmentType;
        m_parentHolder = parentHolder;
        m_segmentObjectPfb = blurrySegmentObjectPfb;
        m_segmentThickness1 = sharpSegmentThickness;
        m_segmentThickness2 = blurThickness;
        m_segmentMaterial1 = sharpSegmentMaterial;
        m_segmentMaterial2 = blurMaterial;
        m_tintColor1 = sharpSegmentColor;
        m_tintColor2 = blurColor;
    }

    public void BuildSegments(bool bAnimated)
    {
        if (bAnimated)
        {
            //Find the animation start node (there is only one)
            for (int i = 0; i != m_nodes.Count; i++)
            {
                if (m_nodes[i].m_animationStartNode)
                {
                    BuildSegmentsForNode(m_nodes[i], true);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i != m_nodes.Count; i++)
            {
                BuildSegmentsForNode(m_nodes[i], false);
            }
        }
    }


    /**
     * Build all segments starting from a single node to all its children
     * **/
    public void BuildSegmentsForNode(SegmentTreeNode node,
                                     bool bAnimated)
    {
        float segmentGrowSpeed = 500.0f;

        if (m_segmentType == SegmentType.COLOR || m_segmentType == SegmentType.TEXTURED)
        {
            for (int iChildIdx = 0; iChildIdx != node.m_children.Count; iChildIdx++)
            {
                SegmentTreeNode childNode = node.m_children[iChildIdx];

                GameObject segmentObject = (GameObject)Instantiate(m_segmentObjectPfb);
                TexturedSegment segment = segmentObject.GetComponent<TexturedSegment>();
                TexturedSegmentAnimator segmentAnimator = segmentObject.GetComponent<TexturedSegmentAnimator>();
                segmentAnimator.SetParentTransform(m_parentHolder.transform);

                if (bAnimated)
                {
                    segment.Build(GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                                  GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                                  m_segmentThickness1,
                                  m_segmentMaterial1,
                                  m_tintColor1,
                                  TextureWrapMode.Clamp);

                    float distanceToCover = (childNode.m_position - node.m_position).magnitude;

                    segmentAnimator.TranslatePointBTo(childNode.m_position, distanceToCover / segmentGrowSpeed);

                    node.m_active = false;
                }
                else
                {
                    segment.Build(GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                                  GeometryUtils.BuildVector3FromVector2(childNode.m_position, 0),
                                  m_segmentThickness1,
                                  m_segmentMaterial1,
                                  m_tintColor1,
                                  TextureWrapMode.Clamp);
                }

                segment.m_parentTree = this;
                segment.m_endTreeNode = childNode;
            }
        }
        else if (m_segmentType == SegmentType.BLURRY)
        {
            for (int iChildIdx = 0; iChildIdx != node.m_children.Count; iChildIdx++)
            {
                SegmentTreeNode childNode = node.m_children[iChildIdx];

                GameObject segmentObject = (GameObject)Instantiate(m_segmentObjectPfb);
                BlurrySegment segment = segmentObject.GetComponent<BlurrySegment>();
                BlurrySegmentAnimator segmentAnimator = segmentObject.GetComponent<BlurrySegmentAnimator>();
                segmentAnimator.SetParentTransform(m_parentHolder.transform);
                segmentAnimator.SetPosition(Vector3.zero);

                if (bAnimated)
                {
                    segment.Build(GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                                  GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                                  m_segmentThickness1,
                                  m_segmentThickness2,
                                  m_segmentMaterial1,
                                  m_segmentMaterial2,
                                  m_tintColor1,
                                  m_tintColor2);

                    float distanceToCover = (childNode.m_position - node.m_position).magnitude;

                    segmentAnimator.TranslatePointBTo(childNode.m_position, distanceToCover / segmentGrowSpeed);

                    node.m_active = false;
                }
                else
                {
                    segment.Build(GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                                  GeometryUtils.BuildVector3FromVector2(childNode.m_position, 0),
                                  m_segmentThickness1,
                                  m_segmentThickness2,
                                  m_segmentMaterial1,
                                  m_segmentMaterial2,
                                  m_tintColor1,
                                  m_tintColor2);
                }

                segment.m_parentTree = this;
                segment.m_endTreeNode = childNode;
            }
        }
    }
}