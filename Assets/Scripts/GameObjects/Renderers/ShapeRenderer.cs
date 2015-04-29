using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Class that renders a mesh formed with grid triangles
 * **/
public class ShapeRenderer : MonoBehaviour
{
    public Shape m_shape { get; set; } //the shape data we want to render
    //public List<GridTriangle> m_gridTriangles { get; set; } //the list of triangles that will serve as mesh triangles to render this shape

    public enum RenderFaces
    {
        FRONT,
        BACK,
        DOUBLE_SIDED
    }; 

    /**
     * Renders the shape based on the m_gridTriangles list
     * We can specify a mesh object if we want to render again on this one or pass null to create a new mesh
     * **/
    public void Render(bool bOverwriteMesh, 
                       RenderFaces renderFaces,
                       bool bUpdateVertices = true,
                       bool bUpdateColors = true,
                       bool renderDebugTriangles = false)
    {

        //Build the mesh
        Mesh mesh;
        if (bOverwriteMesh)
        {
            mesh = new Mesh();
            mesh.name = "ShapeMesh";
        }
        else
            mesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;

        int vertexCount = 0;
        int indexCount = 0;
        Vector3[] vertices = null;
        int[] indices = null;
        Vector3[] normals = null;
        Color[] colors = null;
        Grid grid = null;
        if (bUpdateVertices)
        {
            GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
            grid = gameScene.m_grid;
            vertexCount = 3 * m_shape.m_gridTriangles.Count;
            if (renderFaces == RenderFaces.DOUBLE_SIDED) //we draw front and back faces
                indexCount = 2 * vertexCount;
            else
                indexCount = vertexCount;
            vertices = new Vector3[vertexCount];
            indices = new int[indexCount];
            normals = new Vector3[vertexCount];            
        }

        if (bUpdateColors)
        {
            if (!bUpdateVertices)
                vertexCount = 3 * m_shape.m_gridTriangles.Count;
            colors = new Color[vertexCount];
        }
        
        int iTriangleIndex = 0;
        while (iTriangleIndex != m_shape.m_gridTriangles.Count)
        {
            for (int i = 0; i != 3; i++) //loop over the 3 vertices of this triangle
            {
                ShapeTriangle shapeTriangle = (ShapeTriangle) m_shape.m_gridTriangles[iTriangleIndex];
                if (bUpdateVertices)
                {
                    vertices[iTriangleIndex * 3 + i] = grid.GetWorldCoordinatesFromGridCoordinates(shapeTriangle.m_points[i]);
                    normals[iTriangleIndex * 3 + i] = Vector3.forward;
                }
                if (bUpdateColors)
                {
                    colors[iTriangleIndex * 3 + i] = shapeTriangle.m_color;
                }
            }
            iTriangleIndex++;
        }

        //populate the array of indices separately
        if (bUpdateVertices)
        {
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
        }

        if (bUpdateVertices)
        {
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.normals = normals;
        }
        if (bUpdateColors)
        {
            mesh.colors = colors;
        }

        if (bOverwriteMesh)
            GetComponent<MeshFilter>().sharedMesh = mesh;
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
}

