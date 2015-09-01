using UnityEngine;
using System.Collections.Generic;

public class SegmentTreeNode
{
    public Vector2 m_position { get; set; }
    public List<SegmentTreeNode> m_children { get; set; }
    public bool m_animationStartNode;

    public SegmentTreeNode(Vector2 position)
    {
        m_position = position;
        m_children = new List<SegmentTreeNode>();
        m_animationStartNode = false;
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
 
    public void Awake()
    {
        m_nodes = new List<SegmentTreeNode>();
    }

    public void BuildSegments(bool bAnimated, GameObject parentHolder, GameObject segmentObjectPfb, float segmentThickness, Material segmentMaterial, Color tintColor)
    {
        if (bAnimated)
        {
            float segmentGrowSpeed = 10.0f;

            //Find the animation start node (there is only one)
            for (int i = 0; i != m_nodes.Count; i++)
            {
                if (m_nodes[i].m_animationStartNode)
                {
                    GameObject segmentObject = (GameObject)Instantiate(segmentObjectPfb);
                    segmentObject.transform.parent = parentHolder.transform;

                    for (int iChildIdx = 0; iChildIdx != m_nodes[i].m_children.Count; iChildIdx++)
                    {
                        SegmentTreeNode childNode = m_nodes[i].m_children[iChildIdx];

                        TexturedSegment segment = segmentObject.GetComponent<TexturedSegment>();
                        segment.Build(GeometryUtils.BuildVector3FromVector2(m_nodes[i].m_position, 0),
                                      GeometryUtils.BuildVector3FromVector2(m_nodes[i].m_position, 0),
                                      segmentThickness,
                                      segmentMaterial,
                                      tintColor,
                                      false,
                                      TextureWrapMode.Clamp);

                        float distanceToCover = (childNode.m_position - m_nodes[i].m_position).magnitude;
                        TexturedSegmentAnimator segmentAnimator = segmentObject.GetComponent<TexturedSegmentAnimator>();
                        segmentAnimator.TranslatePointBTo(childNode.m_position, distanceToCover / segmentGrowSpeed);
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i != m_nodes.Count; i++)
            {
                SegmentTreeNode node = m_nodes[i];

                for (int j = 0; j != node.m_children.Count; j++)
                {
                    SegmentTreeNode child = node.m_children[j];

                    GameObject segmentObject = (GameObject)Instantiate(segmentObjectPfb);
                    segmentObject.transform.parent = parentHolder.transform;

                    TexturedSegment segment = segmentObject.GetComponent<TexturedSegment>();
                    segment.Build(GeometryUtils.BuildVector3FromVector2(node.m_position, 0),
                                  GeometryUtils.BuildVector3FromVector2(child.m_position, 0),
                                  segmentThickness,
                                  segmentMaterial,
                                  tintColor,
                                  false,
                                  TextureWrapMode.Clamp);
                }
            }
        }
    }
}