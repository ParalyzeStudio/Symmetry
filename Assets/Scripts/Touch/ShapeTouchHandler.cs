using UnityEngine;

public class ShapeTouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        //First we verify if we entered the move shape mode
        GameScene gameScene = (GameScene) GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        if (!gameScene.IsMoveShapeHUDButtonSelected())
            return false;

        //Get the triangles of this shape from the MeshFilter
        float triangleCount = GetComponent<ShapeRenderer>().m_shape.m_gridTriangles.Count;
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        for (int triangleIndex = 0; triangleIndex != triangleCount; triangleIndex++)
        {
            Vector2 trianglePoint1 = vertices[3 * triangleIndex];
            Vector2 trianglePoint2 = vertices[3 * triangleIndex + 1];
            Vector2 trianglePoint3 = vertices[3 * triangleIndex + 2];
            if (GeometryUtils.IsInsideTriangle(pointerLocation, trianglePoint1, trianglePoint2, trianglePoint3))
                return true;
        }
        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        gameScene.m_grid.GetGridCoordinatesFromWorldCoordinates(pointerLocation);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, ref delta))
            return false;

        //move the shape of delta vector
        this.gameObject.transform.localPosition += GeometryUtils.BuildVector3FromVector2(delta, 0);

        return true;
    }

    protected override void OnPointerUp()
    {
        if (!m_selected)
            return;

        base.OnPointerUp();

        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();

        //Shift all values in the ShapeRenderer.m_gridTriangles by calculating the new GridAnchor position of one of the shape Vertices (take the first one for instance)
        //and applying this shift value to all vertices
        Vector2 oldGridAnchorCoords = shapeRenderer.m_shape.m_gridTriangles[0].m_points[0];
        Vector2 firstShapeVertex = meshFilter.sharedMesh.vertices[0];
        //calculate the world position of this vertex by adding the game object transform.position
        firstShapeVertex += GeometryUtils.BuildVector2FromVector3(this.gameObject.transform.position);

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 newGridAnchorCoords = gameScene.m_grid.GetClosestGridAnchorCoordinatesForPosition(firstShapeVertex);
        Vector2 shift = newGridAnchorCoords - oldGridAnchorCoords;

        shapeRenderer.ShiftShapeVertices(shift); //shift vertices
        this.gameObject.transform.localPosition = Vector3.zero; //reset game object position to zero
        shapeRenderer.m_shape.Fusion();
        shapeRenderer.Render(meshFilter.sharedMesh, ShapeRenderer.RenderFaces.DOUBLE_SIDED, true); //render again the shape
    }

    protected override void OnClick()
    {
        base.OnClick();
    }
}

