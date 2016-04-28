using UnityEngine;
using System.Collections.Generic;
using ClipperLib;

public class ClippingManager : MonoBehaviour
{
    private Clipper m_clipper;
    private Shapes m_shapesHolder;
    
    public void Init()
    {
        m_clipper = new Clipper();
        m_shapesHolder = ((GameScene) this.GetComponent<SceneManager>().m_currentScene).m_shapesHolder;
    }

    private List<IntPoint> CreatePathFromContour(Contour contour)
    {
        List<IntPoint> path = new List<IntPoint>();
        path.Capacity = contour.Count;
        for (int iContourPointIndex = 0; iContourPointIndex != contour.Count; iContourPointIndex++)
        {
            IntPoint pathPoint = contour[iContourPointIndex];
            path.Add(pathPoint);
        }

        return path;
    }

    private Contour CreateContourFromPath(List<IntPoint> path)
    {
        Contour contour = new Contour();
        contour.Capacity = path.Count;
        for (int iPathPointIndex = 0; iPathPointIndex != path.Count; iPathPointIndex++)
        {
            IntPoint pathPoint = path[iPathPointIndex];
            GridPoint contourPoint;
            contourPoint = new GridPoint((int)pathPoint.X, (int)pathPoint.Y, false);
            contour.Add(contourPoint);
        }

        return contour;
    }

