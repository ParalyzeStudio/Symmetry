using UnityEngine;

public class GUIScene : MonoBehaviour
{
    private bool m_dismissSceneRunning;
    private float m_dismissSceneElapsedTime;
    private float m_dismissSceneDelay;
    private float m_dismissSceneDuration;
    protected BackgroundTrianglesRenderer m_backgroundRenderer;

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

    public BackgroundTrianglesRenderer GetBackgroundRenderer()
    {
        if (m_backgroundRenderer == null)
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        return m_backgroundRenderer;
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

