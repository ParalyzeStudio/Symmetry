using UnityEngine;
using System.Collections.Generic;

public class TriangleEdge
{
    public Vector2 m_pointA { get; set; }
    public Vector2 m_pointB { get; set; }

    public TriangleEdge(Vector2 pointA, Vector2 pointB)
    {
        m_pointA = pointA;
        m_pointB = pointB;
    }

    public bool Equals(TriangleEdge other)
    {
        return GeometryUtils.AreSegmentsEqual(this.m_pointA, this.m_pointB, other.m_pointA, other.m_pointB);
    }
}

public class BaseTriangle
{
    public Vector2[] m_points { get; set; }

    public BaseTriangle()
    {
        m_points = new Vector2[3];
    }

    public BaseTriangle(BaseTriangle other) : this()
    {
        for (int i = 0; i != 3; i++)
        {
            m_points[i] = other.m_points[i];
        }
    }

    public Vector2 GetCenter()
    {
        return (m_points[0] + m_points[1] + m_points[2]) / 3.0f;
    }
}

/**
 * Class that holds data for a triangle in grid coordinates (column, line)
 * **/
public class GridTriangle : BaseTriangle
{
    public GridTriangle() : base()
    {
        
    }

    public GridTriangle(GridTriangle other) : base(other)
    {
        
    }

    /**
    * Tells if one of the edges of this triangle intersects the contour passed as parameter
    **/
    public bool IntersectsContour(DottedOutline contour)
    {
        Contour contourPoints = contour.m_contour;
        for (int iContourPointIndex = 0; iContourPointIndex != contourPoints.Count; iContourPointIndex++)
        {
            Vector2 contourSegmentPoint1 = contourPoints[iContourPointIndex];
            Vector2 contourSegmentPoint2 = (iContourPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iContourPointIndex + 1];

            for (int iTriangleEdgeIndex = 0; iTriangleEdgeIndex != 3; iTriangleEdgeIndex++)
            {
                Vector2 triangleEdgePoint1 = m_points[iTriangleEdgeIndex];
                Vector2 triangleEdgePoint2 = (iTriangleEdgeIndex == 2) ? m_points[0] : m_points[iTriangleEdgeIndex + 1];

                if (GeometryUtils.TwoSegmentsIntersect(contourSegmentPoint1, contourSegmentPoint2, triangleEdgePoint1, triangleEdgePoint2))
                    return true;
            }
        }
        return false;
    }

    /**
    * Tells if one of the edges of this triangle intersects the segment defined by segmentPoint1 and segmentPoint2
    * **/
    public bool IntersectsSegment(Vector2 segmentPoint1, Vector2 segmentPoint2)
    {
        return GeometryUtils.TwoSegmentsIntersect(m_points[0], m_points[1], segmentPoint1, segmentPoint2) ||
               GeometryUtils.TwoSegmentsIntersect(m_points[1], m_points[2], segmentPoint1, segmentPoint2) ||
               GeometryUtils.TwoSegmentsIntersect(m_points[2], m_points[0], segmentPoint1, segmentPoint2);
    }

    /**
    * Tells if one of the edges of this triangle intersects the line defined by linePoint1 and linePoint2
    * **/
    public bool IntersectsLine(Vector2 linePoint1, Vector2 linePoint2)
    {
        return GeometryUtils.SegmentIntersectsLine(m_points[0], m_points[1], linePoint1, linePoint2) ||
               GeometryUtils.SegmentIntersectsLine(m_points[1], m_points[2], linePoint1, linePoint2) ||
               GeometryUtils.SegmentIntersectsLine(m_points[2], m_points[0], linePoint1, linePoint2);
    }

    /**
     * Tells if this triangle contains a point
     * **/
    public bool ContainsGridPoint(Vector2 gridPoint)
    {
        return GeometryUtils.IsInsideTriangle(gridPoint, m_points[0], m_points[1], m_points[2]);
    }

    /**
     * Tells if this triangle contains the triangle passed as parameter
     * **/
    public bool ContainsTriangle(GridTriangle triangle)
    {
        return this.ContainsGridPoint(triangle.m_points[0]) &&
               this.ContainsGridPoint(triangle.m_points[1]) &&
               this.ContainsGridPoint(triangle.m_points[2]);
    }

