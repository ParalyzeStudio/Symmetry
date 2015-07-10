using UnityEngine;

public class Levels : GUIScene
{
    public const float LEVELS_SLOTS_Z_VALUE = -10.0f;

    public LevelSlot[] m_levelSlots { get; set; }
    public GameObject m_levelSlotNumberTextPfb;
 
    /**
     * Shows this scene
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);

        ShowLevelsSlots(fDelay);

        //GameObjectAnimator levelsAnimator = this.GetComponent<GameObjectAnimator>();
        //levelsAnimator.SetOpacity(1);

        //ShowTitle(fDelay);
        //ShowChapterInfo(fDelay);
        
    }

    /**
     * Dismisses this scene
     * **/
    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    ///**
    //* Shows title
    //**/
    //public void ShowTitle(float fDelay = 0.0f)
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    GameObject titleObject = (GameObject)Instantiate(m_levelsTitlePfb);
    //    titleObject.transform.parent = this.gameObject.transform;
    //    titleObject.transform.localPosition = new Vector3(0, 0.428f * screenSize.y, -20);
    //    //TextMesh titleTextMesh = titleObject.GetComponent<TextMesh>();
    //    GameObjectAnimator titleAnimator = titleObject.GetComponent<GameObjectAnimator>();

    //    titleAnimator.SetOpacity(0);
    //    titleAnimator.FadeTo(1, 0.5f, fDelay);
    //}

    ///**
    //* Shows information about the selected chapter
    //**/
    //public void ShowChapterInfo(float fDelay = 0.0f)
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

    //    GameObject chapterInfoObject = (GameObject)Instantiate(m_chapterInfoPfb);
    //    chapterInfoObject.transform.parent = this.gameObject.transform;
    //    chapterInfoObject.transform.localPosition = new Vector3(0, -0.5f * screenSize.y + 108.0f, -20);

    //    TextMesh[] childTextMeshes = chapterInfoObject.GetComponentsInChildren<TextMesh>();

    //    TextMesh chapterNumberTextMesh = childTextMeshes[0];
    //    chapterNumberTextMesh.text = levelManager.m_currentChapter.m_number.ToString();

    //    GameObjectAnimator chapterInfoAnimator = chapterInfoObject.GetComponent<GameObjectAnimator>();
    //    chapterInfoAnimator.SetOpacity(0);
    //    chapterInfoAnimator.FadeTo(1, 0.5f, fDelay);
    //}

    ///**
    // * Shows levels slots
    // * **/
    //public void ShowLevelsSlots(float fDelay)
    //{
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    //    m_levelSlots = new GameObject[Chapter.LEVELS_COUNT];

    //    float verticalDistanceBetweenLevelSlots = m_levelsGridHeight / 4.2f;
    //    float horizontalDistanceBetweenLevelSlots = 340.0f / 240.0f * verticalDistanceBetweenLevelSlots;
    //    for (int iLevelSlotIdx = 0; iLevelSlotIdx != LevelManager.LEVELS_PER_CHAPTER; iLevelSlotIdx++)
    //    {
    //        GameObject clonedLevelSlot = (GameObject)Instantiate(m_levelSlotPfb);
    //        m_levelSlots[iLevelSlotIdx] = clonedLevelSlot;
    //        clonedLevelSlot.transform.parent = m_levelsHolder.transform;

    //        int column = iLevelSlotIdx % 4 + 1;
    //        int line = 4 - iLevelSlotIdx / 4;

    //        Vector3 levelSlotPosition = new Vector3((column - 2.5f) * horizontalDistanceBetweenLevelSlots,
    //                                                (line - 2.5f) * verticalDistanceBetweenLevelSlots,
    //                                                0);

    //        clonedLevelSlot.transform.localPosition = levelSlotPosition;

    //        ColorNumberSlot levelSlot = clonedLevelSlot.GetComponent<ColorNumberSlot>();
    //        levelSlot.Init();
    //        levelSlot.SetNumber(iLevelSlotIdx + 1);

    //        Color slotBaseColor = levelManager.GetChapterGroupBaseColor(levelManager.GetCurrentChapterGroup());
    //        Color slotColor = ColorUtils.DarkenColor(slotBaseColor, 0.18f * (column - 1));
    //        SlotAnimator slotAnimator = levelSlot.GetComponent<SlotAnimator>();
    //        slotAnimator.SetColor(slotColor);
    //    }

    //    GameObjectAnimator levelsHolderAnimator = m_levelsHolder.GetComponent<GameObjectAnimator>();
    //    levelsHolderAnimator.SetOpacity(0);
    //    levelsHolderAnimator.FadeTo(1, 0.5f, fDelay);
    //}

    /**
     * Show clickable level slots
     * Build them starting from center line by line (line2, line1 and line3 in that order)
     * **/
    public void ShowLevelsSlots(float fDelay = 0.0f)
    {
        m_levelSlots = new LevelSlot[15]; //TODO replace 15 by the value from the Chapter object

        BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        int iReferenceColumnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2; //immediate column on the right of middle vertical line
        int iLine2ReferenceTriangleIndex = bgRenderer.GetNearestTriangleToScreenYPosition(0, iReferenceColumnIndex, 180).m_indexInColumn;

        //build the center slot (number 8/15)
        LevelSlot slot = new LevelSlot(8);
        slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex, iLine2ReferenceTriangleIndex);
        m_levelSlots[7] = slot;

        //build slots above and below the center slot (number 3/15 and 13/15)
        slot = new LevelSlot(3);
        int iLine1ReferenceTriangleIndex = iLine2ReferenceTriangleIndex - 8;
        slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex, iLine1ReferenceTriangleIndex);
        m_levelSlots[2] = slot;

        slot = new LevelSlot(13);
        int iLine3ReferenceTriangleIndex = iLine2ReferenceTriangleIndex + 8;
        slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex, iLine3ReferenceTriangleIndex);
        m_levelSlots[12] = slot;

        //build remaining slots line by line
        for (int iLineIdx = 0; iLineIdx != 3; iLineIdx++)
        {
            LevelSlot referenceSlot;
            int iReferenceTriangleIndex;
            if (iLineIdx == 0)
            {
                referenceSlot = m_levelSlots[2];
                iReferenceTriangleIndex = iLine1ReferenceTriangleIndex;
            }
            else if (iLineIdx == 1)
            {
                referenceSlot = m_levelSlots[7];
                iReferenceTriangleIndex = iLine2ReferenceTriangleIndex;
            }
            else
            {
                referenceSlot = m_levelSlots[12];
                iReferenceTriangleIndex = iLine3ReferenceTriangleIndex;
            }

            //build slots on the left of reference slotIndex            
            slot = new LevelSlot(referenceSlot.m_number - 1);
            slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex - 4, iReferenceTriangleIndex);
            m_levelSlots[referenceSlot.m_number - 2] = slot;
            slot = new LevelSlot(referenceSlot.m_number - 2);
            slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex - 8, iReferenceTriangleIndex);
            m_levelSlots[referenceSlot.m_number - 3] = slot;

            //and on the right
            slot = new LevelSlot(referenceSlot.m_number + 1);
            slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex + 4, iReferenceTriangleIndex);
            m_levelSlots[referenceSlot.m_number] = slot;
            slot = new LevelSlot(referenceSlot.m_number + 2);
            slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex + 8, iReferenceTriangleIndex);
            m_levelSlots[referenceSlot.m_number + 1] = slot;
        }

        //Set the blend color and the correct number for every slot
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        Chapter parentChapter = levelManager.m_currentChapter;
        Color blendColor = parentChapter.GetThemeColors()[2];
        GameObject levelsNumbersHolder = new GameObject("LevelsNumbersHolder");
        levelsNumbersHolder.transform.parent = this.transform;
        levelsNumbersHolder.transform.localPosition = new Vector3(0, 0, LEVELS_SLOTS_Z_VALUE);

        for (int i = 0; i != m_levelSlots.Length; i++)
        {
            slot = m_levelSlots[i];

            slot.m_blendColor = blendColor;

            GameObject clonedLevelNumberTextObject = (GameObject)Instantiate(m_levelSlotNumberTextPfb);
            clonedLevelNumberTextObject.GetComponent<TextMesh>().text = slot.m_number.ToString();
            clonedLevelNumberTextObject.transform.parent = levelsNumbersHolder.transform;
            clonedLevelNumberTextObject.transform.localPosition = GeometryUtils.BuildVector3FromVector2(slot.GetCenter(), 0);
            slot.m_levelSlotNumberGameObject = clonedLevelNumberTextObject;

            slot.Show(blendColor, GUISlot.BLEND_COLOR_DEFAULT_PROPORTION);
        }       
    }

    public void OnClickLevelSlot(int iLevelSlotIndex)
    {
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.DismissBackButton();

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        levelManager.SetLevelOnCurrentChapter(iLevelSlotIndex + 1);

        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 0.0f, 1.1f);
    }
}