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

    private SymmetryType m_symmetryType;

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
     * Apply symmetry on all shapes located inside the axis ribbon
     * **/
    public void SymmetrizeByAxis()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

        //Split the ribbon first
        m_axis.SplitRibbon(m_symmetryType);

        //Get ribbon left and right clip shapes
        Shape ribbonLeftClipShape = m_axis.Ribbon.m_ribbonLeftSubShape;
        Shape ribbonRightClipShape = m_axis.Ribbon.m_ribbonRightSubShape;

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

            if (ribbonLeftClipShape != null)
            {
                List<Shape> lResultShapes = m_clippingManager.ShapesOperation(shape, ribbonLeftClipShape, ClipperLib.ClipType.ctIntersection);
                
                for (int lShapeIdx = 0; lShapeIdx != lResultShapes.Count; lShapeIdx++)
                {
                    Shape lSymmetricShape = lResultShapes[lShapeIdx].CalculateSymmetricShape(m_axis);
                    lSymmetricShape.Triangulate();
                    lSymmetricShape.m_color = shape.m_color; //dont mix the color of the shape with the color of the ribbon

                    List<Shape> leftClippedInterShapes, leftClippedDiffShapes;
                    m_clippingManager.ClipAgainstStaticShapes(lSymmetricShape, out leftClippedInterShapes, out leftClippedDiffShapes);
                    m_leftClippedInterShapes.AddRange(leftClippedInterShapes);
                    m_leftClippedDiffShapes.AddRange(leftClippedDiffShapes);
                }
            }

            if (ribbonRightClipShape != null)
            {
                List<Shape> rResultShapes = m_clippingManager.ShapesOperation(shape, ribbonRightClipShape, ClipperLib.ClipType.ctIntersection);
                for (int rShapeIdx = 0; rShapeIdx != rResultShapes.Count; rShapeIdx++)
                {
                    Shape rSymmetricShape = rResultShapes[rShapeIdx].CalculateSymmetricShape(m_axis);
                    rSymmetricShape.Triangulate();
                    rSymmetricShape.m_color = shape.m_color; //dont mix the color of the shape with the color of the ribbon

                    List<Shape> rightClippedInterShapes, rightClippedDiffShapes;
                    m_clippingManager.ClipAgainstStaticShapes(rSymmetricShape, out rightClippedInterShapes, out rightClippedDiffShapes);
                    m_rightClippedInterShapes.AddRange(rightClippedInterShapes);
                    m_rightClippedDiffShapes.AddRange(rightClippedDiffShapes);
                }
            }            
        }        
    }

    /**
     * When the job of clipping has been done through threading, this method is called to generate objects and animations on axis inside the main GUI thread
     * **/
    public void OnSymmetryDone()
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
        AxisRenderer axis = this.GetComponent<AxisRenderer>(); 
        axis.OnPerformSymmetry(m_symmetryType);
    }

    public void SymmetrizeByPoint()
    {

    }

    /**
     * Creates a copy of triangles that are on a specific side (or both sides) of this axis
     * **/
    //private void ExtractTrianglesSeparatedByAxis(Vector2[] ribbonVertices, out List<List<ShapeTriangle>> leftTriangles, out List<List<ShapeTriangle>> rightTriangles, bool bExtractLeftTriangles = true, bool bExtractRightTriangles = true)
    //{
    //    AxisRenderer axisRenderer = this.gameObject.GetComponentInChildren<AxisRenderer>();
    //    Vector2 axisNormal = axisRenderer.GetAxisNormal(); //take the normal in clockwise order compared to the axisDirection

    //    ////First get all triangles
    //    GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
    //    List<List<ShapeTriangle>> allShapeTriangles = new List<List<ShapeTriangle>>(); //the vector containing all triangles in the scene per shape
    //    allShapeTriangles.Capacity = gameScene.m_shapes.m_shapesObjects.Count;

    //    for (int iShapeIndex = 0; iShapeIndex != gameScene.m_shapes.m_shapesObjects.Count; iShapeIndex++)
    //    {
    //        Shape shape = gameScene.m_shapes.m_shapesObjects[iShapeIndex].GetComponent<ShapeMesh>().m_shapeData;
    //        allShapeTriangles.Add(shape.GetShapeTriangles());
    //    }

    //    //Find intersections of lines starting from axis endpoint directed by axisNormal with the grid box
    //    //List<Vector2> line1GridBoxIntersections = FindLineGridBoxIntersections(axisRenderer.m_endpoint1GridPosition, axisNormal);
    //    //List<Vector2> line2GridBoxIntersections = FindLineGridBoxIntersections(axisRenderer.m_endpoint2GridPosition, axisNormal);

    //    ////Extract triangles that are in the ribbon of the axis...
    //    //... on the left side of the line that passes by the axisStartPoint
    //    List<List<ShapeTriangle>> extractedTriangles = ExtractTrianglesOnLineSide(ribbonVertices[0],
    //                                                                              ribbonVertices[1],
    //                                                                              allShapeTriangles,
    //                                                                              true);

    //    //... on the right side of the line that passes by the axisEndPoint
    //    extractedTriangles = ExtractTrianglesOnLineSide(ribbonVertices[2],
    //                                                    ribbonVertices[3],
    //                                                    extractedTriangles,
    //                                                    false);

    //    //... finally on the left and right side of the axis itself
    //    if (bExtractLeftTriangles)
    //    {
    //        leftTriangles = ExtractTrianglesOnLineSide(axisRenderer.m_endpoint1GridPosition,
    //                                                   axisRenderer.m_endpoint2GridPosition,
    //                                                   extractedTriangles,
    //                                                   true);
    //    }
    //    else
    //        leftTriangles = null;

    //    if (bExtractRightTriangles)
    //    {
    //        rightTriangles = ExtractTrianglesOnLineSide(axisRenderer.m_endpoint1GridPosition,
    //                                                    axisRenderer.m_endpoint2GridPosition,
    //                                                    extractedTriangles,
    //                                                    false);
    //    }
    //    else
    //        rightTriangles = null;
    //}

    /**
     * Extract all triangles from the list of triangles passed as parameters that are on one side of a line
     * The side of the line is determined when looking from the line start point to axis endpoint
     * If a triangle intersects the line, split it into smaller triangles
     * **/
    //private List<List<ShapeTriangle>> ExtractTrianglesOnLineSide(Vector2 lineStartPoint, Vector2 lineEndPoint, List<List<ShapeTriangle>> triangles, bool bLeftSide)
    //{
    //    List<List<ShapeTriangle>> extractedTriangles = new List<List<ShapeTriangle>>();

    //    for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != triangles.Count; iTrianglesVecIdx++)
    //    {
    //        List<ShapeTriangle> trianglesVec = triangles[iTrianglesVecIdx];
    //        List<ShapeTriangle> extractedTrianglesVec = null;
    //        for (int iTriangleIndex = 0; iTriangleIndex != trianglesVec.Count; iTriangleIndex++)
    //        {
    //            ShapeTriangle triangle = trianglesVec[iTriangleIndex];
    //            if (triangle.IntersectsLine(lineStartPoint, lineEndPoint)) //find the intersection points and split the triangle accordingly
    //            {
    //                ShapeTriangle[] splitTriangles;
    //                int splitTrianglesCount;
    //                triangle.Split(lineStartPoint, lineEndPoint, out splitTriangles, out splitTrianglesCount);

    //                for (int i = 0; i != splitTrianglesCount; i++)
    //                {
    //                    ShapeTriangle splitTriangle = splitTriangles[i];
    //                    splitTriangle.m_color = triangle.m_color;

    //                    Vector2 triangleBarycentre = splitTriangle.GetCenter();
    //                    float barycentreDet = MathUtils.Determinant(lineStartPoint, lineEndPoint, triangleBarycentre);

    //                    //the triangle barycentre is on the side of the line we want
    //                    if (barycentreDet > 0 && bLeftSide
    //                        ||
    //                        barycentreDet < 0 && !bLeftSide)
    //                    {
    //                        if (extractedTrianglesVec == null)
    //                        {
    //                            extractedTrianglesVec = new List<ShapeTriangle>();
    //                            extractedTriangles.Add(extractedTrianglesVec);
    //                        }
    //                        extractedTrianglesVec.Add(splitTriangle);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                Vector2 triangleBarycentre = triangle.GetCenter();
    //                float barycentreDet = MathUtils.Determinant(lineStartPoint, lineEndPoint, triangleBarycentre);

    //                //the triangle barycentre is on the side of the line we want
    //                if (barycentreDet > 0 && bLeftSide
    //                    ||
    //                    barycentreDet < 0 && !bLeftSide)
    //                {
    //                    if (extractedTrianglesVec == null)
    //                    {
    //                        extractedTrianglesVec = new List<ShapeTriangle>();
    //                        extractedTriangles.Add(extractedTrianglesVec);
    //                    }
    //                    extractedTrianglesVec.Add(triangle);
    //                }
    //            }
    //        }
    //    }

    //    return extractedTriangles;
    //}


    //private List<ShapeTriangle> ExtractTrianglesOnLineSide(Vector2 lineStartPoint, Vector2 lineEndPoint, List<ShapeTriangle> triangles, bool bLeftSide)
    //{
    //    if (triangles == null)
    //        return null;

    //    List<List<ShapeTriangle>> wrappedTriangles = new List<List<ShapeTriangle>>();
    //    wrappedTriangles.Add(triangles);
    //    List<List<ShapeTriangle>> extractedTriangles = ExtractTrianglesOnLineSide(lineStartPoint, lineEndPoint, wrappedTriangles, bLeftSide);
    //    if (extractedTriangles.Count == 1)
    //        return extractedTriangles[0];
    //    else //extractedTriangles is empty, return null
    //        return null;
    //}

    /**
     * Find all intersections between a line and the box determined by grid boundaries
     * linePoint has to be specified in grid coordinates
     * **/
    private List<GridPoint> FindLineGridBoxIntersections(GridPoint linePoint, GridPoint lineDirection)
    {
        List<GridPoint> intersections = new List<GridPoint>();
        intersections.Capacity = 2;

        int scalePrecision = GridPoint.DEFAULT_SCALE_PRECISION;

        GridPoint gridTopLeft = scalePrecision * new GridPoint(1, m_gameScene.m_grid.m_numLines, scalePrecision);
        GridPoint gridTopRight = scalePrecision * new GridPoint(m_gameScene.m_grid.m_numColumns, m_gameScene.m_grid.m_numLines, scalePrecision);
        GridPoint gridBottomLeft = scalePrecision * new GridPoint(1, 1, scalePrecision);
        GridPoint gridBottomRight = scalePrecision * new GridPoint(m_gameScene.m_grid.m_numColumns, 1, scalePrecision);

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
    //public List<ShapeTriangle> CalculateTrianglesReflectionsByAxis(Vector2[] ribbonVertices, List<ShapeTriangle> originalTriangles, bool bLeftSide)
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

    //    //trim triangles that are outside of the ribbon
    //    reflectedTriangles = ExtractTrianglesOnLineSide(ribbonVertices[0],
    //                                                    ribbonVertices[1],
    //                                                    reflectedTriangles,
    //                                                    true);

    //    reflectedTriangles = ExtractTrianglesOnLineSide(ribbonVertices[2],
    //                                                    ribbonVertices[3],
    //                                                    reflectedTriangles,
    //                                                    false);


    //    //TODO trim parts of the shape that are outside the grid

    //    return reflectedTriangles;
    //}

    /**
     * Return the point symmetric about the 'axis' parameter
     * **/
    public static GridPoint GetSymmetricPointAboutAxis(GridPoint point, AxisRenderer axis)
    {
        Vector2 axisNormal = axis.GetAxisNormal();
        GridPoint axisPoint1 = axis.m_endpoint1GridPosition;
        GridPoint axisPoint2 = axis.m_endpoint2GridPosition;      

        //Determine if the point is on the 'left' or on the 'right' of the axis
        float det = MathUtils.Determinant(axisPoint1, axisPoint2, point);
        if (det == 0) //point is on the axis
            return point;
        else
        {
            float distanceToAxis = GeometryUtils.DistanceToLine(point, axisPoint1, axis.GetAxisDirection());
            Vector2 v = 2 * distanceToAxis * axisNormal;
            GridPoint gridV = new GridPoint(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
            if (det > 0) // 'left'
            {
                return point + gridV;
            }
            else // 'right'
                return point - gridV;
        }
    }

    /**
     * Set the symmetry type for this axis based on the currently selected action button ID
     * **/
    public void SetSymmetryTypeFromActionButtonID(GUIButton.GUIButtonID buttonID)
    {
        if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES)
            m_symmetryType = SymmetryType.SYMMETRY_AXES_TWO_SIDES;
        else if (buttonID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE)
            m_symmetryType = SymmetryType.SYMMETRY_AXES_ONE_SIDE;
        else if (buttonID == GUIButton.GUIButtonID.ID_POINT_SYMMETRY)
            m_symmetryType = SymmetryType.SYMMETRY_POINT;
        else
            m_symmetryType = SymmetryType.NONE;
    }

    /**
     * Return the axis symmetry type
     * **/
    public SymmetryType GetSymmetryType()
    {
        return m_symmetryType;
    }
}