    /**
     * Tells if this triangle contains a whole shape
     * **/
    public bool ContainsShape(Shape shape)
    {
        for (int iTriangleIndex = 0; iTriangleIndex != shape.m_gridTriangles.Count; iTriangleIndex++)
        {
            GridTriangle triangle = shape.m_gridTriangles[iTriangleIndex];
            if (!this.ContainsTriangle(triangle)) //this triangle does not contains at least one of the shape triangles
                return false;
        }

        return true;
    }

    /**
     * Returns the index of one of the three triangle vertex if the point passed as parameter is equal to it 
     * Otherwise returns -1
     * **/
    public int PointEqualsVertex(Vector2 point)
    {
        if (MathUtils.AreVec2PointsEqual(point, m_points[0]))
            return 0;
        else if (MathUtils.AreVec2PointsEqual(point, m_points[1]))
            return 1;
        else if (MathUtils.AreVec2PointsEqual(point, m_points[2]))
            return 2;

        return -1;
    }

    /**
     * Calculates the area of the triangle
     * **/
    public float GetArea()
    {
        return 0.5f * Mathf.Abs(MathUtils.Determinant(m_points[0], m_points[1], m_points[2], false));
    }

    /**
     * Finds intersections between a line and the edges of this triangle
     * **/
    public List<Vector2> FindIntersectionsWithLine(Vector2 linePoint1, Vector2 linePoint2)
    {
        List<Vector2> intersections = new List<Vector2>();
        intersections.Capacity = 2;

        Vector2 lineDirection = linePoint2 - linePoint1;

        Vector2 intersection;
        bool intersects;
        GeometryUtils.SegmentLineIntersection(m_points[0], m_points[1], linePoint1, lineDirection, out intersection, out intersects);
        if (intersects)
            intersections.Add(intersection);

        GeometryUtils.SegmentLineIntersection(m_points[1], m_points[2], linePoint1, lineDirection, out intersection, out intersects);
        if (intersects)
        {
            if (intersections.Count == 0
                ||
                intersections.Count == 1 && !MathUtils.AreVec2PointsEqual(intersection, intersections[0]))
                intersections.Add(intersection);
        }


        if (intersections.Count < 2)
        {
            GeometryUtils.SegmentLineIntersection(m_points[2], m_points[0], linePoint1, lineDirection, out intersection, out intersects);
            if (intersects)
                intersections.Add(intersection);
        }

        return intersections;
    }

}

/**
 * Triangle that belongs to a Shape
 * **/
public class ShapeTriangle : GridTriangle
{
    public Shape m_parentShape { get; set; }
    public Color m_color { get; set; }

    public ShapeTriangle(Shape parentShape = null) : base()
    {
        m_parentShape = parentShape;
    }

    public ShapeTriangle(Shape parentShape, Color color) : base()
    {
        m_parentShape = parentShape;
        m_color = color;
    }

