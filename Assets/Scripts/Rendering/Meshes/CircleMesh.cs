using UnityEngine;

/**
 * Use this class to render a circle
 * **/
public class CircleMesh : MonoBehaviour
{
    private float m_innerRadius;
    private float m_outerRadius;
    private int m_numSegments;

    public void Init(Material material = null)
    {
        Mesh mesh = new Mesh();
        mesh.name = "CircleMesh";

        GetComponent<MeshFilter>().sharedMesh = mesh;

        if (material != null)
            GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /** Render this circle as a polygon with a certain number of segments
     * The more segments the polygon has the more it will look like a circle but the drawback is that more vertices have to be rendered
     * **/
    public void Render(float innerRadius, float outerRadius, Color color, int numSegments = 64, float angle = 0)
    {
        if (numSegments < 3)
            throw new System.Exception("circle must have at least 3 segments");

        m_innerRadius = innerRadius;
        m_outerRadius = outerRadius;
        m_numSegments = numSegments;

        Mesh circleMesh = GetComponent<MeshFilter>().sharedMesh;
        if (circleMesh == null)
        {
            Init();
            circleMesh = GetComponent<MeshFilter>().sharedMesh;
        }

        Vector3[] meshVertices;
        int[] meshIndices;

        if (innerRadius == 0) //no hole
        {
            int numVertices = (numSegments == 3) ? 3 : numSegments + 1;
            int numTriangles = (numSegments == 3) ? 1 : numSegments;

            meshVertices = new Vector3[numVertices];

            int iOuterVertexStartIdx = (numSegments == 3) ? 0 : 1;
            if (numSegments > 3)
                meshVertices[0] = Vector3.zero;

            for (int i = iOuterVertexStartIdx; i != numVertices; i++)
            {
                float vertexAngle = Mathf.PI / 2.0f + 2 * (i - 1) * Mathf.PI / (float)numSegments;
                meshVertices[i] = outerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);

                if (angle != 0)
                    meshVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * meshVertices[i];
            }

            //Build the mesh triangles
            meshIndices = new int[numTriangles * 3];
            for (int i = 0; i != numTriangles; i++)
            {
                meshIndices[3 * i] = 0;
                meshIndices[3 * i + 1] = (i == numSegments - 1) ? 1 : i + 2;
                meshIndices[3 * i + 2] = i + 1;
            }
        }
        else
        {
            //sample the circle with numSegments times
            Vector3[] outerVertices = new Vector3[numSegments];
            Vector3[] innerVertices = new Vector3[numSegments];
            for (int i = 0; i != numSegments; i++)
            {
                float vertexAngle = Mathf.PI / 2.0f + 2 * (i - 1) * Mathf.PI / (float)numSegments;
                outerVertices[i] = outerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);
                innerVertices[i] = innerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);

                if (angle != 0)
                {
                    outerVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * outerVertices[i];
                    innerVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * innerVertices[i];
                }
            }

            //Build the mesh triangles
            int numVertices = 2 * numSegments; //outer vertices are numbered from 0 to numSegments - 1 and inner vertices from numSegments to 2 * numSegments - 1
            meshVertices = new Vector3[numVertices];
            meshIndices = new int[6 * numSegments];
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
        }

        //Build the actual mesh
        circleMesh.Clear();
        circleMesh.vertices = meshVertices;
        circleMesh.triangles = meshIndices;

        //set the circle color
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;

        int numVertices;
        if (m_innerRadius == 0)
        {
            if (m_numSegments == 3)
                numVertices = 3;
            else
                numVertices = m_numSegments + 1;
        }
        else
            numVertices = m_numSegments * 2;

        Color[] colors = new Color[numVertices];
        if (mesh == null)
            Debug.Log("mesh NULL:" + this.name);
        for (int i = 0; i != mesh.vertexCount; i++)
        {
            colors[i] = color;
        }

        mesh.colors = colors;
    }
}
