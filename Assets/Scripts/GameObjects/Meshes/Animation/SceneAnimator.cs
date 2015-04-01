using UnityEngine;

public class SceneAnimator : GameObjectAnimator
{
    public override void OnStartFading()
    {
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        if (sceneManager.m_displayedContent == SceneManager.DisplayContent.LEVEL_INTRO)
        {
            if (m_fromOpacity == 1 && m_toOpacity == 0)
            {
                sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 0.0f, 0.7f, 0.0f);
            }
        }
    }
}

