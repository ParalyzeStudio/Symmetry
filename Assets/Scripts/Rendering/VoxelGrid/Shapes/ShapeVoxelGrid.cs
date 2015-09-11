using UnityEngine;

public class ShapeVoxelGrid
{
    private float m_voxelSize;
    private ShapeVoxel[] m_voxels;

    /***
     * Init the voxel grid with a size and a density (number of voxels per grid unit)
     * ***/
    public void Init(Grid parentGrid, int density)
    {
        Vector2 gridSize = parentGrid.m_gridSize;
        int xVoxelsCount = (parentGrid.m_numColumns - 1) * (density - 1) + 1; //the number of voxels along the x-dimension of the grid
        int yVoxelsCount = (parentGrid.m_numLines - 1) * (density - 1) + 1; //the number of voxels along the y-dimension of the grid
        m_voxels = new ShapeVoxel[xVoxelsCount * yVoxelsCount];
        m_voxelSize = parentGrid.m_gridSpacing / (density - 1);

        for (int i = 0, y = 0; y < yVoxelsCount; y++)
        {
            for (int x = 0; x < xVoxelsCount; x++, i++)
            {
                Vector2 voxelPosition = new Vector2();
                CreateVoxel(i, x, y);
            }
        }
    }

    //private void CreateVoxel(int i, int x, int y)
    //{
    //    ShapeVoxel voxel = new ShapeVoxel();

    //    GameObject o = Instantiate(voxelPrefab) as GameObject;
    //    o.transform.parent = transform;
    //    o.transform.localPosition = new Vector3((x + 0.5f) * m_voxelSize, (y + 0.5f) * m_voxelSize);
    //    o.transform.localScale = Vector3.one * m_voxelSize * 0.1f;
    //    m_voxelMaterials[i] = o.GetComponent<MeshRenderer>().material;
    //    m_voxels[i] = voxel;
    //}
}