    /**
     * Splits this triangle intersected by a line
     * **/
    public void Split(Vector2 linePoint1, Vector2 linePoint2, out ShapeTriangle[] splitTriangles, out int splitTrianglesCount)
    {
        splitTriangles = new ShapeTriangle[3];
        splitTrianglesCount = 0;

        List<Vector2> intersections = FindIntersectionsWithLine(linePoint1, linePoint2);

        if (intersections.Count != 2)
            return;

        int intersection1IsTriangleVertex = PointEqualsVertex(intersections[0]);
        int intersection2IsTriangleVertex = PointEqualsVertex(intersections[1]);
        if (intersection1IsTriangleVertex >= 0 || intersection2IsTriangleVertex >= 0) //one of the intersection is equal to a triangle vertex
        {
            splitTriangles[0] = new ShapeTriangle(this.m_parentShape);
            splitTriangles[1] = new ShapeTriangle(this.m_parentShape);
            splitTrianglesCount = 2;

            if (intersection1IsTriangleVertex >= 0)
            {
                if (intersection1IsTriangleVertex == 0)
                {
                    splitTriangles[0].m_points[0] = intersections[0];
                    splitTriangles[0].m_points[1] = m_points[1];
                    splitTriangles[0].m_points[2] = intersections[1];

                    splitTriangles[1].m_points[0] = intersections[0];
                    splitTriangles[1].m_points[1] = intersections[1];
                    splitTriangles[1].m_points[2] = m_points[2];
                }
                else if (intersection1IsTriangleVertex == 1)
                {
                    splitTriangles[0].m_points[0] = intersections[0];
                    splitTriangles[0].m_points[1] = m_points[2];
                    splitTriangles[0].m_points[2] = intersections[1];

                    splitTriangles[1].m_points[0] = m_points[0];
                    splitTriangles[1].m_points[1] = intersections[0];
                    splitTriangles[1].m_points[2] = intersections[1];
                }
                else if (intersection1IsTriangleVertex == 2)
                {
                    splitTriangles[0].m_points[0] = intersections[0];
                    splitTriangles[0].m_points[1] = m_points[0];
                    splitTriangles[0].m_points[2] = intersections[1];

                    splitTriangles[1].m_points[0] = intersections[0];
                    splitTriangles[1].m_points[1] = intersections[1];
                    splitTriangles[1].m_points[2] = m_points[1];
                }
            }
            else
            {
                if (intersection2IsTriangleVertex == 0)
                {
                    splitTriangles[0].m_points[0] = intersections[1];
                    splitTriangles[0].m_points[1] = m_points[1];
                    splitTriangles[0].m_points[2] = intersections[0];

                    splitTriangles[1].m_points[0] = intersections[1];
                    splitTriangles[1].m_points[1] = intersections[0];
                    splitTriangles[1].m_points[2] = m_points[2];
                }
                else if (intersection2IsTriangleVertex == 1)
                {
                    splitTriangles[0].m_points[0] = intersections[1];
                    splitTriangles[0].m_points[1] = m_points[2];
                    splitTriangles[0].m_points[2] = intersections[0];

                    splitTriangles[1].m_points[0] = m_points[0];
                    splitTriangles[1].m_points[1] = intersections[1];
                    splitTriangles[1].m_points[2] = intersections[0];
                }
                else if (intersection2IsTriangleVertex == 2)
                {
                    splitTriangles[0].m_points[0] = intersections[1];
                    splitTriangles[0].m_points[1] = m_points[0];
                    splitTriangles[0].m_points[2] = intersections[0];

                    splitTriangles[1].m_points[0] = intersections[1];
                    splitTriangles[1].m_points[1] = intersections[0];
                    splitTriangles[1].m_points[2] = m_points[1];
                }
            }
        }
        else //intersections are strictly inside edges
        {
            splitTriangles[0] = new ShapeTriangle(this.m_parentShape);
            splitTriangles[1] = new ShapeTriangle(this.m_parentShape);
            splitTriangles[2] = new ShapeTriangle(this.m_parentShape);
            splitTrianglesCount = 3;

            //find edges on which intersection points are on
            int[] intersectionEdgesNumber = new int[2];

            bool isEdge1Intersected = false;
            bool isEdge2Intersected = false;
            //find where the first intersection is
            isEdge1Intersected = GeometryUtils.IsPointContainedInSegment(intersections[0], m_points[0], m_points[1]);
            if (isEdge1Intersected)
                intersectionEdgesNumber[0] = 1; //intersection is on the first edge of the triangle
            else
            {
                isEdge2Intersected = GeometryUtils.IsPointContainedInSegment(intersections[0], m_points[1], m_points[2]);
                if (isEdge2Intersected)
                    intersectionEdgesNumber[0] = 2; //intersection is on the second edge of the triangle
                else
                    intersectionEdgesNumber[0] = 3; //intersection is on the third edge of the triangle
            }

            //find where the second intersection is
            if (!isEdge1Intersected)
            {
                isEdge1Intersected = GeometryUtils.IsPointContainedInSegment(intersections[1], m_points[0], m_points[1]);
                if (isEdge1Intersected)
                    intersectionEdgesNumber[1] = 1; //intersection is on the first edge of the triangle
                else
                {
                    if (isEdge2Intersected)
                        intersectionEdgesNumber[1] = 3; //intersection is on the third edge of the triangle
                    else
                        intersectionEdgesNumber[1] = 2; //intersection is on the second edge of the triangle
                }
            }
            else
            {
                isEdge2Intersected = GeometryUtils.IsPointContainedInSegment(intersections[1], m_points[1], m_points[2]);
                if (isEdge2Intersected)
                    intersectionEdgesNumber[1] = 2; //intersection is on the second edge of the triangle
                else
                    intersectionEdgesNumber[1] = 3; //intersection is on the third edge of the triangle
            }

            if (intersectionEdgesNumber[1] < intersectionEdgesNumber[0])
            {
                Vector2 tmpIntersection = intersections[0];
                intersections[0] = intersections[1];
                intersections[1] = tmpIntersection;

                int tmpEdgeNumber = intersectionEdgesNumber[0];
                intersectionEdgesNumber[1] = intersectionEdgesNumber[0];
                intersectionEdgesNumber[0] = tmpEdgeNumber;
            }

            
            if (intersectionEdgesNumber[0] == 1 && intersectionEdgesNumber[1] == 2)
            {
                splitTriangles[0].m_points[0] = intersections[0];
                splitTriangles[0].m_points[1] = m_points[1];
                splitTriangles[0].m_points[2] = intersections[1];

                splitTriangles[1].m_points[0] = intersections[0];
                splitTriangles[1].m_points[1] = intersections[1];
                splitTriangles[1].m_points[2] = m_points[2];

                splitTriangles[2].m_points[0] = intersections[0];
                splitTriangles[2].m_points[1] = m_points[2];
                splitTriangles[2].m_points[2] = m_points[0];
            }
            else if (intersectionEdgesNumber[0] == 1 && intersectionEdgesNumber[1] == 3)
            {
                splitTriangles[0].m_points[0] = m_points[0];
                splitTriangles[0].m_points[1] = intersections[0];
                splitTriangles[0].m_points[2] = intersections[1];

                splitTriangles[1].m_points[0] = intersections[0];
                splitTriangles[1].m_points[1] = m_points[1];
                splitTriangles[1].m_points[2] = m_points[2];

                splitTriangles[2].m_points[0] = intersections[0];
                splitTriangles[2].m_points[1] = m_points[2];
                splitTriangles[2].m_points[2] = intersections[1];
            }
            else if (intersectionEdgesNumber[0] == 2 && intersectionEdgesNumber[1] == 3)
            {
                splitTriangles[0].m_points[0] = m_points[0];
                splitTriangles[0].m_points[1] = m_points[1];
                splitTriangles[0].m_points[2] = intersections[0];

                splitTriangles[1].m_points[0] = intersections[0];
                splitTriangles[1].m_points[1] = intersections[1];
                splitTriangles[1].m_points[2] = m_points[0];

                splitTriangles[2].m_points[0] = intersections[0];
                splitTriangles[2].m_points[1] = m_points[2];
                splitTriangles[2].m_points[2] = intersections[1];
            }
        }
    }
}

