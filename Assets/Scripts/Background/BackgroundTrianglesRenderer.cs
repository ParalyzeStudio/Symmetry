using UnityEngine;
using System.Collections.Generic;

public class BackgroundTrianglesRenderer : MonoBehaviour
{
    public const int NUM_COLUMNS = 26;

    public List<BackgroundTriangleColumn> m_triangleColumns { get; set; }
    public int m_numTrianglesPerColumn { get; set; }
    public Material m_triangleMaterial;
    public Mesh m_mesh { get; set; }
    public bool m_meshBuilt { get; set; }

    //mesh arrays
    public Vector3[] m_meshVertices { get; set; }
    public int[] m_meshTriangles { get; set; }
    public Color[] m_meshColors { get; set; }
    public bool m_meshVerticesDirty { get; set; }
    public bool m_meshTrianglesDirty { get; set; }
    public bool m_meshColorsDirty { get; set; }

    //Variables to handle the delayed rendering of main title
    private bool m_renderingMainMenuTitle;
    private float m_renderingMainMenuTitleElapsedTime;
    private float m_renderingMainMenuTitleDelay;

    //point lights
    public const int MAX_LIGHTS = 5;
    private GameObject[] m_pointLightsObjects;
    public GameObject m_pointLightPfb;

    public void Awake()
    {
        m_meshBuilt = false;
        m_triangleColumns = new List<BackgroundTriangleColumn>();
        m_pointLightsObjects = new GameObject[MAX_LIGHTS];
    }

    public void Init()
    {
        BuildMesh();

        m_renderingMainMenuTitle = false;
    }

    /**
     * Build triangles to fill in the whole screen
     * **/
    public void BuildMesh()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Build the actual mesh
        m_mesh = new Mesh();
        m_mesh.name = "BackgroundTrianglesMesh";
        GetComponent<MeshFilter>().sharedMesh = m_mesh;

        //Fill data using triangles columns
        float triangleHeight = screenSize.x / (float) NUM_COLUMNS;
        float triangleEdgeLength = 2 / Mathf.Sqrt(3) * triangleHeight;

        float fNumberOfTrianglesInColumn = (screenSize.y - 0.5f * triangleEdgeLength) / triangleEdgeLength;
        m_numTrianglesPerColumn = (int)Mathf.Floor(fNumberOfTrianglesInColumn);
        if (fNumberOfTrianglesInColumn != m_numTrianglesPerColumn) //no floating part
            m_numTrianglesPerColumn += 1; //just round to the next integer
        m_numTrianglesPerColumn += 1; //add the first (half) triangle
        m_numTrianglesPerColumn *= 2; //multiply by 2 because there are 2 series of opposite trying forming one column
        int numberOfTriangles = NUM_COLUMNS * m_numTrianglesPerColumn;

        //Set up correct sizes for mesh arrays
        m_meshVertices = new Vector3[6 * numberOfTriangles]; //6 vertices per triangle because 2 sides of different colors
        m_meshTriangles = new int[6 * numberOfTriangles]; //double sided triangles
        m_meshColors = new Color[6 * numberOfTriangles]; //double sided triangles
        m_meshVerticesDirty = true;
        m_meshTrianglesDirty = true;
        m_meshColorsDirty = true;

        for (int i = 0; i != NUM_COLUMNS; i++)
        {
            BackgroundTriangleColumn column = new BackgroundTriangleColumn(this, i);

            for (int j = 0; j != m_numTrianglesPerColumn; j++)
            {
                float trianglePositionY = screenSize.y - 0.5f * j * triangleEdgeLength;
                trianglePositionY -= 0.5f * screenSize.y;

                float triangleAngle;
                float trianglePositionX;
                if (i % 2 == 0)
                {
                    triangleAngle = (j % 2 == 0) ? 0 : 180;
                    trianglePositionX = (j % 2 == 0) ? (1 / 3.0f + i) * triangleHeight : (2 / 3.0f + i) * triangleHeight;
                }
                else
                {
                    triangleAngle = (j % 2 == 0) ? 180 : 0;
                    trianglePositionX = (j % 2 == 0) ? (2 / 3.0f + i) * triangleHeight : (1 / 3.0f + i) * triangleHeight;
                }
                trianglePositionX -= 0.5f * screenSize.x;

                Color defaultTriangleColor = GetDefaultBackgroundColor();
                BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), triangleEdgeLength, triangleAngle, defaultTriangleColor, defaultTriangleColor);
                triangle.m_indexInColumn = j;
                triangle.m_parentColumn = column;
                column.AddTriangle(triangle);

