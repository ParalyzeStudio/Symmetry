using UnityEngine;
using System.Collections.Generic;

public class GridTriangulable : Triangulable
{
    protected bool m_gridPointMode; //are the coordinates of the contour and holes are in grid coordinates system

    public GridTriangulable(bool gridPointMode)
        : base()
    {
        m_gridPointMode = gridPointMode;
    }

    public GridTriangulable(bool gridPointMode, Contour contour)
        : base(contour)
    {
        m_gridPointMode = gridPointMode;
    }

    public GridTriangulable(bool gridPointMode, Contour contour, List<Contour> holes)
        : base(contour, holes)
    {
        m_gridPointMode = gridPointMode;
    }

    public GridTriangulable(GridTriangulable other)
        : base(other)
    {
        m_gridPointMode = other.m_gridPointMode;
    }

    public void TogglePointMode()
    {
        Debug.Log("TogglePointMode");
        m_gridPointMode = !m_gridPointMode;
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();
        GameScene gameScene = (GameScene) sceneManager.m_currentScene;
        Grid grid = gameScene.m_grid;

        if (m_gridPointMode)
        {
            //modify contour points
            for (int i = 0; i != m_contour.Count; i++)
            {
                m_contour[i] = grid.GetPointGridCoordinatesFromWorldCoordinates(m_contour[i]);
            }

            //modify holes points
            for (int i = 0; i != m_holes.Count; i++)
            {
                Contour hole = m_holes[i];
                for (int j = 0; j != hole.Count; j++)
                {
                    hole[j] = grid.GetPointGridCoordinatesFromWorldCoordinates(hole[j]);
                }
            }

            //modify triangles
            for (int i = 0; i != m_triangles.Count; i++)
            {
                BaseTriangle triangle = m_triangles[i];
                for (int j = 0; j != 3; j++)
                {
                    triangle.m_points[j] = grid.GetPointGridCoordinatesFromWorldCoordinates(triangle.m_points[j]);
                }
            }
        }
        else
        {
            //modify contour points
            for (int i = 0; i != m_contour.Count; i++)
            {
                m_contour[i] = grid.GetPointWorldCoordinatesFromGridCoordinates(m_contour[i]);
            }

            //modify holes points
            for (int i = 0; i != m_holes.Count; i++)
            {
                Contour hole = m_holes[i];
                for (int j = 0; j != hole.Count; j++)
                {
                    hole[j] = grid.GetPointWorldCoordinatesFromGridCoordinates(hole[j]);
                }
            }

            //modify triangles
            for (int i = 0; i != m_triangles.Count; i++)
            {
                BaseTriangle triangle = m_triangles[i];
                for (int j = 0; j != 3; j++)
                {
                    triangle.m_points[j] = grid.GetPointWorldCoordinatesFromGridCoordinates(triangle.m_points[j]);
                }
            }
        }
    }
}
