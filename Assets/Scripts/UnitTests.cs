using UnityEngine;
using System.Collections.Generic;

public class UnitTests
{
    private static ClippingManager m_clippingManager;

    public static void Init()
    {
        m_clippingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClippingManager>();
    }

    /**
     * Test segment intersection
     * **/
    //public static void TestSegmentsIntersecion()
    //{
    //    //-----TEST 0-----//
    //    Vector2 seg1Pt1 = new Vector2(0, 0);
    //    Vector2 seg1Pt2 = new Vector2(2, 2);
    //    Vector2 seg2Pt1 = new Vector2(1, 1);
    //    Vector2 seg2Pt2 = new Vector2(3, 3);

    //    bool b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
    //    bool b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
    //    bool b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

    //    if (!b1 && b2 && !b3)
    //        Debug.Log("TEST 0 SUCCESS");
    //    else
    //        Debug.Log("TEST 0 FAILURE");

    //    //-----TEST 1-----//
    //    seg1Pt1 = new Vector2(0, 0);
    //    seg1Pt2 = new Vector2(-1, 0);
    //    seg2Pt1 = new Vector2(1, 1);
    //    seg2Pt2 = new Vector2(3, 3);

    //    b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
    //    b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
    //    b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

    //    if (!b1 && !b2 && !b3)
    //        Debug.Log("TEST 1 SUCCESS");
    //    else
    //        Debug.Log("TEST 1 FAILURE");

    //    //-----TEST 2-----//
    //    seg1Pt1 = new Vector2(0, 0);
    //    seg1Pt2 = new Vector2(3, 0);
    //    seg2Pt1 = new Vector2(5, 6);
    //    seg2Pt2 = new Vector2(3, 0);

    //    b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
    //    b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
    //    b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

    //    if (b1 && !b2 && !b3)
    //        Debug.Log("TEST 2 SUCCESS");
    //    else
    //        Debug.Log("TEST 2 FAILURE");

    //    //-----TEST 3-----//
    //    seg1Pt1 = new Vector2(0, 0);
    //    seg1Pt2 = new Vector2(7, 7);
    //    seg2Pt1 = new Vector2(2, 5);
    //    seg2Pt2 = new Vector2(6, 0);

    //    b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
    //    b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
    //    b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

    //    if (!b1 && !b2 && b3)
    //        Debug.Log("TEST 3 SUCCESS");
    //    else
    //        Debug.Log("TEST 3 FAILURE");

    //    //-----TEST 4-----//
    //    seg1Pt1 = new Vector2(6, 0);
    //    seg1Pt2 = new Vector2(6, 6);
    //    seg2Pt1 = new Vector2(0, 0);
    //    seg2Pt2 = new Vector2(6, 3);

    //    b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
    //    b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
    //    b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

    //    if (b1 && !b2 && !b3)
    //        Debug.Log("TEST 4 SUCCESS");
    //    else
    //        Debug.Log("TEST 4 FAILURE");

    //    //-----TEST 5-----//
    //    seg1Pt1 = new Vector2(0, 0);
    //    seg1Pt2 = new Vector2(5, 5);
    //    seg2Pt1 = new Vector2(8, 8);
    //    seg2Pt2 = new Vector2(7, 7);

    //    b1 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_INTERSECTION_IS_ENDPOINT);
    //    b2 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_OVERLAP);
    //    b3 = GeometryUtils.TwoSegmentsIntersect(seg1Pt1, seg1Pt2, seg2Pt1, seg2Pt2, GeometryUtils.SEGMENTS_STRICT_INTERSECTION);

    //    if (!b1 && !b2 && !b3)
    //        Debug.Log("TEST 5 SUCCESS");
    //    else
    //        Debug.Log("TEST 5 FAILURE");
    //}

    /**
     * Test the clipping operations specified in the ClippingBooleanOperations.cs class between two shapes
     * **/
    public static void TestShapesClipping()
    {
        ////-----TEST 0-----//
        //Contour subjShapeContour1 = new Contour(4);
        //subjShapeContour1.Add(new Vector2(0, 0));
        //subjShapeContour1.Add(new Vector2(300, 0));
        //subjShapeContour1.Add(new Vector2(300, 300));
        //subjShapeContour1.Add(new Vector2(0, 300));
        //Contour clipShapeContour1 = new Contour(4);
        //clipShapeContour1.Add(new Vector2(200, 200));
        //clipShapeContour1.Add(new Vector2(500, 200));
        //clipShapeContour1.Add(new Vector2(500, 500));
        //clipShapeContour1.Add(new Vector2(200, 500));
        //Shape subjShape1 = new Shape(true, subjShapeContour1);
        //Shape clipShape1 = new Shape(true, clipShapeContour1);
        ////intersection
        //List<Shape> result = m_clippingManager.ShapesOperation(subjShape1, clipShape1, ClipperLib.ClipType.ctIntersection);
        //Contour subjShapeContour2 = new Contour(4);
        //subjShapeContour2.Add(new Vector2(-200, 0));
        //subjShapeContour2.Add(new Vector2(-100, 0));
        //subjShapeContour2.Add(new Vector2(-100, 300));
        //subjShapeContour2.Add(new Vector2(-200, 300));
        //Contour clipShapeContour2 = new Contour(4);
        //clipShapeContour2.Add(new Vector2(200, -200));
        //clipShapeContour2.Add(new Vector2(500, -200));
        //clipShapeContour2.Add(new Vector2(500, 100));
        //clipShapeContour2.Add(new Vector2(200, 100));
        //Contour clipShapeHole1 = new Contour(4);
        //clipShapeHole1.Add(new Vector2(250, 50));
        //clipShapeHole1.Add(new Vector2(250, -150));
        //clipShapeHole1.Add(new Vector2(450, -150));
        //clipShapeHole1.Add(new Vector2(450, 50));
        //List<Contour> clipShapeHoles = new List<Contour>(1);
        //clipShapeHoles.Add(clipShapeHole1);
        //Shape subjShape2 = new Shape(true, subjShapeContour2);
        //Shape clipShape2 = new Shape(true, clipShapeContour2, clipShapeHoles);
        //result = m_clippingManager.ShapesOperation(subjShape2, clipShape2, ClipperLib.ClipType.ctIntersection);


        //-----TEST 1-----//
        Contour subjShapeContour = new Contour(4);
        subjShapeContour.Add(new GridPoint(0, 0));
        subjShapeContour.Add(new GridPoint(3, 0));
        subjShapeContour.Add(new GridPoint(3, 3));
        subjShapeContour.Add(new GridPoint(0, 3));
        Contour clipShapeContour = new Contour(4);
        clipShapeContour.Add(new GridPoint(2, 1));
        clipShapeContour.Add(new GridPoint(7, 1));
        clipShapeContour.Add(new GridPoint(7, 2));
        clipShapeContour.Add(new GridPoint(2, 2));
        Shape subjShape = new Shape(subjShapeContour);
        Shape clipShape = new Shape(clipShapeContour);
        //intersection
        List<Shape> result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctIntersection);
        Contour intersectionExpectedContour = new Contour(4);
        intersectionExpectedContour.Add(new GridPoint(2, 1));
        intersectionExpectedContour.Add(new GridPoint(3, 1));
        intersectionExpectedContour.Add(new GridPoint(3, 2));
        intersectionExpectedContour.Add(new GridPoint(2, 2));
        if (result.Count == 1 && result[0].m_contour.EqualsContour(intersectionExpectedContour))
            Debug.Log("TEST 1 INTERSECTION SUCCESS");
        else
            Debug.Log("TEST 1 INTERSECTION FAILURE");
        //union
        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
        Contour unionExpectedContour = new Contour(8);
        unionExpectedContour.Add(new GridPoint(0, 0));
        unionExpectedContour.Add(new GridPoint(3, 0));
        unionExpectedContour.Add(new GridPoint(3, 1));
        unionExpectedContour.Add(new GridPoint(7, 1));
        unionExpectedContour.Add(new GridPoint(7, 2));
        unionExpectedContour.Add(new GridPoint(3, 2));
        unionExpectedContour.Add(new GridPoint(3, 3));
        unionExpectedContour.Add(new GridPoint(0, 3));
        if (result.Count == 1 && result[0].m_contour.EqualsContour(unionExpectedContour))
            Debug.Log("TEST 1 UNION SUCCESS");
        else
            Debug.Log("TEST 1 UNION FAILURE");
        //difference
        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference);
        Contour diffExpectedContour = new Contour(8);
        diffExpectedContour.Add(new GridPoint(0, 0));
        diffExpectedContour.Add(new GridPoint(3, 0));
        diffExpectedContour.Add(new GridPoint(3, 1));
        diffExpectedContour.Add(new GridPoint(2, 1));
        diffExpectedContour.Add(new GridPoint(2, 2));
        diffExpectedContour.Add(new GridPoint(3, 2));
        diffExpectedContour.Add(new GridPoint(3, 3));
        diffExpectedContour.Add(new GridPoint(0, 3));
        if (result.Count == 1 && result[0].m_contour.EqualsContour(diffExpectedContour))
            Debug.Log("TEST 1 DIFF SUCCESS");
        else
            Debug.Log("TEST 1 DIFF FAILURE");

        //-----TEST 2-----//
        subjShapeContour = new Contour(4);
        subjShapeContour.Add(new GridPoint(0, 0));
        subjShapeContour.Add(new GridPoint(6000, 1000));
        subjShapeContour.Add(new GridPoint(5000, 3000));
        subjShapeContour.Add(new GridPoint(2000, 2000));
        List<Contour> subjShapeHoles = new List<Contour>(1);
        Contour subjShapeHole = new Contour(4);
        subjShapeHole.Add(new GridPoint(2000, 1000));
        subjShapeHole.Add(new GridPoint(4000, 1000));
        subjShapeHole.Add(new GridPoint(4000, 2000));
        subjShapeHole.Add(new GridPoint(3000, 2000));
        subjShapeHoles.Add(subjShapeHole);
        clipShapeContour = new Contour(4);
        clipShapeContour.Add(new GridPoint(3000, 0000));
        clipShapeContour.Add(new GridPoint(4000, 0000));
        clipShapeContour.Add(new GridPoint(4000, 4000));
        clipShapeContour.Add(new GridPoint(3000, 4000));
        subjShape = new Shape(subjShapeContour, subjShapeHoles);
        clipShape = new Shape(clipShapeContour);
        //intersection
        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctIntersection);
        Contour intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(3000, 500));
        intersectionExpectedContour1.Add(new GridPoint(4000, 667));
        intersectionExpectedContour1.Add(new GridPoint(4000, 1000));
        intersectionExpectedContour1.Add(new GridPoint(3000, 1000));
        Contour intersectionExpectedContour2 = new Contour(4);
        intersectionExpectedContour2.Add(new GridPoint(3000, 2000));
        intersectionExpectedContour2.Add(new GridPoint(4000, 2000));
        intersectionExpectedContour2.Add(new GridPoint(4000, 2667));
        intersectionExpectedContour2.Add(new GridPoint(3000, 2333));
        if (result.Count == 2 &&
            (
            result[0].m_contour.EqualsContour(intersectionExpectedContour1) &&
            result[1].m_contour.EqualsContour(intersectionExpectedContour2)
            )
            ||
            (
            result[0].m_contour.EqualsContour(intersectionExpectedContour2) &&
            result[1].m_contour.EqualsContour(intersectionExpectedContour1)
            )
            )
            Debug.Log("TEST 2 INTERSECTION SUCCESS");
        else
            Debug.Log("TEST 2 INTERSECTION FAILURE");
        //union
        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion);
        unionExpectedContour = new Contour(12);
        unionExpectedContour.Add(new GridPoint(0, 0));
        unionExpectedContour.Add(new GridPoint(3000, 500));
        unionExpectedContour.Add(new GridPoint(3000, 0));
        unionExpectedContour.Add(new GridPoint(4000, 0));
        unionExpectedContour.Add(new GridPoint(4000, 667));
        unionExpectedContour.Add(new GridPoint(6000, 1000));
        unionExpectedContour.Add(new GridPoint(5000, 3000));
        unionExpectedContour.Add(new GridPoint(4000, 2667));
        unionExpectedContour.Add(new GridPoint(4000, 4000));
        unionExpectedContour.Add(new GridPoint(3000, 4000));
        unionExpectedContour.Add(new GridPoint(3000, 2333));
        unionExpectedContour.Add(new GridPoint(2000, 2000));
        Contour unionExpectedHole = new Contour(3);
        unionExpectedHole.Add(new GridPoint(2000, 1000));
        unionExpectedHole.Add(new GridPoint(3000, 2000));
        unionExpectedHole.Add(new GridPoint(3000, 1000));
        if (result.Count == 1 &&
            result[0].m_contour.EqualsContour(unionExpectedContour) &&
            result[0].m_holes.Count == 1 &&
            result[0].m_holes[0].EqualsContour(unionExpectedHole))
            Debug.Log("TEST 2 UNION SUCCESS");
        else
            Debug.Log("TEST 2 UNION FAILURE");
        //difference
        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference);
        Contour diffExpectedContour1 = new Contour(7);
        diffExpectedContour1.Add(new GridPoint(0, 0));
        diffExpectedContour1.Add(new GridPoint(3000, 500));
        diffExpectedContour1.Add(new GridPoint(3000, 1000));
        diffExpectedContour1.Add(new GridPoint(2000, 1000));
        diffExpectedContour1.Add(new GridPoint(3000, 2000));
        diffExpectedContour1.Add(new GridPoint(3000, 2333));
        diffExpectedContour1.Add(new GridPoint(2000, 2000));
        Contour diffExpectedContour2 = new Contour(4);
        diffExpectedContour2.Add(new GridPoint(4000, 667));
        diffExpectedContour2.Add(new GridPoint(6000, 1000));
        diffExpectedContour2.Add(new GridPoint(5000, 3000));
        diffExpectedContour2.Add(new GridPoint(4000, 2667));
        if (result.Count == 2 &&
            (
            result[0].m_contour.EqualsContour(diffExpectedContour1) &&
            result[1].m_contour.EqualsContour(diffExpectedContour2)
            )
            ||
            (
            result[0].m_contour.EqualsContour(diffExpectedContour2) &&
            result[1].m_contour.EqualsContour(diffExpectedContour1)
            )
            )
            Debug.Log("TEST 2 DIFF SUCCESS");
        else
            Debug.Log("TEST 2 DIFF FAILURE");

        //-----TEST 3-----//
        subjShape = new Shape();
        Contour subjContour = new Contour(4);
        subjContour.Add(new GridPoint(-10, -10));
        subjContour.Add(new GridPoint(10, -10));
        subjContour.Add(new GridPoint(10, 10));
        subjContour.Add(new GridPoint(-10, 10));
        subjShape.m_contour = subjContour;

        clipShape = new Shape();
        Contour clipContour = new Contour(4);
        clipContour.Add(new GridPoint(-8, -8));
        clipContour.Add(new GridPoint(8, -8));
        clipContour.Add(new GridPoint(8, 8));
        clipContour.Add(new GridPoint(-8, 8));
        Contour clipHole = new Contour(4);
        clipHole.Add(new GridPoint(-5, -5));
        clipHole.Add(new GridPoint(5, -5));
        clipHole.Add(new GridPoint(5, 5));
        clipHole.Add(new GridPoint(-5, 5));
        List<Contour> clipHoles = new List<Contour>(1);
        clipHoles.Add(clipHole);
        clipShape.m_contour = clipContour;
        clipShape.m_holes = clipHoles;

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctIntersection, true);
        intersectionExpectedContour = new Contour(4);
        intersectionExpectedContour.Add(new GridPoint(-8, -8));
        intersectionExpectedContour.Add(new GridPoint(8, -8));
        intersectionExpectedContour.Add(new GridPoint(8, 8));
        intersectionExpectedContour.Add(new GridPoint(-8, 8));
        Contour intersectionExpectedHole = new Contour(4);
        intersectionExpectedHole.Add(new GridPoint(-5, -5));
        intersectionExpectedHole.Add(new GridPoint(5, -5));
        intersectionExpectedHole.Add(new GridPoint(5, 5));
        intersectionExpectedHole.Add(new GridPoint(-5, 5));

        if (result.Count == 1 &&
            (
            result[0].m_contour.EqualsContour(intersectionExpectedContour)
            && result[0].m_holes.Count == 1
            && result[0].m_holes[0].EqualsContour(intersectionExpectedHole, false)
            )
            )
            Debug.Log("TEST 3 INTERSECTION SUCCESS");
        else
            Debug.Log("TEST 3 INTERSECTION FAILURE");

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference, false);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(-10, -10));
        intersectionExpectedContour1.Add(new GridPoint(10, -10));
        intersectionExpectedContour1.Add(new GridPoint(10, 10));
        intersectionExpectedContour1.Add(new GridPoint(-10, 10));
        Contour intersectionExpectedHole1 = new Contour(4);
        intersectionExpectedHole1.Add(new GridPoint(-8, -8));
        intersectionExpectedHole1.Add(new GridPoint(8, -8));
        intersectionExpectedHole1.Add(new GridPoint(8, 8));
        intersectionExpectedHole1.Add(new GridPoint(-8, 8));
        intersectionExpectedContour2 = new Contour(4);
        intersectionExpectedContour2.Add(new GridPoint(-5, -5));
        intersectionExpectedContour2.Add(new GridPoint(5, -5));
        intersectionExpectedContour2.Add(new GridPoint(5, 5));
        intersectionExpectedContour2.Add(new GridPoint(-5, 5));
        if (result.Count == 2 &&
            (
            result[0].m_contour.EqualsContour(intersectionExpectedContour1)
            && result[0].m_holes.Count == 1
            && result[0].m_holes[0].EqualsContour(intersectionExpectedHole1, false)
            && 
            result[1].m_contour.EqualsContour(intersectionExpectedContour2)
            )
            )
            Debug.Log("TEST 3 DIFFERENCE SUCCESS");
        else
            Debug.Log("TEST 3 DIFFERENCE FAILURE");

        //-----TEST 4-----//
        subjShape = new Shape();
        subjContour = new Contour(4);
        subjContour.Add(new GridPoint(50, 100));
        subjContour.Add(new GridPoint(50, 50));
        subjContour.Add(new GridPoint(130, 50));
        subjContour.Add(new GridPoint(130, 100));
        subjShape.m_contour = subjContour;

        clipShape = new Shape();
        clipContour = new Contour(4);
        clipContour.Add(new GridPoint(80, 40));
        clipContour.Add(new GridPoint(120, 40));
        clipContour.Add(new GridPoint(120, 110));
        clipContour.Add(new GridPoint(80, 110));
        clipShape.m_contour = clipContour;

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion, true);
        intersectionExpectedContour1 = new Contour(13);
        intersectionExpectedContour1.Add(new GridPoint(50, 100));
        intersectionExpectedContour1.Add(new GridPoint(50, 50));
        intersectionExpectedContour1.Add(new GridPoint(80, 50));
        intersectionExpectedContour1.Add(new GridPoint(80, 40));
        intersectionExpectedContour1.Add(new GridPoint(120, 40));
        intersectionExpectedContour1.Add(new GridPoint(120, 50));
        intersectionExpectedContour1.Add(new GridPoint(130, 50));
        intersectionExpectedContour1.Add(new GridPoint(130, 100));
        intersectionExpectedContour1.Add(new GridPoint(120, 100));
        intersectionExpectedContour1.Add(new GridPoint(120, 110));
        intersectionExpectedContour1.Add(new GridPoint(80, 110));
        intersectionExpectedContour1.Add(new GridPoint(80, 100));

        if (result.Count == 1 &&
                (
                    result[0].m_contour.EqualsContour(intersectionExpectedContour1)
                )
            )
            Debug.Log("TEST 4 UNION SUCCESS");
        else
            Debug.Log("TEST 4 UNION FAILURE");

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference, true);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(50, 100));
        intersectionExpectedContour1.Add(new GridPoint(50, 50));
        intersectionExpectedContour1.Add(new GridPoint(80, 50));
        intersectionExpectedContour1.Add(new GridPoint(80, 100));
        intersectionExpectedContour2 = new Contour(4);
        intersectionExpectedContour2.Add(new GridPoint(120, 100));
        intersectionExpectedContour2.Add(new GridPoint(120, 50));
        intersectionExpectedContour2.Add(new GridPoint(130, 50));
        intersectionExpectedContour2.Add(new GridPoint(130, 100));

        if (result.Count == 2 &&
            (
            result[0].m_contour.EqualsContour(intersectionExpectedContour1) &&
            result[1].m_contour.EqualsContour(intersectionExpectedContour2)
            )
            )
            Debug.Log("TEST 4 DIFFERENCE SUCCESS");
        else
            Debug.Log("TEST 4 DIFFERENCE FAILURE");

        //-----TEST 5-----//
        subjShape = new Shape();
        subjContour = new Contour(4);
        subjContour.Add(new GridPoint(40, 110));
        subjContour.Add(new GridPoint(40, 20));
        subjContour.Add(new GridPoint(140, 20));
        subjContour.Add(new GridPoint(140, 110));
        Contour subjHole = new Contour(4);
        subjHole.Add(new GridPoint(70, 80));
        subjHole.Add(new GridPoint(70, 40));
        subjHole.Add(new GridPoint(120, 40));
        subjHole.Add(new GridPoint(120, 80));
        subjShape.m_contour = subjContour;
        subjShape.m_holes.Add(subjHole);

        clipShape = new Shape();
        clipContour = new Contour(4);
        clipContour.Add(new GridPoint(70, 80));
        clipContour.Add(new GridPoint(120, 80));
        clipContour.Add(new GridPoint(90, 110));
        clipShape.m_contour = clipContour;

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion, true);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(40, 110));
        intersectionExpectedContour1.Add(new GridPoint(40, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 110));
        intersectionExpectedHole1 = new Contour(4);
        intersectionExpectedHole1.Add(new GridPoint(70, 80));
        intersectionExpectedHole1.Add(new GridPoint(70, 40));
        intersectionExpectedHole1.Add(new GridPoint(120, 40));
        intersectionExpectedHole1.Add(new GridPoint(120, 80));

        if (result.Count == 1 &&
                (
                    result[0].m_contour.EqualsContour(intersectionExpectedContour1)
                    && result[0].m_holes.Count == 1
                    && result[0].m_holes[0].EqualsContour(intersectionExpectedHole1, false)
                )
            )
            Debug.Log("TEST 5 UNION SUCCESS");
        else
            Debug.Log("TEST 5 UNION FAILURE");

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctIntersection, false);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(70, 80));
        intersectionExpectedContour1.Add(new GridPoint(120, 80));
        intersectionExpectedContour1.Add(new GridPoint(90, 110));

        if (result.Count == 1 &&
                (
                    result[0].m_contour.EqualsContour(intersectionExpectedContour1)
                )
            )
            Debug.Log("TEST 5 INTERSECTION SUCCESS");
        else
            Debug.Log("TEST 5 INTERSECTION FAILURE");

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference, false);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(40, 110));
        intersectionExpectedContour1.Add(new GridPoint(40, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 110));
        intersectionExpectedHole1 = new Contour(5);
        intersectionExpectedHole1.Add(new GridPoint(70, 80));
        intersectionExpectedHole1.Add(new GridPoint(90, 110));
        intersectionExpectedHole1.Add(new GridPoint(120, 80));
        intersectionExpectedHole1.Add(new GridPoint(120, 40));
        intersectionExpectedHole1.Add(new GridPoint(70, 40));

        if (result.Count == 1 &&
            result[0].m_contour.EqualsContour(intersectionExpectedContour1) &&
            result[0].m_holes.Count == 1 &&
            result[0].m_holes[0].EqualsContour(intersectionExpectedHole1)
            )
            Debug.Log("TEST 5 DIFFERENCE SUCCESS");
        else
            Debug.Log("TEST 5 DIFFERENCE FAILURE");

        //-----TEST 6-----//
        subjShape = new Shape();
        subjContour = new Contour(4);
        subjContour.Add(new GridPoint(40, 110));
        subjContour.Add(new GridPoint(40, 20));
        subjContour.Add(new GridPoint(140, 20));
        subjContour.Add(new GridPoint(140, 110));
        subjHole = new Contour(5);
        subjHole.Add(new GridPoint(70, 80));
        subjHole.Add(new GridPoint(70, 30));
        subjHole.Add(new GridPoint(100, 40));
        subjHole.Add(new GridPoint(120, 40));
        subjHole.Add(new GridPoint(120, 80));
        subjShape.m_contour = subjContour;
        subjShape.m_holes.Add(subjHole);

        clipShape = new Shape();
        clipContour = new Contour(4);
        clipContour.Add(new GridPoint(80, 80));
        clipContour.Add(new GridPoint(100, 40));
        clipContour.Add(new GridPoint(120, 80));
        clipShape.m_contour = clipContour;

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctUnion, true);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(40, 110));
        intersectionExpectedContour1.Add(new GridPoint(40, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 110));
        intersectionExpectedHole1 = new Contour(4);
        intersectionExpectedHole1.Add(new GridPoint(70, 80));
        intersectionExpectedHole1.Add(new GridPoint(70, 30));
        intersectionExpectedHole1.Add(new GridPoint(100, 40));
        intersectionExpectedHole1.Add(new GridPoint(80, 80));
        Contour intersectionExpectedHole2 = new Contour(4);
        intersectionExpectedHole2.Add(new GridPoint(120, 80));
        intersectionExpectedHole2.Add(new GridPoint(100, 40));
        intersectionExpectedHole2.Add(new GridPoint(120, 40));

        if (result.Count == 1 &&
                (
                    result[0].m_contour.EqualsContour(intersectionExpectedContour1)
                    && result[0].m_holes.Count == 2
                    && 
                    (
                        result[0].m_holes[0].EqualsContour(intersectionExpectedHole1, false) &&
                        result[0].m_holes[1].EqualsContour(intersectionExpectedHole2, false)
                        ||
                        result[0].m_holes[0].EqualsContour(intersectionExpectedHole2, false) &&
                        result[0].m_holes[1].EqualsContour(intersectionExpectedHole1, false)
                    )
                )
            )
            Debug.Log("TEST 6 UNION SUCCESS");
        else
            Debug.Log("TEST 6 UNION FAILURE");

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctIntersection, false);

        if (result.Count == 0)
            Debug.Log("TEST 6 INTERSECTION SUCCESS");
        else
            Debug.Log("TEST 6 INTERSECTION FAILURE");

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference, false);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(40, 110));
        intersectionExpectedContour1.Add(new GridPoint(40, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 20));
        intersectionExpectedContour1.Add(new GridPoint(140, 110));
        intersectionExpectedHole1 = new Contour(5);
        intersectionExpectedHole1.Add(new GridPoint(70, 80));
        intersectionExpectedHole1.Add(new GridPoint(70, 30));
        intersectionExpectedHole1.Add(new GridPoint(100, 40));
        intersectionExpectedHole1.Add(new GridPoint(120, 40));
        intersectionExpectedHole1.Add(new GridPoint(120, 80));

        if (result.Count == 1 &&
            result[0].m_contour.EqualsContour(intersectionExpectedContour1) &&
            result[0].m_holes.Count == 1 &&
            result[0].m_holes[0].EqualsContour(intersectionExpectedHole1, false)
            )
            Debug.Log("TEST 6 DIFFERENCE SUCCESS");
        else
            Debug.Log("TEST 6 DIFFERENCE FAILURE");

        //-----TEST 7-----//
        subjShape = new Shape();
        subjContour = new Contour(6);
        subjContour.Add(new GridPoint(50, 50));
        subjContour.Add(new GridPoint(100, 50));
        subjContour.Add(new GridPoint(100, 100));
        subjContour.Add(new GridPoint(30, 100));
        subjContour.Add(new GridPoint(30, 80));
        subjContour.Add(new GridPoint(50, 80));
        subjHole = new Contour(5);
        subjHole.Add(new GridPoint(75, 100));
        subjHole.Add(new GridPoint(60, 75));
        subjHole.Add(new GridPoint(75, 50));
        subjHole.Add(new GridPoint(90, 75));
        subjShape.m_contour = subjContour;
        subjShape.m_holes.Add(subjHole);

        clipShape = new Shape();
        clipContour = new Contour(4);
        clipContour.Add(new GridPoint(30, 80));
        clipContour.Add(new GridPoint(50, 80));
        clipContour.Add(new GridPoint(50, 100));
        clipShape.m_contour = clipContour;

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference, true);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(30, 100));
        intersectionExpectedContour1.Add(new GridPoint(30, 80));
        intersectionExpectedContour1.Add(new GridPoint(50, 100));
        intersectionExpectedContour2 = new Contour(5);
        intersectionExpectedContour2.Add(new GridPoint(75, 50));
        intersectionExpectedContour2.Add(new GridPoint(100, 50));
        intersectionExpectedContour2.Add(new GridPoint(100, 100));
        intersectionExpectedContour2.Add(new GridPoint(75, 100));
        intersectionExpectedContour2.Add(new GridPoint(90, 75));
        Contour intersectionExpectedContour3 = new Contour(5);
        intersectionExpectedContour3.Add(new GridPoint(60, 75));
        intersectionExpectedContour3.Add(new GridPoint(75, 100));
        intersectionExpectedContour3.Add(new GridPoint(50, 100));
        intersectionExpectedContour3.Add(new GridPoint(50, 50));
        intersectionExpectedContour3.Add(new GridPoint(75, 50));

        if (result.Count == 3 &&
            result[0].m_contour.EqualsContour(intersectionExpectedContour1) &&
            result[1].m_contour.EqualsContour(intersectionExpectedContour2) &&
            result[2].m_contour.EqualsContour(intersectionExpectedContour3)
            )
            Debug.Log("TEST 7 DIFFERENCE SUCCESS");
        else
            Debug.Log("TEST 7 DIFFERENCE FAILURE");

        //-----TEST 8-----//
        subjShape = new Shape();
        subjContour = new Contour(6);
        subjContour.Add(new GridPoint(50, 50));
        subjContour.Add(new GridPoint(100, 50));
        subjContour.Add(new GridPoint(100, 100));
        subjContour.Add(new GridPoint(50, 100));
        subjHole = new Contour(5);
        subjHole.Add(new GridPoint(75, 100));
        subjHole.Add(new GridPoint(60, 75));
        subjHole.Add(new GridPoint(75, 50));
        subjHole.Add(new GridPoint(90, 75));
        subjShape.m_contour = subjContour;
        subjShape.m_holes.Add(subjHole);

        clipShape = new Shape();
        clipContour = new Contour(4);
        clipContour.Add(new GridPoint(50, 80));
        clipContour.Add(new GridPoint(50, 65));
        clipContour.Add(new GridPoint(60, 75));
        clipShape.m_contour = clipContour;

        result = m_clippingManager.ShapesOperation(subjShape, clipShape, ClipperLib.ClipType.ctDifference, true);
        intersectionExpectedContour1 = new Contour(4);
        intersectionExpectedContour1.Add(new GridPoint(75, 100));
        intersectionExpectedContour1.Add(new GridPoint(50, 100));
        intersectionExpectedContour1.Add(new GridPoint(50, 80));
        intersectionExpectedContour1.Add(new GridPoint(60, 75));
        intersectionExpectedContour2 = new Contour(4);
        intersectionExpectedContour2.Add(new GridPoint(75, 100));
        intersectionExpectedContour2.Add(new GridPoint(90, 75));
        intersectionExpectedContour2.Add(new GridPoint(75, 50));
        intersectionExpectedContour2.Add(new GridPoint(100, 50));
        intersectionExpectedContour2.Add(new GridPoint(100, 100));
        intersectionExpectedContour3 = new Contour(4);
        intersectionExpectedContour3.Add(new GridPoint(50, 65));
        intersectionExpectedContour3.Add(new GridPoint(50, 50));
        intersectionExpectedContour3.Add(new GridPoint(75, 50));
        intersectionExpectedContour3.Add(new GridPoint(60, 75));

        if (result.Count == 3 &&
            result[0].m_contour.EqualsContour(intersectionExpectedContour1) &&
            result[1].m_contour.EqualsContour(intersectionExpectedContour2) &&
            result[2].m_contour.EqualsContour(intersectionExpectedContour3))
            Debug.Log("TEST 8 DIFFERENCE SUCCESS");
        else
            Debug.Log("TEST 8 DIFFERENCE FAILURE");
    }
    
    /**
     * Test intersections between 2 triangles with the possibility to test if this intersection is non-empty
     * **/
    public static void TestTrianglesIntersections()
    {
        //-----TEST 1-----// O points
        GridTriangle triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(1, 0);
        triangle1.m_points[2] = new GridPoint(0, 2);
        GridTriangle triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(2, 2);
        triangle2.m_points[1] = new GridPoint(4, 2);
        triangle2.m_points[2] = new GridPoint(3, 4);

        bool bResult = triangle1.IntersectsTriangle(triangle2);
        //bool bResult2 = triangle1.IntersectsTriangleWithNonNullIntersection(triangle2);

        if (!bResult)
            Debug.Log("TEST 1 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 1 FAILURE:" + bResult);

        //-----TEST 2-----// 1 point
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(1000, 0);
        triangle1.m_points[2] = new GridPoint(0, 2000);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(500, 1000);
        triangle2.m_points[1] = new GridPoint(3000, 0);
        triangle2.m_points[2] = new GridPoint(3000, 3000);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 2 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 2 FAILURE:" + bResult);


        //-----TEST 3-----//1 point
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(1, 0);
        triangle1.m_points[2] = new GridPoint(0, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(0, 2);
        triangle2.m_points[1] = new GridPoint(3, 2);
        triangle2.m_points[2] = new GridPoint(2, 4);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 3 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 3 FAILURE:" + bResult);

        //-----TEST 4-----//1 point
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(1, 0);
        triangle1.m_points[2] = new GridPoint(0, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(0, 1);
        triangle2.m_points[1] = new GridPoint(3, 1);
        triangle2.m_points[2] = new GridPoint(2, 3);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 4 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 4 FAILURE:" + bResult);

        //-----TEST 5-----//1 point
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(1, 0);
        triangle1.m_points[2] = new GridPoint(0, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(1, 0);
        triangle2.m_points[1] = new GridPoint(3, 3);
        triangle2.m_points[2] = new GridPoint(-2, 2);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 5 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 5 FAILURE:" + bResult);

        //-----TEST 6-----//1 point
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(1, 0);
        triangle1.m_points[2] = new GridPoint(0, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(0, 2);
        triangle2.m_points[1] = new GridPoint(2, 4);
        triangle2.m_points[2] = new GridPoint(-1, 4);

        bResult = triangle1.IntersectsTriangle(triangle2, true);

        if (!bResult)
            Debug.Log("TEST 6 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 6 FAILURE:" + bResult);

        //-----TEST 7-----//2 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(4, 0);
        triangle1.m_points[2] = new GridPoint(2, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(1, 0);
        triangle2.m_points[1] = new GridPoint(3, 0);
        triangle2.m_points[2] = new GridPoint(4, 3);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 7 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 7 FAILURE:" + bResult);

        //-----TEST 8-----//2 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(4, 0);
        triangle1.m_points[2] = new GridPoint(2, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(0, 0);
        triangle2.m_points[1] = new GridPoint(2, -3);
        triangle2.m_points[2] = new GridPoint(2, 0);

        bResult = triangle1.IntersectsTriangle(triangle2, true);

        if (!bResult)
            Debug.Log("TEST 8 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 8 FAILURE:" + bResult);

        //-----TEST 9-----//2 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(4, 0);
        triangle1.m_points[2] = new GridPoint(2, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(0, 0);
        triangle2.m_points[1] = new GridPoint(2, 0);
        triangle2.m_points[2] = new GridPoint(0, -2);

        bResult = triangle1.IntersectsTriangle(triangle2, true);

        if (!bResult)
            Debug.Log("TEST 9 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 9 FAILURE:" + bResult);

        //-----TEST 10-----//2 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(4, 0);
        triangle1.m_points[2] = new GridPoint(2, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(2, 0);
        triangle2.m_points[1] = new GridPoint(5, 2);
        triangle2.m_points[2] = new GridPoint(2, 2);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 10 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 10 FAILURE:" + bResult);

        //-----TEST 11-----//2 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(4, 0);
        triangle1.m_points[2] = new GridPoint(2, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(2, 0);
        triangle2.m_points[1] = new GridPoint(3, 1);
        triangle2.m_points[2] = new GridPoint(0, 2);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 11 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 11 FAILURE:" + bResult);

        //-----TEST 12-----//2 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(4, 0);
        triangle1.m_points[2] = new GridPoint(2, 2);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(1, 1);
        triangle2.m_points[1] = new GridPoint(2, 0);
        triangle2.m_points[2] = new GridPoint(3, 1);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 12 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 12 FAILURE:" + bResult);

        //-----TEST 13-----//0 point
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(6, 0);
        triangle1.m_points[2] = new GridPoint(3, 4);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(2, 1);
        triangle2.m_points[1] = new GridPoint(3, 1);
        triangle2.m_points[2] = new GridPoint(3, 2);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 13 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 13 FAILURE:" + bResult);

        //-----TEST 14-----//3 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(6, 0);
        triangle1.m_points[2] = new GridPoint(3, 3);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(3, 3);
        triangle2.m_points[1] = new GridPoint(1, 1);
        triangle2.m_points[2] = new GridPoint(4, 0);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 14 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 14 FAILURE:" + bResult);

        //-----TEST 15-----//3 points
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(6, 0);
        triangle1.m_points[2] = new GridPoint(3, 3);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(0, 0);
        triangle2.m_points[1] = new GridPoint(6, 0);
        triangle2.m_points[2] = new GridPoint(3, 3);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 15 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 15 FAILURE:" + bResult);

        //-----TEST 16-----//
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 0);
        triangle1.m_points[1] = new GridPoint(3, 0);
        triangle1.m_points[2] = new GridPoint(-2, 3);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(-2, 1);
        triangle2.m_points[1] = new GridPoint(6, 0);
        triangle2.m_points[2] = new GridPoint(3, 3);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 16 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 16 FAILURE:" + bResult);

        //-----TEST 17-----//
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(0, 0);
        triangle2.m_points[1] = new GridPoint(1, 0);
        triangle2.m_points[2] = new GridPoint(0, 1);
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(2, 0);
        triangle1.m_points[1] = new GridPoint(2, 2);
        triangle1.m_points[2] = new GridPoint(0, 0);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 17 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 17 FAILURE:" + bResult);

        //-----TEST 18-----//
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(0, 54);
        triangle1.m_points[1] = new GridPoint(150, 54);
        triangle1.m_points[2] = new GridPoint(0, 204);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(300, 54);
        triangle2.m_points[1] = new GridPoint(300, 204);
        triangle2.m_points[2] = new GridPoint(0, 54);

        bResult = triangle1.IntersectsTriangle(triangle2);

        if (bResult)
            Debug.Log("TEST 18 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 18 FAILURE:" + bResult);

        //-----TEST 19-----//
        triangle1 = new GridTriangle();
        triangle1.m_points[0] = new GridPoint(11, 7);
        triangle1.m_points[1] = new GridPoint(12, 6);
        triangle1.m_points[2] = new GridPoint(12, 7);
        triangle2 = new GridTriangle();
        triangle2.m_points[0] = new GridPoint(10, 8);
        triangle2.m_points[1] = new GridPoint(14, 4);
        triangle2.m_points[2] = new GridPoint(14, 8);

        bResult = triangle1.IntersectsTriangle(triangle2, true);

        if (bResult)
            Debug.Log("TEST 19 SUCCESS:" + bResult);
        else
            Debug.Log("TEST 19 FAILURE:" + bResult);
    }

    /**
     * Test shapes overlapping
     * **/
    public static void TestShapesOverlapping()
    {
        //-----TEST 1-----//
        Contour subjContour = new Contour(8);
        subjContour.Add(new GridPoint(0, 153600));
        subjContour.Add(new GridPoint(0, 28800));
        subjContour.Add(new GridPoint(-124800, 28800));
        subjContour.Add(new GridPoint(-124800, -96000));
        subjContour.Add(new GridPoint(249600, -96000));
        subjContour.Add(new GridPoint(249600, 28800));
        subjContour.Add(new GridPoint(124800, 28800));
        subjContour.Add(new GridPoint(124800, 153600));
        Shape subjShape = new Shape(subjContour);
        subjShape.Triangulate();

        Contour clipContour = new Contour(4);
        clipContour.Add(new GridPoint(124800, -96000));
        clipContour.Add(new GridPoint(0, -96000));
        clipContour.Add(new GridPoint(0, -220800));
        clipContour.Add(new GridPoint(124800, -220800));
        Shape clipShape = new Shape(clipContour);
        clipShape.Triangulate();

        bool result = subjShape.OverlapsShape(clipShape, false);
        if (result)
            Debug.Log("TEST 1 SUCCESS");
        else
            Debug.Log("TEST 1 FAILURE");
    }
}