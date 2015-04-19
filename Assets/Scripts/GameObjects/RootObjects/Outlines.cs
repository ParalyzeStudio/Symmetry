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

            GameObject outlineObject = new GameObject("Outline");
            List<Vector2> contourPoints = clonedOutline.m_contour;
            for (int iPointIndex = 0; iPointIndex != contourPoints.Count; iPointIndex++)
            {
                Vector2 startPointGrid = contourPoints[iPointIndex];
                Vector2 endPointGrid = (iPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iPointIndex + 1];

                GameObject clonedOutlineSegmentObject = (GameObject)Instantiate(m_outlineSegmentPfb);
                OutlineSegment outlineSegment = clonedOutlineSegmentObject.GetComponent<OutlineSegment>();
                outlineSegment.m_startPointGrid = startPointGrid;
                outlineSegment.m_endPointGrid = endPointGrid;
                outlineSegment.transform.parent = outlineObject.transform;
            }

            outlineObject.transform.parent = this.gameObject.transform;
            outlineObject.transform.localPosition = Vector3.zero;

            outlineObject.AddComponent<GameObjectAnimator>();
        }
    }

    /**
     * Fades out the outlines (when a level ends for instance)
     * **/
    public void Dismiss()
    {
        GameObjectAnimator outlinesAnimator = this.GetComponent<GameObjectAnimator>();
        outlinesAnimator.FadeTo(0, 1.0f, 0.0f);
    }
}

