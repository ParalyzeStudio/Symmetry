using UnityEngine;
using System.Collections.Generic;

public class GameScene : GUIScene
{
    public const float GRID_Z_VALUE = -10.0f;
    public const float CONTOURS_Z_VALUE = -20.0f;
    public const float SHAPES_Z_VALUE = -30.0f;

    //public GameObject m_gridAnchorSelectedPfb;

    public GameObject m_gridPfb;
    public GameObject m_contoursHolderPfb;
    public GameObject m_axesHolderPfb;
    public GameObject m_shapesHolderPfb;

    //action buttons prefabs
    public GameObject m_horAxisPfb;
    public GameObject m_vertAxisPfb;
    public GameObject m_straightAxesPfb;
    public GameObject m_leftDiagonalAxisPfb;
    public GameObject m_rightDiagonalAxisPfb;
    public GameObject m_diagonalsAxesPfb;
    public GameObject m_allAxesPfb;

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
        ShowShapes(fDelay);

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
        GameObject clonedGrid = (GameObject) Instantiate(m_gridPfb);
        clonedGrid.transform.parent = this.gameObject.transform;
        clonedGrid.GetComponent<GridBuilder>().Build(false);
        Vector3 gridLocalPosition = clonedGrid.transform.localPosition;
        clonedGrid.transform.localPosition = new Vector3(gridLocalPosition.x, gridLocalPosition.y, GRID_Z_VALUE);

        GameObjectAnimator gridAnimator = clonedGrid.GetComponent<GameObjectAnimator>();
        gridAnimator.SetOpacity(0);
        gridAnimator.FadeTo(1, 0.5f, fDelay);

        ///*** DEBUG TMP ***/
        ///GridBuilder gridBuilder = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridBuilder>();
        ////List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXIS_VERTICAL);
        ////List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXIS_HORIZONTAL);
        ////List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(8, 9), Symmetrizer.SymmetryType.SYMMETRY_AXES_STRAIGHT);
        //List<GameObject> anchors = gridBuilder.GetAnchorsConstrainedBySymmetryType(new Vector2(19, 1), Symmetrizer.SymmetryType.SYMMETRY_AXES_ALL);
        //foreach (GameObject anchor in anchors)
        //{
        //    Vector2 gridPos = gridBuilder.GetGridCoordinatesFromWorldCoordinates(anchor.transform.position);
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
            float yPosition = 0.428f * screenSize.y;
            Vector2 buttonLocalPosition = new Vector2(xPosition, yPosition);

            string tag = actionTags[iTagIndex];
            GameObject clonedButton = null;
            HUDButton hudButton = null;
            if (tag.Equals(ACTION_TAG_SYMMETRY_AXES_ALL))
            {
                clonedButton = (GameObject)Instantiate(m_allAxesPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                hudButton = clonedButton.GetComponentInChildren<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_ALL_AXES;
                hudButton.m_type = HUDButton.HUDButtonType.ACTION;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL))
            {
                clonedButton = (GameObject)Instantiate(m_horAxisPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                hudButton = clonedButton.GetComponentInChildren<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_HORIZONTAL;
                hudButton.m_type = HUDButton.HUDButtonType.ACTION;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_VERTICAL))
            {
                clonedButton = (GameObject)Instantiate(m_vertAxisPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                hudButton = clonedButton.GetComponentInChildren<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_VERTICAL;
                hudButton.m_type = HUDButton.HUDButtonType.ACTION;
            }

            if (clonedButton != null)
                m_actionButtons.Add(clonedButton.GetComponentInChildren<HUDButton>());

            if (hudButton != null)
                hudButton.SetSize(actionButtonSize);
        }
    }

    /**
     * Pause, retry and hints buttons
     * **/
    private void ShowInterfaceButtons(float fDelay)
    {

    }

    /**
     * Counters to show remaining actions for the player
     * **/
    private void ShowCounter(float fDelay)
    {

    }

    /**
     * Here we build the contour of the shape the player has to reproduce
     * **/
    private void ShowContours(float fDelay)
    {
        GameObject clonedContours = (GameObject)Instantiate(m_contoursHolderPfb);
        clonedContours.transform.parent = this.gameObject.transform;
        clonedContours.transform.localPosition = new Vector3(0, 0, CONTOURS_Z_VALUE);
        clonedContours.GetComponent<ContoursBuilder>().Build();

        GameObjectAnimator contoursAnimator = clonedContours.GetComponent<GameObjectAnimator>();
        contoursAnimator.SetOpacity(0);
        contoursAnimator.FadeTo(1, 0.5f, fDelay);
    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void ShowShapes(float fDelay)
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        GameObject clonedShapes = (GameObject) Instantiate(m_shapesHolderPfb);
        clonedShapes.transform.parent = this.gameObject.transform;
        clonedShapes.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);
        ShapeBuilder shapeBuilder = clonedShapes.GetComponent<ShapeBuilder>();
        List<Shape> initialShapes = levelManager.m_currentLevel.m_initialShapes;
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = initialShapes[iShapeIndex];

            //First triangulate the shape
            shape.Triangulate();

            shapeBuilder.CreateFromShapeData(shape);
        }

        clonedShapes.transform.localPosition = new Vector3(0, 0, SHAPES_Z_VALUE);

        GameObjectAnimator shapesAnimator = clonedShapes.GetComponent<GameObjectAnimator>();
        shapesAnimator.SetOpacity(0);
        shapesAnimator.FadeTo(1, 0.5f, fDelay);
    }
}