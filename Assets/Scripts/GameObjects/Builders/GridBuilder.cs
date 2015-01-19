using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class GridBuilder : MonoBehaviour
{
    public float m_gridSpacing = 100.0f;
    private float m_prevGridSpacing;
    public List<GameObject> m_gridAnchors { get; set; }
    private int m_numLines; //number of lines in the grid
    private int m_numColumns; //number of columns in the grid

    public GameObject m_gridAnchorPfb;

    public void Awake()
    {
        m_prevGridSpacing = 0.0f;
        m_gridAnchors = new List<GameObject>();
    }

    public void Build()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        GameController gameController = gameControllerObject.GetComponent<GameController>();

        m_numLines = Mathf.FloorToInt(gameController.m_designScreenSize.y / m_gridSpacing);
        m_numColumns = Mathf.FloorToInt(gameController.m_designScreenSize.x / m_gridSpacing);

        for (int iLineNumber = 1; iLineNumber != m_numLines + 1; iLineNumber++)
        {
            for (int iColumnNumber = 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
            {
                float anchorPositionX, anchorPositionY;

                //find the x position of the anchor
                if (m_numColumns % 2 == 0) //even number of columns
                {
                    anchorPositionX = ((iColumnNumber - m_numColumns / 2 - 0.5f) * m_gridSpacing);
                }
                else //odd number of columns
                {
                    anchorPositionX = ((iColumnNumber - 1 - m_numColumns / 2) * m_gridSpacing);
                }

                //find the y position of the anchor
                if (m_numLines % 2 == 0) //even number of lines
                {
                    anchorPositionY = ((iLineNumber - m_numLines / 2 - 0.5f) * m_gridSpacing);
                }
                else //odd number of lines
                {
                    anchorPositionY = ((iLineNumber - 1 - m_numLines / 2) * m_gridSpacing);
                }

                Vector3 anchorLocalPosition = new Vector3(anchorPositionX, anchorPositionY, 0);
                GameObject clonedGridAnchor = (GameObject)Instantiate(m_gridAnchorPfb, anchorLocalPosition, Quaternion.identity);
                clonedGridAnchor.transform.parent = this.transform;
                clonedGridAnchor.transform.localPosition = anchorLocalPosition;
                m_gridAnchors.Add(clonedGridAnchor);
            }
        }
    }

    /**
     * Calculates the world coordinates of an anchor knowing its grid coordinates (line, column) 
     * **/
    public Vector2 GetAnchorWorldCoordinatesFromGridCoordinates(Vector2 gridCoordinates)
    {
        int targetAnchorIndex = (int) ((gridCoordinates.x - 1) * m_numColumns + (gridCoordinates.y - 1));
        GameObject targetAnchor = m_gridAnchors[targetAnchorIndex];

        return targetAnchor.transform.position;
    }

    /**
     * Calculates the grid coordinates (line, column) of an anchor knowing its world coordinates
     * **/
    public Vector2 GetAnchorGridCoordinatesFromWorldCoordinates(Vector2 worldCoordinates)
    {
        int iLineNumber;
        if (m_numLines % 2 == 0) //even number of lines
        {
            iLineNumber = (int) (worldCoordinates.y / m_gridSpacing + m_numLines / 2 + 0.5f);
        }
        else //odd number of lines
        {
            iLineNumber = (int) (worldCoordinates.y / m_gridSpacing + m_numLines / 2 + 1);
        }

        int iColNumber = 0;
        if (m_numColumns % 2 == 0) //even number of columns
        {
            iColNumber = (int) (worldCoordinates.x / m_gridSpacing + m_numColumns / 2 + 0.5f);
        }
        else //odd number of columns
        {
            iColNumber = (int)(worldCoordinates.x / m_gridSpacing + m_numColumns / 2 + 1);
        }

        return new Vector2(iLineNumber, iColNumber);
    }

    /**
     * Returns the grid anchor coordinates that is the closest to the position vector passed as parameter
     * **/
    public Vector2 GetClosestGridAnchorCoordinatesForPosition(Vector2 position)
    {
        float sqrMinDistance = float.MaxValue;
        int iMinDistanceAnchorIndex = -1;
        for (int iAnchorIndex = 0; iAnchorIndex != m_gridAnchors.Count; iAnchorIndex++)
        {
            Vector2 gridAnchorPosition = m_gridAnchors[iAnchorIndex].transform.position;
            float dx = position.x - gridAnchorPosition.x;
            if (dx <= m_gridSpacing)
            {
                float dy = position.y - gridAnchorPosition.y;
                if (dy <= m_gridSpacing)
                {
                    //this anchor is one of the 4 anchors surrounding the position
                    float sqrDistance = (position - gridAnchorPosition).sqrMagnitude;
                    if (sqrDistance < sqrMinDistance)
                    {
                        sqrMinDistance = sqrDistance;
                        iMinDistanceAnchorIndex = iAnchorIndex;
                    }
                }
            }
        }

        if (iMinDistanceAnchorIndex >= 0)
            return GetAnchorGridCoordinatesFromWorldCoordinates(m_gridAnchors[iMinDistanceAnchorIndex].transform.position);
        else
            return Vector2.zero;
    }

    protected void Update()
    {
        if (m_gridSpacing != m_prevGridSpacing)
        {
            m_prevGridSpacing = m_gridSpacing;
            foreach (GameObject gridAnchor in m_gridAnchors)
            {
                DestroyImmediate(gridAnchor);
            }
            m_gridAnchors.Clear();
            Build();
        }
    }
}
