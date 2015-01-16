using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Vector2 m_designScreenSize;
    private bool m_levelBuilt;
    private LevelManager m_levelManager;

    public GameObject m_gridPfb; //prefab to instantiate the grid
    public GameObject m_contoursPfb; //prefab to instantiate contours

    public const float GRID_Z_VALUE = -10.0f;
    public const float CONTOURS_Z_VALUE = -20.0f;

    protected void Awake()
    {
        m_levelBuilt = false;
        m_levelManager = null;
    }

    protected void Start()
    {
        Debug.Log("START");
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        m_levelManager = levelManagerObject.GetComponent<LevelManager>();
        m_levelManager.ParseLevelsFile();
        BuildLevel(1);
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
            BuildContour();
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
        GameObject grid = (GameObject) Instantiate(m_gridPfb);
        grid.transform.position = new Vector3(0, 0, GRID_Z_VALUE);
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
    private void BuildContour()
    {
        GameObject contours = (GameObject)Instantiate(m_contoursPfb);
        contours.transform.position = new Vector3(0, 0, CONTOURS_Z_VALUE);
    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void BuildShapes()
    {

    }
}
