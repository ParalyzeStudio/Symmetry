using UnityEngine;
using System.Collections.Generic;

public class Chapters : GUIScene
{
    public const float CHAPTER_SLOT_Z_VALUE = -10.0f;
    public const float SELECTION_ARROWS_Z_VALUE = -10.0f;
    public const float CHAPTER_SLOT_BACKGROUND_OPACITY = 0.5f;

    //shared prefabs
    public GameObject m_textMeshPfb;
    public GameObject m_circleMeshPfb;
    public Material m_positionColorMaterial;
    public GameObject m_texQuadPfb;
    public GameObject m_dbgTexSegmentPfb;
    public Material m_dbgTexSegmentMaterial;

    public int m_displayedChapterIndex { get; set; } //the index of the currently displayed chapter. Chapter 1 is index 0, chapter 2 is index 1 and so on...

    //Chapter slot
    private GameObject m_chapterSlotObject;
    private GameObject m_chapterSlotBackground;
    private GameObject m_chapterSlotContour;
    //Info data
    private GameObject m_chapterSlotInfoContainer;
    private GameObject m_chapterSlotTitleObject;
    private GameObject m_chapterSlotNumberObject;
    private GameObject m_progressBar;
    private GameObject m_progressBarBackgroundObject;
    private GameObject m_progressBarFillObject;
    private GameObject m_progressBarCompletionTextObject;
    private Vector2 m_progressBarSize;

    private Vector3 m_chapterSlotPosition;
    public GameObject m_progressBarBgGameObject;
    public GameObject m_progressBarFillGameObject;
    public Material m_progressBarMaterial;
    public Material m_glowContourMaterial;

    //Selection arrows
    public GameObject m_chapterSelectionArrowPfb;
    private GameObject m_leftArrowObject;
    private GameObject m_rightArrowObject;

    /**
     * Show Chapters screen with or without animation
     * **/
    public override void Show()
    {
        base.Show();

        //Set the correct background gradient
        UpdateBackgroundGradient(0.12f);

        //Show chapter slot
        ShowChapterSlot();

        //Show chapter selection arrows
        ShowSelectionArrows();

        //Debug draw a textured segment
        GameObject dbgTexSegmentObject = (GameObject)Instantiate(m_dbgTexSegmentPfb);
        dbgTexSegmentObject.transform.parent = this.transform;
        dbgTexSegmentObject.name = "DebugTexSegment";

        TexturedSegment dbgTexSegment = dbgTexSegmentObject.GetComponent<TexturedSegment>();
        Vector3 pointA = new Vector3(0, 350, -10);
        Vector3 pointB = new Vector3(400, 350, -10);
        float thickness = 16.0f;
        dbgTexSegment.Build(pointA, pointB, thickness, m_dbgTexSegmentMaterial, Color.white, false, TextureWrapMode.Clamp);

        TexturedSegmentAnimator dbgTexSegmentAnimator = dbgTexSegmentObject.GetComponent<TexturedSegmentAnimator>();
    }

    private void ApplyGradientOnBackground(Gradient gradient, float fDelay = 0.0f)
    {
        GetBackgroundRenderer().ApplyGradient(gradient,
                                              0.02f,
                                              true,
                                              BackgroundTrianglesRenderer.GradientAnimationPattern.EXPANDING_CIRCLE,
                                              0.6f,
                                              fDelay,
                                              0.08f,
                                              false);
    }

