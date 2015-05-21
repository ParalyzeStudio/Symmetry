using UnityEngine;
using System.Collections.Generic;

public class ShapeAnimator : GameObjectAnimator
{
    public override void SetOpacity(float opacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(opacity, bPassOnChildren);

        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        shapeRenderer.SetColor(m_color);

        //shapeRenderer.Render(false, ShapeRenderer.RenderFaces.DOUBLE_SIDED, false, true);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        shapeRenderer.SetColor(color);

        //shapeRenderer.Render(false, ShapeRenderer.RenderFaces.DOUBLE_SIDED, false, true);
    }

    public override Vector3 GetGameObjectSize()
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

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 objectSize = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(maxCoords) - gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(minCoords);
        return GeometryUtils.BuildVector3FromVector2(objectSize, 1);
    }

    public override void OnFinishRotating()
    {
        //shapeRenderer.m_shape.Fusion();
        ShapeRenderer shapeRenderer = this.gameObject.GetComponent<ShapeRenderer>();
        //Shapes.PerformFusionOnShape(shapeRenderer.m_shape);
        
        //MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        //shapeRenderer.Render(meshFilter.sharedMesh, ShapeRenderer.RenderFaces.DOUBLE_SIDED, true); //render again the shape
    }
}