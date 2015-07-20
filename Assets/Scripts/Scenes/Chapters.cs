using UnityEngine;
using System.Collections.Generic;

public class Chapters : GUIScene
{
    public const float CHAPTER_SLOT_Z_VALUE = -10.0f;
    public const float SELECTION_ARROWS_Z_VALUE = -10.0f;
    public const float CHAPTER_SLOT_BACKGROUND_OPACITY = 0.5f;

    //shared prefabs
    public GameObject m_textMeshPfb;
    public GameObject m_hexagonMeshPfb;
    public Material m_positionColorMaterial;

    public int m_displayedChapterIndex { get; set; } //the index of the currently displayed chapter. Chapter 1 is index 0, chapter 2 is index 1 and so on...

    //Central item
    private GameObject m_chapterSlotObject;
    private GameObject m_chapterSlotBackground;
    private GameObject m_chapterSlotInfoContainer;
    //private ChapterSlot m_chapterSlot;
    private Vector3 m_chapterSlotPosition;
    public GameObject m_progressBarBgGameObject;
    public GameObject m_progressBarFillGameObject;
    public Material m_progressBarMaterial;

    //Show central item with delay
    private bool m_showingCentralItem;
    private float m_showingCentralItemDelay;
    private float m_showingCentralItemElapsedTime;

