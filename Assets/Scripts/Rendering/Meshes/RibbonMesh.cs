using UnityEngine;
using System.Collections.Generic;

public class RibbonMesh : ColorMesh
{
    private GridTriangulable m_ribbonData; //the points defining the contour of this ribbon

    //parts of the ribbon that are on the left and on the right of the axis
    private GridTriangulable m_leftAxisRibbon;
    private GridTriangulable m_rightAxisRibbon;

    private Grid m_grid;
    private Color m_color; //the color set by the animator (read-only)

    public enum PointSorting
    {
        ASCENDING_X = 1,
        ASCENDING_Y = 2,
        DESCENDING_X = 3,
        DESCENDING_Y = 4
    }

    public override void Init()
    {
        base.Init();
        m_mesh.name = "RibbonMesh";
    }

    /**
     * Find the contour of this ribbon and triangulate it
     * **/
    public virtual void CalculateRibbonForAxis(AxisRenderer axis)
    {
        //clear the mesh
        ClearMesh();
        m_mesh.Clear();

        //Calculate intersection points
        Grid grid = GetGrid();
        Vector2 axisNormal = axis.GetAxisNormal();
        //Grid.GridBoxPoint[] intersectionPoints = new Grid.GridBoxPoint[4];
        Grid.GridBoxPoint[] endpoint1LineIntersections = grid.FindLineGridBoxIntersections(axis.m_endpoint1GridPosition, axisNormal);
        Grid.GridBoxPoint[] endpoint2LineIntersections = grid.FindLineGridBoxIntersections(axis.m_endpoint2GridPosition, axisNormal);
        //intersectionPoints[0] = endpoint1LineIntersections[0];
        //intersectionPoints[1] = endpoint1LineIntersections[1];
        //intersectionPoints[2] = endpoint2LineIntersections[0];
        //intersectionPoints[3] = endpoint2LineIntersections[1];      

        //extract edge points and sort them
        List<Grid.GridBoxPoint> leftEdgePoints = new List<Grid.GridBoxPoint>();
        List<Grid.GridBoxPoint> bottomEdgePoints = new List<Grid.GridBoxPoint>();
        List<Grid.GridBoxPoint> rightEdgePoints = new List<Grid.GridBoxPoint>();
        List<Grid.GridBoxPoint> topEdgePoints = new List<Grid.GridBoxPoint>();
        int leftEdgePoints1Count = 0;
        int bottomEdgePoints1Count = 0;
        int rightEdgePoints1Count = 0;
        int topEdgePoints1Count = 0;

        for (int i = 0; i != endpoint1LineIntersections.Length; i++)
        {
            if (endpoint1LineIntersections[i].m_edge == Grid.GridEdge.LEFT)
            {
                leftEdgePoints.Add(endpoint1LineIntersections[i]);
                leftEdgePoints1Count++;
            }
            else if (endpoint1LineIntersections[i].m_edge == Grid.GridEdge.BOTTOM)
            {
                bottomEdgePoints.Add(endpoint1LineIntersections[i]);
                bottomEdgePoints1Count++;
            }
            else if (endpoint1LineIntersections[i].m_edge == Grid.GridEdge.RIGHT)
            {
                rightEdgePoints.Add(endpoint1LineIntersections[i]);
                rightEdgePoints1Count++;
            }
            else if (endpoint1LineIntersections[i].m_edge == Grid.GridEdge.TOP)
            {
                topEdgePoints.Add(endpoint1LineIntersections[i]);
                topEdgePoints1Count++;
            }
        }

        int leftEdgePoints2Count = 0;
        int bottomEdgePoints2Count = 0;
        int rightEdgePoints2Count = 0;
        int topEdgePoints2Count = 0;
        for (int i = 0; i != endpoint2LineIntersections.Length; i++)
        {
            if (endpoint2LineIntersections[i].m_edge == Grid.GridEdge.LEFT)
            {
                leftEdgePoints.Add(endpoint2LineIntersections[i]);
                leftEdgePoints2Count++;
            }
            else if (endpoint2LineIntersections[i].m_edge == Grid.GridEdge.BOTTOM)
            {
                bottomEdgePoints.Add(endpoint2LineIntersections[i]);
                bottomEdgePoints2Count++;
            }
            else if (endpoint2LineIntersections[i].m_edge == Grid.GridEdge.RIGHT)
            {
                rightEdgePoints.Add(endpoint2LineIntersections[i]);
                rightEdgePoints2Count++;
            }
            else if (endpoint2LineIntersections[i].m_edge == Grid.GridEdge.TOP)
            {
                topEdgePoints.Add(endpoint2LineIntersections[i]);
                topEdgePoints2Count++;
            }
        }
        
        //reorder points so they follow the contour left->bottom->right->top
        ReorderEdgePoints(ref leftEdgePoints, PointSorting.DESCENDING_Y);
        ReorderEdgePoints(ref bottomEdgePoints, PointSorting.ASCENDING_X);
        ReorderEdgePoints(ref rightEdgePoints, PointSorting.ASCENDING_Y);
        ReorderEdgePoints(ref topEdgePoints, PointSorting.DESCENDING_X);

        //Build the contour by adding grid vertices if 2 consecutive intersection points are on 2 adjacent grid edges
        Contour ribbonContour = new Contour(4); //at least 4 points
        bool bAddGridCorners = (leftEdgePoints.Count == 1) && (bottomEdgePoints.Count == 1) && (rightEdgePoints.Count == 1) && (topEdgePoints.Count == 1);
        
        //add left edge points
        for (int i = 0; i != leftEdgePoints.Count; i++)
        {
            ribbonContour.Add(leftEdgePoints[i].m_position);
        }
        //add bottom left hand corner of the grid if necessary
        if (bAddGridCorners &&
            (leftEdgePoints1Count == 1 && bottomEdgePoints2Count == 1 ||
            leftEdgePoints2Count == 1 && bottomEdgePoints1Count == 1))
        {
            ribbonContour.Add(new Vector2(1, 1));
        }         

        //add bottom edge points
        for (int i = 0; i != bottomEdgePoints.Count; i++)
        {
            ribbonContour.Add(bottomEdgePoints[i].m_position);
        }

        //add bottom right hand corner of the grid if necessary
        if (bAddGridCorners &&
            (bottomEdgePoints1Count == 1 && rightEdgePoints2Count == 1 ||
            bottomEdgePoints2Count == 1 && rightEdgePoints1Count == 1))
        {
            ribbonContour.Add(new Vector2(grid.m_numColumns, 1));
        }

        //add right edge points
        for (int i = 0; i != rightEdgePoints.Count; i++)
        {
            ribbonContour.Add(rightEdgePoints[i].m_position);
        }

        //add top right hand corner of the grid if necessary
        if (bAddGridCorners &&
            (rightEdgePoints1Count == 1 && topEdgePoints2Count == 1 ||
            rightEdgePoints2Count == 1 && topEdgePoints1Count == 1))
        {
            ribbonContour.Add(new Vector2(grid.m_numColumns, grid.m_numLines));
        }

        //add top edge points
        for (int i = 0; i != topEdgePoints.Count; i++)
        {
            ribbonContour.Add(topEdgePoints[i].m_position);
        }

        //add top left hand corner of the grid if necessary
        if (bAddGridCorners &&
            (topEdgePoints1Count == 1 && leftEdgePoints2Count == 1 ||
            topEdgePoints2Count == 1 && leftEdgePoints1Count == 1))
        {
            ribbonContour.Add(new Vector2(1, grid.m_numLines));
        }

        //build the ribbon data object
        m_ribbonData = new GridTriangulable(true, ribbonContour);
        m_ribbonData.Triangulate();
        m_ribbonData.TogglePointMode(); //switch to world coordinates

        //Add the triangles to the mesh
        for (int iTriangleIdx = 0; iTriangleIdx != m_ribbonData.m_triangles.Count; iTriangleIdx++)
        {
            BaseTriangle triangle = m_ribbonData.m_triangles[iTriangleIdx];
            AddTriangle(triangle.m_points[0], triangle.m_points[2], triangle.m_points[1]);
        }

        SetColor(m_color); //reset the color

        RefreshMesh();
    }

