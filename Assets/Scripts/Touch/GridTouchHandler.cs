using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridTouchHandler : TouchHandler 
{
    public float m_axisCreationMinDistance;

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        //First we verify if we entered the move shape mode
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        //if (!gameScene.IsSymmetryHUDButtonSelected())
        //    return false;

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
        return;

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        if (guiManager.IsPauseWindowShown()) //swallow the touch
            return;

        base.OnPointerDown(pointerLocation);

        Vector2 closestAnchorGridCoords = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorCoordinatesForPosition(pointerLocation);
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 closestAnchorWorldCoords = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(closestAnchorGridCoords);
        float distanceToAnchor = (closestAnchorWorldCoords - pointerLocation).magnitude;
        if (distanceToAnchor < m_axisCreationMinDistance)
        {
            gameScene.m_axes.BuildAxis(closestAnchorGridCoords);
        }
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        GameObject currentAxis = gameScene.m_axes.GetAxisBeingBuilt();

        if (currentAxis == null)
            return false;

        //render the axis again
        AxisRenderer axisRenderer = currentAxis.GetComponent<AxisRenderer>();

        //find the constrained direction that will allow us to calculate the projected pointer location
        Vector2 constrainedDirection;
        float projectionLength;
        axisRenderer.FindConstrainedDirection(pointerLocation, out constrainedDirection, out projectionLength);
        pointerLocation = axisRenderer.m_endpoint1Position + constrainedDirection * projectionLength;

        if (axisRenderer.isAxisSnapped())
        {
            axisRenderer.TryToUnsnap(pointerLocation);
        }
        else
        {
            //try to snap, if not just render the axis normally
            if (!axisRenderer.TryToSnapAxisEndpointToClosestAnchor())
                axisRenderer.Render(axisRenderer.m_endpoint1Position, pointerLocation, false);
        }

        return true;
    }

    protected override void OnPointerUp()
    {
        if (!m_selected)
            return;

        //GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        GameScene gameScene = this.transform.parent.gameObject.GetComponent<GameScene>();
        GameObject currentAxis = gameScene.m_axes.GetAxisBeingBuilt();
        if (currentAxis != null)
        {
            AxisRenderer axisRenderer = currentAxis.GetComponent<AxisRenderer>();

            if (axisRenderer.isAxisSnapped()) //axis is snapped we can perform symmetry
            {
                Symmetrizer symmetrizer = currentAxis.GetComponent<Symmetrizer>();
                symmetrizer.SymmetrizeByAxis();
            }

            //remove the axis from the axes list and destroy the object
            //gameScene.m_axes.RemoveAxis(currentAxis);
            //Destroy(currentAxis);
        }
    }

    protected override void OnClick(Vector2 clickLocation)
    {
        //Vector2 clickedAnchorGridCoords = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorCoordinatesForPosition(m_prevPointerLocation);
        //AxisRenderer axisBuilder = IsSettingEndpoints();
        //if (axisBuilder) //we build the second endpoint
        //{
        //    axisBuilder.BuildEndpointAtGridPosition(clickedAnchorGridCoords);
        //}
        //else //We can build another axis
        //{
        //    GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        //    gameScene.m_axes.BuildAxis(clickedAnchorGridCoords);
        //}
    }
}