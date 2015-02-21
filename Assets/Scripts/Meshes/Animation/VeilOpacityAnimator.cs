using UnityEngine;

public class VeilOpacityAnimator : ValueAnimator
{
    private bool m_transitioningOverScenes;
    private float m_transitioningOverScenesPhaseDuration;
    private float m_transitioningOverScenesPeakDuration;
    public bool m_destroyVeilOnFinishTransitioning { get; set; }

    protected void Awake()
    {
        m_transitioningOverScenes = false;
        m_destroyVeilOnFinishTransitioning = true;
    }

    public override void OnOpacityChanged(float fNewOpacity)
    {
        Material veilMaterial = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        Color color = veilMaterial.GetColor("_Color");
        color.a = fNewOpacity;
        veilMaterial.SetColor("_Color", color);
    }

    /**
     * Makes the veil fade in and then fade out for a total duration of fDuration after fDelay seconds
     * **/
    public void TransitionOverScenes(float fPhaseDuration, float fPeakDuration, float fDelay = 0.0f)
    {
        m_transitioningOverScenes = true;
        m_transitioningOverScenesPhaseDuration = fPhaseDuration;
        m_transitioningOverScenesPeakDuration = fPeakDuration;
        OnOpacityChanged(0);
        FadeFromTo(0, 1, 0.5f * fPhaseDuration, fDelay);
    }

    public override void OnFinishFading()
    {
        if (m_transitioningOverScenes)
        {
            if (m_opacity == 1) //we reached the peak of the transition, now fade out
            {
                OnTransitionPeakReached();
                FadeFromTo(1, 0, m_transitioningOverScenesPhaseDuration, m_transitioningOverScenesPeakDuration);
            }
            else //we're done transitioning
            {
                m_transitioningOverScenes = false;
                if (m_destroyVeilOnFinishTransitioning)
                    Destroy(this.gameObject);
            }
        }
    }

    public virtual void OnTransitionPeakReached()
    {
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.OnTransitionVeilPeakReached();
    }
}
