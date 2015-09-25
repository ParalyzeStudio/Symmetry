using UnityEngine;
using System.Collections.Generic;

public class ShapeAnimator : GameObjectAnimator
{
    public override void SetOpacity(float opacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(opacity, bPassOnChildren);

        ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
        shapeMesh.SetTintColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
        shapeMesh.SetTintColor(m_color);
    }

    //public override Vector3 GetGameObjectSize()
    //{
    //    Shape shapeData = this.gameObject.GetComponent<ShapeRenderer>().m_shape;
    //    List<GridTriangle> triangles = shapeData.m_gridTriangles;
    //    Vector2 minCoords = new Vector2(float.MaxValue, float.MaxValue);
    //    Vector2 maxCoords = new Vector2(float.MinValue, float.MinValue);
    //    for (int iTriangleIndex = 0; iTriangleIndex != triangles.Count; iTriangleIndex++)
    //    {
    //        GridTriangle triangle = triangles[iTriangleIndex];
    //        for (int i = 0; i != 3; i++)
    //        {
    //            if (triangle.m_points[i].x < minCoords.x)
    //            {
    //                minCoords.x = triangle.m_points[i].x;
    //            }
    //            if (triangle.m_points[i].x > maxCoords.x)
    //            {
    //                maxCoords.x = triangle.m_points[i].x;
    //            }

    //            if (triangle.m_points[i].y < minCoords.y)
    //            {
    //                minCoords.y = triangle.m_points[i].y;
    //            }
    //            if (triangle.m_points[i].y > maxCoords.y)
    //            {
    //                maxCoords.y = triangle.m_points[i].y;
    //            }
    //        }
    //    }

    //    GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
    //    Vector2 objectSize = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(maxCoords) - gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(minCoords);
    //    return GeometryUtils.BuildVector3FromVector2(objectSize, 1);
    //}

    public override void OnFinishRotating()
    {
        //shapeRenderer.m_shape.Fusion();
        //ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
        //Shapes.PerformFusionOnShape(shapeMesh.m_shape);
        
        //MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        //shapeRenderer.Render(meshFilter.sharedMesh, ShapeRenderer.RenderFaces.DOUBLE_SIDED, true); //render again the shape
    }
}