using UnityEngine;

public class GUIManager : MonoBehaviour
{
    public GameObject m_veilPfb; //the prefab to instantiate a veil that covers whole screen
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    private GameObject m_optionsWindow; //the actual options window

    public void Awake()
    {
        m_optionsWindow = null;
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

    public void ShowTransitionVeil(float fPhaseDuration, float fPeakDuration, float fDelay = 0.0f)
    {
        GameObject veil = (GameObject)Instantiate(m_veilPfb);
        veil.GetComponent<VeilOpacityAnimator>().TransitionOverScenes(fPhaseDuration, fPeakDuration);
    }
}
