using UnityEngine;
using System.Collections.Generic;

public class BackgroundTrianglesRenderer : MonoBehaviour
{
    public const float BACKGROUND_TRIANGLES_Z_VALUE = -5.0f;
    public const int NUM_COLUMNS = 26;

    public List<BackgroundTriangleColumn> m_triangleColumns { get; set; }
    public int m_numTrianglesPerColumn { get; set; }
    public Material m_triangleMaterial;
    public Mesh m_mesh { get; set; }
    public bool m_meshBuilt { get; set; }
    public float m_triangleEdgeLength { get; set; }
    public float m_triangleHeight { get; set; }
    public float m_triangleContourThickness { get; set; }
    private int m_numVerticesPerTriangle;
    private int m_numIndicesPerTriangle;

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
    //private bool m_renderingMainMenuTitle;
    //private float m_renderingMainMenuTitleElapsedTime;
    //private float m_renderingMainMenuTitleDelay;

    //gradients
    public float m_verticalOffset { get; set; } //the offset generated when translating background across gradients
    public Gradient m_gradient { get; set; } //the gradient currently used to render background triangles color
    //public Gradient m_mainMenuGradient { get; set; }
    //public List<Gradient> m_transitionGradients { get; set; }
    //public Gradient m_chaptersGradient { get; set; }
    //public float m_transitionGradientLength;

    //gradients animation
    //List available patterns to render an animated gradient
    public enum GradientAnimationPattern
    {
        NONE = 0,
        VERTICAL_STRIPES, //verticale stripes falling down from the top of the screen
        EXPANDING_CIRCLE //a circle centered in the middle of the screen expanding
    };
    
    
    //private bool m_offsetting;
    //private float m_offsettingStartY;
    //private float m_offsettingEndY;
    //private float m_offsettingElapsedTime;
    //private float m_offsettingDelay;
    //private float m_offsettingExponentialFactor; //the factor A in exp(A * x) - 1, used to slow or speed up the way this exponential is raising
    //private float m_offsettingMaxSpeed; //Limit the exponential to a maximum value

    //point lights
    //public const int MAX_LIGHTS = 5;
    //private GameObject[] m_pointLightsObjects;
    //public GameObject m_pointLightPfb;

    public void Awake()
    {
        m_meshBuilt = false;
        m_triangleColumns = new List<BackgroundTriangleColumn>();
        //m_pointLightsObjects = new GameObject[MAX_LIGHTS];
    }

    public void Init()
    {
        //m_transitionGradientLength = ScreenUtils.GetScreenSize().y;

        //GenerateMainMenuGradient();
        //GenerateChapterGradient();
        //GenerateTransitionGradients();

        BuildMesh(0.138f);

        //init the position of the animator to 0
        BackgroundTriangleAnimator animator = this.GetComponent<BackgroundTriangleAnimator>();
        animator.SetPosition(Vector3.zero);

        //m_renderingMainMenuTitle = false;
    }

    /**
     * Build triangles to fill in the whole screen
     * **/
    public void BuildMesh(float triangleContourPercentage)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Build the actual mesh
        m_mesh = new Mesh();
        m_mesh.name = "BackgroundTrianglesMesh";
        GetComponent<MeshFilter>().sharedMesh = m_mesh;

        //Fill data using triangles columns
        m_triangleHeight = screenSize.x / (float)NUM_COLUMNS;
        m_triangleEdgeLength = 2 / Mathf.Sqrt(3) * m_triangleHeight;

        float fNumberOfTrianglesInColumn = screenSize.y / m_triangleEdgeLength;
        m_numTrianglesPerColumn = Mathf.RoundToInt(fNumberOfTrianglesInColumn);
        m_numTrianglesPerColumn += 2; //add 2 other triangles
        m_numTrianglesPerColumn *= 2; //multiply by 2 because there are 2 series of opposite trying forming one column
        int numberOfTriangles = NUM_COLUMNS * m_numTrianglesPerColumn;

        //Calculate the thickness of the contour for every triangle
        m_triangleContourThickness = triangleContourPercentage * 1 / 3.0f * m_triangleHeight;

