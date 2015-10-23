﻿using UnityEngine;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    //Shared prefabs
    public GameObject m_circleMeshPfb;
    public Material m_positionColorMaterial;
    private Material m_gridAnchorMaterial;

    public GameObject[] m_anchors { get; set; }
    public Vector2 m_gridSize { get; set; }
    public int m_numLines { get; set; }
    public int m_numColumns { get; set; }
    public float m_gridSpacing { get; set; }
    //public GameObject m_gridConstraintAnchorPfb;

    public enum GridEdge
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
        m_anchors = new GameObject[exactNumLines * exactNumColumns];

        //get the screen viewport dimensions
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //The grid occupies at maximum 85% of the screen height and 90% of the screen width
        m_gridSize = new Vector2(0.9f * screenSize.x, 0.78f * screenSize.y);
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

        m_gridSize = new Vector2((m_numColumns - 1) * m_gridSpacing, (m_numLines - 1) * m_gridSpacing);

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

                Vector3 anchorLocalPosition = new Vector3(anchorPositionX, anchorPositionY, 0);
                GameObject clonedGridAnchor = (GameObject)Instantiate(m_circleMeshPfb);

                CircleMesh gridAnchorQuad = clonedGridAnchor.GetComponent<CircleMesh>();
                gridAnchorQuad.Init(m_gridAnchorMaterial);
                int iAnchorIndex = (iLineNumber - 1) * exactNumColumns + (iColumnNumber - 1);

                CircleMeshAnimator anchorAnimator = clonedGridAnchor.GetComponent<CircleMeshAnimator>();
                anchorAnimator.SetParentTransform(pointsHolder.transform);
                anchorAnimator.SetNumSegments(4, false);
                anchorAnimator.SetInnerRadius(0, false);
                anchorAnimator.SetOuterRadius(4, true);
                anchorAnimator.SetPosition(anchorLocalPosition);
                anchorAnimator.SetColor(Color.white);

                m_anchors[iAnchorIndex] = clonedGridAnchor;
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
    public Vector2 GetPointWorldCoordinatesFromGridCoordinates(Vector2 gridPoint)
    {
        float anchorPositionX;
        if (m_numColumns % 2 == 0) //even number of columns
        {
            anchorPositionX = ((gridPoint.x - m_numColumns / 2 - 0.5f) * m_gridSpacing);
        }
        else //odd number of columns
        {
            anchorPositionX = ((gridPoint.x - 1 - m_numColumns / 2) * m_gridSpacing);
        }
        float anchorPositionY;
        if (m_numLines % 2 == 0) //even number of lines
        {
            anchorPositionY = ((gridPoint.y - m_numLines / 2 - 0.5f) * m_gridSpacing);
        }
        else //odd number of lines
        {
            anchorPositionY = ((gridPoint.y - 1 - m_numLines / 2) * m_gridSpacing);
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

    //public Vector2 TransformGridVectorToWorldVector(Vector2 gridVector)
    //{
    //    return gridVector / GetGridWorldRatio();
    //}

    //public Vector2 TransformWorldVectorToGridVector(Vector2 worldVector)
    //{
    //    return worldVector * GetGridWorldRatio();
    //}

    /**
     * Returns the anchor game object at grid position passed as parameter
     * **/
    //public GameObject GetAnchorAtGridPosition(Vector2 gridPosition)
    //{
    //    int iColumnNumber = Mathf.RoundToInt(gridPosition.x);
    //    int iLineNumber = Mathf.RoundToInt(gridPosition.y);

    //    return m_anchors[(iLineNumber - 1) * m_numColumns + (iColumnNumber - 1)];
    //}

    /**
     * Returns the grid coordinates of the anchor passed as parameter
     * **/
    //public Vector2 GetAnchorGridCoordinates(GameObject anchor)
    //{
    //    int iAnchorIndex = -1;
    //    for (int i = 0; i != m_anchors.Length; i++)
    //    {
    //        if (m_anchors[i] == anchor)
    //        {
    //            iAnchorIndex = i;
    //            break;
    //        }
    //    }

    //    return GetAnchorGridCoordinatesForAnchorIndex(iAnchorIndex);
    //}

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
    public Vector2 GetClosestGridAnchorCoordinatesForPosition(Vector2 worldPosition)
    {
        Vector2 localPosition = worldPosition - GeometryUtils.BuildVector2FromVector3(gameObject.transform.position);

        float sqrMinDistance = float.MaxValue;
        int iMinDistanceAnchorIndex = -1;
        for (int iAnchorIndex = 0; iAnchorIndex != m_anchors.Length; iAnchorIndex++)
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
            return new Vector2(0, 0);
    }

    public struct GridBoxPoint
    {
        public GridBoxPoint(Vector2 position, GridEdge edge) { m_position = position; m_edge = edge; }

        public Vector2 m_position;
        public GridEdge m_edge;
    }

    /**
    * Find all intersections between a line and the box determined by grid boundaries
    * linePoint has to be specified in grid coordinates
    * **/
    public GridBoxPoint[] FindLineGridBoxIntersections(Vector2 linePoint, Vector2 lineDirection)
    {
        GridBoxPoint[] intersections = new GridBoxPoint[2];
        int intersectionIndex = 0;

        int intersectionCount = 0;

        //Check intersection with left grid border
        GridBoxPoint intersectionPoint = CheckLineIntersectionOnGridEdge(GridEdge.LEFT, linePoint, lineDirection);
        if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
            intersectionCount++;


        //Check intersection with top grid border
        intersectionPoint = CheckLineIntersectionOnGridEdge(GridEdge.TOP, linePoint, lineDirection);
        if (!MathUtils.AreVec2PointsEqual(intersectionPoint.m_position, intersections[0].m_position))
            if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
                intersectionCount++;

        //Check intersection with right grid border
        if (intersectionIndex < 2)
        {
            intersectionPoint = CheckLineIntersectionOnGridEdge(GridEdge.RIGHT, linePoint, lineDirection);
            if (!MathUtils.AreVec2PointsEqual(intersectionPoint.m_position, intersections[0].m_position))
                if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
                    intersectionCount++;
        }

        //Check intersection with bottom grid border
        if (intersectionIndex < 2)
        {
            intersectionPoint = CheckLineIntersectionOnGridEdge(GridEdge.BOTTOM, linePoint, lineDirection);
            if (!MathUtils.AreVec2PointsEqual(intersectionPoint.m_position, intersections[0].m_position))
                if (AddIntersectionGridBoxPointAtIndex(ref intersections, ref intersectionIndex, intersectionPoint))
                    intersectionCount++;
        }

        return intersections;
    }

    /**
     * Check if a line intersects a grid edge
     * **/
    private GridBoxPoint CheckLineIntersectionOnGridEdge(GridEdge edge, Vector2 linePoint, Vector2 lineDirection)
    {
        //Set the grid coordinates of edge endpoints
        Vector2 edgePointA, edgePointB;
        if (edge == GridEdge.LEFT)
        {
            edgePointA = new Vector2(1, m_numLines);
            edgePointB = new Vector2(1, 1);
        }
        else if (edge == GridEdge.BOTTOM)
        {
            edgePointA = new Vector2(1, 1);
            edgePointB = new Vector2(m_numColumns, 1);
        }
        else if (edge == GridEdge.RIGHT)
        {
            edgePointA = new Vector2(m_numColumns, 1);
            edgePointB = new Vector2(m_numColumns, m_numLines);
        }
        else //GridEdge.TOP
        {
            edgePointA = new Vector2(m_numColumns, m_numLines);
            edgePointB = new Vector2(1, m_numLines);
        }

        Vector2 intersection;
        bool intersects;
        GeometryUtils.SegmentLineIntersection(edgePointA, edgePointB, linePoint, lineDirection, out intersection, out intersects);
        if (intersects)
        {
            GridBoxPoint intersectionPoint = new GridBoxPoint(intersection, edge);
            return intersectionPoint;
        }

        return new GridBoxPoint(Vector2.zero, GridEdge.NONE);
    }

    /**
     * Add a point in the array of intersection grid box points
     * **/
    private bool AddIntersectionGridBoxPointAtIndex(ref GridBoxPoint[] array, ref int index, GridBoxPoint point)
    {
        if (point.m_edge != GridEdge.NONE)
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

    /**
     * Check if anchors are overlapped by a shape mesh and set their opacities accordingly
     * **/
    public void RefreshAnchorsStates()
    {
        GameScene gameScene = this.transform.parent.gameObject.GetComponent<GameScene>();
        Shapes shapesHolder = gameScene.m_shapesHolder;
        for (int i = 0; i != m_anchors.Length; i++)
        {
            Vector2 anchorPosition = m_anchors[i].transform.position;

            for (int j = 0; j != shapesHolder.m_shapes.Count; j++)
            {
                ShapeMesh shapeMesh = shapesHolder.m_shapes[j].m_parentMesh;
                if (shapeMesh.ContainsPointInsideVisibleMesh(anchorPosition))
                    m_anchors[i].GetComponent<GameObjectAnimator>().SetOpacity(0.5f);
            }
        }
    }

    //public void Update()
    //{
    //    RefreshAnchorsStates();
    //}
}
