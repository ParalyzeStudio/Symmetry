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

    public static List<Shape> ShapesOperation(Shape subjShape, Shape clipShape, ClipType clipOperation)
    {
        //build subjs paths
        List<List<IntPoint>> subjsPaths = new List<List<IntPoint>>();

        //subj contour
        List<Vector2> contourWithOffset = subjShape.GetContourWithOffset();
        subjsPaths.Add(CreatePathFromShapeContour(contourWithOffset));

        //subj holes
        for (int iHoleIdx = 0; iHoleIdx != subjShape.m_holes.Count; iHoleIdx++)
        {
            subjsPaths.Add(CreatePathFromShapeContour(subjShape.m_holes[iHoleIdx]));
        }

        List<List<Vector2>> holesWithOffset = subjShape.GetHolesWithOffset();

        //build clips paths
        List<List<IntPoint>> clipsPaths = new List<List<IntPoint>>();

        //clip contour
        contourWithOffset = clipShape.GetContourWithOffset();
        clipsPaths.Add(CreatePathFromShapeContour(contourWithOffset));

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
        bool result = clipper.Execute(clipOperation, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

        List<Shape> resultingShapes = new List<Shape>();

        if (result)
        {
            resultingShapes.Capacity = polytree.Total;

            PolyNode polynode = polytree.GetFirst();
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

                //Triangulate shape
                shape.Triangulate();

                //Color
                Color shapeColor = Color.black;
                if (clipOperation == ClipType.ctUnion || clipOperation == ClipType.ctDifference)
                    shapeColor = subjShape.m_color;
                else if (clipOperation == ClipType.ctIntersection)
                    shapeColor = 0.5f * (subjShape.m_color + clipShape.m_color);

                shapeColor.a = Shapes.SHAPES_OPACITY;
                shape.m_color = shapeColor;

                shape.PropagateColorToTriangles();

                //populate the list
                resultingShapes.Add(shape);

                polynode = polynode.GetNext();
            }
        }

        return resultingShapes;
    }

    //public static Shape ShapesUnion(Shape subjShape, Shape clipShape)
    //{
    //    //build subjs paths
    //    List<List<IntPoint> > subjsPaths = new List<List<IntPoint> >();

    //    //subj contour
    //    subjsPaths.Add(CreatePathFromShapeContour(subjShape.m_contour));

    //    //subj holes
    //    for (int iHoleIdx = 0; iHoleIdx != subjShape.m_holes.Count; iHoleIdx++)
    //    {
    //        subjsPaths.Add(CreatePathFromShapeContour(subjShape.m_holes[iHoleIdx]));
    //    }

    //    //build clips paths
    //    List<List<IntPoint>> clipsPaths = new List<List<IntPoint>>();
        
    //    //clip contour
    //    clipsPaths.Add(CreatePathFromShapeContour(clipShape.m_contour));

    //    //clip holes
    //    for (int iHoleIdx = 0; iHoleIdx != clipShape.m_holes.Count; iHoleIdx++)
    //    {
    //        clipsPaths.Add(CreatePathFromShapeContour(clipShape.m_holes[iHoleIdx]));
    //    }

    //    //Add subjs and clips paths to the clipper
    //    Clipper clipper = new Clipper();
    //    clipper.AddPaths(subjsPaths, PolyType.ptSubject, true);
    //    clipper.AddPaths(clipsPaths, PolyType.ptClip, true);

    //    //List<List<IntPoint>> solution = new List<List<IntPoint>>();
    //    //bool result = clipper.Execute(ClipType.ctUnion, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
    //    PolyTree polytree = new PolyTree();
    //    bool result = clipper.Execute(ClipType.ctUnion, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

    //    if (result)
    //    {
    //        //Only one child for the PolyTree node, this is the shape resulting from fusion
    //        PolyNode polynode = polytree.GetFirst();
    //        Shape shape = new Shape();

    //        //contour
    //        List<Vector2> shapeContour = CreateContourFromPath(polynode.Contour);
    //        shape.m_contour = shapeContour;

    //        //holes
    //        for (int iChildIdx = 0; iChildIdx != polynode.ChildCount; iChildIdx++)
    //        {
    //            PolyNode childHole = polynode.Childs[iChildIdx];
    //            shape.m_holes.Add(CreateContourFromPath(childHole.Contour));
    //        }

    //        return shape;
    //    }

    //    return null;
    //}
}
