﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Class that renders a mesh formed with grid triangles
 * **/
public class ShapeRenderer : MonoBehaviour
{
    public Color m_color { get; set; } //the color of this shape
    private Color m_prevColor; //used to detect changes in shape color
    public List<GridTriangle> m_triangles { get; set; } //the list of triangles that will serve as mesh triangles to render this shape

    public enum RenderFaces
    {
        FRONT,
        BACK,
        DOUBLE_SIDED
    };

    /**
     * Renders the shape based on the m_triangles list and the m_color public variables
     * We can specify a mesh object if we want to render again on this one or pass null to create a new mesh
     * **/
    public void Render(Mesh overwriteMesh, RenderFaces renderFaces)
    {
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

        int vertexCount = 3 * m_triangles.Count;
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
        while (iTriangleIndex != m_triangles.Count)
        {
            for (int i = 0; i != 3; i++) //loop over the 3 vertices of this triangle
            {
                GridTriangle gridTriangle = m_triangles[iTriangleIndex];
                vertices[iTriangleIndex * 3 + i] = gridBuilder.GetAnchorWorldCoordinatesFromGridCoordinates(gridTriangle.m_points[i]);
                normals[iTriangleIndex * 3 + i] = Vector3.forward;
                colors[iTriangleIndex * 3 + i] = m_color;
                
            }
            iTriangleIndex++;
        }

        //populate the array of indices separately
        iTriangleIndex = 0;
        while (iTriangleIndex != m_triangles.Count)
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

        m_prevColor = m_color;
    }

    /**
     * Shift all m_triangles value by a grid coordinates vector (deltaLine, deltaColumn) (i.e translate the shape)
     * **/
    public void ShiftShapeVertices(Vector2 shift)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_triangles[iTriangleIndex];
            Vector2[] triangePoints = triangle.m_points;
            for (int iPointIndex = 0; iPointIndex != 3; iPointIndex++)
            {
                triangePoints[iPointIndex] += shift;
            }
        }
    }

    public void Update()
    {
        if (!m_color.Equals(m_prevColor))
        {
            m_prevColor = m_color;

            Color[] prevColors = GetComponent<MeshFilter>().sharedMesh.colors;
            Color[] newColors = new Color[prevColors.Length];
            for (int colorIndex = 0; colorIndex != newColors.Length; colorIndex++)
            {
                newColors[colorIndex] = m_color;
            }

            GetComponent<MeshFilter>().sharedMesh.colors = newColors;
        }
    }
}

