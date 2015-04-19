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

    public void Start()
    {

    }

    public void SymmetrizeByAxis()
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;

        //Extract, triangles
        List<List<ShapeTriangle>> leftTriangles, rightTriangles;
        ExtractTrianglesOnBothSidesOfAxis(out leftTriangles, out rightTriangles);

        //Rebuild shapes from those lists of triangles
        AxisRenderer axisRenderer = this.gameObject.GetComponentInChildren<AxisRenderer>();
        Vector3 axisCenter = axisRenderer.GetAxisCenterInWorldCoordinates();
        Vector3 axisDirection = axisRenderer.GetAxisDirection();
        
        if (leftTriangles.Count > 0)
        {
            for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != leftTriangles.Count; iTrianglesVecIdx++)
            {
                List<ShapeTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(leftTriangles[iTrianglesVecIdx], true);

                //the color of the new shape is the color of the old shape which can be retrieved from one of the ShapeTriangle objects
                Color shapeColor = reflectedTriangles[0].m_parentShape.m_color;

                Shape shapeData = new Shape();
                shapeData.SetShapeTriangles(reflectedTriangles);
                shapeData.m_color = ColorUtils.DarkenColor(shapeColor, 0.5f);
                shapeData.CalculateContour();
                shapeData.CalculateArea();
                GameObject newShapeObject = gameScene.m_shapes.CreateShapeObjectFromData(shapeData);
                ShapeAnimator shapeObjectAnimator = newShapeObject.GetComponent<ShapeAnimator>();
                shapeObjectAnimator.UpdatePivotPointPosition(axisCenter);
                shapeObjectAnimator.SetRotationAxis(axisDirection);
                shapeObjectAnimator.SetRotationAngle(90);
                shapeObjectAnimator.RotateTo(0, 0.5f);
                Color targetColor = ColorUtils.DarkenColor(shapeData.m_color, 0.1f);
                shapeObjectAnimator.SetColor(shapeData.m_color);
                shapeObjectAnimator.ColorChangeTo(targetColor, 0.5f);
            }
        }

        if (rightTriangles.Count > 0)
        {
            for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != rightTriangles.Count; iTrianglesVecIdx++)
            {
                List<ShapeTriangle> reflectedTriangles = CalculateTrianglesReflectionsByAxis(rightTriangles[iTrianglesVecIdx], false);

                //the color of the new shape is the color of the old shape which can be retrieved from one of the ShapeTriangle objects
                Color shapeColor = reflectedTriangles[0].m_parentShape.m_color;

                Shape shapeData = new Shape();
                shapeData.SetShapeTriangles(reflectedTriangles);
                shapeData.m_color = ColorUtils.DarkenColor(shapeColor, 0.5f);
                shapeData.CalculateContour();
                shapeData.CalculateArea();
                GameObject newShapeObject = gameScene.m_shapes.CreateShapeObjectFromData(shapeData);
                ShapeAnimator shapeObjectAnimator = newShapeObject.GetComponent<ShapeAnimator>();
                shapeObjectAnimator.UpdatePivotPointPosition(axisCenter);
                shapeObjectAnimator.SetRotationAxis(axisDirection);
                shapeObjectAnimator.SetRotationAngle(-90);
                shapeObjectAnimator.RotateTo(0, 0.5f);
                Color targetColor = ColorUtils.DarkenColor(shapeData.m_color, 0.1f);
                shapeObjectAnimator.SetColor(shapeData.m_color);
                shapeObjectAnimator.ColorChangeTo(targetColor, 0.5f);
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
    private void ExtractTrianglesOnBothSidesOfAxis(out List<List<ShapeTriangle>> leftTriangles, out List<List<ShapeTriangle>> rightTriangles)
    {
        AxisRenderer axisRenderer = this.gameObject.GetComponentInChildren<AxisRenderer>();
        Vector2 axisNormal = axisRenderer.GetAxisNormal(); //take the normal in clockwise order compared to the axisDirection

        ////First get all triangles
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        List<List<ShapeTriangle>> allShapeTriangles = new List<List<ShapeTriangle>>(); //the vector containing all triangles in the scene per shape
        allShapeTriangles.Capacity = gameScene.m_shapes.m_shapesObj.Count;

        for (int iShapeIndex = 0; iShapeIndex != gameScene.m_shapes.m_shapesObj.Count; iShapeIndex++)
        {
            Shape shape = gameScene.m_shapes.m_shapesObj[iShapeIndex].GetComponent<ShapeRenderer>().m_shape;
            allShapeTriangles.Add(shape.GetShapeTriangles());
        }

        //Find intersections of lines starting from axis endpoint directed by axisNormal with the grid box
        List<Vector2> line1GridBoxIntersections = FindLineGridBoxIntersections(axisRenderer.m_endpoint1GridPosition, axisNormal);
        List<Vector2> line2GridBoxIntersections = FindLineGridBoxIntersections(axisRenderer.m_endpoint2GridPosition, axisNormal);

        ////Extract triangles that are in the ribbon of the axis...
        //... on the left side of the line that passes by the axisStartPoint
        List<List<ShapeTriangle>> extractedTriangles = ExtractTrianglesOnLineSide(line1GridBoxIntersections[0], 
                                                                           line1GridBoxIntersections[1],
                                                                           allShapeTriangles,
                                                                           true);

        //... on the right side of the line that passes by the axisStartPoint
        extractedTriangles = ExtractTrianglesOnLineSide(line2GridBoxIntersections[0],
                                                        line2GridBoxIntersections[1],
                                                        extractedTriangles,
                                                        false);

        //... finally on the left and right side of the axis itself
        leftTriangles = ExtractTrianglesOnLineSide(axisRenderer.m_endpoint1GridPosition,
                                                   axisRenderer.m_endpoint2GridPosition,
                                                   extractedTriangles,
                                                   true);

        rightTriangles = ExtractTrianglesOnLineSide(axisRenderer.m_endpoint1GridPosition,
                                                    axisRenderer.m_endpoint2GridPosition,
                                                    extractedTriangles,
                                                    false);
    }

    /**
     * Extract all triangles from the list of triangles passed as parameters that are on one side of a line
     * The side of the line is determined when looking from the line start point to axis endpoint
     * If a triangle intersects the line, split it into smaller triangles
     * **/
    private List<List<ShapeTriangle>> ExtractTrianglesOnLineSide(Vector2 lineStartPoint, Vector2 lineEndPoint, List<List<ShapeTriangle>> triangles, bool bLeftSide)
    {
        List<List<ShapeTriangle>> extractedTriangles = new List<List<ShapeTriangle>>();

        for (int iTrianglesVecIdx = 0; iTrianglesVecIdx != triangles.Count; iTrianglesVecIdx++)
        {
            List<ShapeTriangle> trianglesVec = triangles[iTrianglesVecIdx];
            List<ShapeTriangle> extractedTrianglesVec = null;
            for (int iTriangleIndex = 0; iTriangleIndex != trianglesVec.Count; iTriangleIndex++)
            {
                ShapeTriangle triangle = trianglesVec[iTriangleIndex];
                if (triangle.IntersectsLine(lineStartPoint, lineEndPoint)) //find the intersection points and split the triangle accordingly
                {
                    ShapeTriangle[] splitTriangles;
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
                                extractedTrianglesVec = new List<ShapeTriangle>();
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
                            extractedTrianglesVec = new List<ShapeTriangle>();
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
     * linePoint has to be specified in grid coordinates
     * **/
    private List<Vector2> FindLineGridBoxIntersections(Vector2 linePoint, Vector2 lineDirection)
    {
        List<Vector2> intersections = new List<Vector2>();
        intersections.Capacity = 2;

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 gridTopLeft = new Vector2(1, gameScene.m_grid.m_numLines);
        Vector2 gridTopRight = new Vector2(gameScene.m_grid.m_numColumns, gameScene.m_grid.m_numLines);
        Vector2 gridBottomLeft = new Vector2(1, 1);
        Vector2 gridBottomRight = new Vector2(gameScene.m_grid.m_numColumns, 1);

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
    public List<ShapeTriangle> CalculateTrianglesReflectionsByAxis(List<ShapeTriangle> originalTriangles, bool bLeftSide)
    {
        List<ShapeTriangle> reflectedTriangles = new List<ShapeTriangle>();
        reflectedTriangles.Capacity = originalTriangles.Count;

        AxisRenderer axisRenderer = this.gameObject.GetComponentInChildren<AxisRenderer>();
        Vector2 axisDirection = axisRenderer.GetAxisDirection();
        Vector2 axisNormal = axisRenderer.GetAxisNormal();

        for (int iTriangleIndex = 0; iTriangleIndex != originalTriangles.Count; iTriangleIndex++)
        {
            ShapeTriangle originalTriangle = originalTriangles[iTriangleIndex];
            ShapeTriangle reflectedTriangle = new ShapeTriangle(originalTriangle.m_parentShape);

            for (int i = 0; i != 3; i++)
            {
                Vector2 originalVertex = originalTriangle.m_points[i];
                float distanceToAxis = GeometryUtils.DistanceToLine(originalVertex, axisRenderer.m_endpoint1GridPosition, axisDirection);
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
        if (strActionTag.Equals(GameScene.ACTION_TAG_SYMMETRY_AXES_ALL))
            return SymmetryType.SYMMETRY_AXES_ALL;
        else if (strActionTag.Equals(GameScene.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT))
            return SymmetryType.SYMMETRY_AXIS_DIAGONAL_LEFT;
        else if (strActionTag.Equals(GameScene.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT))
            return SymmetryType.SYMMETRY_AXIS_DIAGONAL_RIGHT;
        else if (strActionTag.Equals(GameScene.ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL))
            return SymmetryType.SYMMETRY_AXIS_HORIZONTAL;
        else if (strActionTag.Equals(GameScene.ACTION_TAG_SYMMETRY_AXIS_VERTICAL))
            return SymmetryType.SYMMETRY_AXIS_VERTICAL;
        else if (strActionTag.Equals(GameScene.ACTION_TAG_SYMMETRY_AXES_STRAIGHT))
            return SymmetryType.SYMMETRY_AXES_STRAIGHT;
        else if (strActionTag.Equals(GameScene.ACTION_TAG_SYMMETRY_AXES_DIAGONALS))
            return SymmetryType.SYMMETRY_AXES_DIAGONALS;
        else
            return SymmetryType.NONE;
    }
}