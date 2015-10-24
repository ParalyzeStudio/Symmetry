using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GUIScene m_currentScene { get; set; }

    public GameObject m_mainMenuPfb; //the prefab containing the main menu scene
    public GameObject m_chaptersPfb; //the prefab containing the chapters scene
    public GameObject m_levelsPfb; //the prefab containing the levels scene
    public GameObject m_levelIntroPfb; //the prefab containing the level intro scene
    public GameObject m_gameScenePfb; //the prefab containing the game scene

    public enum DisplayContent
    {
        NONE = 0,
        MENU,
        CHAPTERS,
        LEVELS,
        LEVEL_INTRO,
        GAME
    }

    public DisplayContent m_displayedContent { get; set; }
    private DisplayContent m_pendingContent; //the content waiting to be displayed

    /**
     * Init some variables
     * **/
    public void Init()
    {
        m_displayedContent = DisplayContent.NONE;
    }

    /**
     * Replace a scene with another
     * **/
    public void SwitchDisplayedContent(DisplayContent contentToDisplay, 
                                       bool bHideWithAnimation = true,
                                       float fHideDuration = 0.5f)
    {
        HideContent(m_displayedContent, bHideWithAnimation, fHideDuration);
        ShowContent(contentToDisplay, fHideDuration); //show next content 1 second after hiding the previous one
    }

    /**
     * Shows the specified scene passed as parameter
     * **/
    public void ShowContent(DisplayContent contentToDisplay, float fDelay = 0.0f)
    {
        m_pendingContent = contentToDisplay;

        GameObject sceneObject = null;
        if (contentToDisplay == DisplayContent.MENU)
        {
            //build the content
            sceneObject = (GameObject)Instantiate(m_mainMenuPfb);

            m_currentScene = sceneObject.GetComponent<MainMenu>();
        }
        else if (contentToDisplay == DisplayContent.CHAPTERS)
        {
            //build the content
            sceneObject = (GameObject)Instantiate(m_chaptersPfb);

            m_currentScene = sceneObject.GetComponent<Chapters>();
        }
        else if (contentToDisplay == DisplayContent.LEVELS)
        {
            //build the content
            sceneObject = (GameObject)Instantiate(m_levelsPfb);

            m_currentScene = sceneObject.GetComponent<Levels>();
        }
        else if (contentToDisplay == DisplayContent.LEVEL_INTRO)
        {
            //build the content
            sceneObject = (GameObject)Instantiate(m_levelIntroPfb);

            m_currentScene = sceneObject.GetComponent<LevelIntro>();
        }
        else if (contentToDisplay == DisplayContent.GAME)
        {
            //build the content
            sceneObject = (GameObject)Instantiate(m_gameScenePfb);

            m_currentScene = sceneObject.GetComponent<GameScene>();
        }

        if (sceneObject == null) 
            return;

        sceneObject.GetComponent<GameObjectAnimator>().SetParentTransform(this.transform);

        CallFuncHandler callFuncHandler = this.gameObject.GetComponent<CallFuncHandler>();
        callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(m_currentScene.Show), fDelay);
    }

    /**
     * Hide the specified scene passed as parameter
     * **/
    public void HideContent(DisplayContent contentToHide, bool bAnimated, float fDuration, float fDelay = 0.0f)
    {
        if (bAnimated)
            m_currentScene.Dismiss(fDuration, fDelay);
        else
        {
            Destroy(m_currentScene.gameObject);
        }

        m_currentScene = null;
        m_displayedContent = DisplayContent.NONE;
    }

    /**
     * Set the pendingContent as the new displayedContent
     * **/
    private void UpdateDisplayedContent()
    {
        m_displayedContent = m_pendingContent;
    }

    /**
     * Callback when a new scene is displayed
     * **/
    public void OnSceneShown()
    {
        UpdateDisplayedContent();
    }
}
