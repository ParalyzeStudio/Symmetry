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
        GameObject levelManagerObject = GameObject.FindGameObjectWithTag("GameController");
        LevelManager levelManager = levelManagerObject.GetComponent<LevelManager>();
                
        List<DottedOutline> outlines = levelManager.m_currentLevel.m_outlines;
       
        foreach (DottedOutline outline in outlines)
        {            
            DottedOutline clonedOutline = new DottedOutline(outline);
            clonedOutline.TogglePointMode(); //swtich from grid coordinates to world position            
            m_outlinesList.Add(clonedOutline);            
            
            //First triangulate the outline
            clonedOutline.Triangulate();

            //Then draw the outline
            GameObject outlineObject = new GameObject("Outline");

            //Draw the contour
            Contour contourPoints = clonedOutline.m_contour;
            for (int iPointIndex = 0; iPointIndex != contourPoints.Count; iPointIndex++)
            {
                Vector2 gridPointA = contourPoints[iPointIndex];
                Vector2 gridPointB = (iPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iPointIndex + 1];

                GameObject clonedOutlineSegmentObject = (GameObject)Instantiate(m_outlineSegmentPfb);
                OutlineSegment outlineSegment = clonedOutlineSegmentObject.GetComponent<OutlineSegment>();
                outlineSegment.Build(gridPointA, gridPointB, Color.white); //TODO replace white color by contour segment color
                outlineSegment.transform.parent = outlineObject.transform;
            }

            //Draw holes
            List<Contour> holes = clonedOutline.m_holes;
            for (int iHoleIdx = 0; iHoleIdx != holes.Count; iHoleIdx++)
            {
                Contour holePoints = holes[iHoleIdx];
                for (int iHolePointIdx = 0; iHolePointIdx != holePoints.Count; iHolePointIdx++)
                {
                    Vector2 holeGridPointA = holePoints[iHolePointIdx];
                    Vector2 holeGridPointB = (iHolePointIdx == holePoints.Count - 1) ? holePoints[0] : holePoints[iHolePointIdx + 1];

                    GameObject clonedOutlineSegmentObject = (GameObject)Instantiate(m_outlineSegmentPfb);
                    OutlineSegment outlineSegment = clonedOutlineSegmentObject.GetComponent<OutlineSegment>();
                    outlineSegment.Build(holeGridPointA, holeGridPointB, Color.white);
                    outlineSegment.transform.parent = outlineObject.transform;
                }
            }

            outlineObject.transform.parent = this.gameObject.transform;
            outlineObject.transform.localPosition = Vector3.zero;

            outlineObject.AddComponent<GameObjectAnimator>();
        }
    }

    /**
     * Show the outlines holder
     * **/
    public void Show(bool bAnimated = true, float fDuration = 0.5f, float fDelay = 0.0f)
    {
        GameObjectAnimator outlinesAnimator = this.GetComponent<GameObjectAnimator>();
        if (bAnimated)
            outlinesAnimator.FadeTo(1, fDuration, fDelay);
        else
            outlinesAnimator.SetOpacity(1);
    }

    /**
     * Dismiss the outlines holder (when a level ends for instance)
     * **/
    public void Dismiss(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator outlinesAnimator = this.GetComponent<GameObjectAnimator>();
        outlinesAnimator.FadeTo(0, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }
}