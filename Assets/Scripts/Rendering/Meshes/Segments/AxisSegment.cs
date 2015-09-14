using UnityEngine;
using System.Collections;

public class AxisSegment : ColorSegment
{
    public void Build(Vector2 gridPointA, Vector2 gridPointB, float thickness, Material material, Color color)
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
        Grid grid = gameScene.m_grid;
        Vector2 worldPointA = grid.GetPointWorldCoordinatesFromGridCoordinates(gridPointA);
        Vector2 worldPointB = grid.GetPointWorldCoordinatesFromGridCoordinates(gridPointB);

        base.Build(worldPointA, worldPointB, thickness, material, color, true, 0);
    }
}
