using UnityEngine;

public class Levels : GUIScene
{
    public const float LEVELS_SLOTS_Z_VALUE = -10.0f;
    public const float BACK_BUTTON_Z_VALUE = -10.0f;

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

        ApplyBackgroundGradient();

        ShowLevelsSlots();
        ShowDebugLevelSlots();
        ShowBackButton();
    }

    private void ApplyBackgroundGradient()
    {
        if (GetBackgroundRenderer().m_gradient == null)
        {
            Chapter displayedChapter = GetLevelManager().m_currentChapter;

            Gradient gradient = new Gradient();
            gradient.CreateRadial(Vector2.zero,
                                  960,
                                  displayedChapter.GetThemeColors()[0],
                                  displayedChapter.GetThemeColors()[1]);


            GetBackgroundRenderer().ApplyGradient(gradient,
                                                  0.02f,
                                                  true,
                                                  BackgroundTrianglesRenderer.GradientAnimationPattern.EXPANDING_CIRCLE,
                                                  0.5f,
                                                  0.0f,
                                                  0.0f,
                                                  false);
        }
    }

    /**
     * Build the slots for every level in that chapter
     * **/
    public void ShowLevelsSlots()
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
    }

    /**
     * Show a button to display a debug level
     * **/
    private void ShowDebugLevelSlots()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //debug level 1
        GameObject debugLevelButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_DEBUG_LEVEL1, new Vector2(256, 128));
        debugLevelButtonObject.name = "DebugLevel1Btn";

        GameObjectAnimator debugLevelButtonAnimator = debugLevelButtonObject.GetComponent<GameObjectAnimator>();
        debugLevelButtonAnimator.SetParentTransform(this.transform);
        debugLevelButtonAnimator.SetPosition(new Vector3(-0.25f * screenSize.x, -0.5f * screenSize.y + 100, -200));

        //debug level 2
        debugLevelButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_DEBUG_LEVEL2, new Vector2(256, 128));
        debugLevelButtonObject.name = "DebugLevel2Btn";

        debugLevelButtonAnimator = debugLevelButtonObject.GetComponent<GameObjectAnimator>();
        debugLevelButtonAnimator.SetParentTransform(this.transform);
        debugLevelButtonAnimator.SetPosition(new Vector3(0, -0.5f * screenSize.y + 100, -200));

        //debug level 3
        debugLevelButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_DEBUG_LEVEL3, new Vector2(256, 128));
        debugLevelButtonObject.name = "DebugLevel3Btn";

        debugLevelButtonAnimator = debugLevelButtonObject.GetComponent<GameObjectAnimator>();
        debugLevelButtonAnimator.SetParentTransform(this.transform);
        debugLevelButtonAnimator.SetPosition(new Vector3(0.25f * screenSize.x, -0.5f * screenSize.y + 100, -200));
    }

    /**
     * Show the button to go back to chapters screen
     * **/
    private void ShowBackButton()
    {
        Vector2 backButtonSize = new Vector2(128, 128);
        GameObject backButtonObject = GetGUIManager().CreateGUIButtonForID(GUIButton.GUIButtonID.ID_BACK_BUTTON, backButtonSize);
        backButtonObject.name = "BackButton";

        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObjectAnimator backButtonAnimator = backButtonObject.GetComponent<GameObjectAnimator>();
        backButtonAnimator.SetParentTransform(this.transform);
        backButtonAnimator.SetPosition(new Vector3(-0.5f * screenSize.x + GetBackgroundRenderer().m_triangleHeight,
                                                   0.5f * screenSize.y - GetBackgroundRenderer().m_triangleEdgeLength, 
                                                   BACK_BUTTON_Z_VALUE));
    }

    /**
     * Build and render a hexagonal level slot for a particular level
     * **/
    public LevelSlot BuildLevelSlotForNumber(int iLevelNumber)
    {
        GameObject levelSlotObject = (GameObject)Instantiate(m_levelSlotPfb);

        LevelSlot levelSlot = levelSlotObject.GetComponent<LevelSlot>();
        levelSlot.Init(this, iLevelNumber);

        GameObjectAnimator levelSlotAnimator = levelSlotObject.GetComponent<GameObjectAnimator>();
        levelSlotAnimator.SetParentTransform(this.transform);

        return levelSlot;
    }

    /**
     * Call this method when a level slot is clicked.
     * Passed -1 if we clicked on the debug level button
     * **/
    public void OnClickLevelSlot(int iLevelSlotIndex)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Set the level in the LevelManager
        if (iLevelSlotIndex >= 0)
            GetLevelManager().SetLevelOnCurrentChapter(iLevelSlotIndex + 1);

        //Transition black screen
        Vector2 slotPosition;
        if (iLevelSlotIndex == -1) //debug level1
            slotPosition = new Vector3(-0.25f * screenSize.x, -0.5f * screenSize.y + 100);
        else if (iLevelSlotIndex == -2) //debug level2
            slotPosition = new Vector3(0, -0.5f * screenSize.y + 100);
        else if (iLevelSlotIndex == -3) //debug level3
            slotPosition = new Vector3(0.25f * screenSize.x, -0.5f * screenSize.y + 100);
        else
            slotPosition = m_levelSlots[iLevelSlotIndex].transform.position;

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

        GameObject apertureObject = (GameObject)Instantiate(m_circleMeshPfb);
        apertureObject.name = "Aperture";

        CircleMesh apertureMesh = apertureObject.GetComponent<CircleMesh>();
        apertureMesh.Init(Instantiate(m_positionColorMaterial));

        //remove the CircleMeshAnimator and add an ApertureTransitionAnimator instead
        CircleMeshAnimator hexaMeshAnimator = apertureObject.GetComponent<CircleMeshAnimator>();
        Destroy(hexaMeshAnimator);

        ApertureTransitionAnimator apertureAnimator = apertureObject.AddComponent<ApertureTransitionAnimator>();
        apertureAnimator.SetNumSegments(6, false);
        apertureAnimator.SetInnerRadius(2 / Mathf.Sqrt(3) * maxDistanceToScreenVertices, false);
        apertureAnimator.SetOuterRadius(2 / Mathf.Sqrt(3) * maxDistanceToScreenVertices, true);
        apertureAnimator.SetPosition(new Vector3(slotPosition.x, slotPosition.y, -50));
        apertureAnimator.SetColor(Color.black);

        apertureAnimator.m_toSceneContent = SceneManager.DisplayContent.LEVEL_INTRO;
        apertureAnimator.AnimateInnerRadiusTo(0, 0.5f);
    }

    public void OnClickBackButton()
    {
        for (int i = 0; i != m_levelSlots.Length; i++)
        {
            m_levelSlots[i].transform.parent = null;
            m_levelSlots[i].Dismiss(0.5f);
        }

        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.CHAPTERS, false);
    }
}