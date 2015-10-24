using UnityEngine;

public class GUIScene : MonoBehaviour
{
    //Global instances to prevent calls to FindGameObjectWithTag and GetComponent<>
    protected GUIManager m_guiManager;
    protected BackgroundTrianglesRenderer m_backgroundRenderer;
    protected LevelManager m_levelManager;
    protected SceneManager m_sceneManager;
    protected PersistentDataManager m_persistentDataManager;
    protected CallFuncHandler m_callFuncHandler;

    /**
     * Shows this scene
     * **/
    //public virtual void Show(bool bAnimated, float fDelay = 0.0f)
    //{
        
    //}

    public virtual void Show()
    {
        GetSceneManager().OnSceneShown();
    }

    /**
     * Dismisses this scene
     * **/
    public void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(OnSceneDismissed), fDuration + fDelay);

        DismissSelf();
    }

    protected virtual void DismissSelf()
    {
        //fade out the scene
        SceneAnimator sceneAnimator = this.gameObject.GetComponent<SceneAnimator>();
        sceneAnimator.FadeTo(0, 1.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, false);
    }

    public virtual void OnSceneDismissed()
    {
        Destroy(this.gameObject); //Destroy the scene object
    }

    /**
     * Getters for global instances
     * **/
    public GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

        return m_guiManager;
    }

    public BackgroundTrianglesRenderer GetBackgroundRenderer()
    {
        if (m_backgroundRenderer == null)
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundTrianglesRenderer>();

        return m_backgroundRenderer;
    }

    public LevelManager GetLevelManager()
    {
        if (m_levelManager == null)
            m_levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

        return m_levelManager;
    }

    public SceneManager GetSceneManager()
    {
        if (m_sceneManager == null)
            m_sceneManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();

        return m_sceneManager;
    }

    public PersistentDataManager GetPersistentDataManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PersistentDataManager>();

        return m_persistentDataManager;
    }

    public CallFuncHandler GetCallFuncHandler()
    {
        if (m_callFuncHandler == null)
            m_callFuncHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<CallFuncHandler>();

        return m_callFuncHandler;
    }
}

