using UnityEngine;
using System.Collections.Generic;

public class ShapeTouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        GridPoint pointerGridLocation = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointerLocation);

        GUIButton.GUIButtonID topActionID = gameScene.GetActionButtonID(ActionButton.Location.TOP);
        if (topActionID == GUIButton.GUIButtonID.ID_MOVE_SHAPE)
        {
            //Get the triangles of this shape from the MeshFilter
            return GetComponent<ShapeMesh>().m_shapeData.ContainsPoint(pointerGridLocation);
        }
        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        this.GetComponent<ShapeMesh>().m_shapeData.m_offset = GridPoint.zero;

        base.OnPointerDown(pointerLocation);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        //convert the delta vector to grid coordinates and set it to the shape
        ShapeMesh shapeMesh = this.GetComponent<ShapeMesh>();
        Shape shape = shapeMesh.m_shapeData;
        shape.m_offset += delta;
        Grid grid = ((GameScene) GetSceneManager().m_currentScene).m_grid;
        shape.m_gridOffset = grid.TransformWorldVectorIntoGridVector(shape.m_offset);

        //Instead of transforming the delta (which can be very tiny) in grid coordinates that can lead to approximation errors,
        //we add the delta to the world offset of the shape. Then we transform the global offset once
        //(i.e instead of adding multiple big approximation errors we only make one when transforming the whole offset)
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        //gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();

        shapeMesh.Render(); //render again the shape

        return true;
    }

    protected override void OnPointerUp()
    {
        if (m_selected)
        {
            Debug.Log("ShapeTouchHandler +++ OnPointerUp");

            GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

            ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
            Shape shape = shapeMesh.m_shapeData;

            GridPoint firstShapeVertex = shape.m_contour[0] + shape.m_gridOffset;

            //Find the closest anchor to one of this shape vertices
            Grid.GridAnchor closestGridAnchor = gameScene.m_grid.GetClosestGridAnchorForGridPosition(firstShapeVertex);
            //Calculate the translation between old and new shape position
            GridPoint translation = closestGridAnchor.m_gridPosition - shape.m_contour[0];

            shape.Translate(translation); //translate all vertices

            shape.m_offset = Vector2.zero; //reset offset to zero
            shape.m_gridOffset = GridPoint.zero; //reset offset to zero
            //Shapes.PerformFusionOnShape(shapeRenderer.m_shape);
            shapeMesh.Render(); //render again the shape

            //invalidate one more time overlapping shapes and then finalize the clipping operations
            //gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();
            //gameScene.m_shapes.FinalizeClippingOperations();

            m_selected = false;
        }
    }
}

