using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    //Shared prefabs
    public GameObject m_circleMeshPfb;
    public Material m_positionColorMaterial;
    private Material m_gridAnchorMaterial;

    public class GridAnchor
    {
        public GridPoint m_gridPosition { get; set; } //the position of the anchor in local grid coordinates (column, line)
        public Vector2 m_worldPosition { get; set; } //the position of the anchor in world coordinates

        public GridAnchor(GridPoint gridPosition, Vector2 worldPosition)
        {
            m_worldPosition = worldPosition;
            m_gridPosition = gridPosition;
        }
    }

    public GridAnchor[] m_anchors { get; set; }
    public Vector2 m_gridSize { get; set; }
    public int m_numLines { get; set; }
    public int m_numColumns { get; set; }
    public float m_gridSpacing { get; set; }
    //public GameObject m_gridConstraintAnchorPfb;

    public enum GridBoxEdgeLocation
    {
        NONE = 0,
        LEFT,
        BOTTOM,
        RIGHT,
        TOP
    }

    public void Build()
    {
        GameObject pointsHolder = new GameObject("Points");

        GameObjectAnimator pointsHolderAnimator = pointsHolder.AddComponent<GameObjectAnimator>();
        pointsHolderAnimator.SetParentTransform(this.transform);
        pointsHolderAnimator.SetPosition(Vector3.zero);

        //Get the number of min lines and min columns we want for this level
        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        Level currentLevel = levelManager.m_currentLevel;
        int minNumLines = currentLevel.m_gridMinNumLines;
        int minNumColumns = currentLevel.m_gridMinNumColumns;
        int exactNumLines = currentLevel.m_gridExactNumLines;
        int exactNumColumns = currentLevel.m_gridExactNumColumns;
        int anchorsCount = exactNumLines * exactNumColumns;
        m_anchors = new GridAnchor[anchorsCount];

        //get the screen viewport dimensions
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //The grid occupies at maximum 78% of the screen height and 75% of the screen width
        m_gridSize = new Vector2(0.75f * screenSize.x, 0.78f * screenSize.y);
        float lineGridSpacing, columnGridSpacing;
        if (exactNumLines > 0)
            lineGridSpacing = m_gridSize.y / (float)(exactNumLines - 1);
        else
            lineGridSpacing = m_gridSize.y / (float)(minNumLines - 1);

        if (exactNumColumns > 0)
            columnGridSpacing = m_gridSize.x / (float)(exactNumColumns - 1);
        else
            columnGridSpacing = m_gridSize.x / (float)(minNumColumns - 1);

        if (currentLevel.m_maxGridSpacing > 0)
            m_gridSpacing = Mathf.Min(lineGridSpacing, columnGridSpacing, currentLevel.m_maxGridSpacing);
        else
            m_gridSpacing = Mathf.Min(lineGridSpacing, columnGridSpacing);

        m_numLines = (exactNumLines > 0) ? exactNumLines : Mathf.FloorToInt(m_gridSize.y / m_gridSpacing) + 1;
        m_numColumns = (exactNumColumns > 0) ? exactNumColumns : Mathf.FloorToInt(m_gridSize.x / m_gridSpacing) + 1;

        m_gridSize = new Vector2((m_numColumns - 1) * m_gridSpacing, (m_numLines - 1) * m_gridSpacing);

        Vector3 gridPosition = this.transform.position;

        //Build anchors
        m_gridAnchorMaterial = Instantiate(m_positionColorMaterial);

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

                int gridPositionScale = GridPoint.DEFAULT_SCALE_PRECISION;
                GridPoint anchorGridPosition = new GridPoint(iColumnNumber, iLineNumber);
                anchorGridPosition.Scale(gridPositionScale);
                Vector3 anchorPosition = new Vector3(anchorPositionX, anchorPositionY, 0);
                GameObject clonedGridAnchor = (GameObject)Instantiate(m_circleMeshPfb);

                CircleMesh gridAnchorQuad = clonedGridAnchor.GetComponent<CircleMesh>();
                gridAnchorQuad.Init(m_gridAnchorMaterial);
                int iAnchorIndex = (iLineNumber - 1) * exactNumColumns + (iColumnNumber - 1);

                CircleMeshAnimator anchorAnimator = clonedGridAnchor.GetComponent<CircleMeshAnimator>();
                anchorAnimator.SetParentTransform(pointsHolder.transform);
                anchorAnimator.SetNumSegments(4, false);
                anchorAnimator.SetInnerRadius(0, false);
                anchorAnimator.SetOuterRadius(4, true);
                anchorAnimator.SetPosition(anchorPosition);
                anchorAnimator.SetColor(Color.white);

                m_anchors[iAnchorIndex] = new GridAnchor(anchorGridPosition, anchorPosition + gridPosition);
            }
        }
    }

    /**
     * Fades out the grid (when a level ends for instance)
     * **/
    public void Dismiss(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator gridAnimator = this.GetComponent<GameObjectAnimator>();
        gridAnimator.FadeTo(0, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Calculates the world coordinates of a point knowing its grid coordinates (column, line) 
     * **/
    public Vector2 GetPointWorldCoordinatesFromGridCoordinates(GridPoint gridPoint)
    {
        Vector2 columnLinePoint = gridPoint.GetAsColumnLineVector();

        float anchorPositionX;
        if (m_numColumns % 2 == 0) //even number of columns
        {
            anchorPositionX = ((columnLinePoint.x - m_numColumns / 2 - 0.5f) * m_gridSpacing);
        }
        else //odd number of columns
        {
            anchorPositionX = ((columnLinePoint.x - 1 - m_numColumns / 2) * m_gridSpacing);
        }
        float anchorPositionY;
        if (m_numLines % 2 == 0) //even number of lines
        {
            anchorPositionY = ((columnLinePoint.y - m_numLines / 2 - 0.5f) * m_gridSpacing);
        }
        else //odd number of lines
        {
            anchorPositionY = ((columnLinePoint.y - 1 - m_numLines / 2) * m_gridSpacing);
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
     * Returns the grid coordinates of the anchor whose index is passed as parameter
     * **/
    public Vector2 GetAnchorGridCoordinatesForAnchorIndex(int iAnchorIndex)
    {
        if (iAnchorIndex < 0 || iAnchorIndex >= m_anchors.Length)
            return new Vector2(0, 0);

        return new Vector2(iAnchorIndex % m_numColumns + 1, iAnchorIndex / m_numColumns + 1);
    }

    /**
     * Returns the grid anchor coordinates that is the closest to the position vector passed as parameter
     * **/
    public GridAnchor GetClosestGridAnchorForWorldPosition(Vector2 worldPosition)
    {
        float sqrMinDistance = float.MaxValue;
        int iMinDistanceAnchorIndex = -1;
        
        for (int iAnchorIndex = 0; iAnchorIndex != m_anchors.Length; iAnchorIndex++)
        {
            Vector2 gridAnchorWorldPosition = m_anchors[iAnchorIndex].m_worldPosition;
            float dx = worldPosition.x - gridAnchorWorldPosition.x;
            if (dx <= m_gridSpacing)
            {
                float dy = worldPosition.y - gridAnchorWorldPosition.y;
                if (dy <= m_gridSpacing)
                {
                    //this anchor is one of the 4 anchors surrounding the position
                    float sqrDistance = (worldPosition - gridAnchorWorldPosition).sqrMagnitude;
                    if (sqrDistance < sqrMinDistance)
                    {
                        sqrMinDistance = sqrDistance;
                        iMinDistanceAnchorIndex = iAnchorIndex;
                    }
                }
            }
        }

        if (iMinDistanceAnchorIndex >= 0)
            return m_anchors[iMinDistanceAnchorIndex];
        else
            return null;
    }

    public struct GridBoxPoint
    {
        public GridBoxPoint(GridPoint position, GridBoxEdgeLocation edgeLocation) { m_position = position; m_edgeLocation = edgeLocation; }

        public GridPoint m_position;
        public GridBoxEdgeLocation m_edgeLocation;
    }

    /**
    * Find all intersections between a line and the box determined by grid boundaries
    * linePoint has to be specified in grid coordinates
    * **/
    public GridBoxPoint[] FindLineGridBoxIntersections(GridPoint linePoint, GridPoint lineDirection)
    {
        GridBoxPoint[] intersections = new GridBoxPoint[2];
        int intersectionIndex = 0;

        int intersectionCount = 0;

        //Check intersection with left grid border
        GridBoxPoint intersectionPoint = CheckLineIntersectionOnGridEdge(GridBoxEdgeLocation.LEFT, linePoint, lineDirection);
        if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
            intersectionCount++;


        //Check intersection with top grid border
        intersectionPoint = CheckLineIntersectionOnGridEdge(GridBoxEdgeLocation.TOP, linePoint, lineDirection);
        if (intersectionPoint.m_position != intersections[0].m_position)
            if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
                intersectionCount++;

        //Check intersection with right grid border
        if (intersectionIndex < 2)
        {
            intersectionPoint = CheckLineIntersectionOnGridEdge(GridBoxEdgeLocation.RIGHT, linePoint, lineDirection);
            if (intersectionPoint.m_position != intersections[0].m_position)
                if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
                    intersectionCount++;
        }

        //Check intersection with bottom grid border
        if (intersectionIndex < 2)
        {
            intersectionPoint = CheckLineIntersectionOnGridEdge(GridBoxEdgeLocation.BOTTOM, linePoint, lineDirection);
            if (intersectionPoint.m_position != intersections[0].m_position)
                if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
                    intersectionCount++;
        }

        return intersections;
    }

    /**
     * Check if a line intersects a grid edge
     * **/
    private GridBoxPoint CheckLineIntersectionOnGridEdge(GridBoxEdgeLocation edgeLocation, GridPoint linePoint, GridPoint lineDirection)
    {
        //Set the grid coordinates of edge endpoints
        GridPoint edgePointA, edgePointB;
        int scalePrecision = GridPoint.DEFAULT_SCALE_PRECISION;
        if (edgeLocation == GridBoxEdgeLocation.LEFT)
        {
            edgePointA = new GridPoint(1, m_numLines);
            edgePointB = new GridPoint(1, 1);
            edgePointA.Scale(scalePrecision);
            edgePointB.Scale(scalePrecision);
        }
        else if (edgeLocation == GridBoxEdgeLocation.BOTTOM)
        {
            edgePointA = new GridPoint(1, 1);
            edgePointB = new GridPoint(m_numColumns, 1);
            edgePointA.Scale(scalePrecision);
            edgePointB.Scale(scalePrecision);
        }
        else if (edgeLocation == GridBoxEdgeLocation.RIGHT)
        {
            edgePointA = new GridPoint(m_numColumns, 1);
            edgePointB = new GridPoint(m_numColumns, m_numLines);
            edgePointA.Scale(scalePrecision);
            edgePointB.Scale(scalePrecision);
        }
        else //GridBoxEdgeLocation.TOP
        {
            edgePointA = new GridPoint(m_numColumns, m_numLines);
            edgePointB = new GridPoint(1, m_numLines);
            edgePointA.Scale(scalePrecision);
            edgePointB.Scale(scalePrecision);
        }

        GridPoint intersection;
        bool intersects;
        GridEdge edge = new GridEdge(edgePointA, edgePointB);
        edge.IntersectionWithLine(linePoint, lineDirection, out intersects, out intersection);
        if (intersects)
        {
            GridBoxPoint intersectionPoint = new GridBoxPoint(intersection, edgeLocation);
            return intersectionPoint;
        }

        return new GridBoxPoint(GridPoint.zero, GridBoxEdgeLocation.NONE);
    }

    /**
     * Add a point in the array of intersection grid box points
     * **/
    private bool AddIntersectionGridBoxPointAtIndex(ref GridBoxPoint[] array, ref int index, GridBoxPoint point)
    {
        if (point.m_edgeLocation != GridBoxEdgeLocation.NONE)
        {
            //Add the point if not already stored
            if (index == 1)
            {
                if (!array[0].m_position.Equals(point.m_position)) //check if point is different from first one
                {
                    array[1] = point;
                    index++;
                }
            }
            else
            {
                array[0] = point;
                index++;
            }

            return true;
        }

        return false;
    }

    //public void Update()
    //{
    //    RefreshAnchorsStates();
    //}
}
