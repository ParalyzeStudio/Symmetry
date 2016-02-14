//#define REMOVE_THREADS_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Symmetrizer : MonoBehaviour 
{
    public enum SymmetryType
    {
        NONE = 0,
        SYMMETRY_AXES_TWO_SIDES,
        SYMMETRY_AXES_ONE_SIDE, //both straight and diagonal axes
        SYMMETRY_POINT,
    };

    public SymmetryType m_symmetryType { get; set; }

    private Axis m_axis;
    private SymmetryPoint m_symmetryPoint;
    private GameScene m_gameScene;
    private ClippingManager m_clippingManager;

    //Lists to store the results of the symmetry on both left and right sides
    private List<Shape> m_axisLeftClippedInterShapes;
    private List<Shape> m_axisLeftClippedDiffShapes;
    private List<Shape> m_axisRightClippedInterShapes;
    private List<Shape> m_axisRightClippedDiffShapes;
    private List<Shape> m_pointClippedInterShapes;
    private List<Shape> m_pointClippedDiffShapes;

    public void Awake()
    {
        m_symmetryType = SymmetryType.NONE;
    }

    public void Init()
    {
        GameObject gameControllerObject = (GameObject) GameObject.FindGameObjectWithTag("GameController");
        m_gameScene = (GameScene)gameControllerObject.GetComponent<SceneManager>().m_currentScene;
        m_clippingManager = gameControllerObject.GetComponent<ClippingManager>();
        
        //find either the axis or the symmetry point component
        m_axis = this.GetComponent<Axis>();
        m_symmetryPoint = this.GetComponent<SymmetryPoint>();
    }

    /**
     * Apply symmetry to scene elements depending on the symmetry type
     * **/
    //public void SymmetrizeShapes()
    //{      
    //    if (m_symmetryType == SymmetryType.SYMMETRY_AXES_ONE_SIDE || m_symmetryType == SymmetryType.SYMMETRY_AXES_TWO_SIDES)
    //        PerformAxisSymmetry();
    //    else if (m_symmetryType == SymmetryType.SYMMETRY_POINT)
    //        PerformPointSymmetry();
    //}

    public void PerformSymmetry()
    {
#if (REMOVE_THREADS_DEBUG)
        if (m_symmetryType == SymmetryType.SYMMETRY_AXES_ONE_SIDE || m_symmetryType == SymmetryType.SYMMETRY_AXES_TWO_SIDES)
            PerformAxisSymmetry();
        else if (m_symmetryType == SymmetryType.SYMMETRY_POINT)
            PerformPointSymmetry();

        OnSymmetryDone();
#else
        QueuedThreadedJobsManager threadedJobsManager = m_gameScene.GetQueuedThreadedJobsManager();
        if (m_symmetryType == SymmetryType.SYMMETRY_AXES_ONE_SIDE || m_symmetryType == SymmetryType.SYMMETRY_AXES_TWO_SIDES)
        {
            threadedJobsManager.AddJob(new ThreadedJob
                                            (
                                                new ThreadedJob.ThreadFunction(PerformAxisSymmetry),
                                                null,
                                                new ThreadedJob.ThreadFunction(OnSymmetryDone)
                                            )
                                      );
        }
        else if (m_symmetryType == SymmetryType.SYMMETRY_POINT)
        {
            threadedJobsManager.AddJob(new ThreadedJob
                                           (
                                               new ThreadedJob.ThreadFunction(PerformPointSymmetry),
                                               null,
                                               new ThreadedJob.ThreadFunction(OnSymmetryDone)
                                           )
                                     );
        }
#endif
    }

    /**
     * Symmetrize shapes by an axis
     * **/
    private void PerformAxisSymmetry()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

        //Split the strip first
        m_axis.m_stripData.Split();

        //Get strip left and right clip shapes
        Shape stripLeftClipShape = m_axis.m_stripData.m_stripLeftSubShape;
        Shape stripRightClipShape = m_axis.m_stripData.m_stripRightSubShape;

        //Clip all shapes
        List<Shape> shapes = shapesHolder.m_shapes;
        m_axisLeftClippedInterShapes = new List<Shape>(10);
        m_axisLeftClippedDiffShapes = new List<Shape>(10);
        m_axisRightClippedInterShapes = new List<Shape>(10);
        m_axisRightClippedDiffShapes = new List<Shape>(10);

        for (int i = 0; i != shapes.Count; i++)
        {
            Shape shape = shapes[i];

            if (shape.m_state != Shape.ShapeState.STATIC)
                continue;

            if (stripLeftClipShape != null)
            {
                List<Shape> lResultShapes = m_clippingManager.ShapesOperation(shape, stripLeftClipShape, ClipperLib.ClipType.ctIntersection);

                for (int lShapeIdx = 0; lShapeIdx != lResultShapes.Count; lShapeIdx++)
                {
                    Shape lSymmetricShape = CalculateSymmetricShapeByAxis(lResultShapes[lShapeIdx]);
                    lSymmetricShape.Triangulate();
                    //dont mix the color of the shape with the color of the strip
                    lSymmetricShape.m_tint = shape.m_tint;
                    lSymmetricShape.m_color = shape.m_color; 

                    List<Shape> leftClippedInterShapes, leftClippedDiffShapes;
                    m_clippingManager.ClipAgainstStaticShapes(lSymmetricShape, out leftClippedInterShapes, out leftClippedDiffShapes);
                    m_axisLeftClippedInterShapes.AddRange(leftClippedInterShapes);
                    m_axisLeftClippedDiffShapes.AddRange(leftClippedDiffShapes);
                }
            }

            if (stripRightClipShape != null)
            {
                List<Shape> rResultShapes = m_clippingManager.ShapesOperation(shape, stripRightClipShape, ClipperLib.ClipType.ctIntersection);
                for (int rShapeIdx = 0; rShapeIdx != rResultShapes.Count; rShapeIdx++)
                {
                    Shape rSymmetricShape = CalculateSymmetricShapeByAxis(rResultShapes[rShapeIdx]);
                    rSymmetricShape.Triangulate();
                    //dont mix the color of the shape with the color of the strip
                    rSymmetricShape.m_tint = shape.m_tint;
                    rSymmetricShape.m_color = shape.m_color;

                    List<Shape> rightClippedInterShapes, rightClippedDiffShapes;
                    m_clippingManager.ClipAgainstStaticShapes(rSymmetricShape, out rightClippedInterShapes, out rightClippedDiffShapes);
                    m_axisRightClippedInterShapes.AddRange(rightClippedInterShapes);
                    m_axisRightClippedDiffShapes.AddRange(rightClippedDiffShapes);
                }
            }
        }        
    }

    /**
     * Symmetrize shapes and axes by a point
     * **/
    private void PerformPointSymmetry()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

        //Clip all shapes
        List<Shape> shapes = shapesHolder.m_shapes;
        m_pointClippedInterShapes = new List<Shape>(10);
        m_pointClippedDiffShapes = new List<Shape>(10);

        for (int i = 0; i != shapes.Count; i++)
        {
            Shape shape = shapes[i];

            if (shape.m_state != Shape.ShapeState.STATIC)
                continue;

            Shape symmetricShape = CalculateSymmetricShapeByPoint(shape);
            Debug.Log("shape tint:" + shape.m_tint + " symmetric shape tint:" + symmetricShape.m_tint);
            symmetricShape.Triangulate();

            List<Shape> clippedInterShapes, clippedDiffShapes;
            m_clippingManager.ClipAgainstStaticShapes(symmetricShape, out clippedInterShapes, out clippedDiffShapes);
            m_pointClippedInterShapes.AddRange(clippedInterShapes);
            m_pointClippedDiffShapes.AddRange(clippedDiffShapes);
        }        
    }

    /**
     * Return a Shape that is symmetric of the parameter 'shapeToSymmetrize' about this axis
     * **/
    public Shape CalculateSymmetricShapeByAxis(Shape shapeToSymmetrize)
    {
        //Symmetrize contour
        Contour contourToSymmetrize = shapeToSymmetrize.m_contour;
        Contour symmetricContour = new Contour(contourToSymmetrize.Count);

        for (int i = contourToSymmetrize.Count - 1; i != -1; i--)
        {
            symmetricContour.Add(CalculateSymmetricPointByAxis(contourToSymmetrize[i]));
        }

        //Symmetrize holes
        List<Contour> holesToSymmetrize = shapeToSymmetrize.m_holes;
        List<Contour> symmetricHoles = new List<Contour>(holesToSymmetrize.Count);
        for (int i = 0; i != holesToSymmetrize.Count; i++)
        {
            Contour hole = holesToSymmetrize[i];
            Contour symmetricHole = new Contour(hole.Count);
            for (int j = hole.Count - 1; j != -1; j--)
            {
                symmetricHole.Add(CalculateSymmetricPointByAxis(hole[j]));
            }
            symmetricHoles.Add(symmetricHole);
        }

        Shape symmetricShape = new Shape(symmetricContour, symmetricHoles);
        symmetricShape.m_color = shapeToSymmetrize.m_color;
        return symmetricShape;
    }

    /**
     * Return a Shape that is symmetric of the parameter 'shapeToSymmetrize' about this symmetry point
     * **/
    public Shape CalculateSymmetricShapeByPoint(Shape shapeToSymmetrize)
    {
        //Symmetrize contour
        Contour contourToSymmetrize = shapeToSymmetrize.m_contour;
        Contour symmetricContour = new Contour(contourToSymmetrize.Count);

        for (int i = contourToSymmetrize.Count - 1; i != -1; i--)
        {
            symmetricContour.Add(CalculateSymmetricPointByPoint(contourToSymmetrize[i]));
        }
        symmetricContour.Reverse();

        //Symmetrize holes
        List<Contour> holesToSymmetrize = shapeToSymmetrize.m_holes;
        List<Contour> symmetricHoles = new List<Contour>(holesToSymmetrize.Count);
        for (int i = 0; i != holesToSymmetrize.Count; i++)
        {
            Contour hole = holesToSymmetrize[i];
            Contour symmetricHole = new Contour(hole.Count);
            for (int j = hole.Count - 1; j != -1; j--)
            {
                symmetricHole.Add(CalculateSymmetricPointByPoint(hole[j]));
            }
            symmetricHoles.Add(symmetricHole);
        }

        Shape symmetricShape = new Shape(symmetricContour, symmetricHoles);
        symmetricShape.m_tint = shapeToSymmetrize.m_tint;
        symmetricShape.m_color = shapeToSymmetrize.m_color;
        return symmetricShape;
    }

    /**
     * Calculate the symmetric axis of this axis by the parameter 'axis'
     * **/
    //public Axis CalculateSymmetricAxis(Axis axisToSymmetrize)
    //{
    //    Axes axesHolder = m_gameScene.m_axes;

    //    GridPoint axisEndpoint1Position = m_axis.m_pointA;
    //    GridPoint axisEndpoint2Position = m_axis.m_pointB;
    //    GridPoint axisToSymmetrizeEndpoint1Position = axisToSymmetrize.m_pointA;
    //    GridPoint axisToSymmetrizeEndpoint2Position = axisToSymmetrize.m_pointB;

    //    //Axis to symmetrize has to be fully contained in this axis strip in order to be actually symmetrized
    //    GridEdge axisEdge = new GridEdge(axisEndpoint1Position, axisEndpoint2Position);
    //    bool bSymmetrizeEndpoint1 = axisEdge.ContainsPointInStrip(axisToSymmetrizeEndpoint1Position);
    //    bool bSymmetrizeEndpoint2 = axisEdge.ContainsPointInStrip(axisToSymmetrizeEndpoint2Position);

    //    if (!bSymmetrizeEndpoint1 || !bSymmetrizeEndpoint2) //axis to symmetrize is not fully contained in this axis strip
    //        return null;        

    //    if (this.m_symmetryType == SymmetryType.SYMMETRY_AXES_ONE_SIDE)
    //    {
    //        long det1 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint1Position);
    //        long det2 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint2Position);

    //        if (det1 >= 0 && det2 >= 0) //axis to symmetrize is on the 'left' of this axis
    //        {
    //            GridPoint symmetricEndpoint1 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint1Position);
    //            GridPoint symmetricEndpoint2 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint2Position);

    //            //axis pointA has to be on the right of the line passing by the middle of the axis and directed by the clockwise normal of the axis
    //            GridPoint axisMiddle = axisToSymmetrize.GetCenter();
    //            GridPoint axisClockwiseNormal = axisToSymmetrize.GetNormal();
    //            GridEdge normalEdge = new GridEdge(axisMiddle, axisMiddle + axisClockwiseNormal); //this edge represents the normal vector with its two endpoints
    //            GridEdge symmetrizedNormalEdge = CalculateSymmetricEdge(normalEdge);

    //            //Determine the position of each symmetric endpoint about the symmetrizedNormalEdge
    //            long det = MathUtils.Determinant(symmetrizedNormalEdge.m_pointA, symmetrizedNormalEdge.m_pointB, symmetricEndpoint1);              
    //            if (det >= 0) //on the 'left', so swap points
    //            {
    //                return axesHolder.BuildAxis(symmetricEndpoint2, symmetricEndpoint1, m_symmetryType, m_axis.m_type);
    //            }
    //            else
    //            {
    //                return axesHolder.BuildAxis(symmetricEndpoint1, symmetricEndpoint2, m_symmetryType, m_axis.m_type);
    //            }
    //        }
    //        else if (det1 >= 0 && det2 < 0 || det1 < 0 && det2 >= 0)
    //        {
    //            return null; //TODO Build the axis, do not return null
    //        }
    //    }
    //    else if (this.m_symmetryType == SymmetryType.SYMMETRY_AXES_TWO_SIDES)
    //    {
    //        long det1 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint1Position);
    //        long det2 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint2Position);

    //        GridPoint symmetricEndpoint1 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint1Position);
    //        GridPoint symmetricEndpoint2 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint2Position);

    //        //axis pointA has to be on the right of the line passing by the middle of the axis and directed by the clockwise normal of the axis
    //        GridPoint axisMiddle = m_axis.GetCenter();
    //        GridPoint axisClockwiseNormal = m_axis.GetNormal();
    //        GridEdge normalEdge = new GridEdge(axisMiddle, axisMiddle + axisClockwiseNormal); //this edge represents the normal vector with its two endpoints
    //        GridEdge symmetrizedNormalEdge = CalculateSymmetricEdge(normalEdge);

    //        //Determine the position of each symmetric endpoint about the symmetrizedNormalEdge
    //        long det = MathUtils.Determinant(symmetrizedNormalEdge.m_pointA, symmetrizedNormalEdge.m_pointB, symmetricEndpoint1);
    //        if (det >= 0) //on the 'left', so swap points
    //        {
    //            return axesHolder.BuildAxis(symmetricEndpoint2, symmetricEndpoint1, m_symmetryType, m_axis.m_type);
    //        }
    //        else
    //        {
    //            return axesHolder.BuildAxis(symmetricEndpoint1, symmetricEndpoint2, m_symmetryType, m_axis.m_type);
    //        }
    //    }

    //    return null;
    //}  

    /**
     * Return the symmetric edge of the parameter 'edge' about this axis
     * **/
    public GridEdge CalculateSymmetricEdgeByAxis(GridEdge edge)
    {
        GridPoint symmetricPointA = CalculateSymmetricPointByAxis(edge.m_pointA);
        GridPoint symmetricPointB = CalculateSymmetricPointByAxis(edge.m_pointB);

        return new GridEdge(symmetricPointA, symmetricPointB);
    }

    /**
     * Return the symmetric edge of the parameter 'edge' about this symmetry point
     * **/
    public GridEdge CalculateSymmetricEdgeByPoint(GridEdge edge)
    {
        GridPoint symmetricPointA = CalculateSymmetricPointByPoint(edge.m_pointA);
        GridPoint symmetricPointB = CalculateSymmetricPointByPoint(edge.m_pointB);

        return new GridEdge(symmetricPointA, symmetricPointB);
    }

    /**
     * Return the point symmetric of the parameter 'point' about this axis
     * **/
    public GridPoint CalculateSymmetricPointByAxis(GridPoint point)
    {
        GridPoint axisNormal = m_axis.GetNormal();
        GridPoint axisPoint1 = m_axis.m_pointA;
        GridPoint axisPoint2 = m_axis.m_pointB;

        //Determine if the point is on the 'left' or on the 'right' of the axis
        long det = MathUtils.Determinant(axisPoint1, axisPoint2, point);
        GridPoint symmetricPoint;
        if (det == 0) //point is on the axis
            symmetricPoint = point;
        else
        {
            float distanceToAxis = GeometryUtils.DistanceToLine(point, axisPoint1, m_axis.GetDirection());
            Vector2 v = 2 * distanceToAxis * axisNormal / axisNormal.magnitude;
            GridPoint gridV = new GridPoint(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), false);
            
            if (det > 0) // 'left'
            {
                symmetricPoint = point + gridV;
            }
            else // 'right'
                symmetricPoint = point - gridV;
        }

        return symmetricPoint;
    }

    /**
     * Return the point symmetric of the parameter 'point' about this symmetry point
     * **/
    public GridPoint CalculateSymmetricPointByPoint(GridPoint point)
    {
        GridPoint symmetryPointPosition = m_symmetryPoint.GetCircleGridPosition();

        //Calculate the distance between the point and the symmetry point
        GridPoint pointToSymmetryPoint = (symmetryPointPosition - point);

        return symmetryPointPosition + pointToSymmetryPoint;
    }

    /**
    * When the job of clipping has been done through threading, this method is called to generate objects and animations on axis inside the main GUI thread
    * **/
    public void OnSymmetryDone()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

        if (m_symmetryType == SymmetryType.SYMMETRY_AXES_ONE_SIDE || m_symmetryType == SymmetryType.SYMMETRY_AXES_TWO_SIDES)
        {
            //build the difference shape objects
            if (m_axisLeftClippedInterShapes != null)
            {
                for (int p = 0; p != m_axisLeftClippedInterShapes.Count; p++)
                {
                    m_axisLeftClippedInterShapes[p].Triangulate();
                    //shapesHolder.CreateShapeObjectFromData(m_axisLeftClippedInterShapes[p], true);
                    shapesHolder.CreateShapeObjectFromData(m_axisLeftClippedInterShapes[p], false);
                    m_axisLeftClippedInterShapes[p].FinalizeClippingOperations();
                }
            }

            //build the intersection shape objects
            if (m_axisLeftClippedDiffShapes != null)
            {
                for (int p = 0; p != m_axisLeftClippedDiffShapes.Count; p++)
                {
                    m_axisLeftClippedDiffShapes[p].Triangulate();
                    //shapesHolder.CreateShapeObjectFromData(m_axisLeftClippedDiffShapes[p], true);
                    shapesHolder.CreateShapeObjectFromData(m_axisLeftClippedDiffShapes[p], false);
                    m_axisLeftClippedDiffShapes[p].FinalizeClippingOperations();
                }
            }

            //build the difference shape objects
            if (m_axisRightClippedInterShapes != null)
            {
                for (int p = 0; p != m_axisRightClippedInterShapes.Count; p++)
                {
                    m_axisRightClippedInterShapes[p].Triangulate();
                    //shapesHolder.CreateShapeObjectFromData(m_axisRightClippedInterShapes[p], true);
                    shapesHolder.CreateShapeObjectFromData(m_axisRightClippedInterShapes[p], false);
                    m_axisRightClippedInterShapes[p].FinalizeClippingOperations();
                }
            }

            //build the intersection shape objects
            if (m_axisRightClippedDiffShapes != null)
            {
                for (int p = 0; p != m_axisRightClippedDiffShapes.Count; p++)
                {
                    m_axisRightClippedDiffShapes[p].Triangulate();
                    //shapesHolder.CreateShapeObjectFromData(m_axisRightClippedDiffShapes[p], true);
                    shapesHolder.CreateShapeObjectFromData(m_axisRightClippedDiffShapes[p], false);
                    m_axisRightClippedDiffShapes[p].FinalizeClippingOperations();
                }
            }

            //Callback on the AxisRenderer
            m_axis.OnPerformSymmetry();
        }
        else if (m_symmetryType == SymmetryType.SYMMETRY_POINT)
        {
            //build the intersection shape objects
            if (m_pointClippedInterShapes != null)
            {
                for (int p = 0; p != m_pointClippedInterShapes.Count; p++)
                {
                    m_pointClippedInterShapes[p].Triangulate();
                    //shapesHolder.CreateShapeObjectFromData(m_pointClippedInterShapes[p], true);
                    shapesHolder.CreateShapeObjectFromData(m_pointClippedInterShapes[p], false);
                    m_pointClippedInterShapes[p].FinalizeClippingOperations();
                }
            }

            //build the difference shape objects
            if (m_pointClippedDiffShapes != null)
            {
                for (int p = 0; p != m_pointClippedDiffShapes.Count; p++)
                {
                    m_pointClippedDiffShapes[p].Triangulate();
                    //shapesHolder.CreateShapeObjectFromData(m_pointClippedDiffShapes[p], true);
                    shapesHolder.CreateShapeObjectFromData(m_pointClippedDiffShapes[p], false);
                    m_pointClippedDiffShapes[p].FinalizeClippingOperations();
                }
            }

            m_symmetryPoint.OnPerformSymmetry();
        }
    }

    /**
     * Find all intersections between a line and the box determined by grid boundaries
     * linePoint has to be specified in grid coordinates
     * **/
    private List<GridPoint> FindLineGridBoxIntersections(GridPoint linePoint, GridPoint lineDirection)
    {
        List<GridPoint> intersections = new List<GridPoint>();
        intersections.Capacity = 2;

        GridPoint gridTopLeft = new GridPoint(1, m_gameScene.m_grid.m_numLines, true);
        GridPoint gridTopRight = new GridPoint(m_gameScene.m_grid.m_numColumns, m_gameScene.m_grid.m_numLines, true);
        GridPoint gridBottomLeft = new GridPoint(1, 1, true);
        GridPoint gridBottomRight = new GridPoint(m_gameScene.m_grid.m_numColumns, 1, true);

        GridEdge leftEdge = new GridEdge(gridBottomLeft, gridTopLeft);
        GridEdge topEdge = new GridEdge(gridTopLeft, gridTopRight);
        GridEdge rightEdge = new GridEdge(gridTopRight, gridBottomRight);
        GridEdge bottomEdge = new GridEdge(gridBottomRight, gridBottomLeft);

        bool intersects;
        GridPoint intersection;
        leftEdge.IntersectionWithLine(linePoint, lineDirection, out intersects, out intersection);
        if (intersects)
            intersections.Add(intersection);
        topEdge.IntersectionWithLine(linePoint, lineDirection, out intersects, out intersection);
        if (intersects)
            intersections.Add(intersection);
        rightEdge.IntersectionWithLine(linePoint, lineDirection, out intersects, out intersection);
        if (intersects)
            intersections.Add(intersection);
        bottomEdge.IntersectionWithLine(linePoint, lineDirection, out intersects, out intersection);
        if (intersects)
            intersections.Add(intersection);

        //remove duplicates to obtain a vector of 2 elements
        int iIntersectionsCount = intersections.Count;
        if (iIntersectionsCount > 2)
        {
            int iIntersectionIndex = 0;
            while (iIntersectionIndex != iIntersectionsCount)
            {
                for (int i = iIntersectionIndex + 1; i != iIntersectionsCount; i++)
                {
                    if (intersections[i] == intersections[iIntersectionIndex])
                    {
                        intersections.Remove(intersections[i]);
                        iIntersectionsCount--;
                        i--;
                    }
                }

                iIntersectionIndex++;
            }
        }

        //reorder points so the vector between them is collinear to lineDirection
        Vector2 diff = intersections[1] - intersections[0];
        if (MathUtils.DotProduct(diff, lineDirection) < 0) //diff and lineDirection are of opposite directions, swap the two elements
        {
            GridPoint tmpIntersection = intersections[1];
            intersections[1] = intersections[0];
            intersections[0] = tmpIntersection;
        }

        return intersections;
    }

    /**
     * Calculate the coordinates of vertices reflected by the axis
     * Specify whether the original triangles are on the left or right side of the axis
     * **/
    //public List<ShapeTriangle> CalculateTrianglesReflectionsByAxis(Vector2[] stripVertices, List<ShapeTriangle> originalTriangles, bool bLeftSide)
    //{
    //    List<ShapeTriangle> reflectedTriangles = new List<ShapeTriangle>();
    //    reflectedTriangles.Capacity = originalTriangles.Count;

    //    AxisRenderer axisRenderer = this.gameObject.GetComponentInChildren<AxisRenderer>();
    //    Vector2 axisDirection = axisRenderer.GetAxisDirection();
    //    Vector2 axisNormal = axisRenderer.GetAxisNormal();

    //    for (int iTriangleIndex = 0; iTriangleIndex != originalTriangles.Count; iTriangleIndex++)
    //    {
    //        ShapeTriangle originalTriangle = originalTriangles[iTriangleIndex];
    //        ShapeTriangle reflectedTriangle = new ShapeTriangle(originalTriangle.m_parentShape, originalTriangle.m_color);

    //        for (int i = 0; i != 3; i++)
    //        {
    //            Vector2 originalVertex = originalTriangle.m_points[i];
    //            float distanceToAxis = GeometryUtils.DistanceToLine(originalVertex, axisRenderer.m_endpoint1GridPosition, axisDirection);
    //            Vector2 reflectedVertex = originalVertex + (bLeftSide ? 1 : -1) * axisNormal * 2 * distanceToAxis;

    //            //place the reflected vertex in correct order to produce a ccw triangle (invert last two vertices)
    //            if (i == 1)
    //                reflectedTriangle.m_points[2] = reflectedVertex;
    //            else if (i == 2)
    //                reflectedTriangle.m_points[1] = reflectedVertex;
    //            else
    //                reflectedTriangle.m_points[0] = reflectedVertex;

    //        }

    //        reflectedTriangles.Add(reflectedTriangle);            
    //    }

    //    //trim triangles that are outside of the strip
    //    reflectedTriangles = ExtractTrianglesOnLineSide(stripVertices[0],
    //                                                    stripVertices[1],
    //                                                    reflectedTriangles,
    //                                                    true);

    //    reflectedTriangles = ExtractTrianglesOnLineSide(stripVertices[2],
    //                                                    stripVertices[3],
    //                                                    reflectedTriangles,
    //                                                    false);


    //    //TODO trim parts of the shape that are outside the grid

    //    return reflectedTriangles;
    //}

    /**
     * Set the symmetry type for this axis based on the currently selected action button ID
     * **/
    public static SymmetryType GetSymmetryTypeFromActionButtonID(GUIButton.GUIButtonID buttonID)
    {
        if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES)
            return SymmetryType.SYMMETRY_AXES_TWO_SIDES;
        else if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE)
            return SymmetryType.SYMMETRY_AXES_ONE_SIDE;
        else if (buttonID == GUIButton.GUIButtonID.ID_POINT_SYMMETRY)
            return SymmetryType.SYMMETRY_POINT;
        else
            return SymmetryType.NONE;
    }

    /**
     * Return the axis symmetry type
     * **/
    public SymmetryType GetSymmetryType()
    {
        return m_symmetryType;
    }
}