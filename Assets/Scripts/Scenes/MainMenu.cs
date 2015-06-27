using UnityEngine;

public class MainMenu : GUIScene
{
    public const float TITLE_Z_VALUE = -200.0f;
    public const float GUI_BUTTONS_Z_VALUE = -10.0f;
    public const float PLAY_BANNER_Z_VALUE = -10.0f;

    //buttons
    public GameObject m_optionsButtonObject { get; set; }
    public GameObject m_creditsButtonObject { get; set; }

    //play banner
    public GameObject m_playBannerPfb;
    public GameObject m_playBannerObject { get; set; }
    public Material m_playBannerMaterial;

    public bool m_isPlayBannerDisplayed { get; set; }
    private bool m_isPlayBannerAnimating;
    private float m_playBannerAnimationElapsedTime;
    private float m_playBannerAnimationDuration;

    public void Awake()
    {
        m_isPlayBannerDisplayed = false;
        m_isPlayBannerAnimating = false;
    }

    /**
     * Shows MainMenu with or without animation
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);

        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();
        backgroundRenderer.RenderForMainMenu(bAnimated, fDelay);

        GameObjectAnimator menuAnimator = this.GetComponent<GameObjectAnimator>();
        menuAnimator.SetOpacity(1);
        ShowTitle(bAnimated, fDelay + 2.0f);
        ShowPlayBanner(bAnimated, fDelay + 3.5f);
        ShowButtons(bAnimated, fDelay + 5.0f);

        if (!bAnimated)
        {
            menuAnimator.SetOpacity(0);
            menuAnimator.FadeTo(1, 0.5f, 1.0f);
        }
    }

    public void ShowTitle(bool bAnimated, float fDelay)
    {
        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();

        backgroundRenderer.RenderMainMenuTitle(bAnimated, fDelay);
    }

    /**
     * Show the banner with the play button inside
     * **/
    public void ShowPlayBanner(bool bAnimated, float fDelay)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_playBannerObject = (GameObject)Instantiate(m_playBannerPfb);
        m_playBannerObject.transform.parent = this.transform;
        m_playBannerObject.transform.localPosition = new Vector3(0, -240.0f, PLAY_BANNER_Z_VALUE);

        ColorQuad[] childFrames = m_playBannerObject.GetComponentsInChildren<ColorQuad>();
        GameObject redFrameObject = childFrames[0].gameObject;
        GameObject whiteFrameObject = childFrames[1].gameObject;
        GameObject darkFrameObject = childFrames[2].gameObject;

        GameObject playTextObject = m_playBannerObject.GetComponentInChildren<TextMesh>().gameObject;

        TriangleMesh[] childPlayTriangles = m_playBannerObject.GetComponentsInChildren<TriangleMesh>();
        GameObject frontTriangleObject = childPlayTriangles[0].gameObject;
        GameObject backTriangleObject = childPlayTriangles[1].gameObject;

        //Red frame
        float redFrameScaleDuration = 0.8f;
        Vector3 redFramePosition = new Vector3(-0.5f * screenSize.x, 0, 0);
        float redFrameHeight = 175.0f;
        
        ColorQuad redFrameColorQuad = redFrameObject.GetComponent<ColorQuad>();
        redFrameColorQuad.InitQuadMesh();

        MeshRenderer redFrameRenderer = redFrameObject.GetComponent<MeshRenderer>();
        redFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

