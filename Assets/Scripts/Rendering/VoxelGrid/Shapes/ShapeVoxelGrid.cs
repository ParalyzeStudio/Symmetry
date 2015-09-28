using UnityEngine;
using System.Collections.Generic;

public class ShapeVoxelGrid : MonoBehaviour
{    
    public Material m_voxelMaterial;
    public GameObject m_shapeVoxelPfb;
    public GameObject m_shapeCellPfb;

    public float m_voxelSize { get; set; }

    private ShapeVoxel[] m_voxels;
    public ShapeVoxel[] Voxels
    {
        get
        {
            return m_voxels;
        }
    }

    private GameObject m_voxelsHolder;
    private GameObject m_cellsHolder;
    private int m_size;
    private int m_xVoxelsCount; //the number of voxels along the x-dimension of the grid
    public int XVoxelsCount
    {
        get
        {
            return m_xVoxelsCount;
        }
    }
    private int m_yVoxelsCount; //the number of voxels along the y-dimension of the grid
    public int YVoxelsCount
    {
        get
        {
            return m_yVoxelsCount;
        }
    }

    /***
     * Init the voxel grid with a size and a density (number of voxels per grid unit)
     * ***/
    public void Init(int density)
    {
        m_voxelsHolder = new GameObject("Voxels");
        m_voxelsHolder.transform.parent = this.transform;
        m_voxelsHolder.transform.localPosition = Vector3.zero;

        m_cellsHolder = new GameObject("Cells");
        m_cellsHolder.transform.parent = this.transform;
        m_cellsHolder.transform.localPosition = Vector3.zero;

        Grid parentGrid = this.GetComponent<Grid>();

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
        GameObject voxelObject = (GameObject) Instantiate(m_shapeVoxelPfb);
        voxelObject.name = "Voxel";
        voxelObject.transform.parent = m_voxelsHolder.transform;

        ShapeVoxel voxel = voxelObject.GetComponent<ShapeVoxel>();
        voxel.Init(voxelWorldPosition);

        m_voxels[i] = voxel;
    }
}

