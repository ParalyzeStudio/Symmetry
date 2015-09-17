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
        m_gridPointMode = !m_gridPointMode;
        Grid grid = ((GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<SceneManager>().m_currentScene).GetComponentInChildren<Grid>();

        if (m_gridPointMode)
        {
            //modify contour points
            for (int i = 0; i != m_contour.Count; i++)
            {
                Vector2 contourPoint = grid.GetPointGridCoordinatesFromWorldCoordinates(m_contour[i]);
                m_contour[i] = contourPoint;
            }

            //modify holes points
            for (int i = 0; i != m_holes.Count; i++)
            {
                Contour hole = m_holes[i];
                for (int j = 0; j != hole.Count; j++)
                {
                    Vector2 holePoint = grid.GetPointGridCoordinatesFromWorldCoordinates(hole[j]);
                    hole[j] = holePoint;
                }
            }
        }
        else
        {
            //modify contour points
            for (int i = 0; i != m_contour.Count; i++)
            {
                Vector2 contourPoint = grid.GetPointWorldCoordinatesFromGridCoordinates(m_contour[i]);
                m_contour[i] = contourPoint;
            }

            //modify holes points
            for (int i = 0; i != m_holes.Count; i++)
            {
                Contour hole = m_holes[i];
                for (int j = 0; j != hole.Count; j++)
                {
                    Vector2 holePoint = grid.GetPointWorldCoordinatesFromGridCoordinates(hole[j]);
                    hole[j] = holePoint;
                }
            }
        }
    }
}
