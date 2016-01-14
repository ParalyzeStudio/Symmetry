using UnityEngine;
using System.Collections.Generic;

public class ShapeTouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        GridPoint pointerGridLocation = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointerLocation);

        GUIButton.GUIButtonID mainActionsButtonID = gameScene.GetActionButtonID(ActionButton.GroupID.MAIN_ACTIONS);
        if (mainActionsButtonID == GUIButton.GUIButtonID.ID_MOVE_SHAPE)
        {
            Shape shape = GetComponent<ShapeMesh>().m_shapeData;

            if (shape.m_state == Shape.ShapeState.STATIC) //we can only move STATIC shapes
            {
                //Get the triangles of this shape from the MeshFilter
                return shape.ContainsPoint(pointerGridLocation);
            }
        }
        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        Debug.Log("OnPointerDown");
        Shape pickedShape = this.GetComponent<ShapeMesh>().m_shapeData;
        pickedShape.m_offset = Vector2.zero;
        pickedShape.m_gridOffset = GridPoint.zero;
        pickedShape.m_state = Shape.ShapeState.MOVING_ORIGINAL_SHAPE;

        pickedShape.InvalidateSubstitutionShapes();

        ShapeAnimator shapeAnimator = this.GetComponent<ShapeAnimator>();
        Vector3 shapeNewPosition = new Vector3(0, 0, GameScene.TILED_BACKGROUND_RELATIVE_Z_VALUE + 1); //place the shape behind the tiled background so it becomes invisible
        shapeAnimator.SetPosition(shapeNewPosition);

        base.OnPointerDown(pointerLocation);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        Debug.Log("OnPointerMove");

        //convert the delta vector to grid coordinates and set it to the shape
        ShapeMesh shapeMesh = this.GetComponent<ShapeMesh>();
        Shape shape = shapeMesh.m_shapeData;
        shape.m_offset += delta;

        //Instead of transforming the delta (which can be very tiny) in grid coordinates that can lead to approximation errors,
        //we add the delta to the world offset of the shape. Then we transform the global offset once
        //(i.e instead of adding multiple big approximation errors we only make one when transforming the whole offset)
        GridPoint prevGridOffset = shape.m_gridOffset;
        Grid grid = ((GameScene) GetSceneManager().m_currentScene).m_grid;
        shape.m_gridOffset = grid.TransformWorldVectorIntoGridVector(shape.m_offset);
        GridPoint deltaGridOffset = shape.m_gridOffset - prevGridOffset;
        shape.Translate(deltaGridOffset);
                
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
        shape.InvalidateSubstitutionShapes();
        shapeMesh.Render();

        return true;
    }

    protected override void OnPointerUp()
    {
        if (m_selected)
        {
            GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

            ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
            Shape shape = shapeMesh.m_shapeData;

            GridPoint firstShapeVertex = shape.m_contour[0];

            //Find the closest anchor to one of this shape vertices
            Grid.GridAnchor closestGridAnchor = gameScene.m_grid.GetClosestGridAnchorForGridPosition(firstShapeVertex);
            //Calculate the translation between old and new shape position
            GridPoint translation = closestGridAnchor.m_gridPosition - shape.m_contour[0];

            shape.Translate(translation); //translate all vertices
            
            shape.InvalidateSubstitutionShapes();
            shape.FinalizeClippingOperationsOnSubstitutionShapes(); 

            shape.m_offset = Vector2.zero; //reset offset to zero
            shape.m_gridOffset = GridPoint.zero; //reset offset to zero
            shapeMesh.Render(); //render again the shape

            //invalidate one more time overlapping shapes and then finalize the clipping operations
            //gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();
            //gameScene.m_shapes.FinalizeClippingOperations();

            m_selected = false;
        }
    }
}

