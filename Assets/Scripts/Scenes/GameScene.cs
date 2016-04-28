using UnityEngine;
using System.Collections.Generic;

public class GameScene : GUIScene
{
    public const float ACTION_BUTTONS_Z_VALUE = -23.0f;
    public const float INTERFACE_BUTTONS_Z_VALUE = -23.0f;
    public const float DEPLOY_AXIS_BUTTONS_Z_VALUE = -23.0f;
    public const float AXIS_CONSTRAINTS_ICONS_Z_VALUE = -23.0f;
    public const float COUNTER_Z_VALUE = -15.0f;
    public const float CHALKBOARD_Z_VALUE = -10.0f;
    public const float CHALKBOARD_TOP_SEPARATION_Z_VALUE = -23.0f;
    public const float HATCHINGS_Z_VALUE = -20.0f;
    public const float TILED_BACKGROUND_RELATIVE_Z_VALUE = 1.0f;
    public const float GRID_Z_VALUE = -22.0f;
    public const float OUTLINES_Z_VALUE = -23.0f;
    public const float SHAPES_Z_VALUE = -20.0f;
    public const float AXES_Z_VALUE = -23.0f;
    //public const float SYMMETRY_STACK_Z_VALUE = -20.0f;

    //dimensions of scenes partitions
    private const float LEFT_CONTENT_DEFAULT_WIDTH = 242.0f;
    private const float RIGHT_CONTENT_DEFAULT_WIDTH = 168.0f;
    private const float TOP_CONTENT_DEFAULT_HEIGHT = 180.0f;
    private const float BOTTOM_CONTENT_DEFAULT_HEIGHT = 80.0f;

    private float m_leftContentWidth;
    private float m_rightContentWidth;
    private float m_bottomContentHeight;
    private float m_topContentHeight;

    //above a certain distance, we have to draw hatchings between the grid and the side contents, else we just move the side contents borders
    private const float DRAW_HATCHINGS_THRESHOLD = 64.0f; 

    //shared prefabs
    public GameObject m_radialGradientPfb;
    public GameObject m_texQuadPfb;
    public GameObject m_colorQuadPfb;
    public GameObject m_blurrySegmentPfb;
    public GameObject m_textMeshPfb;
    public GameObject m_texturedMeshPfb;
    public Material m_hatchingsMaterial;
    public Material m_transpColorMaterial;

    public Grid m_grid { get; set; }
    public ShapeVoxelGrid m_voxelGrid { get; set; }
    public Counter m_counter { get; set; }
    public Outlines m_outlines { get; set; }
    public Shapes m_shapesHolder { get; set; }
    public Axes m_axesHolder { get; set; }

    //plain white material
    private Material m_plainWhiteMaterial;

    //root objects
    public GameObject m_shapesHolderPfb;
    public GameObject m_gridPfb;
    public GameObject m_outlinesPfb;
    public GameObject m_counterPfb;
    public GameObject m_axesPfb;

    //chalkboard
    private GameObject m_chalkboardObject;
    public Material m_chalkboardMaterial;

    //holders
    private GameObject m_interfaceButtonsHolder;
    private GameObject m_actionButtonsHolder;
    private GameObject m_axisConstraintsIconsHolder;

    //interface buttons
    public GameObject m_debugBlurrySegmentObjectPfb;
    public GameObject m_debugSimplifiedRoundedSegmentObjectPfb;
    public Material m_glowRectangleMaterial;
    public Material m_blurrySegmentMaterial;
    public Material m_sharpSegmentMaterial;

    //action buttons
    private ActionButton[] m_actionButtons;

    //deploy axis buttons
    public GameObject m_deployAxisButtonPfb;

    //symmetry point button
    public GameObject m_symmetryPointPfb;

    //grid contour
    public Material m_gridTopContourMaterial;

    //Constraints on axes
    public const string CONSTRAINT_SYMMETRY_AXIS_HORIZONTAL = "SYMMETRY_AXIS_HORIZONTAL";
    public const string CONSTRAINT_SYMMETRY_AXIS_VERTICAL = "SYMMETRY_AXIS_VERTICAL";
    public const string CONSTRAINT_SYMMETRY_AXES_STRAIGHT = "SYMMETRY_AXES_STRAIGHT";
    public const string CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_LEFT = "SYMMETRY_AXIS_DIAGONAL_LEFT";
    public const string CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_RIGHT = "SYMMETRY_AXIS_DIAGONAL_RIGHT";
    public const string CONSTRAINT_SYMMETRY_AXES_DIAGONALS = "SYMMETRY_AXES_DIAGONALS";

    public List<Vector2> m_constrainedDirections { get; set; }

    //stack for symmetries
    //public GameObject m_gameStackPfb;
    //public GameStack m_gameStack { get; set; }

    public Material m_horizontalAxisMaterial;
    public Material m_verticalAxisMaterial;
    public Material m_straightAxesMaterial;
    public Material m_leftDiagonalAxisMaterial;
    public Material m_rightDiagonalAxisMaterial;
    public Material m_diagonalAxesMaterial;

    private ClippingManager m_clippingManager;
    private ThreadedJobsManager m_threadedJobsManager;
    private QueuedThreadedJobsManager m_queuedThreadedJobsManager;

    public void Init()
    {
        m_gameStatus = GameStatus.WAITING_FOR_START;
    }

    public override void Show()
    {
        base.Show();

        m_plainWhiteMaterial = Instantiate(m_transpColorMaterial);

        //Gradient
        ApplyBackgroundGradient();

        //Show all elements
        //CallFuncHandler callFuncHandler = GetCallFuncHandler();
        //callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowElements), 0.5f);       
        ShowElements();

        ////-----TEST 5-----// 
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();

        //for (int i = 0; i != 1000; i++)
        //{
        //    GridTriangle triangle1 = new GridTriangle();
        //    triangle1.m_points[0] = new GridPoint(0, 3, true);
        //    triangle1.m_points[1] = new GridPoint(0, -3, true);
        //    triangle1.m_points[2] = new GridPoint(4, 0, true);
        //    GridTriangle triangle2 = new GridTriangle();
        //    triangle2.m_points[0] = new GridPoint(1, 1, true);
        //    triangle2.m_points[1] = new GridPoint(1, -1, true);
        //    triangle2.m_points[2] = new GridPoint(4, 0, true);

        //    triangle1.IntersectionWithTriangle(triangle2);
        //}
        //sw.Stop();
        //Debug.Log("elapsedTime1:" + sw.ElapsedMilliseconds + " ms");

        //sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //for (int i = 0; i != 1000; i++)
        //{
        //    Contour contour1 = new Contour(3);
        //    contour1.Add(new GridPoint(0, 3, true));
        //    contour1.Add(new GridPoint(0, -3, true));
        //    contour1.Add(new GridPoint(4, 0, true));
        //    Contour contour2 = new Contour(3);
        //    contour2.Add(new GridPoint(1, 1, true));
        //    contour2.Add(new GridPoint(1, -1, true));
        //    contour2.Add(new GridPoint(4, 0, true));
        //    Shape shape1 = new Shape(contour1);
        //    Shape shape2 = new Shape(contour2);
        //    GetClippingManager().ShapesOperation(shape1, shape2, ClipperLib.ClipType.ctIntersection, true);
        //}
        //sw.Stop();
        //Debug.Log("elapsedTime2:" + sw.ElapsedMilliseconds + " ms");
    }

    public GameObject m_debugSweepLinePfb;

    private void ApplyBackgroundGradient()
    {
        if (GetBackgroundRenderer().m_gradient == null)
        {
            Chapter displayedChapter = GetLevelManager().m_currentChapter;

            Gradient gradient = new Gradient();
            gradient.CreateRadial(Vector2.zero,
                                  960,
                                  displayedChapter.GetThemeColors()[0],
                                  displayedChapter.GetThemeColors()[1]);


            GetBackgroundRenderer().ApplyGradient(gradient,
                                                  0.02f,
                                                  true,
                                                  BackgroundTrianglesRenderer.GradientAnimationPattern.EXPANDING_CIRCLE,
                                                  0.5f,
                                                  0.0f,
                                                  0.0f,
                                                  false);
        }
    }

