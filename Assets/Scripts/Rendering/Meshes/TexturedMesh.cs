using UnityEngine;
using System.Collections.Generic;

public class TexturedMesh : ColorMesh
{
    protected List<Vector2> m_UVs;
    public bool m_meshUVsDirty { get; set; }

    public float m_textureTiling { get; set; }
    public Vector2 m_textureSize { get; set; }

    public override void Init(Material material = null)
    {
        base.Init(material);
        m_mesh.name = "TexturedMesh";
        m_UVs = new List<Vector2>();
        m_meshUVsDirty = false;
        m_textureTiling = 1;
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
    }

    protected override void ClearMesh()
    {
        base.ClearMesh();
        m_UVs.Clear();
    }

    /**
    * Set UVs array for this mesh
    * **/
    public void SetUVs(List<Vector2> UVs)
    {
        if (UVs.Count != m_vertices.Count)
            throw new System.Exception("UVs array has to be the same size as vertices array");

        m_UVs = UVs;
        m_meshUVsDirty = true;
    }

    /**
     * Return the texture UV coordinates associated with this mesh vertex
     * **/
    protected virtual Vector2 GetUVsForVertex(Vector3 vertex)
    {
        if (m_textureSize == Vector2.zero)
            throw new System.Exception("Define a texture size for your mesh");

        float u = vertex.x / m_textureSize.x;
        float v = vertex.y / m_textureSize.y;
        Vector2 uv = new Vector2(u, v);
        uv /= m_textureTiling;

        return uv;
    }

    /**
     * Set the tint color of this textured mesh
     * **/
    public virtual void SetTintColor(Color color)
    {   
        for (int i = 0; i != m_colors.Count; i++)
        {
            m_colors[i] = color;
        }

        m_meshColorsDirty = true;
    }

    public override void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        base.AddTriangle(point1, point2, point3);

        m_UVs.Add(GetUVsForVertex(point1));
        m_UVs.Add(GetUVsForVertex(point2));
        m_UVs.Add(GetUVsForVertex(point3));

        m_meshUVsDirty = true;
    }

    public override void AddQuad(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        base.AddQuad(point1, point2, point3, point4);

        m_UVs.Add(GetUVsForVertex(point1));
        m_UVs.Add(GetUVsForVertex(point2));
        m_UVs.Add(GetUVsForVertex(point3));
        m_UVs.Add(GetUVsForVertex(point4));

        m_meshUVsDirty = true;
    }

    public override void RefreshMesh()
    {
        base.RefreshMesh();

        if (m_meshUVsDirty)
        {
            m_mesh.uv = m_UVs.ToArray();
            m_meshUVsDirty = false;
        }
    }
}

