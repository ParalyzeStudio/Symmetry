using UnityEngine;
using System;
using System.Collections.Generic;
using ClipperLib;

public class ClippingBooleanOperations
{   
    //private static List<IntPoint> CreatePathFromContour(Contour contour)
    //{
    //    List<IntPoint> path = new List<IntPoint>();
    //    path.Capacity = contour.Count;
    //    for (int iContourPointIndex = 0; iContourPointIndex != contour.Count; iContourPointIndex++)
    //    {
    //        IntPoint pathPoint = GeometryUtils.ConvertVector2ToIntPoint(contour[iContourPointIndex]);
    //        path.Add(pathPoint);
    //    }

    //    return path;
    //}

    //private static Contour CreateContourFromPath(List<IntPoint> path, bool bScalePoints = true)
    //{
    //    Contour contour = new Contour();
    //    contour.Capacity = path.Count;
    //    for (int iPathPointIndex = 0; iPathPointIndex != path.Count; iPathPointIndex++)
    //    {
    //        IntPoint pathPoint = path[iPathPointIndex];
    //        Vector2 contourPoint;
    //        if (bScalePoints)
    //            contourPoint = GeometryUtils.ConvertIntPointToVector2(pathPoint);
    //        else
    //            contourPoint = new Vector2(pathPoint.X, pathPoint.Y);
    //        contour.Add(contourPoint);
    //    }

    //    return contour;
    //}

    //public static List<Shape> ShapesOperation(Shape subjShape, Shape clipShape, ClipType clipOperation, Clipper clipperInstance = null)
    //{
    //    //build subjs paths
    //    List<List<IntPoint>> subjsPaths = new List<List<IntPoint>>();

    //    //subj contour
    //    Contour contourWithOffset = subjShape.GetContourWithOffset();
    //    subjsPaths.Add(CreatePathFromContour(contourWithOffset));

    //    //subj holes
    //    List<Contour> holesWithOffset = subjShape.GetHolesWithOffset();
    //    for (int iHoleIdx = 0; iHoleIdx != subjShape.m_holes.Count; iHoleIdx++)
    //    {
    //        subjsPaths.Add(CreatePathFromContour(holesWithOffset[iHoleIdx]));
    //    }

    //    //build clips paths
    //    List<List<IntPoint>> clipsPaths = new List<List<IntPoint>>();

    //    //clip contour
    //    contourWithOffset = clipShape.GetContourWithOffset();
    //    clipsPaths.Add(CreatePathFromContour(contourWithOffset));

    //    //clip holes
    //    for (int iHoleIdx = 0; iHoleIdx != clipShape.m_holes.Count; iHoleIdx++)
    //    {
    //        clipsPaths.Add(CreatePathFromContour(clipShape.m_holes[iHoleIdx]));
    //    }
                
    //    //Add subjs and clips paths to the clipper
    //    if (clipperInstance == null)
    //        clipperInstance = new Clipper();
        
    //    clipperInstance.AddPaths(subjsPaths, PolyType.ptSubject, true);
    //    clipperInstance.AddPaths(clipsPaths, PolyType.ptClip, true);
                
    //    PolyTree polytree = new PolyTree();
    //    bool result = clipperInstance.Execute(clipOperation, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

    //    List<Shape> resultingShapes = new List<Shape>();
    //    if (result)
    //    {
    //        resultingShapes.Capacity = polytree.Total;

    //        PolyNode polynode = polytree.GetFirst();
    //        while (polynode != null)
    //        {                
    //            //contour
    //            Contour polynodeContour = CreateContourFromPath(polynode.Contour, false);
    //            List<Contour> splitContours = polynodeContour.Split();

    //            //eliminate repeated points and down scale them
    //            for (int iContourIdx = 0; iContourIdx != splitContours.Count; iContourIdx++)
    //            {
    //                Contour splitContour = splitContours[iContourIdx];
    //                splitContour.RemoveAlignedVertices();
    //                splitContour.ScalePoints(1 / (float)GeometryUtils.CONVERSION_FLOAT_PRECISION);
    //            }

    //            if (splitContours.Count == 1) //only one shape add all holes to it
    //            {
    //                Shape shape = new Shape(false);
    //                shape.m_contour = splitContours[0];

    //                //child of an outer is always a hole, no need to call IsHole on the child
    //                for (int iChildIdx = 0; iChildIdx != polynode.ChildCount; iChildIdx++)
    //                {
    //                    PolyNode childHole = polynode.Childs[iChildIdx];
    //                    shape.m_holes.Add(CreateContourFromPath(childHole.Contour));
    //                }
    //            }

    //            List<Contour> pendingHoles = new List<Contour>();
    //            //Separate shapes from holes from the splitPaths list
    //            for (int iContourIdx = 0; iContourIdx != splitContours.Count; iContourIdx++)
    //            {
    //                Contour splitContour = splitContours[iContourIdx];

    //                float contourArea = splitContour.GetArea();

    //                if (contourArea > 0) //counter-clockwise orientation, it is a shape contour
    //                {
    //                    Shape shape = new Shape(false, splitContour);                        
    //                    shape.Triangulate(); //tmp triangulation, we will retriangulate if necessary if holes are added

    //                    resultingShapes.Add(shape);
    //                }
    //                else //a hole
    //                {
    //                    pendingHoles.Add(splitContour);
    //                }

    //                //Associated each pending hole to one shape
    //                for (int iPendingHoleIdx = 0; iPendingHoleIdx != pendingHoles.Count; iPendingHoleIdx++)
    //                {
    //                    Contour holeContour = pendingHoles[iPendingHoleIdx];
    //                    Vector2 holeBarycentre = holeContour.GetBarycentre();

    //                    for (int iResultingShapeIdx = 0; iResultingShapeIdx != resultingShapes.Count; iResultingShapeIdx++)
    //                    {
    //                        Shape resultingShape = resultingShapes[iResultingShapeIdx];
    //                        if (resultingShape.ContainsPoint(holeBarycentre))
    //                        {
    //                            resultingShape.m_holes.Add(holeContour);
    //                            resultingShape.Triangulate(); //re-triangulate
    //                        }
    //                    }
    //                }
    //            }

    //            polynode = polynode.GetNext();
    //        }
    //    }

    //    //Set color to each resulting shape
    //    for (int i = 0; i != resultingShapes.Count; i++)
    //    {
    //        if (clipOperation == ClipType.ctUnion)
    //            resultingShapes[i].m_color = subjShape.m_color;
    //        else if (clipOperation == ClipType.ctIntersection)
    //            resultingShapes[i].m_color = 0.5f * (subjShape.m_color + clipShape.m_color);
    //        else if (clipOperation == ClipType.ctDifference)
    //            resultingShapes[i].m_color = subjShape.m_color;
    //    }

    //    return resultingShapes;
    //}
}
