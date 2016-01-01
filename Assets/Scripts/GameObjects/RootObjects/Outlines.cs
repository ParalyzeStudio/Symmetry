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
        Grid grid = this.transform.parent.GetComponent<GameScene>().m_grid;
                
        List<DottedOutline> outlines = levelManager.m_currentLevel.m_outlines;
       
        foreach (DottedOutline outline in outlines)
        {            
            DottedOutline clonedOutline = new DottedOutline(outline);
            //clonedOutline.TogglePointMode(); //switch from grid coordinates to world position
            //clonedOutline.ApproximateVertices(1);
            m_outlinesList.Add(clonedOutline);            
            
            //First triangulate the outline
            clonedOutline.Triangulate();

            //Then draw the outline
            GameObject outlineObject = new GameObject("Outline");

            //Draw the contour
            Contour contourPoints = clonedOutline.m_contour;
            for (int iPointIndex = 0; iPointIndex != contourPoints.Count; iPointIndex++)
            {
                GridPoint gridPointA = contourPoints[iPointIndex];
                GridPoint gridPointB = (iPointIndex == contourPoints.Count - 1) ? contourPoints[0] : contourPoints[iPointIndex + 1];
                Vector2 worldPointA = grid.GetPointWorldCoordinatesFromGridCoordinates(gridPointA);
                Vector2 worldPointB = grid.GetPointWorldCoordinatesFromGridCoordinates(gridPointB);

                GameObject clonedOutlineSegmentObject = (GameObject)Instantiate(m_outlineSegmentPfb);
                OutlineSegment outlineSegment = clonedOutlineSegmentObject.GetComponent<OutlineSegment>();
                outlineSegment.Build(worldPointA, worldPointB, Color.white); //TODO replace white color by contour segment color

                TexturedSegmentAnimator outlineSegmentAnimator = outlineSegment.GetComponent<TexturedSegmentAnimator>();
                outlineSegmentAnimator.SetParentTransform(outlineObject.transform);
            }

            //Draw holes
            List<Contour> holes = clonedOutline.m_holes;
            for (int iHoleIdx = 0; iHoleIdx != holes.Count; iHoleIdx++)
            {
                Contour holePoints = holes[iHoleIdx];
                for (int iHolePointIdx = 0; iHolePointIdx != holePoints.Count; iHolePointIdx++)
                {
                    GridPoint holeGridPointA = holePoints[iHolePointIdx];
                    GridPoint holeGridPointB = (iHolePointIdx == holePoints.Count - 1) ? holePoints[0] : holePoints[iHolePointIdx + 1];
                    Vector2 worldPointA = grid.GetPointWorldCoordinatesFromGridCoordinates(holeGridPointA);
                    Vector2 worldPointB = grid.GetPointWorldCoordinatesFromGridCoordinates(holeGridPointB);

                    GameObject clonedOutlineSegmentObject = (GameObject)Instantiate(m_outlineSegmentPfb);
                    OutlineSegment outlineSegment = clonedOutlineSegmentObject.GetComponent<OutlineSegment>();
                    outlineSegment.Build(worldPointA, worldPointB, Color.white);

                    TexturedSegmentAnimator outlineSegmentAnimator = outlineSegment.GetComponent<TexturedSegmentAnimator>();
                    outlineSegmentAnimator.SetParentTransform(outlineObject.transform);
                }
            }

            GameObjectAnimator outlineAnimator = outlineObject.AddComponent<GameObjectAnimator>();
            outlineAnimator.SetParentTransform(this.transform);
            outlineAnimator.SetPosition(Vector3.zero);
        }
    }

    /**
     * Show the outlines holder
     * **/
    public void Show(bool bAnimated = true, float fDuration = 0.5f, float fDelay = 0.0f)
    {
        GameObjectAnimator outlinesAnimator = this.GetComponent<GameObjectAnimator>();
        if (bAnimated)
        {
            outlinesAnimator.SetOpacity(0);
            outlinesAnimator.FadeTo(1, fDuration, fDelay);
        }
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