    /**
     * Show chapter selection arrows
     * **/
    public void ShowSelectionArrows(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();

        //Left selection arrow
        m_leftArrowObject = (GameObject)Instantiate(m_chapterSelectionArrowPfb);
        m_leftArrowObject.name = "LeftSelectionArrow";
        m_leftArrowObject.transform.parent = this.gameObject.transform;

        ChapterSelectionArrowButton leftArrowButton = m_leftArrowObject.GetComponent<ChapterSelectionArrowButton>();
        leftArrowButton.m_ID = GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS;
        leftArrowButton.m_touchArea = new Vector2(256, 256);

        float backgroundTrianglesColumnWidth = screenSize.x / BackgroundTrianglesRenderer.NUM_COLUMNS;
        float leftArrowPositionX = 3.5f * backgroundTrianglesColumnWidth; //put the arrow on the fourth column
        leftArrowPositionX -= 0.5f * screenSize.x;
        m_leftArrowObject.GetComponent<GameObjectAnimator>().SetPosition(new Vector3(leftArrowPositionX, m_chapterSlotPosition.y, SELECTION_ARROWS_Z_VALUE));

        ChapterSelectionArrowButton arrowButton = m_leftArrowObject.GetComponent<ChapterSelectionArrowButton>();
        arrowButton.Init(null);

        //Right selection arrow
        m_rightArrowObject = (GameObject)Instantiate(m_chapterSelectionArrowPfb);
        m_rightArrowObject.name = "RightSelectionArrow";
        m_rightArrowObject.transform.parent = this.gameObject.transform;

        ChapterSelectionArrowButton rightArrowButton = m_rightArrowObject.GetComponent<ChapterSelectionArrowButton>();
        rightArrowButton.m_ID = GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT;
        rightArrowButton.m_touchArea = new Vector2(256, 256);

        float rightArrowPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS - 3.5f) * backgroundTrianglesColumnWidth;
        rightArrowPositionX -= 0.5f * screenSize.x;
        m_rightArrowObject.GetComponent<GameObjectAnimator>().SetPosition(new Vector3(rightArrowPositionX, m_chapterSlotPosition.y, SELECTION_ARROWS_Z_VALUE));

        arrowButton = m_rightArrowObject.GetComponent<ChapterSelectionArrowButton>();
        arrowButton.Init(null);

        m_rightArrowObject.transform.localRotation = Quaternion.AngleAxis(180, Vector3.forward);

