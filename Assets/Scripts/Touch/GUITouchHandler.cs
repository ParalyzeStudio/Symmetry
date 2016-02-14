using UnityEngine;
using System.Collections.Generic;

public class GUITouchHandler : TouchHandler
{
    private GUIButton m_selectedGUIButton;
    private SymmetryPoint m_selectedSymmetryPoint;

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        if (GetSceneManager().m_displayedContent == SceneManager.DisplayContent.GAME)
        {
            if (GetGUIManager().m_sideButtonsOverlayDisplayed)
            {
                return true;
            }
            else
            {
                //check GUI buttons
                GUIButton[] allButtons = GetSceneManager().m_currentScene.GetComponentsInChildren<GUIButton>();

                for (int i = 0; i != allButtons.Length; i++)
                {
                    if (allButtons[i].ContainsPoint(pointerLocation))
                    {
                        m_selectedGUIButton = allButtons[i];
                        return true;
                    }
                }

                //check symmetry points
                List<SymmetryPoint> symmetryPoints = GetGUIManager().m_symmetryPoints;
                for (int i = 0; i != symmetryPoints.Count; i++)
                {
                    SymmetryPoint symmetryPoint = symmetryPoints[i];
                    if (symmetryPoint.ContainsPointer(pointerLocation))
                    {
                        m_selectedSymmetryPoint = symmetryPoint;
                        return true;
                    }
                }

                return false;
            }
        }

        return true;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, Vector2 delta)
    {
        if (m_selectedSymmetryPoint != null)
        {
            m_selectedSymmetryPoint.DetachCircle(); //detach circle if not done already

            Vector3 oldPosition = m_selectedSymmetryPoint.GetCirclePosition();
            //m_selectedSymmetryPoint.SetCirclePosition(oldPosition + GeometryUtils.BuildVector3FromVector2(delta, 0));

            Grid grid = ((GameScene)GetSceneManager().m_currentScene).m_grid;
            m_selectedSymmetryPoint.SnapToClosestGridAnchor(grid, pointerLocation);

            return true;
        }
        else
            return base.OnPointerMove(pointerLocation, delta);
    }

    protected override void OnPointerUp()
    {
        if (m_selectedSymmetryPoint != null)
        {
            m_selectedSymmetryPoint.ReleaseCircle();
            m_selectedSymmetryPoint = null;
            m_selected = false;
        }
        else
            base.OnPointerUp();
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
        BackgroundTriangleAnimator animator = GetBackgroundRenderer().GetComponent<BackgroundTriangleAnimator>();
        
        //Translate of a even number of triangles
        int numTrianglesPerColumn = GetBackgroundRenderer().m_numTrianglesPerColumn;
        int translationLengthInTriangleUnits = Mathf.RoundToInt(0.3f * numTrianglesPerColumn);
        if (translationLengthInTriangleUnits % 2 == 1) //make it even
            translationLengthInTriangleUnits += 1;
        float triangleEdgeLength = GetBackgroundRenderer().m_triangleEdgeLength;
        animator.TranslateTo(new Vector3(0, translationLengthInTriangleUnits * triangleEdgeLength, 0), 0.1f, 5.0f);

        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.CHAPTERS, true, 5.1f);
    }

    /**
     * The user clicked on the skip button
     * **/
    public void OnClickSkipIntro()
    {
        ((LevelIntro)GetSceneManager().m_currentScene).LaunchGameScene();
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
        MainMenu mainMenu = (MainMenu) GetSceneManager().m_currentScene;

        //transform the click location in screen rect coordinates
        //clickLocation += new Vector2(0.5f * screenSize.x, 0.5f * screenSize.y);
        //clickLocation.y = -clickLocation.y + screenSize.y;

        GameObject playButtonObject = mainMenu.m_playButtonObject; 
        Vector2 playButtonPosition = playButtonObject.transform.position;
        float playButtonTouchArea = playButtonObject.GetComponent<PulsatingButton>().Size;

        if (clickLocation.x >= playButtonPosition.x - 0.5f * playButtonTouchArea && clickLocation.x <= playButtonPosition.x + 0.5f * playButtonTouchArea
            &&
            clickLocation.y <= playButtonPosition.y + 0.5f * playButtonTouchArea && clickLocation.y >= playButtonPosition.y - 0.5f * playButtonTouchArea)
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
            BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundTrianglesRenderer>();
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
        LevelIntro levelintro = (LevelIntro)GetSceneManager().m_currentScene;
        PulsatingButton skipButton = levelintro.m_skipButton;
        Vector2 skipButtonPosition = skipButton.gameObject.transform.position;
        float skipButtonTouchArea = skipButton.Size;

        if (clickLocation.x >= skipButtonPosition.x - 0.5f * skipButtonTouchArea && clickLocation.x <= skipButtonPosition.x + 0.5f * skipButtonTouchArea
            &&
            clickLocation.y <= skipButtonPosition.y + 0.5f * skipButtonTouchArea && clickLocation.y >= skipButtonPosition.y - 0.5f * skipButtonTouchArea)
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