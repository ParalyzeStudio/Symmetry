using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    public List<GameObject> m_anchors { get; set; }
    public Vector2 m_gridSize { get; set; }
    public int m_numLines { get; set; }
    public int m_numColumns { get; set; }
    public float m_gridSpacing { get; set; }
    public GameObject m_gridAnchorPfb;
    public GameObject m_gridConstraintAnchorPfb;

    private Color m_gridColor;

    public void Awake()
    {
        m_anchors = new List<GameObject>();
    }

    public void Build()
    {
        m_gridColor = Color.black;

        //Get the number of min lines and min columns we want for this level
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Level currentLevel = levelManager.m_currentLevel;
        int minNumLines = currentLevel.m_gridMinNumLines;
        int minNumColumns = currentLevel.m_gridMinNumColumns;
        int exactNumLines = currentLevel.m_gridExactNumLines;
        int exactNumColumns = currentLevel.m_gridExactNumColumns;

        //get the screen viewport dimensions
        float fCameraSize = Camera.main.orthographicSize;
        float fScreenWidth = (float)Screen.width;
        float fScreenHeight = (float)Screen.height;
        float fScreenRatio = fScreenWidth / fScreenHeight;
        float fScreenHeightInUnits = 2.0f * fCameraSize;
        float fScreenWidthInUnits = fScreenRatio * fScreenHeightInUnits;

        //The grid occupies 85% of the screen height and 90% of the screen width
        m_gridSize = new Vector2(0.9f * fScreenWidthInUnits, 0.78f * fScreenHeightInUnits);
        float lineGridSpacing, columnGridSpacing;
        if (exactNumLines > 0)
            lineGridSpacing = m_gridSize.y / (float)(exactNumLines - 1);
        else
            lineGridSpacing = m_gridSize.y / (float)(minNumLines - 1);

        if (exactNumColumns > 0)
            columnGridSpacing = m_gridSize.x / (float)(exactNumColumns - 1);
        else
            columnGridSpacing = m_gridSize.x / (float)(minNumColumns - 1);

        m_gridSpacing = Mathf.Min(lineGridSpacing, columnGridSpacing, currentLevel.m_maxGridSpacing);

        m_numLines = (exactNumLines > 0) ? exactNumLines : Mathf.FloorToInt(m_gridSize.y / m_gridSpacing) + 1;
        m_numColumns = (exactNumColumns > 0) ? exactNumColumns : Mathf.FloorToInt(m_gridSize.x / m_gridSpacing) + 1;

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
                clonedGridAnchor.transform.parent = this.transform;
                clonedGridAnchor.transform.localPosition = anchorLocalPosition;

                UVQuad gridAnchorQuad = clonedGridAnchor.GetComponent<UVQuad>();
                gridAnchorQuad.InitQuadMesh();
                TexturedQuadAnimator anchorAnimator = clonedGridAnchor.GetComponent<TexturedQuadAnimator>();
                anchorAnimator.SetColor(m_gridColor);
                m_anchors.Add(clonedGridAnchor);
            }
        }
    }

    /**
     * Fades out the grid (when a level ends for instance)
     * **/
    public void Dismiss(float fDuration, float fDelay)
    {
        GameObjectAnimator gridAnimator = this.GetComponent<GameObjectAnimator>();
        gridAnimator.FadeTo(0, fDuration, fDelay);
    }

    /**
     * Calculates the world coordinates of a point knowing its grid coordinates (column, line) 
     * **/
    public Vector2 GetPointWorldCoordinatesFromGridCoordinates(Vector2 gridCoordinates)
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
    public Vector2 GetPointGridCoordinatesFromWorldCoordinates(Vector2 worldCoordinates)
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
     * Returns the ratio between 1 unit in grid coordinates and 1 unit in world coordinates
     * **/
    public float GetGridWorldRatio()
    {
        return 1 / m_gridSpacing;
    }

    public Vector2 TransformGridVectorToWorldVector(Vector2 gridVector)
    {
        return gridVector / GetGridWorldRatio();
    }

    public Vector2 TransformWorldVectorToGridVector(Vector2 worldVector)
    {
        return worldVector * GetGridWorldRatio();
    }

    /**
     * Returns the anchor game object at grid position passed as parameter
     * **/
    public GameObject GetAnchorAtGridPosition(Vector2 gridPosition)
    {
        int iColumnNumber = Mathf.RoundToInt(gridPosition.x);
        int iLineNumber = Mathf.RoundToInt(gridPosition.y);

        return m_anchors[(iLineNumber - 1) * m_numColumns + (iColumnNumber - 1)];
    }

    /**
     * Returns the grid coordinates of the anchor passed as parameter
     * **/
    public Vector2 GetAnchorGridCoordinates(GameObject anchor)
    {
        int iAnchorIndex = -1;
        for (int i = 0; i != m_anchors.Count; i++)
        {
            if (m_anchors[i] == anchor)
            {
                iAnchorIndex = i;
                break;
            }
        }

        return GetAnchorGridCoordinatesForAnchorIndex(iAnchorIndex);
    }

    /**
     * Returns the grid coordinates of the anchor whose index is passed as parameter
     * **/
    public Vector2 GetAnchorGridCoordinatesForAnchorIndex(int iAnchorIndex)
    {
        if (iAnchorIndex < 0 || iAnchorIndex >= m_anchors.Count)
            return Vector2.zero;

        return new Vector2(iAnchorIndex % m_numColumns + 1, iAnchorIndex / m_numColumns + 1);
    }

    /**
     * Returns the grid anchor coordinates that is the closest to the position vector passed as parameter
     * **/
    public Vector2 GetClosestGridAnchorCoordinatesForPosition(Vector2 worldPosition)
    {
        Vector2 localPosition = worldPosition - GeometryUtils.BuildVector2FromVector3(gameObject.transform.position);

        float sqrMinDistance = float.MaxValue;
        int iMinDistanceAnchorIndex = -1;
        for (int iAnchorIndex = 0; iAnchorIndex != m_anchors.Count; iAnchorIndex++)
        {
            Vector2 gridAnchorLocalPosition = m_anchors[iAnchorIndex].transform.position - gameObject.transform.position;
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
            return GetPointGridCoordinatesFromWorldCoordinates(m_anchors[iMinDistanceAnchorIndex].transform.position);
        else
            return Vector2.zero;
    }

    ///**
    // * Returns the list of anchors the player can draw an axis on knowing the position of the axis start point
    // * For example on a vertical axis return the anchors on the same column as the start point
    // * **/
    //private void InvalidateConstraintAnchors(Vector2 gridPoint, Symmetrizer.SymmetryType symmetryType)
    //{
    //    RemoveConstraintAnchors();

    //    GameObject constraintAnchorsHolder = new GameObject();
    //    constraintAnchorsHolder.name = "ConstraintAnchorsHolder";
    //    constraintAnchorsHolder.tag = "ConstraintAnchorsHolder";
    //    constraintAnchorsHolder.transform.parent = this.transform;
    //    constraintAnchorsHolder.transform.localPosition = Vector3.zero;

    //    bool bStraightAxes = false;
    //    bool bDiagonalAxes = false;
    //    if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_STRAIGHT)
    //        bStraightAxes = true;

    //    if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_DIAGONALS)
    //        bDiagonalAxes = true;

    //    if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_ALL)
    //    {
    //        bStraightAxes = true;
    //        bDiagonalAxes = true;
    //    }

    //    if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_HORIZONTAL || bStraightAxes)
    //    {
    //        for (int iColumnNumber = 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
    //        {
    //            m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, gridPoint.y)));
    //        }
    //    }
    //    if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_VERTICAL || bStraightAxes)
    //    {
    //        for (int iLineNumber = 1; iLineNumber != m_numLines + 1; iLineNumber++)
    //        {
    //            m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(gridPoint.x, iLineNumber)));
    //        }
    //    }
    //    if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_DIAGONAL_LEFT || bDiagonalAxes)
    //    {
    //        int iPointColumnNumber = Mathf.RoundToInt(gridPoint.x);
    //        int iPointLineNumber = Mathf.RoundToInt(gridPoint.y);

    //        //find anchors on the left of the reference anchor
    //        int belowLinesCount = iPointLineNumber - 1;
    //        int leftColumnsCount = iPointColumnNumber - 1;
    //        int minDimension = Mathf.Min(leftColumnsCount, belowLinesCount);
    //        if (minDimension == belowLinesCount)
    //        {
    //            for (int iLineNumber = iPointLineNumber - 1; iLineNumber != 0; iLineNumber--)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber - (iPointLineNumber - iLineNumber), iLineNumber)));
    //            }
    //        }
    //        else
    //        {
    //            for (int iColumnNumber = iPointColumnNumber - 1; iColumnNumber != 0; iColumnNumber--)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber - (iPointColumnNumber - iColumnNumber))));
    //            }
    //        }

    //        //and on the right
    //        int aboveLinesCount = m_numLines - iPointLineNumber;
    //        int rightColumnsCount = m_numColumns - iPointColumnNumber;
    //        minDimension = Mathf.Min(rightColumnsCount, aboveLinesCount);
    //        if (minDimension == aboveLinesCount)
    //        {
    //            for (int iLineNumber = iPointLineNumber + 1; iLineNumber != m_numLines + 1; iLineNumber++)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber + (iLineNumber - iPointLineNumber), iLineNumber)));
    //            }
    //        }
    //        else
    //        {
    //            for (int iColumnNumber = iPointColumnNumber + 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber + (iColumnNumber - iPointColumnNumber))));
    //            }
    //        }
    //    }
    //    if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXIS_DIAGONAL_RIGHT || bDiagonalAxes)
    //    {
    //        int iPointColumnNumber = Mathf.RoundToInt(gridPoint.x);
    //        int iPointLineNumber = Mathf.RoundToInt(gridPoint.y);

    //        //find anchors on the right of the reference anchor
    //        int belowLinesCount = iPointLineNumber - 1;
    //        int rightColumnsCount = m_numColumns - iPointColumnNumber;
    //        int minDimension = Mathf.Min(rightColumnsCount, belowLinesCount);
    //        if (minDimension == belowLinesCount)
    //        {
    //            for (int iLineNumber = iPointLineNumber - 1; iLineNumber != 0; iLineNumber--)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber + (iPointLineNumber - iLineNumber), iLineNumber)));
    //            }
    //        }
    //        else
    //        {
    //            for (int iColumnNumber = iPointColumnNumber + 1; iColumnNumber != m_numColumns + 1; iColumnNumber++)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber - (iColumnNumber - iPointColumnNumber))));
    //            }
    //        }

    //        //and on the left
    //        int aboveLinesCount = m_numLines - iPointLineNumber;
    //        int leftColumnsCount = iPointColumnNumber - 1;
    //        minDimension = Mathf.Min(leftColumnsCount, aboveLinesCount);
    //        if (minDimension == aboveLinesCount)
    //        {
    //            for (int iLineNumber = iPointLineNumber + 1; iLineNumber != m_numLines + 1; iLineNumber++)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iPointColumnNumber - (iLineNumber - iPointLineNumber), iLineNumber)));
    //            }
    //        }
    //        else
    //        {
    //            for (int iColumnNumber = iPointColumnNumber - 1; iColumnNumber != 0; iColumnNumber--)
    //            {
    //                m_constraintAnchors.Add(GetAnchorAtGridPosition(new Vector2(iColumnNumber, iPointLineNumber + (iPointColumnNumber - iColumnNumber))));
    //            }
    //        }
    //    }
    //}

    ///**
    // * Render the constraint anchors when user witch axis mode for instance
    // * **/
    //public void RenderConstraintAnchors(Vector2 gridPoint, Symmetrizer.SymmetryType symmetryType)
    //{
    //    InvalidateConstraintAnchors(gridPoint, symmetryType);

    //    GameObject constraintAnchorsHolder = GameObject.FindGameObjectWithTag("ConstraintAnchorsHolder");
    //    for (int iAnchorIndex = 0; iAnchorIndex != m_constraintAnchors.Count; iAnchorIndex++)
    //    {
    //        GameObject anchor = m_constraintAnchors[iAnchorIndex];
    //        Vector3 anchorPosition = GeometryUtils.BuildVector3FromVector2(anchor.transform.position, -10);
    //        GameObject clonedConstraintAnchor = (GameObject) Instantiate(m_gridConstraintAnchorPfb, anchorPosition, Quaternion.identity);

    //        clonedConstraintAnchor.transform.parent = constraintAnchorsHolder.transform;
    //    }
    //}

    ///**
    // * Remove all constraint anchors from scene
    // * **/
    //public void RemoveConstraintAnchors()
    //{
    //    GameObject constraintAnchorsHolder = GameObject.FindGameObjectWithTag("ConstraintAnchorsHolder");
    //    if (constraintAnchorsHolder != null)
    //        DestroyImmediate(constraintAnchorsHolder);
    //    m_constraintAnchors.Clear();
    //}
}
