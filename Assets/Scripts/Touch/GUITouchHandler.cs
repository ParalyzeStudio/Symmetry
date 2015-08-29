using UnityEngine;

public class GUITouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.GAME)
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

        return true;
    }

    /**
     * Callback when user clicked the screen
     * **/
    protected override void OnClick(Vector2 clickLocation)
    {
        bool bClickProcessed = false;
        if (GetGUIManager().m_sideButtonsOverlayDisplayed) //side buttons overlay is shown
        {
            if (!HandleClickOnChildGUIButtons(GetGUIManager().m_sideButtonsOverlay, clickLocation))
            {
                //check if click is performed on one of the side buttons
                GUIButton.GUIButtonID[] sideButtonsIDs = new GUIButton.GUIButtonID[2];
                sideButtonsIDs[0] = GUIButton.GUIButtonID.ID_OPTIONS_BUTTON;
                sideButtonsIDs[1] = GUIButton.GUIButtonID.ID_CREDITS_BUTTON;
                HandleClickOnChildGUIButtons(this.gameObject, clickLocation, sideButtonsIDs);
            }
        }
        else if (false) //TODO check if pause overlay is displayed
            //HandleClickOnChildGUIButtons(guiManager.m_pauseWindow, clickLocation);
            ;
        else
        {
            bClickProcessed = HandleClickOnChildGUIButtons(this.gameObject, clickLocation);
            if (!bClickProcessed)
                bClickProcessed = HandleClickOnChildGUIButtons(GetSceneManager().m_currentScene.gameObject, clickLocation);
            if (!bClickProcessed)
            {
                if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.MENU)
                {
                    HandleClickOnMainMenu(clickLocation);
                }
                else if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.CHAPTERS)
                {
                    HandleClickOnChapters(clickLocation);
                }
                else if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.LEVELS)
                {
                    HandleClickOnLevels(clickLocation);
                }
                else if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.LEVEL_INTRO)
                {
                    HandleClickOnLevelIntro(clickLocation);
                }
                else if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.GAME)
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
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        BackgroundTriangleAnimator animator = GetBackgroundRenderer().GetComponent<BackgroundTriangleAnimator>();
        
        //Translate of a even number of triangles
        int numTrianglesPerColumn = GetBackgroundRenderer().m_numTrianglesPerColumn;
        int translationLengthInTriangleUnits = Mathf.RoundToInt(0.6f * numTrianglesPerColumn);
        if (translationLengthInTriangleUnits % 2 == 1) //make it even
            translationLengthInTriangleUnits += 1;
        float triangleEdgeLength = GetBackgroundRenderer().m_triangleEdgeLength;
        animator.TranslateTo(new Vector3(0, translationLengthInTriangleUnits * triangleEdgeLength, 0), 0.1f, 5.0f);

        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.CHAPTERS, true, 5.0f);
    }

    /**
     * The user clicked on the skip button
     * **/
    public void OnClickSkipIntro()
    {
        //GUIScene levelIntroScene = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;
        //SceneAnimator sceneAnimator = levelIntroScene.gameObject.GetComponent<SceneAnimator>();
        //sceneAnimator.FadeTo(0, 0.7f);
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 1.4f);
    }

    /**
     * Processes click on interface buttons that are children of the root object passes as parameter
     * **/
    public bool HandleClickOnChildGUIButtons(GameObject rootObject, Vector2 clickLocation, GUIButton.GUIButtonID[] m_filteredButtonIDs = null)
    {
        GUIButton[] childButtons = rootObject.GetComponentsInChildren<GUIButton>();
        for (int iButtonIndex = 0; iButtonIndex != childButtons.Length; iButtonIndex++)
        {
            GUIButton button = childButtons[iButtonIndex];

            if (m_filteredButtonIDs != null) //check if this button belongs to the filtered buttons IDs list
            {
                bool filteredButtonID = false;
                for (int i = 0; i != m_filteredButtonIDs.Length; i++)
                {
                    if (button.m_ID == m_filteredButtonIDs[i])
                    {
                        filteredButtonID = true;
                        break;
                    }
                }

                if (!filteredButtonID)
                    continue;
            }

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

        MainMenu mainMenu = (MainMenu) GetSceneManager().m_currentScene;

        //transform the click location in screen rect coordinates
        //clickLocation += new Vector2(0.5f * screenSize.x, 0.5f * screenSize.y);
        //clickLocation.y = -clickLocation.y + screenSize.y;

        GameObject playButtonObject = mainMenu.m_playButton; 
        Vector2 playButtonPosition = playButtonObject.transform.localPosition;
        float playButtonOuterRadius = playButtonObject.GetComponent<CircleMeshAnimator>().m_outerRadius;

        if ((clickLocation - playButtonPosition).magnitude <= playButtonOuterRadius)
        {
            mainMenu.DismissPlayButton();
            OnClickPlay();
        }
    }

    /**
     * Processes click on chapters scene
     * **/
    public void HandleClickOnChapters(Vector2 clickLocation)
    {     
        Chapters chapters = (Chapters)GetSceneManager().m_currentScene;

        if (!HandleClickOnChildGUIButtons(chapters.gameObject, clickLocation))
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
        Levels levels = (Levels)GetSceneManager().m_currentScene;

        float triangleHeight = GetBackgroundRenderer().m_triangleHeight;
        if (!HandleClickOnChildGUIButtons(levels.gameObject, clickLocation))
        {
            for (int i = 0; i != levels.m_levelSlots.Length; i++)
            {
                Vector3 slotPosition = levels.m_levelSlots[i].transform.position;

                if (clickLocation.x >= slotPosition.x - triangleHeight && clickLocation.x <= slotPosition.x + triangleHeight &&
                   clickLocation.y >= slotPosition.y - triangleHeight && clickLocation.y <= slotPosition.y + triangleHeight)
                {
                    levels.OnClickLevelSlot(i);
                }
            }

            //for (int i = 0; i != levels.m_levelSlots.Length; i++)
            //{
            //    if (levels.m_levelSlots[i].ContainsPoint(clickLocation))
            //    {
            //        levels.OnClickLevelSlot(i);
            //        break;
            //    }
            //}
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