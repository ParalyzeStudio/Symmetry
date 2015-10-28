using UnityEngine;

public class MainMenu : GUIScene
{
    public const float TITLE_Z_VALUE = -10.0f;
    public const float GUI_BUTTONS_Z_VALUE = -10.0f;

    //shared prefabs
    public GameObject m_textMeshPfb;
    public Material m_transpPositionColorMaterial;
    public GameObject m_circleMeshPfb;
    public GameObject m_texRoundedSegmentPfb;
    
    //title
    private GameObject m_titleObject;
    public GameObject m_titleLetterObject;
    //public Material m_titleContourMaterial;
    public Material m_titleMaterial;

    //buttons
    public GameObject m_playButton { get; set; }
    public GameObject m_optionsButtonObject { get; set; }
    public GameObject m_creditsButtonObject { get; set; }

    //play text
    private GameObject m_playTextObject;

    //variables to handle the generation of fading hexagon on play button
    //private float m_hexagonAnimationStartInnerRadius;
    //private float m_hexagonAnimationEndInnerRadius;
    private bool m_generatingFadingHexagons;
    private float m_hexagonAnimationDuration;
    private float m_hexagonAnimationElapsedTime;

    /**
     * Shows MainMenu with or without animation
     * **/
    public override void Show()
    {
        base.Show();

        ApplyGradientOnBackground();
        ShowTitle();
        ShowPlayButton();
    }

