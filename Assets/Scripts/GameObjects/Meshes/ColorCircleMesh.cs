using UnityEngine;

/**
 * Use this class to render a circle
 * **/
[ExecuteInEditMode]
public class ColorCircleMesh : MonoBehaviour
{
    public void Init(Material material = null)
    {
        Mesh mesh = new Mesh();
        mesh.name = "ColorCircleMesh";

        GetComponent<MeshFilter>().sharedMesh = mesh;

        if (material != null)
            GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /** Render this circle as a polygon with a certain number of segments
     * The more segments the polygon has the more it will look like a circle but the drawback is that more vertices have to be rendered
     * **/
    public void Render(float innerRadius, float thickness, Color color, int numSegments = 64)
    {
        Mesh circleMesh = GetComponent<MeshFilter>().sharedMesh;
        if (circleMesh == null)
            Init();

        float outerRadius = innerRadius + thickness;
        
        //sample the circle with numSegments times
        Vector3[] outerVertices = new Vector3[numSegments];
        Vector3[] innerVertices = new Vector3[numSegments];
        for (int i = 0; i != numSegments; i++)
        {
            float vertexAngle = i * 2 * Mathf.PI / (float)numSegments;
            outerVertices[i] = outerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);
            innerVertices[i] = innerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);
        }

        //Build the mesh triangles
        int numVertices = 2 * numSegments; //outer vertices are numbered from 0 to numSegments - 1 and inner vertices from numSegments to 2 * numSegments - 1
        Vector3[] meshVertices = new Vector3[numVertices];
        int[] meshIndices = new int[6 * numSegments];
        for (int i = 0; i != numSegments; i++)
        {
            meshVertices[i] = outerVertices[i];
            meshVertices[numSegments + i] = innerVertices[i];

            meshIndices[6 * i] = i;
            meshIndices[6 * i + 1] = (i == numSegments - 1) ? numSegments : numSegments + i + 1;
            meshIndices[6 * i + 2] = (i == numSegments - 1) ? 0 : i + 1;
            meshIndices[6 * i + 3] = i;
            meshIndices[6 * i + 4] = numSegments + i;
            meshIndices[6 * i + 5] = (i == numSegments - 1) ? numSegments : numSegments + i + 1;
        }

        //Build the actual mesh
        circleMesh.vertices = meshVertices;
        circleMesh.triangles = meshIndices;

        Vector3[] normals = new Vector3[numVertices];
        for (int i = 0; i != numVertices; i++)
        {
            normals[i] = Vector3.forward;
        }
        circleMesh.normals = normals;

        //set the circle color
        SetColor(color);

        //Set the mesh to the MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = circleMesh;
    }

    public void SetColor(Color color)
    {
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;

        Color[] colors = new Color[mesh.vertexCount];
        for (int i = 0; i != mesh.vertexCount; i++)
        {
            colors[i] = color;
        }

        mesh.colors = colors;
    }
}
