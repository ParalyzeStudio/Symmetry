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
        
        GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowLevelsSlots), 1.0f);        
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

    public LevelSlot BuildLevelSlotForNumber(int iLevelNumber)
    {
        GameObject levelSlotObject = (GameObject)Instantiate(m_levelSlotPfb);

        LevelSlot levelSlot = levelSlotObject.GetComponent<LevelSlot>();
        levelSlot.Init(this, iLevelNumber);

        GameObjectAnimator levelSlotAnimator = levelSlotObject.GetComponent<GameObjectAnimator>();
        levelSlotAnimator.SetParentTransform(this.transform);

        return levelSlot;
    }

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

        //Load and display LevelIntro scene
        //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 0.5f, 1.1f);

        //GetGUIManager().DismissBackButton();
        //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 0.0f, 1.1f);
    }
}