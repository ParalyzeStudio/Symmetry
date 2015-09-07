using UnityEngine;

public class ChapterSlot : BaseSlot
{
    public const float CHAPTER_SLOT_BACKGROUND_OPACITY = 0.5f;

    //Shared prefabs
    public Material m_positionColorMaterial;
    public GameObject m_circleMeshPfb;

    public GameObject m_infoContainer;
    public GameObject m_chapterTitleText;
    public GameObject m_chapterNumberText;

    public Material m_glowContourMaterial;

    private int m_chapterNumber;

    //progress bar
    //private GameObject m_progressBar;
    //private Vector2 m_progressBarSize;
    //public Material m_progressBarMaterial;
    //public GameObject m_progressBarBackground;
    //public GameObject m_progressBarFill;

    public void Init(GUIScene parentScene, int iChapterNumber)
    {
        base.Init(parentScene);
        m_chapterNumber = iChapterNumber;

        //Init background mesh
        Color slotBackgroundBlendColor = parentScene.GetLevelManager().GetChapterForNumber(iChapterNumber).GetThemeColors()[2];

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
        float contourTextureScale = 8 * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength / 490.0f; //hexagon is 8 * triangleEdgeLength size, and the contour texture is 496x496 with 8 pixels blur (512x512)
        contourAnimator.SetScale(new Vector3(contourTextureScale * 512, contourTextureScale * 512, 1));
        contourAnimator.SetPosition(new Vector3(0, 0, -1)); //set the contour above hexagon

        //Then create a sharp contour with some thickness
        GameObject contourHexaObject = Instantiate(m_circleMeshPfb);
        contourHexaObject.name = "ContourHexagon";
        contourHexaObject.transform.parent = m_contour.transform;

        CircleMesh contourMesh = contourHexaObject.GetComponent<CircleMesh>();
        contourMesh.Init(Instantiate(m_positionColorMaterial));

        float contourMeshThickness = 8.0f;
        CircleMeshAnimator contourMeshAnimator = contourHexaObject.GetComponent<CircleMeshAnimator>();
        contourMeshAnimator.SetNumSegments(6, false);
        contourMeshAnimator.SetInnerRadius(4 * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength, false);
        contourMeshAnimator.SetOuterRadius(4 * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength + contourMeshThickness, true);
        contourMeshAnimator.SetColor(Color.white);
        contourMeshAnimator.SetPosition(Vector3.zero);

        //Set color on texts
        TextMeshAnimator titleAnimator = m_chapterTitleText.GetComponent<TextMeshAnimator>();
        titleAnimator.SetColor(Color.white);
        titleAnimator.SetFontHeight(0.8f * m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);
        TextMeshAnimator numberAnimator = m_chapterNumberText.GetComponent<TextMeshAnimator>();
        numberAnimator.SetColor(Color.white);
        numberAnimator.SetFontHeight(m_parentScene.GetBackgroundRenderer().m_triangleEdgeLength);
    }

    /**
     * Build the progress bar elements
     * **/
    //private void BuildProgressBar()
    //{
    //    BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

    //    m_progressBar = new GameObject("ProgressBar");
    //    m_progressBar.transform.parent = m_infoContainer.transform;

    //    GameObjectAnimator progressBarAnimator = m_progressBar.AddComponent<GameObjectAnimator>();
    //    progressBarAnimator.SetPosition(new Vector3(0, -1.5f * bgRenderer.m_triangleEdgeLength, 0));

    //    float progressBarWidth = 5.5f * bgRenderer.m_triangleHeight;
    //    float progressBarHeight = 20.0f;
    //    m_progressBarSize = new Vector2(progressBarWidth, progressBarHeight);

    //    Material progressBarBgMaterial = Instantiate(m_progressBarMaterial);
    //    Material progressBarFillMaterial = Instantiate(m_progressBarMaterial);

    //    //Background
    //    m_progressBarBackgroundObject = Instantiate(m_progressBarBgGameObject);
    //    m_progressBarBackgroundObject.transform.parent = m_progressBar.transform;

    //    ColorQuad colorQuad = m_progressBarBackgroundObject.GetComponent<ColorQuad>();
    //    colorQuad.Init(progressBarBgMaterial);

