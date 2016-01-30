using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

/**
 * Class that renders a mesh formed with grid triangles
 * **/
public class ShapeMesh : TexturedMesh
{
    public const float CELL_APPARITION_INTERVAL = 0.05f;
    public const int SHAPE_TEXTURES_TILING = 20; //textures is 20x20 squares

    public Shape m_shapeData { get; set; } //the shape data we want to render

    //cells
    public bool m_renderedWithCells { get; set; }
    public ShapeCell[] m_cells { get; set; }

    //sweeping line associated to this dynamic shape
    public Axis.SweepingLine m_sweepingLine { get; set; }

    private GameScene m_gameScene; //global instance of the Shapes holder

    public override void Init(Material material = null)
    {
        base.Init(material);
        m_mesh.name = "ShapeMesh";
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

        Grid grid = GetGrid();

        if (bCellRendering) //render shape using cells
        {
            GetVoxelGrid().AssignShapeToVoxels(m_shapeData);
            BuildCells();
            //GetVoxelGrid().UnassignShapeFromVoxels(m_shapeData);
        }
        else //render shape with simple triangulation
        {
            //Simply add the shape triangles to the mesh
            for (int i = 0; i != m_shapeData.m_triangles.Count; i++)
            {
                GridTriangle shapeTriangle = m_shapeData.m_triangles[i];

                Vector2 pt1 = grid.GetPointWorldCoordinatesFromGridCoordinates(shapeTriangle.m_points[0]);
                Vector2 pt2 = grid.GetPointWorldCoordinatesFromGridCoordinates(shapeTriangle.m_points[1]);
                Vector2 pt3 = grid.GetPointWorldCoordinatesFromGridCoordinates(shapeTriangle.m_points[2]);

                AddTriangle(pt1, pt3, pt2);
            }

            //Color tintColor = m_shapeData.m_color;
            SetTintColor(m_shapeData.m_color);

            m_meshVerticesDirty = true;
            m_meshIndicesDirty = true;
            m_meshColorsDirty = true;
        }

        RefreshMesh(); //refresh mesh immediately, do not wait next frame Update call
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
                    ShapeCell cell = new ShapeCell(i, this, a, b, c, d);
                    //Debug.Log("a:" + a.m_position + " b:" + b.m_position + " c:" + c.m_position + " d:" + d.m_position);
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
            NullifyCellAtIndex(i);
        }
    }

    /**
     * Sweep mesh cells and reveal them if they have just crossed the sweeping line and are now on the 'side' specified by the associated paramater.
     * return true if all cells have been swept, false otherwise
     * **/
    public bool SweepCellsWithLine(Axis.SweepingLine line, bool bLeftSide)
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
            Vector2 cellWorldPosition = GetGrid().GetPointWorldCoordinatesFromGridCoordinates(cell.m_gridPosition);
            float det = MathUtils.Determinant(m_sweepingLine.PointA, m_sweepingLine.PointB, cellWorldPosition);
            if ((bLeftSide && det >= 0) || (!bLeftSide && det <= 0))
            {
                cell.TriangulateAndShow();
            }
        }

        if (allCellsSwept && m_renderedWithCells) //we just swept all cells, we can now render this whole mesh with simple triangles
        {
            OnMeshSwept();
            return true;
        }

        return false;
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
    public void NullifyCellAtIndex(int index)
    {
        if (m_cells[index] != null)
        {
            m_cells[index] = null;
        }
    }

    /**
     * Callback called when all cells of this mesh have been swept
     * **/
    private void OnMeshSwept()
    {
        m_shapeData.FinalizeClippingOperations();
    }

    public void AddFullQuadCell(ShapeCell cell)
    {
        Grid grid = GetGrid();

        cell.m_startIndex = m_vertices.Count;
        Vector2 ptA = grid.GetPointWorldCoordinatesFromGridCoordinates(cell.m_voxelA.m_gridPosition);
        Vector2 ptB = grid.GetPointWorldCoordinatesFromGridCoordinates(cell.m_voxelB.m_gridPosition);
        Vector2 ptC = grid.GetPointWorldCoordinatesFromGridCoordinates(cell.m_voxelC.m_gridPosition);
        Vector2 ptD = grid.GetPointWorldCoordinatesFromGridCoordinates(cell.m_voxelD.m_gridPosition);
        AddQuad(ptA, ptC, ptB, ptD);
        cell.m_endIndex = m_vertices.Count - 1;
    }

    public void AddClippedCell(ShapeCell cell, List<Shape> clipResult)
    {
        cell.m_startIndex = m_vertices.Count;
        Grid grid = GetGrid();

        for (int i = 0; i != clipResult.Count; i++)
        {
            Shape resultShape = clipResult[i];
            for (int j = 0; j != resultShape.m_triangles.Count; j++)
            {
                GridTriangle triangle = resultShape.m_triangles[j];
                Vector2 pt1 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[0]);
                Vector2 pt2 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[1]);
                Vector2 pt3 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[2]);
                AddTriangle(pt1, pt2, pt3);
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

    /**
    * Set the tint color of this cell
    * **/
    public void SetCellTintColor(ShapeCell cell, Color color)
    {        
        for (int i = cell.m_startIndex; i != cell.m_endIndex + 1; i++)
        {
            m_colors[i] = color;
        }

        m_meshColorsDirty = true;
    }

    /**
     * Return the tint color of this cell (i.e the color of the first vertex of this cell as all cell vertices share the same color)
     * **/
    public Color GetCellTintColor(ShapeCell cell)
    {
        return m_colors[cell.m_startIndex];
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
        vertex += 0.5f * new Vector3(grid.m_gridSize.x, grid.m_gridSize.y, 0); //offset by half the grid size so the bottom left vertex is at (0,0)
        Vector2 uv = localVertex / (voxelGrid.m_voxelSize * grid.m_gridSpacing); //normalize coordinates of the vertex so it is in UV mode
        uv /= SHAPE_TEXTURES_TILING; //divide uvs by the number of tiles along each dimension inside the texture

        return uv;
    }

    /**
     * Set the tint color of this shape
     * **/
    public override void SetTintColor(Color color)
    {
        m_shapeData.m_color = color;
        base.SetTintColor(color);
    }

    /**
     * Tells if a point is contained inside the visible parts of this mesh (i.e with full opacity)
     * **/
    //public bool ContainsPointInsideVisibleMesh(Vector2 point)
    //{
    //    if (m_renderedWithCells)
    //    {
    //        for (int i = 0; i != m_cells.Length; i++)
    //        {
    //            ShapeCell cell = m_cells[i];
    //            if (cell != null)
    //            {
    //                float cellOpacity = cell.GetColor().a;
    //                if (cellOpacity > 0)
    //                {
    //                    return true;
    //                }
    //            }
    //        }
    //    }
    //    else
    //        return m_shapeData.ContainsPoint(point);

    //    return false;
    //}

    public GameScene GetGameScene()
    {
        if (m_gameScene == null)
            m_gameScene = ((GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene);

        return m_gameScene;
    }

    private Grid GetGrid()
    {
        return GetGameScene().m_grid;
    }

    private ShapeVoxelGrid GetVoxelGrid()
    {
        return GetGameScene().m_voxelGrid;
    }

    public CallFuncHandler GetCallFuncHandler()
    {
        return GetGameScene().GetCallFuncHandler();
    }

    public Shapes GetShapesHolder()
    {
        return GetGameScene().m_shapesHolder;
    }

    public ClippingManager GetClippingManager()
    {
        return GetGameScene().GetClippingManager();
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
}