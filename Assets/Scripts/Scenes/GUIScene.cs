using UnityEngine;

public class GUIScene : MonoBehaviour
{
    private bool m_dismissSceneRunning;
    private float m_dismissSceneElapsedTime;
    private float m_dismissSceneDelay;
    private float m_dismissSceneDuration;

    //Global instances to prevent calls to FindGameObjectWithTag and GetComponent<>
    protected GUIManager m_guiManager;
    protected BackgroundTrianglesRenderer m_backgroundRenderer;
    protected LevelManager m_levelManager;
    protected SceneManager m_sceneManager;
    protected PersistentDataManager m_persistentDataManager;

    public virtual void Init()
    {
        m_dismissSceneRunning = false;
    }

    /**
     * Shows this scene
     * **/
    public virtual void Show(bool bAnimated, float fDelay = 0.0f)
    {
        
    }

    /**
     * Dismisses this scene
     * **/
    public virtual void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        m_dismissSceneRunning = true;
        m_dismissSceneElapsedTime = 0;
        m_dismissSceneDuration = fDuration;
        m_dismissSceneDelay = fDelay;

        //fade out the scene
        GameObjectAnimator sceneAnimator = this.gameObject.GetComponent<GameObjectAnimator>();
        sceneAnimator.FadeTo(0, fDuration, fDelay);
    }

    public virtual void OnSceneDismissed()
    {
        Destroy(this.gameObject);
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
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        return m_backgroundRenderer;
    }

    public LevelManager GetLevelManager()
    {
        if (m_levelManager == null)
            m_levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        return m_levelManager;
    }

    public SceneManager GetSceneManager()
    {
        if (m_sceneManager == null)
            m_sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();

        return m_sceneManager;
    }

    public PersistentDataManager GetPersistentDataManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GameObject.FindGameObjectWithTag("PersistentDataManager").GetComponent<PersistentDataManager>();

        return m_persistentDataManager;
    }

    public virtual void Update()
    {
        float dt = Time.deltaTime;

        if (m_dismissSceneRunning)
        {
            m_dismissSceneElapsedTime += dt;
            if (m_dismissSceneElapsedTime > m_dismissSceneDuration + m_dismissSceneDelay)
                OnSceneDismissed();
        }
    }
}