    public List<Shape> ShapesOperation(Shape subjShape, Shape clipShape, ClipType clipOperation, bool bNewPolygonSets = true)
    {
        if (bNewPolygonSets)
        {
            //build subjs paths
            List<List<IntPoint>> subjsPaths = new List<List<IntPoint>>();

            //subj contour
            subjsPaths.Add(CreatePathFromContour(subjShape.m_contour));

            //subj holes
            List<Contour> holesWithOffset = subjShape.m_holes;
            for (int iHoleIdx = 0; iHoleIdx != holesWithOffset.Count; iHoleIdx++)
            {
                subjsPaths.Add(CreatePathFromContour(holesWithOffset[iHoleIdx]));
            }

            //build clips paths
            List<List<IntPoint>> clipsPaths = new List<List<IntPoint>>();

            //clip contour
            clipsPaths.Add(CreatePathFromContour(clipShape.m_contour));

            //clip holes
            holesWithOffset = clipShape.m_holes;
            for (int iHoleIdx = 0; iHoleIdx != holesWithOffset.Count; iHoleIdx++)
            {
                clipsPaths.Add(CreatePathFromContour(holesWithOffset[iHoleIdx]));
            }

            //clear the clipper paths and refill them after that
            m_clipper.Clear();

            //Add subjs and clips paths to the clipper
            m_clipper.AddPaths(subjsPaths, PolyType.ptSubject, true);
            m_clipper.AddPaths(clipsPaths, PolyType.ptClip, true);
        }
        //else do nothing, paths are in place just clip them

        PolyTree polytree = new PolyTree();
        m_clipper.StrictlySimple = true; //we want simple polygons only
        bool result = m_clipper.Execute(clipOperation, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

        List<Shape> resultingShapes = new List<Shape>();
        if (result)
        {
            resultingShapes = ExtractShapesFromPolyNodes(polytree.Childs);
            
            //Triangulate each shape
            for (int iShapeIdx = 0; iShapeIdx != resultingShapes.Count; iShapeIdx++)
            {
                resultingShapes[iShapeIdx].Triangulate();
            }

            //Set color to each resulting shape
            for (int i = 0; i != resultingShapes.Count; i++)
            {
                if (clipOperation == ClipType.ctUnion)
                {
                    resultingShapes[i].m_tint = subjShape.m_tint;
                    resultingShapes[i].m_color = subjShape.m_color;
                }
                else if (clipOperation == ClipType.ctIntersection)
                {
                    if (clipShape.m_state == Shape.ShapeState.TILED_BACKGROUND) //do not mix color when clipping with background
                    {
                        resultingShapes[i].m_tint = subjShape.m_tint;
                        resultingShapes[i].m_color = subjShape.m_color;
                    }
                    else
                    {
                        resultingShapes[i].m_tint = 0.5f * (subjShape.m_tint + clipShape.m_tint);
                        resultingShapes[i].CalculateColorFromTint();
                    }
                }
                else if (clipOperation == ClipType.ctDifference)
                {
                    resultingShapes[i].m_tint = subjShape.m_tint;
                    resultingShapes[i].m_color = subjShape.m_color;
                }
            }
        }
        return resultingShapes;
    }

    /**
     * Extract all outers from a PolyNode object and convert them to Shape objects
     * **/
    private List<Shape> ExtractShapesFromPolyNodes(List<PolyNode> polynodes)
    {
        List<Shape> resultingShapes = new List<Shape>();

        for (int iPolynodeIdx = 0; iPolynodeIdx != polynodes.Count; iPolynodeIdx++)
        {
            PolyNode polynode = polynodes[iPolynodeIdx];

            if (polynode.IsHole) //be sure we are dealing with outers
                continue;

            //we specify the polygons to be strictly simple so no need to split them manually
            Contour shapeContour = CreateContourFromPath(polynode.Contour);
            Shape shape = new Shape(shapeContour);
            resultingShapes.Add(shape); //Add this extracted shape to the list of all resulting shapes

            //child of an outer is always a hole, no need to call IsHole on the child
            List<PolyNode> childHoles = polynode.Childs;
            for (int iHoleIdx = 0; iHoleIdx != childHoles.Count; iHoleIdx++)
            {
                shape.m_holes.Add(CreateContourFromPath(childHoles[iHoleIdx].Contour));
            }           

            //Call recursively the ExtractShapesFromPolyNodes() method on hole Childs (that are outers)
            for (int iHoleIdx = 0; iHoleIdx != childHoles.Count; iHoleIdx++)
            {
                if (childHoles[iHoleIdx].ChildCount > 0)
                    resultingShapes.AddRange(ExtractShapesFromPolyNodes(childHoles[iHoleIdx].Childs));
            }
        }

        List<Shape> finalShapes = new List<Shape>();
        //We need to do some work on the shapes:
        // -remove aligned vertices
        // -split them, as 1 single shape with contour + holes can be seen as more independent shapes
        for (int i = 0; i != resultingShapes.Count; i++)
        {
            Shape shape = resultingShapes[i];
            shape.RemoveAlignedVertices();
            List<Shape> simpleShapes = shape.SplitIntoSimpleShapes();
            if (simpleShapes == null) //no split occured just add the original shape
                finalShapes.Add(shape);
            else
                finalShapes.AddRange(simpleShapes);

        }

        return finalShapes;
    }

    //private List<Contour> ExtractHolesForPolynode(PolyNode polynode)
    //{
    //    List<PolyNode> childHoles = polynode.Childs;
    //    List<Contour> holesContour = new List<Contour>();

    //    for (int iHoleIdx = 0; iHoleIdx != childHoles.Count; iHoleIdx++)
    //    {
    //        //Split each hole
    //        List<List<IntPoint>> splitHoles = SplitPath(childHoles[iHoleIdx].Contour);
    //        for (int iSplitHoleIdx = 0; iSplitHoleIdx != splitHoles.Count; iSplitHoleIdx++)
    //        {
    //            Contour holeContour = CreateContourFromPath(splitHoles[iSplitHoleIdx], true);
    //            //remove possible duplicate points
    //            holeContour.RemoveDuplicateVertices();
    //            if (holeContour.Count < 3)
    //                continue;
    //            //remove aligned vertices
    //            holeContour.RemoveAlignedVertices();
    //            if (holeContour.Count < 3)
    //                continue;
    //            holesContour.Add(holeContour);
    //        }
    //    }

    //    return holesContour;
    //}

    /**
     * Clip a shape against all the static shapes
     * Set bDynamicClipping to true if shapes will be swept by a line after symmetry is done
     * if shapes are clipped when player drags a shape on the grid set it to false
     * **/
    public void ClipAgainstStaticShapes(Shape subjShape, out List<Shape> clippedInterShapes, out List<Shape> clippedDiffShapes, bool bDynamicClipping = true)
    {
        List<Shape> allShapes = m_shapesHolder.m_shapes;
        clippedDiffShapes = new List<Shape>(10); //build a list with big enough capacity to store result of clipping on subjShape
        clippedInterShapes = new List<Shape>(10);

        //Before any clipping with static shapes, we need to find the intersection with the tiled background shape
        Shape tiledBackgroundShape = m_shapesHolder.GetTiledBackgroundShape();
        List<Shape> resultShapes = ShapesOperation(subjShape, tiledBackgroundShape, ClipType.ctIntersection);
        //set their state to MOVING_SUBSTITUTION_DIFFERENCE so if there is no static shape in the scene the clippedDiffShapes list can be directly used
        for (int i = 0; i != resultShapes.Count; i++)
        {
            resultShapes[i].m_state = Shape.ShapeState.MOVING_SUBSTITUTION_DIFFERENCE;
        }
        clippedDiffShapes.AddRange(resultShapes);

        for (int i = 0; i != allShapes.Count; i++)
        {
            Shape shape = allShapes[i];

            if (shape.m_state != Shape.ShapeState.STATIC)
                continue;

            List<Shape> clippedDifferenceShapes = new List<Shape>(10);
            for (int j = 0; j != clippedDiffShapes.Count; j++)
            {
                Shape clipShape = clippedDiffShapes[j];
                if (clipShape.OverlapsShape(shape, true))
                {
                    //difference
                    List<Shape> differenceShapes = ShapesOperation(clipShape, shape, ClipperLib.ClipType.ctDifference);
                    for (int k = 0; k != differenceShapes.Count; k++)
                    {
                        if (bDynamicClipping)
                            differenceShapes[k].m_state = Shape.ShapeState.DYNAMIC_DIFFERENCE;
                        else
                            differenceShapes[k].m_state = Shape.ShapeState.MOVING_SUBSTITUTION_DIFFERENCE;
                        differenceShapes[k].m_tint = clipShape.m_tint; //same color as original
                        differenceShapes[k].m_color = clipShape.m_color; //same color as original
                    }
                    clippedDifferenceShapes.AddRange(differenceShapes);

                    //intersection
                    List<Shape> intersectionShapes = new List<Shape>(5);
                    if (differenceShapes.Count == 0) //no difference, so intersection is the full clipShape
                    {
                        clipShape.m_tint = 0.5f * (clipShape.m_tint + shape.m_tint);
                        clipShape.CalculateColorFromTint();
                        intersectionShapes.Add(clipShape);
                    }
                    else
                    {
                        intersectionShapes.AddRange(ShapesOperation(clipShape, shape, ClipperLib.ClipType.ctIntersection, false)); //same polygon sets
                    }

                    for (int k = 0; k != intersectionShapes.Count; k++)
                    {
                        if (bDynamicClipping)
                            intersectionShapes[k].m_state = Shape.ShapeState.DYNAMIC_INTERSECTION;
                        else
                            intersectionShapes[k].m_state = Shape.ShapeState.MOVING_SUBSTITUTION_INTERSECTION;

                        shape.AddOverlappingShape(intersectionShapes[k]);
                    }
                    clippedInterShapes.AddRange(intersectionShapes);
                }
                else //no intersection, add the full clipShape to clippedDifferenceShapes
                {
                    if (bDynamicClipping)
                        clipShape.m_state = Shape.ShapeState.DYNAMIC_DIFFERENCE;
                    else
                        clipShape.m_state = Shape.ShapeState.MOVING_SUBSTITUTION_DIFFERENCE;
                    clippedDifferenceShapes.Add(clipShape);
                }
            }

            clippedDiffShapes.Clear();
            clippedDiffShapes.AddRange(clippedDifferenceShapes);
        }
    }

    /**
     * Perform a difference clipping operation between one subj shape and a set of clip shapes
     * **/
    public List<Shape> PerformDifferenceAgainstShapes(Shape subjShape, List<Shape> clipShapes)
    {
        List<Shape> clippedDiffShapes = new List<Shape>(10); //build a list with big enough capacity to store result of clipping on subjShape
        clippedDiffShapes.Add(subjShape);

        for (int i = 0; i != clipShapes.Count; i++)
        {
            Shape shape = clipShapes[i];
            {
                List<Shape> clippedDifferenceShapes = new List<Shape>(10);
                for (int j = 0; j != clippedDiffShapes.Count; j++)
                {
                    Shape clipShape = clippedDiffShapes[j];
                    if (clipShape.OverlapsShape(shape, true))
                    {
                        //difference
                        List<Shape> differenceShapes = ShapesOperation(clipShape, shape, ClipperLib.ClipType.ctDifference);
                        for (int k = 0; k != differenceShapes.Count; k++)
                        {
                            differenceShapes[k].m_color = clipShape.m_color; //same color as original
                        }
                        clippedDifferenceShapes.AddRange(differenceShapes);
                    }
                    else //no intersection, add the full clipShape to clippedDifferenceShapes
                    {
                        clippedDifferenceShapes.Add(clipShape);
                    }
                }

                clippedDiffShapes.Clear();
                clippedDiffShapes.AddRange(clippedDifferenceShapes);
            }            
        }

        return clippedDiffShapes;
    }

    /**
     * Ensure that the path has no vertices that repeat.
     * If at least 2 vertices repeat in that contour, split it into several contours
    * **/
    //private List<List<IntPoint>> SplitPath(List<IntPoint> path)
    //{
    //    List<List<IntPoint>> splitPaths = new List<List<IntPoint>>();
    //    splitPaths.Add(path);
    //    return splitPaths;

    //    //return Clipper.SimplifyPolygon(path);

    //    //List<List<IntPoint>> splitPaths = new List<List<IntPoint>>();

    //    //bool bRepeatedVertices = false;
    //    //while (path.Count > 0)
    //    //{
    //    //    for (int i = 0; i != path.Count; i++)
    //    //    {
    //    //        bRepeatedVertices = false;

    //    //        IntPoint pathVertex = path[i];

    //    //        int farthestEqualVertexIndex = -1;
    //    //        for (int j = i + 1; j != path.Count; j++)
    //    //        {
    //    //            IntPoint pathTestVertex = path[j]; //the vertex to be test against contourVertex for equality

    //    //            if (pathTestVertex.Equals(pathVertex))
    //    //                farthestEqualVertexIndex = j;
    //    //        }

    //    //        if (farthestEqualVertexIndex >= 0) //we found the same vertex at a different index
    //    //        {
    //    //            bRepeatedVertices = true;

    //    //            //extract the first split contour
    //    //            List<IntPoint> splitPath = new List<IntPoint>();
    //    //            splitPath.Capacity = path.Count - farthestEqualVertexIndex + i;
    //    //            for (int k = farthestEqualVertexIndex; k != path.Count; k++)
    //    //            {
    //    //                splitPath.Add(path[k]);
    //    //            }
    //    //            for (int k = 0; k != i; k++)
    //    //            {
    //    //                splitPath.Add(path[k]);
    //    //            }

    //    //            if (splitPath.Count > 2)
    //    //                splitPaths.Add(splitPath);

    //    //            //replace the contour with the sub contour
    //    //            path = new List<IntPoint>(path.GetRange(i, farthestEqualVertexIndex - i));

    //    //            break; //break the for loop and continue on the while loop
    //    //        }
    //    //    }
    //    //    if (!bRepeatedVertices) //no repeated vertices in this contour, add it to split contours and break the while loop
    //    //    {
    //    //        if (path.Count > 2)
    //    //            splitPaths.Add(path);
    //    //        break;
    //    //    }
    //    //}

    //    //return splitPaths;
    //}
}
