using UnityEngine;
using System.Collections.Generic;

public class RibbonMesh : ColorMesh
{
    private GridTriangulable m_ribbonData; //the points defining the contour of this ribbon

    //Axis endpoints that define the ribbon
    private GridPoint m_ribbonPointA;
    private GridPoint m_ribbonPointB;

    //parts of the ribbon that are on the left and on the right of the axis
    public Shape m_ribbonLeftSubShape { get; set; }
    public Shape m_ribbonRightSubShape { get; set; }

    private Color m_color; //the color set by the animator (read-only)

    public enum PointSorting
    {
        ASCENDING_X = 1,
        ASCENDING_Y = 2,
        DESCENDING_X = 3,
        DESCENDING_Y = 4
    }

    public override void Init(Material material = null)
    {
        base.Init(material);
        m_mesh.name = "RibbonMesh";
    }

    /**
     * Calls this to hide this ribbon mesh, for instance when axis has zero length
     * Basically clear all mesh arrays and thats it
     * **/
    public void Hide()
    {
        ClearMesh();
        m_mesh.Clear();
    }

    /**
     * Find the contour of this ribbon and triangulate it
     * **/
    public virtual void CalculateRibbonForAxis(AxisRenderer axis)
    {
        m_ribbonPointA = axis.m_pointA;
        m_ribbonPointB = axis.m_pointB;

        //clear the mesh
        ClearMesh();
        m_mesh.Clear();

        //Calculate intersection points
        Grid grid = GetGrid();
        Vector2 axisNormal = axis.GetNormal();
        GridPoint gridAxisNormal = new GridPoint(Mathf.RoundToInt(GridPoint.DEFAULT_SCALE_PRECISION * axisNormal.x), 
                                                 Mathf.RoundToInt(GridPoint.DEFAULT_SCALE_PRECISION * axisNormal.y));

        Grid.GridBoxPoint[] endpoint1LineIntersections = grid.FindLineGridBoxIntersections(axis.m_pointA, gridAxisNormal);
        Grid.GridBoxPoint[] endpoint2LineIntersections = grid.FindLineGridBoxIntersections(axis.m_pointB, gridAxisNormal);

        //extract edge points and sort them
        List<Grid.GridBoxPoint> leftEdgePoints = new List<Grid.GridBoxPoint>();
        List<Grid.GridBoxPoint> bottomEdgePoints = new List<Grid.GridBoxPoint>();
        List<Grid.GridBoxPoint> rightEdgePoints = new List<Grid.GridBoxPoint>();
        List<Grid.GridBoxPoint> topEdgePoints = new List<Grid.GridBoxPoint>();

        for (int i = 0; i != endpoint1LineIntersections.Length; i++)
        {
            if (endpoint1LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.LEFT)
                leftEdgePoints.Add(endpoint1LineIntersections[i]);
            else if (endpoint1LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.BOTTOM)
                bottomEdgePoints.Add(endpoint1LineIntersections[i]);
            else if (endpoint1LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.RIGHT)
                rightEdgePoints.Add(endpoint1LineIntersections[i]);
            else if (endpoint1LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.TOP)
                topEdgePoints.Add(endpoint1LineIntersections[i]);
        }

        for (int i = 0; i != endpoint2LineIntersections.Length; i++)
        {
            if (endpoint2LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.LEFT)
                leftEdgePoints.Add(endpoint2LineIntersections[i]);
            else if (endpoint2LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.BOTTOM)
                bottomEdgePoints.Add(endpoint2LineIntersections[i]);
            else if (endpoint2LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.RIGHT)
                rightEdgePoints.Add(endpoint2LineIntersections[i]);
            else if (endpoint2LineIntersections[i].m_edgeLocation == Grid.GridBoxEdgeLocation.TOP)
                topEdgePoints.Add(endpoint2LineIntersections[i]);
        }
        
        //reorder points so they follow the contour left->bottom->right->top
        ReorderEdgePoints(ref leftEdgePoints, PointSorting.DESCENDING_Y);
        ReorderEdgePoints(ref bottomEdgePoints, PointSorting.ASCENDING_X);
        ReorderEdgePoints(ref rightEdgePoints, PointSorting.ASCENDING_Y);
        ReorderEdgePoints(ref topEdgePoints, PointSorting.DESCENDING_X);

        //Build the contour by adding grid vertices if 2 consecutive intersection points are on 2 adjacent grid edges
        Contour ribbonContour = new Contour(4); //at least 4 points
        int scaleValue = GridPoint.DEFAULT_SCALE_PRECISION;
        GridPoint gridTopLeftCorner = new GridPoint(1, grid.m_numLines);
        GridPoint gridBottomLeftCorner = new GridPoint(1, 1);
        GridPoint gridBottomRightCorner = new GridPoint(grid.m_numColumns, 1);
        GridPoint gridTopRightCorner = new GridPoint(grid.m_numColumns, grid.m_numLines);
        gridTopLeftCorner.Scale(scaleValue);
        gridBottomLeftCorner.Scale(scaleValue);
        gridBottomRightCorner.Scale(scaleValue);
        gridTopRightCorner.Scale(scaleValue);

        GridEdge axisEdge = new GridEdge(axis.m_pointA, axis.m_pointB);

        //add left edge points
        for (int i = 0; i != leftEdgePoints.Count; i++)
        {
            ribbonContour.Add(leftEdgePoints[i].m_position);
        }
        //add bottom left hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridBottomLeftCorner))
            ribbonContour.Add(gridBottomLeftCorner);        

        //add bottom edge points
        for (int i = 0; i != bottomEdgePoints.Count; i++)
        {
            ribbonContour.Add(bottomEdgePoints[i].m_position);
        }

        //add bottom right hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridBottomRightCorner))
            ribbonContour.Add(gridBottomRightCorner);

        //add right edge points
        for (int i = 0; i != rightEdgePoints.Count; i++)
        {
            ribbonContour.Add(rightEdgePoints[i].m_position);
        }

        //add top right hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridTopRightCorner))
            ribbonContour.Add(gridTopRightCorner);

        //add top edge points
        for (int i = 0; i != topEdgePoints.Count; i++)
        {
            ribbonContour.Add(topEdgePoints[i].m_position);
        }

        //add top left hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridTopLeftCorner))
            ribbonContour.Add(gridTopLeftCorner);

        //build the ribbon data object
        m_ribbonData = new GridTriangulable(ribbonContour);
        m_ribbonData.Triangulate();

        //Add the triangles to the mesh
        for (int iTriangleIdx = 0; iTriangleIdx != m_ribbonData.m_triangles.Count; iTriangleIdx++)
        {
            GridTriangle triangle = m_ribbonData.m_triangles[iTriangleIdx];
            Vector2 pt1 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[0]);
            Vector2 pt2 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[1]);
            Vector2 pt3 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[2]);
            AddTriangle(pt1, pt3, pt2);
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
                if (sorting == PointSorting.ASCENDING_X && points[j].m_position.X > points[j + 1].m_position.X ||
                    sorting == PointSorting.ASCENDING_Y && points[j].m_position.Y > points[j + 1].m_position.Y ||
                    sorting == PointSorting.DESCENDING_X && points[j].m_position.X < points[j + 1].m_position.X ||
                    sorting == PointSorting.DESCENDING_Y && points[j].m_position.Y < points[j + 1].m_position.Y)
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
     * Extract sub contours on ribbon to create a left and/or a right shape that will be used to clip polygon shapes inside the grid
     * **/
    public void SplitByAxis(Symmetrizer.SymmetryType symmetryType)
    {
        Contour ribbonContour = m_ribbonData.m_contour;

        //Find the index of the next vertices to axis endpoints inside the ribbon contour
        int indexOfNextVertexToAxisPointA = -1;
        int indexOfNextVertexToAxisPointB = -1;

        for (int i = 0; i != ribbonContour.Count; i++)
        {
            GridPoint ribbonVertex1 = ribbonContour[i];
            GridPoint ribbonVertex2 = (i == ribbonContour.Count - 1) ? ribbonContour[0] : ribbonContour[i + 1];
            GridEdge edge = new GridEdge(ribbonVertex1, ribbonVertex2);

            if (indexOfNextVertexToAxisPointA < 0 && edge.ContainsPoint(m_ribbonPointA))
            {
                indexOfNextVertexToAxisPointA = (i == ribbonContour.Count - 1) ? 0 : i + 1;
            }

            if (indexOfNextVertexToAxisPointB < 0 && edge.ContainsPoint(m_ribbonPointB))
            {
                indexOfNextVertexToAxisPointB = (i == ribbonContour.Count - 1) ? 0 : i + 1;
            }

            if (indexOfNextVertexToAxisPointA >= 0 && indexOfNextVertexToAxisPointB >= 0)
                break;
        }

        if (indexOfNextVertexToAxisPointA == -1 || indexOfNextVertexToAxisPointB == -1)
        {
            throw new System.Exception("Could not place pointA or pointB inside ribbon");
        }

        //Create the two sub contours
        Contour leftSubContour = new Contour(); //the contour associated with the 'left' sub shape
        leftSubContour.Add(m_ribbonPointB);
        for (int i = indexOfNextVertexToAxisPointB; i != indexOfNextVertexToAxisPointA; i++)
        {
            if (i == ribbonContour.Count) //reset to the beginning of the list
            {
                if (indexOfNextVertexToAxisPointA == 0) //we added all points between A and B, break the loop so we can add B
                    break;
                else //more points need to be added, reset to the beginning of the list
                    i = 0;
            }
            leftSubContour.Add(ribbonContour[i]);
        }
        leftSubContour.Add(m_ribbonPointA);

        m_ribbonLeftSubShape = new Shape(leftSubContour);       

        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_TWO_SIDES)
        {
            Contour rightSubContour = new Contour(); //the contour associated with the 'right' sub shape
            rightSubContour.Add(m_ribbonPointA);
            for (int i = indexOfNextVertexToAxisPointA; i != indexOfNextVertexToAxisPointB; i++)
            {
                if (i == ribbonContour.Count)
                {
                    if (indexOfNextVertexToAxisPointB == 0) //we added all points between A and B, break the loop so we can add B
                        break;
                    else //more points need to be added, reset to the beginning of the list
                        i = 0;
                }
                rightSubContour.Add(ribbonContour[i]);
            }
            rightSubContour.Add(m_ribbonPointB);

            m_ribbonRightSubShape = new Shape(rightSubContour);
        }
        else
            m_ribbonRightSubShape = null;
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
        GameScene gameScene = this.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<GameScene>();
        return gameScene.m_grid;
    }
}
