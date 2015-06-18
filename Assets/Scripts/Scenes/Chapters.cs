using UnityEngine;

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
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_chaptersHolder = new GameObject("ChaptersHolder");
        m_chaptersHolder.transform.parent = this.gameObject.transform;
        m_chaptersHolder.transform.localPosition = new Vector3(0, -0.072f * screenSize.y, -20);
        m_chaptersHolder.AddComponent<GameObjectAnimator>();
    }

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
        m_showChaptersCallRunning = false;
        ShowTitle(bAnimated, fDelay);
        ShowChapterSlots(iReachedChapterGroup, bAnimated, fDelay);

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.ShowBackButton(fDelay);
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
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject titleObject = (GameObject)Instantiate(m_chaptersTitlePfb);
        titleObject.transform.parent = this.gameObject.transform;
        titleObject.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, -20);
        //TextMesh titleTextMesh = titleObject.GetComponent<TextMesh>();

        GameObjectAnimator titleAnimator = titleObject.GetComponent<GameObjectAnimator>();
        titleAnimator.SetOpacity(0);
        titleAnimator.FadeTo(1, 0.5f, fDelay);
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
            slotProperties.SetColor(slotColor);
        }

        GameObjectAnimator chaptersHolderAnimator = m_chaptersHolder.GetComponent<GameObjectAnimator>();      

        if (bAnimated)
        {
            m_showChaptersCallRunning = true;
            m_showChaptersDelay = fDelay;

            chaptersHolderAnimator.SetOpacity(1);

            GameObjectAnimator slotAnimator = m_chapterSlots[0].GetComponent<GameObjectAnimator>();
            slotAnimator.SetOpacity(0);

            slotAnimator = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
            slotAnimator.SetPosition(new Vector3(0, 0.5f * slotSize.y, 0));
            slotAnimator.SetRotationAxis(new Vector3(0, 1, 0));
            slotAnimator.SetRotationAngle(90);
            slotAnimator.SetOpacity(0);

            slotAnimator = m_chapterSlots[2].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.5f));
            slotAnimator.SetPosition(new Vector3(0, -0.5f * slotSize.y, 0));
            slotAnimator.SetRotationAxis(new Vector3(0, -1, 0));
            slotAnimator.SetRotationAngle(90);
            slotAnimator.SetOpacity(0);

            slotAnimator = m_chapterSlots[3].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            slotAnimator.SetPosition(new Vector3(0.5f * slotSize.y, 0, 0));
            slotAnimator.SetRotationAxis(new Vector3(1, 0, 0));
            slotAnimator.SetRotationAngle(90);
            slotAnimator.SetOpacity(0);
        }
        else
        {
            chaptersHolderAnimator.SetOpacity(0);
            chaptersHolderAnimator.FadeTo(1, 0.5f, fDelay);
        }
    }

    public void ShowChapterSlotsWithAnimation()
    {
        m_showChaptersCallRunning = false;

        float slotAnimationDuration = 0.3f;

        //Animate slot 1
        GameObjectAnimator slotAnimator = m_chapterSlots[0].GetComponent<GameObjectAnimator>();
        slotAnimator.FadeTo(1, slotAnimationDuration);

        //Animate slot 2
        slotAnimator = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateTo(0, slotAnimationDuration, slotAnimationDuration);
        slotAnimator.FadeTo(1, slotAnimationDuration, slotAnimationDuration);

        //Animate slot 3
        slotAnimator = m_chapterSlots[2].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateTo(0, slotAnimationDuration, 3 * slotAnimationDuration);
        slotAnimator.FadeTo(1, slotAnimationDuration, 3 * slotAnimationDuration);

        //Animate slot 4
        slotAnimator = m_chapterSlots[3].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateTo(0, slotAnimationDuration, 2 * slotAnimationDuration);
        slotAnimator.FadeTo(1, slotAnimationDuration, 2 * slotAnimationDuration);
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

        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, true, 0.0f, 0.7f);
    }
}