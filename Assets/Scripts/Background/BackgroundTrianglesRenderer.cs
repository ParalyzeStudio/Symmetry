using UnityEngine;
using System.Collections.Generic;

public class BackgroundTrianglesRenderer : MonoBehaviour
{
    public List<BackgroundTriangleColumn> m_triangleColumns { get; set; }
    public Material m_triangleMaterial;
    public Mesh m_mesh { get; set; }

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
        m_mesh = new Mesh();
        m_mesh.name = "BackgroundTrianglesMesh";

        //Fill data using triangles columns
        int numberOfColumns = 25;
        float triangleHeight = screenSize.x / (float) numberOfColumns;
        float triangleEdgeLength = 2 / Mathf.Sqrt(3) * triangleHeight;

        float fNumberOfColumnTriangles = (screenSize.y - 0.5f * triangleEdgeLength) / triangleEdgeLength;
        int numberOfColumnTriangles = (int)Mathf.Floor(fNumberOfColumnTriangles);
        if (fNumberOfColumnTriangles != numberOfColumnTriangles) //no floating part
            numberOfColumnTriangles += 1; //just round to the next integer
        numberOfColumnTriangles += 1; //add the first (half) triangle
        numberOfColumnTriangles *= 2; //multiply by 2 because there are 2 series of opposite trying forming one column
        int numberOfTriangles = numberOfColumns * numberOfColumnTriangles;
        float colorPercentageStep = 100 / (float)(numberOfTriangles - 1);
        float colorPercentage = 0.0f;

        //Set up correct sizes for mesh arrays
        Vector3[] vertices = new Vector3[3 * numberOfTriangles];
        int[] triangles = new int[3 * numberOfTriangles];
        Color[] colors = new Color[3 * numberOfTriangles];

        for (int i = 0; i != numberOfColumns; i++)
        {
            BackgroundTriangleColumn column = new BackgroundTriangleColumn(this, i);

            for (int j = 0; j != numberOfColumnTriangles; j++)
            {
                Color triangleColor = ColorUtils.GetRainbowColorAtPercentage(colorPercentage);
                colorPercentage += colorPercentageStep;
                if (colorPercentage > 100)
                    colorPercentage = 100;
                triangleColor = Color.white;

                float trianglePositionY = screenSize.y - 0.5f * j * triangleEdgeLength;
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
                triangle.m_parentColumn = column;
                triangle.m_parentMesh = m_mesh;
                column.AddTriangle(triangle);

                //Fill in the mesh
                int triangleGlobalIndex = i * numberOfColumnTriangles + j;

                //vertices
                vertices[3 * triangleGlobalIndex] = triangle.m_points[0];
                vertices[3 * triangleGlobalIndex + 1] = triangle.m_points[1];
                vertices[3 * triangleGlobalIndex + 2] = triangle.m_points[2];

                //indices
                triangles[3 * triangleGlobalIndex] = 3 * triangleGlobalIndex;
                triangles[3 * triangleGlobalIndex + 1] = 3 * triangleGlobalIndex + 1;
                triangles[3 * triangleGlobalIndex + 2] = 3 * triangleGlobalIndex + 2;

                //colors
                colors[3 * triangleGlobalIndex] = triangle.m_color;
                colors[3 * triangleGlobalIndex + 1] = triangle.m_color;
                colors[3 * triangleGlobalIndex + 2] = triangle.m_color;
            }

            m_triangleColumns.Add(column);
        }

        m_mesh.vertices = vertices;
        m_mesh.triangles = triangles;
        m_mesh.colors = colors;

        GetComponent<MeshFilter>().sharedMesh = m_mesh;

        //Instantiate a new material
        Material clonedMaterial = (Material)Instantiate(m_triangleMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = clonedMaterial;
    }

    public void UpdateTrianglesColumn(BackgroundTriangleColumn column)
    {
        for (int iTriangleIdx = 0; iTriangleIdx != column.Count; iTriangleIdx++)
        {
            column[iTriangleIdx].UpdateMeshData();
        }
    }

    public void RenderForMainMenu()
    {
        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            m_triangleColumns[iColumnIdx].AddLeader(0, new Color(0.82f, 0.93f, 0.99f, 1));
            UpdateTrianglesColumn(m_triangleColumns[iColumnIdx]);
        }
    }

    public void Update()
    {
        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];
            if (!column.IsShifting())
                continue;

            float dt = Time.deltaTime;
            column.m_elapsedTime += dt;

            if (column.m_elapsedTime > BackgroundTriangleColumn.COLOR_INVALIDATION_STEP)
            {
                column.m_elapsedTime = 0;
                column.ShiftTriangles();
            }
        }        
    }
}