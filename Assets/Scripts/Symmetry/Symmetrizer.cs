using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Symmetrizer : MonoBehaviour 
{
    public enum SymmetryType
    {
        NONE,
        SYMMETRY_AXIS_HORIZONTAL, //axis are horizontal
        SYMMETRY_AXIS_VERTICAL, //axis are vertical
        SYMMETRY_AXES_STRAIGHT, //axis are either horizontal or vertical
        SYMMETRY_AXIS_DIAGONAL_LEFT, //axis is diagonal (45 degrees) passing through top left hand corner
        SYMMETRY_AXIS_DIAGONAL_RIGHT, //axis is diagonal (45 degrees) passing through bottom left hand corner
        SYMMETRY_AXES_DIAGONALS, //both diagonals
        SYMMETRY_AXES_ALL, //both straight and diagonal axes
        SYMMETRY_POINT,
        SUBTRACTION_AXIS,
        SUBTRACTION_POINT
    };
    //public const int SYMMETRY_AXIS_HORIZONTAL           = 0x00000001;
    //public const int SYMMETRY_AXIS_VERTICAL             = 0x00000010;
    //public const int SYMMETRY_AXIS_STRAIGHT             = 0x00000011; //SYMMETRY_AXIS_HORIZONTAL && SYMMETRY_AXIS_VERTICAL
    //public const int SYMMETRY_AXIS_DIAGONAL_TOP_LEFT    = 0x00000100;
    //public const int SYMMETRY_AXIS_DIAGONAL_BOTTOM_LEFT = 0x00001000;
    //public const int SYMMETRY_AXIS_DIAGONALS            = 0x00001100; //SYMMETRY_AXIS_DIAGONAL_TOP_LEFT && SYMMETRY_AXIS_DIAGONAL_BOTTOM_LEFT
    //public const int SYMMETRY_AXIS_ALL                  = 0x00001111; //SYMMETRY_AXIS_STRAIGHT && SYMMETRY_AXIS_DIAGONALS

    //public SymmetryType m_type;

    //private LevelManager m_levelManager;

    public void Start()
    {
        //GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        //m_levelManager = levelManagerObject.GetComponent<LevelManager>();
    }

    public void SymmetrizeByAxis()
    {
        GameObject shapesObject = GameObject.FindGameObjectWithTag("Shapes");
        ShapeBuilder shapeBuilder = shapesObject.GetComponent<ShapeBuilder>();

        //Extract, triangles
        List<List<GridTriangle> > leftTriangles, rightTriangles;
        ExtractTrianglesOnBothSidesOfAxis(out leftTriangles, out rightTriangles);

        //Rebuild shapes from those lists of triangles
        AxisRenderer axisRenderer = this.gameObject.GetComponent<AxisRenderer>();
        Vector3 axisCenter = axisRenderer.GetAxisCenterInWorldCoordinates();
        Vector3 axisDirection = axisRenderer.GetAxisDirection();
        
        if (leftTriangles.Count > 0)
        {
            for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != leftTriangles.Count; iTrianglesVecIdx++)
            {
                List<GridTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(leftTriangles[iTrianglesVecIdx], true);

                Shape shapeData = new Shape();
                shapeData.m_gridTriangles = reflectedTriangles;
                shapeData.m_color = new Color(1, 0, 0, 1);
                shapeData.CalculateContour();
                shapeData.CalculateArea();
                GameObject newShapeObject = shapeBuilder.CreateFromShapeData(shapeData);
                GameObjectAnimator shapeObjectAnimator = newShapeObject.GetComponent<GameObjectAnimator>();
                shapeObjectAnimator.UpdatePivotPointPosition(axisCenter);
                //shapeObjectAnimator.OnRotationChanged(90, axisDirection);
                //shapeObjectAnimator.RotateFromToAroundAxis(90, 0, axisDirection, 0.5f);
                shapeObjectAnimator.SetRotationAngle(90);
                shapeObjectAnimator.SetRotationAxis(axisDirection);
                shapeObjectAnimator.RotateTo(0, 0.5f);
            }
        }

        if (rightTriangles.Count > 0)
        {
            for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != leftTriangles.Count; iTrianglesVecIdx++)
            {
                List<GridTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(rightTriangles[iTrianglesVecIdx], false);
                Shape shapeData = new Shape();
                shapeData.m_gridTriangles = reflectedTriangles;
                shapeData.m_color = new Color(1, 0, 0, 1);
                shapeData.CalculateContour();
                shapeData.CalculateArea();
                GameObject newShapeObject = shapeBuilder.CreateFromShapeData(shapeData);
                GameObjectAnimator shapeObjectAnimator = newShapeObject.GetComponent<GameObjectAnimator>();
                shapeObjectAnimator.UpdatePivotPointPosition(axisCenter);
                //shapeObjectAnimator.OnRotationChanged(90, axisDirection);
                //shapeObjectAnimator.RotateFromToAroundAxis(90, 0, axisDirection, 0.5f);
                shapeObjectAnimator.SetRotationAngle(90);
                shapeObjectAnimator.SetRotationAxis(axisDirection);
                shapeObjectAnimator.RotateTo(0, 0.5f);
            }
        }
    }

    public void SymmetrizeByPoint()
    {

    }

    public void SymmetrizeSubtractionByAxis()
    {

    }

    public void SymmetrizeSubtractionByPoint()
    {

    }

    /**
     * Creates a copy of all shape triangles on both sides of the axis of symmetry axis into 2 newly created shapes 
     * that will be symmetrized.
     * **/
    private void ExtractTrianglesOnBothSidesOfAxis(out List<List<GridTriangle> > leftTriangles, out List<List<GridTriangle> > rightTriangles)
    {
        AxisRenderer axisRenderer = this.gameObject.GetComponent<AxisRenderer>();
        Vector2 axisStartPoint = axisRenderer.m_endpoint1GridPosition;
        Vector2 axisEndPoint = axisRenderer.m_endpoint2GridPosition;
        Vector2 axisDirection = axisEndPoint - axisStartPoint;
        axisDirection.Normalize();
        Vector2 axisNormal = new Vector2(axisDirection.y, -axisDirection.x); //take the normal in clockwise order compared to the axisDirection

        ////First get all triangles
        GameObject shapesObj = GameObject.FindGameObjectWithTag("Shapes");
        ShapesHolder shapesHolder = shapesObj.GetComponent<ShapesHolder>();
        ShapeRenderer[] shapeRenderers = shapesHolder.GetComponentsInChildren<ShapeRenderer>();
        List<List<GridTriangle>> allShapeTriangles = new List<List<GridTriangle>>(); //the vector containing all triangles in the scene per shape
        allShapeTriangles.Capacity = shapeRenderers.Length;

        for (int iShapeIndex = 0; iShapeIndex != shapeRenderers.Length; iShapeIndex++)
        {
            Shape shape = shapeRenderers[iShapeIndex].m_shape;
            List<GridTriangle> shapeTriangles = shape.m_gridTriangles;
            allShapeTriangles.Add(shapeTriangles);
        }

        //Find intersections of lines starting from axis endpoint directed by axisNormal with the grid box
        List<Vector2> line1GridBoxIntersections = FindLineGridBoxIntersections(axisStartPoint, axisNormal);
        List<Vector2> line2GridBoxIntersections = FindLineGridBoxIntersections(axisEndPoint, axisNormal);

        ////Extract triangles that are in the ribbon of the axis...
        //... on the left side of the line that passes by the axisStartPoint
        List<List<GridTriangle>> extractedTriangles = ExtractTrianglesOnLineSide(line1GridBoxIntersections[0], 
                                                                           line1GridBoxIntersections[1],
                                                                           allShapeTriangles,
                                                                           true);

        //... on the right side of the line that passes by the axisStartPoint
        extractedTriangles = ExtractTrianglesOnLineSide(line2GridBoxIntersections[0],
                                                        line2GridBoxIntersections[1],
                                                        extractedTriangles,
                                                        false);

        //... finally on the left and right side of the axis itself
        leftTriangles = ExtractTrianglesOnLineSide(axisStartPoint,
                                                   axisEndPoint,
                                                   extractedTriangles,
                                                   true);

        rightTriangles = ExtractTrianglesOnLineSide(axisStartPoint,
                                                    axisEndPoint,
                                                    extractedTriangles,
                                                    false);
    }

    /**
     * Extract all triangles from the list of triangles passes as parameters that are on one side of a line
     * The side of the line is determined when looking from the line start point to axis endpoint
     * If a triangle intersects the line, split it into smaller triangles
     * **/
    private List<List<GridTriangle> > ExtractTrianglesOnLineSide(Vector2 lineStartPoint, Vector2 lineEndPoint, List<List<GridTriangle> > triangles, bool bLeftSide)
    {
        List<List<GridTriangle> > extractedTriangles = new List<List<GridTriangle>>();

        for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != triangles.Count; iTrianglesVecIdx++)
        {
            List<GridTriangle> trianglesVec = triangles[iTrianglesVecIdx];
            List<GridTriangle> extractedTrianglesVec = null;
            for (int iTriangleIndex = 0; iTriangleIndex != trianglesVec.Count; iTriangleIndex++)
            {
                GridTriangle triangle = trianglesVec[iTriangleIndex];
                if (triangle.IntersectsLine(lineStartPoint, lineEndPoint)) //find the intersection points and split the triangle accordingly
                {
                    GridTriangle[] splitTriangles;
                    int splitTrianglesCount;
                    triangle.Split(lineStartPoint, lineEndPoint, out splitTriangles, out splitTrianglesCount);

                    for (int i = 0; i != splitTrianglesCount; i++)
                    {
                        Vector2 triangleBarycentre = splitTriangles[i].GetBarycentre();
                        float barycentreDet = MathUtils.Determinant(lineStartPoint, lineEndPoint, triangleBarycentre, false);

                        //the triangle barycentre is on the side of the line we want
                        if (barycentreDet > 0 && bLeftSide
                            ||
                            barycentreDet < 0 && !bLeftSide)
                        {
                            if (extractedTrianglesVec == null)
                            {
                                extractedTrianglesVec = new List<GridTriangle>();
                                extractedTriangles.Add(extractedTrianglesVec);
                            }
                            extractedTrianglesVec.Add(splitTriangles[i]);
                        }
                    }
                }
                else
                {
                    Vector2 triangleBarycentre = triangle.GetBarycentre();
                    float barycentreDet = MathUtils.Determinant(lineStartPoint, lineEndPoint, triangleBarycentre, false);

                    //the triangle barycentre is on the side of the line we want
                    if (barycentreDet > 0 && bLeftSide
                        ||
                        barycentreDet < 0 && !bLeftSide)
                    {
                        if (extractedTrianglesVec == null)
                        {
                            extractedTrianglesVec = new List<GridTriangle>();
                            extractedTriangles.Add(extractedTrianglesVec);
                        }
                        extractedTrianglesVec.Add(triangle);
                    }
                }
            }
        }

        return extractedTriangles;
    }


    /**
     * Find all intersections between a line and the box determined by grid boundaries
     * **/
    private List<Vector2> FindLineGridBoxIntersections(Vector2 linePoint, Vector2 lineDirection)
    {
        List<Vector2> intersections = new List<Vector2>();
        intersections.Capacity = 2;

        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        GridBuilder gridBuilder = gridObject.GetComponent<GridBuilder>();
        Vector2 gridTopLeft = new Vector2(1, gridBuilder.m_numLines);
        Vector2 gridTopRight = new Vector2(gridBuilder.m_numColumns, gridBuilder.m_numLines);
        Vector2 gridBottomLeft = new Vector2(1, 1);
        Vector2 gridBottomRight = new Vector2(gridBuilder.m_numColumns, 1);

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
    public List<GridTriangle> CalculateTrianglesReflectionsByAxis(List<GridTriangle> originalTriangles, bool bLeftSide)
    {
        List<GridTriangle> reflectedTriangles = new List<GridTriangle>();
        reflectedTriangles.Capacity = originalTriangles.Count;

        AxisRenderer axisRenderer = this.gameObject.GetComponent<AxisRenderer>();
        Vector2 axisStartPoint = axisRenderer.m_endpoint1GridPosition;
        Vector2 axisEndPoint = axisRenderer.m_endpoint2GridPosition;
        Vector2 axisDirection = axisEndPoint - axisStartPoint;
        axisDirection.Normalize();
        Vector2 axisNormal = new Vector2(axisDirection.y, -axisDirection.x);

        for (int iTriangleIndex = 0; iTriangleIndex != originalTriangles.Count; iTriangleIndex++)
        {
            GridTriangle originalTriangle = originalTriangles[iTriangleIndex];
            GridTriangle reflectedTriangle = new GridTriangle();

            for (int i = 0; i != 3; i++)
            {
                Vector2 originalVertex = originalTriangle.m_points[i];
                float distanceToAxis = GeometryUtils.DistanceToLine(originalVertex, axisStartPoint, axisDirection);
                Vector2 reflectedVertex = originalVertex + (bLeftSide ? 1 : -1) * axisNormal * 2 * distanceToAxis;

                //place the reflected vertex in correct order to produce a ccw triangle (invert last two vertices)
                if (i == 1)
                    reflectedTriangle.m_points[2] = reflectedVertex;
                else if (i == 2)
                    reflectedTriangle.m_points[1] = reflectedVertex;
                else
                    reflectedTriangle.m_points[0] = reflectedVertex;

            }

            reflectedTriangles.Add(reflectedTriangle);
        }

        return reflectedTriangles;
    }

    public SymmetryType GetSymmetryTypeForActionTag(string strActionTag)
    {
        if (strActionTag.Equals(GameHUD.ACTION_TAG_SYMMETRY_AXES_ALL))
            return SymmetryType.SYMMETRY_AXES_ALL;
        else if (strActionTag.Equals(GameHUD.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT))
            return SymmetryType.SYMMETRY_AXIS_DIAGONAL_LEFT;
        else if (strActionTag.Equals(GameHUD.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT))
            return SymmetryType.SYMMETRY_AXIS_DIAGONAL_RIGHT;
        else if (strActionTag.Equals(GameHUD.ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL))
            return SymmetryType.SYMMETRY_AXIS_HORIZONTAL;
        else if (strActionTag.Equals(GameHUD.ACTION_TAG_SYMMETRY_AXIS_VERTICAL))
            return SymmetryType.SYMMETRY_AXIS_VERTICAL;
        else if (strActionTag.Equals(GameHUD.ACTION_TAG_SYMMETRY_AXES_STRAIGHT))
            return SymmetryType.SYMMETRY_AXES_STRAIGHT;
        else if (strActionTag.Equals(GameHUD.ACTION_TAG_SYMMETRY_AXES_DIAGONALS))
            return SymmetryType.SYMMETRY_AXES_DIAGONALS;
        else
            return SymmetryType.NONE;
    }
}