using UnityEngine;

public class GUITouchHandler : TouchHandler
{
    public float m_circleButtonsTouchAreaRadius;
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

        if (!HandleClickOnGlobalGUI(clickLocation))
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

    /**
     * The user clicked on the tap to play area
     * **/
    public void OnClickTapToPlay()
    {
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.CHAPTERS, true, 0.0f, 1.3f, 0.5f);
    }

    /**
     * Processes click on gui elements that repeat over scenes (like back button)
     * Returns true if click has been processed so further handlers (that process click on scenes) are not taken into account
     * **/
    public bool HandleClickOnGlobalGUI(Vector2 clickLocation)
    {
        GUIInterfaceButton backBtn = GUIInterfaceButton.FindInObjectChildrenForID(this.gameObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_BACK_BUTTON);
        if (backBtn != null)
        {
            float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(backBtn.gameObject.transform.position) - clickLocation).magnitude;
            if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
            {
                backBtn.OnClick();
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
        GUIManager guiManager = this.gameObject.GetComponent<GUIManager>();
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        if (guiManager.IsOptionsWindowShown())
        {
            GUIInterfaceButton musicBtn = GUIInterfaceButton.FindInObjectChildrenForID(guiManager.m_optionsWindow, GUIInterfaceButton.GUIInterfaceButtonID.ID_MUSIC_BUTTON);
            if (musicBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(musicBtn.gameObject.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    musicBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }

            GUIInterfaceButton soundBtn = GUIInterfaceButton.FindInObjectChildrenForID(guiManager.m_optionsWindow, GUIInterfaceButton.GUIInterfaceButtonID.ID_SOUND_BUTTON);
            if (soundBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(soundBtn.gameObject.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    soundBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }

            GUIInterfaceButton closeBtn = GUIInterfaceButton.FindInObjectChildrenForID(guiManager.m_optionsWindow, GUIInterfaceButton.GUIInterfaceButtonID.ID_CLOSE_BUTTON);
            if (closeBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(closeBtn.gameObject.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    closeBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }
        }
        else
        {
            ////Check if we clicked a button first to swallow touch
            //Options Button
            GameObject mainMenuObject = sceneManager.m_currentScene.gameObject;
            GUIInterfaceButton optionsBtn = GUIInterfaceButton.FindInObjectChildrenForID(mainMenuObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_OPTIONS_BUTTON);
            if (optionsBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(optionsBtn.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    optionsBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }

            //Credits Button
            GUIInterfaceButton creditsBtn = GUIInterfaceButton.FindInObjectChildrenForID(mainMenuObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_CREDITS_BUTTON);
            if (creditsBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(creditsBtn.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    creditsBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }

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
    }

    /**
     * Processes click on chapters scene
     * **/
    public void HandleClickOnChapters(Vector2 clickLocation)
    {        
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        Chapters chapters = (Chapters) sceneManager.m_currentScene;

        GUIInterfaceButton backBtn = GUIInterfaceButton.FindInObjectChildrenForID(this.gameObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_BACK_BUTTON);
        if (backBtn != null)
        {
            float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(backBtn.transform.position) - clickLocation).magnitude;
            if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
            {
                backBtn.OnClick();
                return; //swallow the touch by returning
            }
        }

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
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        GameScene game = (GameScene)sceneManager.m_currentScene;

        GUIInterfaceButton pauseBtn = GUIInterfaceButton.FindInObjectChildrenForID(game.gameObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_PAUSE_BUTTON);
        if (pauseBtn != null)
        {
            float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(pauseBtn.transform.position) - clickLocation).magnitude;
            if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
            {
                pauseBtn.OnClick();
                return; //swallow the touch by returning
            }
        }
    }
}