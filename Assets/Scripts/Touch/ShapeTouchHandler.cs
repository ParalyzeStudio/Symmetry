using UnityEngine;
using System.Collections.Generic;

public class ShapeTouchHandler : TouchHandler
{
    private bool m_shapeMoved; //use this to differentiate between the first call to OnPointerMove and the other ones

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        GridPoint pointerGridLocation = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointerLocation);

        //GUIButton.GUIButtonID mainActionsButtonID = gameScene.GetActionButtonID(ActionButton.GroupID.MAIN_ACTIONS);
        //if (mainActionsButtonID == GUIButton.GUIButtonID.ID_MOVE_SHAPE)
        //{
            Shape shape = GetComponent<ShapeMesh>().m_shapeData;

            if (shape.m_state == Shape.ShapeState.STATIC) //we can only move STATIC shapes
            {
                //Get the triangles of this shape from the MeshFilter
                return shape.ContainsPoint(pointerGridLocation);
            }
        //}
        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        Shape pickedShape = this.GetComponent<ShapeMesh>().m_shapeData;
        pickedShape.m_offset = Vector2.zero;
        pickedShape.m_gridOffset = GridPoint.zero;

        m_shapeMoved = false;

        //pickedShape.InvalidateSubstitutionShapes();

        //TexturedMeshAnimator shapeAnimator = this.GetComponent<TexturedMeshAnimator>();
        //Vector3 shapeNewPosition = new Vector3(0, 0, GameScene.TILED_BACKGROUND_RELATIVE_Z_VALUE + 1); //place the shape behind the tiled background so it becomes invisible
        //shapeAnimator.SetPosition(shapeNewPosition);

        base.OnPointerDown(pointerLocation);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        ShapeMesh shapeMesh = this.GetComponent<ShapeMesh>();
        Shape shape = shapeMesh.m_shapeData;

        if (!m_shapeMoved) //shape has not been moved yet
        {
            //place the shape behind the tiled background so it becomes invisible
            TexturedMeshAnimator shapeAnimator = this.GetComponent<TexturedMeshAnimator>();
            Vector3 shapeNewPosition = new Vector3(0, 0, GameScene.TILED_BACKGROUND_RELATIVE_Z_VALUE + 1);
            shapeAnimator.SetPosition(shapeNewPosition);

            //Change the state of this shape
            shape.m_state = Shape.ShapeState.MOVING_ORIGINAL_SHAPE;

            //toggle the boolean value so the previous code is not called later again
            m_shapeMoved = true;
        }

        //convert the delta vector to grid coordinates and set it to the shape                
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

        return true;
    }

    protected override void OnPointerUp()
    {
        if (m_selected)
        {
            GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

            ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
            Shape shape = shapeMesh.m_shapeData;

            GridPoint minTranslation = GridPoint.zero;
            long minDistance = long.MaxValue;
            for (int i = 0; i != shape.m_contour.Count; i++)
            {
                GridPoint shapeVertex = shape.m_contour[i];
                Grid.GridAnchor closestGridAnchor = gameScene.m_grid.GetClosestGridAnchorForGridPosition(shapeVertex);
                if (closestGridAnchor != null)
                {
                    GridPoint translation = closestGridAnchor.m_gridPosition - shapeVertex;
                    long sqrDistanceToAnchor = translation.sqrMagnitude;
                    if (sqrDistanceToAnchor < minDistance)
                    {
                        minDistance = sqrDistanceToAnchor;
                        minTranslation = translation;
                    }
                }
            }

            Debug.Log(minDistance);
            Debug.Log(minTranslation);

            //if (minDistance < long.MaxValue)
            //{
                shape.Translate(minTranslation); //translate all vertices by the value we found previously

                shape.InvalidateSubstitutionShapes();
                shape.FinalizeClippingOperationsOnSubstitutionShapes();
            //}

            m_selected = false;
        }
    }
}

