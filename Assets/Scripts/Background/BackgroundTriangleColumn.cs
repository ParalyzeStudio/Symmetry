using UnityEngine;
using System.Collections.Generic;

/**
 * A sequence of consecutive triangles inside a column
 * **/
public struct TrianglesBlock
{
    public int m_startIndex;
    public int m_endIndex;
}

/**
 * Class to hold and animate background triangles in one single column
 * **/
public class BackgroundTriangleColumn : List<BackgroundTriangle>
{
    public const float COLOR_INVALIDATION_STEP = 0.05f;

    public BackgroundTrianglesRenderer m_parentRenderer { get; set; }
    public int m_index { get; set; } //the index of this column in the global mesh

    //A column is rendered with a gradient so define here start and end color
    public Color m_gradientStartColor { get; set; }
    public Color m_gradientEndColor { get; set; }

    //List all the blocks of triangles that are currently animated
    List<TrianglesBlock> m_animatedTrianglesBlocks;

    //variables related to the update loop
    public float m_elapsedTime { get; set; }

    public BackgroundTriangleColumn(BackgroundTrianglesRenderer parentRenderer, int index)
        : base()
    {
        m_parentRenderer = parentRenderer;
        m_index = index;
        m_animatedTrianglesBlocks = new List<TrianglesBlock>();
    }

    public void AddTriangle(BackgroundTriangle triangle)
    {
        this.Add(triangle);
    }

    public void RemoveTriangle(BackgroundTriangle triangle)
    {
        this.Remove(triangle);
    }

    /**
     * Apply gradient to this column
     * This will modify the color of every triangle starting from top (gradient startColor) to bottom (gradient endColor)
     * **/
    public void ApplyGradient(Color gradientStartColor, Color gradientEndColor)
    {
        float colorStep = 1 / (float) (this.Count - 1);

        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = this[iTriangleIdx];
            triangle.m_originalColor = Color.Lerp(gradientStartColor, gradientEndColor, colorStep * iTriangleIdx);
            triangle.GenerateColorFromOriginalColor();
        }
    }

    //public void AddLeader(int stopIndex, Color color)
    //{
    //    BackgroundTriangle triangle = this[0];
    //    triangle.m_leader = true;
    //    triangle.m_leaderStopIndex = stopIndex;
    //    triangle.SetColor(color);
    //}

    /**
     * Shift triangles and make them follow their leader
     * **/
    //public void ShiftTriangles()
    //{
    //    List<BackgroundTriangle> shiftedTriangles = new List<BackgroundTriangle>();
    //    shiftedTriangles.Capacity = this.Count;
    //    for (int i = 0; i != this.Count; i++)
    //    {
    //        shiftedTriangles.Add(new BackgroundTriangle(this[i]));
    //    }

    //    for (int i = 0; i != shiftedTriangles.Count; i++)
    //    {
    //        if (this[i].GetLeader() == null) //don't shift triangles without leader
    //            break;

    //        if (i == 0)
    //        {
    //            shiftedTriangles[i].SetColor(Color.Lerp(this[0].m_originalColor, this[0].GetLeader().m_leaderTargetColor, 0.02f));
    //        }
    //        else
    //            shiftedTriangles[i].m_color = this[i - 1].m_color;
    //        if (shiftedTriangles[i].m_leader)
    //        {
    //            shiftedTriangles[i].m_leader = false;
    //            if (i < shiftedTriangles.Count - 1 && i < shiftedTriangles[i].m_leaderStopIndex)
    //            {
    //                shiftedTriangles[i + 1].m_leader = true;
    //                shiftedTriangles[i + 1].m_leaderStopIndex = shiftedTriangles[i].m_leaderStopIndex;
    //            }
    //        }
    //    }

    //    this.Clear();
    //    this.AddRange(shiftedTriangles);

    //    //update mesh colors array
    //    m_parentRenderer.UpdateMeshColorsArrayForColumn(this);
    //}

    /**
     * Animate the column if any triangle has to be shown or hidden
     * **/
    public void AnimateTriangles(float dt)
    {
        m_elapsedTime += dt;

        if (m_elapsedTime > BackgroundTriangleColumn.COLOR_INVALIDATION_STEP)
        {
            m_elapsedTime = 0;

            for (int iBlockIdx = 0; iBlockIdx != m_animatedTrianglesBlocks.Count; iBlockIdx++)
            {
                TrianglesBlock block = m_animatedTrianglesBlocks[iBlockIdx];

                //Find the first triangle that can be animated in this block
                for (int iTriangleIdx = block.m_startIndex; iTriangleIdx != block.m_endIndex + 1; iTriangleIdx++)
                {
                    BackgroundTriangle triangle = this[iTriangleIdx];
                    if (triangle.m_statusSwitchPending)
                    {
                        triangle.m_statusSwitchPending = false; //remove the animation on this triangle
                        triangle.StartColorAnimation(5.0f, 0.0f);

                        if (iTriangleIdx == block.m_endIndex) //finished animating this block, we can remove it
                        {
                            m_animatedTrianglesBlocks.Remove(block);
                            iBlockIdx--;
                        }

                        break; //break the animation process on this block
                    }
                }
            }
        }

        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            this[iTriangleIdx].AnimateColor(dt);
        }

        m_parentRenderer.UpdateMeshColorsArrayForColumn(this);
    }

    /**
     * Animate all triangles (show or hide depending on the current state of the triangle)
     * in this column whose indices are between startIndex and endIndex
     * **/
    public void AddAnimatedTrianglesBlock(int startIndex, int endIndex)
    {
        //Set up the block
        TrianglesBlock block = new TrianglesBlock();
        block.m_startIndex = startIndex;
        block.m_endIndex = endIndex;
        m_animatedTrianglesBlocks.Add(block);

        //Turn on the animation pending value for every triangle in this block
        for (int i = startIndex; i != endIndex + 1; i++)
        {
            this[i].m_statusSwitchPending = true;
        }
    }

    ///**
    // * Does this triangle column contains at least one leader
    // * **/
    //private bool ContainsAtLeastOneLeader()
    //{
    //    for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
    //    {
    //        if (this[iTriangleIdx].m_leader)
    //            return true;
    //    }

    //    return false;
    //}

    ///**
    // * For the moment shifting occurs only if at least one leader triangle exists
    // * **/
    //public bool IsShifting()
    //{
    //    return ContainsAtLeastOneLeader();
    //}
}

