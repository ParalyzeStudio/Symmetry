using UnityEngine;

public class MainMenu : GUIScene
{
    public const float TITLE_Z_VALUE = -200.0f;
    public const float GUI_BUTTONS_Z_VALUE = -10.0f;
    public const float PLAY_BANNER_Z_VALUE = -10.0f;

    public GameObject m_playTextPfb;
    public Material m_playBannerMaterial;

    /**
     * Shows MainMenu with or without animation
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);

        BackgroundTrianglesRenderer backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();
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
        BackgroundTrianglesRenderer backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        backgroundRenderer.RenderMainMenuTitle(bAnimated, fDelay);
    }

    /**
     * Show the banner with the play button inside
     * **/
    public void ShowPlayBanner(bool bAnimated, float fDelay)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject bannerObject = new GameObject("PlayBanner"); //holder
        bannerObject.transform.parent = this.transform;

        //Red frame
        float redFrameYPosition = -240.0f;
        float redFrameScaleDuration = 0.8f;
        Vector3 redFramePosition = new Vector3(-0.5f * screenSize.x, redFrameYPosition, PLAY_BANNER_Z_VALUE);
        float redFrameHeight = 175.0f;

        GameObject redFrameObject = new GameObject("RedFrame");
        redFrameObject.transform.parent = bannerObject.transform;
        redFrameObject.transform.localPosition = redFramePosition;

        redFrameObject.AddComponent<MeshFilter>();
        
        ColorQuad redFrameColorQuad = redFrameObject.AddComponent<ColorQuad>();
        redFrameColorQuad.InitQuadMesh();
        
        MeshRenderer redFrameRenderer = redFrameObject.AddComponent<MeshRenderer>();
        redFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

