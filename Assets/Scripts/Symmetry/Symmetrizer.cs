using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Symmetrizer : MonoBehaviour 
{
    public enum SymmetryType
    {
        SYMMETRY_AXIS_90, //axis are either horizontal or vertical
        SYMMETRY_AXIS_45, //axis are 45 degrees diagonals
        SYMMETRY_POINT,
        SUBTRACTION_AXIS,
        SUBTRACTION_POINT
    };

    public SymmetryType m_type;

    private LevelManager m_levelManager;

    public void Start()
    {
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        m_levelManager = levelManagerObject.GetComponent<LevelManager>();
    }

    public void Symmetrize()
    {
        switch (m_type)
        {
            case SymmetryType.SYMMETRY_AXIS_90:
            case SymmetryType.SYMMETRY_AXIS_45:
                SymmetrizeByAxis();
                break;
            case SymmetryType.SYMMETRY_POINT:
                SymmetrizeByPoint();
                break;
            case SymmetryType.SUBTRACTION_AXIS:
                SymmetrizeSubtractionByAxis();
                break;
            case SymmetryType.SUBTRACTION_POINT:
                SymmetrizeSubtractionByPoint();
                break;
            default:
                break;
        }
    }

    public void SymmetrizeByAxis()
    {
        GameObject shapesObject = GameObject.FindGameObjectWithTag("Shapes");
        ShapeBuilder shapeBuilder = shapesObject.GetComponent<ShapeBuilder>();

        //Extract, triangles
        List<GridTriangle> leftTriangles, rightTriangles;
        ExtractTrianglesOnBothSidesOfAxis(out leftTriangles, out rightTriangles);

        //Rebuild shapes from those lists of triangles
        if (leftTriangles.Count > 0)
        {
            List<GridTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(leftTriangles, true);
            Shape shapeData = new Shape();
            shapeData.m_gridTriangles = reflectedTriangles;
            shapeData.m_color = new Color(1, 0, 0, 0.8f);
            GameObject newShapeObject = shapeBuilder.CreateFromShapeData(shapeData);
        }
        if (rightTriangles.Count > 0)
        {
            List<GridTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(rightTriangles, false);
            Shape shapeData = new Shape();
            shapeData.m_gridTriangles = reflectedTriangles;
            shapeData.m_color = new Color(1, 0, 0, 0.8f);
            GameObject newShapeObject = shapeBuilder.CreateFromShapeData(shapeData);
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
    private void ExtractTrianglesOnBothSidesOfAxis(out List<GridTriangle> leftTriangles, out List<GridTriangle> rightTriangles)
    {
        ////Build the 2 shapes that will acquire the newly created triangles
        Shape newShape1 = new Shape();
        Shape newShape2 = new Shape();

        AxisRenderer axisRenderer = this.gameObject.GetComponent<AxisRenderer>();
        Vector2 axisStartPoint = axisRenderer.m_endpoint1GridPosition;
        Vector2 axisEndPoint = axisRenderer.m_endpoint2GridPosition;
        Vector2 axisDirection = axisEndPoint - axisStartPoint;
        axisDirection.Normalize();
        Vector2 axisNormal = new Vector2(axisDirection.y, -axisDirection.x); //take the normal in clockwise order compared to the axisDirection

        ////First get all triangles
        GameObject shapesObj = GameObject.FindGameObjectWithTag("Shapes");
        ShapesHolder shapesHolder = shapesObj.GetComponent<ShapesHolder>();
        List<GameObject> allShapeObjects = shapesHolder.m_shapesObj;
        List<GridTriangle> allTriangles = new List<GridTriangle>();
        int trianglesListCapacity = 0;
        for (int iShapeIndex = 0; iShapeIndex != allShapeObjects.Count; iShapeIndex++)
        {
            Shape shape = allShapeObjects[iShapeIndex].GetComponent<ShapeRenderer>().m_shape;
            trianglesListCapacity += shape.m_gridTriangles.Count;
        }
        allTriangles.Capacity = trianglesListCapacity;

        for (int iShapeIndex = 0; iShapeIndex != allShapeObjects.Count; iShapeIndex++)
        {
            Shape shape = allShapeObjects[iShapeIndex].GetComponent<ShapeRenderer>().m_shape;
            List<GridTriangle> shapeTriangles = shape.m_gridTriangles;
            allTriangles.AddRange(shapeTriangles);
        }

        //Find intersections of lines starting from axis endpoint directed by axisNormal with the grid box
        List<Vector2> line1GridBoxIntersections = FindLineGridBoxIntersections(axisStartPoint, axisNormal);
        List<Vector2> line2GridBoxIntersections = FindLineGridBoxIntersections(axisEndPoint, axisNormal);

        ////Extract triangles that are in the ribbon of the axis...
        //... on the left side of the line that passes by the axisStartPoint
        List<GridTriangle> extractedTriangles = ExtractTrianglesOnLineSide(line1GridBoxIntersections[0], 
                                                                           line1GridBoxIntersections[1], 
                                                                           allTriangles,
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
    private List<GridTriangle> ExtractTrianglesOnLineSide(Vector2 lineStartPoint, Vector2 lineEndPoint, List<GridTriangle> triangles, bool bLeftSide)
    {
        List<GridTriangle> extractedTriangles = new List<GridTriangle>();

        for (int iTriangleIndex = 0; iTriangleIndex != triangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = triangles[iTriangleIndex];
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
                        extractedTriangles.Add(splitTriangles[i]);
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
                    extractedTriangles.Add(triangle);
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
}
