using UnityEngine;
using System.Collections.Generic;

public class BackgroundTrianglesRenderer : MonoBehaviour
{
    public List<BackgroundTriangleColumn> m_triangleColumns { get; set; }
    public Material m_triangleMaterial;

    public void Awake()
    {
        m_triangleColumns = new List<BackgroundTriangleColumn>();
    }

    public void Init()
    {
        BuildMesh();
    }

    /**
     * Build triangles to fill in the whole screen
     * **/
    public void BuildMesh()
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        //Build the actual mesh
        Mesh mesh = new Mesh();
        mesh.name = "BackgroundTrianglesMesh";

        //Fill data using triangles columns
        int numberOfColumns = 4;
        float triangleHeight = screenSize.x / (float) numberOfColumns;
        float triangleEdgeLength = 2 / Mathf.Sqrt(3) * triangleHeight;

        int numberOfTrianglesInColumn = 2 * (int)Mathf.Floor(screenSize.y / triangleEdgeLength) + 1;
        //int numberOfTrianglesInColumn = 2 * ((int)Mathf.Floor(screenSize.y / triangleEdgeLength) + 2);
        int numberOfTriangles = numberOfColumns * numberOfTrianglesInColumn;
        float colorPercentageStep = 100 / (float)(numberOfTriangles - 1);
        float colorPercentage = 0.0f;

        //Set up correct sizes for mesh arrays
        Vector3[] vertices = new Vector3[3 * numberOfTriangles];
        int[] triangles = new int[3 * numberOfTriangles];
        Color[] colors = new Color[3 * numberOfTriangles];

        for (int i = 0; i != numberOfColumns; i++)
        {
            BackgroundTriangleColumn column = new BackgroundTriangleColumn(i);

            for (int j = 0; j != numberOfTrianglesInColumn; j++)
            {
                Color triangleColor = ColorUtils.GetRainbowColorAtPercentage(colorPercentage);
                colorPercentage += colorPercentageStep;
                if (colorPercentage > 100)
                    colorPercentage = 100;

                float trianglePositionY = 0.5f * j * triangleEdgeLength;
                trianglePositionY -= 0.5f * screenSize.y;

                float triangleAngle;
                float trianglePositionX;
                if (i % 2 == 0)
                {
                    triangleAngle = (j % 2 == 0) ? 0 : 180;
                    trianglePositionX = (j % 2 == 0) ? (1 / 3.0f + i) * triangleHeight : (2 / 3.0f + i) * triangleHeight;
                }
                else
                {
                    triangleAngle = (j % 2 == 0) ? 180 : 0;
                    trianglePositionX = (j % 2 == 0) ? (2 / 3.0f + i) * triangleHeight : (1 / 3.0f + i) * triangleHeight;
                }
                trianglePositionX -= 0.5f * screenSize.x;

                BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), triangleEdgeLength, triangleAngle, triangleColor);
                triangle.m_indexInColumn = j;
                column.Add(triangle);

                //Fill in the mesh
                int triangleIndex = 3 * (i * numberOfTrianglesInColumn + j);

                triangle.InsertInMeshAtIndex(ref vertices, ref triangles, ref colors, 3 * (i * numberOfTrianglesInColumn + j));
            }

            m_triangleColumns.Add(column);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        GetComponent<MeshFilter>().sharedMesh = mesh;

        //Instantiate a new material
        Material clonedMaterial = (Material)Instantiate(m_triangleMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = clonedMaterial;
    }
}