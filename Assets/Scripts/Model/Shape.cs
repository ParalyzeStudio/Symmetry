using UnityEngine;
using System.Collections.Generic;

public class Shape : GridTriangulable
{
    public const float DEFAULT_SATURATION = 1.0f;
    public const float DEFAULT_BRIGHTNESS = 7.5f;

    public Color m_color { get; set; }
    public float m_tint { get; set; } //as the tiled chalkboard texture is black we use tsb value instead of regular color to define the exact color of the shape
    public Vector2 m_offset { get; set; }
    public GridPoint m_gridOffset { get; set; }
    public ShapeMesh m_parentMesh { get; set; }

    public enum ShapeState
    {
        NONE = 0, //default state, probably never used
        TILED_BACKGROUND, //used for the tiled background dark shape
        DYNAMIC_INTERSECTION, //this dynamic shape is the result of the intersection of two shapes
        DYNAMIC_DIFFERENCE, //this dynamic shape is the result of the difference between two shapes
        STATIC, //this shape has been drawn
        STATIC_OVERLAPPED, //this shape is static but still overlapped by one or more INTERSECTION shapes
        MOVING_ORIGINAL_SHAPE, //the original shape being dragged on the grid by the player
        MOVING_SUBSTITUTION_INTERSECTION, //a shape that is the result of an intersection clipping operation between moving shape and static shapes
        MOVING_SUBSTITUTION_DIFFERENCE, //a shape that is the result of a difference clipping operation between moving shape and static shapes
        BUSY_CLIPPING, //the shape is being clipped (using threads) and must not be considered as dynamic anymore nor static yet
        DESTROYED //shape is marked to be destroyed
    }
    public ShapeState m_state { get; set; } //is the shape static or dynamic (i.e rendered with animation)

    public List<Shape> m_overlappingShapes { get; set; } //in case of the state of this shape is STATIC_OVERLAPPED, we store here the list of substitution shapes that share an intersection with it
    public List<Shape> m_shapesToCreate { get; set; } //the list to store shapes from the difference clipping operation with the overlapped static shape
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
        m_tint = other.m_tint;
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
     * Try to fusion this shape with the first static shape
     * A new static shape is created if a fusion occured and old fusioned shapes are destroyed
     * Else null is returned
     * **/
    public bool Fusion()
    {
        List<Shape> shapes = m_parentMesh.GetShapesHolder().m_shapes;
        Shape subjShape = this;
        Shape clipShape;

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

            if (this.OverlapsShape(shapeData, false)) //we have not found a clip shape yet
            {
                clipShape = shapeData;
                //we have to perform the clipping operation here to test if the intersection is made of isolated points.
                //In fact a fusion is declared successful when the result of the clipping operation is one single shape

                //Debug.Log("trying to fusion subjShape >>>");
                //for (int p = 0; p != subjShape.m_contour.Count; p++)
                //{
                //    Debug.Log(subjShape.m_contour[p]);
                //}
                //Debug.Log("with clipShape >>>");
                //for (int p = 0; p != shapeData.m_contour.Count; p++)
                //{
                //    Debug.Log(shapeData.m_contour[p]);
                //}
                List<Shape> resultingShapes = m_parentMesh.GetClippingManager().ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
                //Debug.Log("resultingShapesCount:" + resultingShapes.Count);
                if (resultingShapes.Count == 1) //success
                {
                    Shape fusionedShape = resultingShapes[0];
                    this.m_contour = fusionedShape.m_contour;
                    this.m_holes = fusionedShape.m_holes;

                    shapeData.m_state = ShapeState.DESTROYED;

                    //if some shapes were overlapping the clip shape, they will overlap the shape resulting from the union of subj shape and clip shape
                    //so move them them to that fusioned shape
                    if (clipShape.m_overlappingShapes != null)
                    {
                        if (this.m_overlappingShapes == null)
                            this.m_overlappingShapes = clipShape.m_overlappingShapes;
                        else
                            this.m_overlappingShapes.AddRange(clipShape.m_overlappingShapes);
                    }                   

                    return true;
                }
            }
        }

        //Debug.Log("trying to fusion subjShape >>>");
        //for (int p = 0; p != subjShape.m_contour.Count; p++)
        //{
        //    Debug.Log(subjShape.m_contour[p]);
        //}
        //Debug.Log("with clipShape >>>");
        //for (int p = 0; p != clipShape.m_contour.Count; p++)
        //{
        //    Debug.Log(clipShape.m_contour[p]);
        //}
        //List<Shape> resultingShapes = m_parentMesh.GetClippingManager().ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
        //Debug.Log("resultingShapesCount:" + resultingShapes.Count);
        //if (resultingShapes.Count == 1)
        //{
        //    Shape fusionedShape = resultingShapes[0];
        //    this.m_contour = fusionedShape.m_contour;
        //    this.m_holes = fusionedShape.m_holes;

