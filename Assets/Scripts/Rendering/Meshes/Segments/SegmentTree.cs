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
    public List<SegmentTreeNode> m_nodes { get; set; }
    private GameObject m_parentHolder;
    private GameObject m_segmentObjectPfb;
    private float m_segmentThickness;
    private Material m_segmentMaterial;
    private Color m_tintColor;

 
    public void Awake()
    {
        m_nodes = new List<SegmentTreeNode>();
    }

    public void Init(GameObject parentHolder, GameObject segmentObjectPfb, float segmentThickness, Material segmentMaterial, Color tintColor)
    {
        m_parentHolder = parentHolder;
        m_segmentObjectPfb = segmentObjectPfb;
        m_segmentThickness = segmentThickness;
        m_segmentMaterial = segmentMaterial;
        m_tintColor = tintColor;
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

        for (int iChildIdx = 0; iChildIdx != node.m_children.Count; iChildIdx++)
        {
            SegmentTreeNode childNode = node.m_children[iChildIdx];

            GameObject segmentObject = (GameObject)Instantiate(m_segmentObjectPfb);
            segmentObject.transform.parent = m_parentHolder.transform;
            TexturedSegment segment = segmentObject.GetComponent<TexturedSegment>();

            if (bAnimated)
            {                
                segment.Build(GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                              GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                              m_segmentThickness,
                              m_segmentMaterial,
                              m_tintColor,
                              false,
                              TextureWrapMode.Clamp);

                float distanceToCover = (childNode.m_position - node.m_position).magnitude;
                TexturedSegmentAnimator segmentAnimator = segmentObject.GetComponent<TexturedSegmentAnimator>();
                segmentAnimator.TranslatePointBTo(childNode.m_position, distanceToCover / segmentGrowSpeed);

                node.m_active = false;
            }
            else
            {
                segment.Build(GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                              GeometryUtils.BuildVector3FromVector2(childNode.m_position, 0),
                              m_segmentThickness,
                              m_segmentMaterial,
                              m_tintColor,
                              false,
                              TextureWrapMode.Clamp);
            }

            segment.m_parentTree = this;
            segment.m_endTreeNode = childNode;
        }
    }
}