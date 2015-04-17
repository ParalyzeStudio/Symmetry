using UnityEngine;
using System.Collections.Generic;

public class Contours : MonoBehaviour
{
    public GameObject m_contourSegmentPfb;

    public void Build()
    {
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        LevelManager levelManager = levelManagerObject.GetComponent<LevelManager>();

        List<DottedOutline> contours = levelManager.m_currentLevel.m_outlines;
        foreach (DottedOutline contour in contours)
        {
            //First triangulate the contour
            contour.Triangulate();

            GameObject contourObject = new GameObject("Contour");
            List<Vector2> contourPoints = contour.m_contour;
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

            contourObject.AddComponent<GameObjectAnimator>();
        }
    }
}

