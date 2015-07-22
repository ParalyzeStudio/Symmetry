using UnityEngine;

public class Levels : GUIScene
{
    public const float LEVELS_SLOTS_Z_VALUE = -10.0f;
    public const float LEVEL_SLOT_BACKGROUND_OPACITY = 0.5f;

    //Shared prefabs
    public GameObject m_textMeshPfb;
    public GameObject m_circleMeshPfb;
    public Material m_positionColorMaterial;
    private Material m_slotBackgroundMaterial;

    //Level slots
    public GameObject[] m_levelSlots;

    //LevelSlot data
    //public LevelSlot[] m_levelSlots { get; set; }
 
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
     * Build the slots for every level in that chapter
     * **/
    public void ShowLevelsSlots(float fDelay = 0.0f)
    {
        int iLevelCount = 15;
        m_levelSlots = new GameObject[iLevelCount];
        m_slotBackgroundMaterial = Instantiate(m_positionColorMaterial);

        //build center slot
        GameObject slot = BuildLevelSlotForNumber(8);
        float line2PositionY = GetBackgroundRenderer().GetNearestTriangleToScreenYPosition(0, BackgroundTrianglesRenderer.NUM_COLUMNS / 2, 180).GetCenter().y;
        Vector3 slotPosition = new Vector3(0, line2PositionY, LEVELS_SLOTS_Z_VALUE);

        GameObjectAnimator slotAnimator = slot.GetComponent<GameObjectAnimator>();
        slotAnimator.SetPosition(slotPosition);

        m_levelSlots[7] = slot;

        //build slots above and below the center slot (number 3/15 and 13/15)
        slot = BuildLevelSlotForNumber(3);
        float line1PositionY = line2PositionY + 4 * GetBackgroundRenderer().m_triangleEdgeLength;
        slotPosition = new Vector3(0, line1PositionY, LEVELS_SLOTS_Z_VALUE);

        slotAnimator = slot.GetComponent<GameObjectAnimator>();
        slotAnimator.SetPosition(slotPosition);

        m_levelSlots[2] = slot;

        slot = BuildLevelSlotForNumber(13);
        float line3PositionY = line2PositionY - 4 * GetBackgroundRenderer().m_triangleEdgeLength;
        slotPosition = new Vector3(0, line3PositionY, LEVELS_SLOTS_Z_VALUE);

        slotAnimator = slot.GetComponent<GameObjectAnimator>();
        slotAnimator.SetPosition(slotPosition);

        m_levelSlots[12] = slot;

        //build other slots line by line
        for (int iLineIdx = 0; iLineIdx != 3; iLineIdx++)
        {
            int referenceLevelNumber;
            float referenceLinePositionY;
            if (iLineIdx == 0)
            {
                referenceLevelNumber = 3;
                referenceLinePositionY = line1PositionY;
            }
            else if (iLineIdx == 1)
            {
                referenceLevelNumber = 8;
                referenceLinePositionY = line2PositionY;
            }
            else
            {
                referenceLevelNumber = 13;
                referenceLinePositionY = line3PositionY;
            }

            //build slots on the left of reference slotIndex            
            slot = BuildLevelSlotForNumber(referenceLevelNumber - 1);
            slotAnimator = slot.GetComponent<GameObjectAnimator>();
            slotPosition = new Vector3(-4 * GetBackgroundRenderer().m_triangleHeight, referenceLinePositionY, LEVELS_SLOTS_Z_VALUE);
            slotAnimator.SetPosition(slotPosition);
            m_levelSlots[referenceLevelNumber - 2] = slot;

            slot = BuildLevelSlotForNumber(referenceLevelNumber - 2);
            slotAnimator = slot.GetComponent<GameObjectAnimator>();
            slotPosition = new Vector3(-8 * GetBackgroundRenderer().m_triangleHeight, referenceLinePositionY, LEVELS_SLOTS_Z_VALUE);
            slotAnimator.SetPosition(slotPosition);
            m_levelSlots[referenceLevelNumber - 3] = slot;

            //and on the right
            slot = BuildLevelSlotForNumber(referenceLevelNumber + 1);
            slotAnimator = slot.GetComponent<GameObjectAnimator>();
            slotPosition = new Vector3(4 * GetBackgroundRenderer().m_triangleHeight, referenceLinePositionY, LEVELS_SLOTS_Z_VALUE);
            slotAnimator.SetPosition(slotPosition);
            m_levelSlots[referenceLevelNumber] = slot;

            slot = BuildLevelSlotForNumber(referenceLevelNumber + 2);
            slotAnimator = slot.GetComponent<GameObjectAnimator>();
            slotPosition = new Vector3(8 * GetBackgroundRenderer().m_triangleHeight, referenceLinePositionY, LEVELS_SLOTS_Z_VALUE);
            slotAnimator.SetPosition(slotPosition);
            m_levelSlots[referenceLevelNumber + 1] = slot;
        }

        //animate slots
        float animationDuration = 0.5f;
        for (int i = 0; i != m_levelSlots.Length; i++)
        {
            GameObject levelSlotObject = m_levelSlots[i];

            //slotAnimator = m_levelSlots[i].GetComponent<GameObjectAnimator>();
            //slotAnimator.SetOpacity(1);
            //slotAnimator.FadeTo(1.0f, animationDuration, fDelay);

            CircleMeshAnimator levelSlotBackgroundAnimator = levelSlotObject.GetComponentInChildren<CircleMeshAnimator>();
            TextMeshAnimator levelSlotNumberAnimator = levelSlotObject.GetComponentInChildren<TextMeshAnimator>();

            levelSlotBackgroundAnimator.SetOpacity(0);
            levelSlotBackgroundAnimator.FadeTo(LEVEL_SLOT_BACKGROUND_OPACITY, animationDuration, 0.0f);

            levelSlotNumberAnimator.SetOpacity(0);
            levelSlotNumberAnimator.FadeTo(1.0f, animationDuration, 0.0f);
        }
    }

