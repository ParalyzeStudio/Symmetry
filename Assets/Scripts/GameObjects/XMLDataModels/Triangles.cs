﻿using UnityEngine;
using System.Collections.Generic;

/**
 * Class that holds data for a triangle in grid coordinates (column, line)
 * **/
public class GridTriangle
{
    public Vector2[] m_points { get; set; }

    public GridTriangle()
    {
        m_points = new Vector2[3];
    }

    public bool IntersectsContour(Contour contour)
    {
        List<Vector2> contourPoints = contour.m_contour;
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

    public bool IntersectsSegment(Vector2 segmentPoint1, Vector2 segmentPoint2)
    {
        return GeometryUtils.TwoSegmentsIntersect(m_points[0], m_points[1], segmentPoint1, segmentPoint2) ||
               GeometryUtils.TwoSegmentsIntersect(m_points[1], m_points[2], segmentPoint1, segmentPoint2) ||
               GeometryUtils.TwoSegmentsIntersect(m_points[2], m_points[0], segmentPoint1, segmentPoint2);
    }

    public bool IntersectsLine(Vector2 linePoint1, Vector2 linePoint2)
    {
        return GeometryUtils.SegmentIntersectsLine(m_points[0], m_points[1], linePoint1, linePoint2) ||
               GeometryUtils.SegmentIntersectsLine(m_points[1], m_points[2], linePoint1, linePoint2) ||
               GeometryUtils.SegmentIntersectsLine(m_points[2], m_points[0], linePoint1, linePoint2);
    }

    public Vector2 GetBarycentre()
    {
        return (m_points[0] + m_points[1] + m_points[2]) / 3.0f;
    }

    public bool ContainsGridPoint(Vector2 gridPoint)
    {
        return GeometryUtils.IsInsideTriangle(gridPoint, m_points[0], m_points[1], m_points[2]);
    }

    public float GetArea()
    {
        return 0.5f * Mathf.Abs(MathUtils.Determinant(m_points[0], m_points[1], m_points[2], false));
    }
}
