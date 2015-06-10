using UnityEngine;
using System.Collections.Generic;

public class BackgroundTrianglesRenderer : MonoBehaviour
{
    public const int NUM_COLUMNS = 25;

    public List<BackgroundTriangleColumn> m_triangleColumns { get; set; }
    public Material m_triangleMaterial;
    public Mesh m_mesh { get; set; }

    //mesh arrays
    public Vector3[] m_meshVertices { get; set; }
    public int[] m_meshTriangles { get; set; }
    public Color[] m_meshColors { get; set; }
    private bool m_meshVerticesDirty;
    private bool m_meshTrianglesDirty;
    private bool m_meshColorsDirty;

    //Variables to handle the delayed rendering of main title
    private bool m_renderingMainMenuTitle;
    private float m_renderingMainMenuTitleElapsedTime;
    private float m_renderingMainMenuTitleDelay;

    public void Awake()
    {
        m_triangleColumns = new List<BackgroundTriangleColumn>();
    }

    public void Init()
    {
        BuildMesh();

        m_renderingMainMenuTitle = false;
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
        float triangleHeight = screenSize.x / (float) NUM_COLUMNS;
        float triangleEdgeLength = 2 / Mathf.Sqrt(3) * triangleHeight;

        float fNumberOfColumnTriangles = (screenSize.y - 0.5f * triangleEdgeLength) / triangleEdgeLength;
        int numberOfColumnTriangles = (int)Mathf.Floor(fNumberOfColumnTriangles);
        if (fNumberOfColumnTriangles != numberOfColumnTriangles) //no floating part
            numberOfColumnTriangles += 1; //just round to the next integer
        numberOfColumnTriangles += 1; //add the first (half) triangle
        numberOfColumnTriangles *= 2; //multiply by 2 because there are 2 series of opposite trying forming one column
        int numberOfTriangles = NUM_COLUMNS * numberOfColumnTriangles;
        //float colorPercentageStep = 100 / (float)(numberOfTriangles - 1);
        //float colorPercentage = 0.0f;

        //Set up correct sizes for mesh arrays
        m_meshVertices = new Vector3[3 * numberOfTriangles];
        m_meshTriangles = new int[3 * numberOfTriangles];
        m_meshColors = new Color[3 * numberOfTriangles];
        m_meshVerticesDirty = true;
        m_meshTrianglesDirty = true;
        m_meshColorsDirty = true;

        for (int i = 0; i != NUM_COLUMNS; i++)
        {
            BackgroundTriangleColumn column = new BackgroundTriangleColumn(this, i);

            for (int j = 0; j != numberOfColumnTriangles; j++)
            {
                //Color triangleColor = ColorUtils.GetRainbowColorAtPercentage(colorPercentage);
                //colorPercentage += colorPercentageStep;
                //if (colorPercentage > 100)
                //    colorPercentage = 100;
                //Color triangleColor = Color.white;

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

                Color triangleColor = GetDefaultBackgroundColor();
                BackgroundTriangle triangle = new BackgroundTriangle(new Vector2(trianglePositionX, trianglePositionY), triangleEdgeLength, triangleAngle, triangleColor);
                triangle.m_indexInColumn = j;
                triangle.m_parentColumn = column;
                triangle.m_hidden = true; //hide the triangle when building the mesh
                column.AddTriangle(triangle);

                //Fill in the mesh
                int triangleGlobalIndex = i * numberOfColumnTriangles + j;

                //vertices
                m_meshVertices[3 * triangleGlobalIndex] = triangle.m_points[0];
                m_meshVertices[3 * triangleGlobalIndex + 1] = triangle.m_points[1];
                m_meshVertices[3 * triangleGlobalIndex + 2] = triangle.m_points[2];

                //indices
                m_meshTriangles[3 * triangleGlobalIndex] = 3 * triangleGlobalIndex;
                m_meshTriangles[3 * triangleGlobalIndex + 1] = 3 * triangleGlobalIndex + 1;
                m_meshTriangles[3 * triangleGlobalIndex + 2] = 3 * triangleGlobalIndex + 2;

                //colors
                m_meshColors[3 * triangleGlobalIndex] = triangleColor;
                m_meshColors[3 * triangleGlobalIndex + 1] = triangleColor;
                m_meshColors[3 * triangleGlobalIndex + 2] = triangleColor;
            }

            m_triangleColumns.Add(column);
        }

        GetComponent<MeshFilter>().sharedMesh = m_mesh;

        //Instantiate a new material
        Material clonedMaterial = (Material)Instantiate(m_triangleMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = clonedMaterial;
    }

    /**
     * Returns the default background color
     * **/
    public Color GetDefaultBackgroundColor()
    {
        return Color.white; //just return the color we want as background color
    }

    /**
     * Updates the colors array for the specific column
     * Set bUpdateMeshData to true if the actual mesh data has to be invalidated at this point
     * **/
    public void UpdateMeshColorsArrayForColumn(BackgroundTriangleColumn column)
    {
        for (int iTriangleIdx = 0; iTriangleIdx != column.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = column[iTriangleIdx];

            int triangleGlobalIndex = column.m_index * column.Count + iTriangleIdx;

            Color triangleColor = triangle.GetRenderColor();

            //colors
            m_meshColors[3 * triangleGlobalIndex] = triangleColor;
            m_meshColors[3 * triangleGlobalIndex + 1] = triangleColor;
            m_meshColors[3 * triangleGlobalIndex + 2] = triangleColor;
        }

        m_meshColorsDirty = true;
    }

    /**
    * Same thing that UpdateMeshColorsArrayForColumn() but for all columns instead
    * **/
    public void UpdateMeshColorsArrayForAllColumns()
    {
        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            UpdateMeshColorsArrayForColumn(m_triangleColumns[iColumnIdx]);
        }
    }

