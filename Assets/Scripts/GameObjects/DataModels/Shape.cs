﻿using UnityEngine;
using System.Collections.Generic;

public class Shape : Triangulable
{
    public Color m_color { get; set; }

    public Shape() : base()
    {

    }

    public Shape(List<Vector2> contour, Color color) : base(contour)
    {
        m_color = color;
    }

    public Shape(List<Vector2> contour, List<GridTriangle> triangles, Color color) : base(contour, triangles)
    {
        m_color = color;
    }

    public override void Triangulate()
    {
        List<Vector2> triangles = new List<Vector2>();

        Triangulation.Process(m_contour, ref triangles);

        m_area = 0;
        for (int iVertexIndex = 0; iVertexIndex != triangles.Count; iVertexIndex += 3)
        {
            ShapeTriangle shapeTriangle = new ShapeTriangle(this);
            shapeTriangle.m_points[0] = triangles[iVertexIndex];
            shapeTriangle.m_points[1] = triangles[iVertexIndex + 1];
            shapeTriangle.m_points[2] = triangles[iVertexIndex + 2];

            m_gridTriangles.Add(shapeTriangle);
            m_area += shapeTriangle.GetArea();
        }
    }

    public List<ShapeTriangle> GetShapeTriangles()
    {
        List<ShapeTriangle> shapeTriangles = new List<ShapeTriangle>();
        shapeTriangles.Capacity = m_gridTriangles.Count;
        for (int i = 0; i != m_gridTriangles.Count; i++)
        {
            shapeTriangles.Add((ShapeTriangle) m_gridTriangles[i]);
        }

        return shapeTriangles;
    }

    public void SetShapeTriangles(List<ShapeTriangle> shapeTriangles)
    {
        m_gridTriangles.Clear();
        m_gridTriangles.Capacity = shapeTriangles.Count;
        for (int i = 0; i != shapeTriangles.Count; i++)
        {
            m_gridTriangles.Add(shapeTriangles[i]);
        }
    }

    /**
     * Fusion this shape with every shape that overlaps it. 
     * Every shape except 'this' is destroyed and their triangle are added to 'this' shape
     * **/
    public void Fusion()
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        ShapeRenderer[] shapeRenderers = gameScene.m_shapes.GetComponentsInChildren<ShapeRenderer>();

        //Find the shapes that overlaps this shape
        List<Shape> shapesToUnion = new List<Shape>();
        for (int iShapeIndex = 0; iShapeIndex != shapeRenderers.Length; iShapeIndex++)
        {
            Shape shapeData = shapeRenderers[iShapeIndex].m_shape;
            if (this.OverlapsShape(shapeData))
            {
                shapesToUnion.Add(shapeData);
            }
        }


        //Pass these shapes to the union clipper
        if (shapesToUnion.Count <= 1)
            return;

        Shape resultingShape = ClippingBooleanOperations.ShapesUnion(shapesToUnion);
        if (resultingShape != null)
        {
            gameScene.m_shapes.CreateShapeObjectFromData(resultingShape);

            //Destroy all previous shapes
            for (int iShapeIndex = 0; iShapeIndex != shapeRenderers.Length; iShapeIndex++)
            {
                GameObject shapeObject = shapeRenderers[iShapeIndex].gameObject;
                gameScene.m_shapes.DestroyShape(shapeObject);
                gameScene.m_shapes.RemoveShapeObject(shapeObject);
            }
        }
    }

    public bool IntersectsContour(Contour contour)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_gridTriangles[iTriangleIndex];
            if (triangle.IntersectsContour(contour))
                return true;
        }

        return false;
    }


    /**
     * Does 'this' shape overlaps another shape (i.e intersection is neither a point or null)
     * **/
    public bool OverlapsShape(Shape shape)
    {
        if (shape == this)
            return true;

        //Check if one of the triangle edges intersects triangle edges of the second shape
        for (int iTriangleIndex = 0; iTriangleIndex != m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_gridTriangles[iTriangleIndex];
            for (int iEdgeIndex = 0; iEdgeIndex != 3; iEdgeIndex++)
            {
                Vector2 triangleEdgePoint1 = triangle.m_points[iEdgeIndex];
                Vector2 triangleEdgePoint2 = (iEdgeIndex == 2) ? triangle.m_points[0] : triangle.m_points[iEdgeIndex + 1];

                //the shape intersects or overlaps the edge [triangleEdgePoint1; triangleEdgePoint2]
                if (shape.IntersectsEdge(triangleEdgePoint1, triangleEdgePoint2)
                    ||
                    shape.OverlapsEdge(triangleEdgePoint1, triangleEdgePoint2))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /**
     * Does 'this' shape intersects the edge passed as parameter
     * **/
    private bool IntersectsEdge(Vector2 edgePoint1, Vector2 edgePoint2)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_gridTriangles[iTriangleIndex];
            for (int iEdgeIndex = 0; iEdgeIndex != 3; iEdgeIndex++)
            {
                Vector2 triangleEdgePoint1 = triangle.m_points[iEdgeIndex];
                Vector2 triangleEdgePoint2 = (iEdgeIndex == 2) ? triangle.m_points[0] : triangle.m_points[iEdgeIndex + 1];

                if (GeometryUtils.TwoSegmentsIntersect(triangleEdgePoint1, triangleEdgePoint2, edgePoint1, edgePoint2))
                    return true;
            }
        }

        return false;
    }

    /**
     * Does 'this' shape overlaps the edge passed as parameter
     * **/
    private bool OverlapsEdge(Vector2 edgePoint1, Vector2 edgePoint2)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_gridTriangles[iTriangleIndex];
            for (int iEdgeIndex = 0; iEdgeIndex != 3; iEdgeIndex++)
            {
                Vector2 triangleEdgePoint1 = triangle.m_points[iEdgeIndex];
                Vector2 triangleEdgePoint2 = (iEdgeIndex == 2) ? triangle.m_points[0] : triangle.m_points[iEdgeIndex + 1];

                if (GeometryUtils.TwoSegmentsOverlap(triangleEdgePoint1, triangleEdgePoint2, edgePoint1, edgePoint2))
                    return true;
            }
        }

        return false;
    }
}