    public void ShowElements()
    {
        m_topContentHeight = TOP_CONTENT_DEFAULT_HEIGHT;

        //Show the chalkboard background
        ShowChalkboard();

        //build shapes holder and axes holder
        BuildAxesHolder();
        BuildShapesHolder();

        //Display the grid
        ShowGrid();

        //Display interface buttons (pause, retry and hints)
        ShowInterfaceButtons();

        //Display action counter
        ShowCounter();

        //Show outlines (if applicable)
        ShowOutlines();

        //Show axes the user can drag onto the grid
        ShowDraggableAxes();

        //Show available symmetry axes
        BuildConstrainedDirections();
        //ShowAxisConstraintsIcons();

        //Show action buttons
        //ShowActionButtons();

        //Show axis deployment buttons
        //ShowDeployAxisButtons();

        //Show point symmetry buttons
        //ShowSymmetryPointButtons();

        //Show starting shapes
        //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowInitialShapes), 1.0f);
        ShowInitialShapes();

        //Show stack
        //ShowSymmetryStack();

        //Draw hatchings to show empty areas like holes or borders
        //DrawHatchings();

        //add small debug button to skip the level
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        GameObject skipLevelButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_DEBUG_SKIP_LEVEL, new Vector2(128, 128));
        skipLevelButtonObject.name = "DebugSkipLevel";
        GameObjectAnimator skipLevelButtonAnimator = skipLevelButtonObject.GetComponent<GameObjectAnimator>();
        skipLevelButtonAnimator.SetParentTransform(this.transform);
        skipLevelButtonAnimator.SetPosition(new Vector3(0.5f * screenSize.x - 64.0f, -0.5f * screenSize.y + 64.0f, -10));

        m_gameStatus = GameStatus.RUNNING;

