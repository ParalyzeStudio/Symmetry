using UnityEngine;

public class ChapterSlot : BaseSlot
{
    public const float CHAPTER_SLOT_BACKGROUND_OPACITY = 0.5f;
    public const float FADING_HEXAGONS_GENERATION_TIME_INTERVAL = 1.0f;

    public Chapter m_parentChapter { get; set; }

    //Shared prefabs
    public Material m_positionColorMaterial;
    public GameObject m_circleMeshPfb;

    public GameObject m_infoContainer;
    public GameObject m_chapterTitleText;
    public GameObject m_chapterNumberText;

    public Material m_glowContourMaterial;
    private Vector2 m_blurryContourScale;

    private bool m_slotDismissed;

    //hexagons growing and fading around chapter slot if chapter is unlocked
    private float m_fadingHexagonsGenerationTimeInterval;
    private float m_fadingHexagonsGenerationElapsedTime;

    //progress bar
    public GameObject m_progressBarPfb;
    private ChapterSlotProgressBar m_progressBar;
    private GameObjectAnimator m_progressBarAnimator;

    public void Init(Chapters parentScene)
    {
        base.Init(parentScene);
        m_parentChapter = parentScene.GetLevelManager().GetChapterForNumber(parentScene.m_displayedChapterIndex + 1);

        //Init background mesh
        Color slotBackgroundBlendColor = m_parentChapter.GetThemeColors()[2];

        CircleMesh hexaMesh = m_background.GetComponent<CircleMesh>();
        hexaMesh.Init(Instantiate(m_positionColorMaterial));

        CircleMeshAnimator hexaMeshAnimator = m_background.GetComponent<CircleMeshAnimator>();
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(0, false);
        hexaMeshAnimator.SetOuterRadius(4 * parentScene.GetBackgroundRenderer().m_triangleEdgeLength, true);
        hexaMeshAnimator.SetColor(slotBackgroundBlendColor);
        hexaMeshAnimator.SetPosition(Vector3.zero);

        //Init contour mesh
        //First start with the textured blurry contour
        m_contour.GetComponent<UVQuad>().Init(Instantiate(m_glowContourMaterial));

        TexturedQuadAnimator contourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        float blurryContourTextureScale = 8 * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength / 490.0f; //hexagon is 8 * triangleEdgeLength size, and the contour texture is 496x496 with 8 pixels blur (512x512)
        m_blurryContourScale = new Vector3(blurryContourTextureScale * 512, blurryContourTextureScale * 512, 1);
        contourAnimator.SetScale(m_blurryContourScale);
        contourAnimator.SetPosition(new Vector3(0, 0, -1)); //set the contour above hexagon

        //Set color on texts
        TextMeshAnimator titleAnimator = m_chapterTitleText.GetComponent<TextMeshAnimator>();
        titleAnimator.SetColor(Color.white);
        titleAnimator.SetFontHeight(0.8f * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);
        TextMeshAnimator numberAnimator = m_chapterNumberText.GetComponent<TextMeshAnimator>();
        numberAnimator.SetColor(Color.white);
        numberAnimator.SetFontHeight(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);

        //build the progress bar
        BuildProgressBar();

        //init some variables
        m_fadingHexagonsGenerationElapsedTime = FADING_HEXAGONS_GENERATION_TIME_INTERVAL - 0.5f; //the first hexagon will start fading after some delay (0.5 sec)
        m_fadingHexagonsGenerationTimeInterval = FADING_HEXAGONS_GENERATION_TIME_INTERVAL;
    }

    /**
     * Build the progress bar elements
     * **/
    private void BuildProgressBar()
    {
        GameObject progressBarObject = (GameObject)Instantiate(m_progressBarPfb);
        progressBarObject.name = "ProgressBar";

        m_progressBarAnimator = progressBarObject.GetComponent<GameObjectAnimator>();
        m_progressBarAnimator.SetParentTransform(this.transform);
        m_progressBarAnimator.SetPosition(new Vector3(0, -1.5f * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength, -1));

        float progressBarWidth = 5.5f * m_parentScene.GetBackgroundRenderer().m_triangleHeight;
        float progressBarHeight = 20.0f;
        Vector2 progressBarSize = new Vector2(progressBarWidth, progressBarHeight);
        float fontHeight = 0.33f * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength;

        m_progressBar = progressBarObject.GetComponent<ChapterSlotProgressBar>();
        m_progressBar.Init(this, progressBarSize, fontHeight);
    }

    /**
    * Change the color of slot background with animation
    * **/
    public void UpdateChapterSlotBackgroundColor()
    {
        CircleMeshAnimator chapterSlotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();
        Color toColor = m_parentChapter.GetThemeColors()[2];
        toColor.a = CHAPTER_SLOT_BACKGROUND_OPACITY;
        chapterSlotBackgroundAnimator.ColorChangeTo(toColor, 1.0f);
    }

    /**
     * Replace the texts of the chapter slot with new data
     * **/
    public void UpdateChapterSlotInformation()
    {
        TextMesh chapterTitleTextMesh = m_chapterTitleText.GetComponent<TextMesh>();
        chapterTitleTextMesh.text = LanguageUtils.GetTranslationForTag("chapter");

        TextMesh chapterNumberTextMesh = m_chapterNumberText.GetComponent<TextMesh>();
        chapterNumberTextMesh.text = LanguageUtils.GetLatinNumberForNumber(m_parentChapter.m_number);        
    }

