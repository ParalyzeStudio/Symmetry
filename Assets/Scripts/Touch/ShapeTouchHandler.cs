using UnityEngine;
using System.Collections.Generic;

public class ShapeTouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        //First we verify if we entered the move shape mode
        //if (!m_gameScene.IsMoveShapeHUDButtonSelected())
        //    return false;

        //Get the triangles of this shape from the MeshFilter
        return GetComponent<ShapeRenderer>().m_shape.ContainsWorldPoint(pointerLocation);
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        this.GetComponent<ShapeRenderer>().m_shape.m_gridOffsetOnVertices = Vector2.zero;

        base.OnPointerDown(pointerLocation);

        GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;
        gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointerLocation);

        gameScene.m_shapes.m_translatedShapeObject = this.gameObject;
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, delta))
            return false;

        //move the shape of delta vector        
        this.gameObject.transform.localPosition += GeometryUtils.BuildVector3FromVector2(delta, 0);

        //convert the delta vector to grid coordinates and set it to the shape
        Shape shape = this.GetComponent<ShapeRenderer>().m_shape;
        shape.m_offsetOnVertices += delta;

        //Instead of transforming the delta (which can be very tiny) in grid coordinates that can lead to approximation errors,
        //we add the delta to the world offset of the shape. Then we transform the global offset once
        //(i.e instead of adding multiple big approximation errors we only make one when transforming the whole offset)
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
        Vector2 gridOffset = gameScene.m_grid.TransformWorldVectorToGridVector(shape.m_offsetOnVertices);
        shape.m_gridOffsetOnVertices = gridOffset;

        gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();

        return true;
    }

    protected override void OnPointerUp()
    {
        if (!m_selected)
            return;

        base.OnPointerUp();

        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();

        //Shift all values in the ShapeRenderer.m_gridTriangles by calculating the new GridAnchor position of one of the shape Vertices (take the first one for instance)
        //and applying this shift value to all vertices
        Vector2 oldGridAnchorCoords = shapeRenderer.m_shape.m_gridTriangles[0].m_points[0];
        Vector2 firstShapeVertex = meshFilter.sharedMesh.vertices[0];
        
        //calculate the world position of this vertex by adding the game object transform.position
        firstShapeVertex += GeometryUtils.BuildVector2FromVector3(this.gameObject.transform.position);

        Vector2 newGridAnchorCoords = gameScene.m_grid.GetClosestGridAnchorCoordinatesForPosition(firstShapeVertex);
        Vector2 shift = newGridAnchorCoords - oldGridAnchorCoords;

        shapeRenderer.ShiftShapeVertices(shift); //shift vertices

        this.gameObject.transform.localPosition = Vector3.zero; //reset game object position to zero
        shapeRenderer.m_shape.m_offsetOnVertices = Vector2.zero; //reset offset to zero
        shapeRenderer.m_shape.m_gridOffsetOnVertices = Vector2.zero; //reset offset to zero
        //Shapes.PerformFusionOnShape(shapeRenderer.m_shape);
        //shapeRenderer.Render(ShapeRenderer.RenderFaces.DOUBLE_SIDED); //render again the shape


        gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();

        //Transfer substitution shape objects to normal shape objects list
        List<GameObject> substitutionShapeObjects = gameScene.m_shapes.m_substitutionShapeObjects;
        for (int i = 0; i != substitutionShapeObjects.Count; i++)
        {
            gameScene.m_shapes.m_shapesObjects.Add(substitutionShapeObjects[i]);
        }

        //clean up the list of substition shape objects
        gameScene.m_shapes.ClearSubstitutionShapeObjects();

        //clean up the list of overlapping shape objects and remove them from the normal shape objects list too
        List<GameObject> overlappingShapeObjects = gameScene.m_shapes.m_overlappingShapeObjects;
        for (int i = 0; i != overlappingShapeObjects.Count; i++)
        {
            gameScene.m_shapes.m_shapesObjects.Remove(overlappingShapeObjects[i]);
        }
        gameScene.m_shapes.ClearOverlappingShapeObjects(true);

        //Set the translated shape object to null
        gameScene.m_shapes.m_translatedShapeObject = null;
    }
}

