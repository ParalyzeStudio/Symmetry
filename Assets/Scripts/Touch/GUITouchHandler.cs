using UnityEngine;

public class GUITouchHandler : TouchHandler
{
//    public float m_circleButtonsTouchAreaRadius;
    private GameController m_gameController;

    public override void Start()
    {
        m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return true;
    }

    /**
     * Callback when user clicked the screen
     * **/
    protected override void OnClick()
    {
        Vector2 clickLocation = m_prevPointerLocation;
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        GUIScene currentScene = sceneManager.m_currentScene;

        bool bClickProcessed = false;
        GUIManager guiManager = this.gameObject.GetComponent<GUIManager>();
        if (guiManager.IsOptionsWindowShown()) //an options window is displayed, process it on the window and swallow the click
            HandleClickOnChildrenInterfaceButtons(guiManager.m_optionsWindow, clickLocation);
        else if (guiManager.IsPauseWindowShown()) //a pause window is displayed, process it on the window and swallow the click
            HandleClickOnChildrenInterfaceButtons(guiManager.m_pauseWindow, clickLocation);
        else
        {
            bClickProcessed = HandleClickOnChildrenInterfaceButtons(currentScene.gameObject, clickLocation);
            if (!bClickProcessed)
            {
                if (sceneManager.m_displayedContent == SceneManager.DisplayContent.MENU)
                {
                    HandleClickOnMainMenu(clickLocation);
                }
                else if (sceneManager.m_displayedContent == SceneManager.DisplayContent.CHAPTERS)
                {
                    HandleClickOnChapters(clickLocation);
                }
                else if (sceneManager.m_displayedContent == SceneManager.DisplayContent.LEVELS)
                {
                    HandleClickOnLevels(clickLocation);
                }
                else if (sceneManager.m_displayedContent == SceneManager.DisplayContent.GAME)
                {
                    HandleClickOnGame(clickLocation);
                }
            }
        }
    }

    /**
     * The user clicked on the tap to play area
     * **/
    public void OnClickTapToPlay()
    {
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.CHAPTERS, true, 0.0f, 1.3f, 0.5f);
    }

    /**
     * Processes click on interface buttons that are children of the root object passes as parameter
     * **/
    public bool HandleClickOnChildrenInterfaceButtons(GameObject rootObject, Vector2 clickLocation)
    {
        GUIInterfaceButton[] childButtons = rootObject.GetComponentsInChildren<GUIInterfaceButton>();
        for (int iButtonIndex = 0; iButtonIndex != childButtons.Length; iButtonIndex++)
        {
            GUIInterfaceButton button = childButtons[iButtonIndex];
            float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(button.transform.position) - clickLocation).magnitude;
            if (clickLocationToButtonDistance <= button.m_touchAreaRadius)
            {
                button.OnClick();
                return true; //swallow the touch by returning
            }
        }

        return false;
    }

    /**
     * Processes click on main menu scene
     * **/
    public void HandleClickOnMainMenu(Vector2 clickLocation)
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        //transform the click location in screen rect coordinates
        clickLocation += new Vector2(0.5f * screenSize.x, 0.5f * screenSize.y);
        clickLocation.y = -clickLocation.y + screenSize.y;

        Rect tapToPlayAreaRect = new Rect();

        tapToPlayAreaRect.width = screenSize.x;
        tapToPlayAreaRect.height = 0.25f * screenSize.y;
        tapToPlayAreaRect.position = new Vector2(0, 0.5f * screenSize.y);

        if (tapToPlayAreaRect.Contains(clickLocation))
            OnClickTapToPlay();
    }

    /**
     * Processes click on chapters scene
     * **/
    public void HandleClickOnChapters(Vector2 clickLocation)
    {
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        Chapters chapters = (Chapters)sceneManager.m_currentScene;

        GameObject[] chaptersSlots = chapters.m_chapterSlots;
        for (int iSlotIndex = 0; iSlotIndex != chaptersSlots.Length; iSlotIndex++)
        {
            Vector2 slotSize = chaptersSlots[iSlotIndex].GetComponent<GameObjectAnimator>().GetGameObjectSize();
            Vector2 slotPosition = chaptersSlots[iSlotIndex].transform.position;
            if (clickLocation.x <= slotPosition.x + 0.5f * slotSize.x && clickLocation.x >= slotPosition.x - 0.5f * slotSize.x
                &&
                clickLocation.y <= slotPosition.y + 0.5f * slotSize.y && clickLocation.y >= slotPosition.y - 0.5f * slotSize.y)
            {
                chapters.OnClickChapterSlot(iSlotIndex);
            }
        }
    }

    /**
     * Processes click on levels scene
     * **/
    public void HandleClickOnLevels(Vector2 clickLocation)
    {
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        Levels levels = (Levels)sceneManager.m_currentScene;

        GameObject[] levelsSlots = levels.m_levelSlots;
        for (int iSlotIndex = 0; iSlotIndex != levelsSlots.Length; iSlotIndex++)
        {
            Vector2 slotSize = levelsSlots[iSlotIndex].GetComponent<GameObjectAnimator>().GetGameObjectSize();
            Vector2 slotPosition = levelsSlots[iSlotIndex].transform.position;
            if (clickLocation.x <= slotPosition.x + 0.5f * slotSize.x && clickLocation.x >= slotPosition.x - 0.5f * slotSize.x
                &&
                clickLocation.y <= slotPosition.y + 0.5f * slotSize.y && clickLocation.y >= slotPosition.y - 0.5f * slotSize.y)
            {
                levels.OnClickLevelSlot(iSlotIndex);
            }
        }
    }

    /**
     * Processes click on game scene
     * **/
    public void HandleClickOnGame(Vector2 clickLocation)
    {

    }
}