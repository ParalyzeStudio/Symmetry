using UnityEngine;

/**
 * Class that handles GUI on menus only
 * **/
public class GUIManager : MonoBehaviour
{
    //public MainMenu m_mainMenuScene { get; set; }
    //public Chapters m_chaptersScene { get; set; }
    //public Chapters m_levelsScene { get; set; }

    public GUIScene m_currentScene { get; set; }

    public GameObject m_mainMenuPfb; //the prefab containing all information on main menu
    public GameObject m_chaptersPfb; //the prefab containing all information on chapters
    public GameObject m_levelsPfb; //the prefab containing all information on chapters
    public GameObject m_veilPfb; //the prefab to instantiate a veil that covers whole screen
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    public GameObject m_GUIFramePfb; //the prefab containing a background frame
    public GameObject m_backButtonPfb; //the prefab for the back button
    public GameObject m_optionsWindow { get; set; } //the actual options window

    public Color[] m_framesColors;

    public int m_currentChapter { get; set; }

    public enum DisplayContent
    {
        NONE = 0,
        MENU,
        CHAPTERS,
        LEVELS
    }

    public DisplayContent m_displayedContent { get; set; }
    private DisplayContent m_contentToDisplay;

    public void Init()
    {
        m_optionsWindow = null;
        m_displayedContent = DisplayContent.NONE;
        m_contentToDisplay = DisplayContent.NONE;
        BuildFrames();
    }

    public void ShowContent(DisplayContent contentToDisplay, bool bAnimated = true, float fDelay = 0.0f)
    {
        if (contentToDisplay == DisplayContent.MENU)
        {
            //build the content
            GameObject clonedMainMenuScene = (GameObject) Instantiate(m_mainMenuPfb);
            clonedMainMenuScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedMainMenuScene.GetComponent<MainMenu>();
            ////show it
            //m_mainMenuScene = clonedMainMenuScene.GetComponent<MainMenu>();
            //m_mainMenuScene.Show(bAnimated, fDelay);
        }
        else if (contentToDisplay == DisplayContent.CHAPTERS)
        {
            //build the content
            GameObject clonedChaptersScene = (GameObject)Instantiate(m_chaptersPfb);
            clonedChaptersScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedChaptersScene.GetComponent<Chapters>();

            ////show it
            //m_chaptersScene = clonedChaptersScene.GetComponent<Chapters>();
            //m_chaptersScene.Show(bAnimated, fDelay);
        }
        else if (contentToDisplay == DisplayContent.LEVELS)
        {
            //build the content
            GameObject clonedLevelsScene = (GameObject)Instantiate(m_levelsPfb);
            clonedLevelsScene.transform.parent = this.gameObject.transform;

            m_currentScene = clonedLevelsScene.GetComponent<Levels>();
        }

        m_currentScene.Show(bAnimated, fDelay);
        m_displayedContent = contentToDisplay;
    }

    public void HideContent(DisplayContent contentToHide, float fDuration, float fDelay = 0.0f)
    {
        //if (contentToHide == DisplayContent.MENU)
        //{
        //    m_mainMenuScene.Dismiss(fDuration, fDelay);
        //    m_mainMenuScene = null;
        //}
        //else if (contentToHide == DisplayContent.CHAPTERS)
        //{
        //    m_chaptersScene.Dismiss(fDuration, fDelay);
        //    m_chaptersScene = null;
        //}
        //else if (contentToHide == DisplayContent.LEVELS)
        //{
        //    m_levelsScene.Dismiss(fDuration, fDelay);
        //    m_levelsScene = null;
        //}

        m_currentScene.Dismiss(fDuration, fDelay);
        m_currentScene = null;
        m_displayedContent = DisplayContent.NONE;
    }

    public void ShowOptionsWindow()
    {
        if (m_optionsWindow == null)
        {
            m_optionsWindow = (GameObject)Instantiate(m_optionsWindowPfb);
            m_optionsWindow.transform.parent = m_currentScene.gameObject.transform;
        }
    }

    public void DismissOptionsWindow()
    {
        if (m_optionsWindow != null)
        {
            Destroy(m_optionsWindow);
            m_optionsWindow = null;
        }
    }

    public bool IsOptionsWindowShown()
    {
        return m_optionsWindow != null;
    }

    public void SwitchDisplayedContent(DisplayContent contentToDisplay, bool bShowWithAnimation = true, float fHideDelay = 0.0f, float fShowDelay = 0.0f)
    {
        m_contentToDisplay = contentToDisplay;
        HideContent(m_displayedContent, 0.5f, fHideDelay);
        ShowContent(m_contentToDisplay, bShowWithAnimation, fHideDelay + fShowDelay); //show next content 1 second after hiding the previous one

        //animate frames between scenes
        AnimateFrames(contentToDisplay, 0.5f);
    }

