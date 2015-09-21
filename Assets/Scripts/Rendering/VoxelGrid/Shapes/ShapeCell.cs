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

    private ShapeMesh m_parentMesh;

    private bool m_showing;
    public bool Showing
    {
        get
        {
            return m_showing;
        }
    }

    public bool m_swept { get; set; }

    public void Init(ShapeMesh parentMesh, ShapeVoxel a, ShapeVoxel b, ShapeVoxel c, ShapeVoxel d)
    {
        m_parentMesh = parentMesh;

        m_voxelA = a;
        m_voxelB = b;
        m_voxelC = c;
        m_voxelD = d;

        m_position = 0.25f * (m_voxelA.m_position + m_voxelB.m_position + m_voxelC.m_position + m_voxelD.m_position);
    }

    public void Show()
    {
        m_swept = true;

        ShapeCellAnimator cellAnimator = this.gameObject.GetComponent<ShapeCellAnimator>();
        cellAnimator.SetColor(new Color(1, 1, 1, 0));
        cellAnimator.FadeTo(1.0f, 0.02f);
    }

    /**
     * Set opacity on all vertices contained in this cell
     * **/
    public void SetOpacity(float opacity)
    {
        m_parentMesh.SetCellTintOpacity(this, opacity);     
    }

    /**
     * Set color on all vertices contained in this cell
     * **/
    public void SetColor(Color color)
    {
        m_parentMesh.SetCellTintColor(this, color);
    }
}