/**
 * Triangle used for drawing fancy backgrounds
 * It has a front and a back face with 2 different colors
 * **/
public class BackgroundTriangle : BaseTriangle
{
    public bool m_doubleSided { get; set; } //1 or 2 faces to render this triangle
    public Color m_frontColor { get; set; } //the color of the front face of this triangle (the 3 vertices share the same color)
    public Color m_backColor { get; set; } //the color of the back face of this triangle (the 3 vertices share the same color)
    public Color m_originalFrontColor { get; set; } //the color that this triangle face should have before being offset to m_frontColor
    public Color m_originalBackColor { get; set; } //the color that this triangle face should have before being offset to m_backColor
    private bool m_showingFrontFace; //is this triangle showing its front face
    private Vector3 m_flipAxis; //the axis the triangle is rotating around
    private float m_flipAngle; //the angle the triangle is rotated along its flip axis
    public int m_indexInColumn { get; set; }
    public BackgroundTriangleColumn m_parentColumn { get; set; }
    public float m_edgeLength { get; set; }
    public float m_angle { get; set; } //the angle this triangle is rotated

    //Variables to handle color animation of this triangle
    private Color m_frontFaceAnimationStartColor; //the color when the animation starts
    private Color m_backFaceAnimationStartColor; //the color when the animation starts
    private Color m_frontFaceAnimationEndColor; //the color when the animation ends
    private Color m_backFaceAnimationEndColor; //the color when the animation ends
    private bool m_frontFaceColorAnimating;//is this triangle currently animating its own color when switching its status
    private bool m_backFaceColorAnimating;//is this triangle currently animating its own color when switching its status
    private float m_frontFaceColorAnimationElapsedTime;
    private float m_frontFaceColorAnimationDuration;
    private float m_frontFaceColorAnimationDelay;
    private float m_backFaceColorAnimationElapsedTime;
    private float m_backFaceColorAnimationDuration;
    private float m_backFaceColorAnimationDelay;

