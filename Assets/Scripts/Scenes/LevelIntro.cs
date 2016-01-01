using UnityEngine;

public class LevelIntro : GUIScene
{
    private const float OPAQUE_BACKGROUND_Z_VALUE = -30.0f;
    private const float CONTENT_Z_VALUE = -31.0f;

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
    public PulsatingButton m_skipButton { get; set; }

    //animator of scene elements
    private TextMeshAnimator m_titleAnimator;
    private ColorQuadAnimator m_lineAnimator;
    private GameObjectAnimator m_skipButtonAnimator;
    private TextMeshAnimator m_skipTextAnimator;

    public bool m_loadingLevelIntroFromRetry { get; set; } //is the LevelIntro loaded from the retry button in the GameScene

    public override void Show()
    {
        base.Show();
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

    private void ShowSeparationLine()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject lineObject = (GameObject)Instantiate(m_colorQuadPfb);
        lineObject.name = "SeparationLine";

        ColorQuad line = lineObject.GetComponent<ColorQuad>();
        line.Init(Instantiate(m_transpColorMaterial));

        m_lineAnimator = lineObject.GetComponent<ColorQuadAnimator>();
        m_lineAnimator.SetParentTransform(this.transform);
        m_lineAnimator.SetPosition(new Vector3(0, -0.045f * screenSize.y, CONTENT_Z_VALUE));
        m_lineAnimator.SetScale(new Vector3(1200, 4, 1));
        m_lineAnimator.SetColor(Color.white);
    }

    /**
     * Display the chapter number and the level number of the level to be played
     * **/
    private void ShowChapterAndLevel()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

        string titleText;
        int isDebugLevel = levelManager.IsCurrentLevelDebugLevel();
        if (isDebugLevel > 0)
            titleText = "Debug level " + isDebugLevel;
        else
        {
            int currentChapterNumber = levelManager.m_currentChapter.m_number;
            int currentLevelNumber = levelManager.m_currentLevel.m_chapterRelativeNumber;
            titleText = currentChapterNumber.ToString() + " - " + currentLevelNumber.ToString();
        }

        GameObject titleObject = (GameObject) Instantiate(m_textMeshPfb);
        titleObject.name = "Title";
        titleObject.GetComponent<TextMesh>().text = titleText;

        m_titleAnimator = titleObject.GetComponent<TextMeshAnimator>();
        m_titleAnimator.SetParentTransform(this.transform);
        m_titleAnimator.SetPosition(new Vector3(0, 0.1f * screenSize.y, CONTENT_Z_VALUE));
        m_titleAnimator.SetFontHeight(130);
        m_titleAnimator.SetColor(Color.white);
        if (m_loadingLevelIntroFromRetry)
        {
            m_titleAnimator.SetOpacity(0);
            m_titleAnimator.FadeTo(1.0f, 0.5f);
        }
    }

    /**
     * Display a skip button
     * **/
    private void ShowSkipButton()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject skipButtonObject = (GameObject)Instantiate(m_pulsatingButtonPfb);
        skipButtonObject.name = "SkipButton";

        m_skipButton = skipButtonObject.GetComponent<PulsatingButton>();
        m_skipButton.Build(200, Color.white, 8.0f, 1.0f);

        m_skipButtonAnimator = skipButtonObject.GetComponent<GameObjectAnimator>();
        m_skipButtonAnimator.SetParentTransform(this.transform);
        m_skipButtonAnimator.SetPosition(new Vector3(0, -0.29f * screenSize.y, CONTENT_Z_VALUE));

        //Show text above button
        GameObject m_skipTextObject = (GameObject)Instantiate(m_textMeshPfb);
        m_skipTextObject.name = "SkipText";
        m_skipTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("skip");

        m_skipTextAnimator = m_skipTextObject.GetComponent<TextMeshAnimator>();
        m_skipTextAnimator.SetParentTransform(this.transform);
        m_skipTextAnimator.SetPosition(new Vector3(0, -0.14f * screenSize.y, CONTENT_Z_VALUE));
        m_skipTextAnimator.SetColor(Color.white);
        m_skipTextAnimator.SetFontHeight(40);
    }
    
    private void DismissOpaqueBackground(float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_leftPanelAnimator.SetParentTransform(null);
        m_rightPanelAnimator.SetParentTransform(null);

        m_leftPanelAnimator.TranslateTo(new Vector3(-0.25f * screenSize.x, screenSize.y, OPAQUE_BACKGROUND_Z_VALUE), 2.0f, fDelay, ValueAnimator.InterpolationType.X3, true);
        m_rightPanelAnimator.TranslateTo(new Vector3(0.25f * screenSize.x, screenSize.y, OPAQUE_BACKGROUND_Z_VALUE), 2.0f, fDelay, ValueAnimator.InterpolationType.X3, true);
    }

    private void DismissAllElements(float fDelay = 0.0f)
    {
        m_titleAnimator.SetParentTransform(null);
        m_lineAnimator.SetParentTransform(null);
        m_skipButtonAnimator.SetParentTransform(null);
        m_skipTextAnimator.SetParentTransform(null);
        m_titleAnimator.FadeTo(0.0f, 1.0f, fDelay, ValueAnimator.InterpolationType.LINEAR, true);
        m_lineAnimator.FadeTo(0.0f, 1.0f, fDelay, ValueAnimator.InterpolationType.LINEAR, true);        
        m_skipTextAnimator.FadeTo(0.0f, 1.0f, fDelay, ValueAnimator.InterpolationType.LINEAR, true);


        Vector3 skipButtonPosition = m_skipButtonAnimator.GetPosition();
        m_skipButtonAnimator.TranslateTo(skipButtonPosition + new Vector3(0, 400.0f, 0), 3.0f);
        m_skipButton.StopPulsating();
        Destroy(m_skipButton.gameObject, 3.0f);
    }

    public void LaunchGameScene()
    {
        if (m_gameSceneLaunched)
            return;

        m_gameSceneLaunched = true;
        DismissOpaqueBackground(0.5f);
        DismissAllElements(0.5f);
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.GAME, true, 0.0f);
    }
}