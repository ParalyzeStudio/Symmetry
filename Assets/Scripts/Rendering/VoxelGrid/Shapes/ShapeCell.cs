using UnityEngine;

public class ShapeCell
{
    public Vector3 m_position { get; set; }

    public ShapeVoxel m_voxelA { get; set; }
    public ShapeVoxel m_voxelB { get; set; }
    public ShapeVoxel m_voxelC { get; set; }
    public ShapeVoxel m_voxelD { get; set; }

    public int m_startIndex { get; set; } //the start index of this cell inside the mesh vertices array
    public int m_endIndex { get; set; }

    public ShapeCell(ShapeVoxel a, ShapeVoxel b, ShapeVoxel c, ShapeVoxel d)
    {
        m_voxelA = a;
        m_voxelB = b;
        m_voxelC = c;
        m_voxelD = d;

        m_position = 0.25f * (m_voxelA.m_position + m_voxelB.m_position + m_voxelC.m_position + m_voxelD.m_position);
    }
}