    /**
     * Shows Chapters screen with or without animation
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);
        GameObjectAnimator chaptersAnimator = this.GetComponent<GameObjectAnimator>();
        chaptersAnimator.SetOpacity(1);

        //Display back button
        GetGUIManager().ShowBackButton(fDelay);

        //Show chapter slot
        ShowChapterSlot(fDelay);

        //Show chapter selection arrows
        ShowSelectionArrows(true, fDelay);
    }

    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    public override void OnSceneDismissed()
    {
        base.OnSceneDismissed();
    }

    /**
     * Callback used when the player has clicked on a chapter and wants to display levels
     * **/
    public void OnClickChapterSlot()
    {
        DismissChapterSlot(true, false, 0.5f, 0.0f);

        int iChapterNumber = m_displayedChapterIndex + 1;

        GetLevelManager().SetCurrentChapterByNumber(iChapterNumber);
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, true, 0.0f, 0.7f);
    }

    /**
     * Show chapter selection arrows
     * **/
    public void ShowSelectionArrows(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();

        //Left selection arrow
        GameObject leftArrowObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS,
                                                                          new Vector2(128.0f, 128.0f),
                                                                          Color.black,
                                                                          Color.black);

        leftArrowObject.name = "LeftSelectionArrow";

        leftArrowObject.transform.parent = this.gameObject.transform;
         
        float backgroundTrianglesColumnWidth = screenSize.x / BackgroundTrianglesRenderer.NUM_COLUMNS;
        float leftArrowPositionX = 3.5f * backgroundTrianglesColumnWidth; //put the arrow on the fourth column
        leftArrowPositionX -= 0.5f * screenSize.x;
        leftArrowObject.transform.localPosition = new Vector3(leftArrowPositionX, m_chapterSlotPosition.y, SELECTION_ARROWS_Z_VALUE);

        //Right selection arrow
        GameObject rightArrowObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT,
                                                                           new Vector2(128.0f, 128.0f),
                                                                           Color.black,
                                                                           Color.black);

        rightArrowObject.name = "RightSelectionArrow";

        rightArrowObject.transform.parent = this.gameObject.transform;

        float rightArrowPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS - 3.5f) * backgroundTrianglesColumnWidth; //put the arrow on the fourth column
        rightArrowPositionX -= 0.5f * screenSize.x;
        rightArrowObject.transform.localPosition = new Vector3(rightArrowPositionX, m_chapterSlotPosition.y, SELECTION_ARROWS_Z_VALUE);

        //flip the right arrow horizontally
        rightArrowObject.transform.localRotation = Quaternion.AngleAxis(180, Vector3.forward);

        //Fade in arrows
        GameObjectAnimator leftArrowAnimator = leftArrowObject.GetComponent<GameObjectAnimator>();
        GameObjectAnimator rightArrowAnimator = rightArrowObject.GetComponent<GameObjectAnimator>();

        leftArrowAnimator.SetOpacity(0);
        rightArrowAnimator.SetOpacity(0);
        leftArrowAnimator.FadeTo(1.0f, 0.5f, fDelay);
        rightArrowAnimator.FadeTo(1.0f, 0.5f, fDelay);
    }

    /**
     * Show the clickable slot containing all chapter information
     * **/
    public void ShowChapterSlot(float fDelay = 0.0f)
    {
        if (fDelay > 0)
        {
            m_showingCentralItem = true;
            m_showingCentralItemDelay = fDelay;
            m_showingCentralItemElapsedTime = 0;

            return;
        }

        if (m_chapterSlotObject == null)
        {
            m_chapterSlotObject = new GameObject("ChapterSlot");
            m_chapterSlotObject.AddComponent<GameObjectAnimator>();
            //m_chapterSlot.BuildTriangles();
        }

        m_chapterSlotObject.transform.parent = this.transform;

        float chapterSlotPositionY = GetBackgroundRenderer().GetNearestTriangleToScreenYPosition(0, BackgroundTrianglesRenderer.NUM_COLUMNS / 2, 180).GetCenter().y;
        m_chapterSlotPosition = new Vector3(0, chapterSlotPositionY, CHAPTER_SLOT_Z_VALUE);

        BuildCentralItemBackground();
        BuildCentralItemInformation();

        //m_chapterSlotBackground.GetComponent<HexagonMeshAnimator>().SetOpacity(0.25f);

        //Show chapter slot
        float animationDuration = 0.5f;
        ShowChapterSlotBackground(true, animationDuration + 0.5f);
        ShowChapterSlotInformation(true, animationDuration + 0.5f);

        GameObjectAnimator chapterSlotAnimator = m_chapterSlotObject.GetComponent<GameObjectAnimator>();
        chapterSlotAnimator.SetPosition(m_chapterSlotPosition - new Vector3(0, 100.0f, 0));
        chapterSlotAnimator.TranslateTo(m_chapterSlotPosition, animationDuration, fDelay, ValueAnimator.InterpolationType.LINEAR);

        //m_chapterSlot.Show(slotBackgroundBlendColor, GUISlot.BLEND_COLOR_DEFAULT_PROPORTION, true, 0.5f);
    }

    /**
     * Show the main item containing chapter information
     * **/
    public void BuildCentralItemBackground(float fDuration = 1.0f, float fDelay = 0.0f)
    {
        Color slotBackgroundBlendColor = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1).GetThemeColors()[2];

        m_chapterSlotBackground = (GameObject)Instantiate(m_hexagonMeshPfb);
        m_chapterSlotBackground.name = "SlotBackground";
        m_chapterSlotBackground.transform.parent = m_chapterSlotObject.transform;

        HexagonMesh hexaMesh = m_chapterSlotBackground.GetComponent<HexagonMesh>();
        hexaMesh.Init(Instantiate(m_positionColorMaterial));

        HexagonMeshAnimator hexaMeshAnimator = m_chapterSlotBackground.GetComponent<HexagonMeshAnimator>();
        hexaMeshAnimator.SetInnerRadius(0, false);
        hexaMeshAnimator.SetOuterRadius(4 * GetBackgroundRenderer().m_triangleEdgeLength);
        hexaMeshAnimator.SetColor(slotBackgroundBlendColor);
        hexaMeshAnimator.SetPosition(Vector3.zero);
    }

    /**
     * Update the info inside the central item when a new chapter is displayed
     * **/
    public void BuildCentralItemInformation()
    {        
        Chapter displayedChapter = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);

        m_chapterSlotInfoContainer = new GameObject("SlotInformation");
        m_chapterSlotInfoContainer.transform.parent = m_chapterSlotObject.transform;
        m_chapterSlotInfoContainer.transform.localPosition = new Vector3(0, 0, -1);
        GameObjectAnimator animator = m_chapterSlotInfoContainer.AddComponent<GameObjectAnimator>();

        //m_chapterSlot.m_infoContainer = centralItemContainer;

        BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

        //chapter title
        GameObject clonedChapterTitle = (GameObject)Instantiate(m_textMeshPfb);
        clonedChapterTitle.name = "ChapterText";
        clonedChapterTitle.transform.parent = m_chapterSlotInfoContainer.transform;

        TextMesh chapterTitleTextMesh = clonedChapterTitle.GetComponent<TextMesh>();
        chapterTitleTextMesh.text = LanguageUtils.GetTranslationForTag("chapter");

        TextMeshAnimator titleAnimator = clonedChapterTitle.GetComponent<TextMeshAnimator>();
        titleAnimator.SetFontHeight(0.8f * bgRenderer.m_triangleEdgeLength);
        titleAnimator.SetColor(Color.white);
        titleAnimator.SetPosition(new Vector3(0, 1.5f * bgRenderer.m_triangleEdgeLength, 0));

        //chapter number
        GameObject clonedChapterNumber = (GameObject)Instantiate(m_textMeshPfb);
        clonedChapterNumber.name = "NumberText";
        clonedChapterNumber.transform.parent = m_chapterSlotInfoContainer.transform;

        TextMesh chapterNumberTextMesh = clonedChapterNumber.GetComponent<TextMesh>();
        chapterNumberTextMesh.text = LanguageUtils.GetLatinNumberForNumber(m_displayedChapterIndex + 1);

        TextMeshAnimator numberAnimator = clonedChapterNumber.GetComponent<TextMeshAnimator>();
        numberAnimator.SetFontHeight(bgRenderer.m_triangleEdgeLength);
        numberAnimator.SetColor(Color.white);
        numberAnimator.SetPosition(new Vector3(0, 0.2f * bgRenderer.m_triangleEdgeLength, 0));

        //Display Lock or progress bar
        if (displayedChapter.IsLocked())
        {
            ShowLock();
        }
        else
            ShowProgressBar();        
    }

    /**
     * Fade out the information inside the central item
     * **/
    public void DismissChapterSlot(bool bDismissBackground, bool bAnimated = true, float fDuration = 1.0f, float fDelay = 0.0f)
    {
        if (bDismissBackground)
        {
            GameObjectAnimator slotAnimator = m_chapterSlotInfoContainer.GetComponent<GameObjectAnimator>();
            if (bAnimated)
                slotAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, true);
            else
            {
                Destroy(m_chapterSlotObject);
                m_chapterSlotObject = null;
            }
        }
        else
        {
            GameObjectAnimator slotInfoAnimator = m_chapterSlotInfoContainer.GetComponent<GameObjectAnimator>();
            if (bAnimated)
                slotInfoAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, true);
            else
            {
                Destroy(m_chapterSlotInfoContainer);
                m_chapterSlotInfoContainer = null;
            }
        }

        //m_chapterSlotObject.Dismiss(bDismissBackground, bAnimated, fDuration, fDelay);
    }

    /**
     * Show the chapter slot hexagon background 
     * **/
    public void ShowChapterSlotBackground(bool bAnimated, float fDuration, float fDelay = 0.0f)
    {
        HexagonMeshAnimator slotBackgroundAnimator = m_chapterSlotBackground.GetComponent<HexagonMeshAnimator>();
        if (bAnimated)
        {
            slotBackgroundAnimator.SetOpacity(0);
            slotBackgroundAnimator.FadeTo(CHAPTER_SLOT_BACKGROUND_OPACITY, fDuration, fDelay);
        }
        else
            slotBackgroundAnimator.SetOpacity(CHAPTER_SLOT_BACKGROUND_OPACITY);
    }

    /**
     * Show the chapter slot hexagon background 
     * **/
    public void ShowChapterSlotInformation(bool bAnimated, float fDuration, float fDelay = 0.0f)
    {
        GameObjectAnimator slotInfoContainerAnimator = m_chapterSlotInfoContainer.GetComponent<GameObjectAnimator>();
        if (bAnimated)
        {
            slotInfoContainerAnimator.SetOpacity(0);
            slotInfoContainerAnimator.FadeTo(1.0f, fDuration, fDelay);
        }
        else
            slotInfoContainerAnimator.SetOpacity(1);
    }

    ///**
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
     * Show progress bar
     * **/
    public void ShowProgressBar()
    {
        BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

        GameObject progressBar = new GameObject("ProgressBar");
        progressBar.transform.parent = m_chapterSlotInfoContainer.transform;

        GameObjectAnimator progressBarAnimator = progressBar.AddComponent<GameObjectAnimator>();
        progressBarAnimator.SetPosition(new Vector3(0, -1.5f * bgRenderer.m_triangleEdgeLength, 0));

        float progressBarWidth = 5.5f * bgRenderer.m_triangleHeight;
        progressBarWidth = 352;
        float progressBarHeight = 20.0f;
        float ratio = 0.5f;

        Material progressBarBgMaterial = Instantiate(m_progressBarMaterial);
        Material progressBarFillMaterial = Instantiate(m_progressBarMaterial);

        //Background
        GameObject clonedProgressBarBg = Instantiate(m_progressBarBgGameObject);
        clonedProgressBarBg.transform.parent = progressBar.transform;

        MeshRenderer meshRenderer = clonedProgressBarBg.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = progressBarBgMaterial;

        ColorQuad colorQuad = clonedProgressBarBg.GetComponent<ColorQuad>();
        colorQuad.InitQuadMesh();

        ColorQuadAnimator progressBarBgAnimator = clonedProgressBarBg.GetComponent<ColorQuadAnimator>();
        progressBarBgAnimator.SetPosition(Vector3.zero);
        progressBarBgAnimator.SetScale(new Vector3(progressBarWidth, progressBarHeight, 1));
        Color bgColor = GetCurrentlyDisplayedChapter().GetThemeColors()[3];
        progressBarBgAnimator.SetColor(bgColor);

        //Fill
        GameObject clonedProgressBarFill = Instantiate(m_progressBarBgGameObject);
        clonedProgressBarFill.transform.parent = progressBar.transform;

        meshRenderer = clonedProgressBarFill.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = progressBarFillMaterial;

        colorQuad = clonedProgressBarFill.GetComponent<ColorQuad>();
        colorQuad.InitQuadMesh();

        ColorQuadAnimator progressBarFillAnimator = clonedProgressBarFill.GetComponent<ColorQuadAnimator>();
        progressBarFillAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
        progressBarFillAnimator.SetPosition(new Vector3(-0.5f * progressBarWidth, 0, -1));
        progressBarFillAnimator.SetScale(new Vector3(ratio * progressBarWidth, progressBarHeight, 1));
        progressBarFillAnimator.SetColor(Color.white);

        //completion info
        GameObject clonedCompletionTextObject = Instantiate(m_textMeshPfb);
        clonedCompletionTextObject.transform.parent = progressBar.transform;

        TextMesh completionTextMesh = clonedCompletionTextObject.GetComponent<TextMesh>();
        completionTextMesh.text = (ratio * 100) + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");

        TextMeshAnimator completionTextAnimator = clonedCompletionTextObject.GetComponent<TextMeshAnimator>();
        float fontHeight = 0.33f * bgRenderer.m_triangleEdgeLength;
        completionTextAnimator.SetFontHeight(fontHeight);
        completionTextAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.5f));
        completionTextAnimator.SetPosition(new Vector3(0.5f * progressBarWidth, -0.5f * progressBarHeight - fontHeight - 15.0f, 0));        
        completionTextAnimator.SetColor(Color.white);
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
    public void UpdateBackgroundGradient(float fDuration = 1.0f, float fDelay = 0.0f)
    {
        Chapter displayedChapter = GetLevelManager().GetChapterForNumber(m_displayedChapterIndex + 1);

        BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        Gradient gradient = new Gradient();
        gradient.CreateRadial(bgRenderer.m_chaptersGradient.m_radialGradientCenter,
                              960,
                              displayedChapter.GetThemeColors()[0],
                              displayedChapter.GetThemeColors()[1]);

       bgRenderer.m_chaptersGradient = gradient;
       bgRenderer.ApplyGradient(gradient, 0.02f, true, fDuration, fDelay, 0.0f);
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

    public override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;

        if (m_showingCentralItem)
        {
            m_showingCentralItemElapsedTime += dt;
            if (m_showingCentralItemElapsedTime > m_showingCentralItemDelay)
            {
                m_showingCentralItem = false;
                ShowChapterSlot();
            }
        }
    }
}