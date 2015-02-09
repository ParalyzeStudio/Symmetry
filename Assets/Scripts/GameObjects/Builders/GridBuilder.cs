using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class GridBuilder : MonoBehaviour
{
    public float m_gridSpacing = 100.0f;
    private float m_prevGridSpacing;
    public float m_gridAnchorUniformScaling = 16.0f;
    private float m_prevGridAnchorUniformScaling = 1.0f;
    public List<GameObject> m_gridAnchors { get; set; }
    public int m_numLines { get; set; } //number of lines in the grid
    public int m_numColumns { get; set; } //number of columns in the grid

    public GameObject m_gridAnchorPfb;

    public void Awake()
    {
        m_prevGridSpacing = 0.0f;
        m_prevGridAnchorUniformScaling = 0.0f;
        m_gridAnchors = new List<GameObject>();
    }

    public void Build()
    {
        m_prevGridSpacing = m_gridSpacing;

        //fresh build destroy a previously created anchorsHolder and clear the vector of gridAnchors
        GameObject anchorsHolder = GameObject.FindGameObjectWithTag("AnchorsHolder");
        if (anchorsHolder != null)
            DestroyImmediate(anchorsHolder);
        m_gridAnchors.Clear();

        anchorsHolder = new GameObject();
        anchorsHolder.name = "AnchorsHolder";
        anchorsHolder.tag = "AnchorsHolder";
        anchorsHolder.transform.parent = this.transform;
        anchorsHolder.transform.localPosition = Vector3.zero;

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
                clonedGridAnchor.transform.parent = anchorsHolder.transform;
                clonedGridAnchor.transform.localPosition = anchorLocalPosition;
                m_gridAnchors.Add(clonedGridAnchor);
            }
        }
    }

    /**
     * Calculates the world coordinates of a point knowing its grid coordinates (column, line) 
     * **/
    public Vector2 GetWorldCoordinatesFromGridCoordinates(Vector2 gridCoordinates)
    {
        float anchorPositionX;
        if (m_numColumns % 2 == 0) //even number of columns
        {
            anchorPositionX = ((gridCoordinates.x - m_numColumns / 2 - 0.5f) * m_gridSpacing);
        }
        else //odd number of columns
        {
            anchorPositionX = ((gridCoordinates.x - 1 - m_numColumns / 2) * m_gridSpacing);
        }
        float anchorPositionY;
        if (m_numLines % 2 == 0) //even number of lines
        {
            anchorPositionY = ((gridCoordinates.y - m_numLines / 2 - 0.5f) * m_gridSpacing);
        }
        else //odd number of lines
        {
            anchorPositionY = ((gridCoordinates.y - 1 - m_numLines / 2) * m_gridSpacing);
        }

        return new Vector2(anchorPositionX, anchorPositionY);
    }

    /**
     * Calculates the grid coordinates (column, line) of a point knowing its world coordinates
     * **/
    public Vector2 GetGridCoordinatesFromWorldCoordinates(Vector2 worldCoordinates)
    {
        float iLineNumber;
        if (m_numLines % 2 == 0) //even number of lines
        {
            iLineNumber = worldCoordinates.y / m_gridSpacing + m_numLines / 2 + 0.5f;
        }
        else //odd number of lines
        {
            iLineNumber = worldCoordinates.y / m_gridSpacing + m_numLines / 2 + 1;
        }

        float iColNumber = 0;
        if (m_numColumns % 2 == 0) //even number of columns
        {
            iColNumber = worldCoordinates.x / m_gridSpacing + m_numColumns / 2 + 0.5f;
        }
        else //odd number of columns
        {
            iColNumber = worldCoordinates.x / m_gridSpacing + m_numColumns / 2 + 1;
        }

        return new Vector2(iColNumber, iLineNumber);
    }

    /**
     * Returns the anchor game object at grid position passed as parameter
     * **/
    public GameObject GetAnchorAtGridPosition(Vector2 gridPosition)
    {
        int iColumnNumber = Mathf.RoundToInt(gridPosition.x);
        int iLineNumber = Mathf.RoundToInt(gridPosition.y);

        return m_gridAnchors[(iLineNumber - 1) * m_numColumns + (iColumnNumber - 1)];
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
        {
            Vector2 gridCoordinates = GetGridCoordinatesFromWorldCoordinates(m_gridAnchors[iMinDistanceAnchorIndex].transform.position);
            return GetGridCoordinatesFromWorldCoordinates(m_gridAnchors[iMinDistanceAnchorIndex].transform.position);
        }
        else
            return Vector2.zero;
    }

    /**
     * Returns the list of anchors the player can draw an axis on knowing the position of the axis start point
     * For example on a vertical axis return the anchors on the same column as the start point
     * **/
    public List<GameObject> GetAnchorsConstrainedBySymmetryType(Vector2 point, Symmetrizer.SymmetryType symmetryType)
    {
        List<GameObject> anchors = new List<GameObject>();

        bool bStraightAxes = false;
        bool bDiagonalAxes = false;
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_STRAIGHT)
            bStraightAxes = true;

        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_DIAGONALS)
            bDiagonalAxes = true;

        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_ALL)
        {
            bStraightAxes = true;
            bDiagonalAxes = true;
        }

        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_HORIZONTAL || bStraightAxes)
        {
            for (int iColumnNumber = 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
            {
                anchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, point.y)));
            }
        }
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_VERTICAL || bStraightAxes)
        {
            for (int iLineNumber = 1; iLineNumber != m_numLines + 1; iLineNumber++)
            {
                anchors.Add(GetAnchorAtGridPosition(new Vector2(point.x, iLineNumber)));
            }
        }
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_DIAGONAL_BOTTOM_LEFT || bDiagonalAxes)
        {
            int iPointColumnNumber = Mathf.RoundToInt(point.x);
            int iPointLineNumber = Mathf.RoundToInt(point.y);

            //find anchors on the left of the reference anchor
            int belowLinesCount = iPointLineNumber - 1;
            int leftColumnsCount = iPointColumnNumber - 1;
            int minDimension = Mathf.Min(leftColumnsCount, belowLinesCount);
            if (minDimension == belowLinesCount)
            {
                for (int iLineNumber = iPointLineNumber - 1; iLineNumber != 0; iLineNumber--)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber - (iPointLineNumber - iLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber - 1; iColumnNumber != 0; iColumnNumber--)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber - (iPointColumnNumber - iColumnNumber))));
                }
            }

            //and on the right
            int aboveLinesCount = m_numLines - iPointLineNumber;
            int rightColumnsCount = m_numColumns - iPointColumnNumber;
            minDimension = Mathf.Min(rightColumnsCount, aboveLinesCount);
            if (minDimension == aboveLinesCount)
            {
                for (int iLineNumber = iPointLineNumber + 1; iLineNumber != m_numLines + 1; iLineNumber++)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber + (iLineNumber - iPointLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber + 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber + (iColumnNumber - iPointColumnNumber))));
                }
            }
        }
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_DIAGONAL_TOP_LEFT || bDiagonalAxes)
        {
            int iPointColumnNumber = Mathf.RoundToInt(point.x);
            int iPointLineNumber = Mathf.RoundToInt(point.y);

            //find anchors on the right of the reference anchor
            int belowLinesCount = iPointLineNumber - 1;
            int rightColumnsCount = m_numColumns - iPointColumnNumber;
            int minDimension = Mathf.Min(rightColumnsCount, belowLinesCount);
            if (minDimension == belowLinesCount)
            {
                for (int iLineNumber = iPointLineNumber - 1; iLineNumber != 0; iLineNumber--)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber + (iPointLineNumber - iLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber + 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber - (iColumnNumber - iPointColumnNumber))));
                }
            }

            //and on the left
            int aboveLinesCount = m_numLines - iPointLineNumber;
            int leftColumnsCount = iPointColumnNumber - 1;
            minDimension = Mathf.Min(leftColumnsCount, aboveLinesCount);
            if (minDimension == aboveLinesCount)
            {
                for (int iLineNumber = iPointLineNumber + 1; iLineNumber != m_numLines + 1; iLineNumber++)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber - (iLineNumber - iPointLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber - 1; iColumnNumber != 0; iColumnNumber--)
                {
                    anchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber + (iPointColumnNumber - iColumnNumber))));
                }
            }
        }

        return anchors;
    }

    protected void Update()
    {
        if (m_gridSpacing != m_prevGridSpacing)
        {
            m_prevGridSpacing = m_gridSpacing;

            Build();
        }
        if (m_gridAnchorUniformScaling != m_prevGridAnchorUniformScaling)
        {
            m_prevGridAnchorUniformScaling = m_gridAnchorUniformScaling;
        }
    }
}
