using UnityEngine;
using System;
using System.Collections.Generic;
using ClipperLib;

public class ClippingBooleanOperations
{
    public const double CONVERSION_FLOAT_PRECISION = 1.0E7;

    public static IntPoint ConvertVector2ToIntPoint(Vector2 point)
    {
        return new IntPoint((Int64) Mathf.RoundToInt((float)(point.x * CONVERSION_FLOAT_PRECISION)), (Int64) Mathf.RoundToInt((float)(point.y * CONVERSION_FLOAT_PRECISION)));
    }

    public static Vector2 ConvertIntPointToVector2(IntPoint point)
    {
        return new Vector2(point.X / (float) CONVERSION_FLOAT_PRECISION, point.Y / (float) CONVERSION_FLOAT_PRECISION);
    }

    public static List<IntPoint> CreatePathFromContour(Contour contour)
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

    public static Contour CreateContourFromPath(List<IntPoint> path, bool bScalePoints = true)
    {
        Contour contour = new Contour();
        contour.Capacity = path.Count;
        for (int iPathPointIndex = 0; iPathPointIndex != path.Count; iPathPointIndex++)
        {
            IntPoint pathPoint = path[iPathPointIndex];
            Vector2 contourPoint;
            if (bScalePoints)
                contourPoint = ConvertIntPointToVector2(pathPoint);
            else
                contourPoint = new Vector2(pathPoint.X, pathPoint.Y);
            contour.Add(contourPoint);
        }

        return contour;
    }

    public static List<Shape> ShapesOperation(Shape subjShape, Shape clipShape, ClipType clipOperation)
    {
        //build subjs paths
        List<List<IntPoint>> subjsPaths = new List<List<IntPoint>>();

        //subj contour
        Contour contourWithOffset = subjShape.GetContourWithOffset();
        subjsPaths.Add(CreatePathFromContour(contourWithOffset));

        //subj holes
        for (int iHoleIdx = 0; iHoleIdx != subjShape.m_holes.Count; iHoleIdx++)
        {
            subjsPaths.Add(CreatePathFromContour(subjShape.m_holes[iHoleIdx]));
        }

        List<Contour> holesWithOffset = subjShape.GetHolesWithOffset();

        //build clips paths
        List<List<IntPoint>> clipsPaths = new List<List<IntPoint>>();

        //clip contour
        contourWithOffset = clipShape.GetContourWithOffset();
        clipsPaths.Add(CreatePathFromContour(contourWithOffset));

        //clip holes
        for (int iHoleIdx = 0; iHoleIdx != clipShape.m_holes.Count; iHoleIdx++)
        {
            clipsPaths.Add(CreatePathFromContour(clipShape.m_holes[iHoleIdx]));
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
                //contour
                Contour polynodeContour = CreateContourFromPath(polynode.Contour, false);
                List<Contour> splitContours = polynodeContour.Split();

                //eliminate repeated points and down scale them
                for (int iContourIdx = 0; iContourIdx != splitContours.Count; iContourIdx++)
                {
                    Contour splitContour = splitContours[iContourIdx];
                    splitContour.RemoveAlignedVertices();
                    splitContour.ScalePoints(1 / (float)CONVERSION_FLOAT_PRECISION);
                }

                if (splitContours.Count == 1) //only one shape add all holes to it
                {
                    Shape shape = new Shape();
                    shape.m_contour = splitContours[0];

                    //child of an outer is always a hole, no need to call IsHole on the child
                    for (int iChildIdx = 0; iChildIdx != polynode.ChildCount; iChildIdx++)
                    {
                        PolyNode childHole = polynode.Childs[iChildIdx];
                        shape.m_holes.Add(CreateContourFromPath(childHole.Contour));
                    }
                }

                List<Contour> pendingHoles = new List<Contour>();
                //Separate shapes from holes from the splitPaths list
                for (int iContourIdx = 0; iContourIdx != splitContours.Count; iContourIdx++)
                {
                    Contour splitContour = splitContours[iContourIdx];

                    float contourArea = splitContour.GetArea();

                    if (contourArea > 0) //counter-clockwise orientation, it is a shape contour
                    {
                        Shape shape = new Shape(splitContour);

                        //Color
                        Color shapeColor = Color.black;
                        if (clipOperation == ClipType.ctUnion || clipOperation == ClipType.ctDifference)
                            shapeColor = subjShape.m_color;
                        else if (clipOperation == ClipType.ctIntersection)
                            shapeColor = 0.5f * (subjShape.m_color + clipShape.m_color);

                        shapeColor.a = Shapes.SHAPES_OPACITY;
                        shape.m_color = shapeColor;
                        shape.PropagateColorToTriangles();
                        shape.Triangulate(); //tmp triangulation, we will retriangulate if necessary if holes are added

                        resultingShapes.Add(shape);
                    }
                    else //a hole
                    {
                        pendingHoles.Add(splitContour);
                    }

                    //Associated each pending hole to one shape
                    for (int iPendingHoleIdx = 0; iPendingHoleIdx != pendingHoles.Count; iPendingHoleIdx++)
                    {
                        Contour holeContour = pendingHoles[iPendingHoleIdx];
                        Vector2 holeBarycentre = holeContour.GetBarycentre();

                        for (int iResultingShapeIdx = 0; iResultingShapeIdx != resultingShapes.Count; iResultingShapeIdx++)
                        {
                            Shape resultingShape = resultingShapes[iResultingShapeIdx];
                            if (resultingShape.ContainsGridPoint(holeBarycentre))
                            {
                                resultingShape.m_holes.Add(holeContour);
                                resultingShape.Triangulate(); //re-triangulate
                            }
                        }
                    }
                }

                polynode = polynode.GetNext();
            }
        }

