using UnityEngine;

public class ChapterSlot : BaseSlot
{
    public const float CHAPTER_SLOT_BACKGROUND_OPACITY = 0.5f;
    public const float FADING_HEXAGONS_GENERATION_TIME_INTERVAL = 1.0f;

    //Shared prefabs
    public Material m_positionColorMaterial;
    public GameObject m_circleMeshPfb;

    public GameObject m_infoContainer;
    public GameObject m_chapterTitleText;
    public GameObject m_chapterNumberText;

    public Material m_glowContourMaterial;

    private Vector2 m_blurryContourScale;

    //hexagons growing and fading around chapter slot if chapter is unlocked
    private GameObject m_blurryContourObject;
    //private bool m_generatingFadingHexagons;
    private float m_fadingHexagonsGenerationTimeInterval;
    private float m_fadingHexagonsGenerationElapsedTime;

    //progress bar
    //private GameObject m_progressBar;
    //private Vector2 m_progressBarSize;
    //public Material m_progressBarMaterial;
    //public GameObject m_progressBarBackground;
    //public GameObject m_progressBarFill;

    public void Init(Chapters parentScene)
    {
        base.Init(parentScene);
        int chapterNumber = parentScene.m_displayedChapterIndex + 1;

        //Init background mesh
        Color slotBackgroundBlendColor = parentScene.GetLevelManager().GetChapterForNumber(chapterNumber).GetThemeColors()[2];

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

        m_blurryContourObject = Instantiate(m_contour); //copy the contour at this current state (without adding the sharp contour) to create from this model all fading hexagons

        //Then create a sharp contour with some thickness
        GameObject contourHexaObject = Instantiate(m_circleMeshPfb);
        contourHexaObject.name = "ContourHexagon";

        CircleMesh contourMesh = contourHexaObject.GetComponent<CircleMesh>();
        contourMesh.Init(Instantiate(m_positionColorMaterial));

        float contourThickness = 8.0f;
        CircleMeshAnimator contourMeshAnimator = contourHexaObject.GetComponent<CircleMeshAnimator>();
        contourMeshAnimator.SetParentTransform(m_contour.transform);
        contourMeshAnimator.SetNumSegments(6, false);
        contourMeshAnimator.SetInnerRadius(4 * parentScene.GetBackgroundRenderer().m_triangleEdgeLength, false);
        contourMeshAnimator.SetOuterRadius(4 * parentScene.GetBackgroundRenderer().m_triangleEdgeLength + contourThickness, true);
        contourMeshAnimator.SetColor(Color.white);
        contourMeshAnimator.SetPosition(Vector3.zero);

        //Set color on texts
        TextMeshAnimator titleAnimator = m_chapterTitleText.GetComponent<TextMeshAnimator>();
        titleAnimator.SetColor(Color.white);
        titleAnimator.SetFontHeight(0.8f * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);
        TextMeshAnimator numberAnimator = m_chapterNumberText.GetComponent<TextMeshAnimator>();
        numberAnimator.SetColor(Color.white);
        numberAnimator.SetFontHeight(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);

        //init some variables
        m_fadingHexagonsGenerationElapsedTime = FADING_HEXAGONS_GENERATION_TIME_INTERVAL - 0.5f; //the first hexagon will start fading after some delay (0.5 sec)
        m_fadingHexagonsGenerationTimeInterval = FADING_HEXAGONS_GENERATION_TIME_INTERVAL;
    }

    /**
     * Build the progress bar elements
     * **/
    //private void BuildProgressBar()
    //{
    //    BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

    //    m_progressBar = new GameObject("ProgressBar");

    //    GameObjectAnimator progressBarAnimator = m_progressBar.AddComponent<GameObjectAnimator>();
    //    progressBarAnimator.SetParentTransform(m_infoContainer.transform);
    //    progressBarAnimator.SetPosition(new Vector3(0, -1.5f * bgRenderer.m_triangleEdgeLength, 0));

    //    float progressBarWidth = 5.5f * bgRenderer.m_triangleHeight;
    //    float progressBarHeight = 20.0f;
    //    m_progressBarSize = new Vector2(progressBarWidth, progressBarHeight);

    //    Material progressBarBgMaterial = Instantiate(m_progressBarMaterial);
    //    Material progressBarFillMaterial = Instantiate(m_progressBarMaterial);

    //    //Background
    //    m_progressBarBackgroundObject = Instantiate(m_progressBarBgGameObject);

    //    ColorQuad colorQuad = m_progressBarBackgroundObject.GetComponent<ColorQuad>();
    //    colorQuad.Init(progressBarBgMaterial);

    //    ColorQuadAnimator progressBarBgAnimator = m_progressBarBackgroundObject.GetComponent<ColorQuadAnimator>();
    //    progressBarBgAnimator.SetParentTransform(m_progressBar.transform);
    //    progressBarBgAnimator.SetPosition(Vector3.zero);
    //    progressBarBgAnimator.SetScale(m_progressBarSize);

    //    //Fill
    //    m_progressBarFillObject = Instantiate(m_progressBarBgGameObject);

    //    colorQuad = m_progressBarFillObject.GetComponent<ColorQuad>();
    //    colorQuad.Init(progressBarFillMaterial);

    //    ColorQuadAnimator progressBarFillAnimator = m_progressBarFillObject.GetComponent<ColorQuadAnimator>();
    //    progressBarFillAnimator.SetParentTransform(m_progressBar.transform);
    //    progressBarFillAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
    //    progressBarFillAnimator.SetPosition(new Vector3(-0.5f * progressBarWidth, 0, -1));
    //    progressBarFillAnimator.SetColor(Color.white);

