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

    public Grid m_grid { get; set; }
    public Counter m_counter { get; set; }
    public Outlines m_outlines { get; set; }
    public Shapes m_shapes { get; set; }
    public Axes m_axes { get; set; }

    //action buttons prefabs
    public GameObject m_horAxisPfb;
    public GameObject m_vertAxisPfb;
    public GameObject m_straightAxesPfb;
    public GameObject m_leftDiagonalAxisPfb;
    public GameObject m_rightDiagonalAxisPfb;
    public GameObject m_diagonalsAxesPfb;
    public GameObject m_allAxesPfb;
    public GameObject m_moveShapePfb;

    //interface buttons prefabs
    public GameObject m_menuBtnPfb;
    public GameObject m_retryBtnPfb;
    public GameObject m_hintsBtnPfb;

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

        ShowGrid(fDelay);
        ShowActionButtons(fDelay);
        ShowInterfaceButtons(fDelay);
        ShowCounter(fDelay);
        //ShowGUI(fDelay);
        ShowOutlines(fDelay);
        ShowInitialShapes(fDelay);
        m_axes = this.gameObject.GetComponentInChildren<Axes>();
        m_axes.transform.localPosition = new Vector3(0, 0, AXES_Z_VALUE);

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
     * GUI elements such as pause button, help button, retry button and other stuff
     * **/
    //private void ShowGUI(float fDelay)
    //{
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

    //    GameObject gameHUDObject = GameObject.FindGameObjectWithTag("GameHUD");
    //    GameHUD gameHUD = gameHUDObject.GetComponent<GameHUD>();
    //    gameHUD.BuildForLevel(levelManager.m_currentLevel.m_number);
    //}

    /**
     * Actions the player can do on grid and shapes
     * **/
    private void ShowActionButtons(float fDelay)
    {
        m_actionButtons = new List<HUDButton>();

        GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");
        Vector2 screenSize = backgroundObject.GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        List<string> actionTags = levelManager.m_currentLevel.m_actionButtonsTags;

        m_actionButtonsHolder = new GameObject("ActionButtonsHolder");
        m_actionButtonsHolder.transform.parent = this.gameObject.transform;
        m_actionButtonsHolder.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, COUNTER_Z_VALUE);

        GameObjectAnimator holderAnimator = m_actionButtonsHolder.AddComponent<GameObjectAnimator>();

        float gapBetweenButtons = 20.0f;
        float distanceToScreenLeftSide = 60.0f;
        Vector2 actionButtonSize = new Vector2(96.0f, 96.0f);
        for (int iTagIndex = 0; iTagIndex != actionTags.Count; iTagIndex++)
        {
            //calculate the local position of the button
            int numberOfActionButtons = m_actionButtons.Count;
            float xPosition = -0.5f * screenSize.x +
                              distanceToScreenLeftSide +
                              numberOfActionButtons * actionButtonSize.x +
                              numberOfActionButtons * gapBetweenButtons +
                              0.5f * actionButtonSize.x;
            Vector2 buttonLocalPosition = new Vector2(xPosition, 0);

            string tag = actionTags[iTagIndex];
            GameObject clonedButton = null;
            HUDButton hudButton = null;
            HUDButton.HUDButtonID hudButtonID = 0;
            if (tag.Equals(ACTION_TAG_SYMMETRY_AXES_ALL))
            {
                clonedButton = (GameObject)Instantiate(m_allAxesPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_SYMMETRY_ALL_AXES;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL))
            {
                clonedButton = (GameObject)Instantiate(m_horAxisPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_HORIZONTAL;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_VERTICAL))
            {
                clonedButton = (GameObject)Instantiate(m_vertAxisPfb);                
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_VERTICAL;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXES_STRAIGHT))
            {
                clonedButton = (GameObject)Instantiate(m_straightAxesPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_SYMMETRY_STRAIGHT_AXES;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT))
            {
                clonedButton = (GameObject)Instantiate(m_leftDiagonalAxisPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_LEFT;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT))
            {
                clonedButton = (GameObject)Instantiate(m_rightDiagonalAxisPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_RIGHT;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXES_DIAGONALS))
            {
                clonedButton = (GameObject)Instantiate(m_diagonalsAxesPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_SYMMETRY_DIAGONAL_AXES;
            }
            else if (tag.Equals(ACTION_TAG_MOVE_SHAPE))
            {
                clonedButton = (GameObject)Instantiate(m_moveShapePfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButtonID = HUDButton.HUDButtonID.ID_MOVE_SHAPE;
            }


            if (clonedButton != null)
            {
                clonedButton.transform.parent = m_actionButtonsHolder.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                m_actionButtons.Add(clonedButton.GetComponentInChildren<HUDButton>());
            }

            if (hudButton != null)
            {
                hudButton.Init();
                hudButton.m_ID = hudButtonID;
                hudButton.SetSize(actionButtonSize);
            }
        }


        holderAnimator.SetOpacity(0);
        holderAnimator.FadeTo(1, 0.2f, fDelay);
    }

    /**
     * Pause, retry and hints buttons
     * **/
    private void ShowInterfaceButtons(float fDelay)
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;
        Vector2 interfaceButtonSize = new Vector2(110.0f, 110.0f);

        m_interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");
        m_interfaceButtonsHolder.transform.parent = this.gameObject.transform;
        m_interfaceButtonsHolder.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, INTERFACE_BUTTONS_Z_VALUE);

        GameObjectAnimator holderAnimator = m_interfaceButtonsHolder.AddComponent<GameObjectAnimator>();

        float distanceToRightBorder = 30.0f;
        float distanceBetweenButtons = 30.0f;

        //menu button
        GameObject clonedMenuBtn = (GameObject)Instantiate(m_menuBtnPfb);
        clonedMenuBtn.transform.parent = m_interfaceButtonsHolder.transform;
        float menuBtnXPosition = 0.5f * screenSize.x - distanceToRightBorder - 0.5f * interfaceButtonSize.x;
        clonedMenuBtn.transform.localPosition = new Vector3(menuBtnXPosition, 0, 0);
        GUIInterfaceButton interfaceButton = clonedMenuBtn.GetComponent<GUIInterfaceButton>();
        interfaceButton.Init();
        interfaceButton.SetSize(interfaceButtonSize);

        //retry button
        GameObject clonedRetryBtn = (GameObject)Instantiate(m_retryBtnPfb);
        clonedRetryBtn.transform.parent = m_interfaceButtonsHolder.transform;
        float retryBtnXPosition = menuBtnXPosition - distanceBetweenButtons - interfaceButtonSize.x;
        clonedRetryBtn.transform.localPosition = new Vector3(retryBtnXPosition, 0, 0);
        interfaceButton = clonedRetryBtn.GetComponent<GUIInterfaceButton>();
        interfaceButton.Init();
        interfaceButton.SetSize(interfaceButtonSize);

        //hints button
        GameObject clonedHintsBtn = (GameObject)Instantiate(m_hintsBtnPfb);
        clonedHintsBtn.transform.parent = m_interfaceButtonsHolder.transform;
        float hintsBtnXPosition = retryBtnXPosition - distanceBetweenButtons - interfaceButtonSize.x;
        clonedHintsBtn.transform.localPosition = new Vector3(hintsBtnXPosition, 0, 0);
        interfaceButton = clonedHintsBtn.GetComponent<GUIInterfaceButton>();
        interfaceButton.Init();
        interfaceButton.SetSize(interfaceButtonSize);

        holderAnimator.SetOpacity(0);
        holderAnimator.FadeTo(1, 0.2f, fDelay);
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
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

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
        outlinesAnimator.FadeTo(1, 0.5f, fDelay);
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
            shape.PropagateColorToTriangles();

            m_shapes.CreateShapeObjectFromData(shape);
        }

        m_shapes.gameObject.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        GameObjectAnimator shapesAnimator = m_shapes.gameObject.GetComponent<GameObjectAnimator>();
        shapesAnimator.SetOpacity(0);
        shapesAnimator.FadeTo(0.5f, 0.5f, fDelay);

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