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
    private DisplayContent m_contentToDisplay;

    /**
     * Init some variables
     * **/
    public void Init()
    {
        m_displayedContent = DisplayContent.NONE;
        m_contentToDisplay = DisplayContent.NONE;
    }

    /**
     * Replace a scene with another
     * **/
    public void SwitchDisplayedContent(DisplayContent contentToDisplay, bool bShowWithAnimation = true, float fHideDelay = 0.0f, float fShowDelay = 0.0f)
    {
        m_contentToDisplay = contentToDisplay;
        HideContent(m_displayedContent, 0.5f, fHideDelay);
        ShowContent(m_contentToDisplay, bShowWithAnimation, fHideDelay + fShowDelay); //show next content 1 second after hiding the previous one

        //animate frames between scenes
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.AnimateFrames(contentToDisplay, 0.5f);
    }

    /**
     * Shows the specified scene passed as parameter
     * **/
    public void ShowContent(DisplayContent contentToDisplay, bool bAnimated = true, float fDelay = 0.0f)
    {
        if (contentToDisplay == DisplayContent.MENU)
        {
            //build the content
            GameObject clonedMainMenuScene = (GameObject)Instantiate(m_mainMenuPfb);
            clonedMainMenuScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedMainMenuScene.GetComponent<MainMenu>();
        }
        else if (contentToDisplay == DisplayContent.CHAPTERS)
        {
            //build the content
            GameObject clonedChaptersScene = (GameObject)Instantiate(m_chaptersPfb);
            clonedChaptersScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedChaptersScene.GetComponent<Chapters>();
        }
        else if (contentToDisplay == DisplayContent.LEVELS)
        {
            //build the content
            GameObject clonedLevelsScene = (GameObject)Instantiate(m_levelsPfb);
            clonedLevelsScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedLevelsScene.GetComponent<Levels>();
        }
        else if (contentToDisplay == DisplayContent.LEVEL_INTRO)
        {
            //build the content
            GameObject clonedLevelIntroScene = (GameObject)Instantiate(m_levelIntroPfb);
            clonedLevelIntroScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedLevelIntroScene.GetComponent<LevelIntro>();
        }
        else if (contentToDisplay == DisplayContent.GAME)
        {
            //build the content
            GameObject clonedGameScene = (GameObject)Instantiate(m_gameScenePfb);
            clonedGameScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedGameScene.GetComponent<GameScene>();
        }

        m_currentScene.Show(bAnimated, fDelay);
        m_displayedContent = contentToDisplay;
    }

    /**
     * Hide the specified scene passed as parameter
     * **/
    public void HideContent(DisplayContent contentToHide, float fDuration, float fDelay = 0.0f)
    {
        m_currentScene.Dismiss(fDuration, fDelay);
        m_currentScene = null;
        m_displayedContent = DisplayContent.NONE;
    }
}
