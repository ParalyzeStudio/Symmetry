using UnityEngine;
using System.Collections.Generic;

public class GameScene : GUIScene
{
    public const float INTERFACE_BUTTONS_Z_VALUE = -15.0f;
    public const float AXIS_CONSTRAINTS_ICONS_Z_VALUE = -15.0f;
    public const float COUNTER_Z_VALUE = -15.0f;

    public const float GRID_Z_VALUE = -10.0f;
    public const float CONTOURS_Z_VALUE = -200.0f;
    public const float SHAPES_Z_VALUE = -20.0f;
    public const float AXES_Z_VALUE = -210.0f;

    //shared prefabs
    public GameObject m_radialGradientPfb;
    public GameObject m_texQuadPfb;
    public GameObject m_texRoundedSegmentPfb;

    public Grid m_grid { get; set; }
    public ShapeVoxelGrid m_voxelGrid { get; set; }
    public Counter m_counter { get; set; }
    public Outlines m_outlines { get; set; }
    public Shapes m_shapes { get; set; }
    public Axes m_axes { get; set; }

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

    public void Init()
    {
        m_isShown = false;
    }

    public override void Show()
    {
        base.Show();
        GameObjectAnimator sceneAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        sceneAnimator.SetOpacity(1.0f);

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
        //CallFuncHandler callFuncHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<CallFuncHandler>();
        //callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowInitialShapes), 0.5f);
        ShowInitialShapes();

        //Build Axes holder
        m_axes = this.gameObject.GetComponentInChildren<Axes>();
        m_axes.transform.localPosition = new Vector3(0, 0, AXES_Z_VALUE);

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

        m_isShown = true;
    }

    /**
     * We build the grid of anchors that is displayed on the screen and that will help the player positionning shapes and axis...
     * Two anchors are separated from each other of a distance of m_gridSpacing that can be set in the editor
     * **/
    private void ShowGrid(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Visible grid
        m_grid = this.gameObject.GetComponentInChildren<Grid>();
        m_grid.gameObject.transform.parent = this.gameObject.transform;
        m_grid.Build();

        GameObjectAnimator gridAnimator = m_grid.gameObject.GetComponent<GameObjectAnimator>();
        gridAnimator.SetOpacity(0);
        gridAnimator.FadeTo(1, 0.5f, fDelay);
        gridAnimator.SetPosition(new Vector3(0, -0.075f * screenSize.y, GRID_Z_VALUE));

        //Voxel grid
        m_voxelGrid = this.gameObject.GetComponentInChildren<ShapeVoxelGrid>();
        m_voxelGrid.Init(4);

        ///*** DEBUG TMP ***/
        //Grid grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        ////List<GameObject> anchors = grid.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXIS_VERTICAL);
        ////List<GameObject> anchors = grid.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXIS_HORIZONTAL);
        ////List<GameObject> anchors = grid.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXES_STRAIGHT);
        //List<GameObject> anchors = grid.GetAnchorsConstrainedBySymmetryType(new Vector2(19, 1), Symmetrizer.SymmetryType.SYMMETRY_AXES_ALL);
        //foreach (GameObject anchor in anchors)
        //{
        //    Vector2 gridPos = grid.GetGridCoordinatesFromWorldCoordinates(anchor.transform.position);
        //    Vector3 selectedAnchorPosition = GeometryUtils.BuildVector3FromVector2(anchor.transform.position, -10);
        //    Instantiate(m_gridAnchorSelectedPfb, selectedAnchorPosition, Quaternion.identity);
        //}
        ///*** DEBUG TMP ***/
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

        m_counter = this.gameObject.GetComponentInChildren<Counter>();
        m_counter.gameObject.transform.parent = this.gameObject.transform;
        m_counter.gameObject.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, COUNTER_Z_VALUE);

        m_counter.Init();
        m_counter.Build();

        GameObjectAnimator counterAnimator = m_counter.gameObject.GetComponent<GameObjectAnimator>();
        counterAnimator.SetOpacity(0);
        counterAnimator.FadeTo(1, 0.2f, fDelay);
    }

    /**
     * Here we build the dotted outlines (contour and holes) of the shape the player has to reproduce
     * **/
    private void ShowOutlines(float fDelay = 0.0f)
    {
        m_outlines = this.gameObject.GetComponentInChildren<Outlines>();
        m_outlines.transform.parent = this.gameObject.transform;
        m_outlines.transform.localPosition = new Vector3(0, 0, CONTOURS_Z_VALUE);
        m_outlines.Build();

        GameObjectAnimator outlinesAnimator = m_outlines.gameObject.GetComponent<GameObjectAnimator>();
        outlinesAnimator.SetOpacity(0);
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
        m_shapes = this.gameObject.GetComponentInChildren<Shapes>();
        m_shapes.gameObject.transform.parent = this.gameObject.transform;
        m_shapes.gameObject.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        List<Shape> initialShapes = GetLevelManager().m_currentLevel.m_initialShapes;
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = new Shape(initialShapes[iShapeIndex]); //make a deep copy of the shape object stored in the level manager
            shape.TogglePointMode();

            //First triangulate the shape and set the color of each triangle
            shape.Triangulate();

            GameObject shapeObject = m_shapes.CreateShapeObjectFromData(shape);

            ShapeAnimator shapeAnimator = shapeObject.GetComponent<ShapeAnimator>();
            shapeAnimator.SetOpacity(Shapes.SHAPES_OPACITY);
        }

        m_shapes.gameObject.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        //GameObjectAnimator shapesAnimator = m_shapes.gameObject.GetComponent<GameObjectAnimator>();
        //shapesAnimator.SetOpacity(0);
        //shapesAnimator.FadeTo(Shapes.SHAPES_OPACITY, 0.5f, fDelay);

        //m_voxelGrid.Refresh();

        ////fusion initial shapes
        //GameObject shapeObject = m_shapes.m_shapesObj[0];
        //Shape fusionShape = shapeObject.GetComponent<ShapeRenderer>().m_shape;
        //Shapes.PerformFusionOnShape(fusionShape);
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
}