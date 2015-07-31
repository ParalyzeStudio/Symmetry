using UnityEngine;
using System.Collections.Generic;

public class GameScene : GUIScene
{
    public const float INTERFACE_BUTTONS_Z_VALUE = -30.0f;
    public const float ACTION_BUTTONS_Z_VALUE = -30.0f;
    public const float COUNTER_Z_VALUE = -30.0f;

    public const float GRID_Z_VALUE = -10.0f;
    public const float CONTOURS_Z_VALUE = -200.0f;
    public const float SHAPES_Z_VALUE = -20.0f;
    public const float AXES_Z_VALUE = -210.0f;

    //public GameObject m_gridAnchorSelectedPfb;
    public GameObject m_radialGradientPfb;

    public Grid m_grid { get; set; }
    public Counter m_counter { get; set; }
    public Outlines m_outlines { get; set; }
    public Shapes m_shapes { get; set; }
    public Axes m_axes { get; set; }

    //holders
    public GameObject m_actionButtonsHolder { get; set; }
    public GameObject m_interfaceButtonsHolder { get; set; }

    //Action tags
    public const string ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL = "SYMMETRY_AXIS_HORIZONTAL";
    public const string ACTION_TAG_SYMMETRY_AXIS_VERTICAL = "SYMMETRY_AXIS_VERTICAL";
    public const string ACTION_TAG_SYMMETRY_AXES_STRAIGHT = "SYMMETRY_AXES_STRAIGHT";
    public const string ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT = "SYMMETRY_AXIS_DIAGONAL_LEFT";
    public const string ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT = "SYMMETRY_AXIS_DIAGONAL_RIGHT";
    public const string ACTION_TAG_SYMMETRY_AXES_DIAGONALS = "SYMMETRY_AXES_DIAGONALS";
    public const string ACTION_TAG_SYMMETRY_AXES_ALL = "SYMMETRY_AXES_ALL";
    public const string ACTION_TAG_MOVE_SHAPE = "MOVE_SHAPE";

    public string m_activeActionTag { get; set; }

    public bool m_isShown; //is the current scene displayed entirely

    //buttons
    public List<HUDButton> m_actionButtons { get; set; }
    public List<HUDButton> m_interfaceButtons { get; set; }
    public HUDButton m_selectedActionButton { get; set; } //the action button the player has selected, null if none is selected

