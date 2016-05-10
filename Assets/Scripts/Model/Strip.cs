using UnityEngine;
using System.Collections.Generic;

public class Strip : GridTriangulable
{
    private Axis m_parentAxis;

    //parts of the strip that are on the left and on the right of the axis
    public Shape m_stripLeftSubShape { get; set; }
    public Shape m_stripRightSubShape { get; set; }

    public Strip(Axis parentAxis) : base()
    {
        m_parentAxis = parentAxis;
    }

    /**
    * Find the contour of this strip and triangulate it
    * **/
    public void CalculateContour()
    {
        Grid grid = m_parentAxis.m_parentRenderer.GetGameScene().m_grid;

        //Calculate intersection points
        Vector2 axisNormal = m_parentAxis.GetNormal();
        GridPoint gridAxisNormal = new GridPoint(Mathf.RoundToInt(GridPoint.DEFAULT_SCALE_PRECISION * axisNormal.x),
                                                 Mathf.RoundToInt(GridPoint.DEFAULT_SCALE_PRECISION * axisNormal.y),
                                                 false);

        Grid.GridBoxPoint[] endpoint1LineIntersections = grid.FindLineGridBoxIntersections(m_parentAxis.m_pointA, gridAxisNormal);
        Grid.GridBoxPoint[] endpoint2LineIntersections = grid.FindLineGridBoxIntersections(m_parentAxis.m_pointB, gridAxisNormal);

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
        Contour stripContour = new Contour(4); //at least 4 points
        GridPoint gridTopLeftCorner = new GridPoint(1, grid.m_numLines, true);
        GridPoint gridBottomLeftCorner = new GridPoint(1, 1, true);
        GridPoint gridBottomRightCorner = new GridPoint(grid.m_numColumns, 1, true);
        GridPoint gridTopRightCorner = new GridPoint(grid.m_numColumns, grid.m_numLines, true);

        GridEdge axisEdge = new GridEdge(m_parentAxis.m_pointA, m_parentAxis.m_pointB);

        //add left edge points
        for (int i = 0; i != leftEdgePoints.Count; i++)
        {
            stripContour.Add(leftEdgePoints[i].m_position);
        }
        //add bottom left hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridBottomLeftCorner))
            stripContour.Add(gridBottomLeftCorner);

        //add bottom edge points
        for (int i = 0; i != bottomEdgePoints.Count; i++)
        {
            stripContour.Add(bottomEdgePoints[i].m_position);
        }

        //add bottom right hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridBottomRightCorner))
            stripContour.Add(gridBottomRightCorner);

        //add right edge points
        for (int i = 0; i != rightEdgePoints.Count; i++)
        {
            stripContour.Add(rightEdgePoints[i].m_position);
        }

        //add top right hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridTopRightCorner))
            stripContour.Add(gridTopRightCorner);

        //add top edge points
        for (int i = 0; i != topEdgePoints.Count; i++)
        {
            stripContour.Add(topEdgePoints[i].m_position);
        }

        //add top left hand corner of the grid if necessary
        if (axisEdge.ContainsPointInStrip(gridTopLeftCorner))
            stripContour.Add(gridTopLeftCorner);

        //Assign the contour to this strip and triangulate it
        m_contour = stripContour;
        Triangulate();
    }

    /**
     * Extract sub contours on strip to create a left and/or a right shape that will be used to clip polygon shapes inside the grid
     * **/
    public void Split()
    {
        Contour stripContour = m_contour;

        //Find the index of the next vertices to axis endpoints inside the strip contour
        int indexOfNextVertexToAxisPointA = -1;
        int indexOfNextVertexToAxisPointB = -1;

        for (int i = 0; i != stripContour.Count; i++)
        {
            GridPoint stripVertex1 = stripContour[i];
            GridPoint stripVertex2 = (i == stripContour.Count - 1) ? stripContour[0] : stripContour[i + 1];
            GridEdge edge = new GridEdge(stripVertex1, stripVertex2);

            if (indexOfNextVertexToAxisPointA < 0 && edge.ContainsPoint(m_parentAxis.m_pointA))
            {
                indexOfNextVertexToAxisPointA = (i == stripContour.Count - 1) ? 0 : i + 1;
            }

            if (indexOfNextVertexToAxisPointB < 0 && edge.ContainsPoint(m_parentAxis.m_pointB))
            {
                indexOfNextVertexToAxisPointB = (i == stripContour.Count - 1) ? 0 : i + 1;
            }

            if (indexOfNextVertexToAxisPointA >= 0 && indexOfNextVertexToAxisPointB >= 0)
                break;
        }

        if (indexOfNextVertexToAxisPointA == -1 || indexOfNextVertexToAxisPointB == -1)
        {
            throw new System.Exception("Could not place pointA or pointB inside strip");
        }

        //Create the two sub contours
        Contour leftSubContour = new Contour(); //the contour associated with the 'left' sub shape
        leftSubContour.Add(m_parentAxis.m_pointB);
        for (int i = indexOfNextVertexToAxisPointB; i != indexOfNextVertexToAxisPointA; i++)
        {
            if (i == stripContour.Count) //reset to the beginning of the list
            {
                if (indexOfNextVertexToAxisPointA == 0) //we added all points between A and B, break the loop so we can add B
                    break;
                else //more points need to be added, reset to the beginning of the list
                    i = 0;
            }
            leftSubContour.Add(stripContour[i]);
        }
        leftSubContour.Add(m_parentAxis.m_pointA);

        m_stripLeftSubShape = new Shape(leftSubContour);

        if (m_parentAxis.m_type == Axis.AxisType.SYMMETRY_AXES_TWO_SIDES)
        {
            Contour rightSubContour = new Contour(); //the contour associated with the 'right' sub shape
            rightSubContour.Add(m_parentAxis.m_pointA);
            for (int i = indexOfNextVertexToAxisPointA; i != indexOfNextVertexToAxisPointB; i++)
            {
                if (i == stripContour.Count)
                {
                    if (indexOfNextVertexToAxisPointB == 0) //we added all points between A and B, break the loop so we can add B
                        break;
                    else //more points need to be added, reset to the beginning of the list
                        i = 0;
                }
                rightSubContour.Add(stripContour[i]);
            }
            rightSubContour.Add(m_parentAxis.m_pointB);

            m_stripRightSubShape = new Shape(rightSubContour);
        }
        else
            m_stripRightSubShape = null;
    }


    public enum PointSorting
    {
        ASCENDING_X = 1,
        ASCENDING_Y = 2,
        DESCENDING_X = 3,
        DESCENDING_Y = 4
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
}
