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

    //public GameObject m_gridAnchorSelectedPfb;

    public Grid m_grid { get; set; }
    public Counter m_counter { get; set; }
    public Contours m_contours { get; set; }
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

    //interface buttons prefabs
    public GameObject m_menuBtnPfb;
    public GameObject m_retryBtnPfb;
    public GameObject m_hintsBtnPfb;

    //Action tags
    public const string ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL = "SYMMETRY_AXIS_HORIZONTAL";
    public const string ACTION_TAG_SYMMETRY_AXIS_VERTICAL = "SYMMETRY_AXIS_VERTICAL";
    public const string ACTION_TAG_SYMMETRY_AXES_STRAIGHT = "SYMMETRY_AXES_STRAIGHT";
    public const string ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT = "SYMMETRY_AXIS_DIAGONAL_LEFT";
    public const string ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT = "SYMMETRY_AXIS_DIAGONAL_RIGHT";
    public const string ACTION_TAG_SYMMETRY_AXES_DIAGONALS = "SYMMETRY_AXES_DIAGONALS";
    public const string ACTION_TAG_SYMMETRY_AXES_ALL = "SYMMETRY_AXES_ALL";

    public string m_activeActionTag { get; set; }

    //buttons
    public List<HUDButton> m_actionButtons { get; set; }
    public List<HUDButton> m_interfaceButtons { get; set; }
    public HUDButton m_selectedActionButton { get; set; } //the action button the player has selected, null if none is selected

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
        ShowContours(fDelay);
        ShowInitialShapes(fDelay);
        m_axes = this.gameObject.GetComponentInChildren<Axes>();

        //GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        //guiManager.AnimateFrames(SceneManager.DisplayContent.GAME, fDelay);
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
        m_grid.Build(false);
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

        GameObject actionButtonsHolder = new GameObject("ActionButtonsHolder");
        actionButtonsHolder.transform.parent = this.gameObject.transform;
        actionButtonsHolder.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, COUNTER_Z_VALUE);

        GameObjectAnimator holderAnimator = actionButtonsHolder.AddComponent<GameObjectAnimator>();

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
            if (tag.Equals(ACTION_TAG_SYMMETRY_AXES_ALL))
            {
                clonedButton = (GameObject)Instantiate(m_allAxesPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_ALL_AXES;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL))
            {
                clonedButton = (GameObject)Instantiate(m_horAxisPfb);
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_HORIZONTAL;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_VERTICAL))
            {
                clonedButton = (GameObject)Instantiate(m_vertAxisPfb);                
                hudButton = clonedButton.GetComponent<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_VERTICAL;
            }

            if (clonedButton != null)
            {
                clonedButton.transform.parent = actionButtonsHolder.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                m_actionButtons.Add(clonedButton.GetComponentInChildren<HUDButton>());
            }

            if (hudButton != null)
                hudButton.SetSize(actionButtonSize);
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

        GameObject interfaceButtonsHolder = new GameObject("InterfaceButtonsHolder");
        interfaceButtonsHolder.transform.parent = this.gameObject.transform;
        interfaceButtonsHolder.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, INTERFACE_BUTTONS_Z_VALUE);

        GameObjectAnimator holderAnimator = interfaceButtonsHolder.AddComponent<GameObjectAnimator>();

        float distanceToRightBorder = 30.0f;
        float distanceBetweenButtons = 30.0f;

        //menu button
        GameObject clonedMenuBtn = (GameObject)Instantiate(m_menuBtnPfb);
        clonedMenuBtn.transform.parent = interfaceButtonsHolder.transform;
        float menuBtnXPosition = 0.5f * screenSize.x - distanceToRightBorder - 0.5f * interfaceButtonSize.x;
        clonedMenuBtn.transform.localPosition = new Vector3(menuBtnXPosition, 0, 0);
        GUIInterfaceButton interfaceButton = clonedMenuBtn.GetComponent<GUIInterfaceButton>();
        interfaceButton.SetSize(interfaceButtonSize);

        //retry button
        GameObject clonedRetryBtn = (GameObject)Instantiate(m_retryBtnPfb);
        clonedRetryBtn.transform.parent = interfaceButtonsHolder.transform;
        float retryBtnXPosition = menuBtnXPosition - distanceBetweenButtons - interfaceButtonSize.x;
        clonedRetryBtn.transform.localPosition = new Vector3(retryBtnXPosition, 0, 0);
        interfaceButton = clonedRetryBtn.GetComponent<GUIInterfaceButton>();
        interfaceButton.SetSize(interfaceButtonSize);

        //hints button
        GameObject clonedHintsBtn = (GameObject)Instantiate(m_hintsBtnPfb);
        clonedHintsBtn.transform.parent = interfaceButtonsHolder.transform;
        float hintsBtnXPosition = retryBtnXPosition - distanceBetweenButtons - interfaceButtonSize.x;
        clonedHintsBtn.transform.localPosition = new Vector3(hintsBtnXPosition, 0, 0);
        interfaceButton = clonedHintsBtn.GetComponent<GUIInterfaceButton>();
        interfaceButton.SetSize(interfaceButtonSize);

        holderAnimator.SetOpacity(0);
        holderAnimator.FadeTo(1, 0.2f, fDelay);
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
     * Here we build the contour of the shape the player has to reproduce
     * **/
    private void ShowContours(float fDelay)
    {
        m_contours = this.gameObject.GetComponentInChildren<Contours>();
        m_contours.transform.parent = this.gameObject.transform;
        m_contours.transform.localPosition = new Vector3(0, 0, CONTOURS_Z_VALUE);
        m_contours.Build();

        GameObjectAnimator contoursAnimator = m_contours.gameObject.GetComponent<GameObjectAnimator>();
        contoursAnimator.SetOpacity(0);
        contoursAnimator.FadeTo(1, 0.5f, fDelay);
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
            Shape shape = initialShapes[iShapeIndex];

            //First triangulate the shape
            shape.Triangulate();

            m_shapes.CreateShapeFromData(shape);
        }

        m_shapes.gameObject.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        GameObjectAnimator shapesAnimator = m_shapes.gameObject.GetComponent<GameObjectAnimator>();
        shapesAnimator.SetOpacity(0);
        shapesAnimator.FadeTo(1, 0.5f, fDelay);
    }
}