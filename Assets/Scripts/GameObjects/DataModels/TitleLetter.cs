using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class TitleLetterVertex
{
    public Vector2 m_position;
    public int m_index;
    public List<int> m_neighbors; //list of neighbors (indices) of this node
    public List<int> m_linkedNeighbors; //list of neighbors (indices) which are linked (i.e a segment exists between them or is under construction) 
                                                     //to this vertex
    public TitleLetter m_parentLetter;

    public TitleLetterVertex()
    {
        m_neighbors = new List<int>();
        m_linkedNeighbors = new List<int>();
        m_index = 0;
    }

    public TitleLetterVertex(int index, float x, float y)
        : this()
    {
        m_position = new Vector2(x, y);
        m_index = index;
    }

    public TitleLetterVertex(int index, Vector2 position)
        : this(index, position.x, position.y)
    {

    }

    public void AddNeighbor(int iNodeIndex)
    {
        for (int iNeighborIdx = 0; iNeighborIdx != m_neighbors.Count; iNeighborIdx++)
        {
            if (iNodeIndex == m_neighbors[iNeighborIdx])
                return; //node is alreayd listed as neighbor
        }

        m_neighbors.Add(iNodeIndex);
    }

    public void LinkToNeighbor(int iNodeIndex)
    {
        if (IsLinkedToNeighbor(iNodeIndex)) //this vertex is already linked to that neighbor
            return;

        m_linkedNeighbors.Add(iNodeIndex); //set the neighbor vertex as linked to 'this'
        TitleLetterVertex neighborVertex = m_parentLetter.GetVertexForIndex(iNodeIndex);
        neighborVertex.m_linkedNeighbors.Add(this.m_index); //set 'this' as linked to the neighbor vertex
    }


    public bool IsLinkedToNeighbor(int iNeighborIndex)
    {
        for (int iNeighborIdx = 0; iNeighborIdx != m_linkedNeighbors.Count; iNeighborIdx++)
        {
            if (iNeighborIndex == m_linkedNeighbors[iNeighborIdx])
                return true;
        }
        return false;
    }

    public void SpreadToNeighbors()
    {
        TitleLetter parentLetter = m_parentLetter;
        float spreadingSpeed = 10.0f;
        for (int iNeighborIdx = 0; iNeighborIdx != m_neighbors.Count; iNeighborIdx++)
        {
            int neighborIndex = m_neighbors[iNeighborIdx];
            TitleLetterVertex neighborVertex = parentLetter.GetVertexForIndex(neighborIndex);
            if (!this.IsLinkedToNeighbor(neighborIndex))
            {
                this.LinkToNeighbor(neighborIndex);

                float distanceToNeighborVertex = (neighborVertex.m_position - this.m_position).magnitude;
                TitleLetterSegment letterSegment = parentLetter.BuildSegmentBetweenVertices(this, neighborVertex, this.m_position);

                SegmentAnimator segmentAnimator = letterSegment.GetComponent<SegmentAnimator>();
                float translationAnimationDuration = distanceToNeighborVertex / spreadingSpeed;
                segmentAnimator.TranslatePointBTo(neighborVertex.m_position, translationAnimationDuration, 0.0f);
            }
        }
    }
}

public class TitleLetter : MonoBehaviour
{
    public List<TitleLetterVertex> m_vertices;
    public char m_value;
    public float m_width;
    public List<int> m_spreadVertices; //the list of vertices where spreading starts when title is shown with animation

    public GameObject m_letterSegmentPfb;
    public Material m_letterSegmentMaterial;

    public void Init(char value, float width, Material material)
    {
        m_value = value;
        m_width = width;
        m_vertices = new List<TitleLetterVertex>();
        m_spreadVertices = new List<int>();
        m_letterSegmentMaterial = material;
    }

    /**
     * Get the vertex indexed at position index. index is in the range [1-n] where n is the number of vertices inside this letter
     * **/
    public TitleLetterVertex GetVertexForIndex(int index)
    {
        return this.m_vertices[index - 1];
    }
    
    /**
     * Build a segment between 2 title letter vertices
     * Let the liberty to set the position of pointB
     * **/
    public TitleLetterSegment BuildSegmentBetweenVertices(TitleLetterVertex vertexA, TitleLetterVertex vertexB, Vector2 pointB)
    {
        GameObject clonedSegment = Instantiate(m_letterSegmentPfb);
        clonedSegment.transform.parent = this.transform;
        TitleLetterSegment letterSegment = clonedSegment.GetComponent<TitleLetterSegment>();
        letterSegment.Build(vertexA, vertexB, vertexA.m_position, pointB, TitleBuilder.DEFAULT_LETTER_SEGMENT_THICKNESS, m_letterSegmentMaterial, Color.white);

        return letterSegment;
    }
}