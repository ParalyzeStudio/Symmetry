using UnityEngine;

public class ApertureTransitionAnimator : CircleMeshAnimator
{
    public SceneManager.DisplayContent m_toSceneContent { get; set; }

    public override void OnFinishAnimatingInnerRadius()
    {
        base.OnFinishAnimatingInnerRadius();
        
        if (m_innerRadius == 0)
        {
            AnimateInnerRadiusTo(m_outerRadius, 0.5f, 0.0f, InterpolationType.SINUSOIDAL, true);
            GetSceneManager().SwitchDisplayedContent(m_toSceneContent, false);
        }
    }
}