                //Fill in the mesh
                int triangleGlobalIndex = i * m_numTrianglesPerColumn + j;
                int triangleFirstVertexIndex = 6 * triangleGlobalIndex;

                //vertices                
                m_meshVertices[triangleFirstVertexIndex] = triangle.m_points[0];
                m_meshVertices[triangleFirstVertexIndex + 1] = triangle.m_points[1];
                m_meshVertices[triangleFirstVertexIndex + 2] = triangle.m_points[2];
                m_meshVertices[triangleFirstVertexIndex + 3] = triangle.m_points[0];
                m_meshVertices[triangleFirstVertexIndex + 4] = triangle.m_points[2];
                m_meshVertices[triangleFirstVertexIndex + 5] = triangle.m_points[1];

                //indices
                m_meshTriangles[triangleFirstVertexIndex] = triangleFirstVertexIndex;
                m_meshTriangles[triangleFirstVertexIndex + 1] = triangleFirstVertexIndex + 1;
                m_meshTriangles[triangleFirstVertexIndex + 2] = triangleFirstVertexIndex + 2;
                m_meshTriangles[triangleFirstVertexIndex + 3] = triangleFirstVertexIndex + 3;
                m_meshTriangles[triangleFirstVertexIndex + 4] = triangleFirstVertexIndex + 4;
                m_meshTriangles[triangleFirstVertexIndex + 5] = triangleFirstVertexIndex + 5;

