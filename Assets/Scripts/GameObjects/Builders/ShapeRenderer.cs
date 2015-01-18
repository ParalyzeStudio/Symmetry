using UnityEngine;
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

    public void Render(bool bDoubleSided)
    {
        //Obtain the GridBuilder to make transformations from grid coordinates to world coordinates
        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        GridBuilder gridBuilder = gridObject.GetComponent<GridBuilder>();

        //Build the mesh
        Mesh mesh = new Mesh();
        mesh.name = "ShapeMesh";

        int vertexCount = 3 * m_triangles.Count;
        if (bDoubleSided) //we draw front and back faces
            vertexCount *= 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] indices = new int[vertexCount];
        Color[] colors = new Color[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        int iTriangleIndex = 0;
        while (iTriangleIndex != m_triangles.Count)
        {
            for (int i = 0; i != 3; i++) //loop over the 3 vertices of this triangle
            {
                GridTriangle gridTriangle = m_triangles[iTriangleIndex];
                vertices[iTriangleIndex * 3 + i] = gridBuilder.GetAnchorWorldCoordinatesFromGridCoordinates(gridTriangle.m_points[i]);
                indices[iTriangleIndex * 3 + i] = iTriangleIndex * 3 + i;
                normals[iTriangleIndex * 3 + i] = Vector3.forward;
                colors[iTriangleIndex * 3 + i] = m_color;
                
            }
            iTriangleIndex++;
        }
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.normals = normals;
        mesh.colors = colors;

        GetComponent<MeshFilter>().sharedMesh = mesh;

        m_prevColor = m_color;
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

