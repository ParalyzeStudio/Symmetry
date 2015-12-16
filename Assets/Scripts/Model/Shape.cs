﻿using UnityEngine;
using System.Collections.Generic;

public class Shape : GridTriangulable
{
    public Color m_color { get; set; }
    public Vector2 m_offset { get; set; }
    public GridPoint m_gridOffset { get; set; }
    public ShapeMesh m_parentMesh { get; set; }

    public enum ShapeState
    {
        NONE = 0,
        DYNAMIC_INTERSECTION, //this dynamic shape is the result of the intersection of two shapes
        DYNAMIC_DIFFERENCE, //this dynamic shape is the result of the difference between two shapes
        STATIC, //this shape has been drawn
        MOVING_ORIGINAL_SHAPE, //the original shape being dragged on the grid by the player
        MOVING_SUBSTITUTION_INTERSECTION, //a shape that is the result of an intersection clipping operation between moving shape and static shapes
        MOVING_SUBSTITUTION_DIFFERENCE, //a shape that is the result of a difference clipping operation between moving shape and static shapes
        BUSY_CLIPPING, //the shape is being clipped (using threads) and must not be considered as dynamic anymore and it is not static yet
        DESTROYED
    }
    public ShapeState m_state { get; set; } //is the shape static or dynamic (i.e rendered with animation)

    public Shape m_overlappedStaticShape { get; set; } //when the state of this shape is DYNAMIC_INTERSECTION, we store the shape that is clipped with this one
    public List<Shape> m_shapesToCreate { get; set; } //the list to store shapes from the difference clipping operation with the overlapped static shape
    public HashSet<Shape> m_shapesToDelete { get; set; } //Set of shapes to delete once the difference/fusion occured
    private List<Shape> m_substitutionShapes; //List of shapes that are drawn over the shape that is being moved by the player when clipping occurs

    public Shape()
    {
        m_offset = Vector2.zero;
        m_gridOffset = GridPoint.zero;
        m_state = ShapeState.NONE;
        m_parentMesh = null;
    }

    public Shape(Contour contour)
        : base(contour)
    {
        m_offset = Vector2.zero;
        m_gridOffset = GridPoint.zero;
        m_state = ShapeState.NONE;
        m_parentMesh = null;
    }

    public Shape(Contour contour, List<Contour> holes)
        : base(contour, holes)
    {
        m_offset = Vector2.zero;
        m_gridOffset = GridPoint.zero;
        m_state = ShapeState.NONE;
        m_parentMesh = null;
    }

