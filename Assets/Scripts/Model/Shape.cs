using UnityEngine;
using System.Collections.Generic;

public class Shape : GridTriangulable
{
    public Color m_color { get; set; }
    public Vector2 m_offsetOnVertices { get; set; }
    public Vector2 m_gridOffsetOnVertices { get; set; }

    public Shape(bool gridPointMode)
        : base(gridPointMode)
    {
        m_offsetOnVertices = Vector2.zero;
        m_gridOffsetOnVertices = Vector2.zero;
    }

    public Shape(bool gridPointMode, Contour contour)
        : base(gridPointMode, contour)
    {
        m_offsetOnVertices = Vector2.zero;
        m_gridOffsetOnVertices = Vector2.zero;
    }

    public Shape(bool gridPointMode, Contour contour, List<Contour> holes)
        : base(gridPointMode, contour, holes)
    {
        m_offsetOnVertices = Vector2.zero;
        m_gridOffsetOnVertices = Vector2.zero;
    }

    public Shape(Shape other)
        : base(other)
    {
        m_color = other.m_color;
        m_offsetOnVertices = other.m_offsetOnVertices;
        m_gridOffsetOnVertices = other.m_gridOffsetOnVertices;
    }

    public override void Triangulate()
    {
        m_triangles.Clear(); //clear any previous triangles from a previous triangulation

        Vector2[] triangles = Triangulation.P2tTriangulate(this);

        for (int iVertexIndex = 0; iVertexIndex != triangles.Length; iVertexIndex += 3)
        {
            ShapeTriangle shapeTriangle = new ShapeTriangle(this);
            shapeTriangle.m_points[0] = triangles[iVertexIndex];
            shapeTriangle.m_points[1] = triangles[iVertexIndex + 1];
            shapeTriangle.m_points[2] = triangles[iVertexIndex + 2];

            m_triangles.Add(shapeTriangle);
            m_area += shapeTriangle.GetArea();
        }
    }

    public List<ShapeTriangle> GetShapeTriangles()
    {
        List<ShapeTriangle> shapeTriangles = new List<ShapeTriangle>();
        shapeTriangles.Capacity = m_triangles.Count;
        for (int i = 0; i != m_triangles.Count; i++)
        {
            shapeTriangles.Add((ShapeTriangle)m_triangles[i]);
        }

        return shapeTriangles;
    }

    public void SetShapeTriangles(List<ShapeTriangle> shapeTriangles)
    {
        m_triangles.Clear();
        m_triangles.Capacity = shapeTriangles.Count;
        for (int i = 0; i != shapeTriangles.Count; i++)
        {
            m_triangles.Add(shapeTriangles[i]);
        }
    }

    /**
     * Return a Shape object that is symmetric about the 'axis' parameter
     * **/
    public Shape CalculateSymmetricShape(AxisRenderer axis)
    {
        //Symmetrize contour
        Contour symmetricContour = new Contour(m_contour.Count);
    
        for (int i = m_contour.Count - 1; i != -1; i--)
        {
            symmetricContour.Add(Symmetrizer.GetSymmetricPointAboutAxis(m_contour[i], axis));
        }

        //Symmetrize holes
        List<Contour> symmetricHoles = new List<Contour>(m_holes.Count);
        for (int i = 0; i != m_holes.Count; i++)
        {
            Contour hole = m_holes[i];
            Contour symmetricHole = new Contour(hole.Count);
            for (int j = hole.Count - 1; j != -1; j--)
            {
                symmetricHole.Add(Symmetrizer.GetSymmetricPointAboutAxis(hole[j], axis));
            }
            symmetricHoles.Add(symmetricHole);
        }

        return new Shape(m_gridPointMode, symmetricContour, symmetricHoles);
    }

