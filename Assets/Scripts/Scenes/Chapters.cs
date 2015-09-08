using UnityEngine;
using System.Collections.Generic;

public class Chapters : GUIScene
{
    public const float CHAPTER_SLOT_Z_VALUE = -10.0f;
    public const float SELECTION_ARROWS_Z_VALUE = -10.0f;
    public const float CHAPTER_SLOT_BACKGROUND_OPACITY = 0.5f;

    public int m_displayedChapterIndex { get; set; } //the index of the currently displayed chapter. Chapter 1 is index 0, chapter 2 is index 1 and so on...

    //Chapter slot
    public GameObject m_chapterSlotPfb;
    private ChapterSlot m_chapterSlot;
    private Vector3 m_chapterSlotPosition;

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
        leftArrowButton.SetState((m_displayedChapterIndex > 0) ? true : false);
        rightArrowButton.SetState((m_displayedChapterIndex < LevelManager.CHAPTERS_COUNT - 1) ? true : false);
    }

    protected override void DismissSelf()
    {
        m_chapterSlot.Dismiss();
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
        GameObject chapterSlotObject = (GameObject)Instantiate(m_chapterSlotPfb);
        chapterSlotObject.transform.parent = this.transform;

        float chapterSlotPositionY = GetBackgroundRenderer().GetNearestTriangleToScreenYPosition(0, BackgroundTrianglesRenderer.NUM_COLUMNS / 2, 180).GetCenter().y;
        m_chapterSlotPosition = new Vector3(0, chapterSlotPositionY, CHAPTER_SLOT_Z_VALUE);
        GameObjectAnimator chapterSlotAnimator = chapterSlotObject.GetComponent<GameObjectAnimator>();
        chapterSlotAnimator.SetPosition(m_chapterSlotPosition - new Vector3(0, 100.0f, 0));

        m_chapterSlot = chapterSlotObject.GetComponent<ChapterSlot>();
        m_chapterSlot.Init(this, m_displayedChapterIndex + 1);
        m_chapterSlot.UpdateChapterSlotInformation();

        //Show chapter slot with animation
        m_chapterSlot.Show();
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