    //Variables to handle flip animation of this triangle
    private bool m_flipping;
    private float m_flipToAngle;
    private float m_flipAnimationElapsedTime;
    private float m_flipAnimationDuration;
    private float m_flipAnimationDelay;

    //public BackgroundTriangle(BackgroundTriangle other) : base(other)
    //{
    //    m_frontColor = other.m_frontColor;
    //    m_backColor = other.m_backColor;
    //    m_originalFrontColor = other.m_originalFrontColor;
    //    m_originalBackColor = other.m_originalBackColor;
    //    m_indexInColumn = other.m_indexInColumn;
    //    m_parentColumn = other.m_parentColumn;

    //    m_colorAnimating = other.m_colorAnimating;
    //    m_colorAnimationElapsedTime = other.m_colorAnimationElapsedTime;
    //    m_colorAnimationDuration = other.m_colorAnimationDuration;
    //    m_colorAnimationDuration = other.m_colorAnimationDelay;
    //}

    /**
     * Build an equilateral triangle with the given orientation passed through the angle variable
     * position
     * **/
    public BackgroundTriangle(Vector2 position, float edgeLength, float angle, Color frontColor, Color backColor) : base()
    {
        m_edgeLength = edgeLength;
        m_angle = angle;

        float H = Mathf.Sqrt(3) / 2 * edgeLength * 1.0f;

        Vector2 bisector0 = new Vector2(1, 0); //angle 0
        Vector2 bisector1 = new Vector2(-0.5f, -Mathf.Sqrt(3) / 2); //angle -2 * PI / 3
        Vector2 bisector2 = new Vector2(-0.5f, Mathf.Sqrt(3) / 2); //angle 2 * PI / 3

        m_points[0] = 2 / 3.0f * H * bisector0;
        m_points[1] = 2 / 3.0f * H * bisector1;
        m_points[2] = 2 / 3.0f * H * bisector2;

        if (angle != 0)
        {
            for (int i = 0; i != 3; i++)
            {
                m_points[i] = Quaternion.AngleAxis(angle, Vector3.forward) * m_points[i];
            }
        }

        //Switch to global position
        for (int i = 0; i != 3; i++)
        {
            m_points[i] += position;
        }

        m_originalFrontColor = frontColor;
        m_originalBackColor = backColor;
        m_frontColor = frontColor;
        m_backColor = backColor;
    }

    /**
     * Returns the global index of this triangle inside the mesh
     * **/
    public int GetGlobalIndexInMesh()
    {
        BackgroundTrianglesRenderer parentRenderer = m_parentColumn.m_parentRenderer;

        int globalIndex = 0;
        for (int iColumnIdx = 0; iColumnIdx != m_parentColumn.m_index; iColumnIdx++)
        {
            globalIndex += parentRenderer.m_triangleColumns.Count;
        }

        globalIndex += m_indexInColumn;

        return globalIndex;
    }

