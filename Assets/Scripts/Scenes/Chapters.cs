using UnityEngine;
using System.Collections.Generic;

public class Chapters : GUIScene
{
    public const float CENTRAL_ITEM_Z_VALUE = -10.0f;
    public const float SELECTION_ARROWS_Z_VALUE = -10.0f;

    public GameObject m_chaptersTextPfb;
    public GameObject m_chaptersNumberPfb;

    public int m_displayedChapterIndex { get; set; } //the index of the currently displayed chapter. Chapter 1 is index 0, chapter 2 is index 1 and so on...

    //Central item
    private ChapterSlot m_chapterSlot;
    private Vector3 m_centralItemPosition;
    public GameObject m_progressBarBgGameObject;
    public GameObject m_progressBarFillGameObject;
    public Material m_progressBarMaterial;
    public GameObject m_progressBarCompletionPfb;

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
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.ShowBackButton(fDelay);

        //Show chapter slot
        ShowChapterSlot(5.2f);

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

    //public void ShowChapterSlotsWithAnimation()
    //{
    //    m_showChaptersCallRunning = false;

    //    float slotAnimationDuration = 0.3f;

    //    //Animate slot 1
    //    GameObjectAnimator slotAnimator = m_chapterSlots[0].GetComponent<GameObjectAnimator>();
    //    slotAnimator.FadeTo(1, slotAnimationDuration);

    //    //Animate slot 2
    //    slotAnimator = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
    //    slotAnimator.RotateTo(0, slotAnimationDuration, slotAnimationDuration);
    //    slotAnimator.FadeTo(1, slotAnimationDuration, slotAnimationDuration);

    //    //Animate slot 3
    //    slotAnimator = m_chapterSlots[2].GetComponent<GameObjectAnimator>();
    //    slotAnimator.RotateTo(0, slotAnimationDuration, 3 * slotAnimationDuration);
    //    slotAnimator.FadeTo(1, slotAnimationDuration, 3 * slotAnimationDuration);

    //    //Animate slot 4
    //    slotAnimator = m_chapterSlots[3].GetComponent<GameObjectAnimator>();
    //    slotAnimator.RotateTo(0, slotAnimationDuration, 2 * slotAnimationDuration);
    //    slotAnimator.FadeTo(1, slotAnimationDuration, 2 * slotAnimationDuration);
    //}

