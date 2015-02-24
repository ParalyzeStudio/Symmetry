using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Vector2 m_designScreenSize;
    private bool m_levelBuilt;
    private LevelManager m_levelManager;

    public enum SceneMode
    {
        MENU,
        LEVELS,
        GAME
    }

    public SceneMode m_sceneMode;

    public enum GameStatus
    {
        RUNNING,
        PAUSED,
        VICTORY,
        DEFEAT
    };

    private GameStatus m_gameStatus;

    public const float GRID_Z_VALUE = -10.0f;
    public const float CONTOURS_Z_VALUE = -20.0f;
    public const float SHAPES_Z_VALUE = -30.0f;

    protected void Awake()
    {
        if (m_sceneMode == SceneMode.GAME)
        {
            m_levelBuilt = false;
            m_levelManager = null;
        }
    }

    //TMP DEBUG
    //public GameObject m_gridAnchorSelectedPfb;

    protected void Start()
    {
        GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");
        backgroundObject.GetComponent<BackgroundAdaptativeSize>().InvalidateSize();

        if (m_sceneMode == SceneMode.GAME)
        {
            GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
            m_levelManager = levelManagerObject.GetComponent<LevelManager>();
            m_levelManager.ParseAllLevels();
            BuildAndShowLevel(1);

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
        else
        {
            GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>().ShowContent(GUIManager.DisplayContent.MENU, true);
            //GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>().ShowContent(GUIManager.DisplayContent.CHAPTERS, true);
        }

        TouchHandler.s_touchDeactivated = false;
    }

    protected void Update()
    {
        if (m_sceneMode == SceneMode.GAME)
        {
            GameStatus gameStatus = GetGameStatus();
            if (gameStatus == GameStatus.VICTORY || gameStatus == GameStatus.DEFEAT)
            {
                EndLevel();
            }
        }
    }

    ///**
    // * Builds and shows main menu (play button, options...)
    // * **/
    //public void BuildAndShowMainMenu()
    //{
    //    GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>().ShowContent(GUIManager.DisplayContent.MENU, true);
    //}

    ///**
    // * Builds and shows main menu (play button, options...)
    // * **/
    //public void BuildAndShowLevelsMenu()
    //{

    //}

    public void LoadAndStartLevel(int iLevelNumber)
    {
        BuildAndShowLevel(iLevelNumber);
    }

    /**
     * Builds everything that appears on a screen for a certain level (GUI, grid, contours, shapes)
     * **/
    public void BuildAndShowLevel(int iLevelNumber)
    {
        if (!m_levelBuilt)
        {
            m_levelManager.SetCurrentLevelByNumber(iLevelNumber);
            BuildGrid();
            BuildGUI();
            BuildContours();
            BuildShapes();

            m_levelBuilt = true;

            ShowLevel();
        }
    }

    public void ShowLevel()
    {


    }

    /**
     * We build the grid of anchors that is displayed on the screen and that will help the player positionning shapes and axis...
     * Two anchors are separated from each other of a distance of m_gridSpacing that can be set in the editor
     * **/
    private void BuildGrid()
    {
        GameObject grid = GameObject.FindGameObjectWithTag("Grid");
        grid.transform.position = new Vector3(0, 0, GRID_Z_VALUE);
        grid.GetComponent<GridBuilder>().Build();
    }

    /**
     * GUI elements such as pause button, help button, retry button and other stuff
     * **/
    private void BuildGUI()
    {
        GameObject gameHUDObject = GameObject.FindGameObjectWithTag("GameHUD");
        GameHUD gameHUD = gameHUDObject.GetComponent<GameHUD>();
        gameHUD.BuildForLevel(m_levelManager.m_currentLevel.m_number);
    }

    /**
     * Here we build the contour of the shape the player has to reproduce
     * **/
    private void BuildContours()
    {
        GameObject contours = GameObject.FindGameObjectWithTag("Contours");
        contours.transform.position = new Vector3(0, 0, CONTOURS_Z_VALUE);
        contours.GetComponent<ContoursBuilder>().Build();
    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void BuildShapes()
    {
        GameObject shapesObject = GameObject.FindGameObjectWithTag("Shapes");
        ShapeBuilder shapeBuilder = shapesObject.GetComponent<ShapeBuilder>();
        List<Shape> initialShapes = m_levelManager.m_currentLevel.m_initialShapes;
        for (int iShapeIndex = 0; iShapeIndex != initialShapes.Count; iShapeIndex++)
        {
            Shape shape = initialShapes[iShapeIndex];

            //First triangulate the shape
            shape.Triangulate();

            shapeBuilder.CreateFromShapeData(shape);
        }

        shapesObject.transform.position = new Vector3(0, 0, SHAPES_Z_VALUE);
    }

    /**
     * Returns the current status of the game
     * **/
    public GameStatus GetGameStatus()
    {
        if (m_gameStatus == GameStatus.DEFEAT || m_gameStatus == GameStatus.VICTORY)
            return m_gameStatus;

        bool victory = IsVictory();
        if (victory)
            m_gameStatus = GameStatus.VICTORY;
        else
            m_gameStatus = GameStatus.RUNNING;

        return m_gameStatus;
    }

    /**
     * Checks if the contour is filled exactly
     * Calculate the sum of the areas of contours and compare it to the area occupied by all the shapes
     * **/
    public bool IsVictory()
    {
        //First we check if one of the shapes intersects a contour
        GameObject shapesObject = GameObject.FindGameObjectWithTag("Shapes");
        ShapesHolder shapesHolder = shapesObject.GetComponent<ShapesHolder>();
        List<GameObject> allShapeObjects = shapesHolder.m_shapesObj;
        List<Contour> allContours = m_levelManager.m_currentLevel.m_contours;
        float shapesArea = 0;
        for (int iShapeIndex = 0; iShapeIndex != allShapeObjects.Count; iShapeIndex++)
        {
            Shape shape = allShapeObjects[iShapeIndex].GetComponent<ShapeRenderer>().m_shape;
            bool shapeInsideContour = false;
            for (int iContourIndex = 0; iContourIndex != allContours.Count; iContourIndex++)
            {
                Contour contour = allContours[iContourIndex];
                if (shape.IntersectsContour(contour)) //we check if this shape intersects a contour
                    return false;
                else //if not we check if this shape is inside a contour
                {
                    if (contour.ContainsGridPoint(shape.m_gridTriangles[0].GetBarycentre())) 
                    {
                        shapeInsideContour = true;
                        break;
                    }
                }
            }

            if (!shapeInsideContour)
                return false;

            shapesArea += shape.m_area;
        }

        //Debug.Log("1: NO SHAPE/CONTOUR INTERSECTION");

        //if not we check if every shape is inside a contour
        //for (int iShapeIndex = 0; iShapeIndex != allShapeObjects.Count; iShapeIndex++)
        //{
        //    Shape shape = allShapeObjects[iShapeIndex].GetComponent<ShapeRenderer>().m_shape;
        //    bool shapeInsideContour = false;
        //    for (int iContourIndex = 0; iContourIndex != allContours.Count; iContourIndex++)
        //    {
        //        Contour contour = allContours[iContourIndex];
        //        if (contour.ContainsGridPoint(shape.m_gridTriangles[0].GetBarycentre()))
        //        {
        //            shapeInsideContour = true;
        //            break;
        //        }
        //    }

        //    if (!shapeInsideContour)
        //        return false;

        //    shapesArea += shape.m_area;
        //}

        //Debug.Log("2: ALL SHAPES INSIDE CONTOUR");

        //finally we check if the sum of the areas of all shapes is equal to the sum of the areas of all contours
        float contoursArea = 0;
        for (int iContourIndex = 0; iContourIndex != allContours.Count; iContourIndex++)
        {
            Contour contour = allContours[iContourIndex];
            contoursArea += contour.m_area;
        }

        if (contoursArea == shapesArea)
            Debug.Log("3: SAME AREA");

        return (contoursArea == shapesArea);
    }


    /**
     * Ends the current level by fading out the grid and contours and disabling touch
     * After a few seconds launch next level
     * **/
    public void EndLevel()
    {

    }
}
