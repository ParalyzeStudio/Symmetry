#define REMOVE_THREADS_FOR_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridTouchHandler : TouchHandler 
{
    public float m_axisCreationMinDistance;

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        //First we verify if we entered the move shape mode
        GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;

        GUIButton.GUIButtonID mainActionsButtonID = gameScene.GetActionButtonID(ActionButton.GroupID.MAIN_ACTIONS);
        if (mainActionsButtonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES ||
            mainActionsButtonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE ||
            mainActionsButtonID == GUIButton.GUIButtonID.ID_POINT_SYMMETRY)
        {
            Grid grid = gameScene.m_grid;
            Vector2 gridSize = grid.m_gridSize;
            Vector2 gridPosition = this.gameObject.transform.position;

            Vector2 border = new Vector2(0.5f * grid.m_gridSpacing, 0.5f * grid.m_gridSpacing);
            Vector2 gridMax = gridPosition + 0.5f * gridSize + border;
            Vector2 gridMin = gridPosition - 0.5f * gridSize - border;

            return pointerLocation.x <= gridMax.x && pointerLocation.x >= gridMin.x
                   &&
                   pointerLocation.y <= gridMax.y && pointerLocation.y >= gridMin.y;
        }

        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        Grid.GridAnchor closestAnchor = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorForWorldPosition(pointerLocation);

        Axis.AxisSymmetryType symmetryType = Axis.GetSymmetryTypeFromActionButtonID(gameScene.GetActionButtonID(ActionButton.GroupID.MAIN_ACTIONS));
        Axis axis = new Axis(closestAnchor.m_gridPosition, closestAnchor.m_gridPosition, Axis.AxisType.DYNAMIC_UNSNAPPED, symmetryType);
        gameScene.m_axesHolder.BuildAxis(axis);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
        Axis currentAxis = gameScene.m_axesHolder.GetAxisBeingDrawn();

        if (currentAxis == null)
            return false;

        //render the axis again
        AxisRenderer axisRenderer = currentAxis.m_parentRenderer;

        //find the constrained direction that will allow us to calculate the projected pointer location
        Vector2 constrainedDirection;
        float projectionLength;
        axisRenderer.FindConstrainedDirection(pointerLocation, out constrainedDirection, out projectionLength);
        Vector2 axisEndpoint1WorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(axisRenderer.m_axisData.m_pointA);
        pointerLocation = axisEndpoint1WorldPosition + constrainedDirection * projectionLength;

        axisRenderer.SnapAxisEndpointToClosestAnchor(pointerLocation);

        //if (axisRenderer.isAxisSnapped())
        //{
        //    axisRenderer.TryToUnsnap(pointerLocation);
        //}
        //else
        //{
        //    //try to snap, if not just render the axis normally
        //    if (!axisRenderer.TryToSnapAxisEndpointToClosestAnchor())
        //    {
        //        axisRenderer.Render(axisRenderer.m_endpoint1Position, pointerLocation, false);
        //    }
        //}

        return true;
    }

    protected override void OnPointerUp()
    {
        if (!m_selected)
            return;

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
        Axis currentAxis = gameScene.m_axesHolder.GetAxisBeingDrawn();

        if (currentAxis != null)
        {
            AxisRenderer axisRenderer = currentAxis.m_parentRenderer;
            if (currentAxis.m_type == Axis.AxisType.DYNAMIC_SNAPPED)
            {
                Level currentLevel = GetLevelManager().m_currentLevel;
                if (currentLevel.m_symmetriesStackable)
                {
                    currentAxis.m_type = Axis.AxisType.STATIC_PENDING; //make the axis static

                    //stack the symmetry
                    //gameScene.m_gameStack.PushAxis(currentAxis);
                }
                else
                {
                    currentAxis.m_type = Axis.AxisType.STATIC_DONE; //make the axis static

                    //Launch the symmetry process
                    Symmetrizer symmetrizer = axisRenderer.GetComponent<Symmetrizer>();
                    symmetrizer.Symmetrize();
                }
            }
            else if (currentAxis.m_type == Axis.AxisType.DYNAMIC_UNSNAPPED) //we can get rid off this axis
            {
                //remove the axis from the axes list and destroy the object
                gameScene.m_axesHolder.RemoveAxis(axisRenderer);
                Destroy(axisRenderer.gameObject);
            }
        }
    }
}