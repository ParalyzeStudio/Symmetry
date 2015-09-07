using UnityEngine;

public class LevelSlot : BaseSlot
{
    public const float LEVEL_SLOT_BACKGROUND_OPACITY = 0.5f;

    private int m_levelNumber;

    //Shared prefabs
    public GameObject m_circleMeshPfb;

    public GameObject m_levelNumberText;

    public void Init(GUIScene parentScene, int iLevelNumber)
    {
        base.Init(parentScene);
        m_levelNumber = iLevelNumber;

        //Add a background only if level is completed        
        //int iAbsoluteLevelNumber = m_parentScene.GetLevelManager().GetAbsoluteLevelNumberForCurrentChapterAndLevel(iLevelNumber);
        //if (GetPersistentDataManager().IsLevelDone(iAbsoluteLevelNumber))

        Color slotColor = m_parentScene.GetLevelManager().m_currentChapter.GetThemeColors()[2];
        
        CircleMesh slotBgHexaMesh = m_background.GetComponent<CircleMesh>();
        slotBgHexaMesh.Init(((Levels)m_parentScene).m_slotBackgroundMaterial);

        CircleMeshAnimator slotBgAnimator = m_background.GetComponent<CircleMeshAnimator>();
        slotBgAnimator.SetNumSegments(6, false);
        slotBgAnimator.SetInnerRadius(0, false);
        slotBgAnimator.SetOuterRadius(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength, true);
        slotBgAnimator.SetColor(slotColor);

        //Init contour mesh
        //First start with the textured blurry contour
        m_contour.GetComponent<UVQuad>().Init(((Levels)m_parentScene).m_slotGlowContourMaterial);

        TexturedQuadAnimator glowContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        float contourTextureScale = 2 * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength / 168.0f; //hexagon is 8 * triangleEdgeLength size, and the contour texture is 496x496 with 8 pixels blur (512x512)
        glowContourAnimator.SetScale(new Vector3(contourTextureScale * 256, contourTextureScale * 256, 1));
        glowContourAnimator.SetPosition(new Vector3(0, 0, -1)); //set the contour above hexagon

        //Then create a sharp contour with some thickness
        GameObject contourHexaObject = Instantiate(m_circleMeshPfb);
        contourHexaObject.name = "ContourHexagon";
        contourHexaObject.transform.parent = m_contour.transform;

        CircleMesh sharpContour = contourHexaObject.GetComponent<CircleMesh>();
        sharpContour.Init(((Levels)m_parentScene).m_slotSharpContourMaterial);

        float contourMeshThickness = 8.0f;
        CircleMeshAnimator sharpContourAnimator = contourHexaObject.GetComponent<CircleMeshAnimator>();
        sharpContourAnimator.SetNumSegments(6, false);
        sharpContourAnimator.SetInnerRadius(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength, false);
        sharpContourAnimator.SetOuterRadius(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength + contourMeshThickness, true);
        sharpContourAnimator.SetColor(Color.white);
        sharpContourAnimator.SetPosition(Vector3.zero);

        //Set color on texts
        TextMeshAnimator numberAnimator = m_levelNumberText.GetComponent<TextMeshAnimator>();
        numberAnimator.SetColor(Color.white);
        numberAnimator.SetFontHeight(0.8f * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);

        AdjustNumberPosition(iLevelNumber, numberAnimator);
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Dismiss()
    {
        base.Dismiss();
    }

    protected override void ShowSlotBackground()
    {
        CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();

        slotBackgroundAnimator.SetOpacity(0);
        slotBackgroundAnimator.FadeTo(LEVEL_SLOT_BACKGROUND_OPACITY, 0.5f);

        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();

        slotContourAnimator.SetOpacity(0);
        slotContourAnimator.FadeTo(1.0f, 0.5f);
    }

    protected override void ShowSlotInformation()
    {
        GameObjectAnimator slotInfoContainerAnimator = m_levelNumberText.GetComponent<GameObjectAnimator>();

        slotInfoContainerAnimator.SetOpacity(0);
        slotInfoContainerAnimator.FadeTo(1.0f, 0.5f);
    }

    protected override void DismissSlotBackground(bool bDestroyOnFinish)
    {
        CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();
        slotBackgroundAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);

        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    protected override void DismissSlotInformation(bool bDestroyOnFinish)
    {
        GameObjectAnimator slotInfoContainerAnimator = m_levelNumberText.GetComponent<GameObjectAnimator>();
        slotInfoContainerAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }


    /**
     * Slightly offset number positions so they appeared to be centered
     * **/
    private void AdjustNumberPosition(int iLevelNumber, TextMeshAnimator numberAnimator)
    {
        if (iLevelNumber == 11)
            numberAnimator.SetPosition(new Vector3(-3.0f, 0, -1));
        else if (iLevelNumber >= 10)
            numberAnimator.SetPosition(new Vector3(-5.0f, 0, -1));
        else
            numberAnimator.SetPosition(new Vector3(0, 0, -1));
    }
}