    /**
     * Callback used when the player has clicked on a chapter and wants to display levels
     * **/
    public void OnClickChapterSlot()
    {
        DismissChapterSlot(true, false, 0.5f, 0.0f);

        int iChapterNumber = m_displayedChapterIndex + 1;

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        levelManager.SetCurrentChapterByNumber(iChapterNumber);

        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, true, 0.0f, 0.7f);
    }

    /**
     * Show chapter selection arrows
     * **/
    public void ShowSelectionArrows(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();

        //Left selection arrow
        GameObject leftArrowObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS,
                                                                     new Vector2(128.0f, 128.0f),
                                                                     Color.black,
                                                                     Color.black);

        leftArrowObject.name = "LeftSelectionArrow";

        leftArrowObject.transform.parent = this.gameObject.transform;
         
        float backgroundTrianglesColumnWidth = screenSize.x / BackgroundTrianglesRenderer.NUM_COLUMNS;
        float leftArrowPositionX = 3.5f * backgroundTrianglesColumnWidth; //put the arrow on the fourth column
        leftArrowPositionX -= 0.5f * screenSize.x;
        leftArrowObject.transform.localPosition = new Vector3(leftArrowPositionX, m_centralItemPosition.y, SELECTION_ARROWS_Z_VALUE);

        //Right selection arrow
        GameObject rightArrowObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT,
                                                                      new Vector2(128.0f, 128.0f),
                                                                      Color.black,
                                                                      Color.black);

        rightArrowObject.name = "RightSelectionArrow";

        rightArrowObject.transform.parent = this.gameObject.transform;

        float rightArrowPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS - 3.5f) * backgroundTrianglesColumnWidth; //put the arrow on the fourth column
        rightArrowPositionX -= 0.5f * screenSize.x;
        rightArrowObject.transform.localPosition = new Vector3(rightArrowPositionX, m_centralItemPosition.y, SELECTION_ARROWS_Z_VALUE);

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

    public void ShowChapterSlot(float fDelay = 0.0f)
    {
        if (fDelay > 0)
        {
            m_showingCentralItem = true;
            m_showingCentralItemDelay = fDelay;
            m_showingCentralItemElapsedTime = 0;

            return;
        }

        if (m_chapterSlot == null)
        {
            m_chapterSlot = new ChapterSlot();
            m_chapterSlot.BuildTriangles();
        }

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Color slotBackgroundBlendColor = levelManager.GetChapterForNumber(m_displayedChapterIndex + 1).GetThemeColors()[2];

        //ShowCentralItemBackground();
        BuildCentralItemInformation();

        m_chapterSlot.Show(slotBackgroundBlendColor, GUISlot.BLEND_COLOR_DEFAULT_PROPORTION, true, 0.5f);
    }

    /**
     * Show the main item containing chapter information
     * **/
    //public void ShowCentralItemBackground(float fDuration = 1.0f, float fDelay = 0.0f)
    //{
    //    //Find triangles whose color needs to be modified in order to draw the item hexagon background
    //    BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();        

    //    List<BackgroundTriangle> centralItemsTriangles = new List<BackgroundTriangle>();
    //    centralItemsTriangles.Capacity = 2 * 15 + 2 * 13 + 2 * 11 + 2 * 9;

    //    //color to blend every triangle inside the item
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

    //    //column 1
    //    int columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2;
    //    BackgroundTriangleColumn column = backgroundRenderer.m_triangleColumns[BackgroundTrianglesRenderer.NUM_COLUMNS / 2];
    //    BackgroundTriangle triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex, 180);
    //    int referenceTriangleIndex = triangle.m_indexInColumn; //the index of the triangle taken as a reference for building the column
    //    for (int i = referenceTriangleIndex - 7; i != referenceTriangleIndex + 8; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }

    //    m_centralItemPosition = new Vector3(0, triangle.GetCenter().y, 0); //set the position of the central item according to its hexagon background centered triangles

    //    //column -1
    //    columnIndex--;
    //    column = backgroundRenderer.m_triangleColumns[columnIndex];
    //    for (int i = referenceTriangleIndex - 7; i != referenceTriangleIndex + 8; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }

    //    //column -2
    //    columnIndex--;
    //    column = backgroundRenderer.m_triangleColumns[columnIndex];
    //    for (int i = referenceTriangleIndex - 6; i != referenceTriangleIndex + 7; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }

    //    //column -3
    //    columnIndex--;
    //    column = backgroundRenderer.m_triangleColumns[columnIndex];
    //    for (int i = referenceTriangleIndex - 5; i != referenceTriangleIndex + 6; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }

    //    //column -4
    //    columnIndex--;
    //    column = backgroundRenderer.m_triangleColumns[columnIndex];
    //    for (int i = referenceTriangleIndex - 4; i != referenceTriangleIndex + 5; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }
        
    //    //column 2
    //    columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 + 1;
    //    column = backgroundRenderer.m_triangleColumns[columnIndex];
    //    for (int i = referenceTriangleIndex - 6; i != referenceTriangleIndex + 7; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }

    //    //column 3
    //    columnIndex++;
    //    column = backgroundRenderer.m_triangleColumns[columnIndex];
    //    for (int i = referenceTriangleIndex - 5; i != referenceTriangleIndex + 6; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }             

    //    //column 4
    //    columnIndex++;
    //    column = backgroundRenderer.m_triangleColumns[columnIndex];
    //    for (int i = referenceTriangleIndex - 4; i != referenceTriangleIndex + 5; i++)
    //    {
    //        centralItemsTriangles.Add(column[i]);
    //    }              

    //    //blend all triangles
    //    Color itemBackgroundBlendColor = levelManager.GetChapterForNumber(m_displayedChapterIndex + 1).GetThemeColors()[2];
    //    for (int i = 0; i != centralItemsTriangles.Count; i++)
    //    {
    //        triangle = centralItemsTriangles[i];
    //        Color toColor = Color.Lerp(triangle.m_frontColor, itemBackgroundBlendColor, 0.25f);
    //        triangle.StartColorAnimation(true, toColor, fDuration, fDelay);
    //    }
    //}

    /**
     * Update the info inside the central item when a new chapter is displayed
     * **/
    public void BuildCentralItemInformation()
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Chapter displayedChapter = levelManager.GetChapterForNumber(m_displayedChapterIndex + 1);

        GameObject centralItemContainer = new GameObject("CentralItem");
        centralItemContainer.transform.parent = this.transform;
        centralItemContainer.transform.localPosition = new Vector3(0, 0, CENTRAL_ITEM_Z_VALUE);
        GameObjectAnimator animator = centralItemContainer.AddComponent<GameObjectAnimator>();

        m_chapterSlot.m_infoContainer = centralItemContainer;

        BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

        //chapter title
        GameObject clonedChapterTitle = (GameObject)Instantiate(m_chaptersTextPfb);
        clonedChapterTitle.transform.parent = centralItemContainer.transform;
        clonedChapterTitle.transform.localPosition = new Vector3(0, 1.5f * bgRenderer.m_triangleEdgeLength, 0);

        TextMesh chapterTitleTextMesh = clonedChapterTitle.GetComponent<TextMesh>();
        chapterTitleTextMesh.text = LanguageUtils.GetTranslationForTag("chapter");

        //chapter number
        GameObject clonedChapterNumber = (GameObject)Instantiate(m_chaptersNumberPfb);
        clonedChapterNumber.transform.parent = centralItemContainer.transform;
        clonedChapterNumber.transform.localPosition = new Vector3(0, 0.2f * bgRenderer.m_triangleEdgeLength, 0);

        TextMesh chapterNumberTextMesh = clonedChapterNumber.GetComponent<TextMesh>();
        chapterNumberTextMesh.text = (m_displayedChapterIndex + 1).ToString();

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
        m_chapterSlot.Dismiss(bDismissBackground, bAnimated, fDuration, fDelay);
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
        progressBar.transform.parent = m_chapterSlot.m_infoContainer.transform;
        progressBar.transform.localPosition = new Vector3(0, -1.5f * bgRenderer.m_triangleEdgeLength, 0);

        float progressBarWidth = 5.5f * bgRenderer.m_triangleHeight;
        float progressBarHeight = 20.0f;
        float ratio = 0.64f;

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
        progressBarFillAnimator.SetPosition(new Vector3(-0.5f * progressBarWidth, 0, -1));
        progressBarFillAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
        progressBarFillAnimator.SetScale(new Vector3(ratio * progressBarWidth, progressBarHeight, 1));
        progressBarFillAnimator.SetColor(Color.white);

        //completion info
        GameObject clonedCompletionTextObject = Instantiate(m_progressBarCompletionPfb);
        clonedCompletionTextObject.transform.parent = progressBar.transform;
        clonedCompletionTextObject.transform.localPosition = new Vector3(0.5f * progressBarWidth, -0.5f * progressBarHeight - 30.0f, 0);

        TextMesh completionTextMesh = clonedCompletionTextObject.GetComponent<TextMesh>();
        completionTextMesh.text = (ratio * 100) + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");

        TextMeshAnimator completionTextAnimator = clonedCompletionTextObject.GetComponent<TextMeshAnimator>();
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
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Chapter displayedChapter = levelManager.GetChapterForNumber(m_displayedChapterIndex + 1);

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
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        return levelManager.GetChapterForNumber(m_displayedChapterIndex + 1);
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