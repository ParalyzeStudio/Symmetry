#define REMOVE_THREADS_FOR_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridTouchHandler : TouchHandler 
{
    public float m_axisCreationMinDistance;

    private enum DrawAxisTestMode
    {
        SLIDE_TO_DRAW,
        CLICK_TO_DRAW
    }
    private DrawAxisTestMode m_drawAxisTestMode;

    public override void Awake()
    {
        base.Awake();
        //m_drawAxisTestMode = DrawAxisTestMode.SLIDE_TO_DRAW;
        m_drawAxisTestMode = DrawAxisTestMode.CLICK_TO_DRAW;
    }

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return true;

        ////First we verify if we entered the move shape mode
        //GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;

        //GUIButton.GUIButtonID mainActionsButtonID = gameScene.GetActionButtonID(ActionButton.GroupID.MAIN_ACTIONS);
        //if (mainActionsButtonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES ||
        //    mainActionsButtonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE ||
        //    mainActionsButtonID == GUIButton.GUIButtonID.ID_POINT_SYMMETRY)
        //{
        //    Grid grid = gameScene.m_grid;
        //    Vector2 gridSize = grid.m_gridSize;
        //    Vector2 gridPosition = this.gameObject.transform.position;

        //    Vector2 border = new Vector2(0.5f * grid.m_gridSpacing, 0.5f * grid.m_gridSpacing);
        //    Vector2 gridMax = gridPosition + 0.5f * gridSize + border;
        //    Vector2 gridMin = gridPosition - 0.5f * gridSize - border;

        //    return pointerLocation.x <= gridMax.x && pointerLocation.x >= gridMin.x
        //           &&
        //           pointerLocation.y <= gridMax.y && pointerLocation.y >= gridMin.y;
        //}

        //return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        if (m_drawAxisTestMode == DrawAxisTestMode.SLIDE_TO_DRAW)
        {
            GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

            Grid.GridAnchor closestAnchor = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorForWorldPosition(pointerLocation);

            //Axis.AxisSymmetryType symmetryType = Axis.GetSymmetryTypeFromActionButtonID(gameScene.GetActionButtonID(ActionButton.GroupID.MAIN_ACTIONS));
            Axis.AxisSymmetryType symmetryType = Axis.AxisSymmetryType.SYMMETRY_AXES_ONE_SIDE;
            Axis axis = new Axis(closestAnchor.m_gridPosition, closestAnchor.m_gridPosition, Axis.AxisType.DYNAMIC_UNSNAPPED, symmetryType);
            gameScene.m_axesHolder.BuildAxis(axis);
        }
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        if (m_drawAxisTestMode == DrawAxisTestMode.SLIDE_TO_DRAW)
        {
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
        }
        else if (m_drawAxisTestMode == DrawAxisTestMode.CLICK_TO_DRAW) //do not process the OnPointerMove event when drawing an axis by clicking on the grid, so the shapes can process it
            return false;

        return true;
    }

    protected override void OnPointerUp()
    {
        if (m_drawAxisTestMode == DrawAxisTestMode.SLIDE_TO_DRAW)
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
        else if (m_drawAxisTestMode == DrawAxisTestMode.CLICK_TO_DRAW)
        {
            base.OnPointerUp();
        }
    }

    protected override void OnClick(Vector2 clickLocation)
    {
        if (m_drawAxisTestMode == DrawAxisTestMode.CLICK_TO_DRAW)
        {
            base.OnClick(clickLocation);

            GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

            Grid.GridAnchor closestAnchor = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorForWorldPosition(clickLocation);

            //Test if an axis is already under construction
            Axis axis = gameScene.m_axesHolder.FindAxisUnderConstruction();
            if (axis == null)
            {
                //If not build a new one
                axis = new Axis();
                axis.m_pointA = closestAnchor.m_gridPosition;
                axis.m_type = Axis.AxisType.UNDER_CONSTRUCTION;
                gameScene.m_axesHolder.BuildAxis(axis);
            }
            else
            {
                axis.m_pointB = closestAnchor.m_gridPosition;
                axis.m_type = Axis.AxisType.DYNAMIC_SNAPPED;
                gameScene.m_axesHolder.BuildAxis(axis);

                //perform symmetry
                AxisRenderer axisRenderer = axis.m_parentRenderer;
                if (axis.m_type == Axis.AxisType.DYNAMIC_SNAPPED)
                {
                    Level currentLevel = GetLevelManager().m_currentLevel;
                    axis.m_type = Axis.AxisType.STATIC_DONE; //make the axis static

                    //Launch the symmetry process
                    Symmetrizer symmetrizer = axisRenderer.GetComponent<Symmetrizer>();
                    symmetrizer.Symmetrize();
                    Debug.Log("perform symmetry with axis A:" + axis.m_pointA + " B:" + axis.m_pointB);
                }
            }
        }
    }
}