using UnityEngine;
using System.Collections.Generic;

public class PointSymmetrizer : Symmetrizer
{
    private SymmetryPoint m_symmetryPoint;

    private List<Shape> m_pointClippedInterShapes;
    private List<Shape> m_pointClippedDiffShapes;
    private List<Axis> m_symmetrizedAxes;

    public override void Init()
    {
        base.Init();

        m_symmetryPoint = this.GetComponent<SymmetryPoint>();
    }
    
    /**
     * Symmetrize shapes and axes by a point
     * **/
    protected override void PerformSymmetry()
    {
        //Symmetrize shapes
        List<Shape> shapes = m_gameScene.m_shapesHolder.m_shapes;
        m_pointClippedInterShapes = new List<Shape>(10);
        m_pointClippedDiffShapes = new List<Shape>(10);

        for (int i = 0; i != shapes.Count; i++)
        {
            Shape shape = shapes[i];

            if (shape.m_state != Shape.ShapeState.STATIC)
                continue;

            Shape symmetricShape = CalculateSymmetricShape(shape);
            symmetricShape.Triangulate();

            List<Shape> clippedInterShapes, clippedDiffShapes;
            m_clippingManager.ClipAgainstStaticShapes(symmetricShape, out clippedInterShapes, out clippedDiffShapes);
            m_pointClippedInterShapes.AddRange(clippedInterShapes);
            m_pointClippedDiffShapes.AddRange(clippedDiffShapes);
        }

        //Symmetrize axes
        List<AxisRenderer> axes = m_gameScene.m_axesHolder.m_axes;
        m_symmetrizedAxes = new List<Axis>();

        for (int i = 0; i != axes.Count; i++)
        {
            Axis axis = axes[i].m_axisData;
            //if (axis.m_type == Axis.AxisType.STATIC_PENDING)
            //{
            m_symmetrizedAxes.Add(CalculateSymmetricAxis(axis));
            //}
        }
    }

    /**
     * Return a Shape that is symmetric of the parameter 'shapeToSymmetrize' about this symmetry point
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
                symmetricHole.Add(CalculateSymmetricPoint(hole[j]));
            }
            symmetricHoles.Add(symmetricHole);
        }

        Shape symmetricShape = new Shape(symmetricContour, symmetricHoles);
        symmetricShape.m_tint = shapeToSymmetrize.m_tint;
        symmetricShape.m_color = shapeToSymmetrize.m_color;
        return symmetricShape;
    }

    /**
     * Return the symmetric edge of the parameter 'edge' about this symmetry point
     * **/
    public override GridEdge CalculateSymmetricEdge(GridEdge edge)
    {
        GridPoint symmetricPointA = CalculateSymmetricPoint(edge.m_pointA);
        GridPoint symmetricPointB = CalculateSymmetricPoint(edge.m_pointB);

        return new GridEdge(symmetricPointA, symmetricPointB);
    }

    /**
     * Return the point symmetric of the parameter 'point' about this symmetry point
     * **/
    public override GridPoint CalculateSymmetricPoint(GridPoint point)
    {
        GridPoint symmetryPointPosition = m_symmetryPoint.GetCircleGridPosition();

        //Calculate the distance between the point and the symmetry point
        GridPoint pointToSymmetryPoint = (symmetryPointPosition - point);

        return symmetryPointPosition + pointToSymmetryPoint;
    }

    /**
     * Calculate the symmetric axis of this axis by the parameter 'axis'
     * **/
    public Axis CalculateSymmetricAxis(Axis axis)
    {
        GridPoint symmetricAxisPointA = CalculateSymmetricPoint(axis.m_pointA);
        GridPoint symmetricAxisPointB = CalculateSymmetricPoint(axis.m_pointB);

        Axis symmetricAxis = new Axis(symmetricAxisPointA, symmetricAxisPointB, Axis.AxisType.STATIC_PENDING, axis.m_symmetryType);

        return symmetricAxis;
    }
    
    /**
    * When the job of clipping has been done through threading, this method is called to generate objects and animations on axis inside the main GUI thread
    * **/
    public override void OnSymmetryDone()
    {
        Shapes shapesHolder = m_gameScene.m_shapesHolder;

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

        //build the axes
        if (m_symmetrizedAxes != null)
        {
            Axes axesHolder = m_gameScene.m_axesHolder;
            for (int p = 0; p != m_symmetrizedAxes.Count; p++)
            {
                Axis axis = m_symmetrizedAxes[p];
                axesHolder.BuildAxis(axis);
            }
        }

        m_symmetryPoint.OnPerformSymmetry();
    }
}
