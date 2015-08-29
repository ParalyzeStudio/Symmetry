using UnityEngine;

public class SceneAnimator : GameObjectAnimator
{
    public override void OnFinishFading()
    {
        LevelIntro levelIntro = this.GetComponent<LevelIntro>();
        if (levelIntro != null) //this scene is the level intro scene
        {
            GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 0.0f);
        }
    }
}