    //    ColorQuadAnimator progressBarBgAnimator = m_progressBarBackgroundObject.GetComponent<ColorQuadAnimator>();
    //    progressBarBgAnimator.SetPosition(Vector3.zero);
    //    progressBarBgAnimator.SetScale(m_progressBarSize);

    //    //Fill
    //    m_progressBarFillObject = Instantiate(m_progressBarBgGameObject);
    //    m_progressBarFillObject.transform.parent = m_progressBar.transform;

    //    colorQuad = m_progressBarFillObject.GetComponent<ColorQuad>();
    //    colorQuad.Init(progressBarFillMaterial);

    //    ColorQuadAnimator progressBarFillAnimator = m_progressBarFillObject.GetComponent<ColorQuadAnimator>();
    //    progressBarFillAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
    //    progressBarFillAnimator.SetPosition(new Vector3(-0.5f * progressBarWidth, 0, -1));
    //    progressBarFillAnimator.SetColor(Color.white);

    //    //completion info
    //    m_progressBarCompletionTextObject = Instantiate(m_textMeshPfb);
    //    m_progressBarCompletionTextObject.transform.parent = m_progressBar.transform;

    //    TextMesh completionTextMesh = m_progressBarCompletionTextObject.GetComponent<TextMesh>();
    //    completionTextMesh.text = (0.5f * 100) + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");

    //    TextMeshAnimator completionTextAnimator = m_progressBarCompletionTextObject.GetComponent<TextMeshAnimator>();
    //    float fontHeight = 0.33f * bgRenderer.m_triangleEdgeLength;
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
        Chapter displayedChapter = m_parentScene.GetLevelManager().GetChapterForNumber(m_chapterNumber);

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

        TextMesh chapterNumberTextMesh = m_chapterNumberText.GetComponent<TextMesh>();
        chapterNumberTextMesh.text = LanguageUtils.GetLatinNumberForNumber(m_chapterNumber);

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
    //public void DismissChapterSlot(bool bDismissBackground = true, bool bDestroyBackgroundOnFinish = true, bool bDestroyChapterInfoOnFinish = true)
    //{
    //    if (bDismissBackground)
    //    {
    //        DismissSlotBackground(bDestroyBackgroundOnFinish);

    //        //translate the slot up
    //        float translationLength = 200.0f;
    //        GameObjectAnimator slotBackgroundAnimator = m_background.GetComponent<GameObjectAnimator>();
    //        slotBackgroundAnimator.TranslateTo(slotBackgroundAnimator.GetPosition() + new Vector3(0, translationLength, 0), 0.5f);
    //        GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
    //        slotInfoContainerAnimator.TranslateTo(slotInfoContainerAnimator.GetPosition() + new Vector3(0, translationLength, 0), 0.5f);
    //    }

    //    DismissSlotInformation(bDestroyChapterInfoOnFinish);
    //}

    /**
     * Show the chapter slot hexagon background 
     * **/
    protected override void ShowSlotBackground()
    {
        CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();

        slotBackgroundAnimator.SetOpacity(0);
        slotBackgroundAnimator.FadeTo(CHAPTER_SLOT_BACKGROUND_OPACITY, 0.5f);

        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();

        slotContourAnimator.SetOpacity(0);
        slotContourAnimator.FadeTo(1.0f, 0.5f);
    }

    /**
     * Show the chapter slot hexagon background 
     * **/
    protected override void ShowSlotInformation()
    {
        GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();

        slotInfoContainerAnimator.SetOpacity(0);
        slotInfoContainerAnimator.FadeTo(1.0f, 0.5f);
    }

    /**
     * Fade out chapter background with eventually destroying the object at zero opacity
     * **/
    protected override void DismissSlotBackground(bool bDestroyOnFinish)
    {
        CircleMeshAnimator slotBackgroundAnimator = m_background.GetComponent<CircleMeshAnimator>();
        slotBackgroundAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);

        TexturedQuadAnimator slotContourAnimator = m_contour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Fade out chapter information with eventually destroying the object at zero opacity
     * **/
    protected override void DismissSlotInformation(bool bDestroyOnFinish)
    {
        GameObjectAnimator slotInfoContainerAnimator = m_infoContainer.GetComponent<GameObjectAnimator>();
        slotInfoContainerAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
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
}
