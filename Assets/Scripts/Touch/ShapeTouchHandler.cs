using UnityEngine;
using System.Collections.Generic;

public class ShapeTouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        Vector2 pointerGridLocation = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointerLocation);
        int scalePrecision = GridPoint.DEFAULT_SCALE_PRECISION;
        pointerGridLocation *= scalePrecision;
        GridPoint pointerGridPosition = new GridPoint(Mathf.RoundToInt(pointerGridLocation.x), Mathf.RoundToInt(pointerGridLocation.y), scalePrecision);

        GUIButton.GUIButtonID topActionID = gameScene.GetActionButtonID(ActionButton.Location.TOP);
        if (topActionID == GUIButton.GUIButtonID.ID_MOVE_SHAPE)
        {
            //Get the triangles of this shape from the MeshFilter
            return GetComponent<ShapeMesh>().m_shapeData.ContainsPoint(pointerGridPosition);
        }
        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        this.GetComponent<ShapeMesh>().m_shapeData.m_gridOffsetOnVertices = GridPoint.zero;

        base.OnPointerDown(pointerLocation);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        //TMP remove movement for the moment
        return false;
        //if (!base.OnPointerMove(pointerLocation, delta))
        //    return false;

        ////move the shape of delta vector        
        //this.gameObject.transform.localPosition += GeometryUtils.BuildVector3FromVector2(delta, 0);

        ////convert the delta vector to grid coordinates and set it to the shape
        //Shape shape = this.GetComponent<ShapeRenderer>().m_shape;
        //shape.m_offsetOnVertices += delta;

        ////Instead of transforming the delta (which can be very tiny) in grid coordinates that can lead to approximation errors,
        ////we add the delta to the world offset of the shape. Then we transform the global offset once
        ////(i.e instead of adding multiple big approximation errors we only make one when transforming the whole offset)
        //GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
        //Vector2 gridOffset = gameScene.m_grid.TransformWorldVectorToGridVector(shape.m_offsetOnVertices);
        //shape.m_gridOffsetOnVertices = gridOffset;

        //gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();

        //return true;
    }

    protected override void OnPointerUp()
    {
        //TMP remove movement for the moment
        //if (!m_selected)
        //    return;

        //base.OnPointerUp();

        //GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        //ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        //MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();

        ////Shift all values in the ShapeRenderer.m_gridTriangles by calculating the new GridAnchor position of one of the shape Vertices (take the first one for instance)
        ////and applying this shift value to all vertices
        //Vector2 oldGridAnchorCoords = shapeRenderer.m_shape.m_gridTriangles[0].m_points[0];
        //Vector2 firstShapeVertex = meshFilter.sharedMesh.vertices[0];
        
        ////calculate the world position of this vertex by adding the game object transform.position
        //firstShapeVertex += GeometryUtils.BuildVector2FromVector3(this.gameObject.transform.position);

        //Vector2 newGridAnchorCoords = gameScene.m_grid.GetClosestGridAnchorCoordinatesForPosition(firstShapeVertex);
        //Vector2 shift = newGridAnchorCoords - oldGridAnchorCoords;

        //shapeRenderer.ShiftShapeVertices(shift); //shift vertices

        //this.gameObject.transform.localPosition = Vector3.zero; //reset game object position to zero
        //shapeRenderer.m_shape.m_offsetOnVertices = Vector2.zero; //reset offset to zero
        //shapeRenderer.m_shape.m_gridOffsetOnVertices = Vector2.zero; //reset offset to zero
        //Shapes.PerformFusionOnShape(shapeRenderer.m_shape);
        //shapeRenderer.Render(ShapeRenderer.RenderFaces.DOUBLE_SIDED); //render again the shape

        ////invalidate one more time overlapping shapes and then finalize the clipping operations
        //gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();
        //gameScene.m_shapes.FinalizeClippingOperations();
    }
}