    /**
     * Set the color of each child triangle from the color of the shape itself
     * **/
    public void PropagateColorToTriangles()
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            ShapeTriangle triangle = (ShapeTriangle)m_triangles[iTriangleIndex];
            triangle.m_color = m_color;
        }
    }

    /**
     * Obtain the color of this shape from the triangles
     * **/
    public void ObtainColorFromTriangles()
    {
        if (m_triangles.Count > 0)
            m_color = ((ShapeTriangle)m_triangles[0]).m_color;
    }

    /**
     * Fusion this shape with every shape that overlaps it. 
     * Every shape except 'this' is destroyed and their triangle are added to 'this' shape
     * Returns the shape resulting from the fusion or null if no fusion occured
     * **/
    public Shape Fusion()
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
        List<GameObject> shapeObjects = gameScene.m_shapes.m_shapesObjects;

        //Find the first shape that overlaps this shape
        Shape subjShape = null;
        Shape clipShape = null;
        GameObject subjShapeObject = null;
        GameObject clipShapeObject = null;

        for (int iShapeIndex = 0; iShapeIndex != shapeObjects.Count; iShapeIndex++)
        {
            Shape shapeData = shapeObjects[iShapeIndex].GetComponent<ShapeMesh>().m_shapeData;

            if (!shapeData.m_color.Equals(this.m_color)) //dismiss shapes that are of different color than 'this' shape
                continue;

            if (this == shapeData)
            {
                subjShape = this;
                subjShapeObject = shapeObjects[iShapeIndex];
            }
            else if (clipShape == null && this.OverlapsShape(shapeData)) //we have not found a clip shape yet
            {
                clipShape = shapeData;
                clipShapeObject = shapeObjects[iShapeIndex];                
            }

            if (clipShape != null && subjShape != null) //we have both subj and clip shape we can break the loop
                break;
        }

        if (clipShape == null) //no shape overlaps 'this' shape
            return null;

        List<Shape> resultingShapes = ClippingBooleanOperations.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
        if (resultingShapes.Count > 1)
        {
            Shape resultingShape = resultingShapes[0];
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
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = m_triangles[iTriangleIndex];
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
        for (int iTriangleIndex = 0; iTriangleIndex != shape.m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = shape.m_triangles[iTriangleIndex];

            if (triangle.ContainsShape(this)) //'this' shape is contained entirely inside one of the shape triangles
                return true;
        }

        return false;
    }

    /**
     * Does 'this' shape overlaps another shape (i.e intersection is neither a point or null)
     * We can set an offset (in grid coordiantes) to 'this' shape vertices when the actual shape is being translated
     * **/
    public bool OverlapsShape(Shape shape)
    {
        if (shape == this)
        {
            return true;
        }

        //check if 'this' shape is inluded in one of the shape triangles and vice versa
        if (this.isIncludedInOneShapeTriangle(shape) || shape.isIncludedInOneShapeTriangle(this))
        {
            return true;
        }

        //Check if one of the triangle edges intersects triangle edges of the second shape
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = m_triangles[iTriangleIndex];
            for (int iEdgeIndex = 0; iEdgeIndex != 3; iEdgeIndex++)
            {
                Vector2 triangleEdgePoint1 = triangle.m_points[iEdgeIndex] + m_gridOffsetOnVertices;
                Vector2 triangleEdgePoint2 = (iEdgeIndex == 2) ? triangle.m_points[0] : triangle.m_points[iEdgeIndex + 1];
                triangleEdgePoint2 += m_gridOffsetOnVertices;

                //the shape intersects or overlaps the edge [triangleEdgePoint1; triangleEdgePoint2]
                if (shape.IntersectsEdge(triangleEdgePoint1, triangleEdgePoint2)
                    ||
                    shape.OverlapsEdge(triangleEdgePoint1, triangleEdgePoint2))
                {
                    //Debug.Log("point1 X:" + triangleEdgePoint1.x + " Y:" + triangleEdgePoint1.y);
                    //Debug.Log("point2 X:" + triangleEdgePoint2.x + " Y:" + triangleEdgePoint2.y);
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
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = m_triangles[iTriangleIndex];
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
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = m_triangles[iTriangleIndex];
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

    public Contour GetContourWithOffset()
    {
        if (m_gridOffsetOnVertices == Vector2.zero)
            return m_contour;

        Contour contourWithOffset = new Contour(m_contour);
        for (int i = 0; i != contourWithOffset.Count; i++)
        {
            contourWithOffset[i] += m_gridOffsetOnVertices;
        }

        return contourWithOffset;
    }

    public List<Contour> GetHolesWithOffset()
    {
        if (m_gridOffsetOnVertices == Vector2.zero)
            return m_holes;

        List<Contour> holesWithOffset = new List<Contour>(m_holes);

        for (int i = 0; i != m_holes.Count; i++)
        {
            Contour holesVec = m_holes[i];
            for (int j = 0; j != holesVec.Count; j++)
            {
                holesVec[j] += m_gridOffsetOnVertices;
            }
        }

        return holesWithOffset;
    }
}