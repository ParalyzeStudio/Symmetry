using UnityEngine;

public class GUIScene : MonoBehaviour
{
    private bool m_dismissSceneRunning;
    private float m_dismissSceneElapsedTime;
    private float m_dismissSceneDelay;
    private float m_dismissSceneDuration;

    public void Awake()
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
    public virtual void Dismiss(bool bAnimated, float fDuration, float fDelay = 0.0f)
    {
        if (bAnimated)
        {
            m_dismissSceneRunning = true;
            m_dismissSceneElapsedTime = 0;
            m_dismissSceneDuration = fDuration;
            m_dismissSceneDelay = fDelay;
        }
    }

    public virtual void OnSceneDismissed()
    {

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

