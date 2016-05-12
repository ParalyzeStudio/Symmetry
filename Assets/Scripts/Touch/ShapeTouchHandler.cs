using UnityEngine;
using System.Collections.Generic;

public class ShapeTouchHandler : TouchHandler
{
    private bool m_shapeMoved; //use this to differentiate between the first call to OnPointerMove and the other ones

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        GridPoint pointerGridLocation = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointerLocation);
        Shape shape = GetComponent<ShapeMesh>().m_shapeData;

        if (shape.m_state == Shape.ShapeState.STATIC) //we can only move STATIC shapes
        {
            //Get the triangles of this shape from the MeshFilter
            return shape.ContainsPoint(pointerGridLocation);
        }
        return false;
    }

    private void SelectShape()
    {
        m_selected = true;

        ShapeMesh shapeMesh = this.GetComponent<ShapeMesh>();
        Shape pickedShape = shapeMesh.m_shapeData;
        pickedShape.m_offset = Vector2.zero;
        pickedShape.m_gridOffset = GridPoint.zero;

        m_shapeMoved = false;
    }

    /**
    * Process the pointer event and dispatch it to the relevant callback
    * **/
    public override bool ProcessPointerEvent(Vector2 pointerLocation, TouchManager.PointerEventType eventType)
    {
        //we do not process events on shapes if moving a shape is disabled in this level
        if ((GetLevelManager().m_currentLevel.Actions & Level.ACTION_MOVE_SHAPE) == 0)
            return false;
        else
            return base.ProcessPointerEvent(pointerLocation, eventType);
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        SelectShape();
        base.OnPointerDown(pointerLocation);        
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        ShapeMesh shapeMesh = this.GetComponent<ShapeMesh>();
        Shape shape = shapeMesh.m_shapeData;
        Grid grid = ((GameScene)GetSceneManager().m_currentScene).m_grid;

        if (!m_shapeMoved) //shape has not been moved yet
        {
            //place the shape behind the whole scene (scene is at 0) so it becomes invisible
            TexturedMeshAnimator shapeAnimator = this.GetComponent<TexturedMeshAnimator>();
            shapeAnimator.SetParentTransform(null);
            Vector3 shapeNewPosition = new Vector3(0, 0, 1);
            shapeAnimator.SetPosition(shapeNewPosition);

            //Change the state of this shape
            shape.m_state = Shape.ShapeState.MOVING_ORIGINAL_SHAPE;

            //draw contour around shape
            shapeMesh.DrawSelectionContour();

            //toggle the boolean value so the previous code is not called later again
            m_shapeMoved = true;

            //unlock any grid border that is intersecting this shape
            GridBorder[] gridborders = grid.Borders;
            for (int i = 0; i != gridborders.Length; i++)
            {
                if (shapeMesh.m_shapeData.m_contour.IntersectsEdge(grid.GetGridBoxEdgeForLocation(gridborders[i].m_location)))
                    gridborders[i].Unlock();
            }
        }

        //convert the delta vector to grid coordinates and set it to the shape                
        shape.m_offset += delta;

        //Instead of transforming the delta (which can be very tiny) in grid coordinates that can lead to approximation errors,
        //we add the delta to the world offset of the shape. Then we transform the global offset once
        //(i.e instead of adding multiple big approximation errors we only make one when transforming the whole offset)
        GridPoint prevGridOffset = shape.m_gridOffset;
        shape.m_gridOffset = grid.TransformWorldVectorIntoGridVector(shape.m_offset);
        GridPoint deltaGridOffset = shape.m_gridOffset - prevGridOffset;
        shape.Translate(deltaGridOffset);
                
        shape.InvalidateSubstitutionShapes();

        shapeMesh.TranslateSelectionContour(delta);

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

            //release the contour around shape
            shapeMesh.TranslateSelectionContour(gameScene.m_grid.TransformWorldVectorIntoGridVector(minTranslation));
            shapeMesh.ReleaseSelectionContour();

            //lock any grid border that is intersecting this shape
            Grid grid = gameScene.m_grid;
            GridBorder[] gridborders = grid.Borders;
            for (int i = 0; i != gridborders.Length; i++)
            {
                if (shapeMesh.m_shapeData.m_contour.IntersectsEdge(grid.GetGridBoxEdgeForLocation(gridborders[i].m_location)))
                    gridborders[i].Lock();
            }
        }
    }
}