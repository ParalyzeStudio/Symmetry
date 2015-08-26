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
            BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), triangleEdgeLength, triangleAngle, Color.black, triangleContourThickness);
            triangle.m_indexInColumn = j;
            triangle.m_parentColumn = this;
            this.Add(triangle);

            //Fill in the mesh
            int numVerticesPerTriangle = (triangleContourThickness > 0) ? 15: 3; //3 quads (4 vertices) for edges and inner triangle (3 vertices)
            int numIndicesPerTriangle = (triangleContourThickness > 0) ? 21 : 6; //3 quads (6 indices) for edges and inner triangle (3 indices)
            int triangleGlobalIndex = m_index * numTriangles + j;
            int verticesArrayIndex = numVerticesPerTriangle * triangleGlobalIndex;
            int trianglesArrayIndex = numIndicesPerTriangle * triangleGlobalIndex;

            AddTriangleToMesh(triangle, verticesArrayIndex, trianglesArrayIndex, Color.black);
            
            //Set relations between neighbor triangles
            //Start with edge3 (horizontal neigbor)
            SetHorizontalLeftNeighbor(triangle);

            //then with edge1 and edge2 (vertical neighbors inside one column)
            SetVerticalTopNeighbor(triangle);

            //for the last column specify a color for the edge3 of triangles flipped by 180 degrees (right border)
            if (m_index == BackgroundTrianglesRenderer.NUM_COLUMNS - 1)
            {
                if (triangle.m_angle == 180)
                    triangle.SetEdge3Color(triangle.m_color);
            }
        }
    }

    /**
     * Add a triangle to the parent mesh
     * **/
    private void AddTriangleToMesh(BackgroundTriangle triangle, int verticesArrayIndex, int trianglesArrayIndex, Color color)
    {
        //edge1
        m_parentRenderer.AddQuadToMesh(verticesArrayIndex, trianglesArrayIndex, triangle.m_points[2], triangle.m_points[0], triangle.m_points[3], triangle.m_points[5], new Color(0, 0, 0, 0));
        //edge2
        m_parentRenderer.AddQuadToMesh(verticesArrayIndex + 4, trianglesArrayIndex + 6, triangle.m_points[0], triangle.m_points[1], triangle.m_points[4], triangle.m_points[3], new Color(0, 0, 0, 0));
        //edge3
        m_parentRenderer.AddQuadToMesh(verticesArrayIndex + 8, trianglesArrayIndex + 12, triangle.m_points[1], triangle.m_points[2], triangle.m_points[5], triangle.m_points[4], new Color(0, 0, 0, 0));
        //inner triangle
        m_parentRenderer.AddTriangleToMesh(verticesArrayIndex + 12, trianglesArrayIndex + 18, triangle.m_points[3], triangle.m_points[4], triangle.m_points[5], color);
    }

    /**
     * Set the left neighbor of the triangle parameter
     * **/
    private void SetHorizontalLeftNeighbor(BackgroundTriangle triangle)
    {
        if (triangle.m_angle == 0)
        {
            if (m_index > 0)
            {
                BackgroundTriangle leftNeighborTriangle = m_parentRenderer.m_triangleColumns[m_index - 1][triangle.m_indexInColumn];
                triangle.SetEdge3Neighbor(leftNeighborTriangle); //left neighbor has the same relative index than the current triangle
                leftNeighborTriangle.SetEdge3Neighbor(triangle); //do the same operation in the opposite direction
            }
            else
                triangle.SetEdge3Neighbor(null);
        }        
    }

    /**
     * Set the top neighbor of the triangle parameter
     * **/
    private void SetVerticalTopNeighbor(BackgroundTriangle triangle)
    {
        if (triangle.m_indexInColumn > 0 && triangle.m_indexInColumn < m_parentRenderer.m_numTrianglesPerColumn)
        {
            BackgroundTriangle topNeighbor = this[triangle.m_indexInColumn - 1];

            if (triangle.m_angle == 0) //edge1 is top edge
            {
                triangle.SetEdge1Neighbor(topNeighbor);
                topNeighbor.SetEdge1Neighbor(triangle);
            }
            else //angle == 180, edge2 is top edge
            {
                triangle.SetEdge2Neighbor(topNeighbor);
                topNeighbor.SetEdge2Neighbor(triangle);
            }
        }
        else
        {
            if (triangle.m_angle == 0) //edge1 is top edge
                triangle.SetEdge1Neighbor(null);
            else
                triangle.SetEdge2Neighbor(null);
        }           
    }

    /**
     * Apply gradient to this column
     * This will modify the color of every triangle faces (front or back) starting from top (gradient startColor) to bottom (gradient endColor)
     * Specify a local variance for each triangle to generate a color that is near the color expected from the gradient
     * **/
    public void ApplyGradient(Gradient gradient,
                              float localTriangleVariance = 0.0f,
                              bool bAnimated = false,
                              BackgroundTrianglesRenderer.GradientAnimationPattern animationPattern = BackgroundTrianglesRenderer.GradientAnimationPattern.VERTICAL_STRIPES,
                              float fTriangleAnimationDuration = 1.0f,
                              float fAnimationDelay = 0.0f,
                              float fTriangleAnimationInterval = 0.0f,
                              bool bSetRandomRelativeDelayOnEachTriangle = false)
    {
        float halfScreenSquaredDiagonal = 0.5f * ScreenUtils.GetDiagonalSquareLength();
        float halfScreenDiagonal = Mathf.Sqrt(halfScreenSquaredDiagonal);
        float numberOfTrianglesInDiagonal = halfScreenDiagonal / m_parentRenderer.m_triangleHeight; //number of triangles that can fit inside the circle that covers the whole screen

        for (int iTriangleIdx = 0; iTriangleIdx != this.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = this[iTriangleIdx];

            Vector2 triangleGlobalPosition = triangle.GetCenter() - new Vector2(0, m_parentRenderer.m_verticalOffset);
            triangle.m_originalColor = gradient.GetColorAtPosition(triangleGlobalPosition);

            if (bAnimated)
            {
                Color animationEndColor = ColorUtils.GetRandomNearColor(triangle.m_originalColor, localTriangleVariance);

                float fTriangleDelay = 0;
                if (animationPattern == BackgroundTrianglesRenderer.GradientAnimationPattern.VERTICAL_STRIPES)
                    fTriangleDelay = fAnimationDelay + iTriangleIdx * fTriangleAnimationInterval;
                else if (animationPattern == BackgroundTrianglesRenderer.GradientAnimationPattern.EXPANDING_CIRCLE)
                {
                    fTriangleDelay = fAnimationDelay;
                    //square distance to screen center
                    float squareDistanceToScreenCenter = (triangle.GetCenter() + new Vector2(0, m_parentRenderer.m_verticalOffset)).sqrMagnitude;
                    fTriangleDelay += squareDistanceToScreenCenter / halfScreenSquaredDiagonal;
                    fTriangleDelay = Mathf.Round(fTriangleDelay * numberOfTrianglesInDiagonal);
                    fTriangleDelay *= fTriangleAnimationInterval;
                }
                
                triangle.StartColorAnimation(animationEndColor, fTriangleAnimationDuration, fTriangleDelay);
            }
            else
            {
                triangle.GenerateColorFromOriginalColor(localTriangleVariance);
            }

            SetVerticalTopNeighbor(triangle);
            SetHorizontalLeftNeighbor(triangle);
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
        if (dy == 0)
            return;

        Vector2 screenSize = ScreenUtils.GetScreenSize();
        float verticalOffset = m_parentRenderer.m_verticalOffset;

        float triangleEdgeLength = m_parentRenderer.m_triangleEdgeLength;
        float triangleHeight = m_parentRenderer.m_triangleHeight;

        int numVerticesPerTriangle = (m_parentRenderer.m_triangleContourThickness > 0) ? 15 : 3; //3 quads (4 vertices) for edges and inner triangle (3 vertices)
        int numIndicesPerTriangle = (m_parentRenderer.m_triangleContourThickness > 0) ? 21 : 6; //3 quads (6 indices) for edges and inner triangle (3 indices)

        //First offset all triangles
        for (int iTriangleIndex = 0; iTriangleIndex != this.Count; iTriangleIndex++) //from bottom to top
        {
            BackgroundTriangle triangle = this[iTriangleIndex];
            triangle.Offset(dy);
            int triangleGlobalIndex = m_index * m_parentRenderer.m_numTrianglesPerColumn + triangle.m_indexInColumn;
            AddTriangleToMesh(triangle, triangleGlobalIndex * numVerticesPerTriangle, triangleGlobalIndex * numIndicesPerTriangle, triangle.m_color);
        }

        //Then rearrange triangles by removing (if applicable) to make the column fit in the screen
        //Calculate the distance between the current last triangle and the bottom of the screen
        float distanceFromLastTriangleToScreenBottom = this[this.Count - 1].GetCenter().y + 0.5f * screenSize.y;
        float fNumTrianglesUntilScreenBottom = distanceFromLastTriangleToScreenBottom / triangleEdgeLength;
        int iNumTrianglesToAdd = Mathf.FloorToInt(fNumTrianglesUntilScreenBottom);
        if (MathUtils.HasFractionalPart(fNumTrianglesUntilScreenBottom))
            iNumTrianglesToAdd++;
        iNumTrianglesToAdd *= 2; //double the number of triangles to add because there are 2 triangles sets in one single column

        if (iNumTrianglesToAdd > this.Count)
            iNumTrianglesToAdd = this.Count;

        //remove the same number of triangles at the top of the column
        if (iNumTrianglesToAdd > 0)
        {
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
                int triangleGlobalIndex = m_index * m_parentRenderer.m_numTrianglesPerColumn + triangle.m_indexInColumn;

                //relocate this triangle inside the mesh
                AddTriangleToMesh(triangle, triangleGlobalIndex * numVerticesPerTriangle, triangleGlobalIndex * numIndicesPerTriangle, triangle.m_color);

                SetHorizontalLeftNeighbor(triangle);
                SetVerticalTopNeighbor(triangle);

                if (m_index == BackgroundTrianglesRenderer.NUM_COLUMNS - 1)
                {
                    if (triangle.m_angle == 180)
                        triangle.SetEdge3Color(triangle.m_color);
                }
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
                
                Color triangleColor = m_parentRenderer.m_gradient.GetColorAtPosition(trianglePosition - new Vector2(0, verticalOffset));
                BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY),
                                                                     triangleEdgeLength,
                                                                     triangleAngle,
                                                                     triangleColor,
                                                                     m_parentRenderer.m_triangleContourThickness);

                triangle.m_indexInColumn = this.Count;
                triangle.m_parentColumn = this;

                triangle.GenerateColorFromOriginalColor(0.02f);

                this.Add(triangle);
                int triangleGlobalIndex = m_index * m_parentRenderer.m_numTrianglesPerColumn + triangle.m_indexInColumn;

                AddTriangleToMesh(triangle, triangleGlobalIndex * numVerticesPerTriangle, triangleGlobalIndex * numIndicesPerTriangle, triangle.m_color);

                SetHorizontalLeftNeighbor(triangle);
                SetVerticalTopNeighbor(triangle);

                //for the last column specify a color for the edge3 of triangles flipped by 180 degrees (right border)
                if (m_index == BackgroundTrianglesRenderer.NUM_COLUMNS - 1)
                {
                    if (triangle.m_angle == 180)
                        triangle.SetEdge3Color(triangle.m_color);
                }
            }

            m_parentRenderer.m_meshVerticesDirty = true;
            m_parentRenderer.m_meshTrianglesDirty = true;
            m_parentRenderer.m_meshColorsDirty = true;
        }
        else
            m_parentRenderer.m_meshVerticesDirty = true;
    }
}