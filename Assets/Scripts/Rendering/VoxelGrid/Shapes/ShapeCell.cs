using UnityEngine;
using System.Collections.Generic;

public class ShapeCell : MonoBehaviour
{
    private int m_index; //the index of the cell in the parent mesh cell array

    public Vector3 m_position { get; set; }

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

    public bool m_swept { get; set; }

    //neighboring cells
    public ShapeCell m_topCell { get; set; }
    public ShapeCell m_leftCell { get; set; }
    public ShapeCell m_bottomCell { get; set; }
    public ShapeCell m_rightCell { get; set; }

    public void Init(int index, ShapeMesh parentMesh, ShapeVoxel a, ShapeVoxel b, ShapeVoxel c, ShapeVoxel d)
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
    public bool TriangulateAndShow()
    {
        //Triangulate the cell first
        bool bEmptyCell = !Triangulate();

        if (!bEmptyCell)
        {
            Show();
            return true;
        }
        else //triangulation led to an empty cell so remove it
        {
            m_parentMesh.DestroyAndNullifyCellAtIndex(m_index);
            return false;
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
        ShapeCellAnimator cellAnimator = this.gameObject.GetComponent<ShapeCellAnimator>();
        cellAnimator.SetColor(new Color(1, 1, 1, 0));
        cellAnimator.FadeTo(1.0f, 0.02f);
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
        m_parentMesh.AddFullQuadCell(this);

        //m_startIndex = m_parentMesh.Vertices.Count;
        //m_parentMesh.AddQuad(m_voxelA.m_position, m_voxelC.m_position, m_voxelB.m_position, m_voxelD.m_position);
        //m_endIndex = m_parentMesh.Vertices.Count - 1;
    }

    private bool ClipWithParentMesh()
    {
        Contour cellContour = new Contour(4);
        cellContour.Add(m_voxelA.m_position);
        cellContour.Add(m_voxelB.m_position);
        cellContour.Add(m_voxelD.m_position);
        cellContour.Add(m_voxelC.m_position);
        Shape cellShape = new Shape(false, cellContour);
        List<Shape> clipResult = ClippingBooleanOperations.ShapesOperation(this.m_parentMesh.m_shapeData, cellShape, ClipperLib.ClipType.ctIntersection);

        if (clipResult.Count == 0) //no intersection
            return false;

        m_parentMesh.AddClippedCell(this, clipResult);
        return true;
        //m_startIndex = m_parentMesh.Vertices.Count;

        //for (int i = 0; i != clipResult.Count; i++)
        //{
        //    Shape resultShape = clipResult[i];
        //    resultShape.Triangulate();
        //    for (int j = 0; j != resultShape.m_triangles.Count; j++)
        //    {
        //        BaseTriangle triangle = resultShape.m_triangles[j];
        //        m_parentMesh.AddTriangle(triangle.m_points[0], triangle.m_points[2], triangle.m_points[1]);
        //    }
        //}
        //m_endIndex = m_parentMesh.Vertices.Count - 1;

        //return true;
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