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

    private AxisRenderer m_axis;
    private GameScene m_gameScene;
    private ClippingManager m_clippingManager;

    //Lists to store the results of the symmetry on both left and right sides
    private List<Shape> m_leftClippedInterShapes;
    private List<Shape> m_leftClippedDiffShapes;
    private List<Shape> m_rightClippedInterShapes;
    private List<Shape> m_rightClippedDiffShapes;

    public void Awake()
    {
        m_symmetryType = SymmetryType.NONE;
    }

    public void Init()
    {
        GameObject gameControllerObject = (GameObject) GameObject.FindGameObjectWithTag("GameController");
        m_gameScene = (GameScene)gameControllerObject.GetComponent<SceneManager>().m_currentScene;
        m_clippingManager = gameControllerObject.GetComponent<ClippingManager>();
        m_axis = this.GetComponent<AxisRenderer>();
    }

    /**
     * Apply symmetry on all shapes located inside the axis strip
     * **/
    public void SymmetrizeShapes()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

        //Split the strip first
        m_axis.m_stripData.Split();

        //Get strip left and right clip shapes
        Shape stripLeftClipShape = m_axis.m_stripData.m_stripLeftSubShape;
        Shape stripRightClipShape = m_axis.m_stripData.m_stripRightSubShape;

        //Clip all shapes
        List<Shape> shapes = shapesHolder.m_shapes;
        m_leftClippedInterShapes = new List<Shape>(10);
        m_leftClippedDiffShapes = new List<Shape>(10);
        m_rightClippedInterShapes = new List<Shape>(10);
        m_rightClippedDiffShapes = new List<Shape>(10);

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
                    Shape lSymmetricShape = CalculateSymmetricShape(lResultShapes[lShapeIdx]);
                    lSymmetricShape.Triangulate();
                    lSymmetricShape.m_color = shape.m_color; //dont mix the color of the shape with the color of the strip

                    List<Shape> leftClippedInterShapes, leftClippedDiffShapes;
                    m_clippingManager.ClipAgainstStaticShapes(lSymmetricShape, out leftClippedInterShapes, out leftClippedDiffShapes);
                    m_leftClippedInterShapes.AddRange(leftClippedInterShapes);
                    m_leftClippedDiffShapes.AddRange(leftClippedDiffShapes);
                }
            }

            if (stripRightClipShape != null)
            {
                List<Shape> rResultShapes = m_clippingManager.ShapesOperation(shape, stripRightClipShape, ClipperLib.ClipType.ctIntersection);
                for (int rShapeIdx = 0; rShapeIdx != rResultShapes.Count; rShapeIdx++)
                {
                    Shape rSymmetricShape = CalculateSymmetricShape(rResultShapes[rShapeIdx]);
                    rSymmetricShape.Triangulate();
                    rSymmetricShape.m_color = shape.m_color; //dont mix the color of the shape with the color of the strip

                    List<Shape> rightClippedInterShapes, rightClippedDiffShapes;
                    m_clippingManager.ClipAgainstStaticShapes(rSymmetricShape, out rightClippedInterShapes, out rightClippedDiffShapes);
                    m_rightClippedInterShapes.AddRange(rightClippedInterShapes);
                    m_rightClippedDiffShapes.AddRange(rightClippedDiffShapes);
                }
            }            
        }        
    }

    /**
     * Calculate the symmetrical axis of each axis in the grid by the parameter 'axis'
     * **/
    public void SymmetrizeAxes()
    {
        Axes axesHolder = m_gameScene.m_axes;

        List<AxisRenderer> axes = axesHolder.m_childrenAxes;
        for (int i = 0; i != axes.Count; i++)
        {
            AxisRenderer axis = axes[i];
            if (axis.m_type == AxisRenderer.AxisType.STATIC_PENDING)
            {
                //AxisRenderer symmetricAxis = axis.CalculateSymmetricAxis(this);
            }
        }
    }

    /**
     * Return a Shape that is symmetric of the parameter 'shapeToSymmetrize' about this axis
     * **/
    public Shape CalculateSymmetricShape(Shape shapeToSymmetrize)
    {
        //Symmetrize contour
        Contour contourToSymmetrize = shapeToSymmetrize.m_contour;
        Contour symmetricContour = new Contour(contourToSymmetrize.Count);

        for (int i = contourToSymmetrize.Count - 1; i != -1; i--)
        {
            symmetricContour.Add(CalculateSymmetricPoint(contourToSymmetrize[i]));
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
                symmetricHole.Add(CalculateSymmetricPoint(hole[j]));
            }
            symmetricHoles.Add(symmetricHole);
        }

        Shape symmetricShape = new Shape(symmetricContour, symmetricHoles);
        symmetricShape.m_color = shapeToSymmetrize.m_color;
        return symmetricShape;
    }

    /**
     * Calculate the symmetric axis of this axis by the parameter 'axis'
     * **/
    public AxisRenderer CalculateSymmetricAxis(AxisRenderer axisToSymmetrize)
    {
        Axes axesHolder = m_gameScene.m_axes;

        GridPoint axisEndpoint1Position = m_axis.m_pointA;
        GridPoint axisEndpoint2Position = m_axis.m_pointB;
        GridPoint axisToSymmetrizeEndpoint1Position = axisToSymmetrize.m_pointA;
        GridPoint axisToSymmetrizeEndpoint2Position = axisToSymmetrize.m_pointB;

        //Axis to symmetrize has to be fully contained in this axis strip in order to be actually symmetrized
        GridEdge axisEdge = new GridEdge(axisEndpoint1Position, axisEndpoint2Position);
        bool bSymmetrizeEndpoint1 = axisEdge.ContainsPointInStrip(axisToSymmetrizeEndpoint1Position);
        bool bSymmetrizeEndpoint2 = axisEdge.ContainsPointInStrip(axisToSymmetrizeEndpoint2Position);

        if (!bSymmetrizeEndpoint2)
        {
            Debug.Log("axisEdge A:" + axisEdge.m_pointA + " B:" + axisEdge.m_pointB);
            Debug.Log("axisToSymmetrize A:" + axisToSymmetrizeEndpoint1Position + " B:" + axisToSymmetrizeEndpoint2Position);
        }

        if (!bSymmetrizeEndpoint1 || !bSymmetrizeEndpoint2) //axis to symmetrize is not fully contained in this axis strip
            return null;        

        if (this.m_symmetryType == SymmetryType.SYMMETRY_AXES_ONE_SIDE)
        {
            long det1 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint1Position);
            long det2 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint2Position);

            if (det1 >= 0 && det2 >= 0) //axis to symmetrize is on the 'left' of this axis
            {
                GridPoint symmetricEndpoint1 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint1Position);
                GridPoint symmetricEndpoint2 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint2Position);

                //axis pointA has to be on the right of the line passing by the middle of the axis and directed by the clockwise normal of the axis
                GridPoint axisMiddle = axisToSymmetrize.GetCenter();
                GridPoint axisClockwiseNormal = axisToSymmetrize.GetNormal();
                GridEdge normalEdge = new GridEdge(axisMiddle, axisMiddle + axisClockwiseNormal); //this edge represents the normal vector with its two endpoints
                GridEdge symmetrizedNormalEdge = CalculateSymmetricEdge(normalEdge);

                //Determine the position of each symmetric endpoint about the symmetrizedNormalEdge
                long det = MathUtils.Determinant(symmetrizedNormalEdge.m_pointA, symmetrizedNormalEdge.m_pointB, symmetricEndpoint1);              
                if (det >= 0) //on the 'left', so swap points
                {
                    return axesHolder.BuildAxis(symmetricEndpoint2, symmetricEndpoint1, m_symmetryType, m_axis.m_type);
                }
                else
                {
                    return axesHolder.BuildAxis(symmetricEndpoint1, symmetricEndpoint2, m_symmetryType, m_axis.m_type);
                }
            }
            else if (det1 >= 0 && det2 < 0 || det1 < 0 && det2 >= 0)
            {
                return null; //TODO Build the axis, do not return null
            }
        }
        else if (this.m_symmetryType == SymmetryType.SYMMETRY_AXES_TWO_SIDES)
        {
            long det1 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint1Position);
            long det2 = MathUtils.Determinant(axisEndpoint1Position, axisEndpoint2Position, axisToSymmetrizeEndpoint2Position);

            GridPoint symmetricEndpoint1 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint1Position);
            GridPoint symmetricEndpoint2 = this.CalculateSymmetricPoint(axisToSymmetrizeEndpoint2Position);

            //axis pointA has to be on the right of the line passing by the middle of the axis and directed by the clockwise normal of the axis
            GridPoint axisMiddle = m_axis.GetCenter();
            GridPoint axisClockwiseNormal = m_axis.GetNormal();
            GridEdge normalEdge = new GridEdge(axisMiddle, axisMiddle + axisClockwiseNormal); //this edge represents the normal vector with its two endpoints
            GridEdge symmetrizedNormalEdge = CalculateSymmetricEdge(normalEdge);

            //Determine the position of each symmetric endpoint about the symmetrizedNormalEdge
            long det = MathUtils.Determinant(symmetrizedNormalEdge.m_pointA, symmetrizedNormalEdge.m_pointB, symmetricEndpoint1);
            if (det >= 0) //on the 'left', so swap points
            {
                return axesHolder.BuildAxis(symmetricEndpoint2, symmetricEndpoint1, m_symmetryType, m_axis.m_type);
            }
            else
            {
                return axesHolder.BuildAxis(symmetricEndpoint1, symmetricEndpoint2, m_symmetryType, m_axis.m_type);
            }
        }

        return null;
    }

    /**
     * When the job of clipping has been done through threading, this method is called to generate objects and animations on axis inside the main GUI thread
     * **/
    public void OnSymmetrizingShapesDone()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

        //build the difference shape objects
        if (m_leftClippedInterShapes != null)
        {
            for (int p = 0; p != m_leftClippedInterShapes.Count; p++)
            {
                m_leftClippedInterShapes[p].Triangulate();
                shapesHolder.CreateShapeObjectFromData(m_leftClippedInterShapes[p], true);
            }
        }

        //build the intersection shape objects
        if (m_leftClippedDiffShapes != null)
        {
            for (int p = 0; p != m_leftClippedDiffShapes.Count; p++)
            {
                m_leftClippedDiffShapes[p].Triangulate();
                shapesHolder.CreateShapeObjectFromData(m_leftClippedDiffShapes[p], true);
            }
        }

        //build the difference shape objects
        if (m_rightClippedInterShapes != null)
        {
            for (int p = 0; p != m_rightClippedInterShapes.Count; p++)
            {
                m_rightClippedInterShapes[p].Triangulate();
                shapesHolder.CreateShapeObjectFromData(m_rightClippedInterShapes[p], true);
            }
        }

        //build the intersection shape objects
        if (m_rightClippedDiffShapes != null)
        {
            for (int p = 0; p != m_rightClippedDiffShapes.Count; p++)
            {
                m_rightClippedDiffShapes[p].Triangulate();
                shapesHolder.CreateShapeObjectFromData(m_rightClippedDiffShapes[p], true);
            }
        }

        //Callback on the AxisRenderer
        m_axis.OnPerformSymmetry(m_symmetryType);
    }

    /**
     * Return the symmetric edge of the parameter 'edge' about this axis
     * **/
    public GridEdge CalculateSymmetricEdge(GridEdge edge)
    {
        GridPoint symmetricPointA = CalculateSymmetricPoint(edge.m_pointA);
        GridPoint symmetricPointB = CalculateSymmetricPoint(edge.m_pointB);

        return new GridEdge(symmetricPointA, symmetricPointB);
    }

    /**
     * Return the point symmetric of the parameter 'point' about this axis
     * **/
    public GridPoint CalculateSymmetricPoint(GridPoint point)
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
            GridPoint gridV = new GridPoint(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
            
            if (det > 0) // 'left'
            {
                symmetricPoint = point + gridV;
            }
            else // 'right'
                symmetricPoint = point - gridV;
        }

        symmetricPoint.m_scale = point.m_scale;
        return symmetricPoint;
    }

    /**
     * Called when SymmetrizeAxesByAxis() job is done
     * **/
    public void OnSymmetrizingAxesDone()
    {

    }

    public void SymmetrizeByPoint()
    {

    }

    /**
     * Find all intersections between a line and the box determined by grid boundaries
     * linePoint has to be specified in grid coordinates
     * **/
    private List<GridPoint> FindLineGridBoxIntersections(GridPoint linePoint, GridPoint lineDirection)
    {
        List<GridPoint> intersections = new List<GridPoint>();
        intersections.Capacity = 2;

        int scalePrecision = GridPoint.DEFAULT_SCALE_PRECISION;

        GridPoint gridTopLeft = new GridPoint(1, m_gameScene.m_grid.m_numLines);
        GridPoint gridTopRight = new GridPoint(m_gameScene.m_grid.m_numColumns, m_gameScene.m_grid.m_numLines);
        GridPoint gridBottomLeft = new GridPoint(1, 1);
        GridPoint gridBottomRight = new GridPoint(m_gameScene.m_grid.m_numColumns, 1);
        gridTopLeft.Scale(scalePrecision);
        gridTopRight.Scale(scalePrecision);
        gridBottomLeft.Scale(scalePrecision);
        gridBottomRight.Scale(scalePrecision);

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