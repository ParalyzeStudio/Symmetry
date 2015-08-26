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

    public Grid m_grid { get; set; }
    public Counter m_counter { get; set; }
    public Outlines m_outlines { get; set; }
    public Shapes m_shapes { get; set; }
    public Axes m_axes { get; set; }

    //holders
    public GameObject m_interfaceButtonsHolder { get; set; }

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

    public override void Init()
    {
        base.Init();
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

        ShowInitialShapes();
        m_axes = this.gameObject.GetComponentInChildren<Axes>();
        m_axes.transform.localPosition = new Vector3(0, 0, AXES_Z_VALUE);

        GameObject radialGradientBackground = Instantiate(m_radialGradientPfb);
        radialGradientBackground.transform.parent = this.transform;
        radialGradientBackground.transform.localPosition = new Vector3(0,0,-5.1f);
        radialGradientBackground.transform.localScale = GeometryUtils.BuildVector3FromVector2(ScreenUtils.GetScreenSize(), 1);

        GradientQuad gradientQuad = radialGradientBackground.GetComponent<GradientQuad>();
        gradientQuad.InitRadial(ColorUtils.GetColorFromRGBAVector4(new Vector4(146,21,51,255)),
                                ColorUtils.GetColorFromRGBAVector4(new Vector4(64,12,26,255)),
                                500);

        m_isShown = true;
    }

    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    /**
     * We build the grid of anchors that is displayed on the screen and that will help the player positionning shapes and axis...
     * Two anchors are separated from each other of a distance of m_gridSpacing that can be set in the editor
     * **/
    private void ShowGrid(float fDelay = 0.0f)
    {
        m_grid = this.gameObject.GetComponentInChildren<Grid>();
        m_grid.gameObject.transform.parent = this.gameObject.transform;
        m_grid.Build();
        Vector3 gridLocalPosition = m_grid.gameObject.transform.localPosition;
        m_grid.gameObject.transform.localPosition = new Vector3(gridLocalPosition.x, gridLocalPosition.y, GRID_Z_VALUE);

        GameObjectAnimator gridAnimator = m_grid.gameObject.GetComponent<GameObjectAnimator>();
        gridAnimator.SetOpacity(0);
        gridAnimator.FadeTo(1, 0.5f, fDelay);

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
        Vector2 interfaceButtonSize = new Vector2(110.0f, 110.0f);

        m_interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");
        m_interfaceButtonsHolder.transform.parent = this.gameObject.transform;

        //define some variables to help us positioning buttons correctly
        float distanceToTopBorder = 30.0f;
        float distanceToRightBorder = 30.0f;
        float distanceBetweenButtons = 30.0f;

        GameObjectAnimator holderAnimator = m_interfaceButtonsHolder.AddComponent<GameObjectAnimator>();
        holderAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y - 0.5f * interfaceButtonSize.y - distanceToTopBorder, INTERFACE_BUTTONS_Z_VALUE));

        //menu button
        GameObject pauseButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_MENU_BUTTON, interfaceButtonSize);
        pauseButtonObject.name = "PauseButton";
        pauseButtonObject.transform.parent = m_interfaceButtonsHolder.transform;

        GameObjectAnimator pauseButtonAnimator = pauseButtonObject.GetComponent<GameObjectAnimator>();
        float pauseButtonPositionX = 0.5f * screenSize.x - distanceToRightBorder - 0.5f * interfaceButtonSize.x;
        pauseButtonAnimator.SetPosition(new Vector3(pauseButtonPositionX, 0, 0));

        //retry button
        GameObject retryButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_RETRY_BUTTON, interfaceButtonSize);
        retryButtonObject.name = "RetryButton";
        retryButtonObject.transform.parent = m_interfaceButtonsHolder.transform;

        GameObjectAnimator retryButtonAnimator = retryButtonObject.GetComponent<GameObjectAnimator>();
        float retryBtnPositionX = pauseButtonPositionX - distanceBetweenButtons - interfaceButtonSize.x;
        retryButtonAnimator.SetPosition(new Vector3(retryBtnPositionX, 0, 0));

        //hints button
        GameObject hintsButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_HINTS_BUTTON, interfaceButtonSize);
        hintsButtonObject.name = "HintsButton";
        hintsButtonObject.transform.parent = m_interfaceButtonsHolder.transform;

        GameObjectAnimator hintsButtonAnimator = hintsButtonObject.GetComponent<GameObjectAnimator>();
        float hintsBtnPositionX = retryBtnPositionX - distanceBetweenButtons - interfaceButtonSize.x;
        hintsButtonAnimator.SetPosition(new Vector3(hintsBtnPositionX, 0, 0));

        holderAnimator.SetOpacity(1);
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
        m_middleActionButton.Show(fDelay);
        m_bottomActionButton.Show(fDelay);
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
    private void ShowInitialShapes(float fDelay = 0.0f)
    {
        m_shapes = this.gameObject.GetComponentInChildren<Shapes>();
        m_shapes.gameObject.transform.parent = this.gameObject.transform;
        m_shapes.gameObject.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        List<Shape> initialShapes = GetLevelManager().m_currentLevel.m_initialShapes;
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = new Shape(initialShapes[iShapeIndex]); //make a deep copy of the shape object stored in the level manager

            //First triangulate the shape and set the color of each triangle
            shape.Triangulate();

            GameObject shapeObject = m_shapes.CreateShapeObjectFromData(shape);

            ShapeAnimator shapeAnimator = shapeObject.GetComponent<ShapeAnimator>();
            shapeAnimator.SetOpacity(Shapes.SHAPES_OPACITY);
        }

        m_shapes.gameObject.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        GameObjectAnimator shapesAnimator = m_shapes.gameObject.GetComponent<GameObjectAnimator>();
        shapesAnimator.SetOpacity(0);
        shapesAnimator.FadeTo(Shapes.SHAPES_OPACITY, 0.5f, fDelay);

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