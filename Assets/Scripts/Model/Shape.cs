using UnityEngine;
using System.Collections.Generic;

public class Shape : GridTriangulable
{
    public Color m_color { get; set; }
    public Vector2 m_offsetOnVertices { get; set; }
    public Vector2 m_gridOffsetOnVertices { get; set; }
    public ShapeMesh m_parentMesh { get; set; }

    public enum ShapeState
    {
        NONE = 0,
        MARKED_TO_BE_DESTROYED,
        DYNAMIC_INTERSECTION, //this dynamic shape is the result of the intersection of two shapes
        DYNAMIC_DIFFERENCE, //this dynamic shape is the result of the difference between two shapes
        STATIC
    }
    public ShapeState m_state { get; set; } //is the shape static or dynamic (i.e rendered with animation)

    public Shape m_overlappedStaticShape { get; set; } //when the state of this shape is DYNAMIC_INTERSECTION, we store the shape that is clipped with this one
    private List<Shape> m_overlappedStaticShapeDiffShapes; //use a global member to store the result of the difference between this shape and overlapped static shape through threading

    public Shape(bool gridPointMode)
        : base(gridPointMode)
    {
        m_offsetOnVertices = Vector2.zero;
        m_gridOffsetOnVertices = Vector2.zero;
        m_state = ShapeState.NONE;
        m_parentMesh = null;
    }

    public Shape(bool gridPointMode, Contour contour)
        : base(gridPointMode, contour)
    {
        m_offsetOnVertices = Vector2.zero;
        m_gridOffsetOnVertices = Vector2.zero;
        m_state = ShapeState.NONE;
        m_parentMesh = null;
    }

    public Shape(bool gridPointMode, Contour contour, List<Contour> holes)
        : base(gridPointMode, contour, holes)
    {
        m_offsetOnVertices = Vector2.zero;
        m_gridOffsetOnVertices = Vector2.zero;
        m_state = ShapeState.NONE;
        m_parentMesh = null;
    }

    public Shape(Shape other)
        : base(other)
    {
        m_color = other.m_color;
        m_offsetOnVertices = other.m_offsetOnVertices;
        m_gridOffsetOnVertices = other.m_gridOffsetOnVertices;
        m_state = other.m_state;
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

        Shape symmetricShape = new Shape(m_gridPointMode, symmetricContour, symmetricHoles);
        symmetricShape.m_color = this.m_color;
        return symmetricShape;
    }

    /**
     * Set the color of each child triangle from the color of the shape itself
     * **/
    //public void PropagateColorToTriangles()
    //{
    //    for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
    //    {
    //        ShapeTriangle triangle = (ShapeTriangle)m_triangles[iTriangleIndex];
    //        triangle.m_color = m_color;
    //    }
    //}

    /**
     * Obtain the color of this shape from the triangles
     * **/
    //public void ObtainColorFromTriangles()
    //{
    //    if (m_triangles.Count > 0)
    //        m_color = ((ShapeTriangle)m_triangles[0]).m_color;
    //}

    /**
     * Fusion this shape with every static shape that overlaps it. 
     * A new static shape is created if a fusion occured and old fusioned shapes are destroyed
     * Else null is returned
     * **/
    public bool Fusion()
    {
        List<Shape> shapes = m_parentMesh.GetShapesHolder().m_shapes;

        Shape subjShape = this;
        Shape clipShape = null;

        //Find the first static shape that overlaps this shape
        for (int iShapeIndex = 0; iShapeIndex != shapes.Count; iShapeIndex++)
        {
            Shape shapeData = shapes[iShapeIndex];

            if (shapeData.m_state != ShapeState.STATIC) //check only static shapes
                continue;

            if (!shapeData.m_color.Equals(this.m_color)) //dismiss shapes that are of different color than 'this' shape
                continue;

            if (clipShape == null && this.OverlapsShape(shapeData, false)) //we have not found a clip shape yet
                clipShape = shapeData;

            if (clipShape != null && subjShape != null) //we have both subj and clip shape we can break the loop
                break;
        }

        if (clipShape == null) //no shape overlaps 'this' shape
            return false;

        List<Shape> resultingShapes = m_parentMesh.GetClippingManager().ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
        if (resultingShapes.Count == 1)
        {             
            Shape fusionedShape = resultingShapes[0];
            this.m_contour = fusionedShape.m_contour;
            this.m_holes = fusionedShape.m_holes;
            clipShape.m_state = ShapeState.MARKED_TO_BE_DESTROYED;
            return true;
        }

        return false;
    }

