using UnityEngine;
using System.Collections;

public class GridTouchHandler : TouchHandler 
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return true;
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
        GameObject[] allAxis = GameObject.FindGameObjectsWithTag("SymmetryAxis");
        for (int axisIndex = 0; axisIndex != allAxis.Length; axisIndex++)
        {
            AxisRenderer axisRenderer = allAxis[axisIndex].GetComponent<AxisRenderer>();
            if (axisRenderer.m_buildStatus == AxisRenderer.BuildStatus.FIRST_ENDPOINT_SET)
                return axisRenderer;
        }

        return null;
    }

    /**
     * 
     * **/
    public void adz()
    {
    }

}