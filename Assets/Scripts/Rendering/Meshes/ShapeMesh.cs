using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

/**
 * Class that renders a mesh formed with grid triangles
 * **/
public class ShapeMesh : MonoBehaviour
{  
    public Shape m_shapeData { get; set; } //the shape data we want to render

    private Mesh m_mesh;
    private List<Vector3> m_vertices;
    private List<int> m_indices;
    private int m_maxIndex; //the max index set on this mesh
    private List<Color> m_colors;
    private List<Vector2> m_UVs;

    public Material m_shapeTilesMaterial;

    public List<ShapeCell> m_cells { get; set; }

    public bool m_meshVerticesDirty { get; set; }
    public bool m_meshIndicesDirty { get; set; }
    public bool m_meshColorsDirty { get; set; }
    public bool m_meshUVsDirty { get; set; }

    public void Init(Shape shapeData)
    {
        m_shapeData = shapeData;

        m_mesh = new Mesh();
        m_mesh.name = "ShapeMesh";
        GetComponent<MeshFilter>().sharedMesh = m_mesh;

        m_vertices = new List<Vector3>();
        m_indices = new List<int>();
        m_maxIndex = -1;
        m_colors = new List<Color>();
        m_UVs = new List<Vector2>();

        m_meshVerticesDirty = false;
        m_meshIndicesDirty = false;
        m_meshColorsDirty = false;
        m_meshUVsDirty = false;

        m_cells = new List<ShapeCell>();
    }

    /**
     * Clear the mesh
     * **/
    public void Clear()
    {
        m_vertices.Clear();
        m_indices.Clear();
        m_colors.Clear();
        m_UVs.Clear();
    }

    /**
     * Show the shape
     * **/
    public void Show(bool bAnimated = false)
    {
        if (bAnimated)
        {
            for (int i = 0; i != m_cells.Count; i++)
            {
                ShowVoxelCell(m_cells[i]);
            }
        }
        else
        {

        }
    }

    public void ShowVoxelCell(ShapeCell cell, bool bAnimated = false, float fDuration = 0.0f, float fDelay = 0.0f)
    {
        //First triangulate the cell
        TriangulateVoxelCell(cell);
    }

    /**
     * Triangulate some voxel cell and add vertices to this mesh
     * **/
    public void TriangulateVoxelCell(ShapeCell cell)
    {
        int cellType = 0;
        if (cell.m_voxelA.IsOverlappedByShape(this.m_shapeData))
        {
            cellType |= 1;
        }
        if (cell.m_voxelB.IsOverlappedByShape(this.m_shapeData))
        {
            cellType |= 2;
        }
        if (cell.m_voxelC.IsOverlappedByShape(this.m_shapeData))
        {
            cellType |= 4;
        }
        if (cell.m_voxelD.IsOverlappedByShape(this.m_shapeData))
        {
            cellType |= 8;
        }

        if (cellType > 0)
        {
            if (cellType == 15)
                TriangulateFullQuadCell(cell);
            else
                ClipCell(cell);
        }
    }

    /**
     * TRIANGULATION CASES
     * **/
    private void TriangulateFullQuadCell(ShapeCell cell)
    {
        cell.m_startIndex = m_vertices.Count;
        AddQuad(cell.m_voxelA.m_position, cell.m_voxelC.m_position, cell.m_voxelB.m_position, cell.m_voxelD.m_position);
        cell.m_endIndex = m_vertices.Count;

        m_meshVerticesDirty = true;
        m_meshIndicesDirty = true;
        m_meshColorsDirty = true;
    }

    private void ClipCell(ShapeCell cell)
    {
        Contour cellContour = new Contour(4);
        cellContour.Add(cell.m_voxelA.m_position);
        cellContour.Add(cell.m_voxelB.m_position);
        cellContour.Add(cell.m_voxelD.m_position);
        cellContour.Add(cell.m_voxelC.m_position);
        Shape cellShape = new Shape(false, cellContour);
        List<Shape> clipResult = ClippingBooleanOperations.ShapesOperation(this.m_shapeData, cellShape, ClipType.ctIntersection);

        cell.m_startIndex = m_vertices.Count;

        for (int i = 0; i != clipResult.Count; i++)
        {
            Shape resultShape = clipResult[i];
            resultShape.Triangulate();
            for (int j = 0; j != resultShape.m_triangles.Count; j++)
            {
                BaseTriangle triangle = resultShape.m_triangles[j];
                AddTriangle(triangle.m_points[0], triangle.m_points[2], triangle.m_points[1]);
            }
        }
        cell.m_endIndex = m_vertices.Count;

        m_meshVerticesDirty = true;
        m_meshIndicesDirty = true;
        m_meshColorsDirty = true;
    }

    public void RefreshMesh()
    {
        m_mesh.Clear();

        if (m_meshVerticesDirty)
        {
            m_mesh.vertices = m_vertices.ToArray();
            m_meshVerticesDirty = false;
        }
        if (m_meshIndicesDirty)
        {
            m_mesh.triangles = m_indices.ToArray();
            m_meshIndicesDirty = false;
        }
        if (m_meshColorsDirty)
        {
            m_mesh.colors = m_colors.ToArray();
            m_meshColorsDirty = false;
        }
        if (m_meshUVsDirty)
        {
            m_mesh.uv = m_UVs.ToArray();
            m_meshUVsDirty = false;
        }
    }

    public void BuildUVs()
    {
        GameScene gameScene = (GameScene) GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
        Grid grid = gameScene.GetComponentInChildren<Grid>();
        ShapeVoxelGrid voxelGrid = gameScene.GetComponentInChildren<ShapeVoxelGrid>();
        Vector2 gridPosition = grid.GetComponent<GameObjectAnimator>().GetPosition();

        for (int i = 0; i != m_vertices.Count; i++)
        {
            Vector3 localVertex = m_vertices[i] - GeometryUtils.BuildVector3FromVector2(gridPosition, 0); //coordinates of the vertex in grid local coordinates
            localVertex += 0.5f * new Vector3(grid.m_gridSize.x, grid.m_gridSize.y, 0); //offset by half the grid size so the bottom left vertex is at (0,0)
            Vector2 uv = localVertex / voxelGrid.m_voxelSize; //normalize coordinates of the vertex so it is in UV mode

            m_UVs.Add(uv);
        }

        m_meshUVsDirty = true;
    }

    /**
    * Add a triangle to this shape mesh
    *   2
     * / \
    * 1---3
    * **/
    private void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        m_vertices.Add(point1);
        m_vertices.Add(point2);
        m_vertices.Add(point3);

        for (int i = 0; i != 3; i++)
        {
            m_indices.Add(m_maxIndex + i + 1);
            m_colors.Add(new Color(0, 1, 0, 0.3f));
        }

        m_maxIndex += 3;
    }
    
    /**
     * Add a quad to this shape mesh
     * 2--4
     * |  |
     * 1--3
     * **/
    private void AddQuad(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {        
        m_vertices.Add(point1);
        m_vertices.Add(point2);
        m_vertices.Add(point3);
        m_vertices.Add(point4);

        for (int i = 0; i != 4; i++)
        {
            m_colors.Add(new Color(0, 1, 0, 0.3f));
        }

        m_indices.Add(m_maxIndex + 1);
        m_indices.Add(m_maxIndex + 2);
        m_indices.Add(m_maxIndex + 3);
        m_indices.Add(m_maxIndex + 3);
        m_indices.Add(m_maxIndex + 2);
        m_indices.Add(m_maxIndex + 4);

        m_maxIndex += 4;
    }

    public void SetCellTintColor(ShapeCell cell, Color color)
    {
        for (int i = cell.m_startIndex; i != cell.m_endIndex; i++)
        {
            cell.SetColor(color);
        }
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

    ///**
    // * Sets the color of this shape
    // * **/
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

    ///**
    // * Shift all m_gridTriangles value by a grid coordinates vector (deltaLine, deltaColumn) (i.e translate the shape)
    // * **/
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