    public GameObject BuildLevelSlotForNumber(int iLevelNumber)
    {
        GameObject slot = new GameObject("Slot" + iLevelNumber);
        slot.transform.parent = this.transform;
        GameObjectAnimator slotAnimator = slot.AddComponent<GameObjectAnimator>();

        //Background
        Color slotColor = GetLevelManager().m_currentChapter.GetThemeColors()[2];

        GameObject slotBackground = (GameObject)Instantiate(m_circleMeshPfb);
        slotBackground.name = "SlotBackground";
        slotBackground.transform.parent = slot.transform;

        CircleMesh slotBgHexaMesh = slotBackground.GetComponent<CircleMesh>();
        slotBgHexaMesh.Init(m_slotBackgroundMaterial);

        CircleMeshAnimator slotBgAnimator = slotBackground.GetComponent<CircleMeshAnimator>();
        slotBgAnimator.SetNumSegments(6, false);
        slotBgAnimator.SetInnerRadius(0, false);
        slotBgAnimator.SetOuterRadius(GetBackgroundRenderer().m_triangleEdgeLength, true);
        slotBgAnimator.SetColor(slotColor);

        //number
        GameObject levelNumberObject = (GameObject)Instantiate(m_textMeshPfb);
        levelNumberObject.name = "SlotNumber";
        levelNumberObject.transform.parent = slot.transform;

        TextMesh numberTextMesh = levelNumberObject.GetComponent<TextMesh>();
        numberTextMesh.text = iLevelNumber.ToString();

        TextMeshAnimator numberAnimator = levelNumberObject.GetComponent<TextMeshAnimator>();
        numberAnimator.SetFontHeight(0.8f * GetBackgroundRenderer().m_triangleEdgeLength);
        numberAnimator.SetColor(Color.white);

        //make some adjustements on number x-position
        if (iLevelNumber == 11)
            numberAnimator.SetPosition(new Vector3(-3.0f, 0, -1));
        else if (iLevelNumber >= 10)
            numberAnimator.SetPosition(new Vector3(-5.0f, 0, -1));
        else
            numberAnimator.SetPosition(new Vector3(0, 0, -1));

        return slot;
    }

