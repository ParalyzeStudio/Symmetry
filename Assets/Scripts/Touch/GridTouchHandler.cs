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

        GUIButton.GUIButtonID topActionID = gameScene.GetActionButtonID(ActionButton.Location.TOP);
        if (topActionID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES ||
            topActionID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE ||
            topActionID == GUIButton.GUIButtonID.ID_POINT_SYMMETRY)
        {
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

        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        Vector2 closestAnchorGridCoords = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorCoordinatesForPosition(pointerLocation);

        gameScene.m_axes.BuildAxis(closestAnchorGridCoords);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
        AxisRenderer currentAxis = gameScene.m_axes.GetAxisBeingDrawn();

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
            {
                axisRenderer.Render(axisRenderer.m_endpoint1Position, pointerLocation, false);
            }
        }

        return true;
    }

    protected override void OnPointerUp()
    {
        if (!m_selected)
            return;

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
        AxisRenderer currentAxis = gameScene.m_axes.GetAxisBeingDrawn();

        if (currentAxis != null)
        {
            if (currentAxis.m_type == AxisRenderer.AxisType.DYNAMIC_SNAPPED)
            {
                currentAxis.m_type = AxisRenderer.AxisType.STATIC; //make the axis static
                Symmetrizer symmetrizer = currentAxis.GetComponent<Symmetrizer>();

                ThreadedJobsManager threadedJobsManager = gameScene.GetThreadedJobsManager();
                threadedJobsManager.AddAndRunJob(new ThreadedJob
                                                (
                                                    new ThreadedJob.ThreadFunction(symmetrizer.SymmetrizeByAxis),
                                                    null,
                                                    new ThreadedJob.ThreadFunction(symmetrizer.OnSymmetryDone)
                                                    )
                                                 );
            }
            else if (currentAxis.m_type == AxisRenderer.AxisType.DYNAMIC_UNSNAPPED) //we can get rid off this axis
            {
                //remove the axis from the axes list and destroy the object
                gameScene.m_axes.RemoveAxis(currentAxis.gameObject);
                Destroy(currentAxis.gameObject);
            }
        }
    }
}