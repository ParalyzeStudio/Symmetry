using UnityEngine;
using System.Collections.Generic;

public class ShapeAnimator : GameObjectAnimator
{
    public override void OnOpacityChanged(float fNewOpacity)
    {
        base.OnOpacityChanged(fNewOpacity);

        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        Color oldColor = shapeRenderer.m_color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, fNewOpacity);
        shapeRenderer.m_color = newColor;
    }

    protected override Vector3 GetGameObjectSize()
    {
        Shape shapeData = this.gameObject.GetComponent<ShapeRenderer>().m_shape;
        List<GridTriangle> triangles = shapeData.m_gridTriangles;
        Vector2 minCoords = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxCoords = new Vector2(float.MinValue, float.MinValue);
        for (int iTriangleIndex = 0; iTriangleIndex != triangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = triangles[iTriangleIndex];
            for (int i = 0; i != 3; i++)
            {
                if (triangle.m_points[i].x < minCoords.x)
                {
                    minCoords.x = triangle.m_points[i].x;
                }
                if (triangle.m_points[i].x > maxCoords.x)
                {
                    maxCoords.x = triangle.m_points[i].x;
                }

                if (triangle.m_points[i].y < minCoords.y)
                {
                    minCoords.y = triangle.m_points[i].y;
                }
                if (triangle.m_points[i].y > maxCoords.y)
                {
                    maxCoords.y = triangle.m_points[i].y;
                }
            }
        }

        GridBuilder gridBuilder = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridBuilder>();
        Vector2 objectSize = gridBuilder.GetWorldCoordinatesFromGridCoordinates(maxCoords) - gridBuilder.GetWorldCoordinatesFromGridCoordinates(minCoords);
        return GeometryUtils.BuildVector3FromVector2(objectSize, 1);
    }

    public override void OnFinishRotating()
    {
        Debug.Log("Shape ended rot");
        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        shapeRenderer.m_shape.Fusion();
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        shapeRenderer.Render(meshFilter.sharedMesh, ShapeRenderer.RenderFaces.DOUBLE_SIDED, true); //render again the shape
    }
}