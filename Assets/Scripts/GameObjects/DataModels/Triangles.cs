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
    * Calculates the barycentre of this triangle
    * **/
    public Vector2 GetBarycentre()
    {
        return (m_points[0] + m_points[1] + m_points[2]) / 3.0f;
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
 * **/
public class BackgroundTriangle : BaseTriangle
{
    private Color m_color; //the color of this triangle (the 3 vertices share the same color)
    public Color m_originalColor { get; set; } //the color that this triangle should have before being offset to m_color
    public int m_indexInColumn { get; set; }
    public BackgroundTriangleColumn m_parentColumn { get; set; }
    public bool m_hidden { get; set; } //is this triangle hidden? (i.e replaced by a triangle of the color of default background)
    public bool m_animationPending { get; set; } //does this triangle is waiting to switch from shown to hidden status (or vice versa)

    //Variables to handle this triangle as a column leader
    public bool m_leader { get; set; }
    public int m_leaderStopIndex { get; set; }
    public Color m_leaderTargetColor { get; set; } //the color the triangles behind their leader have to reach

    public BackgroundTriangle(BackgroundTriangle other) : base(other)
    {
        m_color = other.m_color;
        m_originalColor = other.m_originalColor;
        m_indexInColumn = other.m_indexInColumn;
        m_parentColumn = other.m_parentColumn;
        m_hidden = other.m_hidden;
        m_animationPending = other.m_animationPending;

        m_leader = other.m_leader;
        m_leaderStopIndex = other.m_leaderStopIndex;
        m_leaderTargetColor = other.m_leaderTargetColor;
    }

    /**
     * Build an equilateral triangle with the given orientation passed through the angle variable
     * position
     * **/
    public BackgroundTriangle(Vector2 position, float edgeLength, float angle, Color color) : base()
    {
        float H = Mathf.Sqrt(3) / 2 * edgeLength;

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

        m_originalColor = color;
        m_color = color;

        m_hidden = false;
        m_animationPending = false;
    }

    public void UpdateMeshData(ref Vector2[] vertices, ref int[] triangles, ref Color[] colors)
    {
        int triangleGlobalIndex = m_parentColumn.m_index * m_parentColumn.Count + m_indexInColumn;

        //vertices
        if (vertices != null)
        {
            vertices[3 * triangleGlobalIndex] = m_points[0];
            vertices[3 * triangleGlobalIndex + 1] = m_points[1];
            vertices[3 * triangleGlobalIndex + 2] = m_points[2];
        }

        //indices
        if (triangles != null)
        {
            triangles[3 * triangleGlobalIndex] = 3 * triangleGlobalIndex;
            triangles[3 * triangleGlobalIndex + 1] = 3 * triangleGlobalIndex + 1;
            triangles[3 * triangleGlobalIndex + 2] = 3 * triangleGlobalIndex + 2;
        }

        //colors
        if (colors != null)
        {
            colors[3 * triangleGlobalIndex] = m_color;
            colors[3 * triangleGlobalIndex + 1] = m_color;
            colors[3 * triangleGlobalIndex + 2] = m_color;
        }
    }

    /**
     * Returns the leader this triangle is following
     * **/
    public BackgroundTriangle GetLeader()
    {
        if (this.m_leader)
            return this;

        for (int iTriangleIdx = m_indexInColumn + 1; iTriangleIdx != m_parentColumn.Count; iTriangleIdx++)
        {
            if (m_parentColumn[iTriangleIdx].m_leader)
                return m_parentColumn[iTriangleIdx];
        }

        return null;
    }

    public void GenerateColorFromOriginalColor()
    {
        m_color = ColorUtils.GetRandomNearColor(m_originalColor, 0.1f);
    }

    public Color GetColor()
    {
        return (m_hidden) ? m_parentColumn.m_parentRenderer.GetDefaultBackgroundColor() : m_color;
    }
}