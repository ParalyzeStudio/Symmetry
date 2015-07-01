using UnityEngine;
using System.Collections.Generic;

public class Chapters : GUIScene
{
    public const float SELECTION_ARROWS_Z_VALUE = -10.0f;

    public GameObject m_chaptersTitlePfb;
    public GameObject m_chapterSlotPfb;

    public int m_currentDisplayedChapterIndex { get; set; } //the index of the currently displayed chapter. Chapter 1 is index 0, chapter 2 is index 1 and so on...

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
        int reachedChapterNumber = 1;
        int iReachedChapterGroup = ((reachedChapterNumber - 1) / LevelManager.CHAPTERS_PER_GROUP) + 1;
        
        //apply new gradient to triangles
        BackgroundTrianglesRenderer backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();
        backgroundRenderer.RenderForChapter(true, 0.0f);

        //Display back button
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.ShowBackButton(fDelay);

        //Show chapter selection arrows
        ShowSelectionArrows();

        //Show chapter central item
        ShowCentralItem(true, 4.0f);
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

    public void OnClickChapterSlot(int iChapterSlotIndex)
    {
        //int iChapterNumber = (m_currentChapterGroup - 1) * LevelManager.CHAPTERS_PER_GROUP + iChapterSlotIndex + 1;

        //LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        //levelManager.SetCurrentChapterByNumber(iChapterNumber);

        //SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        //sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, true, 0.0f, 0.7f);
    }



    /**
     * Show chapter selection arrows
     * **/
    public void ShowSelectionArrows()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();

        float arrowsYPosition = backgroundRenderer.GetNearestTriangleToScreenYCenter(false, true).GetCenter().y;

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
        leftArrowObject.transform.localPosition = new Vector3(leftArrowPositionX, arrowsYPosition, SELECTION_ARROWS_Z_VALUE);

        //Right selection arrow
        GameObject rightArrowObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS,
                                                                    new Vector2(128.0f, 128.0f),
                                                                    Color.black,
                                                                    Color.black);

        rightArrowObject.name = "RightSelectionArrow";

        rightArrowObject.transform.parent = this.gameObject.transform;

        float rightArrowPositionX = (BackgroundTrianglesRenderer.NUM_COLUMNS - 3.5f) * backgroundTrianglesColumnWidth; //put the arrow on the fourth column
        rightArrowPositionX -= 0.5f * screenSize.x;
        rightArrowObject.transform.localPosition = new Vector3(rightArrowPositionX, arrowsYPosition, SELECTION_ARROWS_Z_VALUE);

        //flip the right arrow horizontally
        rightArrowObject.transform.localRotation = Quaternion.AngleAxis(180, Vector3.forward);
    }

    /**
     * Show the main item containing chapter information
     * **/
    public void ShowCentralItem(bool bShowAfterDelay = false, float fDelay = 0.0f)
    {
        if (bShowAfterDelay)
        {
            m_showingCentralItem = true;
            m_showingCentralItemDelay = fDelay;
            m_showingCentralItemElapsedTime = 0;

            return;
        }

        //Find triangles whose color needs to be modified in order to draw the item hexagon background
        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();        

        List<BackgroundTriangle> centralItemsTriangles = new List<BackgroundTriangle>();
        centralItemsTriangles.Capacity = 2 * 15 + 2 * 13 + 2 * 11 + 2 * 9;

        //color to blend every triangle inside the item
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Color itemBackgroundBlendColor = levelManager.GetChapterForNumber(m_currentDisplayedChapterIndex + 1).GetThemeColors()[2];

        //column 1
        int columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2;
        BackgroundTriangleColumn column = backgroundRenderer.m_triangleColumns[BackgroundTrianglesRenderer.NUM_COLUMNS / 2];
        BackgroundTriangle triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 != 0, columnIndex % 2 == 0, BackgroundTrianglesRenderer.NUM_COLUMNS / 2);
        int triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 7; i != triangleIndex + 8; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }

        //column -1
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 1;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 == 0, columnIndex % 2 != 0, columnIndex);
        triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 7; i != triangleIndex + 8; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }
        
        //column 2
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 + 1;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 == 0, columnIndex % 2 != 0, columnIndex);
        triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 6; i != triangleIndex + 7; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }

        //column -2
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 2;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 != 0, columnIndex % 2 == 0, columnIndex);
        triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 6; i != triangleIndex + 7; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }

        //column 3
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 + 2;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 != 0, columnIndex % 2 == 0, columnIndex);
        triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 5; i != triangleIndex + 6; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }

        //column -3
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 3;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 == 0, columnIndex % 2 != 0, columnIndex);
        triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 5; i != triangleIndex + 6; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }

        //column 4
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 + 3;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 == 0, columnIndex % 2 != 0, columnIndex);
        triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 4; i != triangleIndex + 5; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }

        //column -4
        columnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2 - 4;
        column = backgroundRenderer.m_triangleColumns[columnIndex];
        triangle = backgroundRenderer.GetNearestTriangleToScreenYCenter(columnIndex % 2 != 0, columnIndex % 2 == 0, columnIndex);
        triangleIndex = triangle.m_indexInColumn;
        for (int i = triangleIndex - 4; i != triangleIndex + 5; i++)
        {
            centralItemsTriangles.Add(column[i]);
        }

        //blend all triangles
        for (int i = 0; i != centralItemsTriangles.Count; i++)
        {
            triangle = centralItemsTriangles[i];
            Color toColor = Color.Lerp(triangle.m_frontColor, itemBackgroundBlendColor, 0.25f);
            triangle.StartColorAnimation(true, toColor, 0.3f, 0.0f);
        }
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
                ShowCentralItem();
            }
        }
    }
}