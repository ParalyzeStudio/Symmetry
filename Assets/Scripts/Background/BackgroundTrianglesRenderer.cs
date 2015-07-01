using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BackgroundTrianglesRenderer : MonoBehaviour
{
    public const int NUM_COLUMNS = 25;

    public List<BackgroundTriangleColumn> m_triangleColumns { get; set; }
    public int m_numTrianglesPerColumn { get; set; }
    public Material m_triangleMaterial;
    public Mesh m_mesh { get; set; }
    public bool m_meshBuilt { get; set; }
    public bool m_doubleSided { get; set; }
    public float m_triangleEdgeLength { get; set; }
    public float m_triangleHeight { get; set; }

    //min and max values of triangles centers y-coordinates of this mesh
    public float m_meshMinY { get; set; }
    public float m_meshMaxY { get; set; }

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

    //gradients
    public float m_verticalOffset { get; set; } //the offset generated when translating background across gradients
    public Gradient m_mainMenuGradient { get; set; }
    public List<Gradient> m_transitionGradients { get; set; }
    public Gradient m_chaptersGradient { get; set; }
    public float m_transitionGradientLength;

    ////gradients animation
    //private bool m_offsetting;
    //private float m_offsettingStartY;
    //private float m_offsettingEndY;
    //private float m_offsettingElapsedTime;
    //private float m_offsettingDelay;
    //private float m_offsettingExponentialFactor; //the factor A in exp(A * x) - 1, used to slow or speed up the way this exponential is raising
    //private float m_offsettingMaxSpeed; //Limit the exponential to a maximum value

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

        //init the position of the animator to 0
        BackgroundTriangleAnimator animator = this.GetComponent<BackgroundTriangleAnimator>();
        animator.SetPosition(Vector3.zero);

        m_renderingMainMenuTitle = false;
    }

    /**
     * Build triangles to fill in the whole screen
     * **/
    public void BuildMesh(bool bDoubleSided = false)
    {
        m_doubleSided = bDoubleSided;

        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Build the actual mesh
        m_mesh = new Mesh();
        m_mesh.name = "BackgroundTrianglesMesh";
        GetComponent<MeshFilter>().sharedMesh = m_mesh;

        //Fill data using triangles columns
        m_triangleHeight = screenSize.x / (float) NUM_COLUMNS;
        m_triangleEdgeLength = 2 / Mathf.Sqrt(3) * m_triangleHeight;

        float fNumberOfTrianglesInColumn = screenSize.y / m_triangleEdgeLength;
        m_numTrianglesPerColumn = Mathf.FloorToInt(fNumberOfTrianglesInColumn);
        if (MathUtils.HasFractionalPart(fNumberOfTrianglesInColumn)) //fractional part: round to the next integer
            m_numTrianglesPerColumn += 1;
        m_numTrianglesPerColumn++; //add another triangle
        m_numTrianglesPerColumn *= 2; //multiply by 2 because there are 2 series of opposite trying forming one column
        int numberOfTriangles = NUM_COLUMNS * m_numTrianglesPerColumn;

        //Set up correct sizes for mesh arrays
        int numVerticesPerTriangle = bDoubleSided ? 6 : 3;
        m_meshVertices = new Vector3[numVerticesPerTriangle * numberOfTriangles]; //6 vertices per triangle because 2 sides of different colors
        m_meshTriangles = new int[numVerticesPerTriangle * numberOfTriangles]; //double sided triangles
        m_meshColors = new Color[numVerticesPerTriangle * numberOfTriangles]; //double sided triangles
        m_meshVerticesDirty = true;
        m_meshTrianglesDirty = true;
        m_meshColorsDirty = true;

        for (int i = 0; i != NUM_COLUMNS; i++)
        {
            BackgroundTriangleColumn column = new BackgroundTriangleColumn(this, i);

            for (int j = 0; j != m_numTrianglesPerColumn; j++)
            {
                float trianglePositionY = screenSize.y - 0.5f * (j - 1) * m_triangleEdgeLength;
                trianglePositionY -= 0.5f * screenSize.y;

                float triangleAngle;
                float trianglePositionX;
                if (i % 2 == 0)
                {
                    triangleAngle = (j % 2 == 0) ? 0 : 180;
                    trianglePositionX = (j % 2 == 0) ? (1 / 3.0f + i) * m_triangleHeight : (2 / 3.0f + i) * m_triangleHeight;
                }
                else
                {
                    triangleAngle = (j % 2 == 0) ? 180 : 0;
                    trianglePositionX = (j % 2 == 0) ? (2 / 3.0f + i) * m_triangleHeight : (1 / 3.0f + i) * m_triangleHeight;
                }
                trianglePositionX -= 0.5f * screenSize.x;

                Color defaultTriangleColor = GetDefaultBackgroundColor();
                BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), m_triangleEdgeLength, triangleAngle, defaultTriangleColor, defaultTriangleColor);
                triangle.m_doubleSided = bDoubleSided;
                triangle.m_indexInColumn = j;
                triangle.m_parentColumn = column;
                column.Add(triangle);

                //Fill in the mesh
                int triangleGlobalIndex = i * m_numTrianglesPerColumn + j;
                int triangleFirstVertexIndex = numVerticesPerTriangle * triangleGlobalIndex;

                //vertices                
                m_meshVertices[triangleFirstVertexIndex] = triangle.m_points[0];
                m_meshVertices[triangleFirstVertexIndex + 1] = triangle.m_points[1];
                m_meshVertices[triangleFirstVertexIndex + 2] = triangle.m_points[2];
                if (bDoubleSided)
                {
                    m_meshVertices[triangleFirstVertexIndex + 3] = triangle.m_points[0];
                    m_meshVertices[triangleFirstVertexIndex + 4] = triangle.m_points[2];
                    m_meshVertices[triangleFirstVertexIndex + 5] = triangle.m_points[1];
                }

                //indices
                m_meshTriangles[triangleFirstVertexIndex] = triangleFirstVertexIndex;
                m_meshTriangles[triangleFirstVertexIndex + 1] = triangleFirstVertexIndex + 1;
                m_meshTriangles[triangleFirstVertexIndex + 2] = triangleFirstVertexIndex + 2;
                if (bDoubleSided)
                {
                    m_meshTriangles[triangleFirstVertexIndex + 3] = triangleFirstVertexIndex + 3;
                    m_meshTriangles[triangleFirstVertexIndex + 4] = triangleFirstVertexIndex + 4;
                    m_meshTriangles[triangleFirstVertexIndex + 5] = triangleFirstVertexIndex + 5;
                }

                //colors
                m_meshColors[triangleFirstVertexIndex] = defaultTriangleColor;
                m_meshColors[triangleFirstVertexIndex + 1] = defaultTriangleColor;
                m_meshColors[triangleFirstVertexIndex + 2] = defaultTriangleColor;
                if (bDoubleSided)
                {
                    m_meshColors[triangleFirstVertexIndex + 3] = defaultTriangleColor;
                    m_meshColors[triangleFirstVertexIndex + 4] = defaultTriangleColor;
                    m_meshColors[triangleFirstVertexIndex + 5] = defaultTriangleColor;
                }
            }

            m_triangleColumns.Add(column);
        }

        //Instantiate a new material
        Material clonedMaterial = (Material)Instantiate(m_triangleMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = clonedMaterial;

        m_meshBuilt = true;
    }

    /**
     * Generate the gradient for the main menu
     * **/
    public void GenerateMainMenuGradient()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        Color gradientStartColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(252, 183, 94, 255));//Same gradient for all colors
        Color gradientEndColor = new Color(0.5f, 1.0f, 0.8f, 1.0f);

        if (m_mainMenuGradient == null)
            m_mainMenuGradient = new Gradient();
        m_mainMenuGradient.CreateLinear(new Vector2(0, 0.5f * screenSize.y),
                                        new Vector2(0, -0.5f * screenSize.y),
                                        gradientStartColor,
                                        gradientEndColor);
    }

    /**
     * Generate the gradient that will join main menu gradient and chapter gradient
     * **/
    public void GenerateTransitionGradients()
    {
        m_transitionGradients = new List<Gradient>();
        m_transitionGradients.Capacity = NUM_COLUMNS;

        Vector2 screenSize = ScreenUtils.GetScreenSize();
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        Color gradientStartColor = m_mainMenuGradient.m_linearGradientEndColor;

        for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
        {
            float xPosition = iColumnIdx * m_triangleHeight + 0.5f * m_triangleHeight;
            xPosition -= 0.5f * screenSize.x;
            float yPosition = m_mainMenuGradient.m_linearGradientEndPoint.y - m_transitionGradientLength;
            Color gradientEndColor = m_chaptersGradient.GetColorAtPosition(new Vector2(xPosition, yPosition));

            Gradient gradient = new Gradient();
            gradient.CreateLinear(m_mainMenuGradient.m_linearGradientEndPoint,
                                  m_mainMenuGradient.m_linearGradientEndPoint - new Vector2(0, m_transitionGradientLength),
                                  gradientStartColor,
                                  gradientEndColor);
            m_transitionGradients.Add(gradient);
        }
    }

    /**
     * Generate the radial gradient for the chapters background
     * **/
    public void GenerateChapterGradient()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Chapter chapter = levelManager.GetChapterForNumber(1); //TODO get last reached chapter or last played chapter, not decided yet

        Color gradientInnerColor = chapter.GetThemeColors()[0];
        Color gradientOuterColor = chapter.GetThemeColors()[1];

        if (m_chaptersGradient == null)
            m_chaptersGradient = new Gradient();
        Vector2 chaptersGradientCenter = m_mainMenuGradient.m_linearGradientEndPoint - new Vector2(0, m_transitionGradientLength + 0.5f * screenSize.y);
        m_chaptersGradient.CreateRadial(chaptersGradientCenter,
                                        960,
                                        gradientInnerColor,
                                        gradientOuterColor);
    }

    public Color GetColorAtPosition(Vector2 position)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        if (position.y <= m_mainMenuGradient.m_linearGradientStartPoint.y && position.y >= m_mainMenuGradient.m_linearGradientEndPoint.y)
            return m_mainMenuGradient.GetColorAtPosition(position);
        else if (position.y < m_mainMenuGradient.m_linearGradientEndPoint.y && position.y > m_mainMenuGradient.m_linearGradientEndPoint.y - m_transitionGradientLength)
        {
            int iColumnIndex = Mathf.FloorToInt((position.x + 0.5f * screenSize.x) / m_triangleHeight);
            return m_transitionGradients[iColumnIndex].GetColorAtPosition(position);
        }
        else if (position.y <= m_mainMenuGradient.m_linearGradientStartPoint.y - m_transitionGradientLength)
            return m_chaptersGradient.GetColorAtPosition(position);

        return Color.black;
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
        m_transitionGradientLength = ScreenUtils.GetScreenSize().y;
        GenerateMainMenuGradient();
        GenerateChapterGradient();
        GenerateTransitionGradients();

        for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];

            float randomRelativeDelay = Random.value;
            
            column.ApplyGradient(m_mainMenuGradient, true, 0.02f, true, 0.75f, fDelay + randomRelativeDelay, 0.05f);
        }

        //Modify the colors of some triangles to make the title appear
        RenderMainMenuTitle(bAnimated, fDelay + 5.0f);
    }

    public void RenderForChapter(bool bAnimated, float fDelay = 0.0f)
    {
        //for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
        //{
        //    BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];

        //    //column.ApplyGradient(gradientStartColor, gradientEndColor, 0.05f, true, 1.0f, 0.0f, 0.0f);
        //    column.ApplyGradient(new Color(0.5f, 0.5f, 0.5f, 1.0f), new Color(0.5f, 0.5f, 0.5f, 1.0f), 0.05f, true, 1.0f, 0.0f, 0.0f);
        //}

        Gradient radialGradient = new Gradient();
        Color innerColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(146,21,51,255));
        Color outerColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
        radialGradient.CreateRadial(Vector2.zero, 960, innerColor, outerColor);
        ApplyGradient(radialGradient, true, 0.0f, false, 1.0f, 0.0f, 0.1f, false);
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

                float randomValue = Random.value;
                if (false/*randomValue <= 0.33f*/) //left side
                {
                    int randomRow = Mathf.FloorToInt(Random.value * (m_triangleColumns[0].Count / 2.0f - 2));
                    
                    lightStartPosition.x = -0.5f * screenSize.x;
                    lightStartPosition.y = (0.5f + randomRow) * m_triangleEdgeLength;
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
                        lightStartPosition.y += 0.5f * m_triangleEdgeLength;
                    lightStartPosition.x = randomColumn * Mathf.Sqrt(3) / 2 * m_triangleEdgeLength;                    
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
                    lightStartPosition.y = ((NUM_COLUMNS % 2 == 1) ? randomRow : 0.5f + randomRow) * m_triangleEdgeLength;
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
                light.m_segmentLength = m_triangleEdgeLength;
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
     * Offset vertically all columns 
     * **/
    public void Offset(float dy)
    {
        m_verticalOffset += dy;
        for (int i = 0; i != m_triangleColumns.Count; i++)
        {
            m_triangleColumns[i].Offset(dy);
        }
    }
     
    /**
     * Launch the offsetting animation to move from main menu to chapters or vice versa
     * **/
    public void SwitchBetweenMainMenuBackgroundAndChapterBackground(bool bFromMainMenu, float fDuration, float fDelay = 0.0f)
    {
        BackgroundTriangleAnimator animator = this.GetComponent<BackgroundTriangleAnimator>();

        float toOffset = bFromMainMenu ? 2560 : 0;
        animator.TranslateTo(new Vector3(0, toOffset, 0), fDuration, fDelay);       
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