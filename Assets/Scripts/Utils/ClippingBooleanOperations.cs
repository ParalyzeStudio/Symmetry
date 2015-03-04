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

    public static Shape ShapesUnion(List<Shape> shapes)
    {
        //build subjs paths
        List<List<IntPoint> > subjsPaths = new List<List<IntPoint> >();
        for (int iShapeIdx = 0; iShapeIdx != shapes.Count; iShapeIdx++)
        {
            Shape shape = shapes[iShapeIdx];
            subjsPaths.Add(CreatePathFromShapeContour(shape.m_contour));
        }

        //Add subjs and clips paths to the clipper
        Clipper clipper = new Clipper();
        clipper.AddPaths(subjsPaths, PolyType.ptSubject, true);

        List<List<IntPoint>> solution = new List<List<IntPoint>>();
        bool result = clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

        if (result && solution.Count == 1)
        {
            List<IntPoint> singlePath = solution[0];
            List<Vector2> contour = CreateContourFromPath(singlePath);

            Shape shape = new Shape(contour, shapes[0].m_color);
            return shape;
        }

        return null;
    }
}
