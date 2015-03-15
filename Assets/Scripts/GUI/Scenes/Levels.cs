using UnityEngine;

public class Levels : GUIScene
{
    private GameObject m_levelsHolder;

    public GameObject m_levelSlotPfb;

    public GameObject[] m_levelSlots { get; set; }
 
    /**
     * Shows this scene
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);
        GameObjectAnimator levelsAnimator = this.GetComponent<GameObjectAnimator>();
        levelsAnimator.OnOpacityChanged(1);

        ShowLevelsSlots();

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.DismissBackButton();
    }

    /**
     * Dismisses this scene
     * **/
    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    public void CreateLevelsHolder()
    {
        m_levelsHolder = new GameObject("LevelsHolder");
        m_levelsHolder.transform.parent = this.gameObject.transform;
        m_levelsHolder.transform.localPosition = new Vector3(0, 0, -20);
    }

    public void ShowLevelsSlots()
    {
    //    Destroy(m_chaptersHolder);
        CreateLevelsHolder();

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        float horizontalDistanceBetweenLevelSlots = 340.0f;
        float verticalDistanceBetweenLevelSlots = 240.0f;
        for (int iLevelSlotIdx = 0; iLevelSlotIdx != 16; iLevelSlotIdx++)
        {
            GameObject clonedLevelSlot = (GameObject)Instantiate(m_levelSlotPfb);
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
    }

    public void OnClickLevelSlot(int iLevelSlotIndex)
    {
        Debug.Log("OnClickLevelSlot");
    }
}