    public override void Init()
    {
        base.Init();
        m_isShown = false;
    }

    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);
        GameObjectAnimator sceneAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        sceneAnimator.SetOpacity(1.0f);

        //Display the grid
        ShowGrid(fDelay);
        
        //Display interface buttons (pause, retry and hints)
        ShowInterfaceButtons(fDelay);

        //Display action counter
        ShowCounter(fDelay);

        //Show outlines (if applicable)
        ShowOutlines(fDelay);

        //Show available symmetry axes
        ShowAvailableAxes();

        //Show action buttons
        ShowActionButtons();

        //ShowInitialShapes(fDelay);
        //m_axes = this.gameObject.GetComponentInChildren<Axes>();
        //m_axes.transform.localPosition = new Vector3(0, 0, AXES_Z_VALUE);

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
    private void ShowGrid(float fDelay)
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
    private void ShowInterfaceButtons(float fDelay)
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
     * Fades out interface buttons holder
     * **/
    public void DismissActionButtons(float fDuration, float fDelay)
    {
        GameObjectAnimator actionButtonsHolderAnimator = m_actionButtonsHolder.GetComponent<GameObjectAnimator>();
        actionButtonsHolderAnimator.FadeTo(0.0f, fDuration, fDelay);
    }

    /**
     * Counters to show remaining actions for the player
     * **/
    private void ShowCounter(float fDelay)
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
    private void ShowOutlines(float fDelay)
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
    private void ShowAvailableAxes()
    {

    }

    /**
     * Build the 3 action buttons that appear on the left side of the screen
     * -First button allows player to switch between action modes (symmetry types or shape translation)
     * -Second button is for modifying the behavior of color symmetrization (addition or soustraction)
     * -Third button is for picking a color that will apply to the symmetry done by the user by filtering shapes that are not of that specific color
     * **/
    private void ShowActionButtons()
    {
        Vector2 actionButtonSkinSize = new Vector2(128,128);

        //Show top button
        GameObject topActionButton = GetGUIManager().CreateActionButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
                                                                             actionButtonSkinSize,
                                                                             ActionButton.Location.TOP);

        topActionButton.name = "TopActionBtn";
        topActionButton.transform.parent = this.transform;

        //Show middle button
        GameObject middleActionButton = GetGUIManager().CreateActionButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
                                                                                actionButtonSkinSize,
                                                                                ActionButton.Location.MIDDLE);

        middleActionButton.name = "MiddleActionBtn";
        middleActionButton.transform.parent = this.transform;

        //Show bottom button
        GameObject bottomActionButton = GetGUIManager().CreateActionButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
                                                                                actionButtonSkinSize,
                                                                                ActionButton.Location.BOTTOM);

        bottomActionButton.name = "BottomActionBtn";
        bottomActionButton.transform.parent = this.transform;

        topActionButton.GetComponent<ActionButton>().Show();
        middleActionButton.GetComponent<ActionButton>().Show();
        bottomActionButton.GetComponent<ActionButton>().Show();
    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void ShowInitialShapes(float fDelay)
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        m_shapes = this.gameObject.GetComponentInChildren<Shapes>();
        m_shapes.gameObject.transform.parent = this.gameObject.transform;
        m_shapes.gameObject.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        List<Shape> initialShapes = levelManager.m_currentLevel.m_initialShapes;
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
     * Method that tells if one of the symmetry HUDButton is selected
     * **/
    public bool IsSymmetryHUDButtonSelected()
    {
        return (m_activeActionTag != null &&
                    (
                    m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXES_ALL) ||
                    m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXES_DIAGONALS) ||
                    m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXES_STRAIGHT) ||
                    m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT) ||
                    m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT) ||
                    m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL) ||
                    m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_VERTICAL)
                    )
               );
    }

    /**
     * Method that tells if one of move shape HUDButton is selected
     * **/
    public bool IsMoveShapeHUDButtonSelected()
    {
        return m_activeActionTag != null && m_activeActionTag.Equals(ACTION_TAG_MOVE_SHAPE);
    }

    public List<Vector2> GetDirectionsForSymmetryActiveActionTag()
    {
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

        List<Vector2> directions = new List<Vector2>();

        /***
         * TMP DEBUG: unlock all directions
         * ***/
        directions.Add(rightDirection);
        directions.Add(bottomDirection);
        directions.Add(leftDirection);
        directions.Add(topDirection);
        directions.Add(topRightDirection);
        directions.Add(bottomRightDirection);
        directions.Add(bottomLeftDirection);
        directions.Add(topLeftDirection);
        return directions;
        /***
         * 
         * ***/

        if (m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXES_ALL))
        {
            directions.Add(rightDirection);
            directions.Add(bottomDirection);
            directions.Add(leftDirection);
            directions.Add(topDirection);
            directions.Add(topRightDirection);
            directions.Add(bottomRightDirection);
            directions.Add(bottomLeftDirection);
            directions.Add(topLeftDirection);
        }
        else if (m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXES_STRAIGHT))
        {
            directions.Add(rightDirection);
            directions.Add(bottomDirection);
            directions.Add(leftDirection);
            directions.Add(topDirection);
        }
        else if (m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXES_DIAGONALS))
        {
            directions.Add(topRightDirection);
            directions.Add(bottomRightDirection);
            directions.Add(bottomLeftDirection);
            directions.Add(topLeftDirection);
        }
        else if (m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL))
        {
            directions.Add(rightDirection);
            directions.Add(leftDirection);
        }
        else if (m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_VERTICAL))
        {
            directions.Add(bottomDirection);
            directions.Add(topDirection);
        }
        else if (m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT))
        {
            directions.Add(bottomRightDirection);
            directions.Add(topLeftDirection);
        }
        else if (m_activeActionTag.Equals(ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT))
        {
            directions.Add(topRightDirection);
            directions.Add(bottomLeftDirection);
        }

        return directions;
    }
}