using UnityEngine;

public class LevelSlot : GUISlot
{

    public int m_number { get; set; }
    public GameObject m_levelSlotNumberGameObject { get; set; }

    public LevelSlot(int number)
    {
        m_number = number;
    }

    /**
     * Show the triangles of this slot by blending their color with m_blendColor
     * Show also the number on each slot
     * **/
    public override void Show(Color blendColor, float blendColorProportion, bool bAnimated = true, float fDuration = 1.0f, float fDelay = 0.0f)
    {
        if (m_triangles == null)
            throw new System.Exception("Slot triangles have to be built first");

        m_blendColor = blendColor;
        m_blendColorProportion = blendColorProportion;

        //Show triangles
        ShowBackground(blendColor, blendColorProportion, bAnimated, fDuration, fDelay);

        //Fade in number
        TextMeshAnimator slotTextAnimator = m_levelSlotNumberGameObject.GetComponent<TextMeshAnimator>();
        if (bAnimated)
        {
            slotTextAnimator.SetOpacity(0);
            slotTextAnimator.FadeTo(1.0f, fDuration, fDelay);
        }
        else
            slotTextAnimator.SetOpacity(1);
    }

    /**
     * Hide triangles by going back to their original color
     * Hide also numbers
     * **/
    public override void Dismiss(bool bDismissBackground = true, bool bAnimated = true, float fDuration = 1.0f, float fDelay = 0.0f)
    {
        if (bDismissBackground)
            DismissBackground(bAnimated, fDuration, fDelay);

        //Fade out and destroy number
        TextMeshAnimator slotTextAnimator = m_levelSlotNumberGameObject.GetComponent<TextMeshAnimator>();
        if (bAnimated)
            slotTextAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, true);
        else
            Destroy(slotTextAnimator.gameObject);
    }

    /**
     * Build the hexagon slot taking the reference triangle parameter as a reference
     * **/
    public void BuildTrianglesFromFarRightTriangle(int iReferenceTriangleColumnIndex, int iReferenceTriangleIndex)
    {
        m_triangles = new BackgroundTriangle[6]; //hexagon

        BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        //reference/far right triangle
        m_triangles[0] = bgRenderer.m_triangleColumns[iReferenceTriangleColumnIndex][iReferenceTriangleIndex];

        //top right triangle
        m_triangles[1] = bgRenderer.m_triangleColumns[iReferenceTriangleColumnIndex][iReferenceTriangleIndex - 1];

        //top left triangle
        m_triangles[2] = bgRenderer.m_triangleColumns[iReferenceTriangleColumnIndex - 1][iReferenceTriangleIndex - 1];

        //far left triangle
        m_triangles[3] = bgRenderer.m_triangleColumns[iReferenceTriangleColumnIndex - 1][iReferenceTriangleIndex];

        //bottom left triangle
        m_triangles[4] = bgRenderer.m_triangleColumns[iReferenceTriangleColumnIndex - 1][iReferenceTriangleIndex + 1];

        //bottom right triangle
        m_triangles[5] = bgRenderer.m_triangleColumns[iReferenceTriangleColumnIndex][iReferenceTriangleIndex + 1];
    }
}