    /**
     * Reorder the points on a same edge according to 'sorting' options
     * **/
    private void ReorderEdgePoints(ref List<Grid.GridBoxPoint> points, PointSorting sorting)
    {
        int length = points.Count;
        bool swap = true;
        while (length > 0 && swap)
        {
            swap = false;
            for (int j = 0; j != length - 1; j++)
            {                
                //swap points in either of the following cases
                if (sorting == PointSorting.ASCENDING_X && points[j].m_position.x > points[j + 1].m_position.x ||
                    sorting == PointSorting.ASCENDING_Y && points[j].m_position.y > points[j + 1].m_position.y ||
                    sorting == PointSorting.DESCENDING_X && points[j].m_position.x < points[j + 1].m_position.x ||
                    sorting == PointSorting.DESCENDING_Y && points[j].m_position.y < points[j + 1].m_position.y)
                {
                    Grid.GridBoxPoint tmpPoint = points[j];
                    points[j] = points[j + 1];
                    points[j + 1] = tmpPoint;
                    swap = true;
                }
            }

            length--;
        }
    }

    /**
     * Set the color of this ribbon
     * **/
    public void SetColor(Color color)
    {
        m_color = color;

        for (int i = 0; i != m_colors.Count; i++)
        {
            m_colors[i] = color;
        }

        m_meshColorsDirty = true;
    }

    private Grid GetGrid()
    {
        if (m_grid == null)
            m_grid = ((GameScene) (GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene)).GetComponentInChildren<Grid>();

        return m_grid;
    }
}
