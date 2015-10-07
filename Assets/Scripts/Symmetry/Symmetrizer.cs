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

    private GameScene m_gameScene;

    public void Awake()
    {
        m_symmetryType = SymmetryType.NONE;
    }

    /**
     * Apply symmetry on all shapes located inside the axis ribbon
     * **/
    public void SymmetrizeByAxis()
    {
        Shapes shapesHolder = GetGameScene().GetComponentInChildren<Shapes>();
        AxisRenderer axis = this.GetComponent<AxisRenderer>();

        //Split the ribbon first
        axis.SplitRibbon(m_symmetryType);

        //Get ribbon left and right clip shapes
        Shape ribbonLeftClipShape = this.GetComponent<AxisRenderer>().Ribbon.m_ribbonLeftSubShape;
        Shape ribbonRightClipShape = this.GetComponent<AxisRenderer>().Ribbon.m_ribbonRightSubShape;

        //Clip all shapes
        List<Shape> shapes = GetGameScene().GetComponentInChildren<Shapes>().m_shapes;

        for (int i = 0; i != shapes.Count; i++)
        {
            Shape shape = shapes[i];

            if (shape.m_state != Shape.ShapeState.STATIC)
                continue;

            if (ribbonLeftClipShape != null)
            {
                List<Shape> lResultShapes = ClippingBooleanOperations.ShapesOperation(shape, ribbonLeftClipShape, ClipperLib.ClipType.ctIntersection);
                for (int lShapeIdx = 0; lShapeIdx != lResultShapes.Count; lShapeIdx++)
                {
                    Shape lSymmetricShape = lResultShapes[lShapeIdx].CalculateSymmetricShape(axis);
                    lSymmetricShape.Triangulate();
                    lSymmetricShape.m_color = shape.m_color; //dont mix the color of the shape with the color of the ribbon
                    shapesHolder.ClipAgainstStaticShapes(lSymmetricShape);
                }
            }

            //if (ribbonRightClipShape != null)
            //{
            //    List<Shape> rResultShapes = ClippingBooleanOperations.ShapesOperation(shape, ribbonRightClipShape, ClipperLib.ClipType.ctIntersection);
            //    for (int rShapeIdx = 0; rShapeIdx != rResultShapes.Count; rShapeIdx++)
            //    {
            //        Shape rSymmetricShape = rResultShapes[rShapeIdx].CalculateSymmetricShape(axis);
            //        rSymmetricShape.Triangulate();
            //        rSymmetricShape.m_color = shape.m_color; //dont mix the color of the shape with the color of the ribbon
            //        shapesHolder.ClipAgainstStaticShapes(rSymmetricShape);
            //    }
            //}
        }

        //Callback on the AxisRenderer
        axis.OnPerformSymmetry(m_symmetryType);
    }

    //public void SymmetrizeByAxis()
    //{
    //    GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;

    //    //Find axis ribbon 4 vertices
    //    AxisRenderer axisRenderer = this.gameObject.GetComponent<AxisRenderer>();
    //    Vector2 axisNormal = axisRenderer.GetAxisNormal(); //take the normal in clockwise order compared to the axisDirection
    //    List<Vector2> line1GridBoxIntersections = FindLineGridBoxIntersections(axisRenderer.m_endpoint1GridPosition, axisNormal);
    //    List<Vector2> line2GridBoxIntersections = FindLineGridBoxIntersections(axisRenderer.m_endpoint2GridPosition, axisNormal);
    //    //ribbon vertices = bottom-left, bottom-right, top-left, top-right
    //    Vector2[] ribbonVertices = new Vector2[4] {line1GridBoxIntersections[0], line1GridBoxIntersections[1], line2GridBoxIntersections[0], line2GridBoxIntersections[1]};

    //    //Extract triangles
    //    List<List<ShapeTriangle>> leftTriangles = null;
    //    List<List<ShapeTriangle>> rightTriangles = null;

    //    if (m_symmetryType == SymmetryType.SYMMETRY_AXES_TWO_SIDES)
    //        ExtractTrianglesSeparatedByAxis(ribbonVertices, out leftTriangles, out rightTriangles);
    //    else if (m_symmetryType == SymmetryType.SYMMETRY_AXES_ONE_SIDE)
    //        ExtractTrianglesSeparatedByAxis(ribbonVertices, out leftTriangles, out rightTriangles, true , false);

    //    //Rebuild shapes from those lists of triangles
    //    Vector3 axisCenter = axisRenderer.GetAxisCenterInWorldCoordinates();
    //    Vector3 axisDirection = axisRenderer.GetAxisDirection();

    //    Shape shapeData = null;
    //    if (leftTriangles != null && leftTriangles.Count > 0)
    //    {
    //        for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != leftTriangles.Count; iTrianglesVecIdx++)
    //        {
    //            List<ShapeTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(ribbonVertices, leftTriangles[iTrianglesVecIdx], true);

    //            if (reflectedTriangles != null)
    //            {
    //                shapeData = new Shape(false);
    //                shapeData.SetShapeTriangles(reflectedTriangles);
    //                shapeData.CalculateContour();
    //                shapeData.CalculateArea();
    //                shapeData.ObtainColorFromTriangles(); //invalidate the shape color and update it from the new set of triangles
    //                GameObject newShapeObject = gameScene.m_shapes.CreateShapeObjectFromData(shapeData);
    //                gameScene.m_shapes.InitClippingOperationsOnShapeObject(newShapeObject);
    //                gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();
    //                gameScene.m_shapes.FinalizeClippingOperations();
    //                //ShapeAnimator shapeObjectAnimator = newShapeObject.GetComponent<ShapeAnimator>();
    //                //shapeObjectAnimator.UpdatePivotPointPosition(axisCenter);
    //                //shapeObjectAnimator.SetRotationAxis(axisDirection);
    //                //shapeObjectAnimator.SetRotationAngle(90);
    //                //shapeObjectAnimator.RotateTo(0, 5.0f);
    //            }
    //        }
    //    }

    //    if (rightTriangles != null && rightTriangles.Count > 0)
    //    {
    //        for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != rightTriangles.Count; iTrianglesVecIdx++)
    //        {
    //            List<ShapeTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(ribbonVertices, rightTriangles[iTrianglesVecIdx], false);

    //            if (reflectedTriangles != null)
    //            {
    //                shapeData = new Shape(false);
    //                shapeData.SetShapeTriangles(reflectedTriangles);
    //                shapeData.CalculateContour();
    //                shapeData.CalculateArea();
    //                shapeData.ObtainColorFromTriangles(); //invalidate the shape color and update it from the new set of triangles
    //                GameObject newShapeObject = gameScene.m_shapes.CreateShapeObjectFromData(shapeData);
    //                gameScene.m_shapes.InitClippingOperationsOnShapeObject(newShapeObject);
    //                gameScene.m_shapes.InvalidateOverlappingAndSubstitutionShapes();
    //                gameScene.m_shapes.FinalizeClippingOperations();
    //                //ShapeAnimator shapeObjectAnimator = newShapeObject.GetComponent<ShapeAnimator>();
    //                //shapeObjectAnimator.UpdatePivotPointPosition(axisCenter);
    //                //shapeObjectAnimator.SetRotationAxis(axisDirection);
    //                //shapeObjectAnimator.SetRotationAngle(-90);
    //                //shapeObjectAnimator.RotateTo(0, 5.0f);
    //            }
    //        }
    //    }

    //    if (shapeData != null)
    //    {
    //        ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
    //        Shapes.PerformFusionOnShape(shapeData);

    //        gameScene.m_counter.IncrementCounter();
    //    }
    //}

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
    private List<Vector2> FindLineGridBoxIntersections(Vector2 linePoint, Vector2 lineDirection)
    {
        List<Vector2> intersections = new List<Vector2>();
        intersections.Capacity = 2;

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
        Vector2 gridTopLeft = new Vector2(1, gameScene.m_grid.m_numLines);
        Vector2 gridTopRight = new Vector2(gameScene.m_grid.m_numColumns, gameScene.m_grid.m_numLines);
        Vector2 gridBottomLeft = new Vector2(1, 1);
        Vector2 gridBottomRight = new Vector2(gameScene.m_grid.m_numColumns, 1);

        bool intersects;
        Vector2 intersection;
        GeometryUtils.SegmentLineIntersection(gridBottomLeft, gridTopLeft, linePoint, lineDirection, out intersection, out intersects);
        if (intersects)
            intersections.Add(intersection);
        GeometryUtils.SegmentLineIntersection(gridTopLeft, gridTopRight, linePoint, lineDirection, out intersection, out intersects);
        if (intersects)
            intersections.Add(intersection);
        GeometryUtils.SegmentLineIntersection(gridTopRight, gridBottomRight, linePoint, lineDirection, out intersection, out intersects);
        if (intersects)
            intersections.Add(intersection);
        GeometryUtils.SegmentLineIntersection(gridBottomRight, gridBottomLeft, linePoint, lineDirection, out intersection, out intersects);
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

        if (intersections.Count != 2) //this case should never happen
        {
            intersections.Clear();
            intersections.Capacity = 2;
            intersections.Add(new Vector2(0, 0));
            intersections.Add(new Vector2(0, 0));
        }

        //reorder points so the vector between them is collinear to lineDirection
        Vector2 diff = intersections[1] - intersections[0];
        if (MathUtils.DotProduct(diff, lineDirection) < 0) //diff and lineDirection are of opposite directions, swap the two elements
        {
            Vector2 tmpIntersection = intersections[1];
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
    public static Vector2 GetSymmetricPointAboutAxis(Vector2 point, AxisRenderer axis, bool bGridPoint = false)
    {
        Vector2 axisNormal = axis.GetAxisNormal();
        Vector2 axisPoint1 = bGridPoint ? axis.m_endpoint1GridPosition : axis.m_endpoint1Position;
        Vector2 axisPoint2 = bGridPoint ? axis.m_endpoint2GridPosition : axis.m_endpoint2Position;      

        //Determine if the point is on the 'left' or on the 'right' of the axis
        float det = MathUtils.Determinant(axisPoint1, axisPoint2, point);
        if (det == 0) //point is on the axis
            return point;
        else
        {
            float distanceToAxis = GeometryUtils.DistanceToLine(point, bGridPoint ? axis.m_endpoint1GridPosition : axis.m_endpoint1Position, axis.GetAxisDirection());
            if (det > 0) // 'left'
                return point + 2 * distanceToAxis * axisNormal;
            else // 'right'
                return point - 2 * distanceToAxis * axisNormal;
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

    private GameScene GetGameScene()
    {
        if (m_gameScene == null)
            m_gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
        return m_gameScene;
    }
}