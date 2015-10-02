﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

/**
 * Class that renders a mesh formed with grid triangles
 * **/
public class ShapeMesh : TexturedMesh
{
    //public const float SWEEP_LINE_ANGLE = -45.0f;
    //public const float SWEEP_LINE_SPEED = 300.0f;
    public const float CELL_APPARITION_INTERVAL = 0.05f;
    public const int SHAPE_TEXTURES_TILING = 6; //textures is 6x6 squares

    public Shape m_shapeData { get; set; } //the shape data we want to render

    //Prefabs
    public Material m_shapeTilesMaterial;
    public GameObject m_shapeCellPfb;

    //cells
    public bool m_renderedWithCells { get; set; }
    //public List<ShapeCell> m_cells { get; set; }
    public ShapeCell[] m_cells { get; set; }

    //sweep line
    //private Vector2 m_sweepLineDirection;
    //private Vector2 m_sweepLinePoint;
    //private float m_sweepLineSpeed;
    //private bool m_sweeping;

    //public GameObject m_sweepLinePfb;
    //private GameObject m_debugSweepLineObject;

    private Grid m_grid; //global instance of the game scene grid
    private ShapeVoxelGrid m_voxelGrid; //global instance of the game scene voxel grid
    private CallFuncHandler m_callFuncHandler; //global instance of the CallFunc Handler
    private Shapes m_shapesHolder; //global instance of the Shapes holder
    private GameScene m_gameScene; //global instance of the Shapes holder

    public override void Init()
    {
        base.Init();
        m_mesh.name = "ShapeMesh";

        //m_cells = new List<ShapeCell>();
    }

    /**
     * Assign a shape to this mesh
     * **/
    public void SetShapeData(Shape shapeData)
    {
        m_shapeData = shapeData;
        shapeData.m_parentMesh = this;
    }

    /**
     * Render the shape by either building the shape cell by cell and displaying it later with sweeping lines
     * or triangulate the contour and add triangles to the mesh
     * **/
    public void Render(bool bCellRendering = false)
    {
        m_renderedWithCells = bCellRendering;
        ClearMesh();
        m_mesh.Clear();

        if (bCellRendering) //render shape using cells
        {
            AssignVoxels();
            BuildCells();
            //TODO clear the voxel grid after building cells
        }
        else //render shape with simple triangulation
        {
            //Simply add the shape triangles to the mesh
            for (int i = 0; i != m_shapeData.m_triangles.Count; i++)
            {
                BaseTriangle shapeTriangle = m_shapeData.m_triangles[i];
                AddTriangle(shapeTriangle.m_points[0], shapeTriangle.m_points[2], shapeTriangle.m_points[1]);
            }

            SetTintColor(m_shapeData.m_color);

            m_meshVerticesDirty = true;
            m_meshIndicesDirty = true;
            m_meshColorsDirty = true;
        }
    }

    /**
     * Assign this shape to every voxel contained in it
     **/
    private void AssignVoxels()
    {
        ShapeVoxel[] voxels = GetVoxelGrid().Voxels;

        for (int iVoxelIdx = 0; iVoxelIdx != voxels.Length; iVoxelIdx++)
        {
            ShapeVoxel voxel = voxels[iVoxelIdx];

            if (m_shapeData.ContainsPoint(voxel.m_position))
            {
                voxel.AddOverlappingShape(m_shapeData);
            }
        }        
    }

