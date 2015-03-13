using UnityEngine;

public class Chapters : GUIScene
{
    public const int CHAPTERS_PER_GROUP = 4;
    public Color CHAPTER_GROUP_1_BASE_COLOR { get { return new Color(1, 0, 0, 1); } }
    public Color CHAPTER_GROUP_2_BASE_COLOR { get { return new Color(0, 0, 1, 1); } }
    public Color CHAPTER_GROUP_3_BASE_COLOR { get { return new Color(0, 1, 0, 1); } }

    public GameObject m_chaptersTitlePfb;
    public GameObject m_chapterSlotPfb;
    public GameObject m_levelSlotPfb;

    private GameObject m_chaptersHolder;
    private GameObject m_levelsHolder;

    public GameObject[] m_chapterSlots { get; set; }
    public GameObject[] m_levelSlots { get; set; }

    private int m_chapterGroup;

    private float m_showChaptersDelay;
    private float m_showChaptersElpasedTime;
    private bool m_showChaptersCallRunning;

    public void CreateChaptersHolder()
    {
        m_chaptersHolder = new GameObject("ChaptersHolder");
        m_chaptersHolder.transform.parent = this.gameObject.transform;
        m_chaptersHolder.transform.localPosition = new Vector3(0, 0, -20);

    }

    public void CreateLevelsHolder()
    {
        m_levelsHolder = new GameObject("LevelsHolder");
        m_levelsHolder.transform.parent = this.gameObject.transform;
        m_levelsHolder.transform.localPosition = new Vector3(0, 0, -20);
    }

    /**
     * Shows Chapters screen with or without animation
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);
        GameObjectAnimator chaptersAnimator = this.GetComponent<GameObjectAnimator>();
        chaptersAnimator.OnOpacityChanged(1);
        int reachedChapterNumber = 1;
        m_chapterGroup = ((reachedChapterNumber - 1) / CHAPTERS_PER_GROUP) + 1;
        m_showChaptersCallRunning = false;
        ShowTitle(bAnimated, fDelay);
        ShowChapterSlots(bAnimated, fDelay);
        ShowBackButton(bAnimated, fDelay);
    }

    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    public override void OnSceneDismissed()
    {
        base.OnSceneDismissed();
    }

    public void ShowTitle(bool bAnimated, float fDelay = 0.0f)
    {
        GameObject titleObject = (GameObject)Instantiate(m_chaptersTitlePfb);
        titleObject.transform.parent = this.gameObject.transform;
        //TextMesh titleTextMesh = titleObject.GetComponent<TextMesh>();
        GameObjectAnimator titleAnimator = titleObject.GetComponent<GameObjectAnimator>();

        titleAnimator.OnOpacityChanged(0);
        titleAnimator.FadeFromTo(0, 1, 0.5f, fDelay);
    }

    public void ShowChapterSlots(bool bAnimated, float fDelay = 0.0f)
    {
        CreateChaptersHolder();

        m_chapterSlots = new GameObject[4];
        Vector3 slotSize = Vector3.zero;
        for (int iChapterSlotIndex = 0; iChapterSlotIndex != CHAPTERS_PER_GROUP; iChapterSlotIndex++)
        {
            GameObject clonedChapterSlot = (GameObject) Instantiate(m_chapterSlotPfb);
            m_chapterSlots[iChapterSlotIndex] = clonedChapterSlot;

            //Find the position of each slot
            BoundingBoxCalculator bboxCalculator = clonedChapterSlot.GetComponent<BoundingBoxCalculator>();
            bboxCalculator.InvalidateBounds();
            if (slotSize == Vector3.zero)
                slotSize = bboxCalculator.m_bbox.size;
            Vector3 slotLocalPosition;
            if (iChapterSlotIndex == 0)
                slotLocalPosition = new Vector3(-0.5f * slotSize.x, 0.5f * slotSize.y, 0);
            else if (iChapterSlotIndex == 1)
                slotLocalPosition = new Vector3(0.5f * slotSize.x, 0.5f * slotSize.y, 0);
            else if (iChapterSlotIndex == 2)
                slotLocalPosition = new Vector3(-0.5f * slotSize.x, -0.5f * slotSize.y, 0);
            else
                slotLocalPosition = new Vector3(0.5f * slotSize.x, -0.5f * slotSize.y, 0);

            clonedChapterSlot.transform.parent = m_chaptersHolder.transform;
            clonedChapterSlot.transform.localPosition = slotLocalPosition;

            //Set the correct number on child text mesh and correct color on skin
            ColorNumberSlot slotProperties = clonedChapterSlot.GetComponent<ColorNumberSlot>();
            slotProperties.Init();
            slotProperties.SetNumber((m_chapterGroup - 1) * CHAPTERS_PER_GROUP + iChapterSlotIndex + 1);

            //Set the correct color
            Color baseColor;
            if (m_chapterGroup == 1)
                baseColor = CHAPTER_GROUP_1_BASE_COLOR;
            else if (m_chapterGroup == 2)
                baseColor = CHAPTER_GROUP_2_BASE_COLOR;
            else
                baseColor = CHAPTER_GROUP_3_BASE_COLOR;

            Color slotColor = ColorUtils.DarkenColor(baseColor, 0.18f * iChapterSlotIndex);
            if (bAnimated)
            {
                GameObjectAnimator slotAnimator = m_chapterSlots[iChapterSlotIndex].GetComponent<GameObjectAnimator>();
                slotAnimator.OnOpacityChanged(0);
                slotColor.a = 0;
            }
            slotProperties.SetColor(slotColor);
        }

        if (bAnimated)
        {
            m_showChaptersCallRunning = true;
            m_showChaptersDelay = fDelay;

            GameObjectAnimator slotAnimator = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
            slotAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0, 0.5f * slotSize.y, 0));
            slotAnimator.OnRotationChanged(90, new Vector3(0, 1, 0));

            slotAnimator = m_chapterSlots[2].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.5f));
            slotAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0, -0.5f * slotSize.y, 0));
            slotAnimator.OnRotationChanged(90, new Vector3(0, -1, 0));

            slotAnimator = m_chapterSlots[3].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            slotAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0.5f * slotSize.y, 0, 0));
            slotAnimator.OnRotationChanged(90, new Vector3(1, 0, 0));
        }        
    }

    public void ShowBackButton(bool bAnimated, float fDelay)
    {
        GUIInterfaceButton backButton = GUIInterfaceButton.FindInObjectChildrenForID(this.gameObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_BACK_BUTTON);
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;
        backButton.transform.localPosition = new Vector3(-0.5f * screenSize.x + 110.0f, 0.5f * screenSize.y - 90.0f, -20.0f);
        GameObjectAnimator showButtonAnimator = backButton.GetComponent<GameObjectAnimator>();
        showButtonAnimator.OnOpacityChanged(0);
        if (bAnimated)
            showButtonAnimator.FadeFromTo(0, 1, 0.5f, fDelay);
        else
            showButtonAnimator.OnOpacityChanged(1);
    }

    public void ShowChapterSlotsWithAnimation()
    {
        m_showChaptersCallRunning = false;

        float slotAnimationDuration = 0.3f;

        //Animate slot 1
        GameObjectAnimator slotAnimator = m_chapterSlots[0].GetComponent<GameObjectAnimator>();
        slotAnimator.FadeFromTo(0, 1, slotAnimationDuration);

        //Animate slot 2
        slotAnimator = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateFromToAroundAxis(90, 0, new Vector3(0, 1, 0), slotAnimationDuration, slotAnimationDuration);
        slotAnimator.FadeFromTo(0, 1, slotAnimationDuration, slotAnimationDuration);

        //Animate slot 3
        slotAnimator = m_chapterSlots[2].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateFromToAroundAxis(90, 0, new Vector3(0, -1, 0), slotAnimationDuration, 3 * slotAnimationDuration);
        slotAnimator.FadeFromTo(0, 1, slotAnimationDuration, 3 * slotAnimationDuration);

        //Animate slot 4
        slotAnimator = m_chapterSlots[3].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateFromToAroundAxis(90, 0, new Vector3(1, 0, 0), slotAnimationDuration, 2 * slotAnimationDuration);
        slotAnimator.FadeFromTo(0, 1, slotAnimationDuration, 2 * slotAnimationDuration);
    }

    public void ShowLevelsForChapter(int iChapterNumber)
    {
        Destroy(m_chaptersHolder);
        CreateLevelsHolder();

        //Create levels slots
        //for (int iChapterSlotIdx = 0; iChapterSlotIdx != CHAPTERS_PER_GROUP; iChapterSlotIdx++)
        //{
        //    GameObject chapterSlot = m_chapterSlots[iChapterSlotIdx];
        //    Vector3 chapterSlotPosition = chapterSlot.transform.localPosition;
        //    ColorNumberSlot chapterSlotData = chapterSlot.GetComponent<ColorNumberSlot>();

        //    for (int iLevelSlotIdx = 0; iLevelSlotIdx != 4; iLevelSlotIdx++)
        //    {
        //        GameObject clonedLevelSlot = (GameObject)Instantiate(m_levelSlotPfb);
        //        clonedLevelSlot.transform.parent = m_levelsHolder.transform;
        //        clonedLevelSlot.GetComponent<BoundingBoxCalculator>().InvalidateBounds();
        //        Vector2 levelSlotSize = clonedLevelSlot.GetComponent<GameObjectAnimator>().GetGameObjectSize();

        //        Vector3 levelSlotPosition;
        //        if (iLevelSlotIdx == 0)
        //            levelSlotPosition = chapterSlotPosition + 0.5f * new Vector3(-levelSlotSize.x, levelSlotSize.y, 0);
        //        else if (iLevelSlotIdx == 1)
        //            levelSlotPosition = chapterSlotPosition + 0.5f * new Vector3(levelSlotSize.x, levelSlotSize.y, 0);
        //         else if (iLevelSlotIdx == 2)
        //            levelSlotPosition = chapterSlotPosition + 0.5f * new Vector3(-levelSlotSize.x, -levelSlotSize.y, 0);
        //         else
        //            levelSlotPosition = chapterSlotPosition + 0.5f * new Vector3(levelSlotSize.x, -levelSlotSize.y, 0);
        //        levelSlotPosition.z = chapterSlotPosition.z;
        //        clonedLevelSlot.transform.localPosition = levelSlotPosition;

        //        ColorNumberSlot slotData = clonedLevelSlot.GetComponent<ColorNumberSlot>();
        //        slotData.Init();
        //        slotData.SetColor(chapterSlotData.m_color);
        //        slotData.SetNumber(iChapterSlotIdx * 4 + iLevelSlotIdx + 1);
        //    }
        //}

        float horizontalDistanceBetweenLevelSlots = 340.0f;
        float verticalDistanceBetweenLevelSlots = 240.0f;
        for (int iLevelSlotIdx = 0; iLevelSlotIdx != 16; iLevelSlotIdx++)
        {
            GameObject clonedLevelSlot = (GameObject)Instantiate(m_levelSlotPfb);
            clonedLevelSlot.transform.parent = m_levelsHolder.transform;
            //clonedLevelSlot.GetComponent<BoundingBoxCalculator>().InvalidateBounds();
            //Vector2 levelSlotSize = clonedLevelSlot.GetComponent<GameObjectAnimator>().GetGameObjectSize();

            int column = iLevelSlotIdx % 4 + 1;
            int line = 4 - iLevelSlotIdx / 4;

            Vector3 levelSlotPosition = new Vector3((column - 2.5f) * horizontalDistanceBetweenLevelSlots,
                                                    (line - 2.5f) * verticalDistanceBetweenLevelSlots,
                                                    0);

            clonedLevelSlot.transform.localPosition = levelSlotPosition;

            ColorNumberSlot slotData = clonedLevelSlot.GetComponent<ColorNumberSlot>();
            slotData.Init();
            slotData.SetColor(new Color(1, 0, 0, 1));
            slotData.SetNumber(iLevelSlotIdx + 1);
        }
    }

    public override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;

        if (m_showChaptersCallRunning)
        {
            m_showChaptersElpasedTime += dt;
            if (m_showChaptersElpasedTime >= m_showChaptersDelay)
            {
                ShowChapterSlotsWithAnimation();
            }
        }
    }

    public void OnClickChapterSlot(int iChapterSlotIndex)
    {
        int iChapterNumber = (m_chapterGroup - 1) * CHAPTERS_PER_GROUP + iChapterSlotIndex + 1;

        ShowLevelsForChapter(iChapterNumber);
    }
}