        //Set up the ClippingManager
        InitClippingManager();
    }

    /**
     * Show the main chalkboard background
     * **/
    private void ShowChalkboard()
    {
        m_chalkboardObject = (GameObject)Instantiate(m_texQuadPfb);
        m_chalkboardObject.name = "Chalkboard";

        UVQuad chalkboard = m_chalkboardObject.GetComponent<UVQuad>();
        chalkboard.Init(m_chalkboardMaterial);

        TexturedQuadAnimator chalkboardAnimator = chalkboard.GetComponent<TexturedQuadAnimator>();
        chalkboardAnimator.SetParentTransform(this.transform);

        Vector2 screenSize = ScreenUtils.GetScreenSize();
        Vector2 chalkboardTextureSize = new Vector2(1920, 1110);
        float chalkboardTextureRatio = chalkboardTextureSize.y / chalkboardTextureSize.x;
        float chalkboardScreenHeight = screenSize.y - m_topContentHeight;
        float chalkboardScreenWidth = screenSize.x;
        float chalkboardScreenRatio = chalkboardScreenHeight / chalkboardScreenWidth;

        float chalkboardWidth, chalkboardHeight;
        if (chalkboardScreenRatio >= chalkboardTextureRatio)
        {
            chalkboardHeight = chalkboardScreenHeight;
            chalkboardWidth = chalkboardHeight / chalkboardTextureRatio;
        }
        else
        {
            chalkboardWidth = chalkboardScreenWidth;
            chalkboardHeight = chalkboardWidth * chalkboardTextureRatio;
        }

        Vector3 chalkboardSize = new Vector3(chalkboardWidth, chalkboardHeight, 1);
        Vector3 chalkboardPosition = new Vector3(0, -0.5f * (screenSize.y - chalkboardHeight) - (chalkboardHeight - chalkboardScreenHeight), CHALKBOARD_Z_VALUE);

        chalkboardAnimator.SetScale(chalkboardSize);
        chalkboardAnimator.SetPosition(chalkboardPosition);

        //Top separation
        GameObject topContourObject = Instantiate(m_texQuadPfb);
        topContourObject.name = "ChalkboardTopSeparation";
        UVQuad topContour = topContourObject.GetComponent<UVQuad>();
        topContour.Init(Instantiate(m_gridTopContourMaterial));
        topContour.SetTintColor(ColorUtils.GetRGBAColorFromTSB(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], 1));
        TexturedQuadAnimator topContourAnimator = topContourObject.GetComponent<TexturedQuadAnimator>();
        topContourAnimator.SetParentTransform(this.transform);
        topContourAnimator.SetScale(new Vector3(screenSize.x, 64, 1));
        topContourAnimator.SetPosition(new Vector3(0, chalkboardPosition.y + 0.5f * chalkboardHeight, CHALKBOARD_TOP_SEPARATION_Z_VALUE));
        topContourAnimator.SetColorChannels(new Vector3(100, 0.75f, 1.1f), ValueAnimator.ColorMode.TSB);
    }

    /**
     * We build the grid of anchors that is displayed on the screen and that will help the player positionning shapes and axis...
     * Two anchors are separated from each other of a distance of m_gridSpacing that can be set in the editor
     * **/
    private void ShowGrid()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
               
        GameObject gridObject = (GameObject)Instantiate(m_gridPfb);
        gridObject.name = "Grid";

        GameObjectAnimator gridAnimator = gridObject.GetComponent<GameObjectAnimator>();
        gridAnimator.SetParentTransform(this.transform);        

        //max size of the grid (not the actual grid size)
        Vector2 maxGridSize = new Vector2(screenSize.x - LEFT_CONTENT_DEFAULT_WIDTH - RIGHT_CONTENT_DEFAULT_WIDTH, screenSize.y - m_topContentHeight - BOTTOM_CONTENT_DEFAULT_HEIGHT);     

        //Visible grid
        m_grid = gridObject.GetComponent<Grid>();
        m_grid.Build(maxGridSize, m_plainWhiteMaterial);

        //set final dimension of side and bottom contents and correct position for grid
        float hatchingsSize = 0.5f * (maxGridSize.x - m_grid.m_gridSize.x);
        float gridPositionX, gridPositionY;
        if (hatchingsSize >= DRAW_HATCHINGS_THRESHOLD)
        {
            m_leftContentWidth = LEFT_CONTENT_DEFAULT_WIDTH;
            m_rightContentWidth = RIGHT_CONTENT_DEFAULT_WIDTH;
            gridPositionX = 0.5f * (screenSize.x - maxGridSize.x) - m_rightContentWidth;
        }
        else
        {
            m_leftContentWidth = LEFT_CONTENT_DEFAULT_WIDTH + hatchingsSize;
            m_rightContentWidth = RIGHT_CONTENT_DEFAULT_WIDTH + hatchingsSize;
            gridPositionX = 0.5f * (screenSize.x - m_grid.m_gridSize.x) - m_rightContentWidth;
        }

        hatchingsSize = 0.5f * (maxGridSize.y - m_grid.m_gridSize.y);
        if (hatchingsSize >= DRAW_HATCHINGS_THRESHOLD)
        {
            m_bottomContentHeight = BOTTOM_CONTENT_DEFAULT_HEIGHT;
            gridPositionY = -0.5f * (screenSize.y - maxGridSize.y) + m_bottomContentHeight;
        }
        else
        {
            m_bottomContentHeight = BOTTOM_CONTENT_DEFAULT_HEIGHT + 2 * hatchingsSize;
            gridPositionY = -0.5f * (screenSize.y - m_grid.m_gridSize.y) + m_bottomContentHeight;
        }
        
        gridAnimator.SetPosition(new Vector3(gridPositionX, gridPositionY, GRID_Z_VALUE));

        //Voxel grid
        m_voxelGrid = gridObject.GetComponent<ShapeVoxelGrid>();
        m_voxelGrid.Init(3);

        //Tiled background
        Contour backgroundShapeContour = new Contour(4);
        backgroundShapeContour.Add(new GridPoint(1, 1, true));
        backgroundShapeContour.Add(new GridPoint(m_grid.m_numColumns, 1, true));
        backgroundShapeContour.Add(new GridPoint(m_grid.m_numColumns, m_grid.m_numLines, true));
        backgroundShapeContour.Add(new GridPoint(1, m_grid.m_numLines, true));

        Shape backgroundShape = new Shape(backgroundShapeContour);
        backgroundShape.m_state = Shape.ShapeState.TILED_BACKGROUND; //no special state for the background
        backgroundShape.m_color = Color.white;
        backgroundShape.Triangulate();

        GameObject backgroundShapeObject = m_shapesHolder.CreateShapeObjectFromData(backgroundShape, false);
        backgroundShapeObject.name = "TiledBackground";
        GameObjectAnimator backgroundShapeAnimator = backgroundShapeObject.GetComponent<GameObjectAnimator>();
        backgroundShapeAnimator.SetPosition(new Vector3(0, 0, TILED_BACKGROUND_RELATIVE_Z_VALUE));
        backgroundShapeAnimator.SetColor(Color.white);

        //Borders
        //float contourThickness = 3.0f;
        
        //if (0.5f * (m_grid.m_maxGridSize.y - m_grid.m_gridSize.y) >= DRAW_HATCHINGS_THRESHOLD)
        //{
        //    GameObject topContourObject = Instantiate(m_colorQuadPfb);
        //    topContourObject.name = "TopGridContour";
        //    ColorQuad topContour = topContourObject.GetComponent<ColorQuad>();
        //    topContour.Init(m_plainWhiteMaterial);
        //    ColorQuadAnimator topContourAnimator = topContourObject.GetComponent<ColorQuadAnimator>();
        //    topContourAnimator.SetParentTransform(gridObject.transform);
        //    topContourAnimator.SetScale(new Vector3(m_grid.m_gridSize.x, contourThickness, 1));
        //    topContourAnimator.SetPosition(new Vector3(0, 0.5f * m_grid.m_gridSize.y, 0));
        //    topContourAnimator.SetColor(Color.white);

        //    GameObject bottomContourObject = Instantiate(m_colorQuadPfb);
        //    bottomContourObject.name = "BottomGridContour";
        //    ColorQuad bottomContour = bottomContourObject.GetComponent<ColorQuad>();
        //    bottomContour.Init(m_plainWhiteMaterial);
        //    ColorQuadAnimator bottomContourAnimator = bottomContourObject.GetComponent<ColorQuadAnimator>();
        //    bottomContourAnimator.SetParentTransform(gridObject.transform);
        //    bottomContourAnimator.SetScale(new Vector3(m_grid.m_maxGridSize.x, contourThickness, 1));
        //    bottomContourAnimator.SetPosition(new Vector3(0, -0.5f * m_grid.m_gridSize.y, 0));
        //    bottomContourAnimator.SetColor(Color.white);
        //}

        //if (0.5f * (m_grid.m_maxGridSize.x - m_grid.m_gridSize.x) >= DRAW_HATCHINGS_THRESHOLD)
        //{
        //    GameObject leftContourObject = Instantiate(m_colorQuadPfb);
        //    leftContourObject.name = "LeftGridContour";
        //    ColorQuad leftContour = leftContourObject.GetComponent<ColorQuad>();
        //    leftContour.Init(m_plainWhiteMaterial);
        //    ColorQuadAnimator leftContourAnimator = leftContourObject.GetComponent<ColorQuadAnimator>();
        //    leftContourAnimator.SetParentTransform(gridObject.transform);
        //    leftContourAnimator.SetScale(new Vector3(contourThickness, m_grid.m_gridSize.y, 1));
        //    leftContourAnimator.SetPosition(new Vector3(-0.5f * m_grid.m_gridSize.x, 0, 0));
        //    leftContourAnimator.SetColor(Color.white);

        //    GameObject rightContourObject = Instantiate(m_colorQuadPfb);
        //    rightContourObject.name = "RightGridContour";
        //    ColorQuad rightContour = rightContourObject.GetComponent<ColorQuad>();
        //    rightContour.Init(m_plainWhiteMaterial);
        //    ColorQuadAnimator rightContourAnimator = rightContourObject.GetComponent<ColorQuadAnimator>();
        //    rightContourAnimator.SetParentTransform(gridObject.transform);
        //    rightContourAnimator.SetScale(new Vector3(contourThickness, m_grid.m_gridSize.y, 1));
        //    rightContourAnimator.SetPosition(new Vector3(0.5f * m_grid.m_gridSize.x, 0, 0));
        //    rightContourAnimator.SetColor(Color.white);
        //}       
    }

    /**
     * Pause, retry and hints buttons
     * **/
    private void ShowInterfaceButtons()
    {
        float triangleHeight = GetBackgroundRenderer().m_triangleHeight;
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");

        GameObjectAnimator interfaceButtonsHolderAnimator = m_interfaceButtonsHolder.AddComponent<GameObjectAnimator>();
        interfaceButtonsHolderAnimator.SetParentTransform(this.transform);
        interfaceButtonsHolderAnimator.SetPosition(new Vector3(0, 0, INTERFACE_BUTTONS_Z_VALUE));

        Vector2 interfaceButtonSize = new Vector2(128, 128);

        //pause
        GameObject pauseButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_PAUSE_BUTTON, interfaceButtonSize);
        pauseButtonObject.name = "PauseButton";

        GUIButton pauseButton = pauseButtonObject.GetComponent<GUIButton>();
        pauseButton.SetTouchArea(new Vector2(130, 130));
        pauseButton.SetTintColor(ColorUtils.GetRGBAColorFromTSB(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], 1));

        GameObjectAnimator pauseButtonAnimator = pauseButtonObject.GetComponent<GameObjectAnimator>();
        pauseButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float pauseButtonPositionX = -0.5f * screenSize.x + triangleHeight;
        float pauseButtonPositionY = 0.43f * screenSize.y;
        pauseButtonAnimator.SetPosition(new Vector3(pauseButtonPositionX, pauseButtonPositionY, 0));

        //retry
        GameObject retryButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_RETRY_BUTTON, interfaceButtonSize);
        retryButtonObject.name = "RetryButton";

        GUIButton retryButton = retryButtonObject.GetComponent<GUIButton>();
        retryButton.SetTouchArea(new Vector2(130, 130));
        retryButton.SetTintColor(ColorUtils.GetRGBAColorFromTSB(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], 1));

        GameObjectAnimator retryButtonAnimator = retryButtonObject.GetComponent<GameObjectAnimator>();
        retryButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float retryButtonPositionX = 0.5f * screenSize.x - GetBackgroundRenderer().m_triangleHeight;
        float retryButtonPositionY = 0.5f * screenSize.y - GetBackgroundRenderer().m_triangleEdgeLength;
        //float retryButtonPositionX = 0.5f * (screenSize.x - m_rightContentWidth);
        Vector3 gridPosition = m_grid.transform.position;
        //float retryButtonPositionY = gridPosition.y + 0.5f * m_grid.m_gridSize.y - 80;
        retryButtonAnimator.SetPosition(new Vector3(retryButtonPositionX, retryButtonPositionY, 0));

        //GameObject retryTextObject = (GameObject)Instantiate(m_textMeshPfb);
        //retryTextObject.name = "RetryText";

        //TextMesh retryText = retryTextObject.GetComponent<TextMesh>();
        //retryText.text = LanguageUtils.GetTranslationForTag("retry");

        //TextMeshAnimator retryTextAnimator = retryText.GetComponent<TextMeshAnimator>();
        //retryTextAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        //retryTextAnimator.SetPosition(new Vector3(retryButtonPositionX + 2, retryButtonPositionY - 60, 0));
        //retryTextAnimator.SetFontHeight(30);
        //retryTextAnimator.SetColor(Color.white);

        //hints
        GameObject hintsButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_HINTS_BUTTON, interfaceButtonSize);
        hintsButtonObject.name = "HintsButton";

        GUIButton hintsButton = hintsButtonObject.GetComponent<GUIButton>();
        hintsButton.SetTouchArea(new Vector2(130, 130));
        hintsButton.SetTintColor(ColorUtils.GetRGBAColorFromTSB(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], 1));

        GameObjectAnimator hintsButtonAnimator = hintsButtonObject.GetComponent<GameObjectAnimator>();
        hintsButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float hintsButtonPositionX = 0.5f * screenSize.x - 3 * GetBackgroundRenderer().m_triangleHeight;
        float hintsButtonPositionY = 0.5f * screenSize.y - GetBackgroundRenderer().m_triangleEdgeLength;
        //float hintsButtonPositionX = 0.5f * (screenSize.x - m_rightContentWidth);
        //float hintsButtonPositionY = retryButtonPositionY - 160;
        hintsButtonAnimator.SetPosition(new Vector3(hintsButtonPositionX, hintsButtonPositionY, 0));

        //GameObject hintsTextObject = (GameObject)Instantiate(m_textMeshPfb);
        //hintsTextObject.name = "HintsText";

        //TextMesh hintsText = hintsTextObject.GetComponent<TextMesh>();
        //hintsText.text = LanguageUtils.GetTranslationForTag("hints");

        //TextMeshAnimator hintsTextAnimator = hintsText.GetComponent<TextMeshAnimator>();
        //hintsTextAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        //hintsTextAnimator.SetPosition(new Vector3(hintsButtonPositionX, hintsButtonPositionY - 60, 0));
        //hintsTextAnimator.SetFontHeight(30);
        //hintsTextAnimator.SetColor(Color.white);

        //build a vertical line to show separation between interface buttons and grid
        GameObject lineObject = Instantiate(m_colorQuadPfb);
        lineObject.name = "SeparationLine";
        ColorQuad line = lineObject.GetComponent<ColorQuad>();
        line.Init(m_plainWhiteMaterial);
        ColorQuadAnimator lineAnimator = lineObject.GetComponent<ColorQuadAnimator>();
        lineAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        lineAnimator.SetScale(new Vector3(6.0f, screenSize.y - m_topContentHeight - m_bottomContentHeight, 1));
        lineAnimator.SetPosition(new Vector3(0.5f * screenSize.x - m_rightContentWidth, gridPosition.y, 0));
        lineAnimator.SetColor(Color.white);
    }

    /**
     * Fade out grid points without grid borders
     * **/
    public void DismissGridPoints(float fDuration)
    {
        m_grid.DismissGridPoints(fDuration);
    }

    /**
     * Fade out interface buttons holder
     * **/
    public void DismissInterfaceButtons(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator interfaceButtonsHolderAnimator = m_interfaceButtonsHolder.GetComponent<GameObjectAnimator>();
        interfaceButtonsHolderAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Counter to show remaining actions for the player
     * **/
    private void ShowCounter()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject counterObject = (GameObject)Instantiate(m_counterPfb);
        counterObject.name = "Counter";
        m_counter = counterObject.GetComponent<Counter>();

        m_counter.Init();
        m_counter.Build();

        GameObjectAnimator counterAnimator = counterObject.GetComponent<GameObjectAnimator>();
        counterAnimator.SetParentTransform(this.transform);
        counterAnimator.SetPosition(new Vector3(0, 0.428f * screenSize.y, COUNTER_Z_VALUE));
    }

    /**
     * Fades out and remove the counter
     * **/
    private void DismissCounter(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        m_counter.GetComponent<GameObjectAnimator>().FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Fade out every shape except the tiled background one
     * **/
    private void DismissShapes(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        for (int i = 0; i != m_shapesHolder.m_shapes.Count; i++)
        {
            Shape shape = m_shapesHolder.m_shapes[i];
            if (shape.m_state != Shape.ShapeState.TILED_BACKGROUND)
            {
                TexturedMeshAnimator shapeAnimator = shape.m_parentMesh.gameObject.GetComponent<TexturedMeshAnimator>();
                shapeAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
            }
        }
    }

    /**
     * Here we build the dotted outlines (contour and holes) of the shape the player has to reproduce
     * **/
    private void ShowOutlines()
    {
        GameObject outlinesObject = (GameObject)Instantiate(m_outlinesPfb);
        outlinesObject.name = "Outlines";        

        GameObjectAnimator outlinesAnimator = outlinesObject.GetComponent<GameObjectAnimator>();
        outlinesAnimator.SetParentTransform(this.transform);
        outlinesAnimator.SetPosition(new Vector3(0, 0, OUTLINES_Z_VALUE));

        m_outlines = outlinesObject.GetComponent<Outlines>();
        m_outlines.Build();
        m_outlines.Show(false);
    }

    /**
    * For some chapters, axes are predefined and can be dragged directly onto the scene.
    * We build them here
    **/
    private void ShowDraggableAxes()
    {
        //List<PredefinedAxis> predefinedAxes = GetLevelManager().m_currentLevel.m_predefinedAxes;

        //for (int i = 0; i != predefinedAxes.Count; i++)
        //{
        //    PredefinedAxis predefinedAxis = predefinedAxes[i];

        //    DraggableAxisButton button = (DraggableAxisButton)Instantiate(DraggableAxisButton);
        //}
    }

    /**
     * Draw small icons to show the constraints on axes that the player can draw
     * **/
    private void ShowAxisConstraintsIcons()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        List<string> axisConstraints = GetLevelManager().m_currentLevel.m_axisConstraints;

        //Show icons
        m_axisConstraintsIconsHolder = new GameObject("AxisConstraintsIconsHolder");

        GameObjectAnimator axisConstraintsIconsHolderAnimator = m_axisConstraintsIconsHolder.AddComponent<GameObjectAnimator>();
        axisConstraintsIconsHolderAnimator.SetParentTransform(this.transform);
        axisConstraintsIconsHolderAnimator.SetPosition(new Vector3(0, -0.5f * screenSize.y + 0.5f * m_bottomContentHeight, AXIS_CONSTRAINTS_ICONS_Z_VALUE));

        float iconMargin = 20.0f;
        Vector2 iconSize = new Vector2(64, 64);

        for (int i = 0; i != axisConstraints.Count; i++)
        {
            GameObject iconObject = (GameObject)Instantiate(m_texQuadPfb);
            iconObject.name = "ConstraintAxisIcon";

            UVQuad iconQuad = iconObject.GetComponent<UVQuad>();
            iconQuad.Init(GetMaterialForAxisConstraint(axisConstraints[i]));

            TexturedQuadAnimator iconAnimator = iconObject.GetComponent<TexturedQuadAnimator>();
            iconAnimator.SetParentTransform(m_axisConstraintsIconsHolder.transform);
            iconAnimator.SetScale(iconSize);
            iconAnimator.SetColor(Color.white);
            float iconPositionX;
            if (axisConstraints.Count % 2 == 0) //even
                iconPositionX = (i - axisConstraints.Count / 2 + 0.5f) * (iconSize.x + iconMargin);
            else //odd
                iconPositionX = (i - axisConstraints.Count / 2) * (iconSize.x + iconMargin);
            iconAnimator.SetPosition(new Vector3(iconPositionX, 0, 0));
        }

        //build a horizontal line to show separation between axis constraints and grid
        GameObject lineObject = Instantiate(m_colorQuadPfb);
        lineObject.name = "SeparationLine";
        ColorQuad line = lineObject.GetComponent<ColorQuad>();
        line.Init(m_plainWhiteMaterial);
        ColorQuadAnimator lineAnimator = lineObject.GetComponent<ColorQuadAnimator>();
        lineAnimator.SetParentTransform(m_axisConstraintsIconsHolder.transform);
        lineAnimator.SetScale(new Vector3(screenSize.x, 6.0f, 1));
        lineAnimator.SetPosition(new Vector3(0, 0.5f * m_bottomContentHeight, 0));
        lineAnimator.SetColor(Color.white);
    }

    public void DismissAxisConstraintsIcons(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator iconsHolderAnimator = m_axisConstraintsIconsHolder.GetComponent<GameObjectAnimator>();
        iconsHolderAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Build the 3 action buttons that appear on the left side of the screen
     * -First button allows player to switch between action modes (symmetry types or shape translation)
     * -Second button is for modifying the behavior of color symmetrization (addition or soustraction)
     * -Third button is for picking a color that will apply to the symmetry done by the user by filtering shapes that are not of that specific color
     * **/
    private void ShowActionButtons()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        Vector2 actionButtonsHolderSize = new Vector2(m_leftContentWidth, m_grid.m_maxGridSize.y);
        m_actionButtonsHolder = new GameObject("ActionButtons");
        GameObjectAnimator actionButtonsHolderAnimator = m_actionButtonsHolder.AddComponent<GameObjectAnimator>();
        actionButtonsHolderAnimator.SetParentTransform(this.transform);
        actionButtonsHolderAnimator.SetPosition(new Vector3(-0.5f * screenSize.x + 0.5f * actionButtonsHolderSize.x, m_grid.transform.localPosition.y, ACTION_BUTTONS_Z_VALUE));

        m_actionButtons = new ActionButton[3];

        //Show MAIN_ACTIONS button
        GUIButton.GUIButtonID[] childIDs = new GUIButton.GUIButtonID[4];
        childIDs[0] = GUIButton.GUIButtonID.ID_MOVE_SHAPE;
        childIDs[1] = GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE;
        childIDs[2] = GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES;
        childIDs[3] = GUIButton.GUIButtonID.ID_POINT_SYMMETRY;

        //Vector3 buttonPosition = new Vector3(0, 0.5f * actionButtonsHolderSize.y - 100.0f, 0);
        Vector3 buttonPosition = new Vector3(0, 0.5f * actionButtonsHolderSize.y - 50.0f, 0);

        GameObject buttonObject = GetGUIManager().CreateActionButton(ActionButton.GroupID.MAIN_ACTIONS,
                                                                     childIDs,
                                                                     m_leftContentWidth);
        buttonObject.name = "TopActionBtn";

        ActionButton button = buttonObject.GetComponent<ActionButton>();
        m_actionButtons[0] = button;

        GameObjectAnimator buttonAnimator = buttonObject.GetComponent<GameObjectAnimator>();
        buttonAnimator.SetParentTransform(m_actionButtonsHolder.transform);
        buttonAnimator.SetPosition(buttonPosition);

        ////Show CLIP_OPERATION button
        //childIDs = new GUIButton.GUIButtonID[2];
        //childIDs[0] = GUIButton.GUIButtonID.ID_OPERATION_ADD;
        //childIDs[1] = GUIButton.GUIButtonID.ID_OPERATION_SUBSTRACT;

        //buttonObject = GetGUIManager().CreateActionButton(ActionButton.GroupID.CLIP_OPERATION,
        //                                                  childIDs);

        //buttonObject.name = "MiddleActionBtn";

        //button = buttonObject.GetComponent<ActionButton>();
        //m_actionButtons[1] = button;

        //buttonAnimator = buttonObject.GetComponent<GameObjectAnimator>();
        //buttonAnimator.SetParentTransform(actionButtonsHolder.transform);
        //buttonAnimator.SetPosition(buttonPosition);

        ////Show COLOR_FILTERING button
        //childIDs = new GUIButton.GUIButtonID[1];
        //childIDs[0] = GUIButton.GUIButtonID.ID_COLOR_FILTER;

        //buttonObject = GetGUIManager().CreateActionButton(ActionButton.GroupID.COLOR_FILTERING,
        //                                                  childIDs);

        //buttonObject.name = "BottomActionBtn";

        //button = buttonObject.GetComponent<ActionButton>();
        //m_actionButtons[2] = button;

        //buttonAnimator = buttonObject.GetComponent<GameObjectAnimator>();
        //buttonAnimator.SetParentTransform(actionButtonsHolder.transform);
        //buttonAnimator.SetPosition(buttonPosition);

        for (int i = 0; i != m_actionButtons.Length; i++)
        {
            if (m_actionButtons[i] != null)
                m_actionButtons[i].Show();
        }

        //build a vertical line to show separation between action buttons and grid
        GameObject lineObject = Instantiate(m_colorQuadPfb);
        lineObject.name = "SeparationLine";
        ColorQuad line = lineObject.GetComponent<ColorQuad>();
        line.Init(m_plainWhiteMaterial);
        ColorQuadAnimator lineAnimator = lineObject.GetComponent<ColorQuadAnimator>();
        lineAnimator.SetParentTransform(m_actionButtonsHolder.transform);
        lineAnimator.SetScale(new Vector3(6.0f, screenSize.y - m_topContentHeight - m_bottomContentHeight, 1));
        lineAnimator.SetPosition(new Vector3(0.5f * actionButtonsHolderSize.x, 0, 0));
        lineAnimator.SetColor(Color.white);
    }

    /**
     * Buttons associated with pending axes waiting to be deployed
     * **/
    public void ShowDeployAxisButtons()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        GameObject deployAxisButtonsHolder = new GameObject();
        deployAxisButtonsHolder.name = "DeployAxisButtonsHolder";

        GameObjectAnimator holderAnimator = deployAxisButtonsHolder.AddComponent<GameObjectAnimator>();
        holderAnimator.SetParentTransform(this.transform);
        Vector3 gridPosition = m_grid.transform.position;
        Vector2 gridSize = m_grid.m_gridSize;
        Vector3 holderPosition = new Vector3(0.5f * (screenSize.x - m_rightContentWidth),
                                             gridPosition.y - 0.5f * gridSize.y,
                                             DEPLOY_AXIS_BUTTONS_Z_VALUE);
        holderAnimator.SetPosition(holderPosition);

        int buttonsCount = 4;
        float verticalDistanceBetweenButtons = 130.0f;
        float bottomMargin = 20.0f;
        for (int i = 0; i != buttonsCount; i++)
        {
            GameObject axisDeployButtonObject = (GameObject)Instantiate(m_deployAxisButtonPfb);
            axisDeployButtonObject.name = "AxisDeploy" + (i + 1);

            AxisDeploymentButton axisDeployButton = axisDeployButtonObject.GetComponent<AxisDeploymentButton>();
            axisDeployButton.BuildForNumber(i + 1);

            GameObjectAnimator buttonAnimator = axisDeployButtonObject.GetComponent<GameObjectAnimator>();
            buttonAnimator.SetParentTransform(deployAxisButtonsHolder.transform);
            float buttonPositionY = (i + 0.5f) * verticalDistanceBetweenButtons + bottomMargin;
            buttonAnimator.SetPosition(new Vector3(0, buttonPositionY, 0));
        }
    }

    /**
     * Symmetry points that player can drag onto the scene
     * **/
    private void ShowSymmetryPointButtons()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //holder
        GameObject symmetryPointsHolder = new GameObject();
        symmetryPointsHolder.name = "SymmetryPointsHolder";

        GameObjectAnimator holderAnimator = symmetryPointsHolder.AddComponent<GameObjectAnimator>();
        holderAnimator.SetParentTransform(this.transform);
        Vector3 gridPosition = m_grid.transform.position;
        Vector2 gridSize = m_grid.m_gridSize;
        Vector3 holderPosition = new Vector3(0.5f * (screenSize.x - m_rightContentWidth),
                                             gridPosition.y + 0.5f * gridSize.y,
                                             DEPLOY_AXIS_BUTTONS_Z_VALUE);
        holderAnimator.SetPosition(holderPosition);


        //build points
        int symmetryPointsCount = 3;
        float verticalDistanceBetweenButtons = 130.0f;
        float topMargin = 20.0f;
        for (int i = 0; i != symmetryPointsCount; i++)
        {
            GameObject symmetryPointObject = (GameObject)Instantiate(m_symmetryPointPfb);
            symmetryPointObject.name = "SymmetryPoint" + (i + 1);

            SymmetryPoint symmetryPoint = symmetryPointObject.GetComponent<SymmetryPoint>();
            symmetryPoint.Build();

            GameObjectAnimator symmetryPointAnimator = symmetryPointObject.GetComponent<GameObjectAnimator>();
            symmetryPointAnimator.SetParentTransform(symmetryPointsHolder.transform);
            float buttonPositionY = -(i + 0.5f) * verticalDistanceBetweenButtons - topMargin;
            symmetryPointAnimator.SetPosition(new Vector3(0, buttonPositionY, 0));

            GetGUIManager().m_symmetryPoints.Add(symmetryPoint);
        }
    }

    private void DismissActionButtons(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        m_actionButtonsHolder.GetComponent<GameObjectAnimator>().FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);

        ////Fade out every button
        //for (int i = 0; i != m_actionButtons.Length; i++)
        //{
        //    if (m_actionButtons[i] != null)
        //        m_actionButtons[i].Dismiss(fDuration, fDelay, bDestroyOnFinish);
        //}

    }

    /**
     * Returns the current ID of the action button at specified location
     * **/
    public GUIButton.GUIButtonID GetActionButtonID(ActionButton.GroupID groupID)
    {
        for (int i = 0; i != m_actionButtons.Length; i++)
        {
            if (m_actionButtons[i].m_groupID == groupID)
                return m_actionButtons[i].GetSelectedChildID();
        }

        return GUIButton.GUIButtonID.NONE;
    }

    /**
     * Build the object that will hold shapes
     * **/
    private void BuildShapesHolder()
    {
        GameObject shapesHolderObject = (GameObject)Instantiate(m_shapesHolderPfb);
        shapesHolderObject.name = "Shapes";

        m_shapesHolder = shapesHolderObject.GetComponent<Shapes>();

        GameObjectAnimator shapesAnimator = shapesHolderObject.GetComponent<GameObjectAnimator>();
        shapesAnimator.SetParentTransform(this.transform);
        shapesAnimator.SetPosition(new Vector3(0, 0, SHAPES_Z_VALUE));
    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void ShowInitialShapes()
    {
        List<Shape> initialShapes = GetLevelManager().m_currentLevel.m_initialShapes;
        Debug.Log(GetLevelManager().m_currentLevel.m_chapterRelativeNumber);
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = new Shape(initialShapes[iShapeIndex]); //make a deep copy of the shape object stored in the level manager

            //clean the shape (remove aligned vertices that can lead to errors before actually triangulate the shape)
            shape.PrepareShapeForTriangulation();

            //First triangulate the shape and set it to STATIC
            shape.Triangulate();
            shape.m_state = Shape.ShapeState.STATIC;

            m_shapesHolder.CreateShapeObjectFromData(shape, false);
        }

        //shapesAnimator.SetOpacity(0);
        //shapesAnimator.FadeTo(Shapes.SHAPES_OPACITY, 1.0f);

        //TMP debug
        //InitClippingManager();
        //Shape firstShape = m_shapesHolder.m_shapes[0];
        //List<Shape> resultingShapes = GetClippingManager().ShapesOperation(firstShape, firstShape, ClipperLib.ClipType.ctUnion);
        //for (int iShapeIndex = 0; iShapeIndex != resultingShapes.Count; iShapeIndex++)
        //{
        //    Shape shape = new Shape(resultingShapes[iShapeIndex]); //make a deep copy of the shape object stored in the level manager

        //    //First triangulate the shape and set the color of each triangle
        //    shape.m_state = Shape.ShapeState.STATIC;

        //    m_shapesHolder.CreateShapeObjectFromData(shape, false);
        //}
        //m_shapesHolder.DestroyShapeObjectForShape(firstShape);
        //m_shapesHolder.m_shapes.Remove(firstShape);
    }

    /**
     * Draw hatchings to fill empty areas like holes or borders
     * **/
    private void DrawHatchings()
    {
        //objet holding all hatchings
        GameObject hatchingsHolder = new GameObject("Hatchings");
        Vector2 gridPosition = m_grid.transform.position;

        GameObjectAnimator holderAnimator = hatchingsHolder.AddComponent<GameObjectAnimator>();
        holderAnimator.SetParentTransform(this.transform);
        holderAnimator.SetPosition(new Vector3(0, 0, HATCHINGS_Z_VALUE));

        //left and right hatchings
        if (0.5f * (m_grid.m_maxGridSize.x - m_grid.m_gridSize.x) >= DRAW_HATCHINGS_THRESHOLD)
        {
            //left
            GameObject hatchingsMeshObject = (GameObject)Instantiate(m_texturedMeshPfb);
            hatchingsMeshObject.name = "HatchingsMesh";

            TexturedMeshAnimator hatchingsMeshAnimator = hatchingsMeshObject.GetComponent<TexturedMeshAnimator>();
            hatchingsMeshAnimator.SetParentTransform(hatchingsHolder.transform);
            hatchingsMeshAnimator.SetPosition(Vector3.zero);

            TexturedMesh hatchingsMesh = hatchingsMeshObject.GetComponent<TexturedMesh>();
            hatchingsMesh.Init(m_hatchingsMaterial);
            hatchingsMesh.m_textureSize = new Vector2(64, 64);
            Vector2 point1 = new Vector2(gridPosition.x - 0.5f * m_grid.m_maxGridSize.x, gridPosition.y - 0.5f * m_grid.m_maxGridSize.y);
            Vector2 point2 = new Vector2(gridPosition.x - 0.5f * m_grid.m_maxGridSize.x, gridPosition.y + 0.5f * m_grid.m_maxGridSize.y);
            Vector2 point3 = new Vector2(gridPosition.x - 0.5f * m_grid.m_gridSize.x, gridPosition.y - 0.5f * m_grid.m_maxGridSize.y);
            Vector2 point4 = new Vector2(gridPosition.x - 0.5f * m_grid.m_gridSize.x, gridPosition.y + 0.5f * m_grid.m_maxGridSize.y);
            hatchingsMesh.AddQuad(point1, point2, point3, point4);

            //right
            hatchingsMeshObject = (GameObject)Instantiate(m_texturedMeshPfb);
            hatchingsMeshObject.name = "HatchingsMesh";

            hatchingsMeshAnimator = hatchingsMeshObject.GetComponent<TexturedMeshAnimator>();
            hatchingsMeshAnimator.SetParentTransform(hatchingsHolder.transform);
            hatchingsMeshAnimator.SetPosition(Vector3.zero);

            hatchingsMesh = hatchingsMeshObject.GetComponent<TexturedMesh>();
            hatchingsMesh.Init(m_hatchingsMaterial);
            hatchingsMesh.m_textureSize = new Vector2(64, 64);
            point1 = new Vector2(gridPosition.x + 0.5f * m_grid.m_gridSize.x, gridPosition.y - 0.5f * m_grid.m_maxGridSize.y);
            point2 = new Vector2(gridPosition.x + 0.5f * m_grid.m_gridSize.x, gridPosition.y + 0.5f * m_grid.m_maxGridSize.y);
            point3 = new Vector2(gridPosition.x + 0.5f * m_grid.m_maxGridSize.x, gridPosition.y - 0.5f * m_grid.m_maxGridSize.y);
            point4 = new Vector2(gridPosition.x + 0.5f * m_grid.m_maxGridSize.x, gridPosition.y + 0.5f * m_grid.m_maxGridSize.y);
            hatchingsMesh.AddQuad(point1, point2, point3, point4);
        }

        //bottom and top hatchings
        if (0.5f * (m_grid.m_maxGridSize.y - m_grid.m_gridSize.y) >= DRAW_HATCHINGS_THRESHOLD)
        {
            //top
            GameObject hatchingsMeshObject = (GameObject)Instantiate(m_texturedMeshPfb);
            hatchingsMeshObject.name = "HatchingsMesh";

            TexturedMeshAnimator hatchingsMeshAnimator = hatchingsMeshObject.GetComponent<TexturedMeshAnimator>();
            hatchingsMeshAnimator.SetParentTransform(hatchingsHolder.transform);
            hatchingsMeshAnimator.SetPosition(Vector3.zero);

            TexturedMesh hatchingsMesh = hatchingsMeshObject.GetComponent<TexturedMesh>();
            hatchingsMesh.Init(m_hatchingsMaterial);
            hatchingsMesh.m_textureSize = new Vector2(64, 64);
            Vector2 point1 = new Vector2(gridPosition.x - 0.5f * m_grid.m_gridSize.x, gridPosition.y + 0.5f * m_grid.m_gridSize.y);
            Vector2 point2 = new Vector2(gridPosition.x - 0.5f * m_grid.m_gridSize.x, gridPosition.y + 0.5f * m_grid.m_maxGridSize.y);
            Vector2 point3 = new Vector2(gridPosition.x + 0.5f * m_grid.m_gridSize.x, gridPosition.y + 0.5f * m_grid.m_gridSize.y);
            Vector2 point4 = new Vector2(gridPosition.x + 0.5f * m_grid.m_gridSize.x, gridPosition.y + 0.5f * m_grid.m_maxGridSize.y);
            hatchingsMesh.AddQuad(point1, point2, point3, point4);

            //bottom
            hatchingsMeshObject = (GameObject)Instantiate(m_texturedMeshPfb);
            hatchingsMeshObject.name = "HatchingsMesh";

            hatchingsMeshAnimator = hatchingsMeshObject.GetComponent<TexturedMeshAnimator>();
            hatchingsMeshAnimator.SetParentTransform(hatchingsHolder.transform);
            hatchingsMeshAnimator.SetPosition(Vector3.zero);

            hatchingsMesh = hatchingsMeshObject.GetComponent<TexturedMesh>();
            hatchingsMesh.Init(m_hatchingsMaterial);
            hatchingsMesh.m_textureSize = new Vector2(64, 64);
            point1 = new Vector2(gridPosition.x - 0.5f * m_grid.m_gridSize.x, gridPosition.y - 0.5f * m_grid.m_maxGridSize.y);
            point2 = new Vector2(gridPosition.x - 0.5f * m_grid.m_gridSize.x, gridPosition.y - 0.5f * m_grid.m_gridSize.y);
            point3 = new Vector2(gridPosition.x + 0.5f * m_grid.m_gridSize.x, gridPosition.y - 0.5f * m_grid.m_maxGridSize.y);
            point4 = new Vector2(gridPosition.x + 0.5f * m_grid.m_gridSize.x, gridPosition.y - 0.5f * m_grid.m_gridSize.y);
            hatchingsMesh.AddQuad(point1, point2, point3, point4);
        }

        //outlines holes
        //for (int i = 0; i != m_outlines.m_outlinesList.Count; i++)
        //{
        //    DottedOutline outline = m_outlines.m_outlinesList[i];
        //    for (int j = 0; j != outline.m_holes.Count; j++)
        //    {
        //        GameObject hatchingsMeshObject = (GameObject)Instantiate(m_texturedMeshPfb);
        //        hatchingsMeshObject.name = "HatchingsMesh";

        //        TexturedMeshAnimator hatchingsMeshAnimator = hatchingsMeshObject.GetComponent<TexturedMeshAnimator>();
        //        hatchingsMeshAnimator.SetParentTransform(hatchingsHolder.transform);
        //        hatchingsMeshAnimator.SetPosition(Vector3.zero);

        //        TexturedMesh hatchingsMesh = hatchingsMeshObject.GetComponent<TexturedMesh>();
        //        hatchingsMesh.Init(m_hatchingsMaterial);
        //        hatchingsMesh.m_textureSize = new Vector2(64, 64);

        //        //Quickly make this hole as a GridTriangulable to perform triangulation on it
        //        GridTriangulable holeTriangulable = new GridTriangulable(outline.m_holes[j]);
        //        holeTriangulable.Triangulate();

        //        for (int k = 0; k != holeTriangulable.m_triangles.Count; k++)
        //        {
        //            GridTriangle holeTriangle = holeTriangulable.m_triangles[k];
        //            Vector2 pt1 = m_grid.GetPointWorldCoordinatesFromGridCoordinates(holeTriangle.m_points[0]);
        //            Vector2 pt2 = m_grid.GetPointWorldCoordinatesFromGridCoordinates(holeTriangle.m_points[1]);
        //            Vector2 pt3 = m_grid.GetPointWorldCoordinatesFromGridCoordinates(holeTriangle.m_points[2]);
        //            hatchingsMesh.AddTriangle(pt1, pt3, pt2);
        //        }                    
        //    }
        //}
    }

    private void BuildAxesHolder()
    {
        GameObject axesHolderObject = (GameObject)Instantiate(m_axesPfb);
        m_axesHolder = axesHolderObject.GetComponent<Axes>();
        GameObjectAnimator axesAnimator = axesHolderObject.GetComponent<GameObjectAnimator>();
        axesAnimator.SetParentTransform(this.transform);
        axesAnimator.SetPosition(new Vector3(0, 0, AXES_Z_VALUE));
    }

    /**
     * If symmetries are stackable in this level, show the stack on the right side of the screen
     * **/
    //private void ShowSymmetryStack()
    //{
    //    if (GetLevelManager().m_currentLevel.m_symmetriesStackable)
    //    {
    //        Vector2 screenSize = ScreenUtils.GetScreenSize();

    //        GameObject gameStackObject = (GameObject)Instantiate(m_gameStackPfb);
    //        gameStackObject.name = "SymmetryStack";

    //        GameObjectAnimator gameStackAnimator = gameStackObject.GetComponent<GameObjectAnimator>();
    //        gameStackAnimator.SetParentTransform(this.transform);
    //        gameStackAnimator.SetPosition(new Vector3(0.5f * screenSize.x - 96.0f, 256.0f, SYMMETRY_STACK_Z_VALUE));
    //        gameStackAnimator.SetOpacity(0);
    //        gameStackAnimator.FadeTo(1.0f, 1.0f);

    //        m_gameStack = gameStackObject.GetComponent<GameStack>();
    //        m_gameStack.Build();
    //    }
    //}

    private void InitClippingManager()
    {
        m_clippingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClippingManager>();
        m_clippingManager.Init();

        //make a dummy clipping to speed up the next clipping
        //GetQueuedThreadedJobsManager().AddJob(new ThreadedJob(new ThreadedJob.ThreadFunction(MakeFirstDummyClippingOperation)));        
    }

    /**
     * To speed up the effective clipping operations we will do when drawing axes, we thread here a simple random clipping operation when the game starts
     * **/
    private void MakeFirstDummyClippingOperation()
    {
        Contour dummyContour = new Contour(4);
        dummyContour.Add(new GridPoint(0, 0, true));
        dummyContour.Add(new GridPoint(10, 0, true));
        dummyContour.Add(new GridPoint(0, 5, true));
        Shape dummySubjShape = new Shape(dummyContour);
        Shape dummyClipShape = new Shape(dummyContour);

        m_clippingManager.ShapesOperation(dummySubjShape, dummyClipShape, ClipperLib.ClipType.ctUnion, true);
    }

    /**
     * Build all the directions the axes can be drawn and store them in a list
     * **/
    public void BuildConstrainedDirections()
    {
        m_constrainedDirections = new List<Vector2>();
        m_constrainedDirections.Capacity = 8;

        Vector2 rightDirection = new Vector2(1, 0);
        Vector2 bottomDirection = new Vector2(0, -1);
        Vector2 leftDirection = new Vector2(-1, 0);
        Vector2 topDirection = new Vector2(0, 1);
        Vector2 topRightDirection = new Vector2(1, 1);
        Vector2 bottomRightDirection = new Vector2(1, -1);
        Vector2 bottomLeftDirection = new Vector2(-1, -1);
        Vector2 topLeftDirection = new Vector2(-1, 1);
        topRightDirection.Normalize();
        bottomRightDirection.Normalize();
        bottomLeftDirection.Normalize();
        topLeftDirection.Normalize();

        ///***
        // * TMP DEBUG: unlock all directions
        // * ***/
        //AddConstrainedDirection(rightDirection);
        //AddConstrainedDirection(bottomDirection);
        //AddConstrainedDirection(leftDirection);
        //AddConstrainedDirection(topDirection);
        //AddConstrainedDirection(topRightDirection);
        //AddConstrainedDirection(bottomRightDirection);
        //AddConstrainedDirection(bottomLeftDirection);
        //AddConstrainedDirection(topLeftDirection);
        //return;
        ///***
        // * 
        // * ***/

        List<string> axisConstraints = GetLevelManager().m_currentLevel.m_axisConstraints;
        for (int i = 0; i != axisConstraints.Count; i++)
        {
            if (axisConstraints[i].Equals(CONSTRAINT_SYMMETRY_AXIS_HORIZONTAL))
            {
                AddConstrainedDirection(rightDirection);
                AddConstrainedDirection(leftDirection);
            }
            else if (axisConstraints[i].Equals(CONSTRAINT_SYMMETRY_AXIS_VERTICAL))
            {
                AddConstrainedDirection(topDirection);
                AddConstrainedDirection(bottomDirection);
            }
            else if (axisConstraints[i].Equals(CONSTRAINT_SYMMETRY_AXES_STRAIGHT))
            {
                AddConstrainedDirection(rightDirection);
                AddConstrainedDirection(leftDirection);
                AddConstrainedDirection(topDirection);
                AddConstrainedDirection(bottomDirection);
            }
            else if (axisConstraints[i].Equals(CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_LEFT))
            {
                AddConstrainedDirection(topLeftDirection);
                AddConstrainedDirection(bottomRightDirection);
            }
            else if (axisConstraints[i].Equals(CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_RIGHT))
            {
                AddConstrainedDirection(topRightDirection);
                AddConstrainedDirection(bottomLeftDirection);
            }
            else if (axisConstraints[i].Equals(CONSTRAINT_SYMMETRY_AXES_DIAGONALS))
            {
                AddConstrainedDirection(topLeftDirection);
                AddConstrainedDirection(bottomRightDirection);
                AddConstrainedDirection(topRightDirection);
                AddConstrainedDirection(bottomLeftDirection);
            }
        }
    }

    /**
     * Add a constrained direction if not already stored
     * **/
    private void AddConstrainedDirection(Vector2 direction)
    {
        for (int i = 0; i != m_constrainedDirections.Count; i++)
        {
            if (m_constrainedDirections[i].Equals(direction))
                return;
        }

        m_constrainedDirections.Add(direction);
    }

    /**
     * Clone the relevant material for the passed axis constraint string
     * **/
    private Material GetMaterialForAxisConstraint(string axisConstraint)
    {
        if (axisConstraint.Equals(CONSTRAINT_SYMMETRY_AXIS_HORIZONTAL))
            return Instantiate(m_horizontalAxisMaterial);
        else if (axisConstraint.Equals(CONSTRAINT_SYMMETRY_AXIS_VERTICAL))
            return Instantiate(m_verticalAxisMaterial);
        else if (axisConstraint.Equals(CONSTRAINT_SYMMETRY_AXES_STRAIGHT))
            return Instantiate(m_straightAxesMaterial);
        else if (axisConstraint.Equals(CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_LEFT))
            return Instantiate(m_leftDiagonalAxisMaterial);
        else if (axisConstraint.Equals(CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_RIGHT))
            return Instantiate(m_rightDiagonalAxisMaterial);
        else if (axisConstraint.Equals(CONSTRAINT_SYMMETRY_AXES_DIAGONALS))
            return Instantiate(m_diagonalAxesMaterial);

        return null;
    }

    public ClippingManager GetClippingManager()
    {
        if (m_clippingManager == null)
            m_clippingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClippingManager>();

        return m_clippingManager;
    }

    public ThreadedJobsManager GetThreadedJobsManager()
    {
        if (m_threadedJobsManager == null)
            m_threadedJobsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ThreadedJobsManager>();

        return m_threadedJobsManager;
    }

    public QueuedThreadedJobsManager GetQueuedThreadedJobsManager()
    {
        if (m_queuedThreadedJobsManager == null)
            m_queuedThreadedJobsManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<QueuedThreadedJobsManager>();

        return m_queuedThreadedJobsManager;
    }


    //----------------------------------------------------------------------------------------------------//
    //--------------------------------------------END OF LEVEL--------------------------------------------//
    //----------------------------------------------------------------------------------------------------//

    public enum GameStatus
    {
        WAITING_FOR_START, //game has not started yet
        RUNNING, //game is ready and running
        PAUSED, //player has paused the game by showing some menus for instance
        VICTORY, //player has just won this level
        DEFEAT, //level has ended with a defeat
        FINISHED //the level is done and player is waiting for current level to restart or next level to start
    };

    public GameStatus m_gameStatus { get; set; }

    /**
     * Returns the current status of the game
     * **/
    public GameStatus GetGameStatus()
    {
        if (m_gameStatus == GameStatus.DEFEAT || m_gameStatus == GameStatus.VICTORY || m_gameStatus == GameStatus.FINISHED)
            return m_gameStatus;

        bool victory = IsVictory();
        if (victory)
            m_gameStatus = GameStatus.VICTORY;
        else
        {
            bool defeat = IsDefeat();
            if (defeat)
                m_gameStatus = GameStatus.DEFEAT;
            else //neither victory nor defeat, keep playing
                m_gameStatus = GameStatus.RUNNING;
        }

        return m_gameStatus;
    }

    /**
     * Checks if the contour is filled exactly
     * Calculate the sum of the areas of contours and compare it to the area occupied by all the shapes
     * **/
    public bool IsVictory()
    {
        if (m_shapesHolder == null)
            return false;

        List<Shape> allShapes = m_shapesHolder.m_shapes;

        if (allShapes.Count == 0) //if a level contains no shape (i.e in debug mode for instance)
            return false;

        //First we check if every shape (except the tiled background) is static
        for (int iShapeIdx = 0; iShapeIdx != allShapes.Count; iShapeIdx++)
        {
            Shape shape = allShapes[iShapeIdx];

            if (shape.m_state != Shape.ShapeState.STATIC && shape.m_state != Shape.ShapeState.TILED_BACKGROUND)
                return false;
        }

        //Debug.Log("STEP1 CHECK");

        //Check if the sum of shapes areas is equal to the sum of outlines areas        
        List<DottedOutline> outlines = m_outlines.m_outlinesList;
        float shapesArea = 0;
        float outlinesArea = 0;
        for (int i = 0; i != allShapes.Count; i++)
        {
            if (allShapes[i].m_state == Shape.ShapeState.TILED_BACKGROUND)
                continue;

            allShapes[i].CalculateArea();
            shapesArea += allShapes[i].m_area;
        }

        for (int i = 0; i != outlines.Count; i++)
        {
            outlinesArea += outlines[i].m_area;
        }

        //Debug.Log("shapesArea:" + shapesArea + " outlinesArea:" + outlinesArea);
        //if (MathUtils.AreFloatsEqual(shapesArea, outlinesArea, 1))
        //    Debug.Log("STEP2 CHECK");

        //Finally check if every shape is fully inside one of the dotted outlines
        if (outlines.Count == 0) //if a level contains no outline (i.e in debug mode for instance)
            return false;

        for (int iShapeIdx = 0; iShapeIdx != allShapes.Count; iShapeIdx++)
        {
            Shape shape = allShapes[iShapeIdx];

            if (shape.m_state == Shape.ShapeState.TILED_BACKGROUND)
                continue;

            bool bInsideOneOutline = false; //check if the shape is inside one of the outlines or outside all outlines
            for (int iOutlineIdx = 0; iOutlineIdx != outlines.Count; iOutlineIdx++)
            {
                DottedOutline outline = outlines[iOutlineIdx];

                if (shape.IsInsideOutline(outline)) //strict intersection between the shape and one contour, shape cannot be fully inside the contour
                {
                    bInsideOneOutline = true;
                    break; //no need to test other outlines
                }

                ////Check if every point of every triangle in shape and its center is inside the outline
                //if (!bInsideOneOutline)
                //{
                //    for (int iTriangleIdx = 0; iTriangleIdx != shape.m_triangles.Count; iTriangleIdx++)
                //    {
                //        GridTriangle triangle = shape.m_triangles[iTriangleIdx];
                //        if (outline.ContainsPoint(triangle.m_points[0]) &&
                //            outline.ContainsPoint(triangle.m_points[1]) &&
                //            outline.ContainsPoint(triangle.m_points[2]) &&
                //            outline.ContainsPoint(triangle.GetCenter()))
                //        {
                //            bInsideOneOutline = true;
                //        }
                //    }
                //}
            }

            if (!bInsideOneOutline)
                return false;
        }

        Debug.Log("STEP3 CHECK");

        

        return MathUtils.AreFloatsEqual(shapesArea, outlinesArea, 1); //set an error of 1 to test if shapes areas are equal
    }

    /**
     * Simply checks if counter is at maximum fill, because we know that conditions of victory have already been checked out negatively at this point
     * **/
    public bool IsDefeat()
    {
        return false; //TODO remove this line

        //GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;

        //if (!gameScene.m_isShown)
        //    return false;

        //bool bDefeat = gameScene.m_counter.isFull();
        //if (bDefeat)
        //    Debug.Log("DEFEAT");
        //return gameScene.m_counter.isFull();
    }

    /**
     * If victory:
     * -Ends the current level by fading out the grid and contours and disabling touch
     * -After a few seconds launch next level
     * If defeat:
     * -Restart the level
     * **/
    public void EndLevel(GameStatus gameStatus)
    {
        if (m_gameStatus == GameStatus.FINISHED)
            return;

        m_gameStatus = GameStatus.FINISHED;

        //Clear symmetry points
        GetGUIManager().ClearStoredElements();

        if (gameStatus == GameStatus.VICTORY)
        {
            Debug.Log("EndLevel VICTORY");
            GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(DismissSceneElementsOnVictory), 1.5f);

            LevelManager levelManager = GetLevelManager();
            if (levelManager.IsCurrentLevelDebugLevel() > 0)
            {
                Debug.Log("debug");
                GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(OnFinishEndingLevelVictory), 4.0f);
            }
            else
            {
                //Save the status of this level
                levelManager.m_currentLevel.SetAsDone();

                int currentLevelNumber = levelManager.m_currentLevel.m_chapterRelativeNumber;
                if (currentLevelNumber < levelManager.m_currentChapter.m_levels.Length)
                {
                    levelManager.SetLevelOnCurrentChapter(levelManager.m_currentLevel.m_chapterRelativeNumber + 1);
                    GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(OnFinishEndingLevelVictory), 5.0f);
                }
                else
                {
                    //TODO go to next chapter by showing chapter menu for instance
                }               
            }
        }
        else if (gameStatus == GameStatus.DEFEAT)
        {
            Debug.Log("EndLevel DEFEAT");
            //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false, 0.0f, 0.5f, 0.5f); //restart the level
            //((LevelIntro)GetSceneManager().m_pendingScene).m_startingFromLevelsScene = false;
        }
    }

    /**
     * Fade out and remove every scene element except shapes holder when player completed a level
     * + destroy remaining element after a certain delay leaving time for the level intro scene backgorund to cover the whole screen
     * **/
    private void DismissSceneElementsOnVictory()
    {
        m_outlines.Dismiss(1.0f);
        m_axesHolder.Dismiss(1.0f);
        DismissGridPoints(0.5f);
        DismissInterfaceButtons(0.5f);
        DismissActionButtons(1.0f);
        DismissAxisConstraintsIcons(0.5f);
        DismissCounter(0.5f);
        //DismissShapes(0.5f, 4.0f);

        m_shapesHolder.transform.parent = null;
        Destroy(m_shapesHolder.gameObject, 5.0f);
        m_chalkboardObject.transform.parent = null;
        Destroy(m_chalkboardObject, 5.0f);
        m_grid.gameObject.transform.parent = null;
        Destroy(m_grid.gameObject, 5.0f);
    }

    /**
     * Function called when all scene elements have been faded out except shapes (victory)
     * Time to switch scene and go to next level or next chapter (if level 16 has been reached)
     * **/
    private void OnFinishEndingLevelVictory()
    {
        m_shapesHolder.m_shapes.Clear();
        GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(StartLevelIntroScene), 0.5f);
    }

    private void StartLevelIntroScene()
    {
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false); //restart the level
        ((LevelIntro)GetSceneManager().m_pendingScene).m_startingFromLevelsScene = false;
    }

    public void Update()
    {
        if (m_gameStatus == GameStatus.RUNNING)
        {
            GameStatus gameStatus = GetGameStatus();
            if (gameStatus == GameStatus.VICTORY || gameStatus == GameStatus.DEFEAT)
            {
                EndLevel(gameStatus);
            }
        }
    }
}