        //Set up correct sizes for mesh arrays
        m_numVerticesPerTriangle = (triangleContourPercentage > 0) ? 15: 3; //3 quads (4 vertices) for edges and inner triangle (3 vertices)
        m_numIndicesPerTriangle = (triangleContourPercentage > 0) ? 21 : 3; //3 quads (6 indices) for edges and inner triangle (3 indices)
        m_meshVertices = new Vector3[m_numVerticesPerTriangle * numberOfTriangles];
        m_meshTriangles = new int[m_numIndicesPerTriangle * numberOfTriangles];
        m_meshColors = new Color[m_numVerticesPerTriangle * numberOfTriangles];

        for (int i = 0; i != NUM_COLUMNS; i++)
        {
            BackgroundTriangleColumn column = new BackgroundTriangleColumn(this, i);
            column.Build();            

            m_triangleColumns.Add(column);
        }

        //Declare vertices as 'dirty' to make a first refresh of the mesh
        m_meshVerticesDirty = true;
        m_meshTrianglesDirty = true;
        m_meshColorsDirty = true;            

        //Instantiate a new material
        Material clonedMaterial = (Material)Instantiate(m_triangleMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = clonedMaterial;

        m_meshBuilt = true;
    }

    /**
     * Generate the gradient for the main menu
     * **/
    //public void GenerateMainMenuGradient()
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    Color gradientStartColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(18, 75, 89, 255));
    //    //Color gradientEndColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(98, 127, 134, 255));
    //    Color gradientEndColor = Color.black;

    //    if (m_mainMenuGradient == null)
    //        m_mainMenuGradient = new Gradient();
    //    m_mainMenuGradient.CreateLinear(new Vector2(0, 0.5f * screenSize.y),
    //                                    new Vector2(0, -0.5f * screenSize.y),
    //                                    gradientStartColor,
    //                                    gradientEndColor);
    //}

    /**
     * Generate the gradient that will join main menu gradient and chapter gradient
     * **/
    //public void GenerateTransitionGradients()
    //{
    //    m_transitionGradients = new List<Gradient>();
    //    m_transitionGradients.Capacity = NUM_COLUMNS;

    //    Vector2 screenSize = ScreenUtils.GetScreenSize();
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

    //    Color gradientStartColor = m_mainMenuGradient.m_linearGradientEndColor;

    //    for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
    //    {
    //        float xPosition = iColumnIdx * m_triangleHeight + 0.5f * m_triangleHeight;
    //        xPosition -= 0.5f * screenSize.x;
    //        float yPosition = m_mainMenuGradient.m_linearGradientEndPoint.y - m_transitionGradientLength;
    //        Color gradientEndColor = m_chaptersGradient.GetColorAtPosition(new Vector2(xPosition, yPosition));

    //        Gradient gradient = new Gradient();
    //        gradient.CreateLinear(m_mainMenuGradient.m_linearGradientEndPoint,
    //                              m_mainMenuGradient.m_linearGradientEndPoint - new Vector2(0, m_transitionGradientLength),
    //                              gradientStartColor,
    //                              gradientEndColor);
    //        m_transitionGradients.Add(gradient);
    //    }
    //}

    /**
     * Generate the radial gradient for the chapters background
     * **/
    //public void GenerateChapterGradient()
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

    //    Chapter chapter = levelManager.GetChapterForNumber(1); //TODO get last reached chapter or last played chapter, not decided yet

    //    Color gradientInnerColor = chapter.GetThemeColors()[0];
    //    Color gradientOuterColor = chapter.GetThemeColors()[1];

    //    if (m_chaptersGradient == null)
    //        m_chaptersGradient = new Gradient();
    //    Vector2 chaptersGradientCenter = m_mainMenuGradient.m_linearGradientEndPoint - new Vector2(0, - 0.5f * screenSize.y);
    //    m_chaptersGradient.CreateRadial(chaptersGradientCenter,
    //                                    960,
    //                                    gradientInnerColor,
    //                                    gradientOuterColor);
    //}

    /**
     * Returns the default background color
     * **/
    public Color GetDefaultBackgroundColor()
    {
        return Color.white; //just return the color we want as background color
    }

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

    //public void RenderForMainMenu(bool bAnimated, float fDelay = 0.0f)
    //{
    //    for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
    //    {
    //        BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];
    //        column.ApplyGradient(m_mainMenuGradient, 0.02f, bAnimated, 0.75f, fDelay);
    //    }

    //    m_meshColorsDirty = true;

    //    //Modify the colors of some triangles to make the title appear
    //    //RenderMainMenuTitle(bAnimated, fDelay + 5.0f);
    //}

    //public void RenderForChapter(bool bAnimated, float fDelay = 0.0f)
    //{
    //    //for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
    //    //{
    //    //    BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];

    //    //    //column.ApplyGradient(gradientStartColor, gradientEndColor, 0.05f, true, 1.0f, 0.0f, 0.0f);
    //    //    column.ApplyGradient(new Color(0.5f, 0.5f, 0.5f, 1.0f), new Color(0.5f, 0.5f, 0.5f, 1.0f), 0.05f, true, 1.0f, 0.0f, 0.0f);
    //    //}

    //    //Gradient radialGradient = new Gradient();
    //    //Color innerColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(146,21,51,255));
    //    //Color outerColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
    //    Gradient radialGradient = m_chaptersGradient;
    //    //radialGradient.CreateRadial(Vector2.zero, 960, innerColor, outerColor);
    //    ApplyGradient(radialGradient, 0.02f, true, GradientAnimationPattern.EXPANDING_CIRCLE, 0.4f, 0.0f, 0.05f, false);
    //    //FlipAllTriangles(0.5f, 0.0f);
    //}

    /**
     * Launches the main menu title rendering processing that will draw it after a certain delay
     * **/
    public void RenderMainMenuTitle()
    {
        
    }

    /**
     * Modify the color of all triangles forming the title FLEEC
     * **/
    public void DrawMainMenuTitle(bool bAnimated)
    {       

        //List all triangles inside the title
        List<BackgroundTriangle> titleTriangles = new List<BackgroundTriangle>();
        titleTriangles.Capacity = 52; //52 triangles in the title (9+8+10+10+15)

        int titleOffsetX = 1;
        int titleOffsetY = 3;

        //Add 9 triangles for letter F
        int iReferenceColumnIndex = 4 + titleOffsetX;
        int iReferenceTriangleIndex = 7 + titleOffsetY;
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 2]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 3]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 5]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex - 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 2]);

        //Add 8 triangles for letter L
        iReferenceColumnIndex = 7 + titleOffsetX;
        iReferenceTriangleIndex = 6 + titleOffsetY;
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 2]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 3]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 5]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 6]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 6]);

        //Add 10 triangles for first letter E
        iReferenceColumnIndex = 10 + titleOffsetX;
        iReferenceTriangleIndex = 7 + titleOffsetY;
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 2]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 3]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex - 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 2]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 5]);

        //Add 10 triangles for second letter E
        iReferenceColumnIndex = 14 + titleOffsetX;
        iReferenceTriangleIndex = 6 + titleOffsetY;
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 3]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 5]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 6]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 2]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 3]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 5]);

        //Add 15 triangles for second letter C
        iReferenceColumnIndex = 18 + titleOffsetX;
        iReferenceTriangleIndex = 7 + titleOffsetY;
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 2]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 3]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex - 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 1][iReferenceTriangleIndex + 5]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 2][iReferenceTriangleIndex - 1]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 2][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 2][iReferenceTriangleIndex + 4]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 2][iReferenceTriangleIndex + 5]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 3][iReferenceTriangleIndex]);
        titleTriangles.Add(m_triangleColumns[iReferenceColumnIndex + 3][iReferenceTriangleIndex + 4]);

        //manually set destination color and launch the color animation for every triangle in the previously build list
        Color blendColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(189, 215, 255, 255));
        for (int iTriangleIdx = 0; iTriangleIdx != titleTriangles.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = titleTriangles[iTriangleIdx];
            Color destinationColor = Color.Lerp(triangle.m_color, blendColor, 1.0f);

            //generate a random delay for each title triangle
            float randomDelay = Random.value;
            randomDelay *= 0.1f;

            //launch the color animation
            if (bAnimated)
                triangle.StartColorAnimation(destinationColor, 2.0f, randomDelay);
            else
                triangle.m_color = destinationColor;
        }
    }

    //public void ProcessPointLights()
    //{
    //    if (!m_meshBuilt)
    //        return;

    //    //replace all dead lights
    //    for (int iPointLightIdx = 0; iPointLightIdx != m_pointLightsObjects.Length; iPointLightIdx++)
    //    {
    //        GameObject lightObject = m_pointLightsObjects[iPointLightIdx];
            
    //        bool bReplaceLight = false;
    //        BackgroundMovingLight light;
    //        if (lightObject == null)
    //        {
    //            GameObject newLightObject = (GameObject)Instantiate(m_pointLightPfb);
    //            GameObjectAnimator lightAnimator = newLightObject.GetComponent<GameObjectAnimator>(); //add the light object as a child of the background triangles renderer object
    //            lightAnimator.SetParentTransform(this.transform);
    //            light = newLightObject.GetComponent<BackgroundMovingLight>();
    //            light.Init();
    //            m_pointLightsObjects[iPointLightIdx] = newLightObject;
    //            bReplaceLight = true;
    //        }
    //        else
    //        {
    //            light = lightObject.GetComponent<BackgroundMovingLight>();
    //            if (light.m_evacuated)
    //                bReplaceLight = true;
    //        }
            
    //        if (bReplaceLight) //this light is dead create a new one
    //        {
    //            Vector2 screenSize = ScreenUtils.GetScreenSize();

    //            //Find a random screen side and a location on that side where this light comes from
    //            Vector2 lightStartPosition = Vector2.zero;
    //            Vector2 lightPointDirection = Vector2.zero;

    //            float randomValue = Random.value;
    //            if (false/*randomValue <= 0.33f*/) //left side
    //            {
    //                int randomRow = Mathf.FloorToInt(Random.value * (m_triangleColumns[0].Count / 2.0f - 2));
                    
    //                lightStartPosition.x = -0.5f * screenSize.x;
    //                lightStartPosition.y = (0.5f + randomRow) * m_triangleEdgeLength;
    //                lightStartPosition.y = screenSize.y - lightStartPosition.y; //reverse the Y-coordinates
    //                lightStartPosition.y -= 0.5f * screenSize.y; //offset it

    //                float randomDirectionNumber = Mathf.FloorToInt(Random.value + 1);
    //                if (randomDirectionNumber == 0) //PI / 6
    //                    lightPointDirection = new Vector2(Mathf.Sqrt(3) / 2, 0.5f);
    //                else //1 ==> -PI / 6
    //                    lightPointDirection = new Vector2(Mathf.Sqrt(3) / 2, -0.5f);
    //            }
    //            else if (true/*randomValue > 0.33f && randomValue <= 0.66f*/) //top side
    //            {
    //                int randomColumn = Mathf.FloorToInt(Random.value * (m_triangleColumns.Count) - 2) + 1;

    //                lightStartPosition.y = 0.5f * screenSize.y;
    //                if (randomColumn % 2 == 0) //even column: offset the start position by half a triangle edge length
    //                    lightStartPosition.y += 0.5f * m_triangleEdgeLength;
    //                lightStartPosition.x = randomColumn * Mathf.Sqrt(3) / 2 * m_triangleEdgeLength;                    
    //                lightStartPosition.x -= 0.5f * screenSize.x;

    //                float randomDirectionNumber = Mathf.FloorToInt(Random.value + 2);
    //                //if (randomDirectionNumber == 0) //-PI / 6
    //                //    lightPointDirection = new Vector2(Mathf.Sqrt(3) / 2, -0.5f);
    //                //else if (randomDirectionNumber == 1) //-5 * PI / 6
    //                //    lightPointDirection = new Vector2(-Mathf.Sqrt(3) / 2, -0.5f);
    //                //else //-PI / 2
    //                    lightPointDirection = new Vector2(0, -1.0f);
    //            }
    //            else if (false /*randomValue > 0.66f*/) //right side
    //            {
    //                int randomRow;
    //                if (NUM_COLUMNS % 2 == 1)
    //                    randomRow = Mathf.FloorToInt(Random.value * (m_triangleColumns[0].Count / 2.0f - 2));
    //                else
    //                    randomRow = Mathf.FloorToInt(Random.value * (m_triangleColumns[0].Count / 2.0f - 2)) + 1;

    //                lightStartPosition.x = 0.5f * screenSize.x;
    //                lightStartPosition.y = ((NUM_COLUMNS % 2 == 1) ? randomRow : 0.5f + randomRow) * m_triangleEdgeLength;
    //                lightStartPosition.y = screenSize.y - lightStartPosition.y; //reverse the Y-coordinates
    //                lightStartPosition.y -= 0.5f * screenSize.y; //offset it

    //                float randomDirectionNumber = Mathf.FloorToInt(Random.value + 1);
    //                if (randomDirectionNumber == 0) //PI / 6
    //                    lightPointDirection = new Vector2(-Mathf.Sqrt(3) / 2, 0.5f);
    //                else //1 ==> -PI / 6
    //                    lightPointDirection = new Vector2(-Mathf.Sqrt(3) / 2, -0.5f);
    //            }

    //            light.m_finished = false;
    //            light.m_evacuated = false;
    //            light.m_startPoint = lightStartPosition;
    //            light.m_currentPoint = lightStartPosition;
    //            light.m_pointSpeed = 100;
    //            light.m_pointDirection = lightPointDirection;
    //            light.m_segmentLength = m_triangleEdgeLength;
    //        }
    //    }
    //}

    /**
     * Apply a gradient to all triangles at once
     * **/
    public void ApplyGradient(Gradient gradient,
                              float localTriangleVariance = 0.0f,
                              bool bAnimated = false,
                              GradientAnimationPattern animationPattern = GradientAnimationPattern.NONE,
                              float fTriangleAnimationDuration = 1.0f,
                              float fAnimationDelay = 0.0f,
                              float fTriangleAnimationInterval = 0.0f,
                              bool bSetRandomRelativeDelayOnEachTriangle = false)
    {
        m_gradient = gradient; //set this gradient as currently displayed gradient

        //Call ApplyGradient() on each column
        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];
            if (animationPattern == GradientAnimationPattern.VERTICAL_STRIPES)
            {
                float columnDelay = Random.value * 0.1f;
                fAnimationDelay += columnDelay;
            }
            column.ApplyGradient(gradient, localTriangleVariance, bAnimated, animationPattern, fTriangleAnimationDuration, fAnimationDelay, fTriangleAnimationInterval, bSetRandomRelativeDelayOnEachTriangle);
        }
    }

    /**
     * Flip all triangles
     * **/
    //public void FlipAllTriangles(float fFlipDuration, float fFlipGlobalDelay)
    //{
    //    for (int i = 0; i != NUM_COLUMNS; i++)
    //    {
    //        for (int j = 0; j != m_numTrianglesPerColumn; j++)
    //        {
    //            float fRelativeDelay = Random.value * 1.0f;

    //            BackgroundTriangle triangle = m_triangleColumns[i][j];
    //            triangle.StartFlipAnimation(new Vector3(1, 0, 0), fFlipDuration, fFlipGlobalDelay + fRelativeDelay);
    //        }
    //    }
    //}

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
     * Return the nearest triangle of any column to the screen center y-coordinate
     * By default yPosition is at the center of the screen (=0)
     * Can filter some triangles based on their angle (0 or 180 degrees or -1 for all triangles)
     * Can also choose on which column the operation is performed
     * **/
    public BackgroundTriangle GetNearestTriangleToScreenYPosition(float yPosition = 0, int columnIndex = 0, float bTriangleAngle = -1)
    {
        float minDistance = float.MaxValue;
        BackgroundTriangle nearestTriangle = null;
        for (int i = 0; i != m_triangleColumns[columnIndex].Count; i++)
        {
            BackgroundTriangle triangle = m_triangleColumns[columnIndex][i];

            if (bTriangleAngle > 0 && triangle.m_angle != bTriangleAngle)
                continue;

            Vector2 triangleCenter = triangle.GetCenter();
            float distanceToYPosition = Mathf.Abs(triangleCenter.y - yPosition); //center is at y-coordinate 0
            if (distanceToYPosition < minDistance)
            {
                nearestTriangle = triangle;
                minDistance = distanceToYPosition;
            }
        }

        return nearestTriangle;
    }

    /**
     * Return all triangles surrounding a mesh point (6 triangles forming an hexagon in case of equilateral triangles)
     * **/
    //public BackgroundTriangle[] GetTrianglesAroundMeshPoint(Vector2 point)
    //{
    //    int iNumSurroundingTriangles = 6; //hexagon
    //    BackgroundTriangle[] surroundingTriangles = new BackgroundTriangle[iNumSurroundingTriangles];

    //    int iSurroundingTriangleIdx = 0;
    //    for (int i = 0; i != m_triangleColumns.Count; i++)
    //    {
    //        for (int j = 0; j != m_triangleColumns[i].Count; j++)
    //        {
    //            BackgroundTriangle triangle = m_triangleColumns[i][j];
    //            if (triangle.HasVertex(point))
    //            {
    //                surroundingTriangles[iSurroundingTriangleIdx] = triangle;
    //                if (++iSurroundingTriangleIdx > iNumSurroundingTriangles - 1) //we found all triangles
    //                {
    //                    return surroundingTriangles;
    //                }
    //            }
    //        }
    //    }

    //    return surroundingTriangles;
    //}

    /**
     * Adds a quad to this mesh by populating the vertices array starting at 'verticesArrayIndex'
     * and the triangles array starting at 'trianglesArrayIndex'
     * **/
    public void AddQuadToMesh(int verticesArrayIndex, int trianglesArrayIndex, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color color)
    {
        m_meshVertices[verticesArrayIndex] = a;
        m_meshVertices[verticesArrayIndex + 1] = b;
        m_meshVertices[verticesArrayIndex + 2] = c;
        m_meshVertices[verticesArrayIndex + 3] = d;
        m_meshTriangles[trianglesArrayIndex] = verticesArrayIndex;
        m_meshTriangles[trianglesArrayIndex + 1] = verticesArrayIndex + 1;
        m_meshTriangles[trianglesArrayIndex + 2] = verticesArrayIndex + 2;
        m_meshTriangles[trianglesArrayIndex + 3] = verticesArrayIndex;
        m_meshTriangles[trianglesArrayIndex + 4] = verticesArrayIndex + 2;
        m_meshTriangles[trianglesArrayIndex + 5] = verticesArrayIndex + 3;
        m_meshColors[verticesArrayIndex] = color;
        m_meshColors[verticesArrayIndex + 1] = color;
        m_meshColors[verticesArrayIndex + 2] = color;
        m_meshColors[verticesArrayIndex + 3] = color;
    }

    /**
     * Adds a triangle to this mesh by populating the vertices array starting at 'verticesArrayIndex'
     * and the triangles array starting at 'trianglesArrayIndex'
     * **/
    public void AddTriangleToMesh(int verticesArrayIndex, int trianglesArrayIndex, Vector2 a, Vector2 b, Vector2 c, Color color)
    {
        m_meshVertices[verticesArrayIndex] = a;
        m_meshVertices[verticesArrayIndex + 1] = b;
        m_meshVertices[verticesArrayIndex + 2] = c;
        m_meshTriangles[trianglesArrayIndex] = verticesArrayIndex;
        m_meshTriangles[trianglesArrayIndex + 1] = verticesArrayIndex + 1;
        m_meshTriangles[trianglesArrayIndex + 2] = verticesArrayIndex + 2;
        m_meshColors[verticesArrayIndex] = color;
        m_meshColors[verticesArrayIndex + 1] = color;
        m_meshColors[verticesArrayIndex + 2] = color;
    }

    /**
     * Return the global index of the triangle in the mesh
     * For instance in a mesh with 20 triangles per column the 3rd triangle of the 2th column has the index 22
     * **/
    public int GetTriangleGlobalIndex(int columnIndex, int triangleRelativeIndex)
    {
        return columnIndex * m_numTrianglesPerColumn + triangleRelativeIndex;
    }

    /**
     * Return the index of the first vertex of the triangle determined by 'columnIndex' and 'triangleRelativeIndex' in the vertices array
     * **/
    public int GetTriangleFirstVerticesArrayIndex(int columnIndex, int triangleRelativeIndex)
    {
        return GetTriangleGlobalIndex(columnIndex, triangleRelativeIndex) * m_numVerticesPerTriangle;
    }

    /**
     * Return the index of the first index of the triangle determined by 'columnIndex' and 'triangleRelativeIndex' in the triangles array
     * **/
    public int GetTriangleFirstTrianglesArrayIndex(int columnIndex, int triangleRelativeIndex)
    {
        return GetTriangleGlobalIndex(columnIndex, triangleRelativeIndex) * m_numVerticesPerTriangle;
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
        //if (m_renderingMainMenuTitle)
        //{
        //    m_renderingMainMenuTitleElapsedTime += dt;
        //    if (m_renderingMainMenuTitleElapsedTime >= m_renderingMainMenuTitleDelay)
        //    {
        //        m_renderingMainMenuTitle = false;
        //        DrawMainMenuTitle(true);
        //    }
        //}

        //point lights
        //ProcessPointLights();

        UpdateMeshData(); //update mesh data every frame, 'dirty' variables take care of knowing if mesh has to be actually updated
    }
}