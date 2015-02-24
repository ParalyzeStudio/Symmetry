using UnityEngine;

/**
 * Class that handles GUI on menus only
 * **/
public class GUIManager : MonoBehaviour
{
    public GameObject m_veilPfb; //the prefab to instantiate a veil that covers whole screen
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    private GameObject m_optionsWindow; //the actual options window

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

    public void ShowContent(DisplayContent contentToDisplay, bool bAnimated)
    {
        GameObject contentRootNode = null;
        if (contentToDisplay == DisplayContent.MENU)
        {           
            contentRootNode = GameObject.FindGameObjectWithTag("GUIMainMenu");
            contentRootNode.GetComponent<MainMenu>().Show(bAnimated);
        }
        else if (contentToDisplay == DisplayContent.CHAPTERS)
        {
            contentRootNode = GameObject.FindGameObjectWithTag("GUIChapters");
            contentRootNode.GetComponent<Chapters>().Show(bAnimated);
        }

        m_displayedContent = contentToDisplay;
    }

    public void HideContent(DisplayContent contentToHide)
    {
        GameObject contentRootNode = null;
        if (contentToHide == DisplayContent.MENU)
        {
            contentRootNode = GameObject.FindGameObjectWithTag("GUIMainMenu");
            contentRootNode.GetComponent<MainMenu>().Dismiss();
        }
        else if (contentToHide == DisplayContent.CHAPTERS)
        {
            contentRootNode = GameObject.FindGameObjectWithTag("GUIChapters");
            contentRootNode.GetComponent<Chapters>().Dismiss();
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

    public void SwitchDisplayedContent(DisplayContent contentToDisplay)
    {
        ShowTransitionVeil(2.0f, 0.5f);
        m_contentToDisplay = contentToDisplay;
    }

    public void ShowTransitionVeil(float fPhaseDuration, float fPeakDuration, float fDelay = 0.0f)
    {
        GameObject veil = (GameObject)Instantiate(m_veilPfb);
        veil.GetComponent<VeilOpacityAnimator>().TransitionOverScenes(fPhaseDuration, fPeakDuration, fDelay);
    }

    public void OnTransitionVeilPeakReached()
    {
        HideContent(m_displayedContent);
        ShowContent(m_contentToDisplay, true);
        m_displayedContent = m_contentToDisplay;
    }
}
