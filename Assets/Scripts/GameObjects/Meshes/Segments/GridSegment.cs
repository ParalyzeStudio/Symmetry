using UnityEngine;

/**
 * Class to render a segment with both endpoints as grid coordinates points (column, line)
 * **/
public class GridSegment : Segment
{
    //coordinates of pointA and pointB of this segment in grid coordinates (column, line)
    protected Vector2 m_gridPointA;
    protected Vector2 m_gridPointB;

    public new void Build(Vector2 gridPointA, Vector2 gridPointB, float thickness, Color color, int numSegmentsPerHalfCircle = DEFAULT_NUM_SEGMENTS_PER_HALF_CIRCLE)
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 pointA = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(gridPointA);
        Vector2 pointB = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(gridPointB);
        base.Build(pointA, pointB, thickness, color, numSegmentsPerHalfCircle);
    }

    public void SetGridPointA(Vector2 gridPointA)
    {
        m_gridPointA = gridPointA;

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 pointA = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(gridPointA);
        SetPointA(pointA);
    }

    public void SetGridPointB(Vector2 gridPointB)
    {
        m_gridPointB = gridPointB;

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 pointB = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(gridPointB);
        SetPointB(pointB);
    }
}

