using UnityEngine;
using System.Collections.Generic;

public class AxisSymmetrizer : Symmetrizer
{
    private AxisRenderer m_axisRenderer;

    private List<Shape> m_axisLeftClippedInterShapes;
    private List<Shape> m_axisLeftClippedDiffShapes;
    private List<Shape> m_axisRightClippedInterShapes;
    private List<Shape> m_axisRightClippedDiffShapes;

    public override void Init()
    {
        base.Init();

        m_axisRenderer = this.GetComponent<AxisRenderer>();
    }

    /**
     * Symmetrize shapes by an axis
     * **/
    protected override void PerformSymmetry()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

        //Split the strip first
        m_axisRenderer.m_stripData.Split();

        //Get strip left and right clip shapes
        Shape stripLeftClipShape = m_axisRenderer.m_stripData.m_stripLeftSubShape;
        Shape stripRightClipShape = m_axisRenderer.m_stripData.m_stripRightSubShape;

        //Clip all shapes
        List<Shape> shapes = shapesHolder.m_shapes;
        m_axisLeftClippedInterShapes = new List<Shape>(10);
        m_axisLeftClippedDiffShapes = new List<Shape>(10);
        m_axisRightClippedInterShapes = new List<Shape>(10);
        m_axisRightClippedDiffShapes = new List<Shape>(10);

        for (int i = 0; i != shapes.Count; i++)
        {
            Shape shape = shapes[i];

            Debug.Log("shape.m_state:" + shape.m_state);
            if (shape.m_state != Shape.ShapeState.STATIC)
                continue;

            if (stripLeftClipShape != null)
            {
                List<Shape> lResultShapes = m_clippingManager.ShapesOperation(shape, stripLeftClipShape, ClipperLib.ClipType.ctIntersection);

                //Debug.Log("lResultShapes COUNT:" + lResultShapes.Count);

                for (int lShapeIdx = 0; lShapeIdx != lResultShapes.Count; lShapeIdx++)
                {
                    Shape lSymmetricShape = CalculateSymmetricShape(lResultShapes[lShapeIdx]);
                    lSymmetricShape.Triangulate();
                    //dont mix the color of the shape with the color of the strip
                    lSymmetricShape.m_tint = shape.m_tint;
                    lSymmetricShape.m_color = shape.m_color;

                    List<Shape> leftClippedInterShapes, leftClippedDiffShapes;
                    m_clippingManager.ClipAgainstStaticShapes(lSymmetricShape, out leftClippedInterShapes, out leftClippedDiffShapes);
                    m_axisLeftClippedInterShapes.AddRange(leftClippedInterShapes);
                    m_axisLeftClippedDiffShapes.AddRange(leftClippedDiffShapes);

                    //Debug.Log("leftClippedInterShapes COUNT:" + leftClippedInterShapes.Count);
                    //Debug.Log("leftClippedDiffShapes COUNT:" + leftClippedDiffShapes.Count);
                }
            }

            if (stripRightClipShape != null)
            {
                List<Shape> rResultShapes = m_clippingManager.ShapesOperation(shape, stripRightClipShape, ClipperLib.ClipType.ctIntersection);
                for (int rShapeIdx = 0; rShapeIdx != rResultShapes.Count; rShapeIdx++)
                {
                    Shape rSymmetricShape = CalculateSymmetricShape(rResultShapes[rShapeIdx]);
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
     * Return a Shape that is symmetric of the parameter 'shapeToSymmetrize' about this axis
     * **/
    public override Shape CalculateSymmetricShape(Shape shapeToSymmetrize)
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
     * Return the symmetric edge of the parameter 'edge' about this axis
     * **/
    public override GridEdge CalculateSymmetricEdge(GridEdge edge)
    {
        GridPoint symmetricPointA = CalculateSymmetricPoint(edge.m_pointA);
        GridPoint symmetricPointB = CalculateSymmetricPoint(edge.m_pointB);

        return new GridEdge(symmetricPointA, symmetricPointB);
    }

    /**
     * Return the point symmetric of the parameter 'point' about this axis
     * **/
    public override GridPoint CalculateSymmetricPoint(GridPoint point)
    {
        GridPoint axisNormal = m_axisRenderer.m_axisData.GetNormal();
        GridPoint axisPoint1 = m_axisRenderer.m_axisData.m_pointA;
        GridPoint axisPoint2 = m_axisRenderer.m_axisData.m_pointB;

        //Determine if the point is on the 'left' or on the 'right' of the axis
        long det = MathUtils.Determinant(axisPoint1, axisPoint2, point);
        GridPoint symmetricPoint;
        if (det == 0) //point is on the axis
            symmetricPoint = point;
        else
        {
            float distanceToAxis = GeometryUtils.DistanceToLine(point, axisPoint1, m_axisRenderer.m_axisData.GetDirection());
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
    * When the job of clipping has been done through threading, this method is called to generate objects and animations on axis inside the main GUI thread
    * **/
    public override void OnSymmetryDone()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;


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
        m_axisRenderer.OnPerformSymmetry();
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
}