                //colors
                m_meshColors[triangleFirstVertexIndex] = defaultTriangleColor;
                m_meshColors[triangleFirstVertexIndex + 1] = defaultTriangleColor;
                m_meshColors[triangleFirstVertexIndex + 2] = defaultTriangleColor;
                m_meshColors[triangleFirstVertexIndex + 3] = defaultTriangleColor;
                m_meshColors[triangleFirstVertexIndex + 4] = defaultTriangleColor;
                m_meshColors[triangleFirstVertexIndex + 5] = defaultTriangleColor;
            }

            m_triangleColumns.Add(column);
        }

        //Instantiate a new material
        Material clonedMaterial = (Material)Instantiate(m_triangleMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = clonedMaterial;

        m_meshBuilt = true;
    }

    /**
     * Returns the default background color
     * **/
    public Color GetDefaultBackgroundColor()
    {
        return Color.white; //just return the color we want as background color
    }

    ///**
    // * Updates the colors array for the specific column
    // * **/
    //public void UpdateMeshColorsArrayForColumn(BackgroundTriangleColumn column)
    //{
    //    for (int iTriangleIdx = 0; iTriangleIdx != column.Count; iTriangleIdx++)
    //    {
    //        BackgroundTriangle triangle = column[iTriangleIdx];

    //        int triangleGlobalIndex = column.m_index * column.Count + iTriangleIdx;

    //        //colors
    //        m_meshColors[3 * triangleGlobalIndex] = triangle.m_frontColor;
    //        m_meshColors[3 * triangleGlobalIndex + 1] = triangle.m_frontColor;
    //        m_meshColors[3 * triangleGlobalIndex + 2] = triangle.m_frontColor;
    //        m_meshColors[3 * triangleGlobalIndex + 3] = triangle.m_backColor;
    //        m_meshColors[3 * triangleGlobalIndex + 4] = triangle.m_backColor;
    //        m_meshColors[3 * triangleGlobalIndex + 5] = triangle.m_backColor;
    //    }

    //    m_meshColorsDirty = true;
    //}

    ///**
    //* Same thing that UpdateMeshColorsArrayForColumn() but for all columns instead
    //* **/
    //public void UpdateMeshColorsArrayForAllColumns()
    //{
    //    for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
    //    {
    //        UpdateMeshColorsArrayForColumn(m_triangleColumns[iColumnIdx]);
    //    }
    //}

    public void UpdateMeshData()
    {
        if (m_meshVerticesDirty)
        {
            m_mesh.vertices = m_meshVertices;
            m_meshVerticesDirty = false;
        }
        if (m_meshTrianglesDirty)
        {
            m_mesh.triangles = m_meshTriangles;
            m_meshTrianglesDirty = false;
        }
        if (m_meshColorsDirty)
        {
            m_mesh.colors = m_meshColors;
            m_meshColorsDirty = false;
        }
    }

    public void RenderForMainMenu(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        Color gradientStartColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(252, 183, 94, 255));//Same gradient for all colors
        Color gradientEndColor = Color.white;

        Gradient gradient = new Gradient();
        gradient.CreateLinear(new Vector2(0, 0.5f * screenSize.y),
                              new Vector2(0, -0.5f * screenSize.y),
                              gradientStartColor,
                              gradientEndColor);

        for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];

            float randomRelativeDelay = Random.value;
            
            column.ApplyGradient(gradient, true, 0.02f, true, 1.5f, fDelay + randomRelativeDelay, 0.1f);
        }

        //Modify the colors of some triangles to make the title appear
        RenderMainMenuTitle(bAnimated, fDelay + 5.0f);
    }

    public void RenderForChapter(Color gradientStartColor, Color gradientEndColor, bool bAnimated, float fDelay = 0.0f)
    {
        //for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
        //{
        //    BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];

        //    //column.ApplyGradient(gradientStartColor, gradientEndColor, 0.05f, true, 1.0f, 0.0f, 0.0f);
        //    column.ApplyGradient(new Color(0.5f, 0.5f, 0.5f, 1.0f), new Color(0.5f, 0.5f, 0.5f, 1.0f), 0.05f, true, 1.0f, 0.0f, 0.0f);
        //}

        Gradient radialGradient = new Gradient();
        Color startColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(146,21,51,255));
        Color endColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
        radialGradient.CreateRadial(Vector2.zero, 960, startColor, endColor);
        ApplyGradient(radialGradient, true, 0.02f, true, 1.0f, 0.0f, 0.1f, false);
        //FlipAllTriangles(0.5f, 0.0f);
    }

    /**
     * Launches the main menu title rendering processing that will draw it after a certain delay
     * **/
    public void RenderMainMenuTitle(bool bAnimated, float fDelay)
    {
        return;
        if (bAnimated)
        {
            m_renderingMainMenuTitle = true;
            m_renderingMainMenuTitleElapsedTime = 0;
            m_renderingMainMenuTitleDelay = fDelay;
        }
        else
            DrawMainMenuTitle(false);
    }

    /**
     * Modify the color of all triangles forming the title FLEEC
     * **/
    public void DrawMainMenuTitle(bool bAnimated)
    {
        //List all triangles inside the title
        List<BackgroundTriangle> titleTriangles = new List<BackgroundTriangle>();
        titleTriangles.Capacity = 52; //52 triangles in the title (9+8+10+10+15)

        //Add 9 triangles for letter F
        titleTriangles.Add(m_triangleColumns[4][7]);
        titleTriangles.Add(m_triangleColumns[4][8]);
        titleTriangles.Add(m_triangleColumns[4][9]);
        titleTriangles.Add(m_triangleColumns[4][10]);
        titleTriangles.Add(m_triangleColumns[4][11]);
        titleTriangles.Add(m_triangleColumns[4][12]);
        titleTriangles.Add(m_triangleColumns[5][6]);
        titleTriangles.Add(m_triangleColumns[5][7]);
        titleTriangles.Add(m_triangleColumns[5][9]);

        //Add 8 triangles for letter L
        titleTriangles.Add(m_triangleColumns[7][6]);
        titleTriangles.Add(m_triangleColumns[7][7]);
        titleTriangles.Add(m_triangleColumns[7][8]);
        titleTriangles.Add(m_triangleColumns[7][9]);
        titleTriangles.Add(m_triangleColumns[7][10]);
        titleTriangles.Add(m_triangleColumns[7][11]);
        titleTriangles.Add(m_triangleColumns[7][12]);
        titleTriangles.Add(m_triangleColumns[8][12]);

        //Add 10 triangles for first letter E
        titleTriangles.Add(m_triangleColumns[10][7]);
        titleTriangles.Add(m_triangleColumns[10][8]);
        titleTriangles.Add(m_triangleColumns[10][9]);
        titleTriangles.Add(m_triangleColumns[10][10]);
        titleTriangles.Add(m_triangleColumns[10][11]);
        titleTriangles.Add(m_triangleColumns[11][6]);
        titleTriangles.Add(m_triangleColumns[11][7]);
        titleTriangles.Add(m_triangleColumns[11][9]);
        titleTriangles.Add(m_triangleColumns[11][11]);
        titleTriangles.Add(m_triangleColumns[11][12]);

        //Add 10 triangles for second letter E
        titleTriangles.Add(m_triangleColumns[14][6]);
        titleTriangles.Add(m_triangleColumns[14][7]);
        titleTriangles.Add(m_triangleColumns[14][9]);
        titleTriangles.Add(m_triangleColumns[14][11]);
        titleTriangles.Add(m_triangleColumns[14][12]);
        titleTriangles.Add(m_triangleColumns[15][7]);
        titleTriangles.Add(m_triangleColumns[15][8]);
        titleTriangles.Add(m_triangleColumns[15][9]);
        titleTriangles.Add(m_triangleColumns[15][10]);
        titleTriangles.Add(m_triangleColumns[15][11]);

        //Add 15 triangles for second letter C
        titleTriangles.Add(m_triangleColumns[18][7]);
        titleTriangles.Add(m_triangleColumns[18][8]);
        titleTriangles.Add(m_triangleColumns[18][9]);
        titleTriangles.Add(m_triangleColumns[18][10]);
        titleTriangles.Add(m_triangleColumns[18][11]);
        titleTriangles.Add(m_triangleColumns[19][6]);
        titleTriangles.Add(m_triangleColumns[19][7]);
        titleTriangles.Add(m_triangleColumns[19][11]);
        titleTriangles.Add(m_triangleColumns[19][12]);
        titleTriangles.Add(m_triangleColumns[20][6]);
        titleTriangles.Add(m_triangleColumns[20][7]);
        titleTriangles.Add(m_triangleColumns[20][11]);
        titleTriangles.Add(m_triangleColumns[20][12]);
        titleTriangles.Add(m_triangleColumns[21][7]);
        titleTriangles.Add(m_triangleColumns[21][11]);

        //manually set destination color and launch the color animation for every triangle in the previously build list
        Color blendColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(255, 23, 12, 255));
        for (int iTriangleIdx = 0; iTriangleIdx != titleTriangles.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = titleTriangles[iTriangleIdx];
            Color destinationColor = Color.Lerp(triangle.m_frontColor, blendColor, 0.5f);

            //generate a random delay for each title triangle
            float randomDelay = Random.value;
            randomDelay *= 0.1f;

            //launch the color animation
            if (bAnimated)
                triangle.StartColorAnimation(true, destinationColor, 2.0f, randomDelay);
            else
                triangle.m_frontColor = destinationColor;

            triangle.UpdateParentRendererMeshColorsArray(true, false);
        }
    }

    public void ProcessPointLights()
    {
        if (!m_meshBuilt)
            return;

        //replace all dead lights
        for (int iPointLightIdx = 0; iPointLightIdx != m_pointLightsObjects.Length; iPointLightIdx++)
        {
            GameObject lightObject = m_pointLightsObjects[iPointLightIdx];
            
            bool bReplaceLight = false;
            BackgroundMovingLight light;
            if (lightObject == null)
            {
                GameObject newLightObject = (GameObject)Instantiate(m_pointLightPfb);
                newLightObject.transform.parent = this.gameObject.transform; //add the light object as the child of the background triangles renderer object
                light = newLightObject.GetComponent<BackgroundMovingLight>();
                light.InitQuadMesh();
                m_pointLightsObjects[iPointLightIdx] = newLightObject;
                bReplaceLight = true;
            }
            else
            {
                light = lightObject.GetComponent<BackgroundMovingLight>();
                if (light.m_evacuated)
                    bReplaceLight = true;
            }
            
            if (bReplaceLight) //this light is dead create a new one
            {
                Vector2 screenSize = ScreenUtils.GetScreenSize();

                //Find a random screen side and a location on that side where this light comes from
                Vector2 lightStartPosition = Vector2.zero;
                Vector2 lightPointDirection = Vector2.zero;
                float triangleEdgeLength = m_triangleColumns[0][0].m_edgeLength; //all triangles same edge length, take the first one

                float randomValue = Random.value;
                if (false/*randomValue <= 0.33f*/) //left side
                {
                    int randomRow = Mathf.FloorToInt(Random.value * (m_triangleColumns[0].Count / 2.0f - 2));
                    
                    lightStartPosition.x = -0.5f * screenSize.x;
                    lightStartPosition.y = (0.5f + randomRow) * triangleEdgeLength;
                    lightStartPosition.y = screenSize.y - lightStartPosition.y; //reverse the Y-coordinates
                    lightStartPosition.y -= 0.5f * screenSize.y; //offset it

                    float randomDirectionNumber = Mathf.FloorToInt(Random.value + 1);
                    if (randomDirectionNumber == 0) //PI / 6
                        lightPointDirection = new Vector2(Mathf.Sqrt(3) / 2, 0.5f);
                    else //1 ==> -PI / 6
                        lightPointDirection = new Vector2(Mathf.Sqrt(3) / 2, -0.5f);
                }
                else if (true/*randomValue > 0.33f && randomValue <= 0.66f*/) //top side
                {
                    int randomColumn = Mathf.FloorToInt(Random.value * (m_triangleColumns.Count) - 2) + 1;

                    lightStartPosition.y = 0.5f * screenSize.y;
                    if (randomColumn % 2 == 0) //even column: offset the start position by half a triangle edge length
                        lightStartPosition.y += 0.5f * triangleEdgeLength;
                    lightStartPosition.x = randomColumn * Mathf.Sqrt(3) / 2 * triangleEdgeLength;                    
                    lightStartPosition.x -= 0.5f * screenSize.x;

                    float randomDirectionNumber = Mathf.FloorToInt(Random.value + 2);
                    //if (randomDirectionNumber == 0) //-PI / 6
                    //    lightPointDirection = new Vector2(Mathf.Sqrt(3) / 2, -0.5f);
                    //else if (randomDirectionNumber == 1) //-5 * PI / 6
                    //    lightPointDirection = new Vector2(-Mathf.Sqrt(3) / 2, -0.5f);
                    //else //-PI / 2
                        lightPointDirection = new Vector2(0, -1.0f);
                }
                else if (false /*randomValue > 0.66f*/) //right side
                {
                    int randomRow;
                    if (NUM_COLUMNS % 2 == 1)
                        randomRow = Mathf.FloorToInt(Random.value * (m_triangleColumns[0].Count / 2.0f - 2));
                    else
                        randomRow = Mathf.FloorToInt(Random.value * (m_triangleColumns[0].Count / 2.0f - 2)) + 1;

                    lightStartPosition.x = 0.5f * screenSize.x;
                    lightStartPosition.y = ((NUM_COLUMNS % 2 == 1) ? randomRow : 0.5f + randomRow) * triangleEdgeLength;
                    lightStartPosition.y = screenSize.y - lightStartPosition.y; //reverse the Y-coordinates
                    lightStartPosition.y -= 0.5f * screenSize.y; //offset it

                    float randomDirectionNumber = Mathf.FloorToInt(Random.value + 1);
                    if (randomDirectionNumber == 0) //PI / 6
                        lightPointDirection = new Vector2(-Mathf.Sqrt(3) / 2, 0.5f);
                    else //1 ==> -PI / 6
                        lightPointDirection = new Vector2(-Mathf.Sqrt(3) / 2, -0.5f);
                }

                light.m_finished = false;
                light.m_evacuated = false;
                light.m_startPoint = lightStartPosition;
                light.m_currentPoint = lightStartPosition;
                light.m_pointSpeed = 100;
                light.m_pointDirection = lightPointDirection;
                light.m_segmentLength = triangleEdgeLength;
            }
        }
    }

    /**
     * Apply a gradient to all triangles at once
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
        //Call ApplyGradient() on each column
        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];
            column.ApplyGradient(gradient, bFrontFaces, localTriangleVariance, bAnimated, fTriangleAnimationDuration, fAnimationDelay, fTriangleAnimationInterval, bSetRelativeDelayOnEachTriangle);
        }
    }

    /**
     * Flip all triangles
     * **/
    public void FlipAllTriangles(float fFlipDuration, float fFlipGlobalDelay)
    {
        for (int i = 0; i != NUM_COLUMNS; i++)
        {
            for (int j = 0; j != m_numTrianglesPerColumn; j++)
            {
                float fRelativeDelay = Random.value * 1.0f;

                BackgroundTriangle triangle = m_triangleColumns[i][j];
                triangle.StartFlipAnimation(new Vector3(1, 0, 0), fFlipDuration, fFlipGlobalDelay + fRelativeDelay);
            }
        }
    }

    /**
     * Return the nearest triangle of any column to the screen center y-coordinate
     * Can filter some triangles based on their indices (even/odd or both)
     * Can also choose on which column the operation is performed
     * **/
    public BackgroundTriangle GetNearestTriangleToScreenYCenter(bool evenIndicesTriangles, bool oddIndicesTriangles, int columnIndex = 0)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //take the first column (or any other, it doesn't matter)
        float minDistance = float.MaxValue;
        BackgroundTriangle nearestTriangle = null;
        for (int i = 0; i != m_triangleColumns[columnIndex].Count; i++)
        {
            if (!evenIndicesTriangles && i % 2 == 0)
                continue;
            if (!oddIndicesTriangles && i % 2 == 1)
                continue;

            BackgroundTriangle triangle = m_triangleColumns[columnIndex][i];

            Vector2 triangleCenter = triangle.GetCenter();
            float distanceToCenter = Mathf.Abs(triangleCenter.y); //center is at y-coordinate 0
            if (distanceToCenter < minDistance)
            {
                nearestTriangle = triangle;
                minDistance = distanceToCenter;
            }
        }

        return nearestTriangle;
    }

    /**
     * Update/Render loop
     * **/
    public void Update()
    {
        float dt = Time.deltaTime;

        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];

            column.AnimateTriangles(dt);
        }        

        //render main menu title
        if (m_renderingMainMenuTitle)
        {
            m_renderingMainMenuTitleElapsedTime += dt;
            if (m_renderingMainMenuTitleElapsedTime >= m_renderingMainMenuTitleDelay)
            {
                m_renderingMainMenuTitle = false;
                DrawMainMenuTitle(true);
            }
        }

        //point lights
        //ProcessPointLights();

        UpdateMeshData(); //update mesh data every frame, 'dirty' variables take care of knowing if mesh has to be actually updated
    }
}