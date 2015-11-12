using UnityEngine;

public class LevelIntro : GUIScene
{
    private const float OPAQUE_BACKGROUND_Z_VALUE = -8.0f;
    private const float CONTENT_Z_VALUE = -10.0f;

    public GameObject m_colorQuadPfb;
    //public GameObject m_circleMeshPfb;
    public Material m_transpColorMaterial;
    public GameObject m_textMeshPfb;
    public GameObject m_pulsatingButtonPfb;
    private bool m_gameSceneLaunched;

    //background
    private GameObjectAnimator m_leftPanelAnimator;
    private GameObjectAnimator m_rightPanelAnimator;

    //skip button
    public GameObject m_skipButtonObject { get; set; }

    public bool m_loadingLevelIntroFromRetry { get; set; } //is the LevelIntro loaded from the retry button in the GameScene

    public override void Show()
    {
        Debug.Log("Show:" + m_loadingLevelIntroFromRetry);
        base.Show();
        GameObjectAnimator sceneAnimator = this.GetComponent<GameObjectAnimator>();
        ShowOpaqueBackground();
        ShowChapterAndLevel();
        ShowSeparationLine();
        ShowSkipButton();

        m_gameSceneLaunched = false;
        //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(LaunchGameScene), 5.0f);
    }

    private void ShowOpaqueBackground()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        Material backgroundMaterial = Instantiate(m_transpColorMaterial);
        Color backgroundColor = GetLevelManager().m_currentChapter.GetThemeColors()[0];
        backgroundColor = ColorUtils.DarkenColor(backgroundColor, 0.2f);

        //left panel
        GameObject leftPanelObject = (GameObject)Instantiate(m_colorQuadPfb);
        leftPanelObject.name = "BackgroundLeftPanel";

        ColorQuad leftPanel = leftPanelObject.GetComponent<ColorQuad>();
        leftPanel.Init(backgroundMaterial);

        m_leftPanelAnimator = leftPanelObject.GetComponent<GameObjectAnimator>();
        m_leftPanelAnimator.SetParentTransform(this.transform);
        m_leftPanelAnimator.SetPosition(new Vector3(-0.25f * screenSize.x, 0, OPAQUE_BACKGROUND_Z_VALUE));
        m_leftPanelAnimator.SetScale(new Vector3(0.5f * screenSize.x, screenSize.y, 1));        
        m_leftPanelAnimator.SetColor(backgroundColor);
        if (m_loadingLevelIntroFromRetry)
        {
            m_leftPanelAnimator.SetOpacity(0);
            m_leftPanelAnimator.FadeTo(1.0f, 0.5f);
        }

        //right panel
        GameObject rightPanelObject = (GameObject)Instantiate(m_colorQuadPfb);
        rightPanelObject.name = "BackgroundRightPanel";

        ColorQuad rightPanel = rightPanelObject.GetComponent<ColorQuad>();
        rightPanel.Init(backgroundMaterial);

        m_rightPanelAnimator = rightPanelObject.GetComponent<GameObjectAnimator>();
        m_rightPanelAnimator.SetParentTransform(this.transform);
        m_rightPanelAnimator.SetPosition(new Vector3(0.25f * screenSize.x, 0, OPAQUE_BACKGROUND_Z_VALUE));
        m_rightPanelAnimator.SetScale(new Vector3(0.5f * screenSize.x, screenSize.y, 1));
        m_rightPanelAnimator.SetColor(backgroundColor);
        if (m_loadingLevelIntroFromRetry)
        {
            m_rightPanelAnimator.SetOpacity(0);
            m_rightPanelAnimator.FadeTo(1.0f, 0.5f);
        }
    }

    private void DismissOpaqueBackground()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //m_leftPanelAnimator.SetParentTransform(null);
        //m_rightPanelAnimator.SetParentTransform(null);

        m_leftPanelAnimator.TranslateTo(new Vector3(-1.5f * screenSize.x, 0, OPAQUE_BACKGROUND_Z_VALUE), 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        m_rightPanelAnimator.TranslateTo(new Vector3(1.5f * screenSize.x, 0, OPAQUE_BACKGROUND_Z_VALUE), 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);        
    }

    private void ShowSeparationLine()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject lineObject = (GameObject)Instantiate(m_colorQuadPfb);
        lineObject.name = "SeparationLine";

        ColorQuad line = lineObject.GetComponent<ColorQuad>();
        line.Init(Instantiate(m_transpColorMaterial));

        ColorQuadAnimator lineAnimator = lineObject.GetComponent<ColorQuadAnimator>();
        lineAnimator.SetParentTransform(this.transform);
        lineAnimator.SetPosition(new Vector3(0, -0.045f * screenSize.y, CONTENT_Z_VALUE));
        lineAnimator.SetScale(new Vector3(1200, 4, 1));
        lineAnimator.SetColor(Color.white);
    }

    /**
     * Display the chapter number and the level number of the level to be played
     * **/
    private void ShowChapterAndLevel()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        int currentChapterNumber = levelManager.m_currentChapter.m_number;
        int currentLevelNumber = levelManager.m_currentLevel.m_chapterRelativeNumber;

        GameObject titleObject = (GameObject) Instantiate(m_textMeshPfb);
        titleObject.GetComponent<TextMesh>().text = currentChapterNumber.ToString() + " - " + currentLevelNumber.ToString();

        TextMeshAnimator levelIntroTitleAnimator = titleObject.GetComponent<TextMeshAnimator>();
        levelIntroTitleAnimator.SetParentTransform(this.transform);
        levelIntroTitleAnimator.SetPosition(new Vector3(0, 0.1f * screenSize.y, CONTENT_Z_VALUE));
        levelIntroTitleAnimator.SetFontHeight(130);
        levelIntroTitleAnimator.SetColor(Color.white);
        if (m_loadingLevelIntroFromRetry)
        {
            levelIntroTitleAnimator.SetOpacity(0);
            levelIntroTitleAnimator.FadeTo(1.0f, 0.5f);
        }
    }

    /**
     * Display a skip button
     * **/
    private void ShowSkipButton()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_skipButtonObject = (GameObject)Instantiate(m_pulsatingButtonPfb);
        m_skipButtonObject.name = "SkipButton";

        PulsatingButton skipButton = m_skipButtonObject.GetComponent<PulsatingButton>();
        skipButton.Build(200, Color.white, 8.0f, 1.0f);

        GameObjectAnimator skipButtonAnimator = m_skipButtonObject.GetComponent<GameObjectAnimator>();
        skipButtonAnimator.SetParentTransform(this.transform);
        skipButtonAnimator.SetPosition(new Vector3(0, -0.29f * screenSize.y, CONTENT_Z_VALUE));

        //Show text above button
        GameObject m_skipTextObject = (GameObject)Instantiate(m_textMeshPfb);
        m_skipTextObject.name = "SkipText";
        m_skipTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("skip");

        TextMeshAnimator playTextAnimator = m_skipTextObject.GetComponent<TextMeshAnimator>();
        playTextAnimator.SetParentTransform(this.transform);
        playTextAnimator.SetPosition(new Vector3(0, -0.14f * screenSize.y, CONTENT_Z_VALUE));
        playTextAnimator.SetColor(Color.white);
        playTextAnimator.SetFontHeight(40);
    }

    public void LaunchGameScene()
    {
        if (m_gameSceneLaunched)
            return;

        m_gameSceneLaunched = true;
        DismissOpaqueBackground();
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 1.0f);
    }
}