    /**
    * Replace progress bar
    * **/
    public void UpdateProgressBarData()
    {
        m_progressBar.RefreshData();
    }
    
    /**
     * Show lock icon and with a description on how to unlock the chapter
     * **/
    public void ShowLock()
    {

    }

    public override void Show()
    {
        base.Show();
        m_slotDismissed = false;

        GameObjectAnimator slotAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        slotAnimator.TranslateTo(slotAnimator.GetPosition() + new Vector3(0, 100.0f, 0), 0.5f);
    }

    public override void Dismiss(float fDuration, bool bDestroyOnFinish = true)
    {
        base.Dismiss(fDuration, bDestroyOnFinish);
        DismissProgressBar(fDuration, bDestroyOnFinish);
        m_slotDismissed = true;

        //detach the slot from its parent scene
        this.transform.parent = null;

        GameObjectAnimator slotAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        slotAnimator.TranslateTo(slotAnimator.GetPosition() + new Vector3(0, 800.0f, 0), 4.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    /**
     * Fade out the information inside the chapter slot with eventually the background
     * **/
    //public void DismissChapterSlot()
    //{
    //    //translate the slot up
    //    float translationLength = 200.0f;
    //    GameObjectAnimator slotBackgroundAnimator = m_background.GetComponent<GameObjectAnimator>();
    //    slotBackgroundAnimator.TranslateTo(slotBackgroundAnimator.GetPosition() + new Vector3(0, translationLength, 0), 0.5f);
    //    GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
    //    slotInfoContainerAnimator.TranslateTo(slotInfoContainerAnimator.GetPosition() + new Vector3(0, translationLength, 0), 0.5f);

    //    DismissSlotBackground(true);
    //    DismissSlotContour(true);
    //    DismissSlotInformation(true);
    //}

    public override void ShowSlotBackground()
    {
        CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();

        slotBackgroundAnimator.SetOpacity(0);
        slotBackgroundAnimator.FadeTo(CHAPTER_SLOT_BACKGROUND_OPACITY, 0.5f);
    }

    public override void ShowSlotContour()
    {
        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();

        slotContourAnimator.SetOpacity(0);
        slotContourAnimator.FadeTo(1.0f, 0.5f);
    }

    public override void ShowSlotInformation()
    {
        GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();

        slotInfoContainerAnimator.SetOpacity(0);
        slotInfoContainerAnimator.FadeTo(1.0f, 0.5f);
    }

    public void ShowProgressBar()
    {
        m_progressBarAnimator.SetOpacity(0);
        m_progressBarAnimator.FadeTo(1.0f, 0.5f);
    }

    /**
     * Fade out chapter background with eventually destroying the object at zero opacity
     * **/
    public override void DismissSlotBackground(float fDuration, bool bDestroyOnFinish = true)
    {
        CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();
        slotBackgroundAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);

        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Fade out slot contour
     * **/
    public override void DismissSlotContour(float fDuration, bool bDestroyOnFinish = true)
    {
        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Fade out chapter information with eventually destroying the object at zero opacity
     * **/
    public override void DismissSlotInformation(float fDuration, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
        slotInfoContainerAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Fade out progressBar
     * **/
    public void DismissProgressBar(float fDuration, bool bDestroyOnFinish = true)
    {
        m_progressBarAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }
    
    /**
     * Launch a new fading hexagon from the blurry contour of this chapter slot
     * **/
    private void LaunchFadingHexagon()
    {
        GameObject fadingHexagonObject = (GameObject)Instantiate(m_contour); //instantiate a second contour that will serve as a fading object
        fadingHexagonObject.name = "FadingHexagon";

        UVQuad fadingHexagon = fadingHexagonObject.GetComponent<UVQuad>();
        fadingHexagon.Init(Instantiate(m_glowContourMaterial));

        TexturedQuadAnimator fadingHexagonAnimator = fadingHexagonObject.GetComponent<TexturedQuadAnimator>();
        fadingHexagonAnimator.SetParentTransform(this.transform);
        fadingHexagonAnimator.SetScale(m_blurryContourScale);
        fadingHexagonAnimator.SetPosition(new Vector3(0, 0, -1)); //set the contour above hexagon
        fadingHexagonAnimator.SetOpacity(1); //for the moment hide the fading hexagon

        //launch animation on hexagon
        fadingHexagonAnimator.ScaleTo(1.2f * m_blurryContourScale, 3.0f);
        fadingHexagonAnimator.FadeTo(0.0f, 3.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        if (!m_parentChapter.IsLocked() && !m_slotDismissed) //chapter unlocked, generate fading hexagons
        {
            if (m_fadingHexagonsGenerationElapsedTime > m_fadingHexagonsGenerationTimeInterval)
            {
                LaunchFadingHexagon();
                m_fadingHexagonsGenerationElapsedTime = 0.0f;
            }
            else
                m_fadingHexagonsGenerationElapsedTime += dt;
        }
    }
}
