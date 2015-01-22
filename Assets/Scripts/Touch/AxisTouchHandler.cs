using UnityEngine;
using System.Collections;

public class AxisTouchHandler : TouchHandler 
{
    public Vector2 m_endpointTouchArea;

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        AxisRenderer axisBuilder = this.gameObject.GetComponent<AxisRenderer>();
        return IsPointerLocationContainedInEndpoint(axisBuilder.m_endpoint1, pointerLocation) || IsPointerLocationContainedInEndpoint(axisBuilder.m_endpoint2, pointerLocation);
    }

    private bool IsPointerLocationContainedInEndpoint(GameObject endpoint, Vector2 pointerLocation)
    {
        if (endpoint == null)
            return false;

        Rect touchAreaRect = new Rect();
        touchAreaRect.position = MathUtils.BuildVector2FromVector3(endpoint.transform.position) - 0.5f * m_endpointTouchArea;

        touchAreaRect.width = m_endpointTouchArea.x;
        touchAreaRect.height = m_endpointTouchArea.y;
        return touchAreaRect.Contains(pointerLocation);
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        
    }

    protected override void OnPointerUp()
    {

    }
}