        //    clipShape.m_state = ShapeState.DESTROYED;

        //    return true;
        //}

        return false;
    }

    /**
     * Check if this shape is fully contained inside the parameter 'outline'
     * **/
    public bool IsInsideOutline(DottedOutline outline)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != m_triangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = m_triangles[iTriangleIndex];
            if (!triangle.IsInsideOutline(outline))
                return false;
        }

        return true;
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

    public bool OverlapsTriangulable(Shape shape, bool bEnsureNonNullIntersection)
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
     * Change the state of this shape to STATIC_OVERLAPPED and build the list of overlapping shapes if not already done
     * **/
    public void AddOverlappingShape(Shape overlappingShape)
    {
        if (m_overlappingShapes == null)
            m_overlappingShapes = new List<Shape>();

        m_overlappingShapes.Add(overlappingShape);
    }

    /**
     * Once we performed the difference clipping operation, remove the overlapping shape from the list
     * **/
    private void RemoveOverlappingShape(Shape overlappingShape)
    {
        for (int i = 0; i != m_overlappingShapes.Count; i++)
        {
            if (m_overlappingShapes[i] == overlappingShape)
            {
                m_overlappingShapes.Remove(overlappingShape);
                return;
            }
        }
    }

    /**
     * Tells if the 'parameter' overlappingShape overlaps 'this' shape
     * **/
    private bool ContainsOverlappingShape(Shape overlappingShape)
    {
        if (m_overlappingShapes == null)
            return false;

        for (int i = 0; i != m_overlappingShapes.Count; i++)
        {
            if (m_overlappingShapes[i] == overlappingShape)
                return true;
        }

        return false;
    }

    /**
     * Substract every overlapping shape from this shape
     * **/
    public void PerformDifferenceWithOverlappingShapes()
    {
        ClippingManager clippingManager = m_parentMesh.GetGameScene().GetClippingManager();
        m_shapesToCreate.AddRange(clippingManager.PerformDifferenceAgainstShapes(this, m_overlappingShapes));

        //Destroy this shape
        m_state = ShapeState.DESTROYED;
    }

    /**
     * Calculate the difference between the shape that we found overlapping 'this' shape and 'this' shape
     * **/
    public void PerformDifferenceOnOverlappedStaticShape()
    {
        ClippingManager clippingManager = m_parentMesh.GetGameScene().GetClippingManager();

        List<Shape> allShapes = m_parentMesh.GetGameScene().m_shapesHolder.m_shapes;
        for (int i = 0; i != allShapes.Count; i++)
        {
            Shape shape = allShapes[i];

            if (shape.m_state == ShapeState.STATIC && shape.ContainsOverlappingShape(this)) //we found the overlapped shape, perform clipping operation
            {
                //check if the overlapping shape is actually intersecting the static shape with non-zero intersection
                if (this.OverlapsShape(shape, true))
                {
                    List<Shape> newOverlappedShapes = clippingManager.ShapesOperation(shape, this, ClipperLib.ClipType.ctDifference);
                    m_shapesToCreate.AddRange(newOverlappedShapes);

                    //copy the overlapping shapes to each overlapped shapes                
                    for (int p = 0; p != newOverlappedShapes.Count; p++)
                    {
                        newOverlappedShapes[p].m_overlappingShapes = new List<Shape>(shape.m_overlappingShapes);
                    }

                    shape.m_state = ShapeState.DESTROYED; //mark the old shape for destruction
                }

                shape.RemoveOverlappingShape(this); //in both cases we remove 'this' shape from overlapping shapes
            }
        }
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
        m_parentMesh.GetShapesHolder().DeleteDeadShapes();
    }
    
    /**
     * This method is called when player moves a shape over the grid
     * We recalculate here the intersection and difference shapes that are clipped against the static shapes
     * **/
    public void InvalidateSubstitutionShapes()
    {
        Shapes shapesHolder = m_parentMesh.GetShapesHolder();

        //First remove the INTERSECTION substitution shapes from the list of overlapping shapes on the relevant overlapped static shapes
        for (int i = 0; i != shapesHolder.m_shapes.Count; i++)
        {
            Shape shape = shapesHolder.m_shapes[i];
            if (shape.m_state == ShapeState.STATIC && shape.m_overlappingShapes != null)
                shape.m_overlappingShapes.Clear();
        }        

        if (m_substitutionShapes == null)
            m_substitutionShapes = new List<Shape>();

        //destroy and clear the old substitution shapes       
        for (int i = 0; i != m_substitutionShapes.Count; i++)
        {
            m_substitutionShapes[i].m_state = ShapeState.DESTROYED;
        }
        m_substitutionShapes.Clear();

        //Copy the shape before clipping it and make its state as default DIFFERENCE
        Shape shapeCopy = new Shape(this);
        shapeCopy.m_state = ShapeState.MOVING_SUBSTITUTION_DIFFERENCE;

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
        Shapes shapesHolder1 = this.m_parentMesh.GetShapesHolder();
        for (int i = 0; i != m_substitutionShapes.Count; i++)
        {
            m_substitutionShapes[i].FinalizeClippingOperations();
        }

        //destroy this shape object
        Shapes shapesHolder = this.m_parentMesh.GetShapesHolder();
        this.m_state = ShapeState.DESTROYED;
    }

    private class ContourHoleSharedPoint
    {
        public GridPoint m_point { get; set; }
        public Contour m_contour { get; set; } //the shape contour containing this shared point
        public Contour m_hole { get; set; } //the shape hole containing this shared point
        public int m_contourIndex { get; set; } //the index of this point in the shape contour
        public int m_holeIndex { get; set; } //the index of this point in the hole contour

        public ContourHoleSharedPoint(GridPoint point, Contour contour, Contour hole)
        {
            m_point = point;
            m_contour = contour;
            m_hole = hole;
            m_contourIndex = -1;
            m_holeIndex = -1;
        }

        public void CalculateContourIndex()
        {
            for (int i = 0; i != m_contour.Count; i++)
            {
                if (m_contour[i] == m_point)
                    m_contourIndex = i;
            }
        }
    }

    /**
     * This remove the middle vertex when 3 consecutive aligned vertices are found either on contour or holes
     * **/
    public void RemoveAlignedVertices()
    {
        m_contour.RemoveAlignedVertices();

        for (int i = 0; i != m_holes.Count; i++)
        {
            m_holes[i].RemoveAlignedVertices();
        }
    }

    /**
     * This shape can be split if some holes share points with this shape contour 
     * and/or one contour point is contained in another contour edge
     * **/
    public List<Shape> SplitIntoSimpleShapes()
    {
        List<ContourHoleSharedPoint> sharedPoints = new List<ContourHoleSharedPoint>();
        List<Contour> unsharedHoles = new List<Contour>(); //the list containing holes that do not share more than 1 vertex with the shape contour

        //First detect which hole points are shared with the contour
        for (int i = 0; i != m_holes.Count; i++)
        {
            Contour hole = m_holes[i];
            List<ContourHoleSharedPoint> holeSharedPoints = new List<ContourHoleSharedPoint>();
            for (int j = 0; j != hole.Count; j++)
            {                
                if (m_contour.ContainsPoint(hole[j]))
                {
                    ContourHoleSharedPoint sharedPoint = new ContourHoleSharedPoint(hole[j], m_contour, hole);
                    sharedPoint.m_holeIndex = j;
                    holeSharedPoints.Add(sharedPoint);
                }
            } 
            if (holeSharedPoints.Count > 1)
                sharedPoints.AddRange(holeSharedPoints);
            else
                unsharedHoles.Add(hole);
        }

        List<Shape> splitShapes = new List<Shape>();
        List<Contour> splitContours = new List<Contour>();

        //Test the 1st case: some holes share points with shape main contour
        if (sharedPoints.Count > 0)
        {
            //insert shared points into contour
            for (int i = 0; i != sharedPoints.Count; i++)
            {
                ContourHoleSharedPoint sharedPoint = sharedPoints[i];
                m_contour.InsertPoint(sharedPoint.m_point);
            }

            //Calculate their new indices in contour
            for (int i = 0; i != sharedPoints.Count; i++)
            {
                sharedPoints[i].CalculateContourIndex();
            }

            //Finally traverse the new built contour and build new shapes            
            for (int i = 0; i != sharedPoints.Count; i++)
            {
                Contour splitShapeContour = new Contour();
                splitContours.Add(splitShapeContour);
                ContourHoleSharedPoint startSharedPoint = sharedPoints[i]; //we store the first point of the new split shape contour
                int currentPointIndex = startSharedPoint.m_holeIndex;
                Contour currentHole = startSharedPoint.m_hole; //the hole that is traversed, it is set to null if shape contour is traversed
                bool onContour = false;
                bool bWhileLoopStopCondition = false;
                bool bFirstLoop = true; //use this bool to prevent the loop from breaking when we enter the shared point for the first time (loop must break when we enter it for the second time)
                do
                {
                    //is the new point index a shared point index
                    ContourHoleSharedPoint newSharedPoint = null;
                    for (int p = 0; p != sharedPoints.Count; p++)
                    {
                        ContourHoleSharedPoint sharedPoint = sharedPoints[p];
                        if (onContour && currentPointIndex == sharedPoint.m_contourIndex
                            ||
                            !onContour && currentHole == sharedPoint.m_hole && currentPointIndex == sharedPoint.m_holeIndex)
                        {
                            newSharedPoint = sharedPoint;
                            break;
                        }
                    }

                    if (newSharedPoint != null) //in this case change the split shape fill mode
                    {
                        onContour = !onContour;
                        if (onContour)
                        {
                            currentHole = null;
                            currentPointIndex = newSharedPoint.m_contourIndex;
                        }
                        else
                        {
                            currentHole = newSharedPoint.m_hole;
                            currentPointIndex = newSharedPoint.m_holeIndex;
                        }
                    }

                    //Find the point (contour or hole) to add
                    GridPoint currentPoint;
                    if (onContour)
                        currentPoint = m_contour[currentPointIndex];
                    else
                        currentPoint = currentHole[currentPointIndex];

                    splitShapeContour.Add(currentPoint);

                    if (onContour) //increment the index
                    {
                        currentPointIndex = (currentPointIndex < m_contour.Count - 1) ? currentPointIndex + 1 : 0;
                    }
                    else //increment in the same order despite the fact that hole is returned in contour opposite order by ClipperLib
                    {
                        currentPointIndex = (currentPointIndex < currentHole.Count - 1) ? currentPointIndex + 1 : 0;
                    }

                    bWhileLoopStopCondition = !bFirstLoop &&
                                              (
                                              onContour && currentPointIndex == startSharedPoint.m_contourIndex //on contour and same index as start shared point
                                                    ||
                                              !onContour && currentHole == startSharedPoint.m_hole && currentPointIndex == startSharedPoint.m_holeIndex //on hole and same hole and hole index as start shared point
                                               );
                    bFirstLoop = false;
                }
                while (!bWhileLoopStopCondition);
            }

            //remove duplicate split contours
            if (splitContours.Count > 1)
            {
                List<Contour> uniqueContours = new List<Contour>();
                for (int i = 0; i != splitContours.Count; i++)
                {
                    Contour splitContour = splitContours[i];
                    bool bAddContour = true;
                    for (int j = 0; j != uniqueContours.Count; j++)
                    {
                        if (splitContour.EqualsContour(uniqueContours[j])) //contour has already been added to the uniqueContours list
                            bAddContour = false;
                    }
                    if (bAddContour)
                    {
                        uniqueContours.Add(splitContour);
                    }
                }

                splitContours = uniqueContours;
            }
        }
        else //no shared point with any hole, simply add the shape main contour to the list of splitContours
            splitContours.Add(m_contour);

        //we need to check if every unique contour can be split again into simple contours
        List<Contour> simpleContours = new List<Contour>(splitContours.Count);
        for (int i = 0; i != splitContours.Count; i++)
        {
            Contour contour = splitContours[i];
            List<Contour> splitSimpleContours = contour.SplitIntoSimpleContours();
            simpleContours.AddRange(splitSimpleContours);
        }

        if (sharedPoints.Count == 0 && simpleContours.Count < 2) //no split occured
            return null;
        else
        {
            //Build a shape for every split contour
            for (int i = 0; i != simpleContours.Count; i++)
            {
                Shape splitShape = new Shape(simpleContours[i]);
                splitShape.Triangulate(); //make a first triangulation
                splitShapes.Add(splitShape);
            }

            //Assign remaining holes that do not share more than 2 vertices with contour to one split shape
            for (int i = 0; i != unsharedHoles.Count; i++)
            {
                Contour unsharedHole = unsharedHoles[i];
                for (int j = 0; j != splitShapes.Count; j++)
                {
                    Shape splitShape = splitShapes[j];

                    //check if this shape contains every point of this hole
                    bool bShapeContainsHole = true;
                    for (int p = 0; p != unsharedHole.Count; p++)
                    {
                        if (!splitShape.ContainsPoint(unsharedHole[p]))
                        {
                            bShapeContainsHole = false;
                            break;
                        }
                    }

                    //if so, add the hole to the shape
                    if (bShapeContainsHole)
                    {
                        splitShape.m_holes.Add(unsharedHole);
                        unsharedHoles.Remove(unsharedHole);
                        i--;
                        break;
                    }
                }
            }

            return splitShapes;
        }
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

    public void CalculateColorFromTint()
    {
        m_color = ColorUtils.GetRGBAColorFromTSB(new Vector3(m_tint, DEFAULT_SATURATION, DEFAULT_BRIGHTNESS), 1);
    }

    /**
     * Returns the tint, saturation, brightness of this shape
     * **/
    public Vector3 GetTSBValues()
    {
        return new Vector3(m_tint, DEFAULT_SATURATION, DEFAULT_BRIGHTNESS);
    }
}