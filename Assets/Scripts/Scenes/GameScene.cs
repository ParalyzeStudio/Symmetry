using UnityEngine;
using System.Collections.Generic;

public class GameScene : GUIScene
{
    public const float ACTION_BUTTONS_Z_VALUE = -15.0f;
    public const float INTERFACE_BUTTONS_Z_VALUE = -15.0f;
    public const float AXIS_CONSTRAINTS_ICONS_Z_VALUE = -15.0f;
    public const float COUNTER_Z_VALUE = -15.0f;
    public const float CHALKBOARD_Z_VALUE = -10.0f;
    public const float TILED_BACKGROUND_RELATIVE_Z_VALUE = 5.0f;
    public const float GRID_Z_VALUE = -20.0f;
    public const float CONTOURS_Z_VALUE = -25.0f;
    public const float SHAPES_Z_VALUE = -21.0f;
    public const float AXES_Z_VALUE = -26.0f;
    public const float SYMMETRY_STACK_Z_VALUE = -20.0f;

    //shared prefabs
    public GameObject m_radialGradientPfb;
    public GameObject m_texQuadPfb;
    public GameObject m_colorQuadPfb;
    public GameObject m_blurrySegmentPfb;
    public GameObject m_textMeshPfb;
    public Material m_transpColorMaterial;

    public Grid m_grid { get; set; }
    public ShapeVoxelGrid m_voxelGrid { get; set; }
    public Counter m_counter { get; set; }
    public Outlines m_outlines { get; set; }
    public Shapes m_shapesHolder { get; set; }
    public Axes m_axes { get; set; }

    //plain white material
    private Material m_plainWhiteMaterial;

    //root objects
    public GameObject m_shapesHolderPfb;
    public GameObject m_gridPfb;
    public GameObject m_outlinesPfb;
    public GameObject m_counterPfb;
    public GameObject m_axesPfb;

    //chalkboard background
    public Material m_chalkboardMaterial;

    //holders
    private GameObject m_interfaceButtonsHolder;
    private GameObject m_axisConstraintsIconsHolder;

    //interface buttons
    public GameObject m_debugBlurrySegmentObjectPfb;
    public GameObject m_debugSimplifiedRoundedSegmentObjectPfb;
    public Material m_glowRectangleMaterial;
    public Material m_blurrySegmentMaterial;
    public Material m_sharpSegmentMaterial;

    //action buttons
    ActionButton[] m_actionButtons;

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
    public GameObject m_gameStackPfb;
    public GameStack m_gameStack { get; set; }

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

        Shape subjShape = new Shape();
        Contour subjShapeContour = new Contour(4);
        subjShapeContour.Add(new GridPoint(0, 0, true));
        subjShapeContour.Add(new GridPoint(4, 0, true));
        subjShapeContour.Add(new GridPoint(4, 4, true));
        subjShapeContour.Add(new GridPoint(0, 4, true));

        subjShape.m_contour = subjShapeContour;

        subjShape.m_color = Color.white;

        Shape clipShape = new Shape();
        Contour clipShapeContour = new Contour(4);
        clipShapeContour.Add(new GridPoint(2, 0, true));
        clipShapeContour.Add(new GridPoint(4, 2, true));
        clipShapeContour.Add(new GridPoint(2, 4, true));
        clipShapeContour.Add(new GridPoint(0, 2, true));

