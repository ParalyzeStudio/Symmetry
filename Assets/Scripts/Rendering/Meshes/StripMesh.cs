using UnityEngine;
using System.Collections.Generic;

public class StripMesh : ColorMesh
{
    //the strip data that will serve as rendering this mesh
    private Strip m_strip;

    //the color of the strip set by the animator (read-only)
    private Color m_color;

    /**
     * Initialize the strip mesh
     * Call this function instead of the parent Init(Material material) method
     * **/
    public void Init(Strip stripData, Material material = null)
    {
        base.Init(material);
        m_mesh.name = "StripMesh";
        m_strip = stripData;
    }

    /**
     * Calls this to hide this strip mesh, for instance when axis has zero length
     * Basically clear all mesh arrays and thats it
     * **/
    public void Hide()
    {
        ClearMesh();
        m_mesh.Clear();
    }
    
    /**
     * Render the mesh
     * This operation can only be realised if the strip contour has been calculated previously
     * **/
    public void Render()
    {
        //clear the mesh
        ClearMesh();
        m_mesh.Clear();

        Grid grid = GetGrid();

        //Add the triangles to the mesh
        for (int iTriangleIdx = 0; iTriangleIdx != m_strip.m_triangles.Count; iTriangleIdx++)
        {
            GridTriangle triangle = m_strip.m_triangles[iTriangleIdx];
            Vector2 pt1 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[0]);
            Vector2 pt2 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[1]);
            Vector2 pt3 = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[2]);
            AddTriangle(pt1, pt3, pt2);
        }

        SetColor(m_color); //reset the color

        RefreshMesh();
    }

    /**
     * Set the color of this strip
     * **/
    public void SetColor(Color color)
    {
        m_color = color;

        for (int i = 0; i != m_colors.Count; i++)
        {
            m_colors[i] = color;
        }

        m_meshColorsDirty = true;
    }

    private Grid GetGrid()
    {
        GameScene gameScene = this.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<GameScene>();
        return gameScene.m_grid;
    }
}
