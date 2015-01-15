using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public Vector2 m_designScreenSize;
    public float m_gridSpacing = 100.0f;
    private int m_currentLevelNumber;
    private bool m_levelBuilt;

    public GameObject m_gridAnchorPfb;

    protected void Awake()
    {
        m_levelBuilt = false;
    }

    protected void Start()
    {
        BuildLevel(1);
    }

    protected void Update()
    {

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

    private void BuildGrid()
    {
        float fGridZValue = -10.0f;
        GameObject grid = new GameObject("Grid");
        grid.transform.position = new Vector3(0, 0, fGridZValue);

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

    private void BuildGUI()
    {

    }

    private void BuildContour()
    {

    }

    private void BuildShapes()
    {

    }
}
