﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Vector2 m_designScreenSize;
    private bool m_levelBuilt;
    private LevelManager m_levelManager;

    public GameObject m_gridPfb; //prefab to instantiate the grid
    public GameObject m_contoursPfb; //prefab to instantiate contours
    public GameObject m_shapePfb; //prefab to instantiate a shape

    public enum ActionMode
    {
        SHAPES,
        SYMMETRY_AXIS,
        SYMMETRY_POINT
    };

    public ActionMode m_actionMode = ActionMode.SYMMETRY_AXIS;

    public enum GameStatus
    {
        RUNNING,
        PAUSED,
        VICTORY,
        DEFEAT
    };

    public const float GRID_Z_VALUE = -10.0f;
    public const float CONTOURS_Z_VALUE = -20.0f;
    public const float SHAPES_Z_VALUE = -30.0f;

    protected void Awake()
    {
        m_levelBuilt = false;
        m_levelManager = null;
    }

    protected void Start()
    {
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        m_levelManager = levelManagerObject.GetComponent<LevelManager>();
        m_levelManager.ParseLevelsFile();
        BuildLevel(1);

        TouchHandler.s_touchDeactivated = false;
    }

    protected void Update()
    {
        GameStatus gameStatus = GetGameStatus();
        if (gameStatus == GameStatus.VICTORY)
        {

        }
        else if (gameStatus == GameStatus.DEFEAT)
        {

        }
    }

    /**
     * Builds main menu (play button, options...)
     * **/
    public void BuildMainMenu()
    {
        
    }

    /**
     * Builds everything that appears on a screen for a certain level (GUI, grid, contours, shapes)
     * **/
    public void BuildLevel(int iLevelNumber)
    {
        if (!m_levelBuilt)
        {
            m_levelManager.SetCurrentLevelByNumber(iLevelNumber);
            BuildGrid();
            BuildGUI();
            BuildContours();
            BuildShapes();


            m_levelBuilt = true;
        }
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
        ShapesHolder shapesHolder = shapesObject.GetComponent<ShapesHolder>();
        List<Shape> allShapes = m_levelManager.m_currentLevel.m_shapes;
        foreach (Shape shape in allShapes)
        {
            //First triangulate the shape
            shape.Triangulate();

            //Then pass the data contained into a Shape object to a newly created ShapeRenderer object
            GameObject shapeObject = (GameObject) Instantiate(m_shapePfb);
            ShapeRenderer shapeRenderer = shapeObject.GetComponent<ShapeRenderer>();
            shapeRenderer.m_gridTriangles = shape.m_gridTriangles; //pass the array of grid triangles to the renderer
            //shapeRenderer.m_triangles = shape.m_triangles;
            shapeRenderer.m_color = shape.m_color; //pass the color of the shape to the renderer
            shapeRenderer.Render(null, ShapeRenderer.RenderFaces.DOUBLE_SIDED);

            shapeObject.transform.parent = shapesObject.transform;
            shapeObject.transform.localPosition = Vector3.zero;

            shapesHolder.AddShape(shapeObject);
        }

        shapesObject.transform.position = new Vector3(0, 0, SHAPES_Z_VALUE);
    }

    /**
     * Returns the current status of the game
     * **/
    public GameStatus GetGameStatus()
    {
        return GameStatus.RUNNING;
    }

    /**
     * Checks if the contour is filled exactly
     * Calculate the sum of the areas of contours and compare it to the area occupied by all the shapes
     * **/
    public bool IsVictory()
    {
        float contoursArea = 0;
        List<Contour> contours = m_levelManager.m_currentLevel.m_contours;
        for (int iContourIndex = 0; iContourIndex != contours.Count; iContourIndex++)
        {
            Contour contour = contours[iContourIndex];
            contoursArea += contour.m_area;
        }

        return false;
    }
}
