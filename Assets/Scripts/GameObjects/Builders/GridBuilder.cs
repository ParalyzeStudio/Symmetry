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

        for (int iLineNumber = 0; iLineNumber != m_numLines; iLineNumber++)
        {
            for (int iColumnNumber = 0; iColumnNumber != m_numColumns; iColumnNumber++)
            {
                float anchorPositionX, anchorPositionY;

                //find the x position of the anchor
                if (m_numColumns % 2 == 0) //even number of columns
                {
                    anchorPositionX = ((iColumnNumber + 1 - m_numColumns / 2 - 0.5f) * m_gridSpacing);
                }
                else //odd number of columns
                {
                    anchorPositionX = ((iColumnNumber - m_numColumns / 2) * m_gridSpacing);
                }

                //find the y position of the anchor
                if (m_numLines % 2 == 0) //even number of lines
                {
                    anchorPositionY = ((iLineNumber + 1 - m_numLines / 2 - 0.5f) * m_gridSpacing);
                }
                else //odd number of columns
                {
                    anchorPositionY = ((iLineNumber - m_numLines / 2) * m_gridSpacing);
                }

                Vector3 anchorLocalPosition = new Vector3(anchorPositionX, anchorPositionY, 0);
                GameObject clonedGridAnchor = (GameObject)Instantiate(m_gridAnchorPfb, anchorLocalPosition, Quaternion.identity);
                clonedGridAnchor.transform.parent = this.transform;
                clonedGridAnchor.transform.localPosition = anchorLocalPosition;
                m_gridAnchors.Add(clonedGridAnchor);
            }
        }
    }

    public Vector2 GetAnchorWorldCoordinatesFromGridCoordinates(Vector2 gridCoordinates)
    {
        int targetAnchorIndex = (int) ((gridCoordinates.x - 1) * m_numColumns + (gridCoordinates.y - 1));
        GameObject targetAnchor = m_gridAnchors[targetAnchorIndex];

        return targetAnchor.transform.position;
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
