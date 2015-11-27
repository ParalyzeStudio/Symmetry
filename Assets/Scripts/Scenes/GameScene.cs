using UnityEngine;
using System.Collections.Generic;

public class GameScene : GUIScene
{
    public const float INTERFACE_BUTTONS_Z_VALUE = -15.0f;
    public const float AXIS_CONSTRAINTS_ICONS_Z_VALUE = -15.0f;
    public const float COUNTER_Z_VALUE = -15.0f;

    public const float GRID_Z_VALUE = -200.0f;
    public const float CONTOURS_Z_VALUE = -200.0f;
    public const float SHAPES_Z_VALUE = -10.0f;
    public const float AXES_Z_VALUE = -210.0f;

    //shared prefabs
    public GameObject m_radialGradientPfb;
    public GameObject m_texQuadPfb;
    public GameObject m_blurrySegmentPfb;

    public Grid m_grid { get; set; }
    public ShapeVoxelGrid m_voxelGrid { get; set; }
    public Counter m_counter { get; set; }
    public Outlines m_outlines { get; set; }
    public Shapes m_shapesHolder { get; set; }
    public Axes m_axes { get; set; }

    //root objects
    public GameObject m_shapesHolderPfb;
    public GameObject m_gridPfb;
    public GameObject m_outlinesPfb;
    public GameObject m_counterPfb;
    public GameObject m_axesPfb;

    //holders
    private GameObject m_interfaceButtonsHolder;
    private GameObject m_axisConstraintsIconsHolder;

    //interface buttons
    public GameObject m_debugBlurrySegmentObjectPfb;
    public GameObject m_debugSimplifiedRoundedSegmentObjectPfb;
    public Material m_glowRectangleMaterial;
    public Material m_blurrySegmentMaterial;
    public Material m_sharpSegmentMaterial;

    //Action buttons
    private ActionButton m_topActionButton;
    private ActionButton m_middleActionButton;
    private ActionButton m_bottomActionButton;

    //Constraints on axes
    public const string CONSTRAINT_SYMMETRY_AXIS_HORIZONTAL = "SYMMETRY_AXIS_HORIZONTAL";
    public const string CONSTRAINT_SYMMETRY_AXIS_VERTICAL = "SYMMETRY_AXIS_VERTICAL";
    public const string CONSTRAINT_SYMMETRY_AXES_STRAIGHT = "SYMMETRY_AXES_STRAIGHT";
    public const string CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_LEFT = "SYMMETRY_AXIS_DIAGONAL_LEFT";
    public const string CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_RIGHT = "SYMMETRY_AXIS_DIAGONAL_RIGHT";
    public const string CONSTRAINT_SYMMETRY_AXES_DIAGONALS = "SYMMETRY_AXES_DIAGONALS";

    public List<Vector2> m_constrainedDirections { get; set; }

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

        //Gradient
        ApplyBackgroundGradient();

        //Show all elements
        //CallFuncHandler callFuncHandler = GetCallFuncHandler();
        //callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowElements), 0.5f);       
        ShowElements();

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
        //Display the grid
        ShowGrid();

        //Display interface buttons (pause, retry and hints)
        ShowInterfaceButtons();

        //Display action counter
        ShowCounter();

        //Show outlines (if applicable)
        ShowOutlines();

        //Show available symmetry axes
        BuildConstrainedDirections();
        ShowAxisConstraintsIcons();

        //Show action buttons
        ShowActionButtons();

        //Show starting shapes
        //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowInitialShapes), 1.0f);
        ShowInitialShapes();

        //Build Axes holder
        BuildAxesHolder();

        //add small debug button to skip the level
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        GameObject skipLevelButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_DEBUG_SKIP_LEVEL, new Vector2(128, 128));
        skipLevelButtonObject.name = "DebugSkipLevel";
        GameObjectAnimator skipLevelButtonAnimator = skipLevelButtonObject.GetComponent<GameObjectAnimator>();
        skipLevelButtonAnimator.SetParentTransform(this.transform);
        skipLevelButtonAnimator.SetPosition(new Vector3(0.5f * screenSize.x - 64.0f, 0, -10));

        m_gameStatus = GameStatus.RUNNING;

        //Set up the ClippingManager
        InitClippingManager();
    }

    /**
     * We build the grid of anchors that is displayed on the screen and that will help the player positionning shapes and axis...
     * Two anchors are separated from each other of a distance of m_gridSpacing that can be set in the editor
     * **/
    private void ShowGrid(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
               
        GameObject gridObject = (GameObject)Instantiate(m_gridPfb);
        gridObject.name = "Grid";

        GameObjectAnimator gridAnimator = gridObject.GetComponent<GameObjectAnimator>();
        gridAnimator.SetParentTransform(this.transform);
        gridAnimator.SetPosition(new Vector3(0, -0.075f * screenSize.y, GRID_Z_VALUE));        

        //Visible grid
        m_grid = gridObject.GetComponent<Grid>();
        m_grid.Build();        

        //Voxel grid       
        m_voxelGrid = gridObject.GetComponent<ShapeVoxelGrid>();
        m_voxelGrid.Init(8);

        gridAnimator.SetOpacity(0);
        gridAnimator.FadeTo(1, 1.5f, fDelay);
    }

    /**
     * Pause, retry and hints buttons
     * **/
    private void ShowInterfaceButtons(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");

        GameObjectAnimator interfaceButtonsHolderAnimator = m_interfaceButtonsHolder.AddComponent<GameObjectAnimator>();
        interfaceButtonsHolderAnimator.SetParentTransform(this.transform);
        interfaceButtonsHolderAnimator.SetPosition(new Vector3(0, 0, -10));

        //Build buttons contours
        Color blurrySegmentTintColor = ColorUtils.GetRGBAColorFromTSB(GetLevelManager().m_currentChapter.GetThemeTintValues()[1], 1);
        Color sharpSegmentTintColor = ColorUtils.LightenColor(blurrySegmentTintColor, 0.75f);
        blurrySegmentTintColor.a = 0.5f;

        GameObject buttonsContour = new GameObject("ButtonsContour");
        
        GameObjectAnimator buttonsContourAnimator = buttonsContour.AddComponent<GameObjectAnimator>();
        buttonsContourAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        buttonsContourAnimator.SetPosition(Vector3.zero);

        SegmentTree contourSegmentTree = m_interfaceButtonsHolder.AddComponent<SegmentTree>();
        contourSegmentTree.Init(SegmentTree.SegmentType.BLURRY,
                                buttonsContour,
                                m_blurrySegmentPfb, 
                                4.0f,
                                16.0f,
                                Instantiate(m_sharpSegmentMaterial),
                                Instantiate(m_blurrySegmentMaterial),
                                sharpSegmentTintColor,
                                blurrySegmentTintColor);

        float triangleHeight = GetBackgroundRenderer().m_triangleHeight;
        float triangleEdgeLength = GetBackgroundRenderer().m_triangleEdgeLength;

        SegmentTreeNode node1 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 6) * triangleHeight, 0.5f * screenSize.y));
        SegmentTreeNode node2 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 6) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
        SegmentTreeNode node3 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 5) * triangleHeight, 0.5f * screenSize.y - 2.0f * triangleEdgeLength));
        SegmentTreeNode node4 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 4) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
        SegmentTreeNode node5 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 4) * triangleHeight, 0.5f * screenSize.y));
        SegmentTreeNode node6 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 3) * triangleHeight, 0.5f * screenSize.y - 2.0f * triangleEdgeLength));
        SegmentTreeNode node7 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 2) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
        SegmentTreeNode node8 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 2) * triangleHeight, 0.5f * screenSize.y));
        SegmentTreeNode node9 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 1) * triangleHeight, 0.5f * screenSize.y - 2.0f * triangleEdgeLength));
        SegmentTreeNode node10 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2) * triangleHeight, 0.5f * screenSize.y - 1.5f * triangleEdgeLength));
        SegmentTreeNode node11 = new SegmentTreeNode(new Vector2((BackgroundTrianglesRenderer.NUM_COLUMNS / 2) * triangleHeight, 0.5f * screenSize.y));

        node1.SetAnimationStartNode(true);

        node1.AddChild(node2);
        node2.AddChild(node3);
        node3.AddChild(node4);
        node4.AddChild(node5);
        node4.AddChild(node6);
        node6.AddChild(node7);
        node7.AddChild(node8);
        node7.AddChild(node9);
        node9.AddChild(node10);
        node10.AddChild(node11);

        contourSegmentTree.m_nodes.Add(node1);
        contourSegmentTree.m_nodes.Add(node2);
        contourSegmentTree.m_nodes.Add(node3);
        contourSegmentTree.m_nodes.Add(node4);
        contourSegmentTree.m_nodes.Add(node5);
        contourSegmentTree.m_nodes.Add(node6);
        contourSegmentTree.m_nodes.Add(node7);
        contourSegmentTree.m_nodes.Add(node8);
        contourSegmentTree.m_nodes.Add(node9);
        contourSegmentTree.m_nodes.Add(node10);
        contourSegmentTree.m_nodes.Add(node11);

        contourSegmentTree.BuildSegments(true);

        //Add glowing rectangles above each button skin
        Vector2 glowRectSize = new Vector2(1.733f * triangleHeight, 0.867f * triangleHeight);
        for (int i = 0; i != 3; i++)
        {
            GameObject glowRectObject = (GameObject)Instantiate(m_texQuadPfb);
            glowRectObject.name = "GlowRectangle" + (i + 1);

            glowRectObject.GetComponent<UVQuad>().Init(m_glowRectangleMaterial);

            TexturedQuadAnimator glowRectAnimator = glowRectObject.GetComponent<TexturedQuadAnimator>();
            glowRectAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
            float rectPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - (2 * i + 1)) * triangleHeight;
            float rectPositionY = 0.5f * screenSize.y - 0.4f * triangleEdgeLength;
            glowRectAnimator.SetPosition(new Vector3(rectPositionX, rectPositionY, 0));
            glowRectAnimator.SetScale(glowRectSize);
            glowRectAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);
        }

        //Add button skins
        Vector2 interfaceButtonSize = new Vector2(1.733f * triangleHeight, 1.733f * triangleHeight);

        //pause
        GameObject pauseButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_MENU_BUTTON, interfaceButtonSize);
        pauseButtonObject.name = "PauseButton";

        GUIButton pauseButton = pauseButtonObject.GetComponent<GUIButton>();
        pauseButton.SetTouchArea(new Vector2(2 * triangleHeight, 3 * triangleEdgeLength));

        GameObjectAnimator pauseButtonAnimator = pauseButtonObject.GetComponent<GameObjectAnimator>();
        pauseButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float pauseButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 1) * triangleHeight;
        float pauseButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
        pauseButtonAnimator.SetPosition(new Vector3(pauseButtonPositionX, pauseButtonPositionY, 0));
        pauseButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);

        //retry
        GameObject retryButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_RETRY_BUTTON, interfaceButtonSize);
        retryButtonObject.name = "RetryButton";

        GUIButton retryButton = retryButtonObject.GetComponent<GUIButton>();
        retryButton.SetTouchArea(new Vector2(2 * triangleHeight, 3 * triangleEdgeLength));

        GameObjectAnimator retryButtonAnimator = retryButtonObject.GetComponent<GameObjectAnimator>();
        retryButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float retryButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 3) * triangleHeight - 4.0f; //offset a bit the texture, otherwise it gives a weird impression
        float retryButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
        retryButtonAnimator.SetPosition(new Vector3(retryButtonPositionX, retryButtonPositionY, 0));
        retryButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);

        //hints
        GameObject hintsButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_HINTS_BUTTON, interfaceButtonSize);
        hintsButtonObject.name = "HintsButton";

        GUIButton hintsButton = hintsButtonObject.GetComponent<GUIButton>();
        hintsButton.SetTouchArea(new Vector2(2 * triangleHeight, 3 * triangleEdgeLength));

        GameObjectAnimator hintsButtonAnimator = hintsButtonObject.GetComponent<GameObjectAnimator>();
        hintsButtonAnimator.SetParentTransform(m_interfaceButtonsHolder.transform);
        float hintsButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 5) * triangleHeight;
        float hintsButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
        hintsButtonAnimator.SetPosition(new Vector3(hintsButtonPositionX, hintsButtonPositionY, 0));
        hintsButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);
    }

    /**
     * Fades out interface buttons holder
     * **/
    public void DismissInterfaceButtons(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator interfaceButtonsHolderAnimator = m_interfaceButtonsHolder.GetComponent<GameObjectAnimator>();
        interfaceButtonsHolderAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Counters to show remaining actions for the player
     * **/
    private void ShowCounter(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject counterObject = (GameObject)Instantiate(m_counterPfb);
        counterObject.name = "Counter";
        m_counter = counterObject.GetComponent<Counter>();

        m_counter.Init();
        m_counter.Build();

        GameObjectAnimator counterAnimator = counterObject.GetComponent<GameObjectAnimator>();
        counterAnimator.SetParentTransform(this.transform);
        counterAnimator.SetOpacity(0);
        counterAnimator.FadeTo(1, 1.0f, fDelay);
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
    private void ShowOutlines(float fDelay = 0.0f)
    {
        GameObject outlinesObject = (GameObject)Instantiate(m_outlinesPfb);
        outlinesObject.name = "Outlines";        

        GameObjectAnimator outlinesAnimator = outlinesObject.GetComponent<GameObjectAnimator>();
        outlinesAnimator.SetParentTransform(this.transform);
        outlinesAnimator.SetOpacity(0);
        outlinesAnimator.SetPosition(new Vector3(0, 0, CONTOURS_Z_VALUE));

        m_outlines = outlinesObject.GetComponent<Outlines>();
        m_outlines.Build();
        m_outlines.Show(true, 1.5f, fDelay);
    }

    /**
     * Draw small icons to show the constraints on axes that the player can draw
     * **/
    private void ShowAxisConstraintsIcons(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        List<string> axisConstraints = GetLevelManager().m_currentLevel.m_axisConstraints;
        Color tintColor = GetLevelManager().m_currentChapter.GetThemeColors()[4];

        //Show icons
        m_axisConstraintsIconsHolder = new GameObject("AxisConstraintsIconsHolder");
        
        float iconHolderLeftMargin = 30.0f;
        float iconHolderTopMargin = 30.0f; 

        GameObjectAnimator axisConstraintsIconsHolderAnimator = m_axisConstraintsIconsHolder.AddComponent<GameObjectAnimator>();
        axisConstraintsIconsHolderAnimator.SetParentTransform(this.transform);
        axisConstraintsIconsHolderAnimator.SetPosition(new Vector3(iconHolderLeftMargin - 0.5f * screenSize.x, 0.5f * screenSize.y - iconHolderTopMargin, AXIS_CONSTRAINTS_ICONS_Z_VALUE));              

        float horizontalDistanceBetweenIcons = 65.0f;
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
            iconAnimator.SetColor(tintColor);
            float iconPositionX = 0.5f * iconSize.x + i * horizontalDistanceBetweenIcons;
            iconAnimator.SetPosition(new Vector3(iconPositionX, -0.5f * iconSize.y, 0));
        }
        
        axisConstraintsIconsHolderAnimator.SetOpacity(0);
        axisConstraintsIconsHolderAnimator.FadeTo(1.0f, 0.5f);

        //Show some underline
        GameObject underlineObject = (GameObject)Instantiate(m_blurrySegmentPfb);
        underlineObject.name = "Underline";

        BlurrySegmentAnimator underlineAnimator = underlineObject.GetComponent<BlurrySegmentAnimator>();
        underlineAnimator.SetParentTransform(m_axisConstraintsIconsHolder.transform);
        underlineAnimator.SetPosition(Vector3.zero);

        float underlineTopMargin = 20.0f;
        BlurrySegment underline = underlineObject.GetComponent<BlurrySegment>();
        Vector3 underlinePointA = new Vector3(-iconHolderLeftMargin, -iconSize.y - underlineTopMargin, 0);
        Vector3 underlinePointB = new Vector3(-iconHolderLeftMargin + axisConstraints.Count * iconSize.x + 2 * iconHolderLeftMargin, -iconSize.y - underlineTopMargin, 0);
        underline.Build(underlinePointA,
                        underlinePointA,
                        4,
                        16,
                        Instantiate(m_sharpSegmentMaterial),
                        Instantiate(m_blurrySegmentMaterial),
                        Color.white,
                        tintColor); //TODO obtain the color from the theme;

        underlineAnimator.TranslatePointBTo(underlinePointB, 0.5f);
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
    private void ShowActionButtons(float fDelay = 0.0f)
    {
        Vector2 actionButtonSkinSize = new Vector2(128, 128);

        //Show top button
        GUIButton.GUIButtonID[] childIDs = new GUIButton.GUIButtonID[4];
        childIDs[0] = GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE;
        childIDs[1] = GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES;
        childIDs[2] = GUIButton.GUIButtonID.ID_POINT_SYMMETRY;
        childIDs[3] = GUIButton.GUIButtonID.ID_MOVE_SHAPE;

        GameObject buttonObject = GetGUIManager().CreateActionButton(actionButtonSkinSize,
                                                                     ActionButton.Location.TOP,
                                                                     childIDs);

        buttonObject.name = "TopActionBtn";

        m_topActionButton = buttonObject.GetComponent<ActionButton>();

        GameObjectAnimator topActionButtonAnimator = m_topActionButton.GetComponent<GameObjectAnimator>();
        topActionButtonAnimator.SetParentTransform(this.transform);

        //Show middle button
        childIDs = new GUIButton.GUIButtonID[2];
        childIDs[0] = GUIButton.GUIButtonID.ID_OPERATION_ADD;
        childIDs[1] = GUIButton.GUIButtonID.ID_OPERATION_SUBSTRACT;

        buttonObject = GetGUIManager().CreateActionButton(actionButtonSkinSize,
                                                          ActionButton.Location.MIDDLE,
                                                          childIDs);

        buttonObject.name = "MiddleActionBtn";

        m_middleActionButton = buttonObject.GetComponent<ActionButton>();

        GameObjectAnimator middleActionButtonAnimator = m_middleActionButton.GetComponent<GameObjectAnimator>();
        middleActionButtonAnimator.SetParentTransform(this.transform);

        //Show bottom button
        childIDs = new GUIButton.GUIButtonID[1];
        childIDs[0] = GUIButton.GUIButtonID.ID_COLOR_FILTER;

        buttonObject = GetGUIManager().CreateActionButton(actionButtonSkinSize,
                                                          ActionButton.Location.BOTTOM,
                                                          childIDs);

        buttonObject.name = "BottomActionBtn";

        m_bottomActionButton = buttonObject.GetComponent<ActionButton>();

        GameObjectAnimator bottomActionButtonAnimator = m_bottomActionButton.GetComponent<GameObjectAnimator>();
        bottomActionButtonAnimator.SetParentTransform(this.transform);

        m_topActionButton.Show(fDelay);
        m_middleActionButton.Show(fDelay + 0.2f);
        m_bottomActionButton.Show(fDelay + 0.4f);
    }

    private void DismissActionButtons(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        m_topActionButton.Dismiss(fDuration, fDelay, bDestroyOnFinish);
        m_middleActionButton.Dismiss(fDuration, fDelay + 0.1f, bDestroyOnFinish);
        m_bottomActionButton.Dismiss(fDuration, fDelay + 0.2f, bDestroyOnFinish);
    }

    /**
     * Returns the current ID of the action button at specified location
     * **/
    public GUIButton.GUIButtonID GetActionButtonID(ActionButton.Location buttonLocation)
    {
        if (buttonLocation == ActionButton.Location.TOP)
            return m_topActionButton.m_ID;
        else if (buttonLocation == ActionButton.Location.MIDDLE)
            return m_middleActionButton.m_ID;
        else
            return m_bottomActionButton.m_ID;
    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void ShowInitialShapes()
    {
        GameObject shapesHolderObject = (GameObject)Instantiate(m_shapesHolderPfb);
        shapesHolderObject.name = "Shapes";

        m_shapesHolder = shapesHolderObject.GetComponent<Shapes>();

        GameObjectAnimator shapesAnimator = shapesHolderObject.GetComponent<GameObjectAnimator>();
        shapesAnimator.SetParentTransform(this.transform);
        shapesAnimator.SetPosition(new Vector3(0, 0, SHAPES_Z_VALUE));
        shapesAnimator.SetOpacity(0);
        shapesAnimator.FadeTo(Shapes.SHAPES_OPACITY, 1.0f);

        List<Shape> initialShapes = GetLevelManager().m_currentLevel.m_initialShapes;
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = new Shape(initialShapes[iShapeIndex]); //make a deep copy of the shape object stored in the level manager

            //First triangulate the shape and set the color of each triangle
            shape.Triangulate();
            shape.m_state = Shape.ShapeState.STATIC;

            m_shapesHolder.CreateShapeObjectFromData(shape, false);
        }
    }

    private void BuildAxesHolder()
    {
        GameObject axesHolderObject = (GameObject)Instantiate(m_axesPfb);
        m_axes = axesHolderObject.GetComponent<Axes>();
        GameObjectAnimator axesAnimator = axesHolderObject.GetComponent<GameObjectAnimator>();
        axesAnimator.SetParentTransform(this.transform);
        axesAnimator.SetPosition(new Vector3(0, 0, AXES_Z_VALUE));
    }

    private void InitClippingManager()
    {
        m_clippingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClippingManager>();
        m_clippingManager.Init();

        //make a dummy clipping to speed up the next clipping
        GetQueuedThreadedJobsManager().AddJob(new ThreadedJob(new ThreadedJob.ThreadFunction(MakeFirstDummyClippingOperation)));        
    }

    /**
     * To speed up the effective clipping operations we will do when drawing axes, we thread here a simple random clipping operation when the game starts
     * **/
    private void MakeFirstDummyClippingOperation()
    {
        Contour dummyContour = new Contour(4);
        dummyContour.Add(new GridPoint(0, 0));
        dummyContour.Add(new GridPoint(10, 0));
        dummyContour.Add(new GridPoint(0, 5));
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
        List<Shape> allShapes = m_shapesHolder.m_shapes;

        //First we check if every shape is static
        for (int iShapeIdx = 0; iShapeIdx != allShapes.Count; iShapeIdx++)
        {
            Shape shape = allShapes[iShapeIdx];

            if (shape.m_state != Shape.ShapeState.STATIC)
                return false;
        }

        Debug.Log("STEP1 CHECK");

        //Then check if every shape is fully inside one of the dotted outlines       
        List<DottedOutline> outlines = m_outlines.m_outlinesList;

        for (int iShapeIdx = 0; iShapeIdx != allShapes.Count; iShapeIdx++)
        {
            Shape shape = allShapes[iShapeIdx];

            if (shape.m_state != Shape.ShapeState.STATIC)
                return false;

            bool bInsideOneOutline = false; //check if the shape is inside one of the outlines or outside all outlines
            for (int iOutlineIdx = 0; iOutlineIdx != outlines.Count; iOutlineIdx++)
            {
                DottedOutline outline = outlines[iOutlineIdx];

                if (shape.IntersectsOutline(outline))
                    Debug.Log("IntersectsOutline");

                if (shape.IntersectsOutline(outline)) //strict intersection between the shape and one contour, shape cannot be fully inside the contour
                {
                    return false;
                }

                //Check if every point of every triangle in shape and its center is inside the outline
                if (!bInsideOneOutline)
                {
                    for (int iTriangleIdx = 0; iTriangleIdx != shape.m_triangles.Count; iTriangleIdx++)
                    {
                        GridTriangle triangle = shape.m_triangles[iTriangleIdx];
                        if (outline.ContainsPoint(triangle.m_points[0]) &&
                            outline.ContainsPoint(triangle.m_points[1]) &&
                            outline.ContainsPoint(triangle.m_points[2]) &&
                            outline.ContainsPoint(triangle.GetCenter()))
                        {
                            bInsideOneOutline = true;
                        }
                    }
                }
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
            allShapes[i].CalculateArea();
            shapesArea += allShapes[i].m_area;
        }

        for (int i = 0; i != outlines.Count; i++)
        {
            outlines[i].CalculateArea();
            outlinesArea += outlines[i].m_area;
        }

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
        return;
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