    //    //completion info
    //    m_progressBarCompletionTextObject = Instantiate(m_textMeshPfb);

    //    TextMesh completionTextMesh = m_progressBarCompletionTextObject.GetComponent<TextMesh>();
    //    completionTextMesh.text = (0.5f * 100) + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");

    //    TextMeshAnimator completionTextAnimator = m_progressBarCompletionTextObject.GetComponent<TextMeshAnimator>();
    //    float fontHeight = 0.33f * bgRenderer.m_triangleEdgeLength;
    //    completionTextAnimator.SetParentTransform(m_progressBar.transform);
    //    completionTextAnimator.SetFontHeight(fontHeight);
    //    completionTextAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.5f));
    //    completionTextAnimator.SetPosition(new Vector3(0.5f * progressBarWidth, -0.5f * progressBarHeight - fontHeight - 8.0f, 0));
    //    completionTextAnimator.SetColor(Color.white);
    //}

    /**
    * Change the color of slot background with animation
    * **/
    public void UpdateChapterSlotBackgroundColor()
    {
        int chapterNumber = ((Chapters)m_parentScene).m_displayedChapterIndex + 1;
        Chapter displayedChapter = m_parentScene.GetLevelManager().GetChapterForNumber(chapterNumber);

        CircleMeshAnimator chapterSlotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();
        Color toColor = displayedChapter.GetThemeColors()[2];
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

        int chapterNumber = ((Chapters)m_parentScene).m_displayedChapterIndex + 1;
        TextMesh chapterNumberTextMesh = m_chapterNumberText.GetComponent<TextMesh>();
        chapterNumberTextMesh.text = LanguageUtils.GetLatinNumberForNumber(chapterNumber);

        //Display Lock or progress bar
        //Chapter displayedChapter = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);
        //if (displayedChapter.IsLocked())
        //    ShowLock();
        //else
        //    UpdateProgressBarData();
    }

    public override void Show()
    {
        base.Show();

        GameObjectAnimator slotAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        slotAnimator.TranslateTo(slotAnimator.GetPosition() + new Vector3(0, 100.0f, 0), 0.5f);
    }

    public override void Dismiss()
    {
        base.Dismiss();

        GameObjectAnimator slotAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        slotAnimator.TranslateTo(slotAnimator.GetPosition() + new Vector3(0, 100.0f, 0), 0.5f);
    }

    /**
     * Fade out the information inside the chapter slot with eventually the background
     * **/
    public void DismissChapterSlot()
    {
        //translate the slot up
        float translationLength = 200.0f;
        GameObjectAnimator slotBackgroundAnimator = m_background.GetComponent<GameObjectAnimator>();
        slotBackgroundAnimator.TranslateTo(slotBackgroundAnimator.GetPosition() + new Vector3(0, translationLength, 0), 0.5f);
        GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
        slotInfoContainerAnimator.TranslateTo(slotInfoContainerAnimator.GetPosition() + new Vector3(0, translationLength, 0), 0.5f);

        DismissSlotBackground(true);
        DismissSlotContour(true);
        DismissSlotInformation(true);
    }

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

    /**
     * Fade out chapter background with eventually destroying the object at zero opacity
     * **/
    public override void DismissSlotBackground(bool bDestroyOnFinish)
    {
        CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();
        slotBackgroundAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);

        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    public override void DismissSlotContour(bool bDestroyOnFinish)
    {
        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Fade out chapter information with eventually destroying the object at zero opacity
     * **/
    public override void DismissSlotInformation(bool bDestroyOnFinish)
    {
        GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
        slotInfoContainerAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }
    
    /**
     * Launch a new fading hexagon from the blurry contour of this chapter slot
     * **/
    private void LaunchFadingHexagon()
    {
        GameObject fadingHexagonObject = (GameObject)Instantiate(m_blurryContourObject); //instantiate a second contour that will serve as a fading object
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

    /**
     * Update progress bar data
     * **/
    //public void UpdateProgressBarData()
    //{
    //    float progressBarWidth = m_progressBarSize.x;
    //    float progressBarHeight = m_progressBarSize.y;
    //    float ratio = 0.5f;

    //    //Background
    //    ColorQuadAnimator progressBarBgAnimator = m_progressBarBackgroundObject.GetComponent<ColorQuadAnimator>();
    //    Color bgColor = GetCurrentlyDisplayedChapter().GetThemeColors()[3];
    //    bgColor.a = progressBarBgAnimator.m_color.a;
    //    progressBarBgAnimator.SetColor(bgColor);

    //    //Fill
    //    ColorQuadAnimator progressBarFillAnimator = m_progressBarFillObject.GetComponent<ColorQuadAnimator>();
    //    progressBarFillAnimator.SetScale(new Vector3(ratio * progressBarWidth, progressBarHeight, 1));

    //    //completion info
    //    TextMesh completionTextMesh = m_progressBarCompletionTextObject.GetComponent<TextMesh>();
    //    completionTextMesh.text = (ratio * 100) + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");
    //}

    /**
     * Show lock icon and with a description on how to unlock the chapter
     * **/
    //private void ShowLock()
    //{

    //}

    public void Update()
    {
        float dt = Time.deltaTime;

        int chapterNumber = ((Chapters)m_parentScene).m_displayedChapterIndex + 1;
        Chapter displayedChapter = m_parentScene.GetLevelManager().GetChapterForNumber(chapterNumber);
        if (!displayedChapter.IsLocked()) //chapter unlocked, generate fading hexagons
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
