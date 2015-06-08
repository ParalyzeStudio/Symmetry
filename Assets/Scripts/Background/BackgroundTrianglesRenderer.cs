﻿using UnityEngine;
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

    public void Awake()
    {
        m_triangleColumns = new List<BackgroundTriangleColumn>();
    }

    public void Init()
    {
        BuildMesh();
    }
    
    /**
     * Returns the default background color
     * **/
    public Color GetDefaultBackgroundColor()
    {
        return Color.white; //just return the color we want as background color
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
     * Updates the colors array for the specific column
     * Set bUpdateMeshData to true if the actual mesh data has to be invlaidated at this point
     * **/
    public void UpdateMeshColorsArrayForColumn(BackgroundTriangleColumn column)
    {
        for (int iTriangleIdx = 0; iTriangleIdx != column.Count; iTriangleIdx++)
        {
            BackgroundTriangle triangle = column[iTriangleIdx];

            int triangleGlobalIndex = column.m_index * column.Count + iTriangleIdx;

            Color triangleColor = triangle.GetColor();
            //colors
            m_meshColors[3 * triangleGlobalIndex] = triangleColor;
            m_meshColors[3 * triangleGlobalIndex + 1] = triangleColor;
            m_meshColors[3 * triangleGlobalIndex + 2] = triangleColor;
        }

        m_meshColorsDirty = true;
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

    public void RenderForMainMenu()
    {
        int[] stopIndices = new int[NUM_COLUMNS];
        stopIndices[0] = 8;
        stopIndices[1] = 9;
        stopIndices[2] = 10;
        stopIndices[3] = 9;
        stopIndices[4] = 10;
        stopIndices[5] = 11;
        stopIndices[6] = 12;
        stopIndices[7] = 13;
        stopIndices[8] = 14;
        stopIndices[9] = 14;
        stopIndices[10] = 15;
        stopIndices[11] = 16;
        stopIndices[12] = 16;
        stopIndices[13] = 17;
        stopIndices[14] = 16;
        stopIndices[15] = 15;
        stopIndices[16] = 15;
        stopIndices[17] = 14;
        stopIndices[18] = 13;
        stopIndices[19] = 14;
        stopIndices[20] = 12;
        stopIndices[21] = 10;
        stopIndices[22] = 9;
        stopIndices[23] = 8;
        stopIndices[24] = 8;

        for (int iColumnIdx = 0; iColumnIdx != NUM_COLUMNS; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];
            column.ApplyGradient(Color.cyan, Color.white); //tmp apply same gradient for all columns
            column.AddAnimatedTrianglesBlock(0, stopIndices[iColumnIdx]);
            UpdateMeshColorsArrayForColumn(m_triangleColumns[iColumnIdx]);
        }
    }

    public void Update()
    {
        for (int iColumnIdx = 0; iColumnIdx != m_triangleColumns.Count; iColumnIdx++)
        {
            BackgroundTriangleColumn column = m_triangleColumns[iColumnIdx];
            if (!column.IsAnimated())
                continue;

            float dt = Time.deltaTime;
            column.m_elapsedTime += dt;

            if (column.m_elapsedTime > BackgroundTriangleColumn.COLOR_INVALIDATION_STEP)
            {
                column.m_elapsedTime = 0;
                column.AnimateTriangles();
            }
        }

        UpdateMeshData(); //update mesh data every frame, 'dirty' variables take care of knowing if mesh has to be actually updated
    }
}