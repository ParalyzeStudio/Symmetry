using UnityEngine;

/**
 * Class that handles GUI on menus only
 * **/
public class GUIManager : MonoBehaviour
{
    public GameObject m_veilPfb; //the prefab to instantiate a veil that covers whole screen
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    public GameObject m_GUIFramePfb; //the prefab to show a fram
    private GameObject m_optionsWindow; //the actual options window

    public Color[] m_framesColors;

    public enum DisplayContent
    {
        NONE = 0,
        MENU,
        CHAPTERS,
        LEVELS
    }

    private DisplayContent m_displayedContent;
    private DisplayContent m_contentToDisplay;

    public void Awake()
    {
        m_optionsWindow = null;
        m_displayedContent = DisplayContent.NONE;
        m_contentToDisplay = DisplayContent.NONE;
    }

    public void ShowContent(DisplayContent contentToDisplay, bool bAnimated = true, float fDelay = 0.0f)
    {
        GameObject contentRootNode = null;
        if (contentToDisplay == DisplayContent.MENU)
        {           
            contentRootNode = GameObject.FindGameObjectWithTag("GUIMainMenu");
            contentRootNode.GetComponent<MainMenu>().Show(bAnimated, fDelay);
        }
        else if (contentToDisplay == DisplayContent.CHAPTERS)
        {
            contentRootNode = GameObject.FindGameObjectWithTag("GUIChapters");
            contentRootNode.GetComponent<Chapters>().Show(bAnimated, fDelay);
        }

        m_displayedContent = contentToDisplay;
    }

    public void HideContent(DisplayContent contentToHide, float fDuration, bool bAnimated = true, float fDelay = 0.0f)
    {
        GameObject contentRootNode = null;
        if (contentToHide == DisplayContent.MENU)
        {
            contentRootNode = GameObject.FindGameObjectWithTag("GUIMainMenu");
            contentRootNode.GetComponent<MainMenu>().Dismiss(bAnimated, fDuration, fDelay);
        }
        else if (contentToHide == DisplayContent.CHAPTERS)
        {
            contentRootNode = GameObject.FindGameObjectWithTag("GUIChapters");
            contentRootNode.GetComponent<Chapters>().Dismiss(bAnimated, fDuration, fDelay);
        }
    }

    public void ShowOptionsWindow()
    {
        if (m_optionsWindow == null)
        {
            m_optionsWindow = (GameObject)Instantiate(m_optionsWindowPfb);
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

    public void SwitchDisplayedContent(DisplayContent contentToDisplay, float fDelay = 0.0f)
    {
        //ShowTransitionVeil(2.0f, 0.5f);
        m_contentToDisplay = contentToDisplay;
        HideContent(m_displayedContent, 0.5f, true, fDelay);
        ShowContent(m_contentToDisplay, true, fDelay + 2.0f); //show next content 1 second after hiding the previous one

        //animate frames between scenes
        AnimateFrames(contentToDisplay, 0.3f);
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

        if (contentToDisplay == DisplayContent.MENU)
        {            
            framesHolder.transform.position = new Vector3(0, 0, -5);

            Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;
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
            topFrameAnimator.ScaleFromTo(topFrameSize, new Vector3(topFrameSize.x, 185.0f, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

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
}