        List<Shape> resultShapes = GetClippingManager().ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference);
        Debug.Log("resultShapes.Count:" + resultShapes.Count);

        //shape1.Triangulate();
        //m_shapesHolder.CreateShapeObjectFromData(shape1, false);
        //shape1.CalculateArea();

        //Debug.Log("Diff area:" + (shape1.m_area - shape2.m_area));

        /**
         * TEST TRIANGLES INTERSECTIONS
         * **/
        //UnitTests.TestTrianglesIntersections();

        //Contour subjShapeContour1 = new Contour(4);
        //subjShapeContour1.Add(new Vector2(0, 0));
        //subjShapeContour1.Add(new Vector2(300, 0));
        //subjShapeContour1.Add(new Vector2(300, 300));
        //subjShapeContour1.Add(new Vector2(0, 300));
        //Contour clipShapeContour1 = new Contour(4);
        //clipShapeContour1.Add(new Vector2(200, 200));
        //clipShapeContour1.Add(new Vector2(500, 200));
        //clipShapeContour1.Add(new Vector2(500, 500));
        //clipShapeContour1.Add(new Vector2(200, 500));
        //Shape subjShape1 = new Shape(true, subjShapeContour1);
        //Shape clipShape1 = new Shape(true, clipShapeContour1);

        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //for (int i = 0; i != 100; i++)
        //{
        //m_shapesHolder.CreateShapeObjectFromData(subjShape1, true);
        //}
        //sw.Stop();
        //Debug.Log("cellRendering took " + sw.ElapsedMilliseconds + " ms");
        //sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //for (int i = 0; i != 100; i++)
        //{
        //m_shapesHolder.CreateShapeObjectFromData(subjShape1, false);
        //}
        //sw.Stop();
        //Debug.Log("normalRendering took " + sw.ElapsedMilliseconds + " ms");


        /**
         * TEST SEGMENTS INTERSECTION
         * **/

        //UnitTests.TestSegmentsIntersecion();

        /**
         * TEST SHAPE CLIPPING
         * **/

        //UnitTests.TestShapesClipping();


        /**
         * TEST OVERLAP TIME
         * **/
        //Contour contourShape1 = new Contour(4);
        //contourShape1.Add(new Vector2(0, 0));
        //contourShape1.Add(new Vector2(1, 0));
        //contourShape1.Add(new Vector2(1, 1));
        //contourShape1.Add(new Vector2(0, 1));

        //Shape shape1 = new Shape(true, contourShape1);
        //shape1.Triangulate();

        //Contour contourShape2 = new Contour(4);
        //contourShape2.Add(new Vector2(1, 1));
        //contourShape2.Add(new Vector2(1, 0));
        //contourShape2.Add(new Vector2(2, 0));
        //contourShape2.Add(new Vector2(2, 1));

        //Shape shape2 = new Shape(true, contourShape2);
        //shape2.Triangulate();

        //bool bOverlap = shape1.OverlapsShape(shape2);
        //Debug.Log("Overlaps:" + bOverlap);

        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        //sw.Start();
        //for (int i = 0; i != 100000; i++)
        //{
        //    shape1.OverlapsShape(shape2);
        //}
        //sw.Stop();

        //Debug.Log("1>>>elapsedTime(ms):" + sw.ElapsedMilliseconds);

        //sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //for (int i = 0; i != 100000; i++)
        //{
        //    shape1.OverlapsShape(shape2);
        //    ClippingBooleanOperations.ShapesOperation(shape1, shape2, ClipperLib.ClipType.ctIntersection);
        //}
        //sw.Stop();
        //Debug.Log("2>>>elapsedTime(ms):" + sw.ElapsedMilliseconds);
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
        //Show the chalkboard background
        ShowChalkboard();

        //build shapes holder and axes holder
        BuildAxesHolder();
        BuildShapesHolder();

        //Display the grid
        ShowGrid();

        //Display interface buttons (pause, retry and hints)
        ShowInterfaceButtons2();

        //Display action counter
        ShowCounter();

        //Show outlines (if applicable)
        ShowOutlines();

        //Show available symmetry axes
        BuildConstrainedDirections();
        ShowAxisConstraintsIcons2();

        //Show action buttons
        ShowActionButtons();

        //Show starting shapes
        //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowInitialShapes), 1.0f);
        ShowInitialShapes();

        //Show stack
        //ShowSymmetryStack();

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
        GameObject chalkboardObject = (GameObject)Instantiate(m_texQuadPfb);
        chalkboardObject.name = "Chalkboard";

        UVQuad chalkboard = chalkboardObject.GetComponent<UVQuad>();
        chalkboard.Init(m_chalkboardMaterial);

        TexturedQuadAnimator chalkboardAnimator = chalkboard.GetComponent<TexturedQuadAnimator>();
        chalkboardAnimator.SetParentTransform(this.transform);

        Vector2 screenSize = ScreenUtils.GetScreenSize();
        Vector2 chalkboardTextureSize = new Vector2(1920, 1110);
        float chalkboardTextureRatio = chalkboardTextureSize.y / chalkboardTextureSize.x;
        float chalkboardScreenHeight = 0.859f * screenSize.y;
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
        topContourAnimator.SetPosition(new Vector3(0, chalkboardPosition.y + 0.5f * chalkboardHeight, CHALKBOARD_Z_VALUE - 1));
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

        //max size of the grid (not he actual grid size)
        float leftBorderWidth = 242.0f;
        float rightBorderWidth = 168.0f;
        float topBorderHeight = 180.0f;
        float bottomBorderHeight = 80.0f;
        Vector2 maxGridSize = new Vector2(screenSize.x - leftBorderWidth - rightBorderWidth, screenSize.y - topBorderHeight - bottomBorderHeight);

        //Visible grid
        m_grid = gridObject.GetComponent<Grid>();
        m_grid.Build(maxGridSize, m_plainWhiteMaterial);

        //set correct position for grid
        float gridPositionX = 0.5f * (screenSize.x - maxGridSize.x) - rightBorderWidth;
        float gridPositionY = -0.5f * (screenSize.y - maxGridSize.y) + bottomBorderHeight;
        Vector3 gridPosition = new Vector3(gridPositionX, gridPositionY, GRID_Z_VALUE);
        gridAnimator.SetPosition(gridPosition);

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

        //Contour
        float contourThickness = 3.0f;

        GameObject topContourObject = Instantiate(m_colorQuadPfb);
        topContourObject.name = "TopGridContour";
        ColorQuad topContour = topContourObject.GetComponent<ColorQuad>();
        topContour.Init(m_plainWhiteMaterial);
        ColorQuadAnimator topContourAnimator = topContourObject.GetComponent<ColorQuadAnimator>();
        topContourAnimator.SetParentTransform(gridObject.transform);
        topContourAnimator.SetScale(new Vector3(m_grid.m_gridSize.x, contourThickness, 1));
        topContourAnimator.SetPosition(new Vector3(0, 0.5f * m_grid.m_gridSize.y, 0));
        topContourAnimator.SetColor(Color.white);

        GameObject leftContourObject = Instantiate(m_colorQuadPfb);
        leftContourObject.name = "LeftGridContour";
        ColorQuad leftContour = leftContourObject.GetComponent<ColorQuad>();
        leftContour.Init(m_plainWhiteMaterial);
        ColorQuadAnimator leftContourAnimator = leftContourObject.GetComponent<ColorQuadAnimator>();
        leftContourAnimator.SetParentTransform(gridObject.transform);
        leftContourAnimator.SetScale(new Vector3(contourThickness, m_grid.m_gridSize.y, 1));
        leftContourAnimator.SetPosition(new Vector3(-0.5f * m_grid.m_gridSize.x, 0, 0));
        leftContourAnimator.SetColor(Color.white);

        GameObject rightContourObject = Instantiate(m_colorQuadPfb);
        rightContourObject.name = "RightGridContour";
        ColorQuad rightContour = rightContourObject.GetComponent<ColorQuad>();
        rightContour.Init(m_plainWhiteMaterial);
        ColorQuadAnimator rightContourAnimator = rightContourObject.GetComponent<ColorQuadAnimator>();
        rightContourAnimator.SetParentTransform(gridObject.transform);
        rightContourAnimator.SetScale(new Vector3(contourThickness, m_grid.m_gridSize.y, 1));
        rightContourAnimator.SetPosition(new Vector3(0.5f * m_grid.m_gridSize.x, 0, 0));
        rightContourAnimator.SetColor(Color.white);

        GameObject bottomContourObject = Instantiate(m_colorQuadPfb);
        bottomContourObject.name = "BottomGridContour";
        ColorQuad bottomContour = bottomContourObject.GetComponent<ColorQuad>();
        bottomContour.Init(m_plainWhiteMaterial);
        ColorQuadAnimator bottomContourAnimator = bottomContourObject.GetComponent<ColorQuadAnimator>();
        bottomContourAnimator.SetParentTransform(gridObject.transform);
        bottomContourAnimator.SetScale(new Vector3(m_grid.m_maxGridSize.x, contourThickness, 1));
        bottomContourAnimator.SetPosition(new Vector3(0, -0.5f * m_grid.m_gridSize.y, 0));
        bottomContourAnimator.SetColor(Color.white);
    }

    /**
     * Pause, retry and hints buttons
     * **/
    private void ShowInterfaceButtons2()
    {
        float triangleHeight = GetBackgroundRenderer().m_triangleHeight;
        float triangleEdgeLength = GetBackgroundRenderer().m_triangleEdgeLength;
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");

        GameObjectAnimator interfaceButtonsHolderAnimator = m_interfaceButtonsHolder.AddComponent<GameObjectAnimator>();
        interfaceButtonsHolderAnimator.SetParentTransform(this.transform);
        interfaceButtonsHolderAnimator.SetPosition(new Vector3(0, 0, INTERFACE_BUTTONS_Z_VALUE));

        Vector2 interfaceButtonSize = new Vector2(128, 128);
        float rightBorderWidth = 168.0f; //the width of the column on the right of the grid holding retry and hints buttons

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

        GameObjectAnimator retryButtonAnimator = retryButtonObject.GetComponent<GameObjectAnimator>();
        retryButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float retryButtonPositionX = 0.5f * (screenSize.x - rightBorderWidth);
        Vector3 gridPosition = m_grid.transform.position;
        float retryButtonPositionY = gridPosition.y + 150;
        retryButtonAnimator.SetPosition(new Vector3(retryButtonPositionX, retryButtonPositionY, 0));
        retryButtonAnimator.SetColor(Color.white);

        GameObject retryTextObject = (GameObject)Instantiate(m_textMeshPfb);
        retryTextObject.name = "RetryText";

        TextMesh retryText = retryTextObject.GetComponent<TextMesh>();
        retryText.text = LanguageUtils.GetTranslationForTag("retry");

        TextMeshAnimator retryTextAnimator = retryText.GetComponent<TextMeshAnimator>();
        retryTextAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        retryTextAnimator.SetPosition(new Vector3(retryButtonPositionX + 2, retryButtonPositionY - 60, 0));
        retryTextAnimator.SetFontHeight(30);
        retryTextAnimator.SetColor(Color.white);

        //hints
        GameObject hintsButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_HINTS_BUTTON, interfaceButtonSize);
        hintsButtonObject.name = "HintsButton";

        GUIButton hintsButton = hintsButtonObject.GetComponent<GUIButton>();
        hintsButton.SetTouchArea(new Vector2(130, 130));

        GameObjectAnimator hintsButtonAnimator = hintsButtonObject.GetComponent<GameObjectAnimator>();
        hintsButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float hintsButtonPositionX = 0.5f * screenSize.x - 84;
        float hintsButtonPositionY = gridPosition.y - 150;
        hintsButtonAnimator.SetPosition(new Vector3(hintsButtonPositionX, hintsButtonPositionY, 0));
        hintsButtonAnimator.SetColor(Color.white);

        GameObject hintsTextObject = (GameObject)Instantiate(m_textMeshPfb);
        hintsTextObject.name = "HintsText";

        TextMesh hintsText = hintsTextObject.GetComponent<TextMesh>();
        hintsText.text = LanguageUtils.GetTranslationForTag("hints");

        TextMeshAnimator hintsTextAnimator = hintsText.GetComponent<TextMeshAnimator>();
        hintsTextAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        hintsTextAnimator.SetPosition(new Vector3(hintsButtonPositionX, hintsButtonPositionY - 60, 0));
        hintsTextAnimator.SetFontHeight(30);
        hintsTextAnimator.SetColor(Color.white);

        //build a vertical line to show separation between interface buttons and grid
        GameObject lineObject = Instantiate(m_colorQuadPfb);
        lineObject.name = "SeparationLine";
        ColorQuad line = lineObject.GetComponent<ColorQuad>();
        line.Init(m_plainWhiteMaterial);
        ColorQuadAnimator lineAnimator = lineObject.GetComponent<ColorQuadAnimator>();
        lineAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        lineAnimator.SetScale(new Vector3(6.0f, m_grid.m_maxGridSize.y, 1));
        lineAnimator.SetPosition(new Vector3(0.5f * screenSize.x - rightBorderWidth, gridPosition.y, 0));
        lineAnimator.SetColor(Color.white);
    }

    /**
     * Pause, retry and hints buttons
     * **/
    //private void ShowInterfaceButtons(float fDelay = 0.0f)
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    m_interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");

    //    GameObjectAnimator interfaceButtonsHolderAnimator = m_interfaceButtonsHolder.AddComponent<GameObjectAnimator>();
    //    interfaceButtonsHolderAnimator.SetParentTransform(this.transform);
    //    interfaceButtonsHolderAnimator.SetPosition(new Vector3(0, 0, -10));

    //    //Build buttons contours
    //    Color blurrySegmentTintColor = ColorUtils.GetRGBAColorFromTSB(GetLevelManager().m_currentChapter.GetThemeTintValues()[1], 1);
    //    Color sharpSegmentTintColor = ColorUtils.LightenColor(blurrySegmentTintColor, 0.75f);
    //    blurrySegmentTintColor.a = 0.5f;

    //    GameObject buttonsContour = new GameObject("ButtonsContour");
        
    //    GameObjectAnimator buttonsContourAnimator = buttonsContour.AddComponent<GameObjectAnimator>();
    //    buttonsContourAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
    //    buttonsContourAnimator.SetPosition(Vector3.zero);

    //    SegmentTree contourSegmentTree = m_interfaceButtonsHolder.AddComponent<SegmentTree>();
    //    contourSegmentTree.Init(SegmentTree.SegmentType.BLURRY,
    //                            buttonsContour,
    //                            m_blurrySegmentPfb, 
    //                            4.0f,
    //                            16.0f,
    //                            Instantiate(m_sharpSegmentMaterial),
    //                            Instantiate(m_blurrySegmentMaterial),
    //                            sharpSegmentTintColor,
    //                            blurrySegmentTintColor);

    //    float triangleHeight = GetBackgroundRenderer().m_triangleHeight;
    //    float triangleEdgeLength = GetBackgroundRenderer().m_triangleEdgeLength;

    //    SegmentTreeNode node1 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 6) * triangleHeight, 0.5f * screenSize.y));
    //    SegmentTreeNode node2 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 6) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
    //    SegmentTreeNode node3 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 5) * triangleHeight, 0.5f * screenSize.y - 2.0f * triangleEdgeLength));
    //    SegmentTreeNode node4 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 4) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
    //    SegmentTreeNode node5 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 4) * triangleHeight, 0.5f * screenSize.y));
    //    SegmentTreeNode node6 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 3) * triangleHeight, 0.5f * screenSize.y - 2.0f * triangleEdgeLength));
    //    SegmentTreeNode node7 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 2) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
    //    SegmentTreeNode node8 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 2) * triangleHeight, 0.5f * screenSize.y));
    //    SegmentTreeNode node9 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 1) * triangleHeight, 0.5f * screenSize.y - 2.0f * triangleEdgeLength));
    //    SegmentTreeNode node10 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
    //    SegmentTreeNode node11 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2) * triangleHeight, 0.5f * screenSize.y));

    //    node1.SetAnimationStartNode(true);

    //    node1.AddChild(node2);
    //    node2.AddChild(node3);
    //    node3.AddChild(node4);
    //    node4.AddChild(node5);
    //    node4.AddChild(node6);
    //    node6.AddChild(node7);
    //    node7.AddChild(node8);
    //    node7.AddChild(node9);
    //    node9.AddChild(node10);
    //    node10.AddChild(node11);

    //    contourSegmentTree.m_nodes.Add(node1);
    //    contourSegmentTree.m_nodes.Add(node2);
    //    contourSegmentTree.m_nodes.Add(node3);
    //    contourSegmentTree.m_nodes.Add(node4);
    //    contourSegmentTree.m_nodes.Add(node5);
    //    contourSegmentTree.m_nodes.Add(node6);
    //    contourSegmentTree.m_nodes.Add(node7);
    //    contourSegmentTree.m_nodes.Add(node8);
    //    contourSegmentTree.m_nodes.Add(node9);
    //    contourSegmentTree.m_nodes.Add(node10);
    //    contourSegmentTree.m_nodes.Add(node11);

    //    contourSegmentTree.BuildSegments(true);

    //    //Add glowing rectangles above each button skin
    //    Vector2 glowRectSize = new Vector2(1.733f * triangleHeight, 0.867f * triangleHeight);
    //    for (int i = 0; i != 3; i++)
    //    {
    //        GameObject glowRectObject = (GameObject)Instantiate(m_texQuadPfb);
    //        glowRectObject.name = "GlowRectangle" + (i + 1);

    //        glowRectObject.GetComponent<UVQuad>().Init(m_glowRectangleMaterial);

    //        TexturedQuadAnimator glowRectAnimator = glowRectObject.GetComponent<TexturedQuadAnimator>();
    //        glowRectAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
    //        float rectPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - (2 * i + 1)) * triangleHeight;
    //        float rectPositionY = 0.5f * screenSize.y - 0.4f * triangleEdgeLength;
    //        glowRectAnimator.SetPosition(new Vector3(rectPositionX, rectPositionY, 0));
    //        glowRectAnimator.SetScale(glowRectSize);
    //        glowRectAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);
    //    }

    //    //Add button skins
    //    Vector2 interfaceButtonSize = new Vector2(1.733f * triangleHeight, 1.733f * triangleHeight);

    //    //pause
    //    GameObject pauseButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_PAUSE_BUTTON, interfaceButtonSize);
    //    pauseButtonObject.name = "PauseButton";

    //    GUIButton pauseButton = pauseButtonObject.GetComponent<GUIButton>();
    //    pauseButton.SetTouchArea(new Vector2(2 * triangleHeight, 3 * triangleEdgeLength));

    //    GameObjectAnimator pauseButtonAnimator = pauseButtonObject.GetComponent<GameObjectAnimator>();
    //    pauseButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
    //    float pauseButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 1) * triangleHeight;
    //    float pauseButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
    //    pauseButtonAnimator.SetPosition(new Vector3(pauseButtonPositionX, pauseButtonPositionY, 0));
    //    pauseButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);

    //    //retry
    //    GameObject retryButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_RETRY_BUTTON, interfaceButtonSize);
    //    retryButtonObject.name = "RetryButton";

    //    GUIButton retryButton = retryButtonObject.GetComponent<GUIButton>();
    //    retryButton.SetTouchArea(new Vector2(2 * triangleHeight, 3 * triangleEdgeLength));

    //    GameObjectAnimator retryButtonAnimator = retryButtonObject.GetComponent<GameObjectAnimator>();
    //    retryButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
    //    float retryButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 3) * triangleHeight - 4.0f; //offset a bit the texture, otherwise it gives a weird impression
    //    float retryButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
    //    retryButtonAnimator.SetPosition(new Vector3(retryButtonPositionX, retryButtonPositionY, 0));
    //    retryButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);

    //    //hints
    //    GameObject hintsButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_HINTS_BUTTON, interfaceButtonSize);
    //    hintsButtonObject.name = "HintsButton";

    //    GUIButton hintsButton = hintsButtonObject.GetComponent<GUIButton>();
    //    hintsButton.SetTouchArea(new Vector2(2 * triangleHeight, 3 * triangleEdgeLength));

    //    GameObjectAnimator hintsButtonAnimator = hintsButtonObject.GetComponent<GameObjectAnimator>();
    //    hintsButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
    //    float hintsButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 5) * triangleHeight;
    //    float hintsButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
    //    hintsButtonAnimator.SetPosition(new Vector3(hintsButtonPositionX, hintsButtonPositionY, 0));
    //    hintsButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);
    //}

    /**
     * Fades out interface buttons holder
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
     * Here we build the dotted outlines (contour and holes) of the shape the player has to reproduce
     * **/
    private void ShowOutlines()
    {
        GameObject outlinesObject = (GameObject)Instantiate(m_outlinesPfb);
        outlinesObject.name = "Outlines";        

        GameObjectAnimator outlinesAnimator = outlinesObject.GetComponent<GameObjectAnimator>();
        outlinesAnimator.SetParentTransform(this.transform);
        outlinesAnimator.SetPosition(new Vector3(0, 0, CONTOURS_Z_VALUE));

        m_outlines = outlinesObject.GetComponent<Outlines>();
        m_outlines.Build();
        m_outlines.Show(false);
    }

    /**
     * Draw small icons to show the constraints on axes that the player can draw
     * **/
    private void ShowAxisConstraintsIcons2()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        List<string> axisConstraints = GetLevelManager().m_currentLevel.m_axisConstraints;
        Color tintColor = GetLevelManager().m_currentChapter.GetThemeColors()[4];
        float bottomBorderHeight = 80.0f;

        //Show icons
        m_axisConstraintsIconsHolder = new GameObject("AxisConstraintsIconsHolder");

        GameObjectAnimator axisConstraintsIconsHolderAnimator = m_axisConstraintsIconsHolder.AddComponent<GameObjectAnimator>();
        axisConstraintsIconsHolderAnimator.SetParentTransform(this.transform);
        axisConstraintsIconsHolderAnimator.SetPosition(new Vector3(0, -0.5f * screenSize.y + 0.5f * bottomBorderHeight, AXIS_CONSTRAINTS_ICONS_Z_VALUE));

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
        lineAnimator.SetPosition(new Vector3(0, 0.5f * bottomBorderHeight, 0));
        lineAnimator.SetColor(Color.white);
    }

    /**
     * Draw small icons to show the constraints on axes that the player can draw
    * **/
    //private void ShowAxisConstraintsIcons(float fDelay = 0.0f)
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();
    //    List<string> axisConstraints = GetLevelManager().m_currentLevel.m_axisConstraints;
    //    Color tintColor = GetLevelManager().m_currentChapter.GetThemeColors()[4];

    //    //Show icons
    //    m_axisConstraintsIconsHolder = new GameObject("AxisConstraintsIconsHolder");
        
    //    float iconHolderLeftMargin = 30.0f;
    //    float iconHolderTopMargin = 30.0f; 

    //    GameObjectAnimator axisConstraintsIconsHolderAnimator = m_axisConstraintsIconsHolder.AddComponent<GameObjectAnimator>();
    //    axisConstraintsIconsHolderAnimator.SetParentTransform(this.transform);
    //    axisConstraintsIconsHolderAnimator.SetPosition(new Vector3(iconHolderLeftMargin - 0.5f * screenSize.x, 0.5f * screenSize.y - iconHolderTopMargin, AXIS_CONSTRAINTS_ICONS_Z_VALUE));              

    //    float horizontalDistanceBetweenIcons = 65.0f;
    //    Vector2 iconSize = new Vector2(64, 64);

    //    for (int i = 0; i != axisConstraints.Count; i++)
    //    {
    //        GameObject iconObject = (GameObject)Instantiate(m_texQuadPfb);
    //        iconObject.name = "ConstraintAxisIcon";

    //        UVQuad iconQuad = iconObject.GetComponent<UVQuad>();
    //        iconQuad.Init(GetMaterialForAxisConstraint(axisConstraints[i]));

    //        TexturedQuadAnimator iconAnimator = iconObject.GetComponent<TexturedQuadAnimator>();
    //        iconAnimator.SetParentTransform(m_axisConstraintsIconsHolder.transform);
    //        iconAnimator.SetScale(iconSize);
    //        iconAnimator.SetColor(tintColor);
    //        float iconPositionX = 0.5f * iconSize.x + i * horizontalDistanceBetweenIcons;
    //        iconAnimator.SetPosition(new Vector3(iconPositionX, -0.5f * iconSize.y, 0));
    //    }
        
    //    axisConstraintsIconsHolderAnimator.SetOpacity(0);
    //    axisConstraintsIconsHolderAnimator.FadeTo(1.0f, 0.5f);

    //    //Show some underline
    //    GameObject underlineObject = (GameObject)Instantiate(m_blurrySegmentPfb);
    //    underlineObject.name = "Underline";

    //    BlurrySegmentAnimator underlineAnimator = underlineObject.GetComponent<BlurrySegmentAnimator>();
    //    underlineAnimator.SetParentTransform(m_axisConstraintsIconsHolder.transform);
    //    underlineAnimator.SetPosition(Vector3.zero);

    //    float underlineTopMargin = 20.0f;
    //    BlurrySegment underline = underlineObject.GetComponent<BlurrySegment>();
    //    Vector3 underlinePointA = new Vector3(-iconHolderLeftMargin, -iconSize.y - underlineTopMargin, 0);
    //    Vector3 underlinePointB = new Vector3(-iconHolderLeftMargin + axisConstraints.Count * iconSize.x + 2 * iconHolderLeftMargin, -iconSize.y - underlineTopMargin, 0);
    //    underline.Build(underlinePointA,
    //                    underlinePointA,
    //                    4,
    //                    16,
    //                    Instantiate(m_sharpSegmentMaterial),
    //                    Instantiate(m_blurrySegmentMaterial),
    //                    Color.white,
    //                    tintColor); //TODO obtain the color from the theme;

    //    underlineAnimator.TranslatePointBTo(underlinePointB, 0.5f);
    //}

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

        Vector2 actionButtonsHolderSize = new Vector2(242.0f, m_grid.m_maxGridSize.y);
        GameObject actionButtonsHolder = new GameObject("ActionButtons");
        GameObjectAnimator actionButtonsHolderAnimator = actionButtonsHolder.AddComponent<GameObjectAnimator>();
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
                                                                     childIDs);
        buttonObject.name = "TopActionBtn";

        ActionButton button = buttonObject.GetComponent<ActionButton>();
        m_actionButtons[0] = button;

        GameObjectAnimator buttonAnimator = buttonObject.GetComponent<GameObjectAnimator>();
        buttonAnimator.SetParentTransform(actionButtonsHolder.transform);
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
        lineAnimator.SetParentTransform(actionButtonsHolder.transform);
        lineAnimator.SetScale(new Vector3(6.0f, m_grid.m_maxGridSize.y, 1));
        lineAnimator.SetPosition(new Vector3(0.5f * actionButtonsHolderSize.x, 0, 0));
        lineAnimator.SetColor(Color.white);
    }

    private void DismissActionButtons(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        for (int i = 0; i != m_actionButtons.Length; i++)
        {
            if (m_actionButtons[i] != null)
                m_actionButtons[i].Dismiss(fDuration, fDelay, bDestroyOnFinish);
        }
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
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = new Shape(initialShapes[iShapeIndex]); //make a deep copy of the shape object stored in the level manager

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

    private void BuildAxesHolder()
    {
        GameObject axesHolderObject = (GameObject)Instantiate(m_axesPfb);
        m_axes = axesHolderObject.GetComponent<Axes>();
        GameObjectAnimator axesAnimator = axesHolderObject.GetComponent<GameObjectAnimator>();
        axesAnimator.SetParentTransform(this.transform);
        axesAnimator.SetPosition(new Vector3(0, 0, AXES_Z_VALUE));
    }

    /**
     * If symmetries are stackable in this level, show the stack on the right side of the screen
     * **/
    private void ShowSymmetryStack()
    {
        if (GetLevelManager().m_currentLevel.m_symmetriesStackable)
        {
            Vector2 screenSize = ScreenUtils.GetScreenSize();

            GameObject gameStackObject = (GameObject)Instantiate(m_gameStackPfb);
            gameStackObject.name = "SymmetryStack";

            GameObjectAnimator gameStackAnimator = gameStackObject.GetComponent<GameObjectAnimator>();
            gameStackAnimator.SetParentTransform(this.transform);
            gameStackAnimator.SetPosition(new Vector3(0.5f * screenSize.x - 96.0f, 256.0f, SYMMETRY_STACK_Z_VALUE));
            gameStackAnimator.SetOpacity(0);
            gameStackAnimator.FadeTo(1.0f, 1.0f);

            m_gameStack = gameStackObject.GetComponent<GameStack>();
            m_gameStack.Build();
        }
    }

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

        List<Shape> result = m_clippingManager.ShapesOperation(dummySubjShape, dummyClipShape, ClipperLib.ClipType.ctUnion, true);
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

        Debug.Log("STEP1 CHECK");

        //Then check if every shape is fully inside one of the dotted outlines       
        List<DottedOutline> outlines = m_outlines.m_outlinesList;

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

        Debug.Log("STEP2 CHECK");

        //Finally, check if the sum of shapes areas is equal to the sum of outlines areas
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
            outlines[i].CalculateArea();
            outlinesArea += outlines[i].m_area;
        }

        Debug.Log("shapesArea:" + shapesArea + " outlinesArea:" + outlinesArea);
        if (MathUtils.AreFloatsEqual(shapesArea, outlinesArea, 1))
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

        if (gameStatus == GameStatus.VICTORY)
        {
            Debug.Log("EndLevel VICTORY");
            GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(DismissSceneElementsOnVictory), 2.0f);

            LevelManager levelManager = GetLevelManager();
            if (levelManager.IsCurrentLevelDebugLevel() > 0)
            {
                GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(OnFinishEndingLevelVictory), 5.0f);
            }
            else
            {
                int currentLevelNumber = levelManager.m_currentLevel.m_chapterRelativeNumber;
                if (currentLevelNumber < LevelManager.LEVELS_PER_CHAPTER - 1)
                {
                    levelManager.SetLevelOnCurrentChapter(levelManager.m_currentLevel.m_chapterRelativeNumber + 1);
                    GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(OnFinishEndingLevelVictory), 5.0f);
                }
                else
                {
                    //TODO go to next chapter by showing chapter menu for instance
                }
                //Save the status of this level to preferences
                GetPersistentDataManager().SetLevelDone(currentLevelNumber);
            }
        }
        else if (gameStatus == GameStatus.DEFEAT)
        {
            Debug.Log("EndLevel DEFEAT");
            //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false, 0.0f, 0.5f, 0.5f); //restart the level
        }
    }

    /**
     * Fade out and remove every scene element except shapes holder when player completed a level
     * **/
    private void DismissSceneElementsOnVictory()
    {
        m_grid.Dismiss(1.0f);
        m_outlines.Dismiss(1.0f);
        m_axes.Dismiss(1.0f);
        DismissInterfaceButtons(0.5f);
        DismissActionButtons(1.0f);
        DismissAxisConstraintsIcons(0.5f);
        DismissCounter(0.5f);
    }

    /**
     * Function called when all scene elements have been faded out except shapes (victory)
     * Time to switch scene and go to next level or next chapter (if level 16 has been reached)
     * **/
    private void OnFinishEndingLevelVictory()
    {
        m_shapesHolder.GetComponent<GameObjectAnimator>().FadeTo(0.0f, 0.5f, 0.0f);
        GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(StartLevelIntroScene), 0.5f);
    }

    private void StartLevelIntroScene()
    {
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false); //restart the level
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