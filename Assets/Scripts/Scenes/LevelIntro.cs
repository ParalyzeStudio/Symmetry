using UnityEngine;

public class LevelIntro : GUIScene
{
    public GameObject m_textMeshPfb;

    public override void Show()
    {
        base.Show();
        GameObjectAnimator sceneAnimator = this.GetComponent<GameObjectAnimator>();
        ShowChapterAndLevel();
        ShowSkipButton();

        sceneAnimator.FadeTo(0, 0.7f, 5.0f);
    }

    public void ShowChapterAndLevel()
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        int currentChapterNumber = levelManager.m_currentChapter.m_number;
        int currentLevelNumber = levelManager.m_currentLevel.m_chapterRelativeNumber;

        GameObject clonedLevelIntroTitle = (GameObject) Instantiate(m_textMeshPfb);
        clonedLevelIntroTitle.transform.parent = this.gameObject.transform;
        clonedLevelIntroTitle.GetComponent<TextMesh>().text = currentChapterNumber.ToString() + " - " + currentLevelNumber.ToString();
    }

    public void ShowSkipButton()
    {
        GameObject clonedSkipText = (GameObject)Instantiate(m_textMeshPfb);
        clonedSkipText.transform.parent = this.gameObject.transform;
    }
}

