using UnityEngine;
using System.Collections;

public class GridTouchHandler : TouchHandler 
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Grid grid = gameScene.m_grid;
        Vector2 gridSize = grid.m_gridSize;
        Vector2 gridPosition = this.gameObject.transform.position;

        float borderThickness = 0.5f * grid.m_gridSpacing;
        Vector2 gridMax = gridPosition + 0.5f * gridSize + new Vector2(borderThickness, borderThickness);
        Vector2 gridMin = gridPosition - 0.5f * gridSize - new Vector2(borderThickness, borderThickness);

        return pointerLocation.x <= gridMax.x && pointerLocation.x >= gridMin.x
               &&
               pointerLocation.y <= gridMax.y && pointerLocation.y >= gridMin.y;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);
    }

    protected override void OnPointerUp()
    {
        base.OnPointerUp();
    }

    protected override void OnClick()
    {
        Debug.Log("OnClick grid");
        Vector2 clickedAnchorGridCoords = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorCoordinatesForPosition(m_prevPointerLocation);
        AxisRenderer axisBuilder = IsSettingEndpoints();
        if (axisBuilder) //we build the second endpoint
        {
            axisBuilder.BuildEndpointAtGridPosition(clickedAnchorGridCoords);
        }
        else //We can build another axis
        {
            GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
            gameScene.m_axes.BuildAxis(clickedAnchorGridCoords);
        }
    }

    /**
     * Returns true if the player is currently setting endpoints for an axis (i.e first endpoint has been set but not second)
     * **/
    private AxisRenderer IsSettingEndpoints()
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        for (int axisIndex = 0; axisIndex != gameScene.m_axes.m_childrenAxes.Count; axisIndex++)
        {
            AxisRenderer axisRenderer = gameScene.m_axes.m_childrenAxes[axisIndex].GetComponent<AxisRenderer>();
            if (axisRenderer.m_buildStatus == AxisRenderer.BuildStatus.FIRST_ENDPOINT_SET)
                return axisRenderer;
        }

        return null;
    }
}