    /**
     * Update the colors array of the parent renderer
     * **/
    public void UpdateParentRendererMeshColorsArray(bool bFrontFace = true, bool bBackFace = true)
    {
        int triangleGlobalIndex = m_parentColumn.m_index * m_parentColumn.m_parentRenderer.m_numTrianglesPerColumn + m_indexInColumn;

        if (!m_doubleSided)
        {
            m_parentColumn.m_parentRenderer.m_meshColors[3 * triangleGlobalIndex] = m_frontColor;
            m_parentColumn.m_parentRenderer.m_meshColors[3 * triangleGlobalIndex + 1] = m_frontColor;
            m_parentColumn.m_parentRenderer.m_meshColors[3 * triangleGlobalIndex + 2] = m_frontColor;
        }
        else
        {
            if (bFrontFace)
            {
                m_parentColumn.m_parentRenderer.m_meshColors[6 * triangleGlobalIndex] = m_frontColor;
                m_parentColumn.m_parentRenderer.m_meshColors[6 * triangleGlobalIndex + 1] = m_frontColor;
                m_parentColumn.m_parentRenderer.m_meshColors[6 * triangleGlobalIndex + 2] = m_frontColor;
            }
            if (bBackFace)
            {
                m_parentColumn.m_parentRenderer.m_meshColors[6 * triangleGlobalIndex + 3] = m_backColor;
                m_parentColumn.m_parentRenderer.m_meshColors[6 * triangleGlobalIndex + 4] = m_backColor;
                m_parentColumn.m_parentRenderer.m_meshColors[6 * triangleGlobalIndex + 5] = m_backColor;
            }
        }

        if (bFrontFace || bBackFace)
            m_parentColumn.m_parentRenderer.m_meshColorsDirty = true;
    }

    /**
     * Update the vertices array of the parent renderer
     * **/
    public void UpdateParentRendererMeshVerticesArray()
    {
        int triangleGlobalIndex = m_parentColumn.m_index * m_parentColumn.m_parentRenderer.m_numTrianglesPerColumn + m_indexInColumn;

        //transform each triangle points with the flip rotation
        Vector3 triangleCenter = this.GetCenter();
        Vector3 point0 = m_points[0];
        Vector3 point1 = m_points[1];
        Vector3 point2 = m_points[2];
        
        ////switch to local space
        //point0 -= triangleCenter;
        //point1 -= triangleCenter;
        //point2 -= triangleCenter;

        ////rotate points in local space
        //Quaternion triangleRotation = Quaternion.AngleAxis(m_flipAngle, m_flipAxis);

        //point0 = triangleRotation * point0;
        //point1 = triangleRotation * point1;
        //point2 = triangleRotation * point2;

        ////switch back to global space
        //point0 += triangleCenter;
        //point1 += triangleCenter;
        //point2 += triangleCenter;

        if (!m_doubleSided)
        {
            m_parentColumn.m_parentRenderer.m_meshVertices[3 * triangleGlobalIndex] = point0;
            m_parentColumn.m_parentRenderer.m_meshVertices[3 * triangleGlobalIndex + 1] = point1;
            m_parentColumn.m_parentRenderer.m_meshVertices[3 * triangleGlobalIndex + 2] = point2;
        }
        else
        {
            m_parentColumn.m_parentRenderer.m_meshVertices[6 * triangleGlobalIndex] = point0;
            m_parentColumn.m_parentRenderer.m_meshVertices[6 * triangleGlobalIndex + 1] = point1;
            m_parentColumn.m_parentRenderer.m_meshVertices[6 * triangleGlobalIndex + 2] = point2;
            m_parentColumn.m_parentRenderer.m_meshVertices[6 * triangleGlobalIndex + 3] = point0;
            m_parentColumn.m_parentRenderer.m_meshVertices[6 * triangleGlobalIndex + 4] = point2;
            m_parentColumn.m_parentRenderer.m_meshVertices[6 * triangleGlobalIndex + 5] = point1;
        }

        m_parentColumn.m_parentRenderer.m_meshVerticesDirty = true;
    }

    /**
     * Generate a color slightly different from original color
     * **/
    public void GenerateColorFromOriginalColor(bool bFrontFace, float delta)
    {
        if (bFrontFace)
            m_frontColor = ColorUtils.GetRandomNearColor(m_originalFrontColor, delta);
        else
            m_backColor = ColorUtils.GetRandomNearColor(m_originalBackColor, delta);
    }

