using UnityEngine;
using System.Collections.Generic;

public class ShapeVoxelGrid : MonoBehaviour
{
    public GameObject m_colorQuadPfb;

    private float m_voxelSize;
    private ShapeVoxel[] m_voxels;

    private GameObject m_voxelsHolder;
    private int m_size;

    /***
     * Init the voxel grid with a size and a density (number of voxels per grid unit)
     * ***/
    public void Init(int density)
    {
        m_voxelsHolder = new GameObject("Voxels");
        m_voxelsHolder.transform.parent = this.transform;
        m_voxelsHolder.transform.localPosition = Vector3.zero;

        Grid parentGrid = this.GetComponent<Grid>();
        Vector2 gridSize = parentGrid.m_gridSize;

        int xVoxelsCount = (parentGrid.m_numColumns - 1) * (density - 1) + 1; //the number of voxels along the x-dimension of the grid
        int yVoxelsCount = (parentGrid.m_numLines - 1) * (density - 1) + 1; //the number of voxels along the y-dimension of the grid
        m_size = xVoxelsCount * yVoxelsCount;
        m_voxels = new ShapeVoxel[m_size];
        m_voxelSize = parentGrid.m_gridSpacing / (density - 1);

        for (int i = 0, y = 0; y < yVoxelsCount; y++)
        {
            for (int x = 0; x < xVoxelsCount; x++, i++)
            {
                CreateVoxel(i, x, y);
            }
        }
    }

    /**
     * Creates a voxel at index i and position (x, y) = (column, line)
     * **/
    private void CreateVoxel(int i, int x, int y)
    {
        Grid parentGrid = this.GetComponent<Grid>();
        Vector2 gridSize = parentGrid.m_gridSize;
        
        Vector2 voxelPosition = new Vector2(x * m_voxelSize - 0.5f * gridSize.x, y * m_voxelSize - 0.5f * gridSize.y);
        ShapeVoxel voxel = new ShapeVoxel(voxelPosition);

        GameObject debugQuad = Instantiate(m_colorQuadPfb) as GameObject;
        debugQuad.transform.parent = m_voxelsHolder.transform;
        debugQuad.transform.localPosition = GeometryUtils.BuildVector3FromVector2(voxelPosition, 0);
        debugQuad.transform.localScale = Vector3.one * m_voxelSize * 0.1f;

        debugQuad.GetComponent<ColorQuad>().Init(null);

        m_voxels[i] = voxel;
    }

    /**
     * Refresh the mesh by drawing new polygons where current shapes are
     * **/
    public void Refresh()
    {        
        AssignShapesToVoxels();
        Triangulate();
    }

    /**
     * Assign one single shape or none (set shape to null in this case) to every voxel in this grid
     **/
    public void AssignShapesToVoxels()
    {
        List<GameObject> shapesObjects = this.gameObject.GetComponent<Shapes>().m_shapesObjects;

        for (int i = 0; i != shapesObjects.Count; i++)
        {
            Shape shape = shapesObjects[i].GetComponent<ShapeRenderer>().m_shape;
        }
    }

    /**
     * Triangulate polygons that are being drawn
     * **/
    private void Triangulate()
    {
        //m_vertices.Clear();
        //m_triangles.Clear();
        //m_mesh.Clear();

        //if (m_xNeighbor != null)
        //{
        //    m_dummyX.BecomeXDummyOf(m_xNeighbor.m_voxels[0], m_gridSize);
        //}
        //FillFirstRowCache();
        //TriangulateCellRows();

        //if (m_yNeighbor != null)
        //{
        //    TriangulateGapRow();
        //}

        //m_mesh.vertices = m_vertices.ToArray();
        //m_mesh.triangles = m_triangles.ToArray();
    }

    private void TriangulateCellRows()
    {
        //int cellsCount = m_ - 1;
        //for (int i = 0, y = 0; y < cells; y++, i++)
        //{
        //    SwapRowCaches();
        //    CacheFirstCorner(m_voxels[i + m_resolution]);
        //    CacheNextMiddleEdge(m_voxels[i], m_voxels[i + m_resolution]);

        //    for (int x = 0; x < cells; x++, i++)
        //    {
        //        Voxel
        //               a = m_voxels[i],
        //               b = m_voxels[i + 1],
        //               c = m_voxels[i + m_resolution],
        //               d = m_voxels[i + m_resolution + 1];
        //        int cacheIndex = x * 2;
        //        CacheNextEdgeAndCorner(cacheIndex, c, d);
        //        CacheNextMiddleEdge(b, d);
        //        TriangulateCell(cacheIndex, a, b, c, d);
        //    }

        //    if (m_xNeighbor != null)
        //    {
        //        //Debug.Log("TriangulateGapCell i:" + i + " y:" + y);
        //        TriangulateGapCell(i);
        //    }
        //}
    }
}