        ColorQuadAnimator redFrameAnimator = redFrameObject.AddComponent<ColorQuadAnimator>();
        redFrameAnimator.SetColor(ColorUtils.GetColorFromRGBAVector4(new Vector4(219, 0, 47, 255)));
        redFrameAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
        redFrameAnimator.SetScale(new Vector3(0, redFrameHeight, 1));
        redFrameAnimator.ScaleTo(new Vector3(screenSize.x, redFrameHeight, 1), redFrameScaleDuration, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

        //White frame
        Vector3 whiteFramePosition = new Vector3(0, redFrameYPosition - 0.5f * redFrameHeight, PLAY_BANNER_Z_VALUE);
        float whiteFrameScaleDuration = 0.2f;

        GameObject whiteFrameObject = new GameObject("WhiteFrame");
        whiteFrameObject.transform.parent = bannerObject.transform;
        whiteFrameObject.transform.localPosition = whiteFramePosition;

        whiteFrameObject.AddComponent<MeshFilter>();

        ColorQuad whiteFrameColorQuad = whiteFrameObject.AddComponent<ColorQuad>();
        whiteFrameColorQuad.InitQuadMesh();

        MeshRenderer whiteFrameRenderer = whiteFrameObject.AddComponent<MeshRenderer>();
        whiteFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

        ColorQuadAnimator whiteFrameAnimator = whiteFrameObject.AddComponent<ColorQuadAnimator>();
        whiteFrameAnimator.SetColor(Color.white);
        whiteFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        whiteFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
        whiteFrameAnimator.ScaleTo(new Vector3(screenSize.x, 4.0f, 1), whiteFrameScaleDuration, fDelay + redFrameScaleDuration, ValueAnimator.InterpolationType.SINUSOIDAL);

        //Dark frame
        Vector3 darkFramePosition = new Vector3(0, redFrameYPosition - 0.5f * redFrameHeight, PLAY_BANNER_Z_VALUE + 1);
        float darkFrameScaleDuration = 0.4f;

        GameObject darkFrameObject = new GameObject("DarkFrame");
        darkFrameObject.transform.parent = bannerObject.transform;
        darkFrameObject.transform.localPosition = darkFramePosition;

        darkFrameObject.AddComponent<MeshFilter>();

        ColorQuad darkFrameColorQuad = darkFrameObject.AddComponent<ColorQuad>();
        darkFrameColorQuad.InitQuadMesh();

        MeshRenderer darkFrameRenderer = darkFrameObject.AddComponent<MeshRenderer>();
        darkFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

        ColorQuadAnimator darkFrameAnimator = darkFrameObject.AddComponent<ColorQuadAnimator>();
        darkFrameAnimator.SetColor(Color.black);
        darkFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        darkFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
        darkFrameAnimator.ScaleTo(new Vector3(screenSize.x, 12.0f, 1), darkFrameScaleDuration, fDelay + redFrameScaleDuration, ValueAnimator.InterpolationType.SINUSOIDAL);

        //Play text label
        float playButtonFadeDuration = 0.5f;

        GameObject playTextObject = (GameObject)Instantiate(m_playTextPfb);
        playTextObject.transform.localPosition = new Vector3(0, redFrameYPosition, PLAY_BANNER_Z_VALUE - 5);
        playTextObject.transform.parent = bannerObject.transform;

        TextMesh playTextMesh = playTextObject.GetComponent<TextMesh>();
        playTextMesh.text = LanguageUtils.GetTranslationForTag("play");
        Vector3 playTextMeshSize = playTextMesh.GetComponent<MeshRenderer>().bounds.size;

        TextMeshAnimator playTextAnimator = playTextObject.GetComponent<TextMeshAnimator>();
        playTextAnimator.SetOpacity(0);
        playTextAnimator.FadeTo(1.0f, playButtonFadeDuration, fDelay + redFrameScaleDuration);

        //Play triangles
        GameObject playTrianglesObject = new GameObject("PlayTriangles");
        playTrianglesObject.AddComponent<GameObjectAnimator>();
        playTrianglesObject.transform.parent = bannerObject.transform;
        playTrianglesObject.transform.localPosition = new Vector3(0.5f * playTextMeshSize.x + 60.0f, redFrameYPosition, PLAY_BANNER_Z_VALUE - 5);

        float triangleScale = 20.0f;

        Vector3[] triangleVertices = new Vector3[3];
        triangleVertices[0] = triangleScale * new Vector2(-0.5f, 1.0f);
        triangleVertices[1] = triangleScale * new Vector2(-0.5f, -1.0f);
        triangleVertices[2] = triangleScale * new Vector2(0.5f, 0.0f);

        //Triangle1
        Color triangle1Color = Color.white;

        GameObject triangle1Object = new GameObject("FrontTriangle");
        TriangleAnimator triangle1Animator = triangle1Object.AddComponent<TriangleAnimator>();
        triangle1Object.transform.parent = playTrianglesObject.transform;
        triangle1Object.transform.localPosition = new Vector3(0, 0, -1);
        TriangleMesh triangleMesh = triangle1Object.AddComponent<TriangleMesh>();
        triangleMesh.Init(Instantiate(m_playBannerMaterial));
        triangleMesh.Render(triangleVertices, triangle1Color, false);

        triangle1Animator.SetColor(triangle1Color);
        triangle1Animator.SetOpacity(0);
        triangle1Animator.FadeTo(1.0f, playButtonFadeDuration, fDelay + redFrameScaleDuration);

        //Triangle2
        Color triangle2Color = ColorUtils.GetColorFromRGBAVector4(new Vector4(50, 50, 50, 255));

        GameObject triangle2Object = new GameObject("ShadowTriangle");
        TriangleAnimator triangle2Animator = triangle2Object.AddComponent<TriangleAnimator>();
        triangle2Object.transform.parent = playTrianglesObject.transform;
        triangle2Object.transform.localPosition = Vector3.zero;
        triangleMesh = triangle2Object.AddComponent<TriangleMesh>();
        triangleMesh.Init(Instantiate(m_playBannerMaterial));
        triangleMesh.Render(triangleVertices, triangle2Color, false);

        triangle2Animator.SetOpacity(0);
        triangle2Animator.FadeTo(1.0f, 0.1f, fDelay + redFrameScaleDuration + playButtonFadeDuration);
        triangle2Animator.TranslateTo(new Vector3(3, -3, 0), 0.3f, fDelay + redFrameScaleDuration + playButtonFadeDuration);
    }

    /**
     * Show credits and options button
     * **/
    public void ShowButtons(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

        //Options button
        GameObject optionsButtonObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
                                                                         new Vector2(128.0f, 128.0f),
                                                                         ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
                                                                         Color.black);

        optionsButtonObject.transform.parent = this.transform;

        GameObjectAnimator optionsButtonAnimator = optionsButtonObject.GetComponent<GameObjectAnimator>();
        Vector3 optionsButtonFinalPosition = new Vector3(-200.0f, -0.5f * screenSize.y + 150.0f, GUI_BUTTONS_Z_VALUE);

        //Credits button
        GameObject creditsButtonObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CREDITS_BUTTON,
                                                                         new Vector2(128.0f, 128.0f),
                                                                         ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
                                                                         Color.black);

        creditsButtonObject.transform.parent = this.transform;
        
        GameObjectAnimator creditsButtonAnimator = creditsButtonObject.GetComponent<GameObjectAnimator>();
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
}