    /**
     * Build the cells on this shape mesh based on the assigned voxels
     * **/
    private void BuildCells()
    {
        ShapeVoxelGrid voxelGrid = GetVoxelGrid();

        int XCellsCount = voxelGrid.XVoxelsCount - 1;
        int YCellsCount = voxelGrid.YVoxelsCount - 1;
        m_cells = new ShapeCell[XCellsCount * YCellsCount];

        for (int i = 0, y = 0; y != YCellsCount; y++)
        {
            for (int x = 0; x != XCellsCount; x++, i++)
            {
                ShapeVoxel
                           a = voxelGrid.Voxels[y + i],
                           b = voxelGrid.Voxels[y + i + 1],
                           c = voxelGrid.Voxels[y + i + voxelGrid.XVoxelsCount],
                           d = voxelGrid.Voxels[y + i + voxelGrid.XVoxelsCount + 1];

                //only build a cell if one of the 4 voxels is contained in this shape               
                if (a.IsOverlappedByShape(this.m_shapeData) ||
                    b.IsOverlappedByShape(this.m_shapeData) ||
                    c.IsOverlappedByShape(this.m_shapeData) ||
                    d.IsOverlappedByShape(this.m_shapeData))
                {
                    GameObject cellObject = Instantiate(m_shapeCellPfb);
                    cellObject.name = "Cell";
                    cellObject.transform.parent = this.transform;
                    ShapeCell cell = cellObject.GetComponent<ShapeCell>();
                    cell.Init(i, this, a, b, c, d);
                    if ((i % XCellsCount) > 0) //cell is not at the beginning of a line
                    {
                        ShapeCell leftCell = m_cells[i - 1];
                        if (leftCell != null)
                        {
                            cell.m_leftCell = leftCell;
                            leftCell.m_rightCell = cell;
                        }
                    }
                    if (i > XCellsCount - 1) //not the first line
                    {
                        ShapeCell bottomCell = m_cells[i - XCellsCount];
                        if (bottomCell != null)
                        {
                            cell.m_bottomCell = bottomCell;
                            bottomCell.m_topCell = cell;
                        }
                    }
                    //m_cells.Add(cell);
                    m_cells[i] = cell;
                }
                else
                    m_cells[i] = null;
            }
        }
    }

    /**
     * Destroy all cells and clear the associated array
     * **/
    public void DestroyCells()
    {
        for (int i = 0; i != m_cells.Length; i++)
        {
            DestroyAndNullifyCellAtIndex(i);
        }
    }

    /**
     * Sweep mesh cells and reveal them if they have just crossed the sweeping line and are now on the 'side' specified by the associated paramater
     * **/
    public void SweepCellsWithLine(AxisRenderer.SweepingLine line, bool bLeftSide)
    {
        bool allCellsSwept = true;
        for (int i = 0; i != m_cells.Length; i++)
        {
            ShapeCell cell = m_cells[i];
            if (cell == null || cell.Swept || cell.Showing)
                continue;
            else
                allCellsSwept = false;

            //Test the position of this cell about the axis
            float det = MathUtils.Determinant(line.PointA, line.PointB, cell.m_position);
            if ((bLeftSide && det >= 0) || (!bLeftSide && det <= 0))
            {
                cell.TriangulateAndShow();
            }
        }

        if (allCellsSwept && m_renderedWithCells) //we just swept all cells, we can now render this whole mesh with simple triangles
        {
            //OnMeshSwept();
        }
    }

    /**
     * Sweep mesh cells and reveal them if they have just crossed the sweeping circle
     * **/
    public void SweepCellsWithCircle()
    {

    }

    /**
     * Set a cell to null (for instance if the result of triangulation leads to an empty cell)
     * **/
    public void DestroyAndNullifyCellAtIndex(int index)
    {
        if (m_cells[index] != null)
        {
            Destroy(m_cells[index].gameObject);
            m_cells[index] = null;
        }
    }

    /**
     * Callback called when all cells of this mesh have been swept
     * **/
    private void OnMeshSwept()
    {
        Debug.Log("OnMeshSwept");
        DestroyCells();
        Render(false);
        //GetShapesHolder().MakeDynamicShapeStatic(m_shapeData);
    }

    //public void ShowVoxelCell(ShapeCell cell)
    //{
    //    TriangulateVoxelCell(cell);
    //}

    /**
     * Triangulate some voxel cell and add vertices to this mesh
     * **/
    //public bool TriangulateVoxelCell(ShapeCell cell)
    //{
    //    int cellType = 0;
    //    if (cell.m_voxelA.IsOverlappedByShape(this.m_shapeData))
    //    {
    //        cellType |= 1;
    //    }
    //    if (cell.m_voxelB.IsOverlappedByShape(this.m_shapeData))
    //    {
    //        cellType |= 2;
    //    }
    //    if (cell.m_voxelC.IsOverlappedByShape(this.m_shapeData))
    //    {
    //        cellType |= 4;
    //    }
    //    if (cell.m_voxelD.IsOverlappedByShape(this.m_shapeData))
    //    {
    //        cellType |= 8;
    //    }

