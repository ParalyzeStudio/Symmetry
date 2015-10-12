using UnityEngine;
using System.Collections.Generic;

public class ShapeCell
{
    private int m_index; //the index of the cell in the parent mesh cell array

    public Vector3 m_position { get; set; }

    /**
     * C -- D
     * |    |
     * A -- B
     * **/
    public ShapeVoxel m_voxelA { get; set; }
    public ShapeVoxel m_voxelB { get; set; }
    public ShapeVoxel m_voxelC { get; set; }
    public ShapeVoxel m_voxelD { get; set; }

    public int m_startIndex { get; set; } //the start index of this cell inside the mesh vertices array
    public int m_endIndex { get; set; }

    private ShapeMesh m_parentMesh;

    private bool m_showing;
    public bool Showing
    {
        get
        {
            return m_showing;
        }
    }

    private bool m_swept;
    public bool Swept
    {
        get
        {
            return m_swept;
        }
    }

    //neighboring cells
    public ShapeCell m_topCell { get; set; }
    public ShapeCell m_leftCell { get; set; }
    public ShapeCell m_bottomCell { get; set; }
    public ShapeCell m_rightCell { get; set; }

    public ShapeCell(int index, ShapeMesh parentMesh, ShapeVoxel a, ShapeVoxel b, ShapeVoxel c, ShapeVoxel d)
    {
        m_index = index;
        m_parentMesh = parentMesh;

        m_voxelA = a;
        m_voxelB = b;
        m_voxelC = c;
        m_voxelD = d;

        m_topCell = null;
        m_leftCell = null;
        m_bottomCell = null;
        m_rightCell = null;

        m_position = 0.25f * (m_voxelA.m_position + m_voxelB.m_position + m_voxelC.m_position + m_voxelD.m_position);
    }

    /**
     * Try to triangulate the cell and show it if cell is not empty
     * **/
    public void TriangulateAndShow()
    {
        //Triangulate the cell first
        bool bEmptyCell = !Triangulate();

        if (!bEmptyCell)
        {
            Show();
        }
        else //triangulation led to an empty cell so remove it
        {
            m_parentMesh.NullifyCellAtIndex(m_index);
        }
    }

    /**
     * Show the cell
     * **/
    public void Show()
    {
        if (m_showing || m_swept)
            return;

        m_swept = true;
        m_showing = true;        

        //Show neighboring cells with delay
        //CallFuncHandler callFuncHandler = m_parentMesh.GetCallFuncHandler();
        //if (m_topCell != null)
        //    callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(m_topCell.Show), ShapeMesh.CELL_APPARITION_INTERVAL);
        //if (m_leftCell != null)
        //    callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(m_leftCell.Show), ShapeMesh.CELL_APPARITION_INTERVAL);
        //if (m_bottomCell != null)
        //    callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(m_bottomCell.Show), ShapeMesh.CELL_APPARITION_INTERVAL);
        //if (m_rightCell != null)
        //    callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(m_rightCell.Show), ShapeMesh.CELL_APPARITION_INTERVAL);

        //animate cell by fading in it
        SetColor(m_parentMesh.m_shapeData.m_color);
    }

    /**
     * Triangulate the cell
     * **/
    private bool Triangulate()
    {
        int cellType = 0;
        if (m_voxelA.IsOverlappedByShape(this.m_parentMesh.m_shapeData))
        {
            cellType |= 1;
        }
        if (m_voxelB.IsOverlappedByShape(this.m_parentMesh.m_shapeData))
        {
            cellType |= 2;
        }
        if (m_voxelC.IsOverlappedByShape(this.m_parentMesh.m_shapeData))
        {
            cellType |= 4;
        }
        if (m_voxelD.IsOverlappedByShape(this.m_parentMesh.m_shapeData))
        {
            cellType |= 8;
        }

        if (cellType > 0)
        {
            if (cellType == 15)
            {
                TriangulateFullQuad();
                return true;
            }
            else
                return ClipWithParentMesh();
        }

        return false;
    }

    private void TriangulateFullQuad()
    {
        //Debug.Log("TriangulateFullQuad");

        m_parentMesh.AddFullQuadCell(this);
    }

    private bool ClipWithParentMesh()
    {
        Shape parentShape = m_parentMesh.m_shapeData;

        if (!OverlapsShapeWithNonNullIntersection(parentShape)) //no intersection, no need to waste resources clipping it
            return false;

        Contour cellContour = new Contour(4);
        cellContour.Add(m_voxelA.m_position);
        cellContour.Add(m_voxelB.m_position);
        cellContour.Add(m_voxelD.m_position);
        cellContour.Add(m_voxelC.m_position);
        Shape cellShape = new Shape(false, cellContour);
        List<Shape> clipResult = m_parentMesh.GetClippingManager().ShapesOperation(parentShape, cellShape, ClipperLib.ClipType.ctIntersection);

        if (clipResult.Count == 0) //no intersection
            return false;

        m_parentMesh.AddClippedCell(this, clipResult);
        return true;
    }