    public Shape(Shape other)
        : base(other)
    {
        m_color = other.m_color;
        m_offset = other.m_offset;
        m_gridOffset = other.m_gridOffset;
        m_state = other.m_state;
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

            if (shapeData == this) //do not fusion with itself
                continue;

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

            m_shapesToDelete.Add(clipShape); //add the clip shape to the set of shapes to be deleted
            clipShape.m_state = ShapeState.DESTROYED;

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
            GridTriangle triangle = m_triangles[iTriangleIndex];
            if (triangle.IntersectsOutline(outline, GeometryUtils.SEGMENTS_STRICT_INTERSECTION))
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
    public bool OverlapsTriangle(GridTriangle triangle, bool bEnsureNonNullIntersection)
    {
        for (int i = 0; i != m_triangles.Count; i++)
        {
            if (m_triangles[i].IntersectsTriangle(triangle, bEnsureNonNullIntersection))
                return true;
            //if (bEnsureNonNullIntersection)
            //{
            //    bool bIntersect = m_triangles[i].IntersectsTriangleWithNonNullIntersection(triangle);
            //    if (m_triangles[i].IntersectsTriangleWithNonNullIntersection(triangle))
            //        return true;
            //}
            //else
            //{
            //    if (m_triangles[i].IntersectsTriangle(triangle))
            //        return true;
            //}
        }

        return false;
    }

    /**
     * Translate this shape (contour and holes) by a certain vector
     * **/
    public void Translate(GridPoint translation)
    {
        //Contour
        m_contour.Translate(translation);

        //holes
        for (int iHolesIdx = 0; iHolesIdx != m_holes.Count; iHolesIdx++)
        {
            m_holes[iHolesIdx].Translate(translation);
        }

        //triangles
        for (int iTriangleIdx = 0; iTriangleIdx != m_triangles.Count; iTriangleIdx++)
        {
            m_triangles[iTriangleIdx].Translate(translation);
        }
    }

    /**
     * Create the data structures that will hold temporary created/deleted shapes
     * **/
    public void InitTemporaryStoredShapes()
    {
        m_shapesToDelete = new HashSet<Shape>();
        m_shapesToCreate = new List<Shape>();
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
        m_shapesToCreate.AddRange(m_parentMesh.GetGameScene().GetClippingManager().ShapesOperation(m_overlappedStaticShape, this, ClipperLib.ClipType.ctDifference));

        //Destroy the old overlapped static shape
        m_shapesToDelete.Add(m_overlappedStaticShape);
        m_overlappedStaticShape.m_state = ShapeState.DESTROYED;
        m_overlappedStaticShape = null;
    }

    /**
     * Perform fusion between this shape and all static shapes that overlap it
     * **/
    private void PerformFusionWithStaticShapes()
    {
        //Fusion the shape
        bool bFusionOccured = PerformFusion();
        if (bFusionOccured)
            Triangulate();
    }

    /**
     * Build the extracted shapes from the difference clipping operation and make this shape static
     * **/
    private void OnFinishPerformingDifferenceWithOverlappedStaticShape()
    {
        //build the shape objects from the 'difference' operation
        for (int i = 0; i != m_shapesToCreate.Count; i++)
        {
            Shape shape = m_shapesToCreate[i];
            shape.m_state = Shape.ShapeState.STATIC;
            m_parentMesh.GetShapesHolder().CreateShapeObjectFromData(shape, false);
        }
    }

    /**
     * Re-render this shape that grew if other static shapes fusion with it or stayed the same if no fusion occured
     * **/
    private void OnFinishPerformingFusionWithStaticShapes()
    {
        //Destroy cells array and re-render the shape mesh + make it static
        if (this.IsDynamic())
            m_parentMesh.DestroyCells();
        m_parentMesh.Render(false);
        m_state = Shape.ShapeState.STATIC;

        //delete the shapes that we store in the fusion process
        m_parentMesh.GetShapesHolder().DeleteShapesSet(m_shapesToDelete);
    }

    /**
     * This method is called when player moves a shape over the grid
     * We recalculate here the intersection and difference shapes that are clipped against the static shapes
     * **/
    public void InvalidateSubstitutionShapesOnMove()
    {
        //Copy the shape before clipping it
        Shape shapeCopy = new Shape(this);

        if (m_substitutionShapes == null)
            m_substitutionShapes = new List<Shape>();

        //destroy and clear the old substitution shapes
        Shapes shapesHolder = m_parentMesh.GetShapesHolder();
        for (int i = 0; i != m_substitutionShapes.Count; i++)
        {
            shapesHolder.m_shapes.Remove(m_substitutionShapes[i]);
            shapesHolder.DestroyShapeObjectForShape(m_substitutionShapes[i]);
        }
        m_substitutionShapes.Clear();

        //build the new ones
        ClippingManager clippingManager = m_parentMesh.GetClippingManager();
        List<Shape> clippedInterShapes, clippedDiffShapes;
        clippingManager.ClipAgainstStaticShapes(shapeCopy, out clippedInterShapes, out clippedDiffShapes, false);

        for (int i = 0; i != clippedInterShapes.Count; i++)
        {
            Shape interShape = clippedInterShapes[i];
            GameObject interShapeObject = shapesHolder.CreateShapeObjectFromData(interShape, false);
            GameObjectAnimator interShapeObjectAnimator = interShapeObject.GetComponent<GameObjectAnimator>();
            interShapeObjectAnimator.SetPosition(new Vector3(0, 0, -1));
            m_substitutionShapes.Add(interShape);
        }

        for (int i = 0; i != clippedDiffShapes.Count; i++)
        {
            Shape diffShape = clippedDiffShapes[i];
            GameObject diffShapeObject = shapesHolder.CreateShapeObjectFromData(diffShape, false);
            GameObjectAnimator diffShapeObjectAnimator = diffShapeObject.GetComponent<GameObjectAnimator>();
            diffShapeObjectAnimator.SetPosition(new Vector3(0, 0, -1));
            m_substitutionShapes.Add(diffShape);
        }

        //Debug.Log("INTER ShapesCount:" + clippedInterShapes.Count);
        //Debug.Log("DIFF ShapesCount:" + clippedDiffShapes.Count);
    }

    /**
     * When shape is about to become static, perform the last clipping operations:
     * -difference on overlapped static shape if any
     * -fusion with all static shapes
     * This job is threaded when a symmetry is performed becaused it can make the UI lag and this can be noticed by the user
     * On the other hand when we finalize clipping after releasing shape from drag we do not do it as it is hard to detect any lag
     * **/
    public void FinalizeClippingOperations()
    {
        InitTemporaryStoredShapes();

        if (IsDynamic())
        {
            List<Shape> shapes = m_parentMesh.GetShapesHolder().m_shapes;
            //ensure that FindGameObjectWithTag is not called inside the thread to go by setting relevant global instances in parent classes
            EnsureUnityInstancesAreSetBeforeThreading();

            //Perform the difference clipping operation            
            if (m_state == Shape.ShapeState.DYNAMIC_INTERSECTION)
            {
                m_parentMesh.GetGameScene().GetQueuedThreadedJobsManager().AddJob
                    (
                    new ThreadedJob
                        (
                        new ThreadedJob.ThreadFunction(PerformDifferenceOnOverlappedStaticShape),
                        null,
                        new ThreadedJob.ThreadFunction(OnFinishPerformingDifferenceWithOverlappedStaticShape)
                        )
                    );
            }

            //make the shape busy before the fusion occurs
            m_state = Shape.ShapeState.BUSY_CLIPPING;

            //Perform the fusion
            m_parentMesh.GetGameScene().GetQueuedThreadedJobsManager().AddJob
                (
                new ThreadedJob
                    (
                    new ThreadedJob.ThreadFunction(PerformFusionWithStaticShapes),
                    null,
                    new ThreadedJob.ThreadFunction(OnFinishPerformingFusionWithStaticShapes)
                    )
                );
        }
        else if (IsSubstitutionShape())
        {
            if (m_state == Shape.ShapeState.MOVING_SUBSTITUTION_INTERSECTION)
            {
                PerformDifferenceOnOverlappedStaticShape();
                OnFinishPerformingDifferenceWithOverlappedStaticShape();
            }
            PerformFusionWithStaticShapes();
            OnFinishPerformingFusionWithStaticShapes();
        }
    }

    /**
     * Call this method when player has finished dragging a shape
     * **/
    public void FinalizeClippingOperationsOnSubstitutionShapes()
    {
        for (int i = 0; i != m_substitutionShapes.Count; i++)
        {
            m_substitutionShapes[i].FinalizeClippingOperations();
        }

        //destroy this shape object
        Shapes shapesHolder = this.m_parentMesh.GetShapesHolder();
        shapesHolder.m_shapes.Remove(this);
        shapesHolder.DestroyShapeObjectForShape(this);
    }

    /***
     * We need to set instances before threading because the Unity API is not thread-safe and every call to 
        GameObject.FindGameObjectWithTag (or else) must be prevented
     * **/
    public void EnsureUnityInstancesAreSetBeforeThreading()
    {
        m_parentMesh.GetClippingManager();
        m_parentMesh.GetShapesHolder();
    }

    /**
     * Simply tells if this shape is dynamic 
     * **/
    public bool IsDynamic()
    {
        return m_state == ShapeState.DYNAMIC_DIFFERENCE || m_state == ShapeState.DYNAMIC_INTERSECTION;
    }

    /**
     * Simply tells if this shape is currently a substitution shape 
     * **/
    public bool IsSubstitutionShape()
    {
        return m_state == ShapeState.MOVING_SUBSTITUTION_INTERSECTION || m_state == ShapeState.MOVING_SUBSTITUTION_DIFFERENCE;
    }
}