    private void ApplyGradientOnBackground()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        Color gradientStartColor = ColorUtils.GetColorFromRGBAVector4(new Vector4(93, 156, 18, 255));
        Color gradientEndColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);

        Gradient mainMenuGradient = new Gradient();
        mainMenuGradient.CreateLinear(new Vector2(0, 0.5f * screenSize.y),
                                        new Vector2(0, -2.5f * screenSize.y),
                                        gradientStartColor,
                                        gradientEndColor);

        GetBackgroundRenderer().ApplyGradient(mainMenuGradient, 
                                              0.02f,
                                              true,
                                              BackgroundTrianglesRenderer.GradientAnimationPattern.NONE,
                                              0.5f);
    }

    private void ShowTitle()
    {
        m_titleObject = new GameObject("Title");
        float globalYOffset = GetBackgroundRenderer().GetNearestTriangleToScreenYPosition(0).GetCenter().y;

        GameObjectAnimator titleAnimator = m_titleObject.AddComponent<GameObjectAnimator>();
        titleAnimator.SetParentTransform(this.transform);
        titleAnimator.SetPosition(new Vector3(0, globalYOffset, TITLE_Z_VALUE));

        Vector2 offset = new Vector2(0, 2);
        
        //Build data for each letter of the title
        //LETTER F
        TitleLetter.TitleTriangle[] trianglesData = new TitleLetter.TitleTriangle[9];
        trianglesData[0] = new TitleLetter.TitleTriangle(-9, 0, true);
        trianglesData[1] = new TitleLetter.TitleTriangle(-9, 1, false);
        trianglesData[2] = new TitleLetter.TitleTriangle(-9, 2, true);
        trianglesData[3] = new TitleLetter.TitleTriangle(-9, 3, false);
        trianglesData[4] = new TitleLetter.TitleTriangle(-9, 4, true);
        trianglesData[5] = new TitleLetter.TitleTriangle(-9, 5, false);
        trianglesData[6] = new TitleLetter.TitleTriangle(-8, 3, true);
        trianglesData[7] = new TitleLetter.TitleTriangle(-8, 5, true);
        trianglesData[8] = new TitleLetter.TitleTriangle(-8, 6, false);        

        GameObject letterFObject = (GameObject)Instantiate(m_titleLetterObject);
        letterFObject.name = "F";

        TitleLetter letterF = letterFObject.GetComponent<TitleLetter>();
        letterF.Init(trianglesData, Instantiate(m_titleMaterial), Instantiate(m_titleMaterial), offset);
        letterF.SetForegroundColor(Color.white);
        letterF.SetShadowColor(Color.black);

        TitleLetterAnimator letterFAnimator = letterFObject.GetComponent<TitleLetterAnimator>();
        letterFAnimator.SetParentTransform(m_titleObject.transform);
        letterFAnimator.SetPosition(Vector3.zero);

        //LETTER L
        trianglesData = new TitleLetter.TitleTriangle[8];
        trianglesData[0] = new TitleLetter.TitleTriangle(-6, 0, false);
        trianglesData[1] = new TitleLetter.TitleTriangle(-6, 1, true);
        trianglesData[2] = new TitleLetter.TitleTriangle(-6, 2, false);
        trianglesData[3] = new TitleLetter.TitleTriangle(-6, 3, true);
        trianglesData[4] = new TitleLetter.TitleTriangle(-6, 4, false);
        trianglesData[5] = new TitleLetter.TitleTriangle(-6, 5, true);
        trianglesData[6] = new TitleLetter.TitleTriangle(-6, 6, false);
        trianglesData[7] = new TitleLetter.TitleTriangle(-5, 0, true);

        GameObject letterLObject = (GameObject)Instantiate(m_titleLetterObject);
        letterLObject.name = "L";

        TitleLetter letterL = letterLObject.GetComponent<TitleLetter>();
        letterL.Init(trianglesData, Instantiate(m_titleMaterial), Instantiate(m_titleMaterial), offset);
        letterL.SetForegroundColor(Color.white);
        letterL.SetShadowColor(Color.black);

        TitleLetterAnimator letterLAnimator = letterLObject.GetComponent<TitleLetterAnimator>();
        letterLAnimator.SetParentTransform(m_titleObject.transform);
        letterLAnimator.SetPosition(Vector3.zero);


        //LETTER E
        trianglesData = new TitleLetter.TitleTriangle[10];
        trianglesData[0] = new TitleLetter.TitleTriangle(-3, 1, false);
        trianglesData[1] = new TitleLetter.TitleTriangle(-3, 2, true);
        trianglesData[2] = new TitleLetter.TitleTriangle(-3, 3, false);
        trianglesData[3] = new TitleLetter.TitleTriangle(-3, 4, true);
        trianglesData[4] = new TitleLetter.TitleTriangle(-3, 5, false);
        trianglesData[5] = new TitleLetter.TitleTriangle(-2, 0, false);
        trianglesData[6] = new TitleLetter.TitleTriangle(-2, 1, true);
        trianglesData[7] = new TitleLetter.TitleTriangle(-2, 3, true);
        trianglesData[8] = new TitleLetter.TitleTriangle(-2, 5, true);
        trianglesData[9] = new TitleLetter.TitleTriangle(-2, 6, false);

        GameObject letterEObject = (GameObject)Instantiate(m_titleLetterObject);
        letterEObject.name = "E";

        TitleLetter letterE = letterEObject.GetComponent<TitleLetter>();
        letterE.Init(trianglesData, Instantiate(m_titleMaterial), Instantiate(m_titleMaterial), offset);
        letterE.SetForegroundColor(Color.white);
        letterE.SetShadowColor(Color.black);

        TitleLetterAnimator letterEAnimator = letterEObject.GetComponent<TitleLetterAnimator>();
        letterEAnimator.SetParentTransform(m_titleObject.transform);
        letterEAnimator.SetPosition(Vector3.zero);

        //LETTER E FLIPPED
        trianglesData = new TitleLetter.TitleTriangle[10];
        trianglesData[0] = new TitleLetter.TitleTriangle(2, 0, true);
        trianglesData[1] = new TitleLetter.TitleTriangle(2, 1, false);
        trianglesData[2] = new TitleLetter.TitleTriangle(2, 3, false);
        trianglesData[3] = new TitleLetter.TitleTriangle(2, 5, false);
        trianglesData[4] = new TitleLetter.TitleTriangle(2, 6, true);
        trianglesData[5] = new TitleLetter.TitleTriangle(3, 1, true);
        trianglesData[6] = new TitleLetter.TitleTriangle(3, 2, false);
        trianglesData[7] = new TitleLetter.TitleTriangle(3, 3, true);
        trianglesData[8] = new TitleLetter.TitleTriangle(3, 4, false);
        trianglesData[9] = new TitleLetter.TitleTriangle(3, 5, true);

        GameObject letterFlippedEObject = (GameObject)Instantiate(m_titleLetterObject);
        letterFlippedEObject.name = "FlippedE";

        TitleLetter letterFlippedE = letterFlippedEObject.GetComponent<TitleLetter>();
        letterFlippedE.Init(trianglesData, Instantiate(m_titleMaterial), Instantiate(m_titleMaterial), offset);
        letterFlippedE.SetForegroundColor(Color.white);
        letterFlippedE.SetShadowColor(Color.black);

        TitleLetterAnimator letterFlippedEAnimator = letterFlippedEObject.GetComponent<TitleLetterAnimator>();
        letterFlippedEAnimator.SetParentTransform(m_titleObject.transform);
        letterFlippedEAnimator.SetPosition(Vector3.zero);

        //LETTER C
        trianglesData = new TitleLetter.TitleTriangle[15];
        trianglesData[0] = new TitleLetter.TitleTriangle(6, 1, false);
        trianglesData[1] = new TitleLetter.TitleTriangle(6, 2, true);
        trianglesData[2] = new TitleLetter.TitleTriangle(6, 3, false);
        trianglesData[3] = new TitleLetter.TitleTriangle(6, 4, true);
        trianglesData[4] = new TitleLetter.TitleTriangle(6, 5, false);
        trianglesData[5] = new TitleLetter.TitleTriangle(7, 0, false);
        trianglesData[6] = new TitleLetter.TitleTriangle(7, 1, true);
        trianglesData[7] = new TitleLetter.TitleTriangle(7, 5, true);
        trianglesData[8] = new TitleLetter.TitleTriangle(7, 6, false);
        trianglesData[9] = new TitleLetter.TitleTriangle(8, 0, true);
        trianglesData[10] = new TitleLetter.TitleTriangle(8, 1, false);
        trianglesData[11] = new TitleLetter.TitleTriangle(8, 5, false);
        trianglesData[12] = new TitleLetter.TitleTriangle(8, 6, true);
        trianglesData[13] = new TitleLetter.TitleTriangle(9, 1, true);
        trianglesData[14] = new TitleLetter.TitleTriangle(9, 5, true);

        GameObject letterCObject = (GameObject)Instantiate(m_titleLetterObject);
        letterCObject.name = "C";

        TitleLetter letterC = letterCObject.GetComponent<TitleLetter>();
        letterC.Init(trianglesData, Instantiate(m_titleMaterial), Instantiate(m_titleMaterial), offset);
        letterC.SetForegroundColor(Color.white);
        letterC.SetShadowColor(Color.black);

        TitleLetterAnimator letterCAnimator = letterCObject.GetComponent<TitleLetterAnimator>();
        letterCAnimator.SetParentTransform(m_titleObject.transform);
        letterCAnimator.SetPosition(Vector3.zero);
    }


    //private void ShowTitle(bool bAnimated = true, float fDelay = 0.0f)
    //{
    //    GameObject titleObject = new GameObject("Title");

    //    GameObjectAnimator titleAnimator = titleObject.AddComponent<GameObjectAnimator>();
    //    titleAnimator.SetParentTransform(this.transform);
    //    titleAnimator.SetPosition(new Vector3(0, 0, TITLE_Z_VALUE));

    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    float triangleHeight = GetBackgroundRenderer().m_triangleHeight;
    //    float triangleEdgeLength = GetBackgroundRenderer().m_triangleEdgeLength;
    //    float yOffset = 3;

    //    //LETTER F
    //    GameObject letterFContourObject = new GameObject("LetterFContour");

    //    GameObjectAnimator letterFAnimator = letterFContourObject.AddComponent<GameObjectAnimator>();
    //    letterFAnimator.SetParentTransform(titleObject.transform);
    //    letterFAnimator.SetPosition(Vector3.zero);

    //    SegmentTree contourSegmentTree = letterFContourObject.AddComponent<SegmentTree>();

    //    contourSegmentTree.Init(letterFContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-9 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-9 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-7 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.0f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-7 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-7 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

    //    contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

    //    //set the node n+1 as children of the node n
    //    for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
    //    {
    //        if (i == contourSegmentTree.m_nodes.Count - 1)
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
    //        else
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
    //    }

    //    contourSegmentTree.BuildSegments(true);

    //    //LETTER L
    //    GameObject letterLContourObject = new GameObject("LetterLContour");

    //    GameObjectAnimator letterLAnimator = letterLContourObject.AddComponent<GameObjectAnimator>();
    //    letterLAnimator.SetParentTransform(titleObject.transform);
    //    letterLAnimator.SetPosition(Vector3.zero);

    //    contourSegmentTree = letterLContourObject.AddComponent<SegmentTree>();

    //    contourSegmentTree.Init(letterLContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-6 * triangleHeight, 0.5f * screenSize.y - (yOffset - 0.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-6 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-5 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-4 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-5 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-5 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

    //    contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

    //    //set the node n+1 as children of the node n
    //    for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
    //    {
    //        if (i == contourSegmentTree.m_nodes.Count - 1)
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
    //        else
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
    //    }

    //    contourSegmentTree.BuildSegments(true);

    //    //LETTER E
    //    GameObject letterEContourObject = new GameObject("LetterEContour");

    //    GameObjectAnimator letterEAnimator = letterEContourObject.AddComponent<GameObjectAnimator>();
    //    letterEAnimator.SetParentTransform(titleObject.transform);
    //    letterEAnimator.SetPosition(Vector3.zero);

    //    contourSegmentTree = letterLContourObject.AddComponent<SegmentTree>();

    //    contourSegmentTree.Init(letterEContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-3 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-3 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-1 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

    //    contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

    //    //set the node n+1 as children of the node n
    //    for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
    //    {
    //        if (i == contourSegmentTree.m_nodes.Count - 1)
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
    //        else
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
    //    }

    //    contourSegmentTree.BuildSegments(true);

    //    //LETTER E (flipped horizontally)
    //    GameObject letterFlippedEContourObject = new GameObject("LetterFlippedEContour");

    //    GameObjectAnimator letterFlippedEAnimator = letterFlippedEContourObject.AddComponent<GameObjectAnimator>();
    //    letterFlippedEAnimator.SetParentTransform(titleObject.transform);
    //    letterFlippedEAnimator.SetPosition(Vector3.zero);

    //    contourSegmentTree = letterFlippedEContourObject.AddComponent<SegmentTree>();

    //    contourSegmentTree.Init(letterFlippedEContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(2 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(1 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(3 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(3 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));

    //    contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

    //    //set the node n+1 as children of the node n
    //    for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
    //    {
    //        if (i == contourSegmentTree.m_nodes.Count - 1)
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
    //        else
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
    //    }

    //    contourSegmentTree.BuildSegments(true);

    //    //LETTER C
    //    GameObject letterCContourObject = new GameObject("LetterCContour");

    //    GameObjectAnimator letterCAnimator = letterCContourObject.AddComponent<GameObjectAnimator>();
    //    letterCAnimator.SetParentTransform(titleObject.transform);
    //    letterCAnimator.SetPosition(Vector3.zero);

    //    contourSegmentTree = letterCContourObject.AddComponent<SegmentTree>();

    //    contourSegmentTree.Init(letterCContourObject, m_texRoundedSegmentPfb, 16.0f, m_titleContourMaterial, Color.white);

    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(5 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(5 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset + 3) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(9 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset + 2) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(6 * triangleHeight, 0.5f * screenSize.y - (yOffset + 1.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(6 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(8 * triangleHeight, 0.5f * screenSize.y - (yOffset + 0.5f) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(9 * triangleHeight, 0.5f * screenSize.y - (yOffset) * triangleEdgeLength)));
    //    contourSegmentTree.m_nodes.Add(new SegmentTreeNode(new Vector2(7 * triangleHeight, 0.5f * screenSize.y - (yOffset - 1) * triangleEdgeLength)));

    //    contourSegmentTree.m_nodes[0].SetAnimationStartNode(true);

    //    //set the node n+1 as children of the node n
    //    for (int i = 0; i != contourSegmentTree.m_nodes.Count; i++)
    //    {
    //        if (i == contourSegmentTree.m_nodes.Count - 1)
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[0]);
    //        else
    //            contourSegmentTree.m_nodes[i].AddChild(contourSegmentTree.m_nodes[i + 1]);
    //    }

    //    contourSegmentTree.BuildSegments(true);

    //    //fill inner triangles with different color
    //    //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(GetBackgroundRenderer().RenderMainMenuTitle), fDelay);
    //}

    private void ShowPlayButton()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Center hexagon
        m_playButton = (GameObject)Instantiate(m_circleMeshPfb);
        m_playButton.name = "PlayButton";

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
        hexaMeshAnimator.SetParentTransform(this.transform);
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(outerRadius - hexagonThickness, true);
        hexaMeshAnimator.SetOuterRadius(outerRadius, true);
        hexaMeshAnimator.SetColor(Color.white);

        //generate fading out hexagons
        m_generatingFadingHexagons = true;

        //Show text above button
        m_playTextObject = (GameObject)Instantiate(m_textMeshPfb);
        m_playTextObject.name = "PlayText";
        m_playTextObject.transform.localPosition = m_playButton.transform.localPosition + new Vector3(0, 2.0f * GetBackgroundRenderer().m_triangleEdgeLength, 0);
        m_playTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("play");

        TextMeshAnimator playTextAnimator = m_playTextObject.GetComponent<TextMeshAnimator>();
        playTextAnimator.SetParentTransform(this.transform);
        playTextAnimator.SetColor(Color.white);
        playTextAnimator.SetFontHeight(40);
    }

    public void DismissPlayButton()
    {
        m_playButton.GetComponent<CircleMeshAnimator>().FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        m_playTextObject.GetComponent<TextMeshAnimator>().FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        m_generatingFadingHexagons = false;
    }

    public void DismissTitle()
    {
        GameObjectAnimator titleAnimator = m_titleObject.GetComponent<GameObjectAnimator>();
        Vector3 titleCurrentPosition = titleAnimator.GetPosition();
        titleAnimator.TranslateTo(titleCurrentPosition + new Vector3(0, 400.0f, 0), 2.0f);
        titleAnimator.FadeTo(0.0f, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    private void LaunchAnimatedHexagonOnPlayButton(float hexagonStartInnerRadius, float hexagonEndInnerRadius, float thickness, Color color, float fDuration)
    {
        m_hexagonAnimationDuration = fDuration;
        m_hexagonAnimationElapsedTime = 0;

        GameObject fadingHexagon = (GameObject)Instantiate(m_circleMeshPfb);
        fadingHexagon.name = "FadingHexagon";

        CircleMesh fadingHexagonMesh = fadingHexagon.GetComponent<CircleMesh>();
        fadingHexagonMesh.Init(Instantiate(m_transpPositionColorMaterial));

        CircleMeshAnimator fadingHexagonAnimator = fadingHexagon.GetComponent<CircleMeshAnimator>();
        fadingHexagonAnimator.SetParentTransform(m_playButton.transform);
        fadingHexagonAnimator.SetPosition(Vector3.zero);
        fadingHexagonAnimator.SetNumSegments(6, false);
        fadingHexagonAnimator.SetInnerRadius(hexagonStartInnerRadius, false);
        fadingHexagonAnimator.SetOuterRadius(hexagonStartInnerRadius + thickness, true);
        fadingHexagonAnimator.SetColor(color);
        fadingHexagonAnimator.AnimateInnerRadiusTo(hexagonEndInnerRadius, fDuration);
        fadingHexagonAnimator.AnimateOuterRadiusTo(hexagonEndInnerRadius + thickness, fDuration);
        fadingHexagonAnimator.SetOpacity(1);
        fadingHexagonAnimator.FadeTo(0.0f, fDuration, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    protected override void DismissSelf()
    {
        //dismiss title
        DismissTitle();

        //dismiss play button
        DismissPlayButton();        
    }

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

