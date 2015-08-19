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

    public void Build()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        int numTriangles = m_parentRenderer.m_numTrianglesPerColumn;
        float triangleEdgeLength = m_parentRenderer.m_triangleEdgeLength;
        float triangleHeight = m_parentRenderer.m_triangleHeight;
        float triangleContourThickness = m_parentRenderer.m_triangleContourThickness;

        this.Capacity = numTriangles;
        float columnHeight = 0.5f * (numTriangles + 1) * triangleEdgeLength;
        float columnOffset = 0.5f * (columnHeight - screenSize.y);

        for (int j = 0; j != numTriangles; j++)
        {
            float trianglePositionY = screenSize.y - 0.5f * (j + 1) * triangleEdgeLength + columnOffset;
            trianglePositionY -= 0.5f * screenSize.y;

            float triangleAngle;
            float trianglePositionX;
            if (m_index % 2 == 0)
            {
                triangleAngle = (j % 2 == 0) ? 0 : 180;
                trianglePositionX = (j % 2 == 0) ? (1 / 3.0f + m_index) * triangleHeight : (2 / 3.0f + m_index) * triangleHeight;
            }
            else
            {
                triangleAngle = (j % 2 == 0) ? 180 : 0;
                trianglePositionX = (j % 2 == 0) ? (2 / 3.0f + m_index) * triangleHeight : (1 / 3.0f + m_index) * triangleHeight;
            }
            trianglePositionX -= 0.5f * screenSize.x;

            Vector2 trianglePosition = new Vector2(trianglePositionX, trianglePositionY);
            Color triangleColor = ColorUtils.GetRandomNearColor(m_parentRenderer.GetColorAtPosition(trianglePosition), 0.02f);
            BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), triangleEdgeLength, triangleAngle, triangleColor, triangleContourThickness);
            triangle.m_indexInColumn = j;
            triangle.m_parentColumn = this;
            this.Add(triangle);

            //Fill in the mesh
            int numVerticesPerTriangle = (triangleContourThickness > 0) ? 15: 3; //3 quads (4 vertices) for edges and inner triangle (3 vertices)
            int numIndicesPerTriangle = (triangleContourThickness > 0) ? 21 : 6; //3 quads (6 indices) for edges and inner triangle (3 indices)
            int triangleGlobalIndex = m_index * numTriangles + j;
            int verticesArrayIndex = numVerticesPerTriangle * triangleGlobalIndex;
            int trianglesArrayIndex = numIndicesPerTriangle * triangleGlobalIndex;
 
            //edge1
            m_parentRenderer.AddQuadToMesh(verticesArrayIndex, trianglesArrayIndex, triangle.m_points[2], triangle.m_points[0], triangle.m_points[3], triangle.m_points[5], new Color(0,0,0,0));
            //edge2
            m_parentRenderer.AddQuadToMesh(verticesArrayIndex + 4, trianglesArrayIndex + 6, triangle.m_points[0], triangle.m_points[1], triangle.m_points[4], triangle.m_points[3], new Color(0, 0, 0, 0));
            //edge3
            m_parentRenderer.AddQuadToMesh(verticesArrayIndex + 8, trianglesArrayIndex + 12, triangle.m_points[1], triangle.m_points[2], triangle.m_points[5], triangle.m_points[4], new Color(0, 0, 0, 0));
            //inner triangle
            m_parentRenderer.AddTriangleToMesh(verticesArrayIndex + 12, trianglesArrayIndex + 18, triangle.m_points[3], triangle.m_points[4], triangle.m_points[5], triangleColor);
            
            //Set relations between neighbor triangles
            //Start with edge3 (horizontal neigbors)
            if (m_index > 0)
            {
                if (triangle.m_angle == 0)
                {
                    BackgroundTriangle leftNeighborTriangle = m_parentRenderer.m_triangleColumns[m_index - 1][j];
                    triangle.SetEdge3Neighbor(leftNeighborTriangle); //left neighbor has the same relative index than the current triangle
                    leftNeighborTriangle.SetEdge3Neighbor(triangle); //do the same operation in the opposite direction
                }
            }

            //then with edge1 and edge2 (vertical neighbors inside one column)
            if (j > 0 && j < numTriangles)
            {
                BackgroundTriangle topNeighbor = this[j - 1];

                if (triangle.m_angle == 0) //edge1 is top edge, edge2 is bottom edge
                {
                    triangle.SetEdge1Neighbor(topNeighbor);
                    topNeighbor.SetEdge1Neighbor(triangle);
                }
                else //angle == 180, edge1 is bottom edges and edge2 is top edge
                {
                    triangle.SetEdge2Neighbor(topNeighbor);
                    topNeighbor.SetEdge2Neighbor(triangle);
                }
            }
        }
    }

    /**
     * Set relations between neighbor triangles
     * **/
    //public void SetNeighborTrianglesRelations()
    //{
    //    for (int i = 0; i != this.Count; i++)
    //    {
    //        BackgroundTriangle triangle = this[i];

    //        //Start with edge3 (horizontal neigbors)
    //        if (m_index > 0)
    //        {
    //            if (triangle.m_angle == 0)
    //            {
    //                BackgroundTriangle leftNeighborTriangle = m_parentRenderer.m_triangleColumns[m_index - 1][j];
    //                triangle.SetEdge3Neighbor(leftNeighborTriangle); //left neighbor has the same relative index than the current triangle
    //                leftNeighborTriangle.SetEdge3Neighbor(triangle); //do the same operation in the opposite direction
    //            }
    //        }

    //        //then with edge1 and edge2 (vertical neighbors inside one column)
    //        if (j > 0 && j < numTriangles - 1)
    //        {
    //            Debug.Log(j);
    //            BackgroundTriangle topNeighbor = this[j - 1];
    //            BackgroundTriangle bottomNeighbor = this[j + 1];

    //            if (triangle.m_angle == 0) //edge1 is top edge, edge2 is bottom edge
    //            {
    //                triangle.SetEdge1Neighbor(topNeighbor);
    //                triangle.SetEdge2Neighbor(bottomNeighbor);
    //            }
    //            else //angle == 180, edge1 is bottom edges and edge2 is top edge
    //            {
    //                triangle.SetEdge1Neighbor(bottomNeighbor);
    //                triangle.SetEdge2Neighbor(topNeighbor);
    //            }
    //        }
    //    }
    //}

    /**
     * Apply gradient to this column
     * This will modify the color of every triangle faces (front or back) starting from top (gradient startColor) to bottom (gradient endColor)
     * Specify a local variance for each triangle to generate a color that is near the color expected from the gradient
     * **/
    public void ApplyGradient(Gradient gradient,
                                    float localTriangleVariance = 0.0f,
                                    bool bAnimated = false,
                                    float fTriangleAnimationDuration = 1.0f,
                                    float fAnimationDelay = 0.0f,
                                    float fTriangleAnimationInterval = 0.0f,
                                    bool bSetRelativeDelayOnEachTriangle = false)
    {
        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = this[iTriangleIdx];

            Vector2 triangleGlobalPosition = triangle.GetCenter() - new Vector2(0, m_parentRenderer.m_verticalOffset);
            triangle.m_originalColor = gradient.GetColorAtPosition(triangleGlobalPosition);

            if (bAnimated)
            {
                Color animationEndColor = ColorUtils.GetRandomNearColor(triangle.m_originalColor, localTriangleVariance);
                float fRelativeDelay = (bSetRelativeDelayOnEachTriangle) ? Random.value * 0.5f * fTriangleAnimationDuration : 0;
                float fTriangleDelay = fAnimationDelay + iTriangleIdx * fTriangleAnimationInterval + fRelativeDelay;
                triangle.StartColorAnimation(animationEndColor, fTriangleAnimationDuration, fTriangleDelay);
            }
            else
            {
                triangle.GenerateColorFromOriginalColor(localTriangleVariance);
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
            //this[iTriangleIdx].FlipAnimate(dt);
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
                BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), triangleEdgeLength, triangleAngle, triangleColor);
                triangle.GenerateColorFromOriginalColor(0.02f);

                triangle.m_indexInColumn = this.Count;
                triangle.m_parentColumn = this;
                this.Add(triangle);

                triangle.UpdateParentRendererMeshVerticesArray();
                triangle.UpdateParentRendererMeshColorsArray();
            }
        }    
    }
}