    ///**
    // * Show clickable level slots
    // * Build them starting from center line by line (line2, line1 and line3 in that order)
    // * **/
    //public void ShowLevelsSlots(float fDelay = 0.0f)
    //{
    //    m_levelSlots = new LevelSlot[15]; //TODO replace 15 by the value from the Chapter object

    //    BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

    //    int iReferenceColumnIndex = BackgroundTrianglesRenderer.NUM_COLUMNS / 2; //immediate column on the right of middle vertical line
    //    int iLine2ReferenceTriangleIndex = bgRenderer.GetNearestTriangleToScreenYPosition(0, iReferenceColumnIndex, 180).m_indexInColumn;

    //    //build the center slot (number 8/15)
    //    LevelSlot slot = new LevelSlot(8);
    //    slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex, iLine2ReferenceTriangleIndex);
    //    m_levelSlots[7] = slot;

    //    //build slots above and below the center slot (number 3/15 and 13/15)
    //    slot = new LevelSlot(3);
    //    int iLine1ReferenceTriangleIndex = iLine2ReferenceTriangleIndex - 8;
    //    slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex, iLine1ReferenceTriangleIndex);
    //    m_levelSlots[2] = slot;

    //    slot = new LevelSlot(13);
    //    int iLine3ReferenceTriangleIndex = iLine2ReferenceTriangleIndex + 8;
    //    slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex, iLine3ReferenceTriangleIndex);
    //    m_levelSlots[12] = slot;

    //    //build remaining slots line by line
    //    for (int iLineIdx = 0; iLineIdx != 3; iLineIdx++)
    //    {
    //        LevelSlot referenceSlot;
    //        int iReferenceTriangleIndex;
    //        if (iLineIdx == 0)
    //        {
    //            referenceSlot = m_levelSlots[2];
    //            iReferenceTriangleIndex = iLine1ReferenceTriangleIndex;
    //        }
    //        else if (iLineIdx == 1)
    //        {
    //            referenceSlot = m_levelSlots[7];
    //            iReferenceTriangleIndex = iLine2ReferenceTriangleIndex;
    //        }
    //        else
    //        {
    //            referenceSlot = m_levelSlots[12];
    //            iReferenceTriangleIndex = iLine3ReferenceTriangleIndex;
    //        }

    //        //build slots on the left of reference slotIndex            
    //        slot = new LevelSlot(referenceSlot.m_number - 1);
    //        slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex - 4, iReferenceTriangleIndex);
    //        m_levelSlots[referenceSlot.m_number - 2] = slot;
    //        slot = new LevelSlot(referenceSlot.m_number - 2);
    //        slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex - 8, iReferenceTriangleIndex);
    //        m_levelSlots[referenceSlot.m_number - 3] = slot;

    //        //and on the right
    //        slot = new LevelSlot(referenceSlot.m_number + 1);
    //        slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex + 4, iReferenceTriangleIndex);
    //        m_levelSlots[referenceSlot.m_number] = slot;
    //        slot = new LevelSlot(referenceSlot.m_number + 2);
    //        slot.BuildTrianglesFromFarRightTriangle(iReferenceColumnIndex + 8, iReferenceTriangleIndex);
    //        m_levelSlots[referenceSlot.m_number + 1] = slot;
    //    }

    //    //Set the blend color and the correct number for every slot
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    //    Chapter parentChapter = levelManager.m_currentChapter;
    //    Color blendColor = parentChapter.GetThemeColors()[2];
    //    GameObject levelsNumbersHolder = new GameObject("LevelsNumbersHolder");
    //    levelsNumbersHolder.transform.parent = this.transform;
    //    levelsNumbersHolder.transform.localPosition = new Vector3(0, 0, LEVELS_SLOTS_Z_VALUE);

    //    for (int i = 0; i != m_levelSlots.Length; i++)
    //    {
    //        slot = m_levelSlots[i];

    //        slot.m_blendColor = blendColor;

