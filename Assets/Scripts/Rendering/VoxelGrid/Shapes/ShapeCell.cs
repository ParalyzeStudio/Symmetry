using UnityEngine;

public class ShapeCell : MonoBehaviour
{
    public Vector3 m_position { get; set; }

    public ShapeVoxel m_voxelA { get; set; }
    public ShapeVoxel m_voxelB { get; set; }
    public ShapeVoxel m_voxelC { get; set; }
    public ShapeVoxel m_voxelD { get; set; }

    public int m_startIndex { get; set; } //the start index of this cell inside the mesh vertices array
    public int m_endIndex { get; set; }

    //Variables to handle cell animation
    private bool m_showing;
    private float m_showingElapsedTime;
    private float m_showingDuration;

    private ShapeMesh m_parentMesh;

    public void Init(ShapeMesh parentMesh, ShapeVoxel a, ShapeVoxel b, ShapeVoxel c, ShapeVoxel d)
    {
        m_parentMesh = parentMesh;

        m_voxelA = a;
        m_voxelB = b;
        m_voxelC = c;
        m_voxelD = d;

        m_position = 0.25f * (m_voxelA.m_position + m_voxelB.m_position + m_voxelC.m_position + m_voxelD.m_position);
    }

    public void Show(bool bAnimated = true, float fDuration = 0.5f)
    {
        m_showing = true;
    }

    public void SetColor(Color color)
    {
        m_parentMesh.SetCellTintColor(this, color);
    }
}