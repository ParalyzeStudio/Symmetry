using UnityEngine;

public class SceneAnimator : GameObjectAnimator
{
    public override void OnFinishFading()
    {
        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        LevelIntro levelIntro = this.GetComponent<LevelIntro>();
        if (levelIntro != null) //this scene is the level intro scene
        {
            sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 0.0f, 0.7f, 0.0f);
        }
    }
}

