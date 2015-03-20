using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class GridBuilder : MonoBehaviour
{
    public List<GameObject> m_gridAnchors { get; set; }
    public List<GameObject> m_constraintGridAnchors { get; set; } //the grid anchors the players can use to draw axes
    public Vector2 m_gridSize { get; set; }
    public int m_minNumLines; //minimal number of lines in the grid
    public int m_minNumColumns; //minimal number of columns in the grid
    private int m_prevMinNumLines;
    private int m_prevMinNumColumns;
    public int m_numLines { get; set; }
    public int m_numColumns { get; set; }
    public float m_gridSpacing { get; set; }
    public GameObject m_gridAnchorPfb;
    public GameObject m_gridConstraintAnchorPfb;

    public void Awake()
    {
        m_gridAnchors = new List<GameObject>();
        m_constraintGridAnchors = new List<GameObject>();
    }

    public void Build()
    {
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

        anchorsHolder.AddComponent<GameObjectAnimator>();

        //get the screen viewport dimensions
        float fCameraSize = Camera.main.orthographicSize;
        float fScreenWidth = (float)Screen.width;
        float fScreenHeight = (float)Screen.height;
        float fScreenRatio = fScreenWidth / fScreenHeight;
        float fScreenHeightInUnits = 2.0f * fCameraSize;
        float fScreenWidthInUnits = fScreenRatio * fScreenHeightInUnits;

        //The grid occupies 85% of the screen height and 90% of the screen width
        m_gridSize = new Vector2(0.9f * fScreenWidthInUnits, 0.78f * fScreenHeightInUnits);
        float columnGridSpacing = m_gridSize.x / (float) (m_minNumColumns - 1);
        float lineGridSpacing = m_gridSize.y / (float) (m_minNumLines - 1);
        m_gridSpacing = Mathf.Min(columnGridSpacing, lineGridSpacing);
        m_numColumns = Mathf.FloorToInt(m_gridSize.x / m_gridSpacing) + 1;
        m_numLines = Mathf.FloorToInt(m_gridSize.y / m_gridSpacing) + 1;

        //set the position for the grid
        Vector2 gridPosition = new Vector2(0, -0.5f * m_gridSize.y + 0.5f * fScreenHeightInUnits - 0.17f * fScreenHeightInUnits);
        this.gameObject.transform.localPosition = gridPosition;

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

        Vector2 anchorLocalPosition = new Vector2(anchorPositionX, anchorPositionY);
        Vector2 anchorWorldPosition = anchorLocalPosition + GeometryUtils.BuildVector2FromVector3(gameObject.transform.position);
        return anchorWorldPosition;
    }

    /**
     * Calculates the grid coordinates (column, line) of a point knowing its world coordinates
     * **/
    public Vector2 GetGridCoordinatesFromWorldCoordinates(Vector2 worldCoordinates)
    {
        worldCoordinates -= GeometryUtils.BuildVector2FromVector3(gameObject.transform.position);

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
    public Vector2 GetClosestGridAnchorCoordinatesForPosition(Vector2 worldPosition)
    {
        Vector2 localPosition = worldPosition - GeometryUtils.BuildVector2FromVector3(gameObject.transform.position);

        float sqrMinDistance = float.MaxValue;
        int iMinDistanceAnchorIndex = -1;
        for (int iAnchorIndex = 0; iAnchorIndex != m_gridAnchors.Count; iAnchorIndex++)
        {
            Vector2 gridAnchorLocalPosition = m_gridAnchors[iAnchorIndex].transform.position - gameObject.transform.position;
            float dx = localPosition.x - gridAnchorLocalPosition.x;
            if (dx <= m_gridSpacing)
            {
                float dy = localPosition.y - gridAnchorLocalPosition.y;
                if (dy <= m_gridSpacing)
                {
                    //this anchor is one of the 4 anchors surrounding the position
                    float sqrDistance = (localPosition - gridAnchorLocalPosition).sqrMagnitude;
                    if (sqrDistance < sqrMinDistance)
                    {
                        sqrMinDistance = sqrDistance;
                        iMinDistanceAnchorIndex = iAnchorIndex;
                    }
                }
            }
        }

        if (iMinDistanceAnchorIndex >= 0)
            return GetGridCoordinatesFromWorldCoordinates(m_gridAnchors[iMinDistanceAnchorIndex].transform.position);
        else
            return Vector2.zero;
    }

    /**
     * Returns the list of anchors the player can draw an axis on knowing the position of the axis start point
     * For example on a vertical axis return the anchors on the same column as the start point
     * **/
    private void InvalidateConstraintAnchors(Vector2 gridPoint, Symmetrizer.SymmetryType symmetryType)
    {
        RemoveConstraintAnchors();

        GameObject constraintAnchorsHolder = new GameObject();
        constraintAnchorsHolder.name = "ConstraintAnchorsHolder";
        constraintAnchorsHolder.tag = "ConstraintAnchorsHolder";
        constraintAnchorsHolder.transform.parent = this.transform;
        constraintAnchorsHolder.transform.localPosition = Vector3.zero;

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
                m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, gridPoint.y)));
            }
        }
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_VERTICAL || bStraightAxes)
        {
            for (int iLineNumber = 1; iLineNumber != m_numLines + 1; iLineNumber++)
            {
                m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(gridPoint.x, iLineNumber)));
            }
        }
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_DIAGONAL_LEFT || bDiagonalAxes)
        {
            int iPointColumnNumber = Mathf.RoundToInt(gridPoint.x);
            int iPointLineNumber = Mathf.RoundToInt(gridPoint.y);

            //find anchors on the left of the reference anchor
            int belowLinesCount = iPointLineNumber - 1;
            int leftColumnsCount = iPointColumnNumber - 1;
            int minDimension = Mathf.Min(leftColumnsCount, belowLinesCount);
            if (minDimension == belowLinesCount)
            {
                for (int iLineNumber = iPointLineNumber - 1; iLineNumber != 0; iLineNumber--)
                {
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber - (iPointLineNumber - iLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber - 1; iColumnNumber != 0; iColumnNumber--)
                {
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber - (iPointColumnNumber - iColumnNumber))));
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
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber + (iLineNumber - iPointLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber + 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
                {
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber + (iColumnNumber - iPointColumnNumber))));
                }
            }
        }
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_DIAGONAL_RIGHT || bDiagonalAxes)
        {
            int iPointColumnNumber = Mathf.RoundToInt(gridPoint.x);
            int iPointLineNumber = Mathf.RoundToInt(gridPoint.y);

            //find anchors on the right of the reference anchor
            int belowLinesCount = iPointLineNumber - 1;
            int rightColumnsCount = m_numColumns - iPointColumnNumber;
            int minDimension = Mathf.Min(rightColumnsCount, belowLinesCount);
            if (minDimension == belowLinesCount)
            {
                for (int iLineNumber = iPointLineNumber - 1; iLineNumber != 0; iLineNumber--)
                {
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber + (iPointLineNumber - iLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber + 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
                {
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber - (iColumnNumber - iPointColumnNumber))));
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
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber - (iLineNumber - iPointLineNumber), iLineNumber)));
                }
            }
            else
            {
                for (int iColumnNumber = iPointColumnNumber - 1; iColumnNumber != 0; iColumnNumber--)
                {
                    m_constraintGridAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber + (iPointColumnNumber - iColumnNumber))));
                }
            }
        }
    }

    /**
     * Render the constraint anchors when user witch axis mode for instance
     * **/
    public void RenderConstraintAnchors(Vector2 gridPoint, Symmetrizer.SymmetryType symmetryType)
    {
        InvalidateConstraintAnchors(gridPoint, symmetryType);

        GameObject constraintAnchorsHolder = GameObject.FindGameObjectWithTag("ConstraintAnchorsHolder");
        for (int iAnchorIndex = 0; iAnchorIndex != m_constraintGridAnchors.Count; iAnchorIndex++)
        {
            GameObject anchor = m_constraintGridAnchors[iAnchorIndex];
            Vector3 anchorPosition = GeometryUtils.BuildVector3FromVector2(anchor.transform.position, -10);
            GameObject clonedConstraintAnchor = (GameObject) Instantiate(m_gridConstraintAnchorPfb, anchorPosition, Quaternion.identity);

            clonedConstraintAnchor.transform.parent = constraintAnchorsHolder.transform;
        }
    }

    /**
     * Remove all constraint anchors from scene
     * **/
    public void RemoveConstraintAnchors()
    {
        GameObject constraintAnchorsHolder = GameObject.FindGameObjectWithTag("ConstraintAnchorsHolder");
        if (constraintAnchorsHolder != null)
            DestroyImmediate(constraintAnchorsHolder);
        m_constraintGridAnchors.Clear();
    }

    public void Update()
    {
        if (m_prevMinNumColumns != m_minNumColumns || m_prevMinNumLines != m_minNumLines)
        {
            Build();

            m_prevMinNumColumns = m_minNumColumns;
            m_prevMinNumLines = m_minNumLines;
        }
    }
}
