using UnityEngine;

public class ApertureTransitionAnimator : CircleMeshAnimator
{
    public SceneManager.DisplayContent m_toSceneContent { get; set; }

    public override void OnFinishAnimatingInnerRadius()
    {
        if (m_innerRadiusAnimationDestroyOnFinish)
            Destroy(this.gameObject);
        else
        {
            if (m_toSceneContent == SceneManager.DisplayContent.LEVEL_INTRO)
            {
                if (m_innerRadius == 0)
                {
                    AnimateInnerRadiusTo(m_outerRadius, 1.0f, 0.0f, InterpolationType.SINUSOIDAL, true);
                    GetSceneManager().SwitchDisplayedContent(m_toSceneContent, false);

                    //the Show() method will be called in the next frame due to the way CallFuncHandler works.
                    //Thus we can set the boolean in this frame and be sure it will be set before Show() is actually called
                    LevelIntro levelIntroPendingScene = (LevelIntro)GetSceneManager().m_pendingScene;

                    //remove side buttons
                    GetGUIManager().DismissSideButtons(false);
                }
            }
        }
    }
}

