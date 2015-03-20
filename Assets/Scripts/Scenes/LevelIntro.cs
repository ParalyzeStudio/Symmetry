﻿using UnityEngine;

public class LevelIntro : GUIScene
{
    public GameObject m_levelIntroTitlePfb;

    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);
        GameObjectAnimator sceneAnimator = this.GetComponent<GameObjectAnimator>();
        //sceneAnimator.OnOpacityChanged(1);
        sceneAnimator.SetOpacity(1);
        ShowChapterAndLevel(fDelay);

        SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 5.0f, 0.7f);
    }

    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        base.Dismiss(fDuration, fDelay);
    }

    public void ShowChapterAndLevel(float fDelay)
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        int currentChapterNumber = levelManager.m_currentChapter.m_number;
        int currentLevelNumber = levelManager.m_currentLevel.m_number;

        GameObject clonedLevelIntroTitle = (GameObject) Instantiate(m_levelIntroTitlePfb);
        clonedLevelIntroTitle.transform.parent = this.gameObject.transform;
        clonedLevelIntroTitle.GetComponent<TextMesh>().text = currentChapterNumber.ToString() + " - " + currentLevelNumber.ToString();

        TextMeshAnimator titleAnimator = clonedLevelIntroTitle.GetComponent<TextMeshAnimator>();
        //titleAnimator.OnOpacityChanged(0);
        //titleAnimator.FadeFromTo(0, 1, 0.5f, fDelay);
        titleAnimator.SetOpacity(0);
        titleAnimator.FadeTo(1, 0.5f, fDelay);
    }
}

