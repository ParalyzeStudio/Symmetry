﻿using UnityEngine;

public class Chapters : GUIScene
{
    public GameObject m_chaptersTitlePfb;
    public GameObject m_chapterSlotPfb;

    private GameObject m_chaptersHolder;

    public int m_currentChapterGroup { get; set; }

    public GameObject[] m_chapterSlots { get; set; }

    private float m_showChaptersDelay;
    private float m_showChaptersElapsedTime;
    private bool m_showChaptersCallRunning;

    public void CreateChaptersHolder()
    {
        m_chaptersHolder = new GameObject("ChaptersHolder");
        m_chaptersHolder.transform.parent = this.gameObject.transform;
        m_chaptersHolder.transform.localPosition = new Vector3(0, 0, -20);
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
        int iReachedChapterGroup = ((reachedChapterNumber - 1) / LevelManager.CHAPTERS_PER_GROUP) + 1;
        m_showChaptersCallRunning = false;
        ShowTitle(bAnimated, fDelay);
        ShowChapterSlots(iReachedChapterGroup, bAnimated, fDelay);

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.ShowBackButton();
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

    public void ShowChapterSlots(int iChapterGroup, bool bAnimated, float fDelay = 0.0f)
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        m_currentChapterGroup = iChapterGroup;

        CreateChaptersHolder();

        m_chapterSlots = new GameObject[4];
        Vector3 slotSize = Vector3.zero;
        for (int iChapterSlotIndex = 0; iChapterSlotIndex != LevelManager.CHAPTERS_PER_GROUP; iChapterSlotIndex++)
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
            slotProperties.SetNumber((iChapterGroup - 1) * LevelManager.CHAPTERS_PER_GROUP + iChapterSlotIndex + 1);

            //Set the correct color
            Color baseColor = levelManager.GetChapterGroupBaseColor(iChapterGroup);

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

    public override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;

        if (m_showChaptersCallRunning)
        {
            m_showChaptersElapsedTime += dt;
            if (m_showChaptersElapsedTime >= m_showChaptersDelay)
            {
                ShowChapterSlotsWithAnimation();
            }
        }
    }

    public void OnClickChapterSlot(int iChapterSlotIndex)
    {
        int iChapterNumber = (m_currentChapterGroup - 1) * LevelManager.CHAPTERS_PER_GROUP + iChapterSlotIndex + 1;

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        levelManager.SetCurrentChapterByNumber(iChapterNumber);

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.SwitchDisplayedContent(GUIManager.DisplayContent.LEVELS);
    }
}