        //Fade in arrows
        GameObjectAnimator leftArrowAnimator = m_leftArrowObject.GetComponent<GameObjectAnimator>();
        leftArrowAnimator.SetOpacity(0);
        leftArrowAnimator.FadeTo(1.0f, 0.5f, fDelay);
        GameObjectAnimator rightArrowAnimator = m_rightArrowObject.GetComponent<GameObjectAnimator>();
        rightArrowAnimator.SetOpacity(0);
        rightArrowAnimator.FadeTo(1.0f, 0.5f, fDelay);
    }

    protected override void DismissSelf()
    {
        DismissChapterSlot();
        DismissSelectionArrows();    
    }

    /**
     * Fade out selection arrows
     * **/
    private void DismissSelectionArrows(bool bDestroyOnFinish = true)
    {
        GameObjectAnimator leftArrowAnimator = m_leftArrowObject.GetComponent<GameObjectAnimator>();
        leftArrowAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
        GameObjectAnimator rightArrowAnimator = m_rightArrowObject.GetComponent<GameObjectAnimator>();
        rightArrowAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Build and show the clickable slot containing all chapter information
     * **/
    public void ShowChapterSlot()
    {
        m_chapterSlotObject = new GameObject("ChapterSlot");
        GameObjectAnimator chapterSlotAnimator = m_chapterSlotObject.AddComponent<GameObjectAnimator>();

        //build elements
        BuildChapterSlotBackground();
        BuildChapterSlotInformation();

        m_chapterSlotObject.transform.parent = this.transform;

        //set correct position
        float chapterSlotPositionY = GetBackgroundRenderer().GetNearestTriangleToScreenYPosition(0, BackgroundTrianglesRenderer.NUM_COLUMNS / 2, 180).GetCenter().y;
        m_chapterSlotPosition = new Vector3(0, chapterSlotPositionY, CHAPTER_SLOT_Z_VALUE);

        //update data
        UpdateChapterSlotInformation();

        //Show chapter slot with animation
        ShowChapterSlotBackground();
        ShowChapterSlotInformation();

        chapterSlotAnimator.SetPosition(m_chapterSlotPosition - new Vector3(0, 100.0f, 0));
        chapterSlotAnimator.TranslateTo(m_chapterSlotPosition, 0.5f);
    }

    /**
     * Show the main item containing chapter information
     * **/
    public void BuildChapterSlotBackground(float fDuration = 1.0f, float fDelay = 0.0f)
    {
        Color slotBackgroundBlendColor = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1).GetThemeColors()[2];

        m_chapterSlotBackground = (GameObject)Instantiate(m_circleMeshPfb);
        m_chapterSlotBackground.name = "SlotBackground";
        m_chapterSlotBackground.transform.parent = m_chapterSlotObject.transform;

        CircleMesh hexaMesh = m_chapterSlotBackground.GetComponent<CircleMesh>();
        hexaMesh.Init(Instantiate(m_positionColorMaterial));

        CircleMeshAnimator hexaMeshAnimator = m_chapterSlotBackground.GetComponent<CircleMeshAnimator>();
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(0, false);
        hexaMeshAnimator.SetOuterRadius(4 * GetBackgroundRenderer().m_triangleEdgeLength, true);
        hexaMeshAnimator.SetColor(slotBackgroundBlendColor);
        hexaMeshAnimator.SetPosition(Vector3.zero);

        //Add a contour to the hexagon
        m_chapterSlotContour = (GameObject)Instantiate(m_texQuadPfb);
        m_chapterSlotContour.name = "SlotContour";
        m_chapterSlotContour.transform.parent = m_chapterSlotObject.transform;

        m_chapterSlotContour.GetComponent<UVQuad>().Init(m_glowContourMaterial);

        TexturedQuadAnimator contourAnimator = m_chapterSlotContour.GetComponent<TexturedQuadAnimator>();
        float contourTextureScale = 8 * GetBackgroundRenderer().m_triangleEdgeLength / 490.0f; //hexagon is 8 * triangleEdgeLength size, and the contour texture is 496x496 with 8 pixels blur (512x512)
        contourAnimator.SetScale(new Vector3(contourTextureScale * 512, contourTextureScale * 512, 1));
        contourAnimator.SetPosition(new Vector3(0, 0, -1)); //set the contour above hexagon

        GameObject contourHexaObject = Instantiate(m_circleMeshPfb);
        contourHexaObject.name = "ContourHexagon";
        contourHexaObject.transform.parent = m_chapterSlotContour.transform;

        CircleMesh contourMesh = contourHexaObject.GetComponent<CircleMesh>();
        contourMesh.Init(Instantiate(m_positionColorMaterial));

        float contourMeshThickness = 4.0f;
        CircleMeshAnimator contourMeshAnimator = contourHexaObject.GetComponent<CircleMeshAnimator>();
        contourMeshAnimator.SetNumSegments(6, false);
        contourMeshAnimator.SetInnerRadius(4 * GetBackgroundRenderer().m_triangleEdgeLength, false);
        contourMeshAnimator.SetOuterRadius(4 * GetBackgroundRenderer().m_triangleEdgeLength + contourMeshThickness, true);
        contourMeshAnimator.SetColor(Color.white);
        contourMeshAnimator.SetPosition(Vector3.zero);

        //TODO Add another contour a little bigger than the other one and with scale/opacity animation      
    }

    /**
     * Update the info inside the central item when a new chapter is displayed
     * **/
    public void BuildChapterSlotInformation()
    {        
        Chapter displayedChapter = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);

        m_chapterSlotInfoContainer = new GameObject("SlotInformation");
        m_chapterSlotInfoContainer.transform.parent = m_chapterSlotObject.transform;
        m_chapterSlotInfoContainer.transform.localPosition = new Vector3(0, 0, -1);
        GameObjectAnimator animator = m_chapterSlotInfoContainer.AddComponent<GameObjectAnimator>();

        BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

        //chapter title
        m_chapterSlotTitleObject = (GameObject)Instantiate(m_textMeshPfb);
        m_chapterSlotTitleObject.name = "ChapterText";
        m_chapterSlotTitleObject.transform.parent = m_chapterSlotInfoContainer.transform;

        TextMesh chapterTitleTextMesh = m_chapterSlotTitleObject.GetComponent<TextMesh>();
        chapterTitleTextMesh.text = LanguageUtils.GetTranslationForTag("chapter");

        TextMeshAnimator titleAnimator = m_chapterSlotTitleObject.GetComponent<TextMeshAnimator>();
        titleAnimator.SetFontHeight(0.8f * bgRenderer.m_triangleEdgeLength);
        titleAnimator.SetColor(Color.white);
        titleAnimator.SetPosition(new Vector3(0, 1.5f * bgRenderer.m_triangleEdgeLength, 0));

        //chapter number
        m_chapterSlotNumberObject = (GameObject)Instantiate(m_textMeshPfb);
        m_chapterSlotNumberObject.name = "NumberText";
        m_chapterSlotNumberObject.transform.parent = m_chapterSlotInfoContainer.transform;

        TextMesh chapterNumberTextMesh = m_chapterSlotNumberObject.GetComponent<TextMesh>();
        chapterNumberTextMesh.text = LanguageUtils.GetLatinNumberForNumber(m_displayedChapterIndex + 1);

        TextMeshAnimator numberAnimator = m_chapterSlotNumberObject.GetComponent<TextMeshAnimator>();
        numberAnimator.SetFontHeight(bgRenderer.m_triangleEdgeLength);
        numberAnimator.SetColor(Color.white);
        numberAnimator.SetPosition(new Vector3(0, 0.2f * bgRenderer.m_triangleEdgeLength, 0));

        //Build Lock and progress bar
        BuildProgressBar();      
    }

    /**
     * Build the progress bar elements
     * **/
    private void BuildProgressBar()
    {
        BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

        m_progressBar = new GameObject("ProgressBar");
        m_progressBar.transform.parent = m_chapterSlotInfoContainer.transform;

        GameObjectAnimator progressBarAnimator = m_progressBar.AddComponent<GameObjectAnimator>();
        progressBarAnimator.SetPosition(new Vector3(0, -1.5f * bgRenderer.m_triangleEdgeLength, 0));

        float progressBarWidth = 5.5f * bgRenderer.m_triangleHeight;
        float progressBarHeight = 20.0f;
        m_progressBarSize = new Vector2(progressBarWidth, progressBarHeight);

        Material progressBarBgMaterial = Instantiate(m_progressBarMaterial);
        Material progressBarFillMaterial = Instantiate(m_progressBarMaterial);

        //Background
        m_progressBarBackgroundObject = Instantiate(m_progressBarBgGameObject);
        m_progressBarBackgroundObject.transform.parent = m_progressBar.transform;

        ColorQuad colorQuad = m_progressBarBackgroundObject.GetComponent<ColorQuad>();
        colorQuad.Init(progressBarBgMaterial);

        ColorQuadAnimator progressBarBgAnimator = m_progressBarBackgroundObject.GetComponent<ColorQuadAnimator>();
        progressBarBgAnimator.SetPosition(Vector3.zero);
        progressBarBgAnimator.SetScale(m_progressBarSize);

        //Fill
        m_progressBarFillObject = Instantiate(m_progressBarBgGameObject);
        m_progressBarFillObject.transform.parent = m_progressBar.transform;

        colorQuad = m_progressBarFillObject.GetComponent<ColorQuad>();
        colorQuad.Init(progressBarFillMaterial);

        ColorQuadAnimator progressBarFillAnimator = m_progressBarFillObject.GetComponent<ColorQuadAnimator>();
        progressBarFillAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
        progressBarFillAnimator.SetPosition(new Vector3(-0.5f * progressBarWidth, 0, -1));
        progressBarFillAnimator.SetColor(Color.white);

        //completion info
        m_progressBarCompletionTextObject = Instantiate(m_textMeshPfb);
        m_progressBarCompletionTextObject.transform.parent = m_progressBar.transform;

        TextMesh completionTextMesh = m_progressBarCompletionTextObject.GetComponent<TextMesh>();
        completionTextMesh.text = (0.5f * 100) + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");

        TextMeshAnimator completionTextAnimator = m_progressBarCompletionTextObject.GetComponent<TextMeshAnimator>();
        float fontHeight = 0.33f * bgRenderer.m_triangleEdgeLength;
        completionTextAnimator.SetFontHeight(fontHeight);
        completionTextAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.5f));
        completionTextAnimator.SetPosition(new Vector3(0.5f * progressBarWidth, -0.5f * progressBarHeight - fontHeight - 8.0f, 0));
        completionTextAnimator.SetColor(Color.white);
    }

    /**
     * Change the color of slot background with animation
     * **/
    public void UpdateChapterSlotBackgroundColor()
    {
        Chapter displayedChapter = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);

        CircleMeshAnimator chapterSlotBackgroundAnimator = m_chapterSlotBackground.GetComponent<CircleMeshAnimator>();
        Color toColor = displayedChapter.GetThemeColors()[2];
        toColor.a = CHAPTER_SLOT_BACKGROUND_OPACITY;
        chapterSlotBackgroundAnimator.ColorChangeTo(toColor, 1.0f);
    }

    /**
     * Replace the texts of the chapter slot with new data
     * **/
    public void UpdateChapterSlotInformation()
    {
        TextMesh chapterTitleTextMesh = m_chapterSlotTitleObject.GetComponent<TextMesh>();
        chapterTitleTextMesh.text = LanguageUtils.GetTranslationForTag("chapter");

        TextMesh chapterNumberTextMesh = m_chapterSlotNumberObject.GetComponent<TextMesh>();
        chapterNumberTextMesh.text = LanguageUtils.GetLatinNumberForNumber(m_displayedChapterIndex + 1);

        //Display Lock or progress bar
        Chapter displayedChapter = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);
        if (displayedChapter.IsLocked())
            ShowLock();
        else
            UpdateProgressBarData();
    }

    /**
     * Fade out the information inside the chapter slot with eventually the background
     * **/
    public void DismissChapterSlot(bool bDismissBackground = true, bool bDestroyBackgroundOnFinish = true, bool bDestroyChapterInfoOnFinish = true)
    {
        if (bDismissBackground)
        {
            DismissChapterSlotBackground(bDestroyBackgroundOnFinish);
        }
        
        DismissChapterSlotInformation(bDestroyChapterInfoOnFinish);
    }

    /**
     * Show the chapter slot hexagon background 
     * **/
    public void ShowChapterSlotBackground()
    {
        CircleMeshAnimator slotBackgroundAnimator = m_chapterSlotBackground.GetComponent<CircleMeshAnimator>();

        slotBackgroundAnimator.SetOpacity(0);
        slotBackgroundAnimator.FadeTo(CHAPTER_SLOT_BACKGROUND_OPACITY, 0.5f);

        TexturedQuadAnimator slotContourAnimator = m_chapterSlotContour.GetComponent<TexturedQuadAnimator>();

        slotContourAnimator.SetOpacity(0);
        slotContourAnimator.FadeTo(1.0f, 0.5f);
    }

    /**
     * Show the chapter slot hexagon background 
     * **/
    public void ShowChapterSlotInformation()
    {
        GameObjectAnimator slotInfoContainerAnimator = m_chapterSlotInfoContainer.GetComponent<GameObjectAnimator>();

        slotInfoContainerAnimator.SetOpacity(0);
        slotInfoContainerAnimator.FadeTo(1.0f, 0.5f);
    }

    /**
     * Fade out chapter background with eventually destroying the object at zero opacity
     * **/
    private void DismissChapterSlotBackground(bool bDestroyOnFinish)
    {
        CircleMeshAnimator slotBackgroundAnimator = m_chapterSlotBackground.GetComponent<CircleMeshAnimator>();
        slotBackgroundAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);

        TexturedQuadAnimator slotContourAnimator = m_chapterSlotContour.GetComponent<TexturedQuadAnimator>();
        slotContourAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Fade out chapter information with eventually destroying the object at zero opacity
     * **/
    private void DismissChapterSlotInformation(bool bDestroyOnFinish)
    {
        GameObjectAnimator slotInfoContainerAnimator = m_chapterSlotInfoContainer.GetComponent<GameObjectAnimator>();
        slotInfoContainerAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    //**
    // * Show the chapter number
    // * **/
    //public void BuildTitle()
    //{
    //    BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

    //    //chapter title
    //    GameObject clonedChapterTitle = (GameObject)Instantiate(m_chaptersTextPfb);
    //    clonedChapterTitle.transform.parent = centralItemContainer.transform;
    //    clonedChapterTitle.transform.localPosition = new Vector3(0, 1.5f * bgRenderer.m_triangleEdgeLength, 0);

    //    TextMesh chapterTitleTextMesh = clonedChapterTitle.GetComponent<TextMesh>();
    //    chapterTitleTextMesh.text = LanguageUtils.GetTranslationForTag("chapter");

    //    //chapter number
    //    GameObject clonedChapterNumber = (GameObject)Instantiate(m_chaptersNumberPfb);
    //    clonedChapterNumber.transform.parent = centralItemContainer.transform;
    //    clonedChapterNumber.transform.localPosition = new Vector3(0, 0.2f * bgRenderer.m_triangleEdgeLength, 0);

    //    TextMesh chapterNumberTextMesh = clonedChapterNumber.GetComponent<TextMesh>();
    //    chapterNumberTextMesh.text = (m_displayedChapterIndex + 1).ToString();
    //}

    /**
     * Update progress bar data
     * **/
    public void UpdateProgressBarData()
    {
        float progressBarWidth = m_progressBarSize.x;
        float progressBarHeight = m_progressBarSize.y;
        float ratio = 0.5f;

        //Background
        ColorQuadAnimator progressBarBgAnimator = m_progressBarBackgroundObject.GetComponent<ColorQuadAnimator>();
        Color bgColor = GetCurrentlyDisplayedChapter().GetThemeColors()[3];
        bgColor.a = progressBarBgAnimator.m_color.a;
        progressBarBgAnimator.SetColor(bgColor);

        //Fill
        ColorQuadAnimator progressBarFillAnimator = m_progressBarFillObject.GetComponent<ColorQuadAnimator>();
        progressBarFillAnimator.SetScale(new Vector3(ratio * progressBarWidth, progressBarHeight, 1));

        //completion info
        TextMesh completionTextMesh = m_progressBarCompletionTextObject.GetComponent<TextMesh>();
        completionTextMesh.text = (ratio * 100) + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");
    }    

    /**
     * Show lock icon and with a description on how to unlock the chapter
     * **/
    private void ShowLock()
    {

    }

    /**
     * Update the background gradient for its color to match with the currently displayed chapter
     * **/
    public void UpdateBackgroundGradient(float fDelay = 0.0f)
    {
        Chapter displayedChapter = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);

        Gradient gradient = new Gradient();
        gradient.CreateRadial(Vector2.zero,
                              960,
                              displayedChapter.GetThemeColors()[0],
                              displayedChapter.GetThemeColors()[1]);

        ApplyGradientOnBackground(gradient, fDelay);
    }

    /**
     * Return the chapter associated to the currently displayed chapter index
     * **/
    public Chapter GetCurrentlyDisplayedChapter()
    {
        return GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);
    }

    /**
     * Decrement the value of m_displayedChapterIndex if possible
     * **/
    public bool DecrementChapterIndex()
    {
        if (m_displayedChapterIndex > 0)
        {
            m_displayedChapterIndex--;
            return true;
        }

        return false;
    }

    /**
     * Increment the value of m_displayedChapterIndex if possible
     * **/
    public bool IncrementChapterIndex()
    {
        if (m_displayedChapterIndex < LevelManager.CHAPTERS_COUNT - 1)
        {
            m_displayedChapterIndex++;
            return true;
        }

        return false;
    }       

    /**
     * Callback used when the player has clicked on a chapter and wants to display levels
     * **/
    public void OnClickChapterSlot()
    {
        int iChapterNumber = m_displayedChapterIndex + 1;

        GetLevelManager().SetCurrentChapterByNumber(iChapterNumber);
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, true, 0.5f);
    }
}