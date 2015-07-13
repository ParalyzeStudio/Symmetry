using UnityEngine;

public class HexagonMesh : MonoBehaviour
{
    public float m_innerRadius { get; set; }
    public float m_outerRadius { get; set; }

    public void Init(Material material = null)
    {
        Mesh mesh = new Mesh();
        mesh.name = "HexagonMesh";

        GetComponent<MeshFilter>().sharedMesh = mesh;

        if (material != null)
            GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /** Render this hexagon
     * inner radius: the radius of the inner circle containing the hexagon hole, set 0 for no hole
     * outer radius: the radius of the outer circle containing the hexagon contour
     * thickness: difference between outer and inner radii
     * **/
    //public void Render(float innerRadius, float thickness, Color color, float angle = 0)
    //{
    //    m_innerRadius = innerRadius;
    //    m_outerRadius = innerRadius + thickness;

    //    Mesh hexagonMesh = GetComponent<MeshFilter>().sharedMesh;
    //    if (hexagonMesh == null)
    //    {
    //        Init();
    //        hexagonMesh = GetComponent<MeshFilter>().sharedMesh;
    //    }

    //    Vector3[] meshVertices;
    //    int[] meshIndices;

    //    if (innerRadius == 0) //no hole
    //    {
    //        meshVertices = new Vector3[7];

    //        meshVertices[0] = Vector3.zero; //center of the hexagon
    //        for (int i = 1; i != 7; i++)
    //        {
    //            float vertexAngle = Mathf.PI / 2.0f + (i - 1) * Mathf.PI / 3.0f;
    //            meshVertices[i] = m_outerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);

    //            if (angle != 0)
    //                meshVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * meshVertices[i];
    //        }

    //        //Build the mesh triangles
    //        meshIndices = new int[18];
    //        for (int i = 0; i != 6; i++)
    //        {
    //            meshIndices[3 * i] = 0;
    //            meshIndices[3 * i + 1] = (i == 5) ? 1 : i + 2;
    //            meshIndices[3 * i + 2] = i + 1;
    //        }
    //    }
    //    else
    //    {
    //        //sample the circle 6 times
    //        Vector3[] outerVertices = new Vector3[6];
    //        Vector3[] innerVertices = new Vector3[6];
    //        for (int i = 0; i != 6; i++)
    //        {
    //            float vertexAngle = Mathf.PI / 2.0f + i * Mathf.PI / 3.0f;
    //            outerVertices[i] = m_outerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);
    //            innerVertices[i] = innerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);

    //            if (angle != 0)
    //            {
    //                outerVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * outerVertices[i];
    //                innerVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * innerVertices[i];
    //            }
    //        }

    //        //Build the mesh triangles
    //        meshVertices = new Vector3[12];
    //        meshIndices = new int[36];
    //        for (int i = 0; i != 6; i++)
    //        {
    //            meshVertices[i] = outerVertices[i];
    //            meshVertices[6 + i] = innerVertices[i];

    //            meshIndices[6 * i] = i;
    //            meshIndices[6 * i + 1] = (i == 5) ? 6 : 6 + i + 1;
    //            meshIndices[6 * i + 2] = (i == 5) ? 0 : i + 1;
    //            meshIndices[6 * i + 3] = i;
    //            meshIndices[6 * i + 4] = 6 + i;
    //            meshIndices[6 * i + 5] = (i == 5) ? 6 : 6 + i + 1;
    //        }
    //    }        

    //    //Build the actual mesh
    //    hexagonMesh.vertices = meshVertices;
    //    hexagonMesh.triangles = meshIndices;

    //    //set the hexagon color
    //    SetColor(color);
    //}

    public void Render(float innerRadius, float outerRadius, Color color, float angle = 0)
    {
        m_innerRadius = innerRadius;
        m_outerRadius = outerRadius;

        Mesh hexagonMesh = GetComponent<MeshFilter>().sharedMesh;
        if (hexagonMesh == null)
        {
            Init();
            hexagonMesh = GetComponent<MeshFilter>().sharedMesh;
        }

        Vector3[] meshVertices;
        int[] meshIndices;

        if (innerRadius == 0) //no hole
        {
            meshVertices = new Vector3[7];

            meshVertices[0] = Vector3.zero; //center of the hexagon
            for (int i = 1; i != 7; i++)
            {
                float vertexAngle = Mathf.PI / 2.0f + (i - 1) * Mathf.PI / 3.0f;
                meshVertices[i] = m_outerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);

                if (angle != 0)
                    meshVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * meshVertices[i];
            }

            //Build the mesh triangles
            meshIndices = new int[18];
            for (int i = 0; i != 6; i++)
            {
                meshIndices[3 * i] = 0;
                meshIndices[3 * i + 1] = (i == 5) ? 1 : i + 2;
                meshIndices[3 * i + 2] = i + 1;
            }
        }
        else
        {
            //sample the circle 6 times
            Vector3[] outerVertices = new Vector3[6];
            Vector3[] innerVertices = new Vector3[6];
            for (int i = 0; i != 6; i++)
            {
                float vertexAngle = Mathf.PI / 2.0f + i * Mathf.PI / 3.0f;
                outerVertices[i] = m_outerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);
                innerVertices[i] = innerRadius * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);

                if (angle != 0)
                {
                    outerVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * outerVertices[i];
                    innerVertices[i] = Quaternion.AngleAxis(angle, Vector3.forward) * innerVertices[i];
                }
            }

            //Build the mesh triangles
            meshVertices = new Vector3[12];
            meshIndices = new int[36];
            for (int i = 0; i != 6; i++)
            {
                meshVertices[i] = outerVertices[i];
                meshVertices[6 + i] = innerVertices[i];

                meshIndices[6 * i] = i;
                meshIndices[6 * i + 1] = (i == 5) ? 6 : 6 + i + 1;
                meshIndices[6 * i + 2] = (i == 5) ? 0 : i + 1;
                meshIndices[6 * i + 3] = i;
                meshIndices[6 * i + 4] = 6 + i;
                meshIndices[6 * i + 5] = (i == 5) ? 6 : 6 + i + 1;
            }
        }

        //Build the actual mesh
        hexagonMesh.Clear();
        hexagonMesh.vertices = meshVertices;
        hexagonMesh.triangles = meshIndices;

        //set the hexagon color
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        Mesh mesh = this.GetComponent<MeshFilter>().sharedMesh;

        Color[] colors = new Color[(m_innerRadius == 0) ? 7 : 12];
        for (int i = 0; i != mesh.vertexCount; i++)
        {
            colors[i] = color;
        }

        mesh.colors = colors;
    }
}

