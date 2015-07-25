using UnityEngine;

public class ApertureTransitionAnimator : CircleMeshAnimator
{
    public SceneManager.DisplayContent m_toSceneContent { get; set; }

    public override void OnFinishAnimatingInnerRadius()
    {
        base.OnFinishAnimatingInnerRadius();

        Debug.Log("end APERTURE");

        AnimateInnerRadiusTo(m_outerRadius, 0.5f, 0.0f, InterpolationType.SINUSOIDAL, true);
        GetSceneManager().SwitchDisplayedContent(m_toSceneContent, false);
    }
}

