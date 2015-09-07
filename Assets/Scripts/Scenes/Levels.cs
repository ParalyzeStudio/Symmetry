using UnityEngine;

public class Levels : GUIScene
{
    public const float LEVELS_SLOTS_Z_VALUE = -10.0f;

    //Shared prefabs
    //public GameObject m_texQuadPfb;
    //public GameObject m_textMeshPfb;
    public GameObject m_circleMeshPfb;
    public Material m_positionColorMaterial;    

    //Level slots
    public GameObject m_levelSlotPfb;
    public Material m_slotBackgroundMaterial { get; set; }
    public LevelSlot[] m_levelSlots { get; set; }
    public Material m_glowContourMaterial;
    public Material m_slotGlowContourMaterial { get; set; }
    public Material m_slotSharpContourMaterial { get; set; }

    //LevelSlot data
    //public LevelSlot[] m_levelSlots { get; set; }
 
    /**
     * Shows this scene
     * **/
    public override void Show()
    {
        base.Show();
        
        ShowLevelsSlots(0.5f);        
    }

    /**
     * Build the slots for every level in that chapter
     * **/
    public void ShowLevelsSlots(float fDelay = 0.0f)
    {
        int iLevelCount = 15;
        m_levelSlots = new LevelSlot[iLevelCount];
        m_slotBackgroundMaterial = Instantiate(m_positionColorMaterial);
        m_slotGlowContourMaterial = Instantiate(m_glowContourMaterial);
        m_slotSharpContourMaterial = Instantiate(m_positionColorMaterial);

        //build center slot
        LevelSlot slot = BuildLevelSlotForNumber(8);
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
        for (int i = 0; i != m_levelSlots.Length; i++)
        {
            m_levelSlots[i].Show();
        }

        //animate slots
        //float animationDuration = 5.0f;
        //for (int i = 0; i != m_levelSlots.Length; i++)
        //{
        //    LevelSlot levelSlotObject = m_levelSlots[i];

        //    CircleMeshAnimator levelSlotBackgroundAnimator = levelSlotObject.GetComponentInChildren<CircleMeshAnimator>();
        //    TextMeshAnimator levelSlotNumberAnimator = levelSlotObject.GetComponentInChildren<TextMeshAnimator>();

        //    if (levelSlotBackgroundAnimator != null) //we have a background to animate (i.e level is done)
        //    {
        //        levelSlotBackgroundAnimator.SetOpacity(0);
        //        levelSlotBackgroundAnimator.FadeTo(LEVEL_SLOT_BACKGROUND_OPACITY, animationDuration, fDelay);
        //    }

        //    levelSlotNumberAnimator.SetOpacity(0);
        //    levelSlotNumberAnimator.FadeTo(animationDuration, animationDuration, fDelay);
        //}
    }

    public LevelSlot BuildLevelSlotForNumber(int iLevelNumber)
    {
        GameObject levelSlotObject = (GameObject)Instantiate(m_levelSlotPfb);
        levelSlotObject.transform.parent = this.transform;

        LevelSlot levelSlot = levelSlotObject.GetComponent<LevelSlot>();
        levelSlot.Init(this, iLevelNumber);

        return levelSlot;
    }

    //public GameObject BuildLevelSlotForNumber(int iLevelNumber)
    //{
    //    GameObject slot = new GameObject("Slot" + iLevelNumber);
    //    slot.transform.parent = this.transform;
    //    GameObjectAnimator slotAnimator = slot.AddComponent<GameObjectAnimator>();

    //    //Add a background only if level is completed        
    //    int iAbsoluteLevelNumber = GetLevelManager().GetAbsoluteLevelNumberForCurrentChapterAndLevel(iLevelNumber);
    //    //if (GetPersistentDataManager().IsLevelDone(iAbsoluteLevelNumber))
    //        if (true)
    //    {
    //        Color slotColor = GetLevelManager().m_currentChapter.GetThemeColors()[2];

    //        GameObject slotBackground = (GameObject)Instantiate(m_circleMeshPfb);
    //        slotBackground.name = "SlotBackground";
    //        slotBackground.transform.parent = slot.transform;

    //        CircleMesh slotBgHexaMesh = slotBackground.GetComponent<CircleMesh>();
    //        slotBgHexaMesh.Init(m_slotBackgroundMaterial);

