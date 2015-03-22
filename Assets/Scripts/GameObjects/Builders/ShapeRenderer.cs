﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Class that renders a mesh formed with grid triangles
 * **/
public class ShapeRenderer : MonoBehaviour
{
    public Color m_color; //the overriden color of this shape
    private Color m_prevColor; //used to detect changes in shape color
    public Shape m_shape { get; set; } //the shape data we want to render
    //public List<GridTriangle> m_gridTriangles { get; set; } //the list of triangles that will serve as mesh triangles to render this shape

    public enum RenderFaces
    {
        FRONT,
        BACK,
        DOUBLE_SIDED
    };

    public void Start()
    {
        m_color = m_shape.m_color;
        m_prevColor = m_color;
    }   

    /**
     * Renders the shape based on the m_gridTriangles list and the m_color public variables
     * We can specify a mesh object if we want to render again on this one or pass null to create a new mesh
     * **/
    public void Render(Mesh overwriteMesh, RenderFaces renderFaces, bool renderDebugTriangles)
    {
        m_color = m_shape.m_color;
        m_prevColor = m_color;

        //Obtain the GridBuilder to make transformations from grid coordinates to world coordinates
        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        GridBuilder gridBuilder = gridObject.GetComponent<GridBuilder>();

        //Build the mesh
        Mesh mesh;
        if (overwriteMesh == null)
        {
            mesh = new Mesh();
            mesh.name = "ShapeMesh";
        }
        else
            mesh = overwriteMesh;

        int vertexCount = 3 * m_shape.m_gridTriangles.Count;
        int indexCount;
        if (renderFaces == RenderFaces.DOUBLE_SIDED) //we draw front and back faces
            indexCount = 2 * vertexCount;
        else
            indexCount = vertexCount;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] indices = new int[indexCount];
        Color[] colors = new Color[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        int iTriangleIndex = 0;
        while (iTriangleIndex != m_shape.m_gridTriangles.Count)
        {
            for (int i = 0; i != 3; i++) //loop over the 3 vertices of this triangle
            {
                GridTriangle gridTriangle = m_shape.m_gridTriangles[iTriangleIndex];
                vertices[iTriangleIndex * 3 + i] = gridBuilder.GetWorldCoordinatesFromGridCoordinates(gridTriangle.m_points[i]);
                normals[iTriangleIndex * 3 + i] = Vector3.forward;
                colors[iTriangleIndex * 3 + i] = m_shape.m_color;
            }
            iTriangleIndex++;
        }

        //populate the array of indices separately
        iTriangleIndex = 0;
        while (iTriangleIndex != m_shape.m_gridTriangles.Count)
        {
            if (renderFaces == RenderFaces.FRONT)
            {
                indices[iTriangleIndex * 3] = iTriangleIndex * 3;
                indices[iTriangleIndex * 3 + 1] = iTriangleIndex * 3 + 1;
                indices[iTriangleIndex * 3 + 2] = iTriangleIndex * 3 + 2;
            }
            else if (renderFaces == RenderFaces.BACK)
            {
                indices[iTriangleIndex * 3] = iTriangleIndex * 3;
                indices[iTriangleIndex * 3 + 1] = iTriangleIndex * 3 + 2;
                indices[iTriangleIndex * 3 + 2] = iTriangleIndex * 3 + 1;
            }
            else //double sided
            {
                indices[iTriangleIndex * 6] = iTriangleIndex * 3;
                indices[iTriangleIndex * 6 + 1] = iTriangleIndex * 3 + 1;
                indices[iTriangleIndex * 6 + 2] = iTriangleIndex * 3 + 2;
                indices[iTriangleIndex * 6 + 3] = iTriangleIndex * 3;
                indices[iTriangleIndex * 6 + 4] = iTriangleIndex * 3 + 2;
                indices[iTriangleIndex * 6 + 5] = iTriangleIndex * 3 + 1;
            }
            iTriangleIndex++;
        }

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.normals = normals;
        mesh.colors = colors;

        if (overwriteMesh == null)
            GetComponent<MeshFilter>().sharedMesh = mesh;
        else
            mesh.RecalculateBounds();
    }

    /**
     * Shift all m_gridTriangles value by a grid coordinates vector (deltaLine, deltaColumn) (i.e translate the shape)
     * **/
    public void ShiftShapeVertices(Vector2 shift)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_shape.m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_shape.m_gridTriangles[iTriangleIndex];
            Vector2[] trianglePoints = triangle.m_points;
            for (int iPointIndex = 0; iPointIndex != 3; iPointIndex++)
            {
                trianglePoints[iPointIndex] += shift;
            }
        }

        for (int iContourPointIdx = 0; iContourPointIdx != m_shape.m_contour.Count; iContourPointIdx++)
        {
            m_shape.m_contour[iContourPointIdx] += shift;
        }
    }

    public void SetColor(Color color)
    {
        m_color = color;
        m_prevColor = color;
        m_shape.m_color = m_color;

        Color[] prevColors = GetComponent<MeshFilter>().sharedMesh.colors;
        Color[] newColors = new Color[prevColors.Length];
        for (int colorIndex = 0; colorIndex != newColors.Length; colorIndex++)
        {
            newColors[colorIndex] = m_color;
        }

        GetComponent<MeshFilter>().sharedMesh.colors = newColors;
    }

    public void Update()
    {
        if (!m_color.Equals(m_prevColor))
        {
            SetColor(m_color);
        }
    }
}