    //    if (cellType > 0)
    //    {
    //        if (cellType == 15)
    //        {
    //            TriangulateFullQuadCell(cell);
    //            return true;
    //        }
    //        else
    //            return ClipCell(cell);
    //    }

    //    return false;
    //}

    /**
     * TRIANGULATION CASES
     * **/
    //private void TriangulateFullQuadCell(ShapeCell cell)
    //{
    //    cell.m_startIndex = m_vertices.Count;
    //    AddQuad(cell.m_voxelA.m_position, cell.m_voxelC.m_position, cell.m_voxelB.m_position, cell.m_voxelD.m_position);
    //    cell.m_endIndex = m_vertices.Count - 1;
    //}

    //private bool ClipCell(ShapeCell cell)
    //{
    //    Contour cellContour = new Contour(4);
    //    cellContour.Add(cell.m_voxelA.m_position);
    //    cellContour.Add(cell.m_voxelB.m_position);
    //    cellContour.Add(cell.m_voxelD.m_position);
    //    cellContour.Add(cell.m_voxelC.m_position);
    //    Shape cellShape = new Shape(false, cellContour);
    //    List<Shape> clipResult = ClippingBooleanOperations.ShapesOperation(this.m_shapeData, cellShape, ClipType.ctIntersection);

    //    if (clipResult.Count == 0) //no intersection
    //        return false;

    //    cell.m_startIndex = m_vertices.Count;

    //    for (int i = 0; i != clipResult.Count; i++)
    //    {
    //        Shape resultShape = clipResult[i];
    //        resultShape.Triangulate();
    //        for (int j = 0; j != resultShape.m_triangles.Count; j++)
    //        {
    //            BaseTriangle triangle = resultShape.m_triangles[j];
    //            AddTriangle(triangle.m_points[0], triangle.m_points[2], triangle.m_points[1]);
    //        }
    //    }
    //    cell.m_endIndex = m_vertices.Count - 1;

    //    return true;
    //}

    public void AddFullQuadCell(ShapeCell cell)
    {
        cell.m_startIndex = m_vertices.Count;
        AddQuad(cell.m_voxelA.m_position, cell.m_voxelC.m_position, cell.m_voxelB.m_position, cell.m_voxelD.m_position);
        cell.m_endIndex = m_vertices.Count - 1;
    }

    public void AddClippedCell(ShapeCell cell, List<Shape> clipResult)
    {
        cell.m_startIndex = m_vertices.Count;

        for (int i = 0; i != clipResult.Count; i++)
        {
            Shape resultShape = clipResult[i];
            for (int j = 0; j != resultShape.m_triangles.Count; j++)
            {
                BaseTriangle triangle = resultShape.m_triangles[j];
                AddTriangle(triangle.m_points[0], triangle.m_points[2], triangle.m_points[1]);
            }
        }
        cell.m_endIndex = m_vertices.Count - 1;
    }

    public void SetCellTintOpacity(ShapeCell cell, float opacity)
    {
        for (int i = cell.m_startIndex; i != cell.m_endIndex + 1; i++)
        {
            Color oldColor = m_colors[i];
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, opacity);
            m_colors[i] = newColor;
        }

        m_meshColorsDirty = true;
    }

    public void SetCellTintColor(ShapeCell cell, Color color)
    {        
        for (int i = cell.m_startIndex; i != cell.m_endIndex + 1; i++)
        {
            m_colors[i] = color;
        }

        m_meshColorsDirty = true;
    }

    /**
     * Return the texture UV coordinates associated with this mesh vertex
     * **/
    protected override Vector2 GetUVsForVertex(Vector3 vertex)
    {
        Grid grid = GetGrid();
        ShapeVoxelGrid voxelGrid = GetVoxelGrid();
        Vector2 gridPosition = grid.GetComponent<GameObjectAnimator>().GetPosition();

        Vector3 localVertex = vertex - GeometryUtils.BuildVector3FromVector2(gridPosition, 0); //coordinates of the vertex in grid local coordinates
        localVertex += 0.5f * new Vector3(grid.m_gridSize.x, grid.m_gridSize.y, 0); //offset by half the grid size so the bottom left vertex is at (0,0)
        Vector2 uv = localVertex / voxelGrid.m_voxelSize; //normalize coordinates of the vertex so it is in UV mode
        uv /= SHAPE_TEXTURES_TILING; //divide uvs by the number of tiles along each dimension inside the texture

        return uv;
    }

    /**
     * Set the tint color of this shape
     * **/
    public void SetTintColor(Color color)
    {
        for (int i = 0; i != m_colors.Count; i++)
        {
            m_colors[i] = color;
        }

        m_meshColorsDirty = true;
    }

    private GameScene GetGameScene()
    {
        if (m_gameScene == null)
            m_gameScene = ((GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene);

        return m_gameScene;
    }

    private Grid GetGrid()
    {
        if (m_grid == null)
        {
            if (m_voxelGrid == null)
            {
                m_grid = GetGameScene().GetComponentInChildren<Grid>();
                m_voxelGrid = m_grid.gameObject.GetComponent<ShapeVoxelGrid>();
            }
            else
                m_grid = m_voxelGrid.gameObject.GetComponent<Grid>();
        }

        return m_grid;
    }

    private ShapeVoxelGrid GetVoxelGrid()
    {
        if (m_voxelGrid == null)
        {
            if (m_grid == null)
            {
                m_voxelGrid = GetGameScene().GetComponentInChildren<ShapeVoxelGrid>();
                m_grid = m_voxelGrid.gameObject.GetComponent<Grid>();
            }
            else
                m_voxelGrid = m_grid.gameObject.GetComponent<ShapeVoxelGrid>();
        }

        return m_voxelGrid;
    }

    public CallFuncHandler GetCallFuncHandler()
    {
        if (m_callFuncHandler == null)
            m_callFuncHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<CallFuncHandler>();

        return m_callFuncHandler;
    }

    private Shapes GetShapesHolder()
    {
        if (m_shapesHolder == null)
            m_shapesHolder = GetGameScene().GetComponentInChildren<Shapes>();

        return m_shapesHolder;
    }

    //-------------------------------------------------------------//
    //                        SEPARATION
    //-------------------------------------------------------------//

    /**
     * Renders the shape based on the m_gridTriangles list
     * We can specify a mesh object if we want to render again on this one or pass null to create a new mesh
     * **/
    //public void Render(RenderFaces renderFaces,
    //                   bool bUpdateVertices = true,
    //                   bool bUpdateColors = true,
    //                   bool renderDebugTriangles = false)
    //{
    //    //Build the mesh
    //    Mesh mesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
    //    if (mesh == null)
    //    {
    //        mesh = new Mesh();
    //        mesh.name = "ShapeMesh";
    //        GetComponent<MeshFilter>().sharedMesh = mesh;
    //    }

    //    //reset mesh
    //    mesh.Clear();        

    //    int vertexCount = 0;
    //    int indexCount = 0;
    //    Vector3[] vertices = null;
    //    int[] indices = null;
    //    Vector3[] normals = null;
    //    Color[] colors = null;
    //    Grid grid = null;
    //    if (bUpdateVertices)
    //    {
    //        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
    //        grid = gameScene.m_grid;
    //        vertexCount = 3 * m_shapeData.m_triangles.Count;
    //        if (renderFaces == RenderFaces.DOUBLE_SIDED) //we draw front and back faces
    //            indexCount = 2 * vertexCount;
    //        else
    //            indexCount = vertexCount;
    //        vertices = new Vector3[vertexCount];
    //        indices = new int[indexCount];
    //        normals = new Vector3[vertexCount];            
    //    }

    //    if (bUpdateColors)
    //    {
    //        if (!bUpdateVertices)
    //            vertexCount = 3 * m_shapeData.m_triangles.Count;
    //        colors = new Color[vertexCount];
    //    }
        
    //    int iTriangleIndex = 0;
    //    while (iTriangleIndex != m_shapeData.m_triangles.Count)
    //    {
    //        for (int i = 0; i != 3; i++) //loop over the 3 vertices of this triangle
    //        {
    //            ShapeTriangle shapeTriangle = (ShapeTriangle)m_shapeData.m_triangles[iTriangleIndex];
    //            if (bUpdateVertices)
    //            {
    //                vertices[iTriangleIndex * 3 + i] = grid.GetPointWorldCoordinatesFromGridCoordinates(shapeTriangle.m_points[i]);
    //                normals[iTriangleIndex * 3 + i] = Vector3.forward;
    //            }
    //            if (bUpdateColors)
    //            {
    //                colors[iTriangleIndex * 3 + i] = shapeTriangle.m_color;
    //            }
    //        }
    //        iTriangleIndex++;
    //    }

    //    //populate the array of indices separately
    //    if (bUpdateVertices)
    //    {
    //        iTriangleIndex = 0;
    //        while (iTriangleIndex != m_shapeData.m_triangles.Count)
    //        {
    //            if (renderFaces == RenderFaces.FRONT)
    //            {
    //                indices[iTriangleIndex * 3] = iTriangleIndex * 3;
    //                indices[iTriangleIndex * 3 + 1] = iTriangleIndex * 3 + 1;
    //                indices[iTriangleIndex * 3 + 2] = iTriangleIndex * 3 + 2;
    //            }
    //            else if (renderFaces == RenderFaces.BACK)
    //            {
    //                indices[iTriangleIndex * 3] = iTriangleIndex * 3;
    //                indices[iTriangleIndex * 3 + 1] = iTriangleIndex * 3 + 2;
    //                indices[iTriangleIndex * 3 + 2] = iTriangleIndex * 3 + 1;
    //            }
    //            else //double sided
    //            {
    //                indices[iTriangleIndex * 6] = iTriangleIndex * 3;
    //                indices[iTriangleIndex * 6 + 1] = iTriangleIndex * 3 + 1;
    //                indices[iTriangleIndex * 6 + 2] = iTriangleIndex * 3 + 2;
    //                indices[iTriangleIndex * 6 + 3] = iTriangleIndex * 3;
    //                indices[iTriangleIndex * 6 + 4] = iTriangleIndex * 3 + 2;
    //                indices[iTriangleIndex * 6 + 5] = iTriangleIndex * 3 + 1;
    //            }
    //            iTriangleIndex++;
    //        }
    //    }

    //    if (bUpdateVertices)
    //    {
    //        mesh.vertices = vertices;
    //        mesh.triangles = indices;
    //        mesh.normals = normals;
    //    }
    //    if (bUpdateColors)
    //    {
    //        mesh.colors = colors;
    //    }
    //}

    /**
     * Sets the color of this shape
     * **/
    //public void SetColor(Color color)
    //{
    //    m_shapeData.m_color = color;
    //    m_shapeData.PropagateColorToTriangles();

    //    Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;

    //    Color[] meshColors = mesh.colors;
    //    for (int i = 0; i != mesh.vertexCount; i++)
    //    {
    //        meshColors[i] = color;
    //    }

    //    mesh.colors = meshColors;
    //}

    /**
     * Shift all m_gridTriangles value by a grid coordinates vector (deltaLine, deltaColumn) (i.e translate the shape)
     * **/
    //public void ShiftShapeVertices(Vector2 shift)
    //{
    //    for (int iTriangleIndex = 0; iTriangleIndex != m_shapeData.m_triangles.Count; iTriangleIndex++)
    //    {
    //        BaseTriangle triangle = m_shapeData.m_triangles[iTriangleIndex];
    //        Vector2[] trianglePoints = triangle.m_points;
    //        for (int iPointIndex = 0; iPointIndex != 3; iPointIndex++)
    //        {
    //            trianglePoints[iPointIndex] += shift;
    //        }
    //    }

    //    for (int iContourPointIdx = 0; iContourPointIdx != m_shapeData.m_contour.Count; iContourPointIdx++)
    //    {
    //        m_shapeData.m_contour[iContourPointIdx] += shift;
    //    }
    //}
}