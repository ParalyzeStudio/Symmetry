using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GameController : MonoBehaviour
{
    public Vector2 m_designScreenSize;
    public float m_gridSpacing = 100.0f;
    private float m_prevGridSpacing;
    private int m_currentLevelNumber;
    private bool m_levelBuilt;

    public GameObject m_gridAnchorPfb;

    protected void Awake()
    {
        m_levelBuilt = false;
        m_prevGridSpacing = 0.0f;
    }

    protected void Start()
    {
        Debug.Log("START");
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        LevelManager levelManager = (LevelManager)levelManagerObject.GetComponent<LevelManager>();
        levelManager.ParseLevelsFile();
        BuildLevel(1);
    }

    protected void Update()
    {
        if (m_gridSpacing != m_prevGridSpacing)
        {
            m_prevGridSpacing = m_gridSpacing;
            Debug.Log("UPDATE EDITOR");
            GameObject[] grids = GameObject.FindGameObjectsWithTag("Grid");
            foreach (GameObject grid in grids)
            {
                DestroyImmediate(grid);
            }
            BuildGrid();
        }
        //if (Application.isEditor)
        //{
        //    Debug.Log("UPDATE");

        //    DestroyImmediate(m_grid);
        //    BuildGrid();
        //}
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
            Debug.Log("BUILDING LEVEL");
            m_currentLevelNumber = iLevelNumber;
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
        float fGridZValue = -10.0f;
        GameObject grid = new GameObject("Grid");
        grid.transform.position = new Vector3(0, 0, fGridZValue);
        grid.tag = "Grid";

        int numLines = Mathf.FloorToInt(m_designScreenSize.y / m_gridSpacing);
        int numColumns = Mathf.FloorToInt(m_designScreenSize.x / m_gridSpacing);


        for (int iLineNumber = 0; iLineNumber != numLines; iLineNumber++)
        {
            for (int iColumnNumber = 0; iColumnNumber != numColumns; iColumnNumber++)
            {
                float anchorPositionX, anchorPositionY;

                //find the x position of the anchor
                if (numColumns % 2 == 0) //even number of columns
                {
                    anchorPositionX = ((iColumnNumber + 1 - numColumns / 2 - 0.5f) * m_gridSpacing);
                }
                else //odd number of columns
                {
                    anchorPositionX = ((iColumnNumber - numColumns / 2) * m_gridSpacing);
                }

                //find the y position of the anchor
                if (numLines % 2 == 0) //even number of lines
                {
                    anchorPositionY = ((iLineNumber + 1 - numLines / 2 - 0.5f) * m_gridSpacing);
                }
                else //odd number of columns
                {
                    anchorPositionY = ((iLineNumber - numLines / 2) * m_gridSpacing);
                }

                Vector3 anchorLocalPosition = new Vector3(anchorPositionX, anchorPositionY, 0);
                GameObject clonedGridAnchor = (GameObject)Instantiate(m_gridAnchorPfb, anchorLocalPosition, Quaternion.identity);
                clonedGridAnchor.transform.parent = grid.transform;
                clonedGridAnchor.transform.localPosition = anchorLocalPosition;
            }
        }
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

    }

    /**
     * We set the shapes the player initally starts with
     * **/
    private void BuildShapes()
    {

    }
}
