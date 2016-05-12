using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    //Shared prefabs
    public GameObject m_circleMeshPfb;

    public class GridAnchor
    {
        public GridPoint m_gridPosition { get; set; } //the position of the anchor in local grid coordinates (column, line)
        public Vector2 m_localPosition { get; set; } //the position of the anchor relatively to its grid parent

        public GridAnchor(GridPoint gridPosition, Vector2 localPosition)
        {
            m_localPosition = localPosition;
            m_gridPosition = gridPosition;
        }
    }

    private GameObject m_pointsHolder;
    public GridAnchor[] m_anchors { get; set; }
    public Vector2 m_maxGridSize { get; set; }
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

    //grid borders (left, top, right, bottom in this order)
    private GridBorder[] m_borders;
    public GridBorder[] Borders
    {
        get
        {
            return m_borders;
        }
    }
    public GameObject m_gridBorderPfb;

    public void Build(Vector2 maxGridSize, Material gridMaterial)
    {
        m_maxGridSize = maxGridSize;

        m_pointsHolder = new GameObject("Points");

        GameObjectAnimator pointsHolderAnimator = m_pointsHolder.AddComponent<GameObjectAnimator>();
        pointsHolderAnimator.SetParentTransform(this.transform);
        pointsHolderAnimator.SetPosition(Vector3.zero);

        //Get the number of min lines and min columns we want for this level
        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        Level currentLevel = levelManager.m_currentLevel;
        int minNumLines = currentLevel.m_gridMinNumLines;
        int minNumColumns = currentLevel.m_gridMinNumColumns;
        int exactNumLines = currentLevel.m_gridExactNumLines;
        int exactNumColumns = currentLevel.m_gridExactNumColumns;

        //get the screen viewport dimensions     
        float lineGridSpacing, columnGridSpacing;
        if (exactNumLines > 0)
            lineGridSpacing = maxGridSize.y / (float)(exactNumLines - 1);
        else
            lineGridSpacing = maxGridSize.y / (float)(minNumLines - 1);

        if (exactNumColumns > 0)
            columnGridSpacing = maxGridSize.x / (float)(exactNumColumns - 1);
        else
            columnGridSpacing = maxGridSize.x / (float)(minNumColumns - 1);


        //TMP disable maxGridSpacing
        //if (currentLevel.m_maxGridSpacing > 0)
        //    m_gridSpacing = Mathf.Min(lineGridSpacing, columnGridSpacing, currentLevel.m_maxGridSpacing);
        //else
            m_gridSpacing = Mathf.Min(lineGridSpacing, columnGridSpacing);

        m_numLines = (exactNumLines > 0) ? exactNumLines : Mathf.FloorToInt(maxGridSize.y / m_gridSpacing) + 1;
        m_numColumns = (exactNumColumns > 0) ? exactNumColumns : Mathf.FloorToInt(maxGridSize.x / m_gridSpacing) + 1;

        int anchorsCount = m_numLines * m_numColumns;
        m_anchors = new GridAnchor[anchorsCount];

        m_gridSize = new Vector2((m_numColumns - 1) * m_gridSpacing, (m_numLines - 1) * m_gridSpacing);

        //Build anchors
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

                GridPoint anchorGridPosition = new GridPoint(iColumnNumber, iLineNumber, true);
                Vector3 anchorPosition = new Vector3(anchorPositionX, anchorPositionY, 0);
                GameObject clonedGridAnchor = (GameObject)Instantiate(m_circleMeshPfb);

                CircleMesh gridAnchorQuad = clonedGridAnchor.GetComponent<CircleMesh>();
                gridAnchorQuad.Init(gridMaterial);
                int iAnchorIndex = (iLineNumber - 1) * exactNumColumns + (iColumnNumber - 1);

                CircleMeshAnimator anchorAnimator = clonedGridAnchor.GetComponent<CircleMeshAnimator>();
                anchorAnimator.SetParentTransform(m_pointsHolder.transform);
                anchorAnimator.SetNumSegments(4, false);
                anchorAnimator.SetInnerRadius(0, false);
                anchorAnimator.SetOuterRadius(3, true);
                anchorAnimator.SetPosition(anchorPosition);
                anchorAnimator.SetColor(Color.white);

                m_anchors[iAnchorIndex] = new GridAnchor(anchorGridPosition, anchorPosition);
            }
        }

        BuildBorders();
    }

    /**
    * Build a border around the grid
    **/
    private void BuildBorders()
    {
        //build a line for every grid border (left, top, right, bottom)
        m_borders = new GridBorder[4];
        float margin = 0.1f * this.m_gridSpacing; //the relaxed position is a bit off the grid       

        //left
        GameObject lineObject = (GameObject)Instantiate(m_gridBorderPfb);
        lineObject.name = "LeftBorder";

        Vector3 contractedPosition = new Vector3(-0.5f * m_gridSize.x, 0, 0);
        Vector3 relaxedPosition = contractedPosition - new Vector3(margin, 0, 0);

        UVQuad lineQuad = lineObject.GetComponent<UVQuad>();
        lineQuad.Init();

        GameObjectAnimator lineAnimator = lineObject.GetComponent<GameObjectAnimator>();
        lineAnimator.SetParentTransform(this.transform);
        lineAnimator.SetPosition(relaxedPosition);
        lineAnimator.SetScale(new Vector3(m_gridSize.y, 16, 1));
        lineAnimator.SetRotationAxis(Vector3.forward);
        lineAnimator.SetRotationAngle(90);

        m_borders[0] = new GridBorder(GridBoxEdgeLocation.LEFT, lineObject, relaxedPosition, contractedPosition);

        //top
        lineObject = (GameObject)Instantiate(m_gridBorderPfb);
        lineObject.name = "TopBorder";

        contractedPosition = new Vector3(0, 0.5f * m_gridSize.y, 0);
        relaxedPosition = contractedPosition + new Vector3(0,margin,0);

        lineQuad = lineObject.GetComponent<UVQuad>();
        lineQuad.Init();

        lineAnimator = lineObject.GetComponent<GameObjectAnimator>();
        lineAnimator.SetParentTransform(this.transform);
        lineAnimator.SetPosition(relaxedPosition);
        lineAnimator.SetScale(new Vector3(m_gridSize.x, 16, 1));

        m_borders[1] = new GridBorder(GridBoxEdgeLocation.TOP, lineObject, relaxedPosition, contractedPosition);

        //right
        lineObject = (GameObject)Instantiate(m_gridBorderPfb);
        lineObject.name = "RightBorder";

        contractedPosition = new Vector3(0.5f * m_gridSize.x, 0, 0);
        relaxedPosition = contractedPosition + new Vector3(margin, 0, 0);

        lineQuad = lineObject.GetComponent<UVQuad>();
        lineQuad.Init();

        lineAnimator = lineObject.GetComponent<GameObjectAnimator>();
        lineAnimator.SetParentTransform(this.transform);
        lineAnimator.SetPosition(relaxedPosition);
        lineAnimator.SetScale(new Vector3(m_gridSize.y, 16, 1));
        lineAnimator.SetRotationAxis(Vector3.forward);
        lineAnimator.SetRotationAngle(90);

        m_borders[2] = new GridBorder(GridBoxEdgeLocation.RIGHT, lineObject, relaxedPosition, contractedPosition);

        //bottom
        lineObject = (GameObject)Instantiate(m_gridBorderPfb);
        lineObject.name = "RightBorder";

        contractedPosition = new Vector3(0, -0.5f * m_gridSize.y, 0);
        relaxedPosition = contractedPosition - new Vector3(0, margin, 0);

        lineQuad = lineObject.GetComponent<UVQuad>();
        lineQuad.Init();

        lineAnimator = lineObject.GetComponent<GameObjectAnimator>();
        lineAnimator.SetParentTransform(this.transform);
        lineAnimator.SetPosition(relaxedPosition);
        lineAnimator.SetScale(new Vector3(m_gridSize.x, 16, 1));

        m_borders[3] = new GridBorder(GridBoxEdgeLocation.BOTTOM, lineObject, relaxedPosition, contractedPosition);
    }

    /**
     * Fade out all the points inside the grid
     * **/
    public void DismissGridPoints(float fDuration)
    {
        m_pointsHolder.GetComponent<GameObjectAnimator>().FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
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
     * Calculates the grid coordinates of a point knowing its world coordinates
     * **/
    public GridPoint GetPointGridCoordinatesFromWorldCoordinates(Vector2 worldCoordinates)
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

        GridPoint gridCoords = new GridPoint(Mathf.RoundToInt(iColNumber * GridPoint.DEFAULT_SCALE_PRECISION), 
                                             Mathf.RoundToInt(iLineNumber * GridPoint.DEFAULT_SCALE_PRECISION),
                                             false);
        return gridCoords;
    }

    /**
     * Converts a world vector into a grid vector
     * **/
    public GridPoint TransformWorldVectorIntoGridVector(Vector2 worldVector)
    {
        int X = Mathf.RoundToInt(worldVector.x / m_gridSpacing * GridPoint.DEFAULT_SCALE_PRECISION);
        int Y = Mathf.RoundToInt(worldVector.y / m_gridSpacing * GridPoint.DEFAULT_SCALE_PRECISION);

        return new GridPoint(X, Y, false);
    }

    /**
     * Converts a grid vector into a world vector
     * **/
    public Vector2 TransformWorldVectorIntoGridVector(GridPoint gridVector)
    {
        float X = gridVector.X * m_gridSpacing / (float) GridPoint.DEFAULT_SCALE_PRECISION;
        float Y = gridVector.Y * m_gridSpacing / (float)GridPoint.DEFAULT_SCALE_PRECISION;

        return new Vector2(X, Y);
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
        //first transform the worldPosition into grid local position
        Vector2 gridPosition = this.transform.position;
        Vector2 localPosition = worldPosition - gridPosition;

        float sqrMinDistance = float.MaxValue;
        int iMinDistanceAnchorIndex = -1;

        int neighborAnchorCount = 0;
        
        for (int iAnchorIndex = 0; iAnchorIndex != m_anchors.Length; iAnchorIndex++)
        {
            Vector2 gridAnchorLocalPosition = m_anchors[iAnchorIndex].m_localPosition;
            float dx = Mathf.Abs(localPosition.x - gridAnchorLocalPosition.x);
            if (dx <= m_gridSpacing)
            {
                float dy = Mathf.Abs(localPosition.y - gridAnchorLocalPosition.y);
                if (dy <= m_gridSpacing)
                {
                    neighborAnchorCount++;
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

        Debug.Log("neighborAnchorCount:" + neighborAnchorCount);

        if (iMinDistanceAnchorIndex >= 0)
            return m_anchors[iMinDistanceAnchorIndex];
        else
            return null;
    }

    /**
     * Returns the grid anchor coordinates that is the closest to the position vector passed as parameter
     * **/
    public GridAnchor GetClosestGridAnchorForGridPosition(GridPoint gridPosition)
    {
        for (int iAnchorIndex = 0; iAnchorIndex != m_anchors.Length; iAnchorIndex++)
        {
            GridPoint gridAnchorWorldPosition = m_anchors[iAnchorIndex].m_gridPosition;
            int dx = Mathf.Abs(gridPosition.X - gridAnchorWorldPosition.X);
            int dy = Mathf.Abs(gridPosition.Y - gridAnchorWorldPosition.Y);
            if (dx <= 0.5f * GridPoint.DEFAULT_SCALE_PRECISION && dy <= 0.5f * GridPoint.DEFAULT_SCALE_PRECISION)
            {
                return m_anchors[iAnchorIndex];
            }
        }

        return null;
    }

    /**
    * Find all anchors that can be used to draw the second endpoint of an axis based on the constrained directions defined in the current level
    **/
    public List<GridPoint> FindAvailableAnchorsForAxis(GridPoint axisPointA, List<GridPoint> constrainedDirections)
    {
        List<GridPoint> availableAnchorsPositions = new List<GridPoint>();

        for (int i = 0; i != m_anchors.Length; i++)
        {
            GridPoint anchorPosition = m_anchors[i].m_gridPosition;

            if (anchorPosition == axisPointA) //same point
                continue;

            GridPoint anchorToAxisEndpoint = axisPointA - anchorPosition;

            //GridPoints are scaled, and as they are grid anchors positions we can downscale them before performing calculation on them
            anchorToAxisEndpoint /= GridPoint.DEFAULT_SCALE_PRECISION;
            

            for (int j = 0; j != constrainedDirections.Count; j++)
            {
                GridPoint constrainedDirection = constrainedDirections[j];
                long dotProduct = MathUtils.DotProduct(anchorToAxisEndpoint, constrainedDirection);
                if ((dotProduct * dotProduct) == (anchorToAxisEndpoint.sqrMagnitude * constrainedDirection.sqrMagnitude))
                {
                    availableAnchorsPositions.Add(anchorPosition);
                }
            }
        }

        return availableAnchorsPositions;
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
        GridEdge borderEdge = GetGridBoxEdgeForLocation(edgeLocation);

        GridPoint intersection;
        bool intersects;
        GridEdge edge = new GridEdge(borderEdge.m_pointA, borderEdge.m_pointB);
        edge.IntersectionWithLine(linePoint, lineDirection, out intersects, out intersection);
        if (intersects)
        {
            GridBoxPoint intersectionPoint = new GridBoxPoint(intersection, edgeLocation);
            return intersectionPoint;
        }

        return new GridBoxPoint(GridPoint.zero, GridBoxEdgeLocation.NONE);
    }

    /**
    * Return the edge associated with one of the four grid borders
    **/
    public GridEdge GetGridBoxEdgeForLocation(GridBoxEdgeLocation edgeLocation)
    {
        //Set the grid coordinates of edge endpoints
        GridPoint edgePointA, edgePointB;
        if (edgeLocation == GridBoxEdgeLocation.LEFT)
        {
            edgePointA = new GridPoint(1, m_numLines, true);
            edgePointB = new GridPoint(1, 1, true);
        }
        else if (edgeLocation == GridBoxEdgeLocation.BOTTOM)
        {
            edgePointA = new GridPoint(1, 1, true);
            edgePointB = new GridPoint(m_numColumns, 1, true);
        }
        else if (edgeLocation == GridBoxEdgeLocation.RIGHT)
        {
            edgePointA = new GridPoint(m_numColumns, 1, true);
            edgePointB = new GridPoint(m_numColumns, m_numLines, true);
        }
        else //GridBoxEdgeLocation.TOP
        {
            edgePointA = new GridPoint(m_numColumns, m_numLines, true);
            edgePointB = new GridPoint(1, m_numLines, true);
        }

        return new GridEdge(edgePointA, edgePointB);
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