    //        CircleMeshAnimator slotBgAnimator = slotBackground.GetComponent<CircleMeshAnimator>();
    //        slotBgAnimator.SetNumSegments(6, false);
    //        slotBgAnimator.SetInnerRadius(0, false);
    //        slotBgAnimator.SetOuterRadius(GetBackgroundRenderer().m_triangleEdgeLength, true);
    //        slotBgAnimator.SetColor(slotColor);
    //    }

    //    //Add a contour to the hexagon
    //    GameObject contourObject = (GameObject)Instantiate(m_texQuadPfb);
    //    contourObject.name = "SlotContour";
    //    contourObject.transform.parent = slot.transform;

    //    contourObject.GetComponent<UVQuad>().Init(m_glowContourMaterial);

    //    TexturedQuadAnimator contourAnimator = contourObject.GetComponent<TexturedQuadAnimator>();
    //    float contourTextureScale = 2 * GetBackgroundRenderer().m_triangleEdgeLength / 168.0f; //hexagon is 2 * triangleEdgeLength size, and the contour texture is 168x168 with 44 pixels blur/padding (256x256)
    //    contourAnimator.SetScale(new Vector3(contourTextureScale * 256, contourTextureScale * 256, 1));
    //    contourAnimator.SetPosition(new Vector3(0, 0, -1)); //set the contour above hexagon

    //    GameObject contourHexaObject = Instantiate(m_circleMeshPfb);
    //    contourHexaObject.name = "ContourHexagon";
    //    contourHexaObject.transform.parent = contourObject.transform;

    //    CircleMesh contourMesh = contourHexaObject.GetComponent<CircleMesh>();
    //    contourMesh.Init(Instantiate(m_positionColorMaterial));

    //    float contourMeshThickness = 4.0f;
    //    CircleMeshAnimator contourMeshAnimator = contourHexaObject.GetComponent<CircleMeshAnimator>();
    //    contourMeshAnimator.SetNumSegments(6, false);
    //    contourMeshAnimator.SetInnerRadius(GetBackgroundRenderer().m_triangleEdgeLength, false);
    //    contourMeshAnimator.SetOuterRadius(GetBackgroundRenderer().m_triangleEdgeLength + contourMeshThickness, true);
    //    contourMeshAnimator.SetColor(Color.white);
    //    contourMeshAnimator.SetPosition(Vector3.zero);

    //    //number
    //    GameObject levelNumberObject = (GameObject)Instantiate(m_textMeshPfb);
    //    levelNumberObject.name = "SlotNumber";
    //    levelNumberObject.transform.parent = slot.transform;

    //    TextMesh numberTextMesh = levelNumberObject.GetComponent<TextMesh>();
    //    numberTextMesh.text = iLevelNumber.ToString();

    //    TextMeshAnimator numberAnimator = levelNumberObject.GetComponent<TextMeshAnimator>();
    //    numberAnimator.SetFontHeight(0.8f * GetBackgroundRenderer().m_triangleEdgeLength);
    //    numberAnimator.SetColor(Color.white);

    //    //make some adjustements on number x-position
    //    AdjustNumberPosition(iLevelNumber, numberAnimator);        

    //    return slot;
    //}

    /**
     * Slightly offset number positions so they appeared to be centered
     * **/
    //private void AdjustNumberPosition(int iLevelNumber, TextMeshAnimator numberAnimator)
    //{
    //    if (iLevelNumber == 11)
    //        numberAnimator.SetPosition(new Vector3(-3.0f, 0, -1));
    //    else if (iLevelNumber >= 10)
    //        numberAnimator.SetPosition(new Vector3(-5.0f, 0, -1));
    //    else
    //        numberAnimator.SetPosition(new Vector3(0, 0, -1));
    //}

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
    //    LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
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

        ApertureTransitionAnimator apertureAnimator = hexagonMeshObject.AddComponent<ApertureTransitionAnimator>();
        apertureAnimator.SetNumSegments(6, false);
        apertureAnimator.SetInnerRadius(2 / Mathf.Sqrt(3) * maxDistanceToScreenVertices, false);
        apertureAnimator.SetOuterRadius(2 / Mathf.Sqrt(3) * maxDistanceToScreenVertices, true);
        apertureAnimator.SetPosition(new Vector3(slotPosition.x, slotPosition.y, -50));
        apertureAnimator.SetColor(Color.black);

        apertureAnimator.m_toSceneContent = SceneManager.DisplayContent.LEVEL_INTRO;
        apertureAnimator.AnimateInnerRadiusTo(0, 0.5f);

        //Load and display LevelIntro scene
        //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 0.5f, 1.1f);

        //GetGUIManager().DismissBackButton();
        //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 0.0f, 1.1f);
    }
}