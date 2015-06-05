using UnityEngine;
using System.Collections.Generic;

public struct LeadTriangle
{
    public int m_startIndex;
    public int m_currentIndex;
    public int m_stopIndex;
}

/**
 * Class to hold and animate background triangles in one single column
 * **/
public class BackgroundTriangleColumn : List<BackgroundTriangle>
{
    public const float COLOR_INVALIDATION_STEP = 0.04f;

    public BackgroundTrianglesRenderer m_parentRenderer { get; set; }
    public int m_index { get; set; } //the index of this column in the global mesh

    //variables related to the update loop
    public float m_elapsedTime { get; set; }

    public BackgroundTriangleColumn(BackgroundTrianglesRenderer parentRenderer, int index)
        : base()
    {
        m_parentRenderer = parentRenderer;
        m_index = index;
    }

    public void AddTriangle(BackgroundTriangle triangle)
    {
        this.Add(triangle);
    }

    public void RemoveTriangle(BackgroundTriangle triangle)
    {
        this.Remove(triangle);
    }

    public void AddLeader(int index, Color color)
    {
        //TMP get rid of index and put 0 instead
        this[0].m_leader = true;
        this[0].m_color = color;
    }

    /**
     * Shift triangles and make them follow their leader
     * **/
    public void ShiftTriangles()
    {
        List<BackgroundTriangle> shiftedTriangles = new List<BackgroundTriangle>();
        shiftedTriangles.Capacity = this.Count;
        for (int i = 0; i != this.Count; i++)
        {
            shiftedTriangles.Add(new BackgroundTriangle(this[i]));
        }

        for (int i = 0; i != shiftedTriangles.Count; i++)
        {
            if (this[i].GetLeader() == null) //don't shift triangles without leader
                break;

            if (i == 0)
            {
                shiftedTriangles[i].m_color = Color.Lerp(this[0].m_color, this[0].GetLeader().m_leaderTargetColor, 0.02f);
            }
            else
                shiftedTriangles[i].m_color = this[i - 1].m_color;
            if (shiftedTriangles[i].m_leader)
            {
                shiftedTriangles[i].m_leader = false;
                if (i < shiftedTriangles.Count - 1)
                    shiftedTriangles[i + 1].m_leader = true;
            }
        }

        this.Clear();
        this.AddRange(shiftedTriangles);

        //update mesh colors array
        InvalidateMeshColors();
    }

    /**
     * Invalidate mesh colors array for all triangles contained in that column
     * **/
    private void InvalidateMeshColors()
    {
        m_parentRenderer.UpdateTrianglesColumn(this);
    }

    /**
     * Does this triangle column contains at least one leader
     * **/
    private bool ContainsAtLeastOneLeader()
    {
        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            if (this[iTriangleIdx].m_leader)
                return true;
        }

        return false;
    }

    /**
     * For the moment shifting occurs only if at least one leader triangle exists
     * **/
    public bool IsShifting()
    {
        return ContainsAtLeastOneLeader();
    }
}

