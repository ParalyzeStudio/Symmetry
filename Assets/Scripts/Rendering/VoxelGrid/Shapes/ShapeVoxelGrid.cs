using UnityEngine;
using System.Collections.Generic;

public class ShapeVoxelGrid : MonoBehaviour
{    
    public Material m_voxelMaterial;
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
        voxelWorldPosition.z = 0;

        ShapeVoxel voxel = new ShapeVoxel(voxelWorldPosition);

        m_voxels[i] = voxel;
    }

    /**
     * Find voxels that are inside the parameter 'shape' and add a reference to them
     * **/
    public void AssignShapeToVoxels(Shape shape)
    {
        for (int iVoxelIdx = 0; iVoxelIdx != m_voxels.Length; iVoxelIdx++)
        {
            ShapeVoxel voxel = m_voxels[iVoxelIdx];
            if (shape.ContainsPoint(voxel.m_position))
            {               
                voxel.AddOverlappingShape(shape);
            }
        }  
    }

    /**
     * Find voxels that have been referenced by the parameter 'shape' and remove it
     * **/
    public void UnassignShapeFromVoxels(Shape shape)
    {
        for (int iVoxelIdx = 0; iVoxelIdx != m_voxels.Length; iVoxelIdx++)
        {
            ShapeVoxel voxel = m_voxels[iVoxelIdx];              
            voxel.RemoveOverlappingShape(shape);
        }  
    }

    /**
     * Clear each voxel in this grid from shapes that could have been added to them
     * **/
    public void ClearVoxels()
    {
        for (int i = 0; i != m_voxels.Length; i++)
        {
            m_voxels[i].ClearOverlappingShapes();
        }
    }
}

