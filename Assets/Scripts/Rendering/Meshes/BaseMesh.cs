using UnityEngine;
using System.Collections.Generic;

public class BaseMesh : MonoBehaviour
{
    //mesh
    protected Mesh m_mesh;
    protected List<Vector3> m_vertices;
    public List<Vector3> Vertices
    {
        get
        {
            return m_vertices;
        }
    }
    protected List<int> m_indices;
    protected int m_maxIndex; //the max index set on this mesh

    public bool m_meshVerticesDirty { get; set; }
    public bool m_meshIndicesDirty { get; set; }

    public virtual void Init()
    {
        m_mesh = new Mesh();
        m_mesh.name = "BaseMesh";
        GetComponent<MeshFilter>().sharedMesh = m_mesh;

        m_vertices = new List<Vector3>();
        m_indices = new List<int>();
        m_maxIndex = -1;

        m_meshVerticesDirty = false;
        m_meshIndicesDirty = false;
    }

    protected virtual void ClearMesh()
    {
        m_vertices.Clear();
        m_indices.Clear();
        m_maxIndex = -1;
    }

    /**
     * Populate this mesh with a complete array of vertices with relevant indices
     * **/
    public void SetVertices(List<Vector3> vertices, List<int> triangles)
    {
        m_vertices = vertices;
        m_meshVerticesDirty = true;
        m_indices = triangles;
        m_meshIndicesDirty = true;
    }

    /**
    * Add a triangle to this mesh
    *   2
     * / \
    * 1---3
    * **/
    protected virtual void AddTriangle(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        //add vertices
        m_vertices.Add(point1);
        m_vertices.Add(point2);
        m_vertices.Add(point3);

        //add indices and colors
        for (int i = 0; i != 3; i++)
        {
            m_indices.Add(m_maxIndex + i + 1); 
        }

        m_maxIndex += 3; //increment the max index

        m_meshVerticesDirty = true;
        m_meshIndicesDirty = true;
    }

    /**
     * Add a quad to this mesh
     * 2--4
     * |  |
     * 1--3
     * **/
    protected virtual void AddQuad(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4)
    {
        //add vertices
        m_vertices.Add(point1);
        m_vertices.Add(point2);
        m_vertices.Add(point3);
        m_vertices.Add(point4);

        //add indices
        m_indices.Add(m_maxIndex + 1);
        m_indices.Add(m_maxIndex + 2);
        m_indices.Add(m_maxIndex + 3);
        m_indices.Add(m_maxIndex + 3);
        m_indices.Add(m_maxIndex + 2);
        m_indices.Add(m_maxIndex + 4);

        m_maxIndex += 4;

        m_meshVerticesDirty = true;
        m_meshIndicesDirty = true;
    }

    public virtual void RefreshMesh()
    {
        //refresh the mesh if needed
        if (m_meshVerticesDirty)
        {
            m_mesh.vertices = m_vertices.ToArray();
            m_meshVerticesDirty = false;
        }
        if (m_meshIndicesDirty)
        {
            m_mesh.triangles = m_indices.ToArray();
            m_meshIndicesDirty = false;
        }
    }

    public virtual void Update()
    {
        RefreshMesh();
    }
}
