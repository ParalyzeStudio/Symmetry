using UnityEngine;

public class TriangleMesh : MonoBehaviour
{
    public void Init(Material material = null)
    {
        Mesh mesh = new Mesh();
        mesh.name = "TriangleMesh";

        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

        if (material != null)
            meshRenderer.sharedMaterial = material;
    }

    /** Render this circle as a polygon with a certain number of segments
     * The more segments the polygon has the more it will look like a circle but the drawback is that more vertices have to be rendered
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