        ColorQuadAnimator redFrameAnimator = redFrameObject.GetComponent<ColorQuadAnimator>();
        redFrameAnimator.SetColor(ColorUtils.GetColorFromRGBAVector4(new Vector4(219, 0, 47, 255)));
        redFrameAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
        redFrameAnimator.SetPosition(redFramePosition);
        redFrameAnimator.SetScale(new Vector3(0, redFrameHeight, 1));
        redFrameAnimator.ScaleTo(new Vector3(screenSize.x, redFrameHeight, 1), redFrameScaleDuration, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

        //White frame
        Vector3 whiteFramePosition = new Vector3(0, -0.5f * redFrameHeight, 0);
        float whiteFrameScaleDuration = 0.2f;

        ColorQuad whiteFrameColorQuad = whiteFrameObject.GetComponent<ColorQuad>();
        whiteFrameColorQuad.InitQuadMesh();

        MeshRenderer whiteFrameRenderer = whiteFrameObject.GetComponent<MeshRenderer>();
        whiteFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

        ColorQuadAnimator whiteFrameAnimator = whiteFrameObject.GetComponent<ColorQuadAnimator>();
        whiteFrameAnimator.SetColor(Color.white);
        whiteFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        whiteFrameAnimator.SetPosition(whiteFramePosition);
        whiteFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
        whiteFrameAnimator.ScaleTo(new Vector3(screenSize.x, 4.0f, 1), whiteFrameScaleDuration, fDelay + redFrameScaleDuration, ValueAnimator.InterpolationType.SINUSOIDAL);

        //Dark frame
        Vector3 darkFramePosition = new Vector3(0, -0.5f * redFrameHeight, 1);
        float darkFrameScaleDuration = 0.4f;

        ColorQuad darkFrameColorQuad = darkFrameObject.GetComponent<ColorQuad>();
        darkFrameColorQuad.InitQuadMesh();

        MeshRenderer darkFrameRenderer = darkFrameObject.GetComponent<MeshRenderer>();
        darkFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

        ColorQuadAnimator darkFrameAnimator = darkFrameObject.AddComponent<ColorQuadAnimator>();
        darkFrameAnimator.SetColor(Color.black);
        darkFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        darkFrameAnimator.SetPosition(darkFramePosition);
        darkFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
        darkFrameAnimator.ScaleTo(new Vector3(screenSize.x, 12.0f, 1), darkFrameScaleDuration, fDelay + redFrameScaleDuration, ValueAnimator.InterpolationType.SINUSOIDAL);

        //Play text label
        float playButtonFadeDuration = 0.5f;

        playTextObject.transform.localPosition = new Vector3(0, 0, -1);
        playTextObject.transform.parent = m_playBannerObject.transform;

        TextMesh playTextMesh = playTextObject.GetComponent<TextMesh>();
        playTextMesh.text = LanguageUtils.GetTranslationForTag("play");
        Vector3 playTextMeshSize = playTextMesh.GetComponent<MeshRenderer>().bounds.size;

        TextMeshAnimator playTextAnimator = playTextObject.GetComponent<TextMeshAnimator>();
        playTextAnimator.SetOpacity(0);
        playTextAnimator.FadeTo(1.0f, playButtonFadeDuration, fDelay + redFrameScaleDuration);

        //Play triangles
        GameObject playTrianglesObject = frontTriangleObject.transform.parent.gameObject;
        playTrianglesObject.transform.localPosition = new Vector3(playTextMeshSize.x + 20.0f, 0, -1);
        playTrianglesObject.GetComponent<GameObjectAnimator>().FadeTo(1.0f, playButtonFadeDuration, fDelay + redFrameScaleDuration);


        float triangleScale = 20.0f;

        Vector3[] triangleVertices = new Vector3[3];
        triangleVertices[0] = triangleScale * new Vector2(-0.5f, 1.0f);
        triangleVertices[1] = triangleScale * new Vector2(-0.5f, -1.0f);
        triangleVertices[2] = triangleScale * new Vector2(0.5f, 0.0f);

        //Front triangle
        Color frontTriangleColor = Color.white;
        
        frontTriangleObject.transform.parent = frontTriangleObject.transform;
        frontTriangleObject.transform.localPosition = new Vector3(0, 0, -1);

        TriangleMesh triangleMesh = frontTriangleObject.GetComponent<TriangleMesh>();
        triangleMesh.Init(Instantiate(m_playBannerMaterial));
        triangleMesh.Render(triangleVertices, frontTriangleColor, false);

        TriangleAnimator frontTriangleAnimator = frontTriangleObject.AddComponent<TriangleAnimator>();
        frontTriangleAnimator.SetColor(frontTriangleColor);
        frontTriangleAnimator.SetOpacity(0);

        //Back triangle
        Color backTriangleColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(50, 50, 50, 255));

        triangleMesh = backTriangleObject.AddComponent<TriangleMesh>();
        triangleMesh.Init(Instantiate(m_playBannerMaterial));
        triangleMesh.Render(triangleVertices, backTriangleColor, false);

        TriangleAnimator backTriangleAnimator = backTriangleObject.AddComponent<TriangleAnimator>();
        backTriangleAnimator.SetOpacity(0);
        backTriangleAnimator.TranslateTo(new Vector3(3, -3, 0), 0.3f, fDelay + redFrameScaleDuration + playButtonFadeDuration);