        return resultingShapes;
    }

    

    //public static List<List<IntPoint>> SplitPath(List<IntPoint> path)
    //{
    //    List<List<IntPoint>> splitPaths = new List<List<IntPoint>>();

    //    bool bRepeatedVertices = false;
    //    while (path.Count > 0)
    //    {
    //        for (int i = 0; i != path.Count; i++)
    //        {
    //            bRepeatedVertices = false;

    //            IntPoint contourVertex = path[i];

    //            int farthestEqualVertexIndex = -1;
    //            for (int j = i + 1; j != path.Count; j++)
    //            {
    //                IntPoint contourTestVertex = path[j]; //the vertex to be test against contourVertex for equality

    //                if (contourTestVertex.Equals(contourVertex))
    //                    farthestEqualVertexIndex = j;
    //            }

    //            if (farthestEqualVertexIndex >= 0) //we found the same vertex at a different index
    //            {
    //                bRepeatedVertices = true;

    //                //extract the first split contour
    //                List<IntPoint> splitPath = new List<IntPoint>();
    //                splitPath.Capacity = path.Count - farthestEqualVertexIndex + i;
    //                for (int k = farthestEqualVertexIndex; k != path.Count; k++)
    //                {
    //                    splitPath.Add(path[k]);
    //                }
    //                for (int k = 0; k != i; k++)
    //                {
    //                    splitPath.Add(path[k]);
    //                }

    //                splitPaths.Add(splitPath);

    //                //replace the contour with the sub contour
    //                path = path.GetRange(i, farthestEqualVertexIndex - i);

    //                break; //break the for loop and continue on the while loop
    //            }
    //        }
    //        if (!bRepeatedVertices) //no repeated vertices in this contour, add it to split contours and break the while loop
    //        {
    //            splitPaths.Add(path);
    //            break;
    //        }
    //    }

    //    return splitPaths;
    //}

    //public static float ContourArea(Contour contour)
    //{
    //    int n = contour.Count;

    //    float A = 0.0f;

    //    for (int p = n - 1, q = 0; q < n; p = q++)
    //    {
    //        A += contour[p].x * contour[q].y - contour[q].x * contour[p].y;
    //    }
    //    return A * 0.5f;
    //}

    //public static Vector2 ContourBarycentre(Contour contour)
    //{
    //    Vector2 barycentre = Vector2.zero;
    //    for (int i = 0; i != contour.Count; i++)
    //    {
    //        barycentre += contour[i];
    //    }

    //    barycentre /= contour.Count;

    //    return barycentre;
    //}
}
