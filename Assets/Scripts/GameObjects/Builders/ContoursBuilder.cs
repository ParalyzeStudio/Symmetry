using UnityEngine;
using System.Collections.Generic;

public class ContoursBuilder : MonoBehaviour
{
    public GameObject m_contourSegmentPfb;

    public void Build()
    {
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        LevelManager levelManager = levelManagerObject.GetComponent<LevelManager>();

        ContoursHolder contoursHolder = this.gameObject.GetComponent<ContoursHolder>();
        List<Contour> contours = levelManager.m_currentLevel.m_contours;
        foreach (Contour contour in contours)
        {
            GameObject contourObject = new GameObject("Contour");
            List<Vector2> contourPoints = contour.m_points;
            for (int iPointIndex = 0; iPointIndex != contourPoints.Count; iPointIndex++)
            {
                Vector2 startPointGrid = contourPoints[iPointIndex];
                Vector2 endPointGrid = (iPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iPointIndex + 1];

                GameObject clonedContourSegmentObject = (GameObject)Instantiate(m_contourSegmentPfb);
                ContourSegment contourSegment = clonedContourSegmentObject.GetComponent<ContourSegment>();
                contourSegment.m_startPointGrid = startPointGrid;
                contourSegment.m_endPointGrid = endPointGrid;
                contourSegment.transform.parent = contourObject.transform;
            }

            contourObject.transform.parent = this.gameObject.transform;
            contourObject.transform.localPosition = Vector3.zero;

            contoursHolder.AddContour(contourObject);
        }
    }
}

