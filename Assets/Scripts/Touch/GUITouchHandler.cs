using UnityEngine;

public class GUITouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.GAME)
        {
            GUIManager guiManager = this.gameObject.GetComponent<GUIManager>();
            if (guiManager.IsPauseWindowShown())
            {
                return true;
            }
            else
            {

                GUIButton[] allButtons = GetSceneManager().m_currentScene.GetComponentsInChildren<GUIButton>();

                for (int i = 0; i != allButtons.Length; i++)
                {
                    if (allButtons[i].ContainsPoint(pointerLocation))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        return true;
    }

    /**
     * Callback when user clicked the screen
     * **/
    protected override void OnClick(Vector2 clickLocation)
    {
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        GUIScene currentScene = sceneManager.m_currentScene;

        bool bClickProcessed = false;
        GUIManager guiManager = this.gameObject.GetComponent<GUIManager>();
        if (guiManager.IsOptionsWindowShown()) //an options window is displayed, process it on the window and swallow the click
            HandleClickOnChildButtons(guiManager.m_optionsWindow, clickLocation);
        else if (guiManager.IsPauseWindowShown()) //a pause window is displayed, process it on the window and swallow the click
            HandleClickOnChildButtons(guiManager.m_pauseWindow, clickLocation);
        else
        {
            bClickProcessed = HandleClickOnChildButtons(this.gameObject, clickLocation);
            if (!bClickProcessed)
                bClickProcessed = HandleClickOnChildButtons(currentScene.gameObject, clickLocation);
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
                else if (sceneManager.m_displayedContent == SceneManager.DisplayContent.LEVEL_INTRO)
                {
                    HandleClickOnLevelIntro(clickLocation);
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
    public void OnClickPlay()
    {
        BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();
        bgRenderer.m_transitionGradientLength = ScreenUtils.GetScreenSize().y;
        bgRenderer.GenerateChapterGradient();
        bgRenderer.GenerateTransitionGradients();
        bgRenderer.SwitchBetweenMainMenuBackgroundAndChapterBackground(true, 5.0f);

        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.CHAPTERS, true, 0.0f, 5.0f);
    }

    /**
     * The user clicked on the skip button
     * **/
    public void OnClickSkipIntro()
    {
        //GUIScene levelIntroScene = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        //SceneAnimator sceneAnimator = levelIntroScene.gameObject.GetComponent<SceneAnimator>();
        //sceneAnimator.FadeTo(0, 0.7f);
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 0.0f, 1.4f, 0.7f);
    }

    /**
     * Processes click on interface buttons that are children of the root object passes as parameter
     * **/
    public bool HandleClickOnChildButtons(GameObject rootObject, Vector2 clickLocation)
    {
        GUIButton[] childButtons = rootObject.GetComponentsInChildren<GUIButton>();
        for (int iButtonIndex = 0; iButtonIndex != childButtons.Length; iButtonIndex++)
        {
            GUIButton button = childButtons[iButtonIndex];
            if (button.ContainsPoint(clickLocation))
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
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        MainMenu mainMenu = (MainMenu) GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;

        //transform the click location in screen rect coordinates
        clickLocation += new Vector2(0.5f * screenSize.x, 0.5f * screenSize.y);
        clickLocation.y = -clickLocation.y + screenSize.y;

        //if (mainMenu.m_isPlayBannerDisplayed)
        //{
        //    Rect tapToPlayAreaRect = new Rect();

        //    tapToPlayAreaRect.width = screenSize.x;
        //    tapToPlayAreaRect.height = 175.0f;
        //    tapToPlayAreaRect.position = new Vector2(0, 0.5f * screenSize.y + 152.5f);

        //    if (tapToPlayAreaRect.Contains(clickLocation))
        //        OnClickPlay();
        //}
    }

    /**
     * Processes click on chapters scene
     * **/
    public void HandleClickOnChapters(Vector2 clickLocation)
    {     
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        Chapters chapters = (Chapters)sceneManager.m_currentScene;

        if (!HandleClickOnChildButtons(chapters.gameObject, clickLocation))
        {
            BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();
            float triangleHeight = bgRenderer.m_triangleHeight;

            if (clickLocation.x >= -4 * triangleHeight && clickLocation.x <= 4 * triangleHeight &&
                clickLocation.y >= -4 * triangleHeight && clickLocation.y <= 4 * triangleHeight)
            {
                chapters.OnClickChapterSlot();
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

        if (!HandleClickOnChildButtons(levels.gameObject, clickLocation))
        {
            for (int i = 0; i != levels.m_levelSlots.Length; i++)
            {
                if (levels.m_levelSlots[i].ContainsPoint(clickLocation))
                {
                    levels.OnClickLevelSlot(i);
                    break;
                }
            }
        }
    }

    /**
     * Processes click on levels scene
     * **/
    public void HandleClickOnLevelIntro(Vector2 clickLocation)
    {
        float skipAreaWidth = 800;
        float skipAreaHeight = 200;
        Vector2 skipAreaPosition = new Vector2(0, -130.0f); //position of the center of the skip text

        if (clickLocation.x >= skipAreaPosition.x - 0.5f * skipAreaWidth && clickLocation.x <= skipAreaPosition.x + 0.5f * skipAreaWidth
            &&
            clickLocation.y <= skipAreaPosition.y + 0.5f * skipAreaHeight && clickLocation.y >= skipAreaPosition.y - 0.5f * skipAreaHeight)
        {
            OnClickSkipIntro();
        }         
    }

    /**
     * Processes click on game scene
     * **/
    public void HandleClickOnGame(Vector2 clickLocation)
    {

    }
}