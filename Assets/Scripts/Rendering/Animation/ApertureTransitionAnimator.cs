using UnityEngine;

public class ApertureTransitionAnimator : CircleMeshAnimator
{
    public SceneManager.DisplayContent m_toSceneContent { get; set; }

    public override void OnFinishAnimatingInnerRadius()
    {
        base.OnFinishAnimatingInnerRadius();

        Debug.Log("end APERTURE");
        //GetSceneManager().SwitchDisplayedContent(m_toSceneContent, false);
    }
}

