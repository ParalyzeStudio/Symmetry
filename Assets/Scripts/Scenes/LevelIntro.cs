using UnityEngine;

public class LevelIntro : GUIScene
{
    private const float TITLE_Z_VALUE = -10.0f;

    public GameObject m_textMeshPfb;
    private bool m_gameSceneLaunched;

    public override void Show()
    {
        base.Show();
        GameObjectAnimator sceneAnimator = this.GetComponent<GameObjectAnimator>();
        ShowChapterAndLevel();
        ShowSkipButton();

        m_gameSceneLaunched = false;
        GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(LaunchGameScene), 5.0f);
    }

    /**
     * Display the chapter number and the level number of the level to be played
     * **/
    public void ShowChapterAndLevel()
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        int currentChapterNumber = levelManager.m_currentChapter.m_number;
        int currentLevelNumber = levelManager.m_currentLevel.m_chapterRelativeNumber;

        GameObject titleObject = (GameObject) Instantiate(m_textMeshPfb);
        titleObject.transform.parent = this.gameObject.transform;
        titleObject.GetComponent<TextMesh>().text = currentChapterNumber.ToString() + " - " + currentLevelNumber.ToString();

        TextMeshAnimator levelIntroTitleAnimator = titleObject.GetComponent<TextMeshAnimator>();
        levelIntroTitleAnimator.SetPosition(new Vector3(0, 0, TITLE_Z_VALUE));
        levelIntroTitleAnimator.SetFontHeight(80);
        levelIntroTitleAnimator.SetColor(Color.white);
        levelIntroTitleAnimator.SetOpacity(0);
        levelIntroTitleAnimator.FadeTo(1.0f, 0.5f);
    }

    /**
     * Display a skip button
     * **/
    public void ShowSkipButton()
    {
        GameObject skipTextObject = (GameObject)Instantiate(m_textMeshPfb);
        skipTextObject.transform.parent = this.gameObject.transform;

        skipTextObject.GetComponent<TextMesh>().text = "Skip>>>";
        
        TextMeshAnimator skipButtonAnimator = skipTextObject.GetComponent<TextMeshAnimator>();
        skipButtonAnimator.SetPosition(new Vector3(0, -100, TITLE_Z_VALUE));
        skipButtonAnimator.SetFontHeight(30);
        skipButtonAnimator.SetOpacity(0);
        skipButtonAnimator.SetColor(Color.white);
        skipButtonAnimator.FadeTo(1.0f, 0.5f);
    }

    public void LaunchGameScene()
    {
        if (m_gameSceneLaunched)
            return;

        m_gameSceneLaunched = true;
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 1.0f);
    }
}

