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
            IntPoint pathPoint = GeometryUtils.ConvertVector2ToIntPoint(contour[iContourPointIndex]);
            path.Add(pathPoint);
        }

        return path;
    }

    private Contour CreateContourFromPath(List<IntPoint> path, bool bScalePoints = true)
    {
        Contour contour = new Contour();
        contour.Capacity = path.Count;
        for (int iPathPointIndex = 0; iPathPointIndex != path.Count; iPathPointIndex++)
        {
            IntPoint pathPoint = path[iPathPointIndex];
            Vector2 contourPoint;
            if (bScalePoints)
                contourPoint = GeometryUtils.ConvertIntPointToVector2(pathPoint);
            else
                contourPoint = new Vector2(pathPoint.X, pathPoint.Y);
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
            Contour contourWithOffset = subjShape.GetContourWithOffset();
            subjsPaths.Add(CreatePathFromContour(contourWithOffset));

            //subj holes
            List<Contour> holesWithOffset = subjShape.GetHolesWithOffset();
            for (int iHoleIdx = 0; iHoleIdx != subjShape.m_holes.Count; iHoleIdx++)
            {
                subjsPaths.Add(CreatePathFromContour(holesWithOffset[iHoleIdx]));
            }

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

            //clear the clipper paths and refill them after that
            m_clipper.Clear();

            //Add subjs and clips paths to the clipper
            m_clipper.AddPaths(subjsPaths, PolyType.ptSubject, true);
            m_clipper.AddPaths(clipsPaths, PolyType.ptClip, true);
        }
        //else do nothing, paths are in place just clip them

        PolyTree polytree = new PolyTree();
        bool result = m_clipper.Execute(clipOperation, polytree, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

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
                    splitContour.ScalePoints(1 / (float)GeometryUtils.CONVERSION_FLOAT_PRECISION);
                }

                if (splitContours.Count == 1) //only one shape add all holes to it
                {
                    Shape shape = new Shape(false);
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
                        Shape shape = new Shape(false, splitContour);
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
                            if (resultingShape.ContainsPoint(holeBarycentre))
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

        //Set color to each resulting shape
        for (int i = 0; i != resultingShapes.Count; i++)
        {
            if (clipOperation == ClipType.ctUnion)
                resultingShapes[i].m_color = subjShape.m_color;
            else if (clipOperation == ClipType.ctIntersection)
                resultingShapes[i].m_color = 0.5f * (subjShape.m_color + clipShape.m_color);
            else if (clipOperation == ClipType.ctDifference)
                resultingShapes[i].m_color = subjShape.m_color;
        }
        return resultingShapes;
    }

    /**
     * Clip a shape against all the static shapes
     * **/
    public void ClipAgainstStaticShapes(Shape subjShape, out List<Shape> clippedInterShapes, out List<Shape> clippedDiffShapes)
    {
        List<Shape> allShapes = m_shapesHolder.m_shapes;
        clippedDiffShapes = new List<Shape>(10); //build a list with big enough capacity to store result of clipping on subjShape
        clippedInterShapes = new List<Shape>(10);
        clippedDiffShapes.Add(subjShape);

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
                        differenceShapes[k].m_state = Shape.ShapeState.DYNAMIC_DIFFERENCE;
                        differenceShapes[k].m_color = clipShape.m_color; //same color as original
                    }
                    clippedDifferenceShapes.AddRange(differenceShapes);

                    //intersection
                    List<Shape> intersectionShapes = new List<Shape>(5);
                    if (differenceShapes.Count == 0) //no difference, so intersection is the full clipShape
                    {
                        clipShape.m_color = 0.5f * (clipShape.m_color + shape.m_color);
                        intersectionShapes.Add(clipShape);
                    }
                    else
                    {
                        intersectionShapes.AddRange(ShapesOperation(clipShape, shape, ClipperLib.ClipType.ctIntersection, false)); //same polygon sets
                    }

                    for (int k = 0; k != intersectionShapes.Count; k++)
                    {
                        intersectionShapes[k].m_state = Shape.ShapeState.DYNAMIC_INTERSECTION;
                        intersectionShapes[k].m_overlappedStaticShape = shape;
                    }
                    clippedInterShapes.AddRange(intersectionShapes);
                }
                else //no intersection, add the full clipShape to clippedDifferenceShapes
                {
                    clipShape.m_state = Shape.ShapeState.DYNAMIC_DIFFERENCE;
                    clippedDifferenceShapes.Add(clipShape);
                }
            }

            clippedDiffShapes.Clear();
            clippedDiffShapes.AddRange(clippedDifferenceShapes);
        }
    }
}