    /**
     * Start the color animation process
     * **/
    public void StartColorAnimation(bool bFrontFace, Color toColor, float fDuration, float fDelay = 0.0f)
    {
        if (bFrontFace)
        {
            m_frontFaceAnimationStartColor = m_frontColor;
            m_frontFaceAnimationEndColor = toColor;
            m_frontFaceColorAnimationElapsedTime = 0;
            m_frontFaceColorAnimationDuration = fDuration;
            m_frontFaceColorAnimationDelay = fDelay;

            m_frontFaceColorAnimating = true;
        }
        else
        {
            m_backFaceAnimationStartColor = m_backColor;
            m_backFaceAnimationEndColor = toColor;
            m_backFaceColorAnimationElapsedTime = 0;
            m_backFaceColorAnimationDuration = fDuration;
            m_backFaceColorAnimationDelay = fDelay;

            m_backFaceColorAnimating = true;
        }
    }    

    /**
     * Animates the color. This function is called from the Update loop of the parent renderer (via the column)
     * **/
    public void AnimateColor(float dt)
    {
        if (m_frontFaceColorAnimating)
        {
            m_frontFaceColorAnimationElapsedTime += dt;

            if (m_frontFaceColorAnimationElapsedTime < m_frontFaceColorAnimationDelay)
                return;

            if (m_frontFaceColorAnimationElapsedTime >= m_frontFaceColorAnimationDuration + m_frontFaceColorAnimationDelay)
            {
                m_frontFaceColorAnimating = false;
                m_frontColor = m_frontFaceAnimationEndColor;
            }
            else
            {
                float timeRatio = (m_frontFaceColorAnimationElapsedTime - m_frontFaceColorAnimationDelay) / m_frontFaceColorAnimationDuration;
                m_frontColor = Color.Lerp(m_frontFaceAnimationStartColor, m_frontFaceAnimationEndColor, timeRatio);
            }

            UpdateParentRendererMeshColorsArray(true, false);
        }
        else if (m_backFaceColorAnimating)
        {
            m_backFaceColorAnimationElapsedTime += dt;

            if (m_backFaceColorAnimationElapsedTime < m_backFaceColorAnimationDelay)
                return;

            if (m_backFaceColorAnimationElapsedTime >= m_backFaceColorAnimationDuration + m_backFaceColorAnimationDelay)
            {
                m_backFaceColorAnimating = false;
                m_backColor = m_backFaceAnimationEndColor;
            }
            else
            {
                float timeRatio = (m_backFaceColorAnimationElapsedTime - m_backFaceColorAnimationDelay) / m_backFaceColorAnimationDuration;
                m_backColor = Color.Lerp(m_backFaceAnimationStartColor, m_backFaceAnimationEndColor, timeRatio);
            }

            UpdateParentRendererMeshColorsArray(false, true);
        }
    }

    /**
     * Start the flipping animation process
     * **/
    public void StartFlipAnimation(Vector2 axis, float fDuration, float fDelay = 0.0f)
    {
        m_flipping = true;
        m_flipAxis = axis;
        m_flipToAngle = (m_flipAngle == 0) ? 180 : 360;
        m_flipAnimationElapsedTime = 0;
        m_flipAnimationDuration = fDuration;
        m_flipAnimationDelay = fDelay;

        //m_flipAngle = 90;
        //m_flipAxis = new Vector3(1, 0, 0);
        //UpdateParentRendererMeshVerticesArray();
    }

    public void FlipAnimate(float dt)
    {
        if (!m_flipping)
            return;

        m_flipAnimationElapsedTime += dt;

        if (m_flipAnimationElapsedTime < m_flipAnimationDelay)
            return;

        if (m_flipAnimationElapsedTime >= m_flipAnimationDuration + m_flipAnimationDelay)
        {
            m_flipping = false;
            m_flipAngle = (m_flipToAngle == 360) ? 0 : m_flipToAngle;
            m_showingFrontFace = (m_flipAngle == 0);
        }
        else
        {
            float deltaAngle = (dt / m_flipAnimationDuration) * 180.0f;
            m_flipAngle += deltaAngle;
        }

        UpdateParentRendererMeshVerticesArray();
    }

    public void Offset(float dy)
    {
        Vector2 offset = new Vector2(0, dy);

        m_points[0] += offset;
        m_points[1] += offset;
        m_points[2] += offset;
    }
}