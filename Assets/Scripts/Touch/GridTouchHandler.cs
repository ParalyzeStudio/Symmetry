#define REMOVE_THREADS_FOR_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridTouchHandler : TouchHandler 
{
    public float m_axisCreationMinDistance;

    public override void Awake()
    {
        base.Awake();
    }

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        Grid grid = gameScene.m_grid;
        Vector2 gridSize = grid.m_gridSize;
        Vector2 gridPosition = this.gameObject.transform.position;

        Vector2 border = new Vector2(0.5f * grid.m_gridSpacing, 0.5f * grid.m_gridSpacing);
        Vector2 gridMax = gridPosition + 0.5f * gridSize + border;
        Vector2 gridMin = gridPosition - 0.5f * gridSize - border;

        return pointerLocation.x <= gridMax.x && pointerLocation.x >= gridMin.x
               &&
               pointerLocation.y <= gridMax.y && pointerLocation.y >= gridMin.y;

        ////First we verify if we entered the move shape mode
        //GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

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
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        //do not process the OnPointerMove event when drawing an axis by clicking on the grid, so the shapes can process it
        return false;
    }

    protected override void OnClick(Vector2 clickLocation)
    {
        base.OnClick(clickLocation);

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        Grid.GridAnchor closestAnchor = this.gameObject.GetComponent<Grid>().GetClosestGridAnchorForWorldPosition(clickLocation);

        //Test if an axis is already under construction

        if (gameScene.m_currentAction == GameScene.Action.NONE)
        {
            if (gameScene.ShowActionMenu(closestAnchor)) //show a contextual menu if available or directly draw the first axis endpoint
            {
                gameScene.m_currentAction = GameScene.Action.SHOWING_ACTION_MENU;
            }
            else
            {
                Axis axis = new Axis();
                axis.m_pointA = closestAnchor.m_gridPosition;
                axis.m_state = Axis.AxisState.UNDER_CONSTRUCTION;
                gameScene.m_axesHolder.BuildAxisRenderer(axis);
                axis.m_parentRenderer.InitializeRendering();

                axis.m_type = Axis.AxisType.SYMMETRY_AXES_ONE_SIDE;
                
                gameScene.DisplayAnchorsForAxis(axis.m_pointA);
                gameScene.m_currentAction = GameScene.Action.DRAWING_AXIS;                
            }
        }
        else if (gameScene.m_currentAction == GameScene.Action.DRAWING_AXIS)
        {
            //make sure the anchor is available
            if (!gameScene.AvailableAnchors.Contains(closestAnchor.m_gridPosition))
                return;

            Axis axis = gameScene.m_axesHolder.FindAxisUnderConstruction();
            axis.m_pointB = closestAnchor.m_gridPosition;

            if (axis.m_pointB == axis.m_pointA) //we clicked on the same anchor, remove the axis
            {
                Destroy(axis.m_parentRenderer.gameObject);
                gameScene.m_axesHolder.RemoveAxis(axis.m_parentRenderer);
                gameScene.DismissActionMenu();
                return;
            }

            axis.m_state = Axis.AxisState.DYNAMIC_SNAPPED;

            AxisRenderer axisRenderer = axis.m_parentRenderer;
            axisRenderer.FinalizeRendering();

            //perform symmetry
            //if (axis.m_state == Axis.AxisState.DYNAMIC_SNAPPED)
            //{
            //    Level currentLevel = GetLevelManager().m_currentLevel;
            //    axis.m_state = Axis.AxisState.STATIC_DONE; //make the axis static

            //    //Launch the symmetry process
            //    Symmetrizer symmetrizer = axisRenderer.GetComponent<Symmetrizer>();
            //    symmetrizer.Symmetrize();
            //}
        }
    }
}