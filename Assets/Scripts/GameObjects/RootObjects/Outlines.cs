using UnityEngine;
using System.Collections.Generic;

public class Outlines : MonoBehaviour
{
    public GameObject m_outlineSegmentPfb;
    public List<DottedOutline> m_outlinesList { get; set; }

    public void Awake()
    {
        m_outlinesList = new List<DottedOutline>();
    }

    public void Build()
    {
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("LevelManager");
        LevelManager levelManager = levelManagerObject.GetComponent<LevelManager>();

        List<DottedOutline> outlines = levelManager.m_currentLevel.m_outlines;
        foreach (DottedOutline outline in outlines)
        {
            DottedOutline clonedOutline = new DottedOutline(outline);
            m_outlinesList.Add(clonedOutline);

            //First triangulate the outline
            clonedOutline.Triangulate();

            //Then draw the outline
            GameObject outlineObject = new GameObject("Outline");

            //Draw the contour
            List<Vector2> contourPoints = clonedOutline.m_contour;
            for (int iPointIndex = 0; iPointIndex != contourPoints.Count; iPointIndex++)
            {
                Vector2 gridPointA = contourPoints[iPointIndex];
                Vector2 gridPointB = (iPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iPointIndex + 1];

                GameObject clonedOutlineSegmentObject = (GameObject)Instantiate(m_outlineSegmentPfb);
                OutlineSegment outlineSegment = clonedOutlineSegmentObject.GetComponent<OutlineSegment>();
                outlineSegment.Build(gridPointA, gridPointB);
                outlineSegment.transform.parent = outlineObject.transform;
            }

            //Draw holes
            List<List<Vector2>> holes = clonedOutline.m_holes;
            for (int iHoleIdx = 0; iHoleIdx != holes.Count; iHoleIdx++)
            {
                List<Vector2> holePoints = holes[iHoleIdx];
                for (int iHolePointIdx = 0; iHolePointIdx != holePoints.Count; iHolePointIdx++)
                {
                    Vector2 holeGridPointA = holePoints[iHolePointIdx];
                    Vector2 holeGridPointB = (iHolePointIdx == holePoints.Count - 1) ? holePoints[0] : holePoints[iHolePointIdx + 1];

                    GameObject clonedOutlineSegmentObject = (GameObject)Instantiate(m_outlineSegmentPfb);
                    OutlineSegment outlineSegment = clonedOutlineSegmentObject.GetComponent<OutlineSegment>();
                    outlineSegment.Build(holeGridPointA, holeGridPointB);
                    outlineSegment.transform.parent = outlineObject.transform;
                }
            }

            outlineObject.transform.parent = this.gameObject.transform;
            outlineObject.transform.localPosition = Vector3.zero;

            outlineObject.AddComponent<GameObjectAnimator>();
        }
    }

    /**
     * Fades out the outlines (when a level ends for instance)
     * **/
    public void Dismiss(float fDuration, float fDelay)
    {
        GameObjectAnimator outlinesAnimator = this.GetComponent<GameObjectAnimator>();
        outlinesAnimator.FadeTo(0, fDuration, fDelay);
    }
}