    private bool OverlapsShapeWithNonNullIntersection(Shape shape)
    {
        //Split the cell in two triangles
        BaseTriangle cellTriangle1 = new BaseTriangle();
        cellTriangle1.m_points[0] = m_voxelA.m_position;
        cellTriangle1.m_points[1] = m_voxelB.m_position;
        cellTriangle1.m_points[2] = m_voxelC.m_position;

        BaseTriangle cellTriangle2 = new BaseTriangle();
        cellTriangle2.m_points[0] = m_voxelB.m_position;
        cellTriangle2.m_points[1] = m_voxelD.m_position;
        cellTriangle2.m_points[2] = m_voxelC.m_position;

        return shape.OverlapsTriangle(cellTriangle1, true) || shape.OverlapsTriangle(cellTriangle2, true);
    }

    /**
     * Check if this cell has a non-null intersection with the given shape
     * **/
    //private bool OverlapsShape(Shape shape)
    //{
    //    Contour shapeContour = shape.m_contour;

    //    for (int i = 0; i != shapeContour.Count; i++)
    //    {
    //        Vector2 contourEdgePoint1 = shapeContour[i];
    //        Vector2 contourEdgePoint2 = shapeContour[i == shape.m_contour.Count - 1 ? 0 : i + 1];

    //        if (GeometryUtils.TwoSegmentsIntersect(contourEdgePoint1, contourEdgePoint2, m_voxelA.m_position, m_voxelB.m_position) ||
    //            GeometryUtils.TwoSegmentsIntersect(contourEdgePoint1, contourEdgePoint2, m_voxelB.m_position, m_voxelD.m_position) ||
    //            GeometryUtils.TwoSegmentsIntersect(contourEdgePoint1, contourEdgePoint2, m_voxelD.m_position, m_voxelC.m_position) ||
    //            GeometryUtils.TwoSegmentsIntersect(contourEdgePoint1, contourEdgePoint2, m_voxelC.m_position, m_voxelA.m_position))
    //        {
    //            if (!ShareOnlyEdgesOrPointsWithShape(shape)) //this is not an 'empty' intersection
    //                return true;
    //        }
    //    }

    //    //Shape does not intersect with one of the 4 edges of the cell, the only remaining possibility is that the shape itself is contained inside the cell (very unlikely but have to test it anyway)
    //    //Thus test if the barycentre of the shape is inside the cell. That means that all shape vertices are inside the cell because of no intersection with cell edges
    //    if (ContainsPoint(shape.GetBarycentre()))
    //        return true;

    //    return false;
    //}

    /**
     * Use this method to eliminate case of intersections where two shapes are only touching at some edges or points but with no real intersection
     * (i.e their intersection should be null despite the fact that OverlapsShape returns true)
     * **/
    public bool ShareOnlyEdgesOrPointsWithShape(Shape shape)
    {
        Contour shapeContour = shape.m_contour;
        Contour cellContour = new Contour(4);
        cellContour.Add(m_voxelA.m_position);
        cellContour.Add(m_voxelB.m_position);
        cellContour.Add(m_voxelD.m_position);
        cellContour.Add(m_voxelC.m_position);

        //locate shape contour vertices about cell contour
        for (int i = 0; i != shapeContour.Count; i++)
        {
            Vector2 shapeContourVertex = shapeContour[i];
            if (!this.ContainsPointOnContour(shapeContourVertex)) //the point is either inside or outside the shape contour, but not on it
            {
                if (this.ContainsPoint(shapeContourVertex)) //we have a shape contour vertex inside the cell contour, there is an intersection
                    return false;
            }
        }

        //locate cell contour vertices about shape contour
        for (int i = 0; i != 4; i++)
        {
            Vector2 cellContourVertex = cellContour[i];
            if (!shapeContour.ContainsPoint(cellContourVertex)) //the point is either inside or outside the cell contour, but not on it
            {
                if (shape.ContainsPoint(cellContourVertex)) //we have a cell contour vertex inside the shape contour, there is an intersection
                    return false;
            }
        }

        return true;
    }

    /**
     * Check if this cell contains the parameter 'point'
     * **/
    private bool ContainsPoint(Vector2 point)
    {
        return (point.x >= m_voxelA.m_position.x && point.x <= m_voxelB.m_position.x) && (point.y >= m_voxelA.m_position.y && point.y <= m_voxelC.m_position.y);
    }

    /**
     * Check if this cell contour contains the parameter 'point'
     * **/
    private bool ContainsPointOnContour(Vector2 point)
    {
        return ((point.x >= m_voxelA.m_position.x  && point.x <= m_voxelB.m_position.x && point.y == m_voxelA.m_position.y) ||
                (point.x >= m_voxelA.m_position.x  && point.x <= m_voxelB.m_position.x && point.y == m_voxelC.m_position.y) ||
                (point.y >= m_voxelA.m_position.y  && point.y <= m_voxelC.m_position.x && point.x == m_voxelA.m_position.x) ||
                (point.y >= m_voxelA.m_position.y  && point.y <= m_voxelC.m_position.x && point.x == m_voxelB.m_position.x));
    }

    /**
     * Set opacity on all vertices contained in this cell
     * **/
    public void SetOpacity(float opacity)
    {
        m_parentMesh.SetCellTintOpacity(this, opacity);     
    }

    /**
     * Set color on all vertices contained in this cell
     * **/
    public void SetColor(Color color)
    {
        m_parentMesh.SetCellTintColor(this, color);
    }
}