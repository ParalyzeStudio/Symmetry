using UnityEngine;
using System.Collections.Generic;

public class ShapeVoxelGrid : MonoBehaviour
{
    public GameObject m_colorQuadPfb;
    public Material m_voxelMaterial;

    public float m_voxelSize { get; set; }
    private ShapeVoxel[] m_voxels;

    private GameObject m_voxelsHolder;
    private int m_size;
    private int m_xVoxelsCount; //the number of voxels along the x-dimension of the grid
    private int m_yVoxelsCount; //the number of voxels along the y-dimension of the grid

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

        m_xVoxelsCount = (parentGrid.m_numColumns - 1) * (density - 1) + 1; 
        m_yVoxelsCount = (parentGrid.m_numLines - 1) * (density - 1) + 1;
        m_size = m_xVoxelsCount * m_yVoxelsCount;
        m_voxels = new ShapeVoxel[m_size];
        m_voxelSize = parentGrid.m_gridSpacing / (density - 1);

        for (int i = 0, y = 0; y < m_yVoxelsCount; y++)
        {
            for (int x = 0; x < m_xVoxelsCount; x++, i++)
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
        
        Vector3 voxelLocalPosition = new Vector3(x * m_voxelSize - 0.5f * gridSize.x, y * m_voxelSize - 0.5f * gridSize.y, 0);
        Vector3 voxelWorldPosition = voxelLocalPosition + this.gameObject.GetComponent<GameObjectAnimator>().GetPosition(); //add the position of the grid
        ShapeVoxel voxel = new ShapeVoxel(voxelWorldPosition);

        GameObject debugQuad = Instantiate(m_colorQuadPfb) as GameObject;
        debugQuad.transform.parent = m_voxelsHolder.transform;
        GameObjectAnimator debugQuadAnimator = debugQuad.GetComponent<GameObjectAnimator>();
        debugQuadAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(voxelLocalPosition, 0));
        debugQuadAnimator.SetScale(Vector3.one * m_voxelSize * 0.1f);

        debugQuad.GetComponent<ColorQuad>().Init(Instantiate(m_voxelMaterial));

        debugQuad.GetComponent<ColorQuadAnimator>().SetColor(Color.red);

        voxel.m_debugQuad = debugQuad;
        m_voxels[i] = voxel;
    }

    /**
     * Refresh the mesh by drawing new polygons where current shapes are
     * **/
    public void Refresh()
    {        
        AssignShapesToVoxels();
        TriangulateShapeMeshes();
    }

    /**
     * Assign one single shape or none (set shape to null in this case) to every voxel in this grid
     **/
    public void AssignShapesToVoxels()
    {
        List<GameObject> shapesObjects = this.transform.parent.gameObject.GetComponentInChildren<Shapes>().m_shapesObjects;

        for (int iVoxelIdx = 0; iVoxelIdx != m_voxels.Length; iVoxelIdx++)
        {
            ShapeVoxel voxel = m_voxels[iVoxelIdx];
            voxel.ClearOverlappingShapes(); //clear any previous shape set on this voxel

            for (int iShapeIdx = 0; iShapeIdx != shapesObjects.Count; iShapeIdx++)
            {
                Shape shape = shapesObjects[iShapeIdx].GetComponent<ShapeMesh>().m_shapeData;

                if (shape.ContainsPoint(voxel.m_position))
                {
                    voxel.m_overlappingShapes.Add(shape);

                    voxel.m_debugQuad.GetComponent<ColorQuadAnimator>().SetColor(Color.blue);

                    //Debug.Log("voxelPosition:" + voxelWorldPosition);

                    break; //we break because only one shape can contain this voxel
                }
            }            
        }        
    }

    /**
     * Triangulate polygons that are being drawn
     * **/
    private void TriangulateShapeMeshes()
    {
        TriangulateCellRows();

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
        List<GameObject> shapesObjects = this.transform.parent.gameObject.GetComponentInChildren<Shapes>().m_shapesObjects;

        for (int iShapeIdx = 0; iShapeIdx != shapesObjects.Count; iShapeIdx++)
        {
            ShapeMesh shapeMesh = shapesObjects[iShapeIdx].GetComponent<ShapeMesh>();
            
            for (int i = 0, y = 0; y != m_yVoxelsCount - 1; y++)
            {
                for (int x = 0; x < m_xVoxelsCount - 1; x++, i++)
                {
                    ShapeVoxel
                               a = m_voxels[i],
                               b = m_voxels[i + 1],
                               c = m_voxels[i + m_xVoxelsCount],
                               d = m_voxels[i + m_xVoxelsCount + 1];

                    ShapeCell cell = new ShapeCell(a, b, c, d);
                    shapeMesh.m_cells.Add(cell);
                    shapeMesh.TriangulateVoxelCell(cell);
                }
            }

            shapeMesh.RefreshMesh();
        }

        //int cells = m_ - 1;
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

