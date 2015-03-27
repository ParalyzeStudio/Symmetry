using UnityEngine;
using System.Collections;

public class GridTouchHandler : TouchHandler 
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        GridBuilder gridBuilder = gridObject.GetComponent<GridBuilder>();
        Vector2 gridSize = gridBuilder.m_gridSize;
        Vector2 gridPosition = this.gameObject.transform.position;

        float borderThickness = 0.5f * gridBuilder.m_gridSpacing;
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
        Debug.Log("OnClick");
        Vector2 clickedAnchorGridCoords = this.gameObject.GetComponent<GridBuilder>().GetClosestGridAnchorCoordinatesForPosition(m_prevPointerLocation);
        AxisRenderer axisBuilder = IsSettingEndpoints();
        if (axisBuilder) //we build the second endpoint
        {
            axisBuilder.BuildEndpointAtGridPosition(clickedAnchorGridCoords);
        }
        else //We can build another axis
        {
            AxesHolder axesHolder = GameObject.FindGameObjectWithTag("Axes").GetComponent<AxesHolder>();
            axesHolder.BuildAxis(clickedAnchorGridCoords);
        }
    }

    /**
     * Returns true if the player is currently setting endpoints for an axis (i.e first endpoint has been set but not second)
     * **/
    private AxisRenderer IsSettingEndpoints()
    {
        AxesHolder axesHolder = GameObject.FindGameObjectWithTag("Axes").GetComponent<AxesHolder>();
        for (int axisIndex = 0; axisIndex != axesHolder.m_axes.Count; axisIndex++)
        {
            AxisRenderer axisRenderer = axesHolder.m_axes[axisIndex].GetComponent<AxisRenderer>();
            if (axisRenderer.m_buildStatus == AxisRenderer.BuildStatus.FIRST_ENDPOINT_SET)
                return axisRenderer;
        }

        return null;
    }
}