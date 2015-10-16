using UnityEngine;
using System.Collections.Generic;

public class UnitTests
{
    /**
     * Test segment intersection
     * **/
    public static void TestSegmentsIntersecion()
    {
        //-----TEST 0-----//
        Vector2 seg1Pt1 = new Vector2(0, 0);
        Vector2 seg1Pt2 = new Vector2(2, 2);
        Vector2 seg2Pt1 = new Vector2(1, 1);
        Vector2 seg2Pt2 = new Vector2(3, 3);

        bool b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
        bool b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
        bool b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

        if (!b1 && b2 && !b3)
            Debug.Log("TEST 0 SUCCESS");
        else
            Debug.Log("TEST 0 FAILURE");

        //-----TEST 1-----//
        seg1Pt1 = new Vector2(0, 0);
        seg1Pt2 = new Vector2(-1, 0);
        seg2Pt1 = new Vector2(1, 1);
        seg2Pt2 = new Vector2(3, 3);

        b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
        b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
        b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

        if (!b1 && !b2 && !b3)
            Debug.Log("TEST 1 SUCCESS");
        else
            Debug.Log("TEST 1 FAILURE");

        //-----TEST 2-----//
        seg1Pt1 = new Vector2(0, 0);
        seg1Pt2 = new Vector2(3, 0);
        seg2Pt1 = new Vector2(5, 6);
        seg2Pt2 = new Vector2(3, 0);

        b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
        b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
        b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

        if (b1 && !b2 && !b3)
            Debug.Log("TEST 2 SUCCESS");
        else
            Debug.Log("TEST 2 FAILURE");

        //-----TEST 3-----//
        seg1Pt1 = new Vector2(0, 0);
        seg1Pt2 = new Vector2(7, 7);
        seg2Pt1 = new Vector2(2, 5);
        seg2Pt2 = new Vector2(6, 0);

        b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
        b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
        b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

        if (!b1 && !b2 && b3)
            Debug.Log("TEST 3 SUCCESS");
        else
            Debug.Log("TEST 3 FAILURE");

        //-----TEST 4-----//
        seg1Pt1 = new Vector2(6, 0);
        seg1Pt2 = new Vector2(6, 6);
        seg2Pt1 = new Vector2(0, 0);
        seg2Pt2 = new Vector2(6, 3);

        b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
        b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
        b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

        if (b1 && !b2 && !b3)
            Debug.Log("TEST 4 SUCCESS");
        else
            Debug.Log("TEST 4 FAILURE");

        //-----TEST 5-----//
        seg1Pt1 = new Vector2(0, 0);
        seg1Pt2 = new Vector2(5, 5);
        seg2Pt1 = new Vector2(8, 8);
        seg2Pt2 = new Vector2(7, 7);

        b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
        b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
        b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

        if (!b1 && !b2 && !b3)
            Debug.Log("TEST 5 SUCCESS");
        else
            Debug.Log("TEST 5 FAILURE");
    }

    /**
     * Test the clipping operations specified in the ClippingBooleanOperations.cs class between two shapes
     * **/
    public static void TestShapesClipping()
    {
        ClippingManager clippingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClippingManager>();

        //-----TEST 0-----//
        Contour subjShapeContour1 = new Contour(4);
        subjShapeContour1.Add(new Vector2(0, 0));
        subjShapeContour1.Add(new Vector2(300, 0));
        subjShapeContour1.Add(new Vector2(300, 300));
        subjShapeContour1.Add(new Vector2(0, 300));
        Contour clipShapeContour1 = new Contour(4);
        clipShapeContour1.Add(new Vector2(200, 200));
        clipShapeContour1.Add(new Vector2(500, 200));
        clipShapeContour1.Add(new Vector2(500, 500));
        clipShapeContour1.Add(new Vector2(200, 500));
        Shape subjShape1 = new Shape(true, subjShapeContour1);
        Shape clipShape1 = new Shape(true, clipShapeContour1);
        //intersection
        List<Shape> result = clippingManager.ShapesOperation(subjShape1, clipShape1, ClipperLib.ClipType.ctIntersection);
        Contour subjShapeContour2 = new Contour(4);
        subjShapeContour2.Add(new Vector2(-200, 0));
        subjShapeContour2.Add(new Vector2(-100, 0));
        subjShapeContour2.Add(new Vector2(-100, 300));
        subjShapeContour2.Add(new Vector2(-200, 300));
        Contour clipShapeContour2 = new Contour(4);
        clipShapeContour2.Add(new Vector2(200, -200));
        clipShapeContour2.Add(new Vector2(500, -200));
        clipShapeContour2.Add(new Vector2(500, 100));
        clipShapeContour2.Add(new Vector2(200, 100));
        Contour clipShapeHole1 = new Contour(4);
        clipShapeHole1.Add(new Vector2(250, 50));
        clipShapeHole1.Add(new Vector2(250, -150));
        clipShapeHole1.Add(new Vector2(450, -150));
        clipShapeHole1.Add(new Vector2(450, 50));
        List<Contour> clipShapeHoles = new List<Contour>(1);
        clipShapeHoles.Add(clipShapeHole1);
        Shape subjShape2 = new Shape(true, subjShapeContour2);
        Shape clipShape2 = new Shape(true, clipShapeContour2, clipShapeHoles);
        result = clippingManager.ShapesOperation(subjShape2, clipShape2, ClipperLib.ClipType.ctIntersection);
        

        ////-----TEST 1-----//
        //Contour subjShapeContour = new Contour(4);
        //subjShapeContour.Add(new Vector2(0, 0));
        //subjShapeContour.Add(new Vector2(3, 0));
        //subjShapeContour.Add(new Vector2(3, 3));
        //subjShapeContour.Add(new Vector2(0, 3));
        //Contour clipShapeContour = new Contour(4);
        //clipShapeContour.Add(new Vector2(2, 1));
        //clipShapeContour.Add(new Vector2(7, 1));
        //clipShapeContour.Add(new Vector2(7, 2));
        //clipShapeContour.Add(new Vector2(2, 2));
        //Shape subjShape = new Shape(true, subjShapeContour);
        //Shape clipShape = new Shape(true, clipShapeContour);
        ////intersection
        //List<Shape> result = ClippingBooleanOperations.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctIntersection);
        //Contour intersectionExpectedContour = new Contour(4);
        //intersectionExpectedContour.Add(new Vector2(2, 1));
        //intersectionExpectedContour.Add(new Vector2(3, 1));
        //intersectionExpectedContour.Add(new Vector2(3, 2));
        //intersectionExpectedContour.Add(new Vector2(2, 2));
        //if (result.Count == 1 && result[0].m_contour.EqualsContour(intersectionExpectedContour))
        //    Debug.Log("TEST 1 INTERSECTION SUCCESS");
        //else
        //    Debug.Log("TEST 1 INTERSECTION FAILURE");
        ////union
        //result = ClippingBooleanOperations.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
        //Contour unionExpectedContour = new Contour(8);
        //unionExpectedContour.Add(new Vector2(0, 0));
        //unionExpectedContour.Add(new Vector2(3, 0));
        //unionExpectedContour.Add(new Vector2(3, 1));
        //unionExpectedContour.Add(new Vector2(7, 1));
        //unionExpectedContour.Add(new Vector2(7, 2));
        //unionExpectedContour.Add(new Vector2(3, 2));
        //unionExpectedContour.Add(new Vector2(3, 3));
        //unionExpectedContour.Add(new Vector2(0, 3));
        //if (result.Count == 1 && result[0].m_contour.EqualsContour(unionExpectedContour))
        //    Debug.Log("TEST 1 UNION SUCCESS");
        //else
        //    Debug.Log("TEST 1 UNION FAILURE");
        ////difference
        //result = ClippingBooleanOperations.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference);
        //Contour diffExpectedContour = new Contour(8);
        //diffExpectedContour.Add(new Vector2(0, 0));
        //diffExpectedContour.Add(new Vector2(3, 0));
        //diffExpectedContour.Add(new Vector2(3, 1));
        //diffExpectedContour.Add(new Vector2(2, 1));
        //diffExpectedContour.Add(new Vector2(2, 2));
        //diffExpectedContour.Add(new Vector2(3, 2));
        //diffExpectedContour.Add(new Vector2(3, 3));
        //diffExpectedContour.Add(new Vector2(0, 3));
        //if (result.Count == 1 && result[0].m_contour.EqualsContour(diffExpectedContour))
        //    Debug.Log("TEST 1 DIFF SUCCESS");
        //else
        //    Debug.Log("TEST 1 DIFF FAILURE");

        ////-----TEST 2-----//
        //subjShapeContour = new Contour(4);
        //subjShapeContour.Add(new Vector2(0, 0));
        //subjShapeContour.Add(new Vector2(6, 1));
        //subjShapeContour.Add(new Vector2(5, 3));
        //subjShapeContour.Add(new Vector2(2, 2));
        //List<Contour> subjShapeHoles = new List<Contour>(1);
        //Contour subjShapeHole = new Contour(4);
        //subjShapeHole.Add(new Vector2(2, 1));
        //subjShapeHole.Add(new Vector2(4, 1));
        //subjShapeHole.Add(new Vector2(4, 2));
        //subjShapeHole.Add(new Vector2(3, 2));
        //subjShapeHoles.Add(subjShapeHole);
        //clipShapeContour = new Contour(4);
        //clipShapeContour.Add(new Vector2(3, 0));
        //clipShapeContour.Add(new Vector2(4, 0));
        //clipShapeContour.Add(new Vector2(4, 4));
        //clipShapeContour.Add(new Vector2(3, 4));
        //subjShape = new Shape(true, subjShapeContour, subjShapeHoles);
        //clipShape = new Shape(true, clipShapeContour);
        ////intersection
        //result = ClippingBooleanOperations.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctIntersection);
        //Contour intersectionExpectedContour1 = new Contour(4);
        //intersectionExpectedContour1.Add(new Vector2(3, 0.5f));
        //intersectionExpectedContour1.Add(new Vector2(4, 4 / 6.0f));
        //intersectionExpectedContour1.Add(new Vector2(4, 1));
        //intersectionExpectedContour1.Add(new Vector2(3, 1));
        //Contour intersectionExpectedContour2 = new Contour(4);
        //intersectionExpectedContour2.Add(new Vector2(3, 2));
        //intersectionExpectedContour2.Add(new Vector2(4, 2));
        //intersectionExpectedContour2.Add(new Vector2(4, 8 / 3.0f));
        //intersectionExpectedContour2.Add(new Vector2(3, 7 / 3.0f));
        //if (result.Count == 2 &&
        //    (
        //    result[0].m_contour.EqualsContour(intersectionExpectedContour1) &&
        //    result[1].m_contour.EqualsContour(intersectionExpectedContour2)
        //    )
        //    ||
        //    (
        //    result[0].m_contour.EqualsContour(intersectionExpectedContour2) &&
        //    result[1].m_contour.EqualsContour(intersectionExpectedContour1)
        //    )
        //    )
        //    Debug.Log("TEST 2 INTERSECTION SUCCESS");
        //else
        //    Debug.Log("TEST 2 INTERSECTION FAILURE");
        ////union
        //result = ClippingBooleanOperations.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
        //unionExpectedContour = new Contour(12);
        //unionExpectedContour.Add(new Vector2(0, 0));
        //unionExpectedContour.Add(new Vector2(3, 0.5f));
        //unionExpectedContour.Add(new Vector2(3, 0));
        //unionExpectedContour.Add(new Vector2(4, 0));
        //unionExpectedContour.Add(new Vector2(4, 4 / 6.0f));
        //unionExpectedContour.Add(new Vector2(6, 1));
        //unionExpectedContour.Add(new Vector2(5, 3));
        //unionExpectedContour.Add(new Vector2(4, 8 / 3.0f));
        //unionExpectedContour.Add(new Vector2(4, 4));
        //unionExpectedContour.Add(new Vector2(3, 4));
        //unionExpectedContour.Add(new Vector2(3, 7 / 3.0f));
        //unionExpectedContour.Add(new Vector2(2, 2));
        //Contour unionExpectedHole = new Contour(3);
        //unionExpectedHole.Add(new Vector2(2, 1));
        //unionExpectedHole.Add(new Vector2(3, 2));
        //unionExpectedHole.Add(new Vector2(3, 1));
        //if (result.Count == 1 &&
        //    result[0].m_contour.EqualsContour(unionExpectedContour) &&
        //    result[0].m_holes.Count == 1 &&
        //    result[0].m_holes[0].EqualsContour(unionExpectedHole))
        //    Debug.Log("TEST 2 UNION SUCCESS");
        //else
        //    Debug.Log("TEST 2 UNION FAILURE");
        ////difference
        //result = ClippingBooleanOperations.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference);
        //Contour diffExpectedContour1 = new Contour(7);
        //diffExpectedContour1.Add(new Vector2(0, 0));
        //diffExpectedContour1.Add(new Vector2(3, 0.5f));
        //diffExpectedContour1.Add(new Vector2(3, 1));
        //diffExpectedContour1.Add(new Vector2(2, 1));
        //diffExpectedContour1.Add(new Vector2(3, 2));
        //diffExpectedContour1.Add(new Vector2(3, 7 / 3.0f));
        //diffExpectedContour1.Add(new Vector2(2, 2));
        //Contour diffExpectedContour2 = new Contour(4);
        //diffExpectedContour2.Add(new Vector2(4, 4 / 6.0f));
        //diffExpectedContour2.Add(new Vector2(6, 1));
        //diffExpectedContour2.Add(new Vector2(5, 3));
        //diffExpectedContour2.Add(new Vector2(4, 8 / 3.0f));
        //if (result.Count == 2 &&
        //    (
        //    result[0].m_contour.EqualsContour(diffExpectedContour1) &&
        //    result[1].m_contour.EqualsContour(diffExpectedContour2)
        //    )
        //    ||
        //    (
        //    result[0].m_contour.EqualsContour(diffExpectedContour2) &&
        //    result[1].m_contour.EqualsContour(diffExpectedContour1)
        //    )
        //    )
        //    Debug.Log("TEST 2 DIFF SUCCESS");
        //else
        //    Debug.Log("TEST 2 DIFF FAILURE");
    }

        /**
     * Test intersections between 2 triangles with the possibility to test if this intersection is non-empty
     * **/
    public static void TestTrianglesIntersections()
    {
        //-----TEST 1-----// O points
        BaseTriangle triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(1, 0);
        triangle1.m_points[2] = new Vector2(0, 2);
        BaseTriangle triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(2, 2);
        triangle2.m_points[1] = new Vector2(4, 2);
        triangle2.m_points[2] = new Vector2(3, 4);

        bool bResult1 = triangle1.IntersectsTriangle(triangle2);
        bool bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (!bResult1 && !bResult2)
            Debug.Log("TEST 1 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 1 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 2-----// 1 point
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(1, 0);
        triangle1.m_points[2] = new Vector2(0, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0.5f, 1);
        triangle2.m_points[1] = new Vector2(3, 0);
        triangle2.m_points[2] = new Vector2(3, 3);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && !bResult2)
            Debug.Log("TEST 2 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 2 FAILURE:" + bResult1 + "-" + bResult2);


        //-----TEST 3-----//1 point
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(1, 0);
        triangle1.m_points[2] = new Vector2(0, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0, 2);
        triangle2.m_points[1] = new Vector2(3, 2);
        triangle2.m_points[2] = new Vector2(2, 4);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && !bResult2)
            Debug.Log("TEST 3 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 3 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 4-----//1 point
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(1, 0);
        triangle1.m_points[2] = new Vector2(0, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0, 1);
        triangle2.m_points[1] = new Vector2(3, 1);
        triangle2.m_points[2] = new Vector2(2, 3);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 4 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 4 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 5-----//1 point
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(1, 0);
        triangle1.m_points[2] = new Vector2(0, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(1, 0);
        triangle2.m_points[1] = new Vector2(3, 3);
        triangle2.m_points[2] = new Vector2(-2, 2);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 5 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 5 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 6-----//1 point
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(1, 0);
        triangle1.m_points[2] = new Vector2(0, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0, 2);
        triangle2.m_points[1] = new Vector2(2, 4);
        triangle2.m_points[2] = new Vector2(-1, 4);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && !bResult2)
            Debug.Log("TEST 6 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 6 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 7-----//2 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(4, 0);
        triangle1.m_points[2] = new Vector2(2, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(1, 0);
        triangle2.m_points[1] = new Vector2(3, 0);
        triangle2.m_points[2] = new Vector2(4, 3);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 7 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 7 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 8-----//2 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(4, 0);
        triangle1.m_points[2] = new Vector2(2, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0, 0);
        triangle2.m_points[1] = new Vector2(2, -3);
        triangle2.m_points[2] = new Vector2(2, 0);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && !bResult2)
            Debug.Log("TEST 8 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 8 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 9-----//2 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(4, 0);
        triangle1.m_points[2] = new Vector2(2, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0, 0);
        triangle2.m_points[1] = new Vector2(2, 0);
        triangle2.m_points[2] = new Vector2(0, -2);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && !bResult2)
            Debug.Log("TEST 9 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 9 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 10-----//2 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(4, 0);
        triangle1.m_points[2] = new Vector2(2, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(2, 0);
        triangle2.m_points[1] = new Vector2(5, 2);
        triangle2.m_points[2] = new Vector2(2, 2);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 10 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 10 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 11-----//2 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(4, 0);
        triangle1.m_points[2] = new Vector2(2, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(2, 0);
        triangle2.m_points[1] = new Vector2(3, 1);
        triangle2.m_points[2] = new Vector2(0, 2);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 11 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 11 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 12-----//2 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(4, 0);
        triangle1.m_points[2] = new Vector2(2, 2);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(1, 1);
        triangle2.m_points[1] = new Vector2(2, 0);
        triangle2.m_points[2] = new Vector2(3, 1);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 12 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 12 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 13-----//0 point
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(6, 0);
        triangle1.m_points[2] = new Vector2(3, 4);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(2, 1);
        triangle2.m_points[1] = new Vector2(3, 1);
        triangle2.m_points[2] = new Vector2(3, 2);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 13 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 13 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 14-----//3 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(6, 0);
        triangle1.m_points[2] = new Vector2(3, 3);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(3, 3);
        triangle2.m_points[1] = new Vector2(1, 1);
        triangle2.m_points[2] = new Vector2(4, 0);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 14 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 14 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 15-----//3 points
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(6, 0);
        triangle1.m_points[2] = new Vector2(3, 3);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0, 0);
        triangle2.m_points[1] = new Vector2(6, 0);
        triangle2.m_points[2] = new Vector2(3, 3);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 15 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 15 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 16-----//
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 0);
        triangle1.m_points[1] = new Vector2(3, 0);
        triangle1.m_points[2] = new Vector2(-2, 3);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(-2, 1);
        triangle2.m_points[1] = new Vector2(6, 0);
        triangle2.m_points[2] = new Vector2(3, 3);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 16 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 16 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 17-----//
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(0, 0);
        triangle2.m_points[1] = new Vector2(1, 0);
        triangle2.m_points[2] = new Vector2(0, 1);
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(2, 0);
        triangle1.m_points[1] = new Vector2(2, 2);
        triangle1.m_points[2] = new Vector2(0, 0);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 17 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 17 FAILURE:" + bResult1 + "-" + bResult2);

        //-----TEST 18-----//
        triangle1 = new BaseTriangle();
        triangle1.m_points[0] = new Vector2(0, 54);
        triangle1.m_points[1] = new Vector2(150, 54);
        triangle1.m_points[2] = new Vector2(0, 204);
        triangle2 = new BaseTriangle();
        triangle2.m_points[0] = new Vector2(300, 54);
        triangle2.m_points[1] = new Vector2(300, 204);
        triangle2.m_points[2] = new Vector2(0, 54);

        bResult1 = triangle1.IntersectsTriangle(triangle2);
        bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (bResult1 && bResult2)
            Debug.Log("TEST 18 SUCCESS:" + bResult1 + "-" + bResult2);
        else
            Debug.Log("TEST 18 FAILURE:" + bResult1 + "-" + bResult2);
    }
}