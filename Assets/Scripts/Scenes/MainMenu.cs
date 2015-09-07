﻿using UnityEngine;

public class MainMenu : GUIScene
{
    public const float TITLE_Z_VALUE = -20.0f;
    public const float GUI_BUTTONS_Z_VALUE = -10.0f;

    //shared prefabs
    public GameObject m_textMeshPfb;
    public Material m_transpPositionColorMaterial;
    public GameObject m_circleMeshPfb;
    public GameObject m_texRoundedSegmentPfb;
    
    //title
    public Material m_titleContourMaterial;

    //buttons
    public GameObject m_playButton { get; set; }
    public GameObject m_optionsButtonObject { get; set; }
    public GameObject m_creditsButtonObject { get; set; }

    //play text
    private GameObject m_playTextObject;

    //variables to handle the generation of fading hexagon on play button
    private float m_hexagonAnimationStartInnerRadius;
    private float m_hexagonAnimationEndInnerRadius;
    private bool m_generatingFadingHexagons;
    private float m_hexagonAnimationDuration;
    private float m_hexagonAnimationElapsedTime;

    /**
     * Shows MainMenu with or without animation
     * **/
    public override void Show()
    {
        base.Show();

        //GetBackgroundRenderer().RenderForMainMenu(false, fDelay);
        //GetBackgroundRenderer().RenderForChapter(bAnimated, fDelay);

        ApplyGradientOnBackground();
        ShowTitle(true, 2.0f);
        ShowPlayButton();
        //ShowPlayBanner(bAnimated, fDelay + 3.5f);
        //ShowSideButtons(bAnimated, fDelay + 5.0f);
    }

    private void ApplyGradientOnBackground()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        Color gradientStartColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(18, 75, 89, 255));
        Color gradientEndColor = Color.gray;

        Gradient mainMenuGradient = new Gradient();
        mainMenuGradient.CreateLinear(new Vector2(0, 0.5f * screenSize.y),
                                        new Vector2(0, -0.5f * screenSize.y),
                                        gradientStartColor,
                                        gradientEndColor);

        GetBackgroundRenderer().ApplyGradient(mainMenuGradient, 
                                              0.02f,
                                              true,
                                              BackgroundTrianglesRenderer.GradientAnimationPattern.NONE,
                                              0.5f);
    }

    private void ShowTitle(bool bAnimated = true, float fDelay = 0.0f)
    {
        GameObject titleObject = new GameObject("Title");
        titleObject.transform.parent = this.transform;

        GameObjectAnimator titleAnimator = titleObject.AddComponent<GameObjectAnimator>();
        titleAnimator.SetPosition(new Vector3(0, 0, TITLE_Z_VALUE));

        BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        float triangleHeight = GetBackgroundRenderer().m_triangleHeight;
        float triangleEdgeLength = GetBackgroundRenderer().m_triangleEdgeLength;
        float yOffset = 3;

        //LETTER F
        GameObject letterFContourObject = new GameObject("LetterFContour");
        letterFContourObject.transform.parent = titleObject.transform;

        GameObjectAnimator letterFAnimator = letterFContourObject.AddComponent<GameObjectAnimator>();
        letterFAnimator.SetPosition(Vector3.zero);

        SegmentTree contourSegmentTree = letterFContourObject.AddComponent<SegmentTree>();

        contourSegmentTree.Init(letterFContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-9 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-9 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-7 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.0f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-7 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-7 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

        contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

        //set the node n+1 as children of the node n
        for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
        {
            if (i == contourSegmentTree.m_nodes.Count - 1)
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
            else
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
        }

        contourSegmentTree.BuildSegments(true);

        //LETTER L
        GameObject letterLContourObject = new GameObject("LetterLContour");
        letterLContourObject.transform.parent = titleObject.transform;

        GameObjectAnimator letterLAnimator = letterLContourObject.AddComponent<GameObjectAnimator>();
        letterLAnimator.SetPosition(Vector3.zero);

        contourSegmentTree = letterLContourObject.AddComponent<SegmentTree>();

        contourSegmentTree.Init(letterLContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-6 * triangleHeight, 0.5f * screenSize.y - (yOffset - 0.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-6 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-5 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-4 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-5 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-5 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

        contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

        //set the node n+1 as children of the node n
        for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
        {
            if (i == contourSegmentTree.m_nodes.Count - 1)
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
            else
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
        }

        contourSegmentTree.BuildSegments(true);

        //LETTER E
        GameObject letterEContourObject = new GameObject("LetterEContour");
        letterEContourObject.transform.parent = titleObject.transform;

        GameObjectAnimator letterEAnimator = letterEContourObject.AddComponent<GameObjectAnimator>();
        letterEAnimator.SetPosition(Vector3.zero);

        contourSegmentTree = letterLContourObject.AddComponent<SegmentTree>();

        contourSegmentTree.Init(letterEContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-3 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-3 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

        contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

        //set the node n+1 as children of the node n
        for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
        {
            if (i == contourSegmentTree.m_nodes.Count - 1)
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
            else
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
        }

        contourSegmentTree.BuildSegments(true);

        //LETTER E (flipped horizontally)
        GameObject letterFlippedEContourObject = new GameObject("LetterFlippedEContour");
        letterFlippedEContourObject.transform.parent = titleObject.transform;

        GameObjectAnimator letterFlippedEAnimator = letterFlippedEContourObject.AddComponent<GameObjectAnimator>();
        letterFlippedEAnimator.SetPosition(Vector3.zero);

        contourSegmentTree = letterFlippedEContourObject.AddComponent<SegmentTree>();

        contourSegmentTree.Init(letterFlippedEContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(3 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(3 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));

        contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

        //set the node n+1 as children of the node n
        for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
        {
            if (i == contourSegmentTree.m_nodes.Count - 1)
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
            else
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
        }

        contourSegmentTree.BuildSegments(true);

        //LETTER C
        GameObject letterCContourObject = new GameObject("LetterCContour");
        letterCContourObject.transform.parent = titleObject.transform;

        GameObjectAnimator letterCAnimator = letterCContourObject.AddComponent<GameObjectAnimator>();
        letterCAnimator.SetPosition(Vector3.zero);

        contourSegmentTree = letterCContourObject.AddComponent<SegmentTree>();

        contourSegmentTree.Init(letterCContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(5 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(5 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(9 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(6 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(6 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(9 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
        contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

        contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

        //set the node n+1 as children of the node n
        for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
        {
            if (i == contourSegmentTree.m_nodes.Count - 1)
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
            else
                contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
        }

        contourSegmentTree.BuildSegments(true);

        //fill inner triangles with different color
        //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(GetBackgroundRenderer().RenderMainMenuTitle), fDelay);
    }

    private void ShowPlayButton()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Center hexagon
        m_playButton = (GameObject)Instantiate(m_circleMeshPfb);
        m_playButton.name = "PlayButton";
        m_playButton.transform.parent = this.transform;

        BackgroundTriangle hexagonFarRightTriangle = GetBackgroundRenderer().GetNearestTriangleToScreenYPosition(-0.28f * screenSize.y,
                                                                                                                 BackgroundTrianglesRenderer.NUM_COLUMNS / 2,
                                                                                                                 180);
        m_playButton.transform.localPosition = new Vector3(0, hexagonFarRightTriangle.GetCenter().y, GUI_BUTTONS_Z_VALUE);

        //Set the correct material on it
        Material hexagonMaterial = Instantiate(m_transpPositionColorMaterial);

        //Build the mesh
        CircleMesh hexaMesh = m_playButton.GetComponent<CircleMesh>();
        hexaMesh.Init(hexagonMaterial);
        float hexagonThickness = 30.0f;
        float outerRadius = GetBackgroundRenderer().m_triangleEdgeLength;

        CircleMeshAnimator hexaMeshAnimator = m_playButton.GetComponent<CircleMeshAnimator>();
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(outerRadius - hexagonThickness, true);
        hexaMeshAnimator.SetOuterRadius(outerRadius, true);
        hexaMeshAnimator.SetColor(Color.white);

        //generate fading out hexagons
        m_generatingFadingHexagons = true;

        //Show text above button
        m_playTextObject = (GameObject)Instantiate(m_textMeshPfb);
        m_playTextObject.name = "PlayText";
        m_playTextObject.transform.parent = this.transform;
        m_playTextObject.transform.localPosition = m_playButton.transform.localPosition + new Vector3(0, 2.0f * GetBackgroundRenderer().m_triangleEdgeLength, 0);
        m_playTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("play");

        TextMeshAnimator playTextAnimator = m_playTextObject.GetComponent<TextMeshAnimator>();
        playTextAnimator.SetColor(Color.white);
        playTextAnimator.SetFontHeight(40);
    }

    public void DismissPlayButton()
    {
        m_playButton.GetComponent<CircleMeshAnimator>().FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        m_playTextObject.GetComponent<TextMeshAnimator>().FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        m_generatingFadingHexagons = false;
    }

    private void LaunchAnimatedHexagonOnPlayButton(float hexagonStartInnerRadius, float hexagonEndInnerRadius, float thickness, Color color, float fDuration)
    {
        m_hexagonAnimationDuration = fDuration;
        m_hexagonAnimationElapsedTime = 0;
        m_hexagonAnimationStartInnerRadius = hexagonStartInnerRadius;
        m_hexagonAnimationEndInnerRadius = hexagonEndInnerRadius;

        GameObject fadingHexagon = (GameObject)Instantiate(m_circleMeshPfb);
        fadingHexagon.name = "FadingHexagon";
        fadingHexagon.transform.parent = this.transform;
        fadingHexagon.transform.localPosition = m_playButton.transform.localPosition;

        CircleMesh fadingHexagonMesh = fadingHexagon.GetComponent<CircleMesh>();
        fadingHexagonMesh.Init(Instantiate(m_transpPositionColorMaterial));

        CircleMeshAnimator fadingHexagonAnimator = fadingHexagon.GetComponent<CircleMeshAnimator>();
        fadingHexagonAnimator.SetNumSegments(6, false);
        fadingHexagonAnimator.SetInnerRadius(hexagonStartInnerRadius, false);
        fadingHexagonAnimator.SetOuterRadius(hexagonStartInnerRadius + thickness, true);
        fadingHexagonAnimator.SetColor(color);
        fadingHexagonAnimator.AnimateInnerRadiusTo(hexagonEndInnerRadius, fDuration);
        fadingHexagonAnimator.AnimateOuterRadiusTo(hexagonEndInnerRadius + thickness, fDuration);
        fadingHexagonAnimator.SetOpacity(1);
        fadingHexagonAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    ///**
    // * Show the banner with the play button inside
    // * **/
    //public void ShowPlayBanner(bool bAnimated, float fDelay)
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    m_playBannerObject = (GameObject)Instantiate(m_playBannerPfb);
    //    m_playBannerObject.transform.parent = this.transform;
    //    m_playBannerObject.transform.localPosition = new Vector3(0, -240.0f, PLAY_BANNER_Z_VALUE);

    //    ColorQuad[] childFrames = m_playBannerObject.GetComponentsInChildren<ColorQuad>();
    //    GameObject redFrameObject = childFrames[0].gameObject;
    //    GameObject whiteFrameObject = childFrames[1].gameObject;
    //    GameObject darkFrameObject = childFrames[2].gameObject;

    //    GameObject playTextObject = m_playBannerObject.GetComponentInChildren<TextMesh>().gameObject;

    //    TriangleMesh[] childPlayTriangles = m_playBannerObject.GetComponentsInChildren<TriangleMesh>();
    //    GameObject frontTriangleObject = childPlayTriangles[0].gameObject;
    //    GameObject backTriangleObject = childPlayTriangles[1].gameObject;

    //    //Red frame
    //    float redFrameScaleDuration = 0.8f;
    //    Vector3 redFramePosition = new Vector3(-0.5f * screenSize.x, 0, 0);
    //    float redFrameHeight = 175.0f;

    //    ColorQuad redFrameColorQuad = redFrameObject.GetComponent<ColorQuad>();
    //    redFrameColorQuad.InitQuadMesh();

    //    MeshRenderer redFrameRenderer = redFrameObject.GetComponent<MeshRenderer>();
    //    redFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

    //    ColorQuadAnimator redFrameAnimator = redFrameObject.GetComponent<ColorQuadAnimator>();
    //    redFrameAnimator.SetColor(ColorUtils.GetColorFromRGBAVector4(new Vector4(219, 0, 47, 255)));
    //    redFrameAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
    //    redFrameAnimator.SetPosition(redFramePosition);
    //    redFrameAnimator.SetScale(new Vector3(0, redFrameHeight, 1));
    //    redFrameAnimator.ScaleTo(new Vector3(screenSize.x, redFrameHeight, 1), redFrameScaleDuration, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

    //    //White frame
    //    Vector3 whiteFramePosition = new Vector3(0, -0.5f * redFrameHeight, 0);
    //    float whiteFrameScaleDuration = 0.2f;

    //    ColorQuad whiteFrameColorQuad = whiteFrameObject.GetComponent<ColorQuad>();
    //    whiteFrameColorQuad.InitQuadMesh();

    //    MeshRenderer whiteFrameRenderer = whiteFrameObject.GetComponent<MeshRenderer>();
    //    whiteFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

    //    ColorQuadAnimator whiteFrameAnimator = whiteFrameObject.GetComponent<ColorQuadAnimator>();
    //    whiteFrameAnimator.SetColor(Color.white);
    //    whiteFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //    whiteFrameAnimator.SetPosition(whiteFramePosition);
    //    whiteFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
    //    whiteFrameAnimator.ScaleTo(new Vector3(screenSize.x, 4.0f, 1), whiteFrameScaleDuration, fDelay + redFrameScaleDuration, ValueAnimator.InterpolationType.SINUSOIDAL);

    //    //Dark frame
    //    Vector3 darkFramePosition = new Vector3(0, -0.5f * redFrameHeight, 1);
    //    float darkFrameScaleDuration = 0.4f;

    //    ColorQuad darkFrameColorQuad = darkFrameObject.GetComponent<ColorQuad>();
    //    darkFrameColorQuad.InitQuadMesh();

    //    MeshRenderer darkFrameRenderer = darkFrameObject.GetComponent<MeshRenderer>();
    //    darkFrameRenderer.sharedMaterial = Instantiate(m_playBannerMaterial);

    //    ColorQuadAnimator darkFrameAnimator = darkFrameObject.AddComponent<ColorQuadAnimator>();
    //    darkFrameAnimator.SetColor(Color.black);
    //    darkFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //    darkFrameAnimator.SetPosition(darkFramePosition);
    //    darkFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
    //    darkFrameAnimator.ScaleTo(new Vector3(screenSize.x, 12.0f, 1), darkFrameScaleDuration, fDelay + redFrameScaleDuration, ValueAnimator.InterpolationType.SINUSOIDAL);

    //    //Play text label
    //    float playButtonFadeDuration = 0.5f;

    //    playTextObject.transform.localPosition = new Vector3(0, 0, -1);
    //    playTextObject.transform.parent = m_playBannerObject.transform;

    //    TextMesh playTextMesh = playTextObject.GetComponent<TextMesh>();
    //    playTextMesh.text = LanguageUtils.GetTranslationForTag("play");
    //    Vector3 playTextMeshSize = playTextMesh.GetComponent<MeshRenderer>().bounds.size;

    //    TextMeshAnimator playTextAnimator = playTextObject.GetComponent<TextMeshAnimator>();
    //    playTextAnimator.SetOpacity(0);
    //    playTextAnimator.FadeTo(1.0f, playButtonFadeDuration, fDelay + redFrameScaleDuration);

    //    //Play triangles
    //    GameObject playTrianglesObject = frontTriangleObject.transform.parent.gameObject;
    //    playTrianglesObject.transform.localPosition = new Vector3(playTextMeshSize.x + 20.0f, 0, -1);
    //    playTrianglesObject.GetComponent<GameObjectAnimator>().FadeTo(1.0f, playButtonFadeDuration, fDelay + redFrameScaleDuration);


    //    float triangleScale = 20.0f;

    //    Vector3[] triangleVertices = new Vector3[3];
    //    triangleVertices[0] = triangleScale * new Vector2(-0.5f, 1.0f);
    //    triangleVertices[1] = triangleScale * new Vector2(-0.5f, -1.0f);
    //    triangleVertices[2] = triangleScale * new Vector2(0.5f, 0.0f);

    //    //Foreground triangle
    //    Color frontTriangleColor = Color.white;

    //    frontTriangleObject.transform.parent = frontTriangleObject.transform;
    //    frontTriangleObject.transform.localPosition = new Vector3(0, 0, -1);

    //    TriangleMesh triangleMesh = frontTriangleObject.GetComponent<TriangleMesh>();
    //    triangleMesh.Init(Instantiate(m_playBannerMaterial));
    //    triangleMesh.Render(triangleVertices, frontTriangleColor, false);

    //    TriangleAnimator frontTriangleAnimator = frontTriangleObject.AddComponent<TriangleAnimator>();
    //    frontTriangleAnimator.SetColor(frontTriangleColor);
    //    frontTriangleAnimator.SetOpacity(0);

    //    //Background triangle
    //    Color backTriangleColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(50, 50, 50, 255));

    //    triangleMesh = backTriangleObject.AddComponent<TriangleMesh>();
    //    triangleMesh.Init(Instantiate(m_playBannerMaterial));
    //    triangleMesh.Render(triangleVertices, backTriangleColor, false);

    //    TriangleAnimator backTriangleAnimator = backTriangleObject.AddComponent<TriangleAnimator>();
    //    backTriangleAnimator.SetOpacity(0);
    //    backTriangleAnimator.TranslateTo(new Vector3(3, -3, 0), 0.3f, fDelay + redFrameScaleDuration + playButtonFadeDuration);

    //    //Set the play banner as 'displayed' after animation ended
    //    m_isPlayBannerAnimating = true;
    //    m_playBannerAnimationDuration = fDelay + redFrameScaleDuration + playButtonFadeDuration;
    //    m_playBannerAnimationElapsedTime = 0;
    //}

    ///**
    // * Removes the play banner with animation and eventually destroy it at the end of this animation
    // * **/
    //public void DismissPlayBanner()
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    ColorQuad[] childFrames = m_playBannerObject.GetComponentsInChildren<ColorQuad>();
    //    GameObject redFrameObject = childFrames[0].gameObject;
    //    GameObject whiteFrameObject = childFrames[1].gameObject;
    //    GameObject darkFrameObject = childFrames[2].gameObject;

    //    GameObject playTextObject = m_playBannerObject.GetComponentInChildren<TextMesh>().gameObject;

    //    TriangleMesh[] childPlayTriangles = m_playBannerObject.GetComponentsInChildren<TriangleMesh>();
    //    GameObject frontTriangleObject = childPlayTriangles[0].gameObject;
    //    GameObject backTriangleObject = childPlayTriangles[1].gameObject;

    //    //white and dark frames
    //    whiteFrameObject.GetComponent<ColorQuadAnimator>().ScaleTo(new Vector3(screenSize.x, 0, 1), 0.2f, 0.0f);
    //    darkFrameObject.GetComponent<ColorQuadAnimator>().ScaleTo(new Vector3(screenSize.x, 0, 1), 0.4f, 0.0f);

    //    //red frame
    //    ColorQuadAnimator redFrameAnimator = redFrameObject.GetComponent<ColorQuadAnimator>();
    //    redFrameAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.0f));
    //    redFrameAnimator.ScaleTo(new Vector3(0, redFrameAnimator.transform.localScale.y, redFrameAnimator.transform.localScale.z), 0.8f, 0.4f);

    //    //Play triangles
    //    GameObject playTrianglesObject = frontTriangleObject.transform.parent.gameObject;
    //    playTrianglesObject.GetComponent<GameObjectAnimator>().FadeTo(0.0f, 0.4f, 0.0f);

    //    //Play text
    //    playTextObject.GetComponent<TextMeshAnimator>().FadeTo(0.0f, 0.4f, 0.0f);

    //    m_isPlayBannerDisplayed = false;
    //}

    ///**
    // * Show credits and options button
    // * **/
    //public void ShowButtons(bool bAnimated, float fDelay = 0.0f)
    //{
    //    return;
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

    //    //Options button
    //    m_optionsButtonObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
    //                                                                     new Vector2(128.0f, 128.0f),
    //                                                                     ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
    //                                                                     Color.black);
    //    m_optionsButtonObject.name = "OptionsButton";

    //    m_optionsButtonObject.transform.parent = this.transform;

    //    GameObjectAnimator optionsButtonAnimator = m_optionsButtonObject.GetComponent<GameObjectAnimator>();
    //    Vector3 optionsButtonFinalPosition = new Vector3(-200.0f, -0.5f * screenSize.y + 150.0f, GUI_BUTTONS_Z_VALUE);

    //    //Credits button
    //    m_creditsButtonObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CREDITS_BUTTON,
    //                                                                     new Vector2(128.0f, 128.0f),
    //                                                                     ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
    //                                                                     Color.black);
    //    m_creditsButtonObject.name = "CreditsButton";

    //    m_creditsButtonObject.transform.parent = this.transform;

    //    GameObjectAnimator creditsButtonAnimator = m_creditsButtonObject.GetComponent<GameObjectAnimator>();
    //    Vector3 creditsButtonFinalPosition = new Vector3(200.0f, -0.5f * screenSize.y + 150.0f, GUI_BUTTONS_Z_VALUE);

    //    if (bAnimated)
    //    {
    //        Vector3 optionsButtonFromPosition = new Vector3(-200.0f, -0.5f * screenSize.y + 50.0f/*- 150.0f */, GUI_BUTTONS_Z_VALUE);
    //        Vector3 creditsButtonFromPosition = new Vector3(200.0f, -0.5f * screenSize.y + 50.0f/* - 150.0f */, GUI_BUTTONS_Z_VALUE);

    //        optionsButtonAnimator.SetPosition(optionsButtonFromPosition);
    //        creditsButtonAnimator.SetPosition(creditsButtonFromPosition);
    //        optionsButtonAnimator.TranslateTo(optionsButtonFinalPosition, 0.4f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
    //        creditsButtonAnimator.TranslateTo(creditsButtonFinalPosition, 0.4f, fDelay + 0.12f, ValueAnimator.InterpolationType.SINUSOIDAL);

    //        optionsButtonAnimator.SetOpacity(0);
    //        creditsButtonAnimator.SetOpacity(0);
    //        optionsButtonAnimator.FadeTo(1.0f, 0.4f, fDelay);
    //        creditsButtonAnimator.FadeTo(1.0f, 0.4f, fDelay + 0.12f);
    //    }
    //    else
    //    {
    //        optionsButtonAnimator.SetPosition(optionsButtonFinalPosition);
    //        creditsButtonAnimator.SetPosition(creditsButtonFinalPosition);
    //    }
    //}

    //public void DismissButtons()
    //{
    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    GameObjectAnimator optionsButtonAnimator = m_optionsButtonObject.GetComponent<GameObjectAnimator>();
    //    GameObjectAnimator creditsButtonAnimator = m_creditsButtonObject.GetComponent<GameObjectAnimator>();

    //    Vector3 optionsButtonFinalPosition = new Vector3(-200.0f, -0.5f * screenSize.y - 150.0f, GUI_BUTTONS_Z_VALUE);
    //    Vector3 creditsButtonFinalPosition = new Vector3(200.0f, -0.5f * screenSize.y - 150.0f, GUI_BUTTONS_Z_VALUE);

    //    optionsButtonAnimator.TranslateTo(optionsButtonFinalPosition, 0.5f, 0, ValueAnimator.InterpolationType.SINUSOIDAL);
    //    creditsButtonAnimator.TranslateTo(creditsButtonFinalPosition, 0.5f, 0, ValueAnimator.InterpolationType.SINUSOIDAL);
    //}

    public void Update()
    {
        if (m_generatingFadingHexagons)
        {
            float dt = Time.deltaTime;

            m_hexagonAnimationElapsedTime += dt;

            if (m_hexagonAnimationElapsedTime > m_hexagonAnimationDuration)
            {
                float hexagonStartRadius = 0.8f * GetBackgroundRenderer().m_triangleEdgeLength;
                LaunchAnimatedHexagonOnPlayButton(hexagonStartRadius, 2 * hexagonStartRadius, 5, Color.white, 2.0f);
            }
        }
    }
}