    /**
     * Check if this shape intersects stricly one of the contours of this shape (main contour and holes)
     * **/
    public bool IntersectsOutline(DottedOutline outline)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            BaseTriangle triangle = m_triangles[iTriangleIndex];
            if (triangle.IntersectsContour(outline, GeometryUtils.SEGMENTS_STRICT_INTERSECTION))
                return true;
        }

        return false;
    }

    /**
     * Does 'this' shape is included inside one triangle of the shape passed as parameter
     * (i.e all triangles of the first shape are inside one of the second shape triangles)
     * **/
    //public bool isIncludedInOneShapeTriangle(Shape shape)
    //{
    //    for (int iTriangleIndex = 0; iTriangleIndex != shape.m_triangles.Count; iTriangleIndex++)
    //    {
    //        BaseTriangle triangle = shape.m_triangles[iTriangleIndex];

    //        if (triangle.ContainsShape(this)) //'this' shape is contained entirely inside one of the shape triangles
    //            return true;
    //    }

    //    return false;
    //}

    /**
     * Does 'this' shape overlaps another shape (i.e intersection is neither a point or null)
     * We can set an offset (in grid coordiantes) to 'this' shape vertices when the actual shape is being translated
     * **/
    //public bool OverlapsShape(Shape shape)
    //{
    //    if (this.m_triangles.Count == 0 || shape.m_triangles.Count == 0)
    //        throw new System.Exception("One (or both) of the shapes is not triangulated");

    //    if (shape == this)
    //    {
    //        return true;
    //    }

    //    //check if 'this' shape is inluded in one of the shape triangles and vice versa
    //    if (this.isIncludedInOneShapeTriangle(shape) || shape.isIncludedInOneShapeTriangle(this))
    //    {
    //        return true;
    //    }

    //    //Check if one of the triangle edges intersects triangle edges of the second shape
    //    for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
    //    {
    //        BaseTriangle triangle = m_triangles[iTriangleIndex];
    //        for (int iEdgeIndex = 0; iEdgeIndex != 3; iEdgeIndex++)
    //        {
    //            Vector2 triangleEdgePoint1 = triangle.m_points[iEdgeIndex] + m_gridOffsetOnVertices;
    //            Vector2 triangleEdgePoint2 = (iEdgeIndex == 2) ? triangle.m_points[0] : triangle.m_points[iEdgeIndex + 1];
    //            triangleEdgePoint2 += m_gridOffsetOnVertices;

    //            //the shape intersects or overlaps the edge [triangleEdgePoint1; triangleEdgePoint2]
    //            if (shape.IntersectsEdge(triangleEdgePoint1, triangleEdgePoint2)
    //                ||
    //                shape.OverlapsEdge(triangleEdgePoint1, triangleEdgePoint2))
    //            {
    //                //Debug.Log("point1 X:" + triangleEdgePoint1.x + " Y:" + triangleEdgePoint1.y);
    //                //Debug.Log("point2 X:" + triangleEdgePoint2.x + " Y:" + triangleEdgePoint2.y);
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}

    /**
     * Does 'this' shape intersects the edge passed as parameter
     * **/
    //private bool IntersectsEdge(Vector2 edgePoint1, Vector2 edgePoint2)
    //{
    //    for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
    //    {
    //        BaseTriangle triangle = m_triangles[iTriangleIndex];
    //        for (int iEdgeIndex = 0; iEdgeIndex != 3; iEdgeIndex++)
    //        {
    //            Vector2 triangleEdgePoint1 = triangle.m_points[iEdgeIndex];
    //            Vector2 triangleEdgePoint2 = (iEdgeIndex == 2) ? triangle.m_points[0] : triangle.m_points[iEdgeIndex + 1];

    //            if (GeometryUtils.TwoSegmentsIntersect(triangleEdgePoint1, triangleEdgePoint2, edgePoint1, edgePoint2))
    //                return true;
    //        }
    //    }

    //    return false;
    //}

    /**
     * Does 'this' shape overlaps (i.e both edges are collinear and share a portion of segment) the edge passed as parameter
     * **/
    //private bool OverlapsEdge(Vector2 edgePoint1, Vector2 edgePoint2)
    //{
    //    for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
    //    {
    //        BaseTriangle triangle = m_triangles[iTriangleIndex];
    //        for (int iEdgeIndex = 0; iEdgeIndex != 3; iEdgeIndex++)
    //        {
    //            Vector2 triangleEdgePoint1 = triangle.m_points[iEdgeIndex];
    //            Vector2 triangleEdgePoint2 = (iEdgeIndex == 2) ? triangle.m_points[0] : triangle.m_points[iEdgeIndex + 1];

    //            if (GeometryUtils.TwoSegmentsOverlap(triangleEdgePoint1, triangleEdgePoint2, edgePoint1, edgePoint2))
    //                return true;
    //        }
    //    }

    //    return false;
    //}

    /**
    * Does 'this' shape overlaps the shape passed as parameter
    * We set a boolean to decide if we accept empty intersections (points and portions of contour edges in common exclusively)
    * **/
    public bool OverlapsShape(Shape shape, bool bEnsureNonNullIntersection)
    {
        for (int i = 0; i != shape.m_triangles.Count; i++)
        {
            if (OverlapsTriangle(shape.m_triangles[i], bEnsureNonNullIntersection))
                return true;
        }

        return false;
    }

    /**
    * Does 'this' shape overlaps the triangle passed as parameter
     * We set a boolean to decide if we accept empty intersections (points and portions of contour edges in common exclusively)
    * **/
    public bool OverlapsTriangle(BaseTriangle triangle, bool bEnsureNonNullIntersection)
    {
        for (int i = 0; i != m_triangles.Count; i++)
        {
            if (bEnsureNonNullIntersection)
            {
                if (m_triangles[i].IntersectsTriangleWithNonNullIntersection(triangle))
                    return true;
            }
            else
            {
                if (m_triangles[i].IntersectsTriangle(triangle))
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

    /**
     * Calls recursively Shape.Fusion() on this shape and then on the shape resulting from previous fusion
     * This way we are sure that the initial shape is fusionned to every shape that overlapped it at the beginning
     * **/
    public bool PerformFusion()
    {
        bool bFusionOccured = false;
        bool bFusioning = false;
        do
        {
            bFusioning = Fusion();
            if (bFusioning)
                bFusionOccured = true;
        }
        while (bFusioning);

        return bFusionOccured;
    }

    /**
     * Calculate the difference between the shape that we found overlapping 'this' shape and 'this' shape
     * **/
    public void PerformDifferenceOnOverlappedStaticShape()
    {
        m_overlappedStaticShapeDiffShapes = m_parentMesh.GetGameScene().GetClippingManager().ShapesOperation(m_overlappedStaticShape, this, ClipperLib.ClipType.ctDifference);        
    }

    /**
     * Called when thread has finished calculating the difference shapes on the current overlapped static shape
     * **/
    private void OnFinishDifferenceOnOverlappedStaticShape()
    {
        for (int i = 0; i != m_overlappedStaticShapeDiffShapes.Count; i++)
        {
            m_overlappedStaticShapeDiffShapes[i].m_state = Shape.ShapeState.STATIC;
            m_parentMesh.GetShapesHolder().CreateShapeObjectFromData(m_overlappedStaticShapeDiffShapes[i], false);
        }

        //Destroy the old overlapped static shape
        m_overlappedStaticShape.m_parentMesh.GetComponent<ShapeAnimator>().SetOpacity(0);//Set zero opacity on this shape during the time it is waiting for its destruction
        m_parentMesh.GetShapesHolder().DestroyShapeObjectForShape(m_overlappedStaticShape, 0.5f); //add a delay because Destroy occurs before rendering so the mesh can be destroyed before the new one is actually rendered which leads to a graphical glitch
        m_parentMesh.GetShapesHolder().m_shapes.Remove(m_overlappedStaticShape);
        m_overlappedStaticShape = null;
    }
}