using UnityEngine;
using System.Collections.Generic;

public class ChapterSlot : GUISlot
{
    public int m_number { get; set; }
    public GameObject m_infoContainer { get; set; }

    public override void Show(Color blendColor, float blendColorProportion, bool bAnimated = true, float fDuration = 1.0f, float fDelay = 0.0f)
    {
        ShowBackground(blendColor, blendColorProportion, bAnimated, fDuration, fDelay);

        GameObjectAnimator infoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
        infoContainerAnimator.SetOpacity(0);
        infoContainerAnimator.FadeTo(1.0f, fDuration, fDelay);
    }

    public override void Dismiss(bool bDismissBackground = false, bool bAnimated = true, float fDuration = 1.0f, float fDelay = 0.0f)
    {
        if (bDismissBackground)
            DismissBackground(bAnimated, fDuration, fDelay);

        GameObjectAnimator infoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
        if (bAnimated)
            infoContainerAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, true);
        else
            Destroy(infoContainerAnimator.gameObject);
    }

    public void BuildTriangles()
    {
        //Find triangles whose color needs to be modified in order to draw the item hexagon background
        BackgroundTrianglesRenderer backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        int triangleCount = 2 * 15 + 2 * 13 + 2 * 11 + 2 * 9;
        m_triangles = new BackgroundTriangle[triangleCount];

        //color to blend every triangle inside the item
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        int iTriangleIdx = 0;

        //column 1
        int columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2;
        BackgroundTriangleColumn column = backgroundRenderer.m_triangleColumns[BackgroundTrianglesRenderer.NUM_COLUMNS / 2];
        BackgroundTriangle triangle = backgroundRenderer.GetNearestTriangleToScreenYPosition(0, columnIndex, 180);
        int referenceTriangleIndex = triangle.m_indexInColumn; //the index of the triangle taken as a reference for building the column
        for (int i = referenceTriangleIndex - 7; i != referenceTriangleIndex + 8; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }

        //column -1
        columnIndex--;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        for (int i = referenceTriangleIndex - 7; i != referenceTriangleIndex + 8; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }

        //column -2
        columnIndex--;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        for (int i = referenceTriangleIndex - 6; i != referenceTriangleIndex + 7; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }

        //column -3
        columnIndex--;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        for (int i = referenceTriangleIndex - 5; i != referenceTriangleIndex + 6; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }

        //column -4
        columnIndex--;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        for (int i = referenceTriangleIndex - 4; i != referenceTriangleIndex + 5; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }

        //column 2
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 + 1;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        for (int i = referenceTriangleIndex - 6; i != referenceTriangleIndex + 7; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }

        //column 3
        columnIndex++;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        for (int i = referenceTriangleIndex - 5; i != referenceTriangleIndex + 6; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }

        //column 4
        columnIndex++;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        for (int i = referenceTriangleIndex - 4; i != referenceTriangleIndex + 5; i++)
        {
            m_triangles[iTriangleIdx] = column[i];
            iTriangleIdx++;
        }
    }   
}
