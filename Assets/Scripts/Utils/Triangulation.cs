using UnityEngine;
using System.Collections.Generic;

using Poly2Tri;

public class Triangulation
{
    public const float EPSILON = 0.0000000001f;

    //public static float Area(Contour contour)
    //{
    //    int n = contour.Count;

    //    float A = 0.0f;

    //    for (int p = n - 1, q = 0; q < n; p = q++)
    //    {
    //        A += contour[p].x * contour[q].y - contour[q].x * contour[p].y;
    //    }
    //    return A * 0.5f;
    //}

    ///**
    // * InsideTriangle decides if a point P is Inside of the triangle
    // * defined by A, B, C.
    // **/
    //private static bool InsideTriangle(float Ax, float Ay,
    //                    float Bx, float By,
    //                    float Cx, float Cy,
    //                    float Px, float Py)
    //{
    //    float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
    //    float cCROSSap, bCROSScp, aCROSSbp;

    //    ax = Cx - Bx; ay = Cy - By;
    //    bx = Ax - Cx; by = Ay - Cy;
    //    cx = Bx - Ax; cy = By - Ay;
    //    apx = Px - Ax; apy = Py - Ay;
    //    bpx = Px - Bx; bpy = Py - By;
    //    cpx = Px - Cx; cpy = Py - Cy;

    //    aCROSSbp = ax * bpy - ay * bpx;
    //    cCROSSap = cx * apy - cy * apx;
    //    bCROSScp = bx * cpy - by * cpx;

    //    return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    //}

    //private static bool Snip(List<Vector2> contour, int u, int v, int w, int n, int[] V)
    //{
    //    int p;
    //    float Ax, Ay, Bx, By, Cx, Cy, Px, Py;

    //    Ax = contour[V[u]].x;
    //    Ay = contour[V[u]].y;

    //    Bx = contour[V[v]].x;
    //    By = contour[V[v]].y;

    //    Cx = contour[V[w]].x;
    //    Cy = contour[V[w]].y;

    //    if (EPSILON > (((Bx - Ax) * (Cy - Ay)) - ((By - Ay) * (Cx - Ax)))) return false;

    //    for (p = 0; p < n; p++)
    //    {
    //        if ((p == u) || (p == v) || (p == w)) continue;
    //        Px = contour[V[p]].x;
    //        Py = contour[V[p]].y;
    //        if (InsideTriangle(Ax, Ay, Bx, By, Cx, Cy, Px, Py)) return false;
    //    }

    //    return true;
    //}

    //public static bool Process(List<Vector2> contour, ref List<Vector2> result)
    //{
    //    /* allocate and initialize list of Vertices in polygon */
    //    int n = contour.Count;
    //    if (n < 3) return false;

    //    int[] V = new int[n];

    //    /* we want a counter-clockwise polygon in V */
    //    if (0.0f < Area(contour))
    //        for (int v = 0; v < n; v++) V[v] = v;
    //    else
    //        for (int v = 0; v < n; v++) V[v] = (n - 1) - v;

    //    int nv = n;

    //    /*  remove nv-2 Vertices, creating 1 triangle every time */
    //    int count = 2 * nv;   /* error detection */

    //    for (int m = 0, v = nv - 1; nv > 2; )
    //    {
    //        /* if we loop, it is probably a non-simple polygon */
    //        if (0 >= (count--))
    //        {
    //            //** Triangulate: ERROR - probable bad polygon!
    //            return false;
    //        }

    //        /* three consecutive vertices in current polygon, <u,v,w> */
    //        int u = v; if (nv <= u) u = 0;     /* previous */
    //        v = u + 1; if (nv <= v) v = 0;     /* new v    */
    //        int w = v + 1; if (nv <= w) w = 0;     /* next     */

    //        if (Snip(contour, u, v, w, nv, V))
    //        {
    //            int a, b, c, s, t;

    //            /* true names of the vertices */
    //            a = V[u]; b = V[v]; c = V[w];

    //            /* output Triangle */
    //            result.Add(contour[a]);
    //            result.Add(contour[b]);
    //            result.Add(contour[c]);

    //            m++;

    //            /* remove v from remaining polygon */
    //            for (s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t]; nv--;

    //            /* resest error detection counter */
    //            count = 2 * nv;
    //        }
    //    }

    //    return true;
    //}

    /**
     * Converts a TriangulationPoint from Poly2Tri library to Unity Vector2
     * **/
    public static GridPoint ConvertTriangulationPointToGridPoint(TriangulationPoint triangulationPoint)
    {
        return new GridPoint((int) triangulationPoint.Xf, (int) triangulationPoint.Yf, false);
    }

    /**
     * Converts a Unity Vector2 from Poly2Tri library to PolygonPoint from Poly2Tri library
     * **/
    public static PolygonPoint ConvertGridPointToPolygonPoint(GridPoint vec2Point)
    {
        return new PolygonPoint((double)vec2Point.X, (double)vec2Point.Y);
    }

    /**
     * Converts a Triangulable object to a Poly2Tri Polygon object
     * **/
    public static Polygon ConvertTriangulableToPolygon(GridTriangulable triangulable)
    {
        Contour contour = triangulable.m_contour;
        List<Contour> holes = triangulable.m_holes;

        //Create contour for polygon
        PolygonPoint[] contourPoints = new PolygonPoint[contour.Count];
        for (int iContourPointIdx = 0; iContourPointIdx != contour.Count; iContourPointIdx++)
        {
            contourPoints[iContourPointIdx] = ConvertGridPointToPolygonPoint(contour[iContourPointIdx]);
        }

        Polygon polygon = new Polygon(contourPoints);

        //Add holes
        for (int iHoleIdx = 0; iHoleIdx != holes.Count; iHoleIdx++)
        {
            Contour hole = holes[iHoleIdx];

            PolygonPoint[] holePoints = new PolygonPoint[hole.Count];
            for (int iHolePointIdx = 0; iHolePointIdx != hole.Count; iHolePointIdx++)
            {
                holePoints[iHolePointIdx] = ConvertGridPointToPolygonPoint(hole[iHolePointIdx]);
            }

            Polygon polyHole = new Polygon(holePoints);
            polygon.AddHole(polyHole);
        }

        return polygon;
    }

    /**
     * Converts a Poly2Tri Polygon object to a Triangulable object
     * **/
    public static GridTriangulable ConvertPolygonToTriangulable(Polygon polygon)
    {
        //Convert holes
        IList<Polygon> polyHoles = polygon.Holes;
        List<Contour> holes = new List<Contour>();
        for (int iPolyHoleIdx = 0; iPolyHoleIdx != polyHoles.Count; iPolyHoleIdx++)
        {
            Polygon polyHole = polyHoles[iPolyHoleIdx];
            IList<TriangulationPoint> polyHolePoints = polyHole.Points;
            Contour holePoints = new Contour();
            holePoints.Capacity = polyHolePoints.Count;
            for (int iHolePointIdx = 0; iHolePointIdx != polyHolePoints.Count; iHolePointIdx++)
            {
                TriangulationPoint polyHolePoint = polyHolePoints[iHolePointIdx];
                holePoints.Add(ConvertTriangulationPointToGridPoint(polyHolePoint));
            }

            holes.Add(holePoints);
        }

        //contour
        IList<TriangulationPoint> polyContour = polygon.Points;
        Contour contour = new Contour();
        contour.Capacity = polyContour.Count;
        for (int iContourPointIdx = 0; iContourPointIdx != polyContour.Count; iContourPointIdx++)
        {
            contour.Add(ConvertTriangulationPointToGridPoint(polyContour[iContourPointIdx]));
        }

        return new GridTriangulable(contour, holes);
    }

    public static Vector2[] P2tTriangulate(GridTriangulable t)
    {
        //Convert the Triangulable object to a Polygon object
        Polygon p = Triangulation.ConvertTriangulableToPolygon(t);

        //Perform the actual triangulation
        P2T.Triangulate(TriangulationAlgorithm.DTSweep, p);

        //Transform the resulting DelaunayTriangle objects into an array of Vector2
        IList<DelaunayTriangle> resultTriangles = p.Triangles;
        Vector2[] triangles = new Vector2[3 * resultTriangles.Count];
        for (int iTriangleIndex = 0; iTriangleIndex != resultTriangles.Count; iTriangleIndex++)
        {
            DelaunayTriangle triangle = resultTriangles[iTriangleIndex];
            FixedArray3<TriangulationPoint> trianglePoints = triangle.Points;
            for (int i = 0; i != 3; i++)
            {
                triangles[iTriangleIndex * 3 + i] = ConvertTriangulationPointToGridPoint(trianglePoints[i]);
            }
        }

        return triangles;
    }
}

