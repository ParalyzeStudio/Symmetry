using UnityEngine;
using System;
using System.Collections.Generic;
using ClipperLib;

public class ClippingBooleanOperations
{
    public const double CONVERSION_FLOAT_PRECISION = 1.0E4;

    public static IntPoint ConvertVector2ToIntPoint(Vector2 point)
    {
        return new IntPoint((Int64) Mathf.RoundToInt((float)(point.x * CONVERSION_FLOAT_PRECISION)), (Int64) Mathf.RoundToInt((float)(point.y * CONVERSION_FLOAT_PRECISION)));
    }

    public static Vector2 ConvertIntPointToVector2(IntPoint point)
    {
        return new Vector2(point.X / (float) CONVERSION_FLOAT_PRECISION, point.Y / (float) CONVERSION_FLOAT_PRECISION);
    }

    public static List<IntPoint> CreatePathFromShapeContour(List<Vector2> contour)
    {
        List<IntPoint> path = new List<IntPoint>();
        path.Capacity = contour.Count;
        for (int iContourPointIndex = 0; iContourPointIndex != contour.Count; iContourPointIndex++)
        {
            IntPoint pathPoint = ConvertVector2ToIntPoint(contour[iContourPointIndex]);
            path.Add(pathPoint);
        }

        return path;
    }

    public static List<Vector2> CreateContourFromPath(List<IntPoint> path)
    {
        List<Vector2> contour = new List<Vector2>();
        contour.Capacity = path.Count;
        for (int iPathPointIndex = 0; iPathPointIndex != path.Count; iPathPointIndex++)
        {
            Vector2 contourPoint = ConvertIntPointToVector2(path[iPathPointIndex]);
            contour.Add(contourPoint);
        }

        return contour;
    }

    public static Shape ShapesUnion(Shape subjShape, Shape clipShape)
    {
        //build subjs paths
        List<List<IntPoint> > subjsPaths = new List<List<IntPoint> >();

        //subj contour
        subjsPaths.Add(CreatePathFromShapeContour(subjShape.m_contour));

        //subj holes
        for (int iHoleIdx = 0; iHoleIdx != subjShape.m_holes.Count; iHoleIdx++)
        {
            subjsPaths.Add(CreatePathFromShapeContour(subjShape.m_holes[iHoleIdx]));
        }

        //build clips paths
        List<List<IntPoint>> clipsPaths = new List<List<IntPoint>>();
        
        //clip contour
        clipsPaths.Add(CreatePathFromShapeContour(clipShape.m_contour));

        //clip holes
        for (int iHoleIdx = 0; iHoleIdx != clipShape.m_holes.Count; iHoleIdx++)
        {
            clipsPaths.Add(CreatePathFromShapeContour(clipShape.m_holes[iHoleIdx]));
        }

        //Add subjs and clips paths to the clipper
        Clipper clipper = new Clipper();
        clipper.AddPaths(subjsPaths, PolyType.ptSubject, true);
        clipper.AddPaths(clipsPaths, PolyType.ptClip, true);

        //List<List<IntPoint>> solution = new List<List<IntPoint>>();
        //bool result = clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
        PolyTree polytree = new PolyTree();
        bool result = clipper.Execute(ClipType.ctUnion, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

        if (result)
        {
            //Only one child for the PolyTree node, this is the shape resulting from fusion
            PolyNode polynode = polytree.GetFirst();
            Shape shape = new Shape();

            //contour
            List<Vector2> shapeContour = CreateContourFromPath(polynode.Contour);
            shape.m_contour = shapeContour;

            //holes
            for (int iChildIdx = 0; iChildIdx != polynode.ChildCount; iChildIdx++)
            {
                PolyNode childHole = polynode.Childs[iChildIdx];
                shape.m_holes.Add(CreateContourFromPath(childHole.Contour));
            }

            return shape;
        }

        return null;
    }

    public static List<Shape> ShapesIntersection(Shape subjShape, Shape clipShape)
    {
        //build subjs paths
        List<List<IntPoint>> subjsPaths = new List<List<IntPoint>>();

        //subj contour
        subjsPaths.Add(CreatePathFromShapeContour(subjShape.m_contour));

        //subj holes
        for (int iHoleIdx = 0; iHoleIdx != subjShape.m_holes.Count; iHoleIdx++)
        {
            subjsPaths.Add(CreatePathFromShapeContour(subjShape.m_holes[iHoleIdx]));
        }

        //build clips paths
        List<List<IntPoint>> clipsPaths = new List<List<IntPoint>>();

        //clip contour
        clipsPaths.Add(CreatePathFromShapeContour(clipShape.m_contour));

        //clip holes
        for (int iHoleIdx = 0; iHoleIdx != clipShape.m_holes.Count; iHoleIdx++)
        {
            clipsPaths.Add(CreatePathFromShapeContour(clipShape.m_holes[iHoleIdx]));
        }

        //Add subjs and clips paths to the clipper
        Clipper clipper = new Clipper();
        clipper.AddPaths(subjsPaths, PolyType.ptSubject, true);
        clipper.AddPaths(clipsPaths, PolyType.ptClip, true);

        PolyTree polytree = new PolyTree();
        bool result = clipper.Execute(ClipType.ctIntersection, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

        List<Shape> resultingShapes = new List<Shape>();

        if (result)
        {
            int resultingShapeCount = 0;
            PolyNode polynode = polytree.GetFirst();
            while (polynode != null)
            {
                resultingShapeCount++;
                polynode = polynode.GetNext();
            }

            resultingShapes.Capacity = resultingShapeCount;

            while (polynode != null)
            {
                Shape shape = new Shape();

                //contour
                List<Vector2> shapeContour = CreateContourFromPath(polynode.Contour);
                shape.m_contour = shapeContour;

                //holes
                for (int iChildIdx = 0; iChildIdx != polynode.ChildCount; iChildIdx++)
                {
                    PolyNode childHole = polynode.Childs[iChildIdx];
                    shape.m_holes.Add(CreateContourFromPath(childHole.Contour));
                }

                resultingShapes.Add(shape);

                polynode = polynode.GetNext();
            }
        }

        return resultingShapes;
    }
}
