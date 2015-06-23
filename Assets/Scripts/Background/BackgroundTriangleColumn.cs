using UnityEngine;
using System.Collections.Generic;
using System;

/**
 * Class to hold and animate background triangles in one single column
 * **/
public class BackgroundTriangleColumn : List<BackgroundTriangle>
{
    public const float COLOR_INVALIDATION_STEP = 0.1f;

    public BackgroundTrianglesRenderer m_parentRenderer { get; set; }
    public int m_index { get; set; } //the index of this column in the global mesh

    //A column is rendered with a gradient so define here start and end color
    public Color m_gradientStartColor { get; set; }
    public Color m_gradientEndColor { get; set; }

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

    /**
     * Apply gradient to this column
     * This will modify the color of every triangle faces (front or back) starting from top (gradient startColor) to bottom (gradient endColor)
     * Specify a local variance for each triangle to generate a color that is near the color expected from the gradient
     * **/
    public void ApplyGradient(Gradient gradient, 
                                    bool bFrontFaces, 
                                    float localTriangleVariance = 0.0f,
                                    bool bAnimated = false,
                                    float fTriangleAnimationDuration = 1.0f,
                                    float fAnimationDelay = 0.0f,
                                    float fTriangleAnimationInterval = 0.1f)
    {
        float colorStep = 1 / (float)(this.Count - 1);

        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = this[iTriangleIdx];

            Color triangleOriginalColor = gradient.GetColorAtPosition(triangle.GetCenter());
            if (bFrontFaces)
                triangle.m_originalFrontColor = triangleOriginalColor;
            else
                triangle.m_originalBackColor = triangleOriginalColor;

            if (bAnimated)
            {
                Color animationEndColor = ColorUtils.GetRandomNearColor(triangleOriginalColor, localTriangleVariance);
                triangle.StartColorAnimation(bFrontFaces, animationEndColor, fTriangleAnimationDuration, fAnimationDelay + iTriangleIdx * fTriangleAnimationInterval);
            }
            else
            {
                triangle.GenerateColorFromOriginalColor(bFrontFaces, localTriangleVariance);
                triangle.UpdateParentRendererMeshColorsArray(bFrontFaces, bFrontFaces);
            }
        }

        m_parentRenderer.m_meshColorsDirty = true;
    }

    /**
     * Modify the color of triangles that are under color animation
     * **/
    public void AnimateTriangles(float dt)
    {
        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            this[iTriangleIdx].AnimateColor(dt);
            this[iTriangleIdx].FlipAnimate(dt);
        }

        //m_parentRenderer.UpdateMeshColorsArrayForColumn(this);
    }

    //public void SwitchTrianglesVisibilityStatusBetweenIndices(bool bAnimated, int startIndex, int endIndex, float fDelay = 0.0f)
    //{
    //    //Turn on the animation pending value for every triangle in this block
    //    for (int i = startIndex; i != endIndex + 1; i++)
    //    {
    //        int relativeIndex = i - startIndex;

    //        this[i].ToggleVisibility(bAnimated, 2.5f, fDelay + relativeIndex * BackgroundTriangleColumn.COLOR_INVALIDATION_STEP);
    //    }
    //}

    //public void LaunchColorAnimationForTrianglesBetweenIndices(int startIndex, int endIndex, float fTriangleAnimationDuration, float fGlobalDelay, float fTriangleAnimationRelativeDelay)
    //{
    //    //Turn on the animation pending value for every triangle in this block
    //    for (int i = startIndex; i != endIndex + 1; i++)
    //    {
    //        BackgroundTriangle triangle = this[i];
    //        int relativeIndex = i - startIndex;

    //        triangle.StartColorAnimation(triangle.m_color, fTriangleAnimationDuration, fGlobalDelay + relativeIndex * fTriangleAnimationRelativeDelay);
    //    }
    //}
}

