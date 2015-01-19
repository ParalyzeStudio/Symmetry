using UnityEngine;

public class ShapeTouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        //Get the triangles of this shape from the MeshFilter
        float triangleCount = GetComponent<ShapeRenderer>().m_triangles.Count;
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        for (int triangleIndex = 0; triangleIndex != triangleCount; triangleIndex++)
        {
            Vector2 trianglePoint1 = vertices[3 * triangleIndex];
            Vector2 trianglePoint2 = vertices[3 * triangleIndex + 1];
            Vector2 trianglePoint3 = vertices[3 * triangleIndex + 2];
            if (MathUtils.IsInsideTriangle(pointerLocation, trianglePoint1, trianglePoint2, trianglePoint3))
                return true;
        }
        return false;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        gridObject.GetComponent<GridBuilder>().GetAnchorGridCoordinatesFromWorldCoordinates(pointerLocation);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, ref delta))
            return false;

        //move the shape of delta vector
        this.gameObject.transform.localPosition += MathUtils.BuildVector3FromVector2(delta, 0);

        return true;
    }

    protected override void OnPointerUp()
    {
        base.OnPointerUp();

        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();

        //Shift all values in the ShapeRenderer.m_triangles by calculating the new GridAnchor position of one of the shape Vertices (take the first one for instance)
        //and applying this shift value to all vertices
        Vector2 oldGridAnchorCoords = shapeRenderer.m_triangles[0].m_points[0];
        Vector2 firstShapeVertex = meshFilter.sharedMesh.vertices[0];
        //calculate the world position of this vertex by adding the game object transform.position
        firstShapeVertex += MathUtils.BuildVector2FromVector3(this.gameObject.transform.position);

        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        Vector2 newGridAnchorCoords = gridObject.GetComponent<GridBuilder>().GetClosestGridAnchorCoordinatesForPosition(firstShapeVertex);
        Vector2 shift = newGridAnchorCoords - oldGridAnchorCoords;

        shapeRenderer.ShiftShapeVertices(shift); //shift vertices
        this.gameObject.transform.localPosition = Vector3.zero; //reset game object position to zero
        shapeRenderer.Render(meshFilter.sharedMesh, ShapeRenderer.RenderFaces.DOUBLE_SIDED); //render again the shape
    }
}

