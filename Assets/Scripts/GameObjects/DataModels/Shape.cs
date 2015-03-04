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

    public Shape(List<Vector2> contour, List<GridTriangle> triangles, Color color) : base(contour, triangles)
    {
        m_color = color;
    }

    /**
     * Fusion this shape with every shape that overlaps it. 
     * Every shape except 'this' is destroyed and their triangle are added to 'this' shape
     * **/
    public void Fusion()
    {
        GameObject shapesObject = GameObject.FindGameObjectWithTag("Shapes");
        ShapesHolder shapesHolder = shapesObject.GetComponent<ShapesHolder>();
        ShapeRenderer[] shapeRenderers = shapesHolder.GetComponentsInChildren<ShapeRenderer>();

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
            ShapeBuilder shapeBuilder = shapesObject.GetComponent<ShapeBuilder>();
            shapeBuilder.CreateFromShapeData(resultingShape);

            //Destroy all previous shapes
            for (int iShapeIndex = 0; iShapeIndex != shapeRenderers.Length; iShapeIndex++)
            {
                shapesHolder.DestroyShape(shapeRenderers[iShapeIndex].gameObject);
            }
        }


        //GameObject shapesObject = GameObject.FindGameObjectWithTag("Shapes");
        //ShapesHolder shapesHolder = shapesObject.GetComponent<ShapesHolder>();
        //ShapeBuilder shapeBuilder = shapesObject.GetComponent<ShapeBuilder>();
        //ShapeRenderer[] allShapes = shapesHolder.GetComponentsInChildren<ShapeRenderer>();

        //for (int iShapeIndex = 0; iShapeIndex != allShapes.Length; iShapeIndex++)
        //{
        //    Shape shapeData = allShapes[iShapeIndex].m_shape;
        //    if (shapeData == this)
        //        continue;

        //    if (this.OverlapsShape(shapeData))
        //    {
        //        this.m_gridTriangles.AddRange(shapeData.m_gridTriangles); //add triangles from second shape to the first one
        //        shapesHolder.DestroyShape(allShapes[iShapeIndex].gameObject); //destroy the second shape
        //    }
        //}

        //CalculateContour();
        //Triangulate();
        //CalculateArea(); //recalculate the area of this shape after triangles have been added to it
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