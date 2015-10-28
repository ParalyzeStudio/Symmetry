using UnityEngine;

public class TriangleMesh : ColorMesh
{
    public override void Init(Material material = null)
    {
        base.Init(material);
        m_mesh.name = "TriangleMesh";
    }

    /** Render this triangle
     * **/
    public void Render(Vector3[] vertices, Color color, bool bDoubleSided = false)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Init();
            meshFilter = GetComponent<MeshFilter>();
        }

        Mesh triangleMesh = meshFilter.sharedMesh;

        int[] triangleIndices = new int[bDoubleSided ? 6 : 3];
        triangleIndices[0] = 0;
        triangleIndices[1] = 2;
        triangleIndices[2] = 1;
        if (bDoubleSided)
        {
            triangleIndices[3] = 0;
            triangleIndices[4] = 1;
            triangleIndices[5] = 2;
        }

        Color[] triangleColors = new Color[3];
        triangleColors[0] = color;
        triangleColors[1] = color;
        triangleColors[2] = color;

        //Build the actual mesh
        triangleMesh.vertices = vertices;
        triangleMesh.triangles = triangleIndices;
        triangleMesh.colors = triangleColors;

        //set the circle color
        SetColor(color);

        //Set the mesh to the MeshFilter component
        meshFilter.sharedMesh = triangleMesh;
    }

    public void SetColor(Color color)
    {
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;

        Color[] colors = new Color[3];
        for (int i = 0; i != mesh.vertexCount; i++)
        {
            colors[i] = color;
        }

        mesh.colors = colors;
    }
}
