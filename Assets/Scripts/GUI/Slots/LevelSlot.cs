using UnityEngine;

public class LevelSlot : BaseSlot
{
    public const float LEVEL_SLOT_BACKGROUND_OPACITY = 0.75f;

    private int m_levelNumber;

    //Shared prefabs
    public GameObject m_circleMeshPfb;

    public GameObject m_levelNumberText;

    public void Init(GUIScene parentScene, int iLevelNumber)
    {
        base.Init(parentScene);
        m_levelNumber = iLevelNumber;

        //Add a background only if level is completed        
        int iAbsoluteLevelNumber = m_parentScene.GetLevelManager().GetAbsoluteLevelNumberForCurrentChapterAndLevel(iLevelNumber);
        CircleMeshAnimator slotBgAnimator = m_background.GetComponent<CircleMeshAnimator>();
        if (m_parentScene.GetPersistentDataManager().IsLevelDone(iAbsoluteLevelNumber))
        {
            Color slotColor = m_parentScene.GetLevelManager().m_currentChapter.GetThemeColors()[2];

            CircleMesh slotBgHexaMesh = m_background.GetComponent<CircleMesh>();
            slotBgHexaMesh.Init(((Levels)m_parentScene).m_slotBackgroundMaterial);

            slotBgAnimator.SetNumSegments(6, false);
            slotBgAnimator.SetInnerRadius(0, false);
            slotBgAnimator.SetOuterRadius(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength, true);
            slotBgAnimator.SetColor(slotColor);
        }
        else
        {
            m_background.transform.parent = null;
            Destroy(m_background);
            m_background = null;
        }

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

        CircleMesh sharpContour = contourHexaObject.GetComponent<CircleMesh>();
        sharpContour.Init(((Levels)m_parentScene).m_slotSharpContourMaterial);

        float contourThickness = 4.0f;
        CircleMeshAnimator sharpContourAnimator = contourHexaObject.GetComponent<CircleMeshAnimator>();
        sharpContourAnimator.SetParentTransform(m_contour.transform);
        sharpContourAnimator.SetNumSegments(6, false);
        sharpContourAnimator.SetInnerRadius(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength, false);
        sharpContourAnimator.SetOuterRadius(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength + contourThickness, true);
        sharpContourAnimator.SetColor(Color.white);
        sharpContourAnimator.SetPosition(Vector3.zero);

        //Set correct level number
        m_levelNumberText.GetComponent<TextMesh>().text = m_levelNumber.ToString();

        //Set color on texts
        TextMeshAnimator numberAnimator = m_levelNumberText.GetComponent<TextMeshAnimator>();
        numberAnimator.SetColor(Color.white);
        numberAnimator.SetFontHeight(0.8f * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);

        AdjustNumberPosition(iLevelNumber, numberAnimator);
    }

    public override void Show()
    {
        base.Show();

        GameObjectAnimator slotAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        slotAnimator.SetScale(new Vector3(0, 0, 1));
        slotAnimator.SetOpacity(0);

        float localDelay = (m_levelNumber - 1) * 0.025f;
        slotAnimator.ScaleTo(new Vector3(1, 1, 1), 0.5f, localDelay);
    }

    public override void ShowSlotBackground()
    {
        if (m_background != null)
        {
            CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();

            slotBackgroundAnimator.SetOpacity(0);
            slotBackgroundAnimator.FadeTo(LEVEL_SLOT_BACKGROUND_OPACITY, 0.5f);
        }
    }

    public override void ShowSlotContour()
    {
        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();

        slotContourAnimator.SetOpacity(0);
        slotContourAnimator.FadeTo(1.0f, 0.5f);
    }

    public override void ShowSlotInformation()
    {
        GameObjectAnimator slotInfoContainerAnimator = m_levelNumberText.GetComponent<GameObjectAnimator>();

        slotInfoContainerAnimator.SetOpacity(0);
        slotInfoContainerAnimator.FadeTo(1.0f, 0.5f);
    }

    public override void DismissSlotBackground(float fDuration, bool bDestroyOnFinish = true)
    {
        if (m_background != null)
        {
            CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();
            slotBackgroundAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
        }
    }

    public override void DismissSlotContour(float fDuration, bool bDestroyOnFinish)
    {
        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    public override void DismissSlotInformation(float fDuration, bool bDestroyOnFinish)
    {
        GameObjectAnimator slotInfoContainerAnimator = m_levelNumberText.GetComponent<GameObjectAnimator>();
        slotInfoContainerAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
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