    /**
     * Build frames that will be animated through scenes
     * **/
    public void BuildFrames()
    {
        GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
        GameObject topFrame = (GameObject)Instantiate(m_GUIFramePfb);
        GameObject middleFrame = (GameObject)Instantiate(m_GUIFramePfb);

        topFrame.transform.parent = framesHolder.transform;
        middleFrame.transform.parent = framesHolder.transform;

        //set the color
        topFrame.GetComponent<TintColorMaterialAssignment>().m_tintColor = m_framesColors[0];
        middleFrame.GetComponent<TintColorMaterialAssignment>().m_tintColor = m_framesColors[1];

        //set the height of those frames to zero
        GameObjectAnimator topFrameAnimator = topFrame.GetComponent<GameObjectAnimator>();
        GameObjectAnimator middleFrameAnimator = middleFrame.GetComponent<GameObjectAnimator>();
        topFrameAnimator.OnScaleChanged(Vector3.zero);
        middleFrameAnimator.OnScaleChanged(Vector3.zero);
    }

    /**
     * Animate frames for the specified menu scene
     * **/
    public void AnimateFrames(DisplayContent contentToDisplay, float fDelay = 0.0f)
    {
        GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
        GameObjectAnimator[] framesAnimators = framesHolder.GetComponentsInChildren<GameObjectAnimator>();

        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;
        if (contentToDisplay == DisplayContent.MENU)
        {
            framesHolder.transform.position = new Vector3(0, 0, -5);
           
            GameObject axesHolder = GameObject.FindGameObjectWithTag("MainMenuAxes");
            float distanceToScreenTopBorder = 0.5f * screenSize.y - axesHolder.transform.position.y;
            float topFrameHeight = 2 * distanceToScreenTopBorder;

            GameObjectAnimator topFrameAnimator = framesAnimators[0];
            topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            topFrameAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0, 0.5f * screenSize.y, -1));
            Vector3 fromScale = new Vector3(screenSize.x, 0, 1);
            Vector3 toScale = new Vector3(screenSize.x, topFrameHeight, 1);
            topFrameAnimator.OnScaleChanged(new Vector3(screenSize.x, 0, 1));
            topFrameAnimator.ScaleFromTo(fromScale, toScale, 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

            float middleFrameHeight = topFrameHeight + 0.23f * screenSize.y;
            GameObjectAnimator middleFrameAnimator = framesAnimators[1];
            middleFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            middleFrameAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0, 0.5f * screenSize.y, 0));
            fromScale = new Vector3(screenSize.x, 0, 1);
            toScale = new Vector3(screenSize.x, middleFrameHeight, 1);
            middleFrameAnimator.OnScaleChanged(new Vector3(screenSize.x, 0, 1));
            middleFrameAnimator.ScaleFromTo(fromScale, toScale, 1.2f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else if (contentToDisplay == DisplayContent.CHAPTERS || contentToDisplay == DisplayContent.LEVELS)
        {
            //animate top frame
            GameObjectAnimator topFrameAnimator = framesAnimators[0];
            topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            Vector3 topFrameSize = topFrameAnimator.GetGameObjectSize();
            topFrameAnimator.ScaleFromTo(topFrameSize, new Vector3(topFrameSize.x, 0.144f * screenSize.y, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

            //animate middle frame
            GameObjectAnimator middleFrameAnimator = framesAnimators[1];
            middleFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            Vector3 middleFrameSize = middleFrameAnimator.GetGameObjectSize();
            middleFrameAnimator.ScaleFromTo(middleFrameSize, new Vector3(middleFrameSize.x, 0, middleFrameSize.z), 0.6f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
    }

    public void ShowTransitionVeil(float fPhaseDuration, float fPeakDuration, float fDelay = 0.0f)
    {
        GameObject veil = (GameObject)Instantiate(m_veilPfb);
        veil.GetComponent<VeilOpacityAnimator>().TransitionOverScenes(fPhaseDuration, fPeakDuration, fDelay);
    }

    public void OnTransitionVeilPeakReached()
    {        
        ShowContent(m_contentToDisplay, true);
        m_displayedContent = m_contentToDisplay;
    }

    /**
     * Shows the back button that is shared across scenes
     * If it's already shown do nothing
     * **/
    public void ShowBackButton(float fDelay = 0.0f)
    {
        GUIInterfaceButton backButton = GUIInterfaceButton.FindInObjectChildrenForID(this.gameObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_BACK_BUTTON);
        if (backButton == null)
        {
            Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

            GameObject clonedBackButtonObject = (GameObject)Instantiate(m_backButtonPfb);
            clonedBackButtonObject.transform.parent = this.gameObject.transform;
            clonedBackButtonObject.transform.localPosition = new Vector3(-0.5f * screenSize.x + 110.0f, 0.5f * screenSize.y - 90.0f, -20.0f);

            backButton = clonedBackButtonObject.GetComponent<GUIInterfaceButton>();
            GameObjectAnimator showButtonAnimator = backButton.GetComponent<GameObjectAnimator>();
            showButtonAnimator.OnOpacityChanged(0);
            showButtonAnimator.FadeFromTo(0, 1, 0.5f, fDelay);
        }
    }

    public void DismissBackButton()
    {
        GUIInterfaceButton backButton = GUIInterfaceButton.FindInObjectChildrenForID(this.gameObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_BACK_BUTTON);
        Destroy(backButton.gameObject);
    }
}