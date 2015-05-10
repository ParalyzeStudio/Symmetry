using UnityEngine;
using System.Collections.Generic;

public class TitleLetterVertex
{
    public Vector2 m_position { get; set; }
    public int m_index { get; set; }
    public List<int> m_neighbors { get; set; } //list of neighbors (indices) of this node
    public List<int> m_linkedNeighbors { get; set; } //list of neighbors (indices) which are linked (i.e a segment exists between them or is under construction) 
                                                     //to this vertex

    public TitleLetterVertex()
    {
        m_neighbors = new List<int>();
        m_linkedNeighbors = new List<int>();
        m_index = 0;
    }

    public TitleLetterVertex(int index, float x, float y) : this()
    {
        m_position = new Vector2(x, y);
        m_index = index;
    }

    public TitleLetterVertex(int index, Vector2 position)
        : this(index, position.x, position.y)
    {
        
    }

    public TitleLetterVertex(TitleLetterVertex other)
    {
        m_position = other.m_position;
        m_index = other.m_index;

        //deep copy neighbors
        m_neighbors = new List<int>();
        m_neighbors.Capacity = other.m_neighbors.Count;
        m_neighbors.AddRange(other.m_neighbors);

        //deep copy linked neighbors
        m_linkedNeighbors = new List<int>();
        m_linkedNeighbors.Capacity = other.m_linkedNeighbors.Count;
        m_linkedNeighbors.AddRange(other.m_linkedNeighbors);
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

    public bool IsLinkedToNeighbor(int iNeighborIndex)
    {
        for (int iNeighborIdx = 0; iNeighborIdx != m_linkedNeighbors.Count; iNeighborIdx++)
        {
            if (iNeighborIndex == m_linkedNeighbors[iNeighborIdx])
                return true;
        }
        return false;
    }
}

public class TitleLetter : List<TitleLetterVertex>
{
    public char m_value { get; set; }

    public TitleLetter()
    {

    }

    public TitleLetter(TitleLetter other) : this()
    {
        m_value = other.m_value;

        this.Capacity = other.Count;
        for (int i = 0; i != other.Count; i++)
        {
            this.Add(new TitleLetterVertex(other[i]));
        }
    }

    /**
     * Get the vertex indexed at position index. index is in the range [1-n] where n is the number of vertices inside this letter
     * **/
    public TitleLetterVertex GetVertexForIndex(int index)
    {
        return this[index - 1];
    }
}