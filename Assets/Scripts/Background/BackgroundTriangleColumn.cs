using UnityEngine;
using System.Collections.Generic;

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
                                    float fTriangleAnimationInterval = 0.1f,
                                    bool bSetRelativeDelayOnEachTriangle = false)
    {
        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = this[iTriangleIdx];

            Vector2 triangleGlobalPosition = triangle.GetCenter() - new Vector2(0, m_parentRenderer.m_verticalOffset);
            Color triangleOriginalColor = gradient.GetColorAtPosition(triangleGlobalPosition);
            if (bFrontFaces)
                triangle.m_originalFrontColor = triangleOriginalColor;
            else
                triangle.m_originalBackColor = triangleOriginalColor;

            if (bAnimated)
            {
                Color animationEndColor = ColorUtils.GetRandomNearColor(triangleOriginalColor, localTriangleVariance);
                float fRelativeDelay = (bSetRelativeDelayOnEachTriangle) ? Random.value * 0.5f * fTriangleAnimationDuration : 0;
                float fTriangleDelay = fAnimationDelay + iTriangleIdx * fTriangleAnimationInterval + fRelativeDelay;
                triangle.StartColorAnimation(bFrontFaces, animationEndColor, fTriangleAnimationDuration, fTriangleDelay);
            }
            else
            {
                triangle.GenerateColorFromOriginalColor(bFrontFaces, localTriangleVariance);
                triangle.UpdateParentRendererMeshColorsArray();
            }
        }
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
    }

    /**
     * Offset the column triangles by dy units
     * **/
    public void Offset(float dy)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        float verticalOffset = m_parentRenderer.m_verticalOffset;

        float triangleEdgeLength = m_parentRenderer.m_triangleEdgeLength;
        float triangleHeight = m_parentRenderer.m_triangleHeight;

        //First offset all triangles
        for (int iTriangleIndex = 0; iTriangleIndex != this.Count; iTriangleIndex++) //from bottom to top
        {
            BackgroundTriangle triangle = this[iTriangleIndex];
            triangle.Offset(dy);
            triangle.UpdateParentRendererMeshVerticesArray();
        }

        //Then rearrange triangles by removing/adding to make the column fit in the screen
        if (dy > 0)
        {
            //Calculate the distance between the current last triangle and the bottom of the screen
            float distanceFromLastTriangleToScreenBottom = this[this.Count - 1].GetCenter().y + 0.5f * screenSize.y;
            float fNumTrianglesUntilScreenBottom = distanceFromLastTriangleToScreenBottom / triangleEdgeLength;
            int iNumTrianglesToAdd = Mathf.FloorToInt(fNumTrianglesUntilScreenBottom);
            if (MathUtils.HasFractionalPart(fNumTrianglesUntilScreenBottom))
                iNumTrianglesToAdd++;
            iNumTrianglesToAdd *= 2; //double the number of triangles to add because there are 2 triangles sets in one single column

            if (iNumTrianglesToAdd > this.Count)
                iNumTrianglesToAdd = this.Count;
            //Debug.Log("iNumTrianglesToAdd:" + iNumTrianglesToAdd);

            //remove the same number of triangles at the top of the column            
            int iNumTrianglesToRemove = iNumTrianglesToAdd;
            for (int iTriangleIndex = 0; iTriangleIndex != this.Count; iTriangleIndex++) //from bottom to top
            {
                BackgroundTriangle triangle = this[iTriangleIndex];

                if (iNumTrianglesToRemove > 0)
                {
                    this.Remove(triangle);
                    iNumTrianglesToRemove--;
                    iTriangleIndex--;
                }
            }

            //and shift indices for remaining triangles
            for (int iTriangleIndex = 0; iTriangleIndex != this.Count; iTriangleIndex++) //from bottom to top
            {
                BackgroundTriangle triangle = this[iTriangleIndex];
                triangle.m_indexInColumn -= iNumTrianglesToAdd;
                triangle.UpdateParentRendererMeshVerticesArray();
                triangle.UpdateParentRendererMeshColorsArray();
            }

            //Determine the orientation of the next triangle to add in this column
            bool triangleRightPointing;
            if (this.Count > 0)
                triangleRightPointing = (this[this.Count - 1].m_angle == 0);
            else
                triangleRightPointing = (m_index % 2 == 0);             

            //Finally build and add the triangles
            for (int i = 0; i != iNumTrianglesToAdd; i++)
            {
                triangleRightPointing = !triangleRightPointing;
                float triangleAngle = triangleRightPointing ? 0 : 180;
                float trianglePositionX = triangleRightPointing ? (1 / 3.0f + m_index) * triangleHeight : (2 / 3.0f + m_index) * triangleHeight;
                trianglePositionX -= 0.5f * screenSize.x;
                float trianglePositionY;
                if (this.Count > 0)
                    trianglePositionY = this[this.Count - 1].GetCenter().y - 0.5f * triangleEdgeLength;
                else
                {
                    trianglePositionY = screenSize.y + 0.5f * triangleEdgeLength;
                    trianglePositionY -= 0.5f * screenSize.y;
                }
                Vector2 trianglePosition = new Vector2(trianglePositionX, trianglePositionY);
                Color triangleColor = m_parentRenderer.GetColorAtPosition(trianglePosition - new Vector2(0, verticalOffset));
                BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), triangleEdgeLength, triangleAngle, triangleColor, triangleColor);
                triangle.GenerateColorFromOriginalColor(true, 0.02f);

                triangle.m_doubleSided = m_parentRenderer.m_doubleSided;
                triangle.m_indexInColumn = this.Count;
                triangle.m_parentColumn = this;
                this.Add(triangle);

                triangle.UpdateParentRendererMeshVerticesArray();
                triangle.UpdateParentRendererMeshColorsArray();
            }
        }    
    }
}

