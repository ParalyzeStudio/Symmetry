using UnityEngine;

public class VeilOpacityAnimator : ValueAnimator
{
    private bool m_transitioningOverScenes;
    private float m_transitioningOverScenesDuration;
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
    public void TransitionOverScenes(float fDuration, float fDelay = 0.0f)
    {
        m_transitioningOverScenes = true;
        m_transitioningOverScenesDuration = fDuration;
        OnOpacityChanged(0);
        FadeFromTo(0, 1, 0.5f * fDuration, fDelay);
    }

    public override void OnFinishFading()
    {
        if (m_transitioningOverScenes)
        {
            if (m_opacity == 1) //we reached the peak of the transition, now fade out
            {
                FadeFromTo(1, 0, 0.5f * m_transitioningOverScenesDuration);
            }
            else //we're done transitioning
            {
                m_transitioningOverScenes = false;
                if (m_destroyVeilOnFinishTransitioning)
                    Destroy(this.gameObject);
            }
        }
    }

    //protected override void Update()
    //{
    //    base.Update();
    //    Debug.Log("Update");
    //}
}