        //Set the play banner as 'displayed' after animation ended
        m_isPlayBannerAnimating = true;
        m_playBannerAnimationDuration = fDelay + redFrameScaleDuration + playButtonFadeDuration;
        m_playBannerAnimationElapsedTime = 0;
    }

    /**
     * Removes the play banner with animation and eventually destroy it at the end of this animation
     * **/
    public void DismissPlayBanner()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        ColorQuad[] childFrames = m_playBannerObject.GetComponentsInChildren<ColorQuad>();
        GameObject redFrameObject = childFrames[0].gameObject;
        GameObject whiteFrameObject = childFrames[1].gameObject;
        GameObject darkFrameObject = childFrames[2].gameObject;

        GameObject playTextObject = m_playBannerObject.GetComponentInChildren<TextMesh>().gameObject;

        TriangleMesh[] childPlayTriangles = m_playBannerObject.GetComponentsInChildren<TriangleMesh>();
        GameObject frontTriangleObject = childPlayTriangles[0].gameObject;
        GameObject backTriangleObject = childPlayTriangles[1].gameObject;

        //white and dark frames
        whiteFrameObject.GetComponent<ColorQuadAnimator>().ScaleTo(new Vector3(screenSize.x, 0, 1), 0.2f, 0.0f);
        darkFrameObject.GetComponent<ColorQuadAnimator>().ScaleTo(new Vector3(screenSize.x, 0, 1), 0.4f, 0.0f);

        //red frame
        ColorQuadAnimator redFrameAnimator = redFrameObject.GetComponent<ColorQuadAnimator>();
        redFrameAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.0f));
        redFrameAnimator.ScaleTo(new Vector3(0, redFrameAnimator.transform.localScale.y, redFrameAnimator.transform.localScale.z), 0.8f, 0.4f);

        //Play triangles
        GameObject playTrianglesObject = frontTriangleObject.transform.parent.gameObject;
        playTrianglesObject.GetComponent<GameObjectAnimator>().FadeTo(0.0f, 0.4f, 0.0f);

        //Play text
        playTextObject.GetComponent<TextMeshAnimator>().FadeTo(0.0f, 0.4f, 0.0f);

        m_isPlayBannerDisplayed = false;
    }

    /**
     * Show credits and options button
     * **/
    public void ShowButtons(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

        //Options button
        m_optionsButtonObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
                                                                         new Vector2(128.0f, 128.0f),
                                                                         ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
                                                                         Color.black);
        m_optionsButtonObject.name = "OptionsButton";

        m_optionsButtonObject.transform.parent = this.transform;

        GameObjectAnimator optionsButtonAnimator = m_optionsButtonObject.GetComponent<GameObjectAnimator>();
        Vector3 optionsButtonFinalPosition = new Vector3(-200.0f, -0.5f * screenSize.y + 150.0f, GUI_BUTTONS_Z_VALUE);

        //Credits button
        m_creditsButtonObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CREDITS_BUTTON,
                                                                         new Vector2(128.0f, 128.0f),
                                                                         ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
                                                                         Color.black);
        m_creditsButtonObject.name = "CreditsButton";

        m_creditsButtonObject.transform.parent = this.transform;

        GameObjectAnimator creditsButtonAnimator = m_creditsButtonObject.GetComponent<GameObjectAnimator>();
        Vector3 creditsButtonFinalPosition = new Vector3(200.0f, -0.5f * screenSize.y + 150.0f, GUI_BUTTONS_Z_VALUE);

        if (bAnimated)
        {
            Vector3 optionsButtonFromPosition = new Vector3(-200.0f, -0.5f * screenSize.y - 150.0f, GUI_BUTTONS_Z_VALUE);
            Vector3 creditsButtonFromPosition = new Vector3(200.0f, -0.5f * screenSize.y - 150.0f, GUI_BUTTONS_Z_VALUE);

            optionsButtonAnimator.SetPosition(optionsButtonFromPosition);
            creditsButtonAnimator.SetPosition(creditsButtonFromPosition);
            optionsButtonAnimator.TranslateTo(optionsButtonFinalPosition, 0.5f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
            creditsButtonAnimator.TranslateTo(creditsButtonFinalPosition, 0.5f, fDelay + 0.15f, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else
        {
            optionsButtonAnimator.SetPosition(optionsButtonFinalPosition);
            creditsButtonAnimator.SetPosition(creditsButtonFinalPosition);
        }
    }

    public void DismissButtons()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObjectAnimator optionsButtonAnimator = m_optionsButtonObject.GetComponent<GameObjectAnimator>();
        GameObjectAnimator creditsButtonAnimator = m_creditsButtonObject.GetComponent<GameObjectAnimator>();

        Vector3 optionsButtonFinalPosition = new Vector3(-200.0f, -0.5f * screenSize.y - 150.0f, GUI_BUTTONS_Z_VALUE);
        Vector3 creditsButtonFinalPosition = new Vector3(200.0f, -0.5f * screenSize.y - 150.0f, GUI_BUTTONS_Z_VALUE);

        optionsButtonAnimator.TranslateTo(optionsButtonFinalPosition, 0.5f, 0, ValueAnimator.InterpolationType.SINUSOIDAL);
        creditsButtonAnimator.TranslateTo(creditsButtonFinalPosition, 0.5f, 0, ValueAnimator.InterpolationType.SINUSOIDAL);
    }

    public override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;

        if (m_isPlayBannerAnimating)
        {
            m_playBannerAnimationElapsedTime += dt;
            if (m_playBannerAnimationElapsedTime >= m_playBannerAnimationDuration)
            {
                m_isPlayBannerDisplayed = true;
            }
        }
    }
}

