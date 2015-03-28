using UnityEngine;

/**
 * Class that handles GUI on menus only
 * **/
public class GUIManager : MonoBehaviour
{
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    public GameObject m_pauseWindowPfb; //the prefab needed to instantiate the pause window
    public GameObject m_GUIFramePfb; //the prefab containing a background frame
    public GameObject m_backButtonPfb; //the prefab for the back button
    public GameObject m_optionsWindow { get; set; } //the actual options window
    public GameObject m_pauseWindow { get; set; } //the pause menu we can access from game

    public Color[] m_framesColors;

    /**
     * Init some variables
     * **/
    public void Init()
    {
        m_optionsWindow = null;
        BuildFrames();
    }

    /**
     * Shows the options window on top of the current scene
     * **/
    public void ShowOptionsWindow()
    {
        if (m_optionsWindow == null)
        {
            m_optionsWindow = (GameObject)Instantiate(m_optionsWindowPfb);
            m_optionsWindow.transform.parent = this.gameObject.transform;
        }
    }

    /**
     * Dismisses the options window
     * **/
    public void DismissOptionsWindow()
    {
        if (m_optionsWindow != null)
        {
            Destroy(m_optionsWindow);
            m_optionsWindow = null;
        }
    }

    /**
     * Shows the pause window on top of the current scene
     * **/
    public void ShowPauseWindow()
    {
        if (m_pauseWindow == null)
        {
            m_pauseWindow = (GameObject)Instantiate(m_pauseWindowPfb);
            m_pauseWindow.transform.parent = this.gameObject.transform;
        }
    }

    /**
     * Dismisses the pause window
     * **/
    public void DismissPauseWindow()
    {
        if (m_pauseWindow != null)
        {
            Destroy(m_pauseWindow);
            m_pauseWindow = null;
        }
    }

    /**
     * Tells if the options window is displayed
     * **/
    public bool IsOptionsWindowShown()
    {
        return m_optionsWindow != null;
    }

    /**
     * Tells if the pause window is displayed
     * **/
    public bool IsPauseWindowShown()
    {
        return m_pauseWindow != null;
    }

    /**
     * Build frames that will be animated through scenes
     * **/
    public void BuildFrames()
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

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
        topFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
        middleFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
    }

    /**
     * Animate frames for the specified menu scene
     * **/
    public void AnimateFrames(SceneManager.DisplayContent contentToDisplay, float fDelay = 0.0f)
    {
        GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
        GameObjectAnimator[] framesAnimators = framesHolder.GetComponentsInChildren<GameObjectAnimator>();

        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        if (contentToDisplay == SceneManager.DisplayContent.MENU)
        {
            framesHolder.transform.position = new Vector3(0, 0, -5);
           
            GameObject axesHolder = GameObject.FindGameObjectWithTag("MainMenuAxes");
            float distanceToScreenTopBorder = 0.5f * screenSize.y - axesHolder.transform.position.y;
            float topFrameHeight = 2 * distanceToScreenTopBorder;

            GameObjectAnimator topFrameAnimator = framesAnimators[0];
            topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
            Vector3 fromScale = new Vector3(screenSize.x, 0, 1);
            Vector3 toScale = new Vector3(screenSize.x, topFrameHeight, 1);
            topFrameAnimator.SetScale(fromScale);
            topFrameAnimator.ScaleTo(toScale, 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

            float middleFrameHeight = topFrameHeight + 0.23f * screenSize.y;
            GameObjectAnimator middleFrameAnimator = framesAnimators[1];
            middleFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            middleFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, 0));
            fromScale = new Vector3(screenSize.x, 0, 1);
            toScale = new Vector3(screenSize.x, middleFrameHeight, 1);
            middleFrameAnimator.SetScale(fromScale);
            middleFrameAnimator.ScaleTo(toScale, 1.2f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else if (contentToDisplay == SceneManager.DisplayContent.CHAPTERS || contentToDisplay == SceneManager.DisplayContent.LEVELS)
        {
            //animate top frame
            GameObjectAnimator topFrameAnimator = framesAnimators[0];
            topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
            Vector3 topFrameSize = topFrameAnimator.GetGameObjectSize();
            topFrameAnimator.ScaleTo(new Vector3(screenSize.x, 0.144f * screenSize.y, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

            //animate middle frame
            GameObjectAnimator middleFrameAnimator = framesAnimators[1];
            middleFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            middleFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
            Vector3 middleFrameSize = middleFrameAnimator.GetGameObjectSize();
            middleFrameAnimator.ScaleTo(new Vector3(screenSize.x, 0, middleFrameSize.z), 0.6f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else if (contentToDisplay == SceneManager.DisplayContent.LEVEL_INTRO)
        {
            //animate middle frame
            GameObjectAnimator topFrameAnimator = framesAnimators[0];
            topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
            Vector3 topFrameSize = topFrameAnimator.GetGameObjectSize();
            topFrameAnimator.ScaleTo(new Vector3(screenSize.x, screenSize.y, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else if (contentToDisplay == SceneManager.DisplayContent.GAME)
        {
            //animate top frame
            GameObjectAnimator topFrameAnimator = framesAnimators[0];
            topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
            Vector3 topFrameSize = topFrameAnimator.GetGameObjectSize();
            topFrameAnimator.ScaleTo(new Vector3(screenSize.x, 0.144f * screenSize.y, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
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
            showButtonAnimator.SetOpacity(0);
            showButtonAnimator.FadeTo(1, 0.5f, fDelay);
        }
    }

    /**
     * Dismisses the back button
     * **/
    public void DismissBackButton()
    {
        GUIInterfaceButton backButton = GUIInterfaceButton.FindInObjectChildrenForID(this.gameObject, GUIInterfaceButton.GUIInterfaceButtonID.ID_BACK_BUTTON);
        if (backButton != null)
            Destroy(backButton.gameObject);
    }
}