    //        GameObject clonedLevelNumberTextObject = (GameObject)Instantiate(m_textMeshPfb);
    //        clonedLevelNumberTextObject.GetComponent<TextMesh>().text = slot.m_number.ToString();
    //        clonedLevelNumberTextObject.transform.parent = levelsNumbersHolder.transform;

    //        Vector3 numberPosition = GeometryUtils.BuildVector3FromVector2(slot.GetCenter(), 0);
    //        if (slot.m_number >= 10)
    //            numberPosition -= new Vector3(5.0f, 0, 0);
    //        TextMeshAnimator numberAnimator = clonedLevelNumberTextObject.GetComponent<TextMeshAnimator>();
    //        numberAnimator.SetFontHeight(0.8f * bgRenderer.m_triangleEdgeLength);
    //        numberAnimator.SetPosition(numberPosition);
    //        slot.m_levelSlotNumberGameObject = clonedLevelNumberTextObject;

    //        slot.Show(blendColor, GUISlot.BLEND_COLOR_DEFAULT_PROPORTION);
    //    }       
    //}

    public void OnClickLevelSlot(int iLevelSlotIndex)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Set the level in the LevelManager
        GetLevelManager().SetLevelOnCurrentChapter(iLevelSlotIndex + 1);

        //Transition black screen
        Vector2 slotPosition = m_levelSlots[iLevelSlotIndex].transform.position;

        float[] distancesToScreenVertices = new float[4];
        distancesToScreenVertices[0] = (slotPosition - new Vector2(-0.5f * screenSize.x, 0.5f * screenSize.y)).magnitude; //distance to the top-left vertex
        distancesToScreenVertices[1] = (slotPosition - new Vector2(0.5f * screenSize.x, 0.5f * screenSize.y)).magnitude; //distance to the top-right vertex
        distancesToScreenVertices[2] = (slotPosition - new Vector2(0.5f * screenSize.x, -0.5f * screenSize.y)).magnitude; //distance to the bottom-right vertex
        distancesToScreenVertices[3] = (slotPosition - new Vector2(-0.5f * screenSize.x, -0.5f * screenSize.y)).magnitude; //distance to the bottom-left vertex
        float maxDistanceToScreenVertices = Mathf.Max(distancesToScreenVertices);

        //float[] distancesToScreenBorders = new float[4];
        //distancesToScreenBorders[0] = slotPosition.x + 0.5f * screenSize.x; //distance to the left border
        //distancesToScreenBorders[1] = 0.5f * screenSize.x - slotPosition.x; //distance to the right border
        //distancesToScreenBorders[2] = slotPosition.y + 0.5f * screenSize.y; //distance to the bottom border
        //distancesToScreenBorders[3] = 0.5f * screenSize.y - slotPosition.y; //distance to the top border
        //float maxDistanceToScreenBorders = Mathf.Max(distancesToScreenBorders);


        GameObject hexagonMeshObject = (GameObject)Instantiate(m_circleMeshPfb);

        CircleMesh hexaMesh = hexagonMeshObject.GetComponent<CircleMesh>();
        hexaMesh.Init(Instantiate(m_positionColorMaterial));
        
        //remove the CircleMeshAnimator and add an ApertureTransitionAnimator instead
        CircleMeshAnimator hexaMeshAnimator = hexagonMeshObject.GetComponent<CircleMeshAnimator>();
        Destroy(hexaMeshAnimator);

        hexaMeshAnimator = hexagonMeshObject.AddComponent<ApertureTransitionAnimator>();
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(2 / Mathf.Sqrt(3) * maxDistanceToScreenVertices, false);
        hexaMeshAnimator.SetOuterRadius(2 / Mathf.Sqrt(3) * maxDistanceToScreenVertices, true);
        hexaMeshAnimator.SetPosition(new Vector3(slotPosition.x, slotPosition.y, -50));
        hexaMeshAnimator.SetColor(Color.black);

        hexaMeshAnimator.AnimateInnerRadiusTo(0, 0.5f);

        //Load and display LevelIntro scene
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 0.5f, 1.1f);

        //GetGUIManager().DismissBackButton();
        //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 0.0f, 1.1f);
    }
}