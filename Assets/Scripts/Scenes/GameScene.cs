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
    public GameObject m_texRoundedSegmentPfb;

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
    public GameObject m_interfaceButtonsHolder { get; set; }

    //interface buttons
    public Material m_glowRectangleMaterial;
    public Material m_glowSegmentMaterial;

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

    //is the current scene displayed entirely
    public bool m_isShown { get; set; }

    private ClippingManager m_clippingManager;

    public void Init()
    {
        m_isShown = false;
    }

    public override void Show()
    {
        base.Show();

        //Gradient
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
        //GameObject radialGradientBackground = Instantiate(m_radialGradientPfb);
        //radialGradientBackground.transform.parent = this.transform;
        //radialGradientBackground.transform.localPosition = new Vector3(0,0,-5.1f);
        //radialGradientBackground.transform.localScale = GeometryUtils.BuildVector3FromVector2(ScreenUtils.GetScreenSize(), 1);

        //GradientQuad gradientQuad = radialGradientBackground.GetComponent<GradientQuad>();
        //gradientQuad.InitRadial(ColorUtils.GetColorFromRGBAVector4(new Vector4(146,21,51,255)),
        //                        ColorUtils.GetColorFromRGBAVector4(new Vector4(64,12,26,255)),
        //                        500);
        
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

        m_isShown = true;
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
    }

    /**
     * We build the grid of anchors that is displayed on the screen and that will help the player positionning shapes and axis...
     * Two anchors are separated from each other of a distance of m_gridSpacing that can be set in the editor
     * **/
    private void ShowGrid(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Visible grid
        GameObject gridObject = (GameObject)Instantiate(m_gridPfb);
        gridObject.name = "Grid";
        gridObject.transform.parent = this.transform;
        m_grid = gridObject.GetComponent<Grid>();
        m_grid.Build();

        GameObjectAnimator gridAnimator = gridObject.GetComponent<GameObjectAnimator>();
        gridAnimator.SetOpacity(0);
        gridAnimator.FadeTo(1, 0.5f, fDelay);
        gridAnimator.SetPosition(new Vector3(0, -0.075f * screenSize.y, GRID_Z_VALUE));
        
        //Voxel grid       
        m_voxelGrid = gridObject.GetComponentInChildren<ShapeVoxelGrid>();
        m_voxelGrid.Init(8);        
    }

    /**
     * Pause, retry and hints buttons
     * **/
    private void ShowInterfaceButtons(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");
        m_interfaceButtonsHolder.transform.parent = this.gameObject.transform;
        m_interfaceButtonsHolder.transform.localPosition = new Vector3(0, 0, -10);

        //Build buttons contours
        Color contourTintColor = ColorUtils.GetRGBAColorFromTSB(GetLevelManager().m_currentChapter.GetThemeTintValues()[1], 1);

        GameObject buttonsContour = new GameObject("ButtonsContour");
        buttonsContour.transform.parent = m_interfaceButtonsHolder.transform;

        SegmentTree contourSegmentTree = m_interfaceButtonsHolder.AddComponent<SegmentTree>();
        contourSegmentTree.Init(m_interfaceButtonsHolder, m_texRoundedSegmentPfb, 16.0f, Instantiate(m_glowSegmentMaterial), contourTintColor);

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
            glowRectObject.transform.parent = m_interfaceButtonsHolder.transform;

            glowRectObject.GetComponent<UVQuad>().Init(m_glowRectangleMaterial);

            TexturedQuadAnimator glowRectAnimator = glowRectObject.GetComponent<TexturedQuadAnimator>();
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
        pauseButtonObject.transform.parent = m_interfaceButtonsHolder.transform;        

        GameObjectAnimator pauseButtonAnimator = pauseButtonObject.GetComponent<GameObjectAnimator>();
        float pauseButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 1) * triangleHeight;
        float pauseButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
        pauseButtonAnimator.SetPosition(new Vector3(pauseButtonPositionX, pauseButtonPositionY, 0));
        pauseButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);    

        //retry
        GameObject retryButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_RETRY_BUTTON, interfaceButtonSize);
        retryButtonObject.name = "RetryButton";
        retryButtonObject.transform.parent = m_interfaceButtonsHolder.transform;

        GameObjectAnimator retryButtonAnimator = retryButtonObject.GetComponent<GameObjectAnimator>();
        float retryButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 3) * triangleHeight - 4.0f; //offset a bit the texture, otherwise it gives a weird impression
        float retryButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
        retryButtonAnimator.SetPosition(new Vector3(retryButtonPositionX, retryButtonPositionY, 0));
        retryButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);  

        //hints
        GameObject hintsuttonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_HINTS_BUTTON, interfaceButtonSize);
        hintsuttonObject.name = "HintsButton";
        hintsuttonObject.transform.parent = m_interfaceButtonsHolder.transform;

        GameObjectAnimator hintsButtonAnimator = hintsuttonObject.GetComponent<GameObjectAnimator>();
        float hintsButtonPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 5) * triangleHeight;
        float hintsButtonPositionY = 0.5f * screenSize.y - 1.14f * triangleEdgeLength;
        hintsButtonAnimator.SetPosition(new Vector3(hintsButtonPositionX, hintsButtonPositionY, 0));
        hintsButtonAnimator.SetColorChannels(GetLevelManager().m_currentChapter.GetThemeTintValues()[2], ValueAnimator.ColorMode.TSB);
    }

    /**
     * Fades out interface buttons holder
     * **/
    public void DismissInterfaceButtons(float fDuration, float fDelay)
    {
        GameObjectAnimator interfaceButtonsHolderAnimator = m_interfaceButtonsHolder.GetComponent<GameObjectAnimator>();
        interfaceButtonsHolderAnimator.FadeTo(0.0f, fDuration, fDelay);
    }

    /**
     * Counters to show remaining actions for the player
     * **/
    private void ShowCounter(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject counterObject = (GameObject)Instantiate(m_counterPfb);
        counterObject.name = "Counter";
        counterObject.transform.parent = this.transform;
        m_counter = counterObject.GetComponent<Counter>();

        m_counter.Init();
        m_counter.Build();

        GameObjectAnimator counterAnimator = counterObject.GetComponent<GameObjectAnimator>();
        counterAnimator.SetOpacity(0);
        counterAnimator.FadeTo(1, 0.2f, fDelay);
        counterAnimator.SetPosition(new Vector3(0, 0.428f * screenSize.y, COUNTER_Z_VALUE));
    }

    /**
     * Here we build the dotted outlines (contour and holes) of the shape the player has to reproduce
     * **/
    private void ShowOutlines(float fDelay = 0.0f)
    {
        GameObject outlinesObject = (GameObject)Instantiate(m_outlinesPfb);
        outlinesObject.name = "Outlines";
        outlinesObject.transform.parent = this.transform;
        m_outlines = outlinesObject.GetComponent<Outlines>();
        m_outlines.Build();

        GameObjectAnimator outlinesAnimator = outlinesObject.GetComponent<GameObjectAnimator>();
        outlinesAnimator.SetOpacity(0);
        outlinesAnimator.SetPosition(new Vector3(0, 0, CONTOURS_Z_VALUE));
        m_outlines.Show(true, 0.5f, fDelay);
    }

    /**
     * Draw small icons to show the constraints on axes that the player can draw
     * **/
    private void ShowAxisConstraintsIcons(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject iconsHolder = new GameObject("AxisConstraintsIconsHolder");
        iconsHolder.transform.parent = this.transform;

        float iconHolderLeftMargin = 30.0f;
        float iconHolderTopMargin = 30.0f;

        GameObjectAnimator iconsHolderAnimator = iconsHolder.AddComponent<GameObjectAnimator>();
        iconsHolderAnimator.SetPosition(new Vector3(iconHolderLeftMargin - 0.5f * screenSize.x, 0.5f * screenSize.y - iconHolderTopMargin, AXIS_CONSTRAINTS_ICONS_Z_VALUE));

        List<string> axisConstraints = GetLevelManager().m_currentLevel.m_axisConstraints;

        float horizontalDistanceBetweenIcons = 65.0f;
        Vector2 iconSize = new Vector2(64, 64);

        for (int i = 0; i != axisConstraints.Count; i++)
        {
            GameObject iconObject = (GameObject)Instantiate(m_texQuadPfb);
            iconObject.name = "ConstraintAxisIcon";
            iconObject.transform.parent = iconsHolder.transform;

            UVQuad iconQuad = iconObject.GetComponent<UVQuad>();
            iconQuad.Init(GetMaterialForAxisConstraint(axisConstraints[i]));

            TexturedQuadAnimator iconAnimator = iconObject.GetComponent<TexturedQuadAnimator>();
            iconAnimator.SetScale(iconSize);
            iconAnimator.SetColor(Color.white);
            float iconPositionX = 0.5f * iconSize.x + i * horizontalDistanceBetweenIcons;
            iconAnimator.SetPosition(new Vector3(iconPositionX, -0.5f * iconSize.y, 0));
        }
    }

    /**
     * Build the 3 action buttons that appear on the left side of the screen
     * -First button allows player to switch between action modes (symmetry types or shape translation)
     * -Second button is for modifying the behavior of color symmetrization (addition or soustraction)
     * -Third button is for picking a color that will apply to the symmetry done by the user by filtering shapes that are not of that specific color
     * **/
    private void ShowActionButtons(float fDelay = 0.0f)
    {
        Vector2 actionButtonSkinSize = new Vector2(128,128);

        //Show top button
        GUIButton.GUIButtonID[] childIDs = new GUIButton.GUIButtonID[4];
        childIDs[0] = GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES;
        childIDs[1] = GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE;
        childIDs[2] = GUIButton.GUIButtonID.ID_POINT_SYMMETRY;
        childIDs[3] = GUIButton.GUIButtonID.ID_MOVE_SHAPE;

        GameObject buttonObject = GetGUIManager().CreateActionButton(actionButtonSkinSize,
                                                                     ActionButton.Location.TOP,
                                                                     childIDs);

        buttonObject.name = "TopActionBtn";
        buttonObject.transform.parent = this.transform;

        m_topActionButton = buttonObject.GetComponent<ActionButton>();

        //Show middle button
        childIDs = new GUIButton.GUIButtonID[2];
        childIDs[0] = GUIButton.GUIButtonID.ID_OPERATION_ADD;
        childIDs[1] = GUIButton.GUIButtonID.ID_OPERATION_SUBSTRACT;

        buttonObject = GetGUIManager().CreateActionButton(actionButtonSkinSize,
                                                          ActionButton.Location.MIDDLE,
                                                          childIDs);

        buttonObject.name = "MiddleActionBtn";
        buttonObject.transform.parent = this.transform;

        m_middleActionButton = buttonObject.GetComponent<ActionButton>();

        //Show bottom button
        childIDs = new GUIButton.GUIButtonID[1];
        childIDs[0] = GUIButton.GUIButtonID.ID_COLOR_FILTER;

        buttonObject = GetGUIManager().CreateActionButton(actionButtonSkinSize,
                                                          ActionButton.Location.BOTTOM,
                                                          childIDs);

        buttonObject.name = "BottomActionBtn";
        buttonObject.transform.parent = this.transform;

        m_bottomActionButton = buttonObject.GetComponent<ActionButton>();

        m_topActionButton.Show(fDelay);
        m_middleActionButton.Show(fDelay + 0.2f);
        m_bottomActionButton.Show(fDelay + 0.4f);
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
        shapesHolderObject.transform.parent = this.transform;
        m_shapesHolder = shapesHolderObject.GetComponent<Shapes>();

        GameObjectAnimator shapesAnimator = shapesHolderObject.GetComponent<GameObjectAnimator>();
        shapesAnimator.SetPosition(new Vector3(0, 0, SHAPES_Z_VALUE));

        List<Shape> initialShapes = GetLevelManager().m_currentLevel.m_initialShapes;
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = new Shape(initialShapes[iShapeIndex]); //make a deep copy of the shape object stored in the level manager
            shape.TogglePointMode();

            //First triangulate the shape and set the color of each triangle
            shape.Triangulate();
            shape.m_state = Shape.ShapeState.STATIC;

            m_shapesHolder.CreateShapeObjectFromData(shape, false);

            //ShapeAnimator shapeAnimator = shapeObject.GetComponent<ShapeAnimator>();
            //shapeAnimator.SetOpacity(Shapes.SHAPES_OPACITY);
        }

        //GameObjectAnimator shapesAnimator = m_shapes.gameObject.GetComponent<GameObjectAnimator>();
        //shapesAnimator.SetOpacity(0);
        //shapesAnimator.FadeTo(Shapes.SHAPES_OPACITY, 0.5f, fDelay);

        //m_voxelGrid.Refresh();

        ////fusion initial shapes
        //GameObject shapeObject = m_shapes.m_shapesObj[0];
        //Shape fusionShape = shapeObject.GetComponent<ShapeRenderer>().m_shape;
        //Shapes.PerformFusionOnShape(fusionShape);
    }

    private void BuildAxesHolder()
    {        
        GameObject axesHolderObject = (GameObject)Instantiate(m_axesPfb);
        axesHolderObject.transform.parent = this.transform;
        m_axes = axesHolderObject.GetComponent<Axes>();
        GameObjectAnimator axesAnimator = axesHolderObject.GetComponent<GameObjectAnimator>();
        axesAnimator.SetPosition(new Vector3(0, 0, AXES_Z_VALUE));
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
}