    public void UpdateMeshData()
    {
        if (m_meshVerticesDirty)
        {
            m_mesh.vertices = m_meshVertices;
            m_meshVerticesDirty = false;
        }
        if (m_meshTrianglesDirty)
        {
            m_mesh.triangles = m_meshTriangles;
            m_meshTrianglesDirty = false;
        }
        if (m_meshColorsDirty)
        {
            m_mesh.colors = m_meshColors;
            m_meshColorsDirty = false;
        }
    }

    public void RenderForMainMenu(float fDelay)
    {
        for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];
            
            column.ApplyGradient(ColorUtils.GetColorFromRGBAVector4(new Vector4(252, 183, 94, 255)), Color.white); //tmp apply same gradient for all columns
            //column.AddAnimatedTrianglesBlock(0, column.Count - 1);
            column.SwitchTrianglesVisibilityStatusBetweenIndices(0, column.Count - 1, fDelay);
        }

        //Modify the colors of some triangles to make the title appear
        RenderMainMenuTitle(fDelay + 5.0f);

        UpdateMeshColorsArrayForAllColumns();
    }

    /**
     * Launches the main menu title rendering processing that will draw it after a certain delay
     * **/
    public void RenderMainMenuTitle(float fDelay)
    {
        m_renderingMainMenuTitle = true;
        m_renderingMainMenuTitleElapsedTime = 0;
        m_renderingMainMenuTitleDelay = fDelay;
    }

    /**
     * Modify the color of all triangles forming the title FLEEC
     * **/
    public void DrawMainMenuTitle()
    {
        //List all triangles inside the title
        List<BackgroundTriangle> titleTriangles = new List<BackgroundTriangle>();
        titleTriangles.Capacity = 52; //52 triangles in the title (9+8+10+10+15)

        //Add 9 triangles for letter F
        titleTriangles.Add(m_triangleColumns[4][7]);
        titleTriangles.Add(m_triangleColumns[4][8]);
        titleTriangles.Add(m_triangleColumns[4][9]);
        titleTriangles.Add(m_triangleColumns[4][10]);
        titleTriangles.Add(m_triangleColumns[4][11]);
        titleTriangles.Add(m_triangleColumns[4][12]);
        titleTriangles.Add(m_triangleColumns[5][6]);
        titleTriangles.Add(m_triangleColumns[5][7]);
        titleTriangles.Add(m_triangleColumns[5][9]);

        //Add 8 triangles for letter L
        titleTriangles.Add(m_triangleColumns[7][6]);
        titleTriangles.Add(m_triangleColumns[7][7]);
        titleTriangles.Add(m_triangleColumns[7][8]);
        titleTriangles.Add(m_triangleColumns[7][9]);
        titleTriangles.Add(m_triangleColumns[7][10]);
        titleTriangles.Add(m_triangleColumns[7][11]);
        titleTriangles.Add(m_triangleColumns[7][12]);
        titleTriangles.Add(m_triangleColumns[8][12]);

        //Add 10 triangles for first letter E
        titleTriangles.Add(m_triangleColumns[10][7]);
        titleTriangles.Add(m_triangleColumns[10][8]);
        titleTriangles.Add(m_triangleColumns[10][9]);
        titleTriangles.Add(m_triangleColumns[10][10]);
        titleTriangles.Add(m_triangleColumns[10][11]);
        titleTriangles.Add(m_triangleColumns[11][6]);
        titleTriangles.Add(m_triangleColumns[11][7]);
        titleTriangles.Add(m_triangleColumns[11][9]);
        titleTriangles.Add(m_triangleColumns[11][11]);
        titleTriangles.Add(m_triangleColumns[11][12]);

        //Add 10 triangles for second letter E
        titleTriangles.Add(m_triangleColumns[14][6]);
        titleTriangles.Add(m_triangleColumns[14][7]);
        titleTriangles.Add(m_triangleColumns[14][9]);
        titleTriangles.Add(m_triangleColumns[14][11]);
        titleTriangles.Add(m_triangleColumns[14][12]);
        titleTriangles.Add(m_triangleColumns[15][7]);
        titleTriangles.Add(m_triangleColumns[15][8]);
        titleTriangles.Add(m_triangleColumns[15][9]);
        titleTriangles.Add(m_triangleColumns[15][10]);
        titleTriangles.Add(m_triangleColumns[15][11]);

        //Add 15 triangles for second letter C
        titleTriangles.Add(m_triangleColumns[18][7]);
        titleTriangles.Add(m_triangleColumns[18][8]);
        titleTriangles.Add(m_triangleColumns[18][9]);
        titleTriangles.Add(m_triangleColumns[18][10]);
        titleTriangles.Add(m_triangleColumns[18][11]);
        titleTriangles.Add(m_triangleColumns[19][6]);
        titleTriangles.Add(m_triangleColumns[19][7]);
        titleTriangles.Add(m_triangleColumns[19][11]);
        titleTriangles.Add(m_triangleColumns[19][12]);
        titleTriangles.Add(m_triangleColumns[20][6]);
        titleTriangles.Add(m_triangleColumns[20][7]);
        titleTriangles.Add(m_triangleColumns[20][11]);
        titleTriangles.Add(m_triangleColumns[20][12]);
        titleTriangles.Add(m_triangleColumns[21][7]);
        titleTriangles.Add(m_triangleColumns[21][11]);

        //manually set destination color and launch the color animation for every triangle in the previously build list
        Color blendColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(255, 23, 12, 255));
        for (int iTriangleIdx = 0; iTriangleIdx != titleTriangles.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = titleTriangles[iTriangleIdx];
            Color destinationColor = Color.Lerp(triangle.GetTrueColor(), blendColor, 0.5f);

            //generate a random delay for each title triangle
            float randomDelay = Random.value;
            randomDelay *= 0.1f;

            //launch the color animation
            triangle.StartColorAnimation(destinationColor, 1.5f, randomDelay, true);
        }
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];

            column.AnimateTriangles(dt);
        }

        UpdateMeshData(); //update mesh data every frame, 'dirty' variables take care of knowing if mesh has to be actually updated

        //render main menu title
        if (m_renderingMainMenuTitle)
        {
            m_renderingMainMenuTitleElapsedTime += dt;
            if (m_renderingMainMenuTitleElapsedTime >= m_renderingMainMenuTitleDelay)
            {
                m_renderingMainMenuTitle = false;
                DrawMainMenuTitle();
            }            
        }
    }
}