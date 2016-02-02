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
    public bool m_startingFromLevelsScene { get; set; }

    //backgroundm_startingFromLevelsScene
    private GameObjectAnimator m_backgroundAnimator;

    //skip button
    public PulsatingButton m_skipButton { get; set; }

    //animator of scene elements
    private TextMeshAnimator m_titleAnimator;
    private ColorQuadAnimator m_lineAnimator;
    private GameObjectAnimator m_skipButtonAnimator;
    private TextMeshAnimator m_skipTextAnimator;

    public override void Show()
    {
        base.Show();
        ShowOpaqueBackground();
        if (m_startingFromLevelsScene)
        {
            ShowChapterAndLevel();
            ShowSeparationLine();
            ShowSkipButton();
        }
        else //delay the display of elements
        {
            GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowChapterAndLevel), 1.0f);
            GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowSeparationLine), 1.0f);
            GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(ShowSkipButton), 1.0f);
        }

        m_gameSceneLaunched = false;
        //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(LaunchGameScene), 5.0f);
    }

    private void ShowOpaqueBackground()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        Material backgroundMaterial = Instantiate(m_transpColorMaterial);
        Color backgroundColor = GetLevelManager().m_currentChapter.GetThemeColors()[0];
        backgroundColor = ColorUtils.DarkenColor(backgroundColor, 0.2f);

        //background panel
        GameObject backgroundObject = (GameObject)Instantiate(m_colorQuadPfb);
        backgroundObject.name = "Background";

        ColorQuad backgroundQuad = backgroundObject.GetComponent<ColorQuad>();
        backgroundQuad.Init(backgroundMaterial);

        m_backgroundAnimator = backgroundObject.GetComponent<GameObjectAnimator>();
        m_backgroundAnimator.SetParentTransform(this.transform);
        m_backgroundAnimator.SetScale(new Vector3(screenSize.x, screenSize.y, 1));
        m_backgroundAnimator.SetColor(backgroundColor);
        if (m_startingFromLevelsScene)
            m_backgroundAnimator.SetPosition(new Vector3(0, 0, OPAQUE_BACKGROUND_Z_VALUE));
        else
        {
            m_backgroundAnimator.SetPosition(new Vector3(0, screenSize.y, OPAQUE_BACKGROUND_Z_VALUE));
            m_backgroundAnimator.TranslateTo(new Vector3(0, 0, OPAQUE_BACKGROUND_Z_VALUE), 1.0f);
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
        if (!m_startingFromLevelsScene)
        {
            m_lineAnimator.SetOpacity(0);
            m_lineAnimator.FadeTo(1.0f, 0.5f);
        }
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
        if (!m_startingFromLevelsScene)
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

        m_backgroundAnimator.SetParentTransform(null);

        m_backgroundAnimator.TranslateTo(new Vector3(0, screenSize.y, OPAQUE_BACKGROUND_Z_VALUE), 2.0f, fDelay, ValueAnimator.InterpolationType.X3, true);
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