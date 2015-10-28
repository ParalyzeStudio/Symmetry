using UnityEngine;
using System.Collections.Generic;

public class ColorMesh : BaseMesh
{
    protected List<Color> m_colors;
    public bool m_meshColorsDirty { get; set; }

    public override void Init(Material material = null)
    {
        base.Init(material);
        m_mesh.name = "ColorMesh";
        m_colors = new List<Color>();
        m_meshColorsDirty = false;
    }

    protected override void ClearMesh()
    {
        base.ClearMesh();
        m_colors.Clear();
    }

    /**
     * Set colors array for this mesh
     * **/
    public void SetColors(List<Color> colors)
    {
        if (colors.Count != m_vertices.Count)
            throw new System.Exception("colors array has to be the same size as vertices array");

        m_colors = colors;
        m_meshColorsDirty = true;
    }

    protected override void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        base.AddTriangle(point1, point2, point3);

        for (int i = 0; i != 3; i++)
        {
            m_colors.Add(new Color(0, 0, 0, 0)); //add empty color
        }
        m_meshColorsDirty = true;
    }

    protected override void AddQuad(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        base.AddQuad(point1, point2, point3, point4);

        for (int i = 0; i != 4; i++)
        {
            m_colors.Add(new Color(0, 0, 0, 0)); //add empty color
        }
        m_meshColorsDirty = true;
    }

    public override void RefreshMesh()
    {
        base.RefreshMesh();

        if (m_meshColorsDirty)
        {
            m_mesh.colors = m_colors.ToArray();
            m_meshColorsDirty = false;
        }
    }
}

