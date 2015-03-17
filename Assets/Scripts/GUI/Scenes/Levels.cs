using UnityEngine;

public class Levels : GUIScene
{
    private GameObject m_levelsHolder;

    public GameObject m_levelsTitlePfb;
    public GameObject m_levelSlotPfb;
    public GameObject m_chapterInfoPfb;

    public GameObject[] m_levelSlots { get; set; }

    private float m_levelsGridHeight; //the maximum height of the levels grid
 
    /**
     * Shows this scene
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);

        CreateLevelsHolder();
        InitLevelsGridHeight();

        GameObjectAnimator levelsAnimator = this.GetComponent<GameObjectAnimator>();
        levelsAnimator.OnOpacityChanged(1);

        ShowTitle(fDelay);
        ShowChapterInfo(fDelay);
        ShowLevelsSlots(fDelay);
    }

    /**
     * Dismisses this scene
     * **/
    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    /**
     * Instantiates the object that will hold grid level slots
     * **/
    public void CreateLevelsHolder()
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        m_levelsHolder = new GameObject("LevelsHolder");
        m_levelsHolder.transform.parent = this.gameObject.transform;
        m_levelsHolder.transform.localPosition = new Vector3(0, -42.5f / 1280.0f * screenSize.y, -20);
        m_levelsHolder.AddComponent<GameObjectAnimator>();
    }

    /**
     * Calculates the height of the levels grid
     * **/
    private void InitLevelsGridHeight()
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        float topFrameHeight = 0.144f * screenSize.y; //the height of the top frame containing back button and title
        float bottomFrameHeight = 0.078f * screenSize.y; //the height of the bottom frame containing chapter info
        float borderHeight = 20.0f / 1980.0f * screenSize.y;
        m_levelsGridHeight = screenSize.y - topFrameHeight - bottomFrameHeight - 2 * borderHeight;
    }

    /**
    * Shows title
    **/
    public void ShowTitle(float fDelay = 0.0f)
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        GameObject titleObject = (GameObject)Instantiate(m_levelsTitlePfb);
        titleObject.transform.parent = this.gameObject.transform;
        titleObject.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, -20);
        //TextMesh titleTextMesh = titleObject.GetComponent<TextMesh>();
        GameObjectAnimator titleAnimator = titleObject.GetComponent<GameObjectAnimator>();

        titleAnimator.OnOpacityChanged(0);
        titleAnimator.FadeFromTo(0, 1, 0.5f, fDelay);
    }

    /**
    * Shows information about the selected chapter
    **/
    public void ShowChapterInfo(float fDelay = 0.0f)
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        GameObject chapterObject = (GameObject)Instantiate(m_chapterInfoPfb);
        chapterObject.transform.parent = this.gameObject.transform;
        chapterObject.transform.localPosition = new Vector3(0, -0.5f * screenSize.y + 118.0f, -20);

        TextMesh[] childTextMeshes = chapterObject.GetComponentsInChildren<TextMesh>();

        TextMesh chapterNumberTextMesh = childTextMeshes[0];
        chapterNumberTextMesh.text = levelManager.m_currentChapter.m_number.ToString();
    }

    /**
     * Shows levels slots
     * **/
    public void ShowLevelsSlots(float fDelay)
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        m_levelSlots = new GameObject[Chapter.LEVELS_COUNT];

        float verticalDistanceBetweenLevelSlots = m_levelsGridHeight / 4.2f;
        float horizontalDistanceBetweenLevelSlots = 340.0f / 240.0f * verticalDistanceBetweenLevelSlots;
        for (int iLevelSlotIdx = 0; iLevelSlotIdx != 16; iLevelSlotIdx++)
        {
            GameObject clonedLevelSlot = (GameObject)Instantiate(m_levelSlotPfb);
            m_levelSlots[iLevelSlotIdx] = clonedLevelSlot;
            clonedLevelSlot.transform.parent = m_levelsHolder.transform;

            int column = iLevelSlotIdx % 4 + 1;
            int line = 4 - iLevelSlotIdx / 4;

            Vector3 levelSlotPosition = new Vector3((column - 2.5f) * horizontalDistanceBetweenLevelSlots,
                                                    (line - 2.5f) * verticalDistanceBetweenLevelSlots,
                                                    0);

            clonedLevelSlot.transform.localPosition = levelSlotPosition;

            ColorNumberSlot slotData = clonedLevelSlot.GetComponent<ColorNumberSlot>();
            slotData.Init();
            slotData.SetNumber(iLevelSlotIdx + 1);

            Color slotBaseColor = levelManager.GetChapterGroupBaseColor(levelManager.GetCurrentChapterGroup());
            Color slotColor = ColorUtils.DarkenColor(slotBaseColor, 0.18f * (column - 1));
            slotData.SetColor(slotColor);
        }

        GameObjectAnimator levelsHolderAnimator = m_levelsHolder.GetComponent<GameObjectAnimator>();
        levelsHolderAnimator.OnOpacityChanged(0);
        levelsHolderAnimator.FadeFromTo(0, 1, 0.5f, fDelay);
    }

    public void OnClickLevelSlot(int iLevelSlotIndex)
    {
        Debug.Log("OnClickLevelSlot " + (iLevelSlotIndex + 1));
    }
}