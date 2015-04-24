using UnityEngine;
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

    public Shape(List<Vector2> contour, List<List<Vector2>> holes, Color color)
        : base(contour, holes)
    {
        m_color = color;
    }

    public Shape(Shape other)
        : base(other)
    {
        m_color = other.m_color; //Color(Vector4) is a value type so no need to clone them deeply
    }

    public override void Triangulate()
    {
        Vector2[] triangles = Triangulation.P2tTriangulate(this);

        for (int iVertexIndex = 0; iVertexIndex != triangles.Length; iVertexIndex += 3)
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
     * Returns the shape resulting from the fusion or null if no fusion occured
     * **/
    public Shape Fusion()
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        List<GameObject> shapeObjects = gameScene.m_shapes.m_shapesObj;

        //Find the first shape that overlaps this shape
        Shape subjShape = null;
        Shape clipShape = null;
        GameObject subjShapeObject = null;
        GameObject clipShapeObject = null;

        for (int iShapeIndex = 0; iShapeIndex != shapeObjects.Count; iShapeIndex++)
        {
            Shape shapeData = shapeObjects[iShapeIndex].GetComponent<ShapeRenderer>().m_shape;
            if (this == shapeData)
            {
                subjShape = this;
                subjShapeObject = shapeObjects[iShapeIndex];
            }
            else if (this.OverlapsShape(shapeData))
            {
                clipShape = shapeData;
                clipShapeObject = shapeObjects[iShapeIndex];
                break;
            }
        }

        if (clipShape == null) //no shape overlaps 'this' shape
            return null;

        Shape resultingShape = ClippingBooleanOperations.ShapesUnion(subjShape, clipShape);
        if (resultingShape != null)
        {
            gameScene.m_shapes.CreateShapeObjectFromData(resultingShape);

            gameScene.m_shapes.DestroyShape(subjShapeObject);
            gameScene.m_shapes.DestroyShape(clipShapeObject);
            gameScene.m_shapes.RemoveShapeObject(subjShapeObject);
            gameScene.m_shapes.RemoveShapeObject(clipShapeObject);

            return resultingShape;
        }

        return null;
    }

    public bool IntersectsOutline(DottedOutline contour)
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
     * Does 'this' shape is included inside one triangle of the shape passed as parameter
     * (i.e all triangles of the first shape are inside one of the second shape triangles)
     * **/
    public bool isIncludedInOneShapeTriangle(Shape shape)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != shape.m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = shape.m_gridTriangles[iTriangleIndex];

            if (triangle.ContainsShape(this)) //'this' shape is contained entirely inside one of the shape triangles
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

        //check if 'this' shape is inluded in one of the shape triangles and vice versa
        if (this.isIncludedInOneShapeTriangle(shape) || shape.isIncludedInOneShapeTriangle(this))
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