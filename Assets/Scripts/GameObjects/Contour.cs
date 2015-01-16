using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Contour
{
    public List<Vector2> m_points { get; set; } //the list of points in this contour in GridAnchor coordinates (line, column)

    public Contour()
    {
        m_points = new List<Vector2>();
    }

    public void Build()
    {
        float fContourZValue = -20.0f;

        GameObject contoursObject = new GameObject("Contours");
        //contoursObject.transform.position = new Vector3(0, 0, fContourZValue);
        //List<Contour> contours = m_levelManager.m_currentLevel.m_contours;
        //foreach (Contour contour in contours)
        //{
        //    GameObject contourObject = new GameObject("Contour");
        //    List<Vector2> contourPoints = contour.m_points;
        //    for (int iPointIndex = 0; iPointIndex != contourPoints.Count; iPointIndex++)
        //    {
        //        Vector2 startPoint = contourPoints[iPointIndex];
        //        Vector2 endPoint = (iPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iPointIndex + 1];

        //        GameObject clonedContourSegmentObject = (GameObject)Instantiate(m_contourSegmentPfb);
        //        ContourSegment contourSegment = clonedContourSegmentObject.GetComponent<ContourSegment>();
        //        contourSegment.m_startPointGrid = startPoint;
        //        contourSegment.m_endPointGrid = endPoint;
        //        contourSegment.transform.parent = contourObject.transform;
        //    }

        //    contourObject.transform.parent = contoursObject.transform;
        //    contourObject.transform.localPosition = Vector3.zero;
        //}
    }
}
