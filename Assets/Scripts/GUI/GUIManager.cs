using UnityEngine;

/**
 * Class that handles GUI on menus only
 * **/
public class GUIManager : MonoBehaviour
{
    public const float SIDE_OVERLAY_Z_VALUE = -20.0f;

    //shared prefabs
    public GameObject m_colorQuadPfb;
    public Material m_transpPositionColorMaterial;
    public GameObject m_textMeshPfb;
    public GameObject m_circleMeshPfb;
    public Material m_glow1Material;
    public GameObject m_texRoundedSegmentPfb;

    //buttons prefabs
    public GameObject m_GUIButtonPfb; //the prefab to create a gui button with default skin only (no background, no shadow)
    public GameObject m_actionButtonPfb; //the prefab to create a gui button with default skin only (no background, no shadow)
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    public GameObject m_pauseWindowPfb; //the prefab needed to instantiate the pause window
    public GameObject m_GUIFramePfb; //the prefab containing a background frame

    public Material m_plainWhiteMaterial { get; set; } //use this shared material to draw plain white meshes

    //public GameObject m_optionsWindow { get; set; } //the actual options window
    private GameObject m_sideButtonsHolder;
    public GameObject m_sideButtonsOverlay { get; set; }
    public bool m_sideButtonsOverlayDisplayed { get; set; }

    //global instances
    private BackgroundTrianglesRenderer m_backgroundRenderer;

    public enum OverlayDisplayedContent
    {
        NONE = 0,
        OPTIONS,
        CREDITS
    }
    public OverlayDisplayedContent m_overlayDisplayedContent { get; set; }
    //public GameObject m_pauseWindow { get; set; } //the pause menu we can access from game

    //which side button is selected when showing the overlay
    public GUIButton.GUIButtonID m_selectedSideButtonID { get; set; }

    //side buttons overlay elements
    public GameObject m_displayedContentHolder { get; set; }
    private GameObject m_optionsContentHolder;
    private GameObject m_creditsContentHolder;

    //Materials for GUI buttons
    public Material m_skinCloseOverlay;
    public Material m_skinSelectionArrow;
    public Material m_skinCredits;
    public Material m_skinLevels;
    public Material m_skinOptions;
    public Material m_skinMusic;
    public Material m_skinSound;
    public Material m_skinReset;
    public Material m_skinHints;
    public Material m_skinRetry;
    public Material m_skinPause;
    public Material m_skinDebugSkipLevel;

    //Materials for action buttons
    public Material m_skinSymmetryTwoSides;
    public Material m_skinSymmetryOneSide;
    public Material m_skinPointSymmetry;
    public Material m_skinMoveShape;
    public Material m_skinOperationAdd;
    public Material m_skinOperationSubstract;
    public Material m_skinColorFilter;
    public Material m_GUIButtonBackground;
    public Material m_GUIButtonShadow;

    public Color[] m_framesColors;

    /**
     * Init some variables
     * **/
    public void Init()
    {
        //m_optionsWindow = null;
        m_plainWhiteMaterial = Instantiate(m_transpPositionColorMaterial);
    }

    /**
     * Create a GUI Button object with the specified 
     * -ID
     * -size
     * -in case a material could not be determined for the ID,
     *  force eventually a material for the skin (if the button has several materials for instance force one of them)
     * **/
    public GameObject CreateGUIButtonForID(GUIButton.GUIButtonID iID, Vector2 size, Material forcedSkinMaterial = null)
    {
        GameObject buttonObject = (GameObject)Instantiate(m_GUIButtonPfb);

        //Set the relevant skin material for the specified button ID
        Material buttonSkinMaterial = (forcedSkinMaterial == null) ? GetClonedSkinMaterialForID(iID)  : forcedSkinMaterial;
        GUIButton button = buttonObject.GetComponent<GUIButton>();
        button.Init(buttonSkinMaterial);
        button.m_ID = iID;

        //Set the size of the button skin
        button.SetSize(size);

        return buttonObject;
    }

    /**
     * Same as previous method but for an action button
     * **/
    public GameObject CreateActionButton(Vector2 size, ActionButton.Location location, GUIButton.GUIButtonID[] childIDs)
    {
        GameObject buttonObject = (GameObject)Instantiate(m_actionButtonPfb);

        //Set the relevant skin material for the specified button ID
        ActionButton button = buttonObject.GetComponent<ActionButton>();
        button.Init(location, childIDs);

        //Set the size of the button skin
        button.SetSize(size);

        //add an animator
        buttonObject.AddComponent<GameObjectAnimator>();

        return buttonObject;
    }

    public Material GetClonedSkinMaterialForID(GUIButton.GUIButtonID iID)
    {
        Material skinMaterial = null;
        if (iID == GUIButton.GUIButtonID.ID_BACK_TO_LEVELS_BUTTON)
            skinMaterial = m_skinLevels;
        else if (iID == GUIButton.GUIButtonID.ID_CLOSE_OVERLAY_BUTTON)
            skinMaterial = m_skinCloseOverlay;        
        else if (iID == GUIButton.GUIButtonID.ID_CREDITS_BUTTON)
            skinMaterial = m_skinCredits;
        else if (iID == GUIButton.GUIButtonID.ID_HINTS_BUTTON)
            skinMaterial = m_skinHints;
        else if (iID == GUIButton.GUIButtonID.ID_OPTIONS_BUTTON)
            skinMaterial = m_skinOptions;
        else if (iID == GUIButton.GUIButtonID.ID_MUSIC_BUTTON)
            skinMaterial = m_skinMusic;
        else if (iID == GUIButton.GUIButtonID.ID_SOUND_BUTTON)
            skinMaterial = m_skinSound;
        else if (iID == GUIButton.GUIButtonID.ID_RESET_BUTTON)
            skinMaterial = m_skinReset;
        else if (iID == GUIButton.GUIButtonID.ID_MENU_BUTTON)
            skinMaterial = m_skinPause;
        else if (iID == GUIButton.GUIButtonID.ID_RETRY_BUTTON)
            skinMaterial = m_skinRetry;
        else if (iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS || iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT)
            skinMaterial = m_skinSelectionArrow;
        else if (iID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES)
            skinMaterial = m_skinSymmetryTwoSides;
        else if (iID == GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE)
            skinMaterial = m_skinSymmetryOneSide;
        else if (iID == GUIButton.GUIButtonID.ID_POINT_SYMMETRY)
            skinMaterial = m_skinPointSymmetry;
        else if (iID == GUIButton.GUIButtonID.ID_MOVE_SHAPE)
            skinMaterial = m_skinMoveShape;
        else if (iID == GUIButton.GUIButtonID.ID_OPERATION_ADD)
            skinMaterial = m_skinOperationAdd;
        else if (iID == GUIButton.GUIButtonID.ID_OPERATION_SUBSTRACT)
            skinMaterial = m_skinOperationSubstract;
        else if (iID == GUIButton.GUIButtonID.ID_COLOR_FILTER)
            skinMaterial = m_skinColorFilter;
        else if (iID == GUIButton.GUIButtonID.ID_DEBUG_SKIP_LEVEL)
            skinMaterial = m_skinDebugSkipLevel;
        else
            skinMaterial = m_skinOptions;

        if (skinMaterial == null)
            skinMaterial = m_skinCloseOverlay;

        return Instantiate(skinMaterial);
    }  

    /**
     * Show options, credits button on the left side of the screen
     * **/
    public void ShowSideButtons(bool bAnimated = true, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();

        m_sideButtonsHolder = new GameObject("SideButtonsHolder");

        GameObjectAnimator sideButtonsHolderAnimator = m_sideButtonsHolder.AddComponent<GameObjectAnimator>();
        sideButtonsHolderAnimator.SetParentTransform(this.transform);
        Vector2 sideButtonSize = new Vector2(1.7f * bgRenderer.m_triangleHeight, 1.7f * bgRenderer.m_triangleHeight);
              
        //Credits button
        GameObject creditsButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CREDITS_BUTTON,
                                                              sideButtonSize);
        creditsButtonObject.name = "CreditsButton";

        GameObjectAnimator creditsButtonAnimator = creditsButtonObject.GetComponent<GameObjectAnimator>();
        creditsButtonAnimator.SetParentTransform(m_sideButtonsHolder.transform);
        float creditsButtonYPosition = bgRenderer.GetNearestTriangleToScreenYPosition(-0.39f * screenSize.y, 0, 0).GetCenter().y;
        creditsButtonAnimator.SetPosition(new Vector3(0, creditsButtonYPosition, 0));

        //Options button
        GameObject optionsButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
                                                              sideButtonSize);
        optionsButtonObject.name = "OptionsButton";

        GameObjectAnimator optionsButtonAnimator = optionsButtonObject.GetComponent<GameObjectAnimator>();
        optionsButtonAnimator.SetParentTransform(m_sideButtonsHolder.transform);
        float optionsButtonYPosition = creditsButtonYPosition + 2 * bgRenderer.m_triangleEdgeLength;
        optionsButtonAnimator.SetPosition(new Vector3(0, optionsButtonYPosition, 0));

        if (bAnimated)
        {
            sideButtonsHolderAnimator.SetPosition(new Vector3(-2.0f * bgRenderer.m_triangleHeight - 0.5f * screenSize.x, 0, SIDE_OVERLAY_Z_VALUE - 1));
            sideButtonsHolderAnimator.TranslateTo(new Vector3(bgRenderer.m_triangleHeight - 0.5f * screenSize.x, 0, SIDE_OVERLAY_Z_VALUE - 1), 
                                                  0.8f, 
                                                  fDelay, 
                                                  ValueAnimator.InterpolationType.SINUSOIDAL);

            //Vector3 creditsButtonFromPosition = new Vector3(0, creditsButtonYPosition, 0);
            //creditsButtonAnimator.SetPosition(creditsButtonFromPosition);
            //creditsButtonAnimator.TranslateTo(creditsButtonFinalPosition, 0.8f, fDelay + 0.4f, ValueAnimator.InterpolationType.SINUSOIDAL);

            //Vector3 optionsButtonFromPosition = new Vector3(0, optionsButtonYPosition, 0);
            //optionsButtonAnimator.SetPosition(optionsButtonFromPosition);
            //optionsButtonAnimator.TranslateTo(optionsButtonFinalPosition, 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else
        {
            sideButtonsHolderAnimator.SetPosition(new Vector3(bgRenderer.m_triangleHeight - 0.5f * screenSize.x, 0, SIDE_OVERLAY_Z_VALUE - 1));
            //creditsButtonAnimator.SetPosition(creditsButtonFinalPosition);
            //optionsButtonAnimator.SetPosition(optionsButtonFinalPosition);
        }

        //Contour
        //GameObject contourObject = new GameObject("SideButtonsContour");

        //GameObjectAnimator contourAnimator = contourObject.AddComponent<GameObjectAnimator>();
        //contourAnimator.SetParentTransform(this.transform);
        //contourAnimator.SetPosition(new Vector3(0, 0, SIDE_OVERLAY_Z_VALUE));

        //SegmentTree contourTree = contourObject.AddComponent<SegmentTree>();
        //Color tintColor = ColorUtils.GetRGBAColorFromTSB(new Vector3(0, 0, 0.5f), 1);
        //contourTree.Init(contourObject, m_texRoundedSegmentPfb, 16.0f, Instantiate(m_glow1Material), tintColor);

        //contourTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-0.5f * screenSize.x, creditsButtonYPosition - 1.5f * bgRenderer.m_triangleEdgeLength)));
        //contourTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-0.5f * screenSize.x + 2 * bgRenderer.m_triangleHeight, creditsButtonYPosition - 0.5f * bgRenderer.m_triangleEdgeLength)));
        //contourTree.m_nodes.Add(new SegmentTreeNode(new Vector2(-0.5f * screenSize.x + 2 * bgRenderer.m_triangleHeight, creditsButtonYPosition + 3.5f * bgRenderer.m_triangleEdgeLength)));

        //for (int i = 0; i != contourTree.m_nodes.Count - 1; i++)
        //{
        //    contourTree.m_nodes[i].AddChild(contourTree.m_nodes[i + 1]);
        //}

        //contourTree.m_nodes[0].SetAnimationStartNode(true);

        //contourTree.BuildSegments(true);
    }

    public void DismissSideButtons(bool bAnimated = true)
    {
        if (m_sideButtonsHolder == null) //this case may happen when we start directly from a scene different than MainMenu scene (debug)
            return;

        GameObjectAnimator sideButtonsHolderAnimator = m_sideButtonsHolder.GetComponent<GameObjectAnimator>();
        if (bAnimated)
        {
            BackgroundTrianglesRenderer backgroundRenderer = GetBackgroundRenderer();
            Vector2 screenSize = ScreenUtils.GetScreenSize();
            sideButtonsHolderAnimator.TranslateTo(new Vector3(-2.0f * backgroundRenderer.m_triangleHeight - 0.5f * screenSize.x, 0, SIDE_OVERLAY_Z_VALUE - 1),
                                                  0.8f,
                                                  0.0f,
                                                  ValueAnimator.InterpolationType.LINEAR,
                                                  true);
        }
        else
        {
            Destroy(m_sideButtonsHolder);
        }
    }

    /**
     * Show the overlay containing all information the player would like to find by pressing any of the side buttons
     * **/
    public void ShowSideButtonsOverlay()
    {
        if (m_sideButtonsOverlayDisplayed)
            return;

        m_sideButtonsOverlayDisplayed = true;

        Vector2 screenSize = ScreenUtils.GetScreenSize();
        float overlayTranslationDuration = 0.8f;

        m_sideButtonsOverlay = new GameObject("SideButtonsOverlay");
        
        GameObjectAnimator overlayAnimator = m_sideButtonsOverlay.AddComponent<GameObjectAnimator>();
        overlayAnimator.SetParentTransform(this.transform);
        overlayAnimator.SetPosition(new Vector3(0, -screenSize.y, SIDE_OVERLAY_Z_VALUE));
        overlayAnimator.TranslateTo(new Vector3(0, 0, SIDE_OVERLAY_Z_VALUE), overlayTranslationDuration, 0.0f, ValueAnimator.InterpolationType.SINUSOIDAL);

        //background
        GameObject overlayBackgroundObject = (GameObject)Instantiate(m_colorQuadPfb);
        overlayBackgroundObject.name = "Background";

        ColorQuad overlayBackgroundQuad = overlayBackgroundObject.GetComponent<ColorQuad>();
        overlayBackgroundQuad.Init(Instantiate(m_transpPositionColorMaterial));

        ColorQuadAnimator overlayBackgroundAnimator = overlayBackgroundObject.GetComponent<ColorQuadAnimator>();
        overlayBackgroundAnimator.SetParentTransform(m_sideButtonsOverlay.transform);
        overlayBackgroundAnimator.SetPosition(Vector3.zero);
        overlayBackgroundAnimator.SetColor(ColorUtils.GetColorFromRGBAVector4(new Vector4(11, 12, 19, 247)));
        overlayBackgroundAnimator.SetScale(GeometryUtils.BuildVector3FromVector2(screenSize, 1));

        //separation line below title
        GameObject separationLineObject = (GameObject)Instantiate(m_colorQuadPfb);
        separationLineObject.name = "SeparationLine";

        ColorQuad separationLineQuad = separationLineObject.GetComponent<ColorQuad>();
        separationLineQuad.Init(Instantiate(m_transpPositionColorMaterial));

        ColorQuadAnimator separationLineAnimator = separationLineObject.GetComponent<ColorQuadAnimator>();
        separationLineAnimator.SetParentTransform(m_sideButtonsOverlay.transform);
        separationLineAnimator.SetPosition(new Vector3(0, 0.29f * screenSize.y, -1));
        separationLineAnimator.SetScale(new Vector3(0.73f * screenSize.x, 4, 1));
        separationLineAnimator.SetColor(Color.white);

        //close button
        GameObject closeButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CLOSE_OVERLAY_BUTTON,
                                                            new Vector2(128, 128));
        closeButtonObject.name = "CloseButton";

        GameObjectAnimator closeButtonAnimator = closeButtonObject.GetComponent<GameObjectAnimator>();
        closeButtonAnimator.SetParentTransform(m_sideButtonsOverlay.transform);
        closeButtonAnimator.SetPosition(new Vector3(133 - 0.5f * screenSize.x, 0.5f * screenSize.y - 106, -1));


        if (m_selectedSideButtonID == GUIButton.GUIButtonID.ID_OPTIONS_BUTTON)
            ShowOverlayContent(OverlayDisplayedContent.OPTIONS, false);
        else if (m_selectedSideButtonID == GUIButton.GUIButtonID.ID_CREDITS_BUTTON)
            ShowOverlayContent(OverlayDisplayedContent.CREDITS, false);        
    }

    /**
     * Dismiss the overlay that has been shown after clicking on one of the side buttons
     * **/
    public void DismissSideButtonsOverlay()
    {
        if (!m_sideButtonsOverlayDisplayed)
            return;

        m_sideButtonsOverlayDisplayed = false;
        m_selectedSideButtonID = GUIButton.GUIButtonID.NONE;

        Vector2 screenSize = ScreenUtils.GetScreenSize();
        float overlayTranslationDuration = 0.8f;

        GameObjectAnimator overlayAnimator = m_sideButtonsOverlay.GetComponent<GameObjectAnimator>();
        overlayAnimator.TranslateTo(new Vector3(0, -screenSize.y, SIDE_OVERLAY_Z_VALUE), overlayTranslationDuration, 0.0f, ValueAnimator.InterpolationType.SINUSOIDAL, true);
    }

    /**
     * Show the content for the currently selected side button ID
     * **/
    public void ShowOverlayContent(OverlayDisplayedContent displayedContent, bool bAnimated, float fDelay = 0.0f)
    {  
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Build title
        GameObject overlayTitleObject = (GameObject)Instantiate(m_textMeshPfb);

        TextMesh overlayTitleTextMesh = overlayTitleObject.GetComponent<TextMesh>();
        TextMeshAnimator overlayTitleAnimator = overlayTitleObject.GetComponent<TextMeshAnimator>();
        overlayTitleAnimator.SetColor(Color.white);
        overlayTitleAnimator.SetFontHeight(60);

        //options content
        if (displayedContent == OverlayDisplayedContent.OPTIONS)
        {
            BuildOptionsContent();
            m_displayedContentHolder = m_optionsContentHolder;
            overlayTitleAnimator.SetParentTransform(m_optionsContentHolder.transform);
            overlayTitleTextMesh.text = LanguageUtils.GetTranslationForTag("options");           
        }
        //credits content
        else if (displayedContent == OverlayDisplayedContent.CREDITS)
        {
            BuildCreditsContent();
            m_displayedContentHolder = m_creditsContentHolder;
            overlayTitleAnimator.SetParentTransform(m_creditsContentHolder.transform);
            overlayTitleTextMesh.text = LanguageUtils.GetTranslationForTag("about");
        }
        
        //set the position after setting the parent transform
        overlayTitleAnimator.SetPosition(new Vector3(0, 0.377f * screenSize.y, -1));

        GameObjectAnimator contentHolderAnimator = m_displayedContentHolder.GetComponent<GameObjectAnimator>();
        if (bAnimated)
        {            
            contentHolderAnimator.SetPosition(new Vector3(screenSize.x, 0, 0));
            contentHolderAnimator.TranslateTo(Vector3.zero, 0.5f, 0.0f);
        }
    }

    /**
     * Dismiss the overlay content currently displayed
     * **/
    public void DismissCurrentlyDisplayedOverlayContent()
    {
        GameObjectAnimator contentHolderAnimator = m_displayedContentHolder.GetComponent<GameObjectAnimator>();
        contentHolderAnimator.FadeTo(0.0f, 0.2f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    public void SwitchOverlayContent(OverlayDisplayedContent newlyDisplayedContent)
    {
        DismissCurrentlyDisplayedOverlayContent();
        ShowOverlayContent(newlyDisplayedContent, true);        
    }

    /**
     * Build content for options screen
     * **/
    private void BuildOptionsContent()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        float horizontalGapBetweenButtons = 394.0f;

        m_optionsContentHolder = new GameObject("OptionsContentHolder");
        GameObjectAnimator optionsContentHolderAnimator = m_optionsContentHolder.AddComponent<GameObjectAnimator>();
        optionsContentHolderAnimator.SetParentTransform(m_sideButtonsOverlay.transform);
        optionsContentHolderAnimator.SetPosition(new Vector3(0, 0, SIDE_OVERLAY_Z_VALUE - 1));

        //sound button
        Vector3 soundButtonPosition = new Vector3(-horizontalGapBetweenButtons, - 0.06f * screenSize.x, -1);
        GameObject soundButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_SOUND_BUTTON, new Vector2(100.0f, 100.0f));
        soundButtonObject.name = "SoundButton";
        GameObjectAnimator soundButtonAnimator = soundButtonObject.GetComponent<GameObjectAnimator>();
        soundButtonAnimator.SetParentTransform(m_optionsContentHolder.transform);
        soundButtonAnimator.SetPosition(soundButtonPosition);

        //music button
        Vector3 musicButtonPosition = new Vector3(0, -0.06f * screenSize.x, -1);
        GameObject musicButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_MUSIC_BUTTON, new Vector2(128.0f, 128.0f));
        musicButtonObject.name = "MusicButton";
        GameObjectAnimator musicButtonAnimator = musicButtonObject.GetComponent<GameObjectAnimator>();
        musicButtonAnimator.SetParentTransform(m_optionsContentHolder.transform);
        musicButtonAnimator.SetPosition(musicButtonPosition);

        //reset button
        Vector3 resetButtonPosition = new Vector3(horizontalGapBetweenButtons, -0.06f * screenSize.x, -1);
        GameObject resetButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_RESET_BUTTON, new Vector2(100.0f, 100.0f));
        resetButtonObject.name = "ResetButton";
        GameObjectAnimator resetButtonAnimator = resetButtonObject.GetComponent<GameObjectAnimator>();
        resetButtonAnimator.SetParentTransform(m_optionsContentHolder.transform);
        resetButtonAnimator.SetPosition(resetButtonPosition); 

        //Build hexagons around every button skin
        Material transpWhiteMaterial = Instantiate(m_transpPositionColorMaterial);
        GameObject soundButtonHexagon = (GameObject)Instantiate(m_circleMeshPfb);
        soundButtonHexagon.name = "SoundButtonHexagon";
        CircleMesh hexaMesh = soundButtonHexagon.GetComponent<CircleMesh>();
        hexaMesh.Init(transpWhiteMaterial);
        CircleMeshAnimator hexaMeshAnimator = soundButtonHexagon.GetComponent<CircleMeshAnimator>();
        hexaMeshAnimator.SetParentTransform(m_optionsContentHolder.transform);
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(96, false);
        hexaMeshAnimator.SetOuterRadius(102, true);
        hexaMeshAnimator.SetColor(Color.white);
        hexaMeshAnimator.SetPosition(soundButtonPosition);

        GameObject musicButtonHexagon = (GameObject)Instantiate(m_circleMeshPfb);
        musicButtonHexagon.name = "MusicButtonHexagon";
        hexaMesh = musicButtonHexagon.GetComponent<CircleMesh>();
        hexaMesh.Init(transpWhiteMaterial);
        hexaMeshAnimator = musicButtonHexagon.GetComponent<CircleMeshAnimator>();
        hexaMeshAnimator.SetParentTransform(m_optionsContentHolder.transform);
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(96, false);
        hexaMeshAnimator.SetOuterRadius(102, true);
        hexaMeshAnimator.SetColor(Color.white);
        hexaMeshAnimator.SetPosition(musicButtonPosition);

        GameObject resetButtonHexagon = (GameObject)Instantiate(m_circleMeshPfb);
        resetButtonHexagon.name = "ResetButtonHexagon";
        hexaMesh = resetButtonHexagon.GetComponent<CircleMesh>();
        hexaMesh.Init(transpWhiteMaterial);
        hexaMeshAnimator = resetButtonHexagon.GetComponent<CircleMeshAnimator>();
        hexaMeshAnimator.SetParentTransform(m_optionsContentHolder.transform);
        hexaMeshAnimator.SetNumSegments(6, false);
        hexaMeshAnimator.SetInnerRadius(96, false);
        hexaMeshAnimator.SetOuterRadius(102, true);
        hexaMeshAnimator.SetColor(Color.white);
        hexaMeshAnimator.SetPosition(resetButtonPosition);

        //Text labels
        //music
        GameObject musicTextObject = (GameObject)Instantiate(m_textMeshPfb);
        musicTextObject.name = "MusicText";

        musicTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("music");

        TextMeshAnimator musicTextAnimator = musicTextObject.GetComponent<TextMeshAnimator>();
        musicTextAnimator.SetParentTransform(m_optionsContentHolder.transform);
        musicTextAnimator.SetPosition(musicButtonPosition - new Vector3(0, 190.0f, 0));
        musicTextAnimator.SetFontHeight(40);
        musicTextAnimator.SetColor(Color.white);

        //sound
        GameObject soundTextObject = (GameObject)Instantiate(m_textMeshPfb);
        soundTextObject.name = "SoundText";

        soundTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("sound");

        TextMeshAnimator soundTextAnimator = soundTextObject.GetComponent<TextMeshAnimator>();
        soundTextAnimator.SetParentTransform(m_optionsContentHolder.transform);
        soundTextAnimator.SetParentTransform(m_optionsContentHolder.transform);
        soundTextAnimator.SetPosition(soundButtonPosition - new Vector3(0, 190.0f, 0));
        soundTextAnimator.SetFontHeight(40);
        soundTextAnimator.SetColor(Color.white);

        //reset
        GameObject resetTextObject = (GameObject)Instantiate(m_textMeshPfb);
        resetTextObject.name = "ResetText";

        resetTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("reset");

        TextMeshAnimator resetTextAnimator = resetTextObject.GetComponent<TextMeshAnimator>();
        resetTextAnimator.SetParentTransform(m_optionsContentHolder.transform);
        resetTextAnimator.SetPosition(resetButtonPosition - new Vector3(0, 190.0f, 0));
        resetTextAnimator.SetFontHeight(40);
        resetTextAnimator.SetColor(Color.white);
    }

    /**
     * Build content for credits screen
     * **/
    private void BuildCreditsContent()
    {
        m_creditsContentHolder = new GameObject("CreditsContentHolder");
        GameObjectAnimator creditsContentHolderAnimator = m_creditsContentHolder.AddComponent<GameObjectAnimator>();
        creditsContentHolderAnimator.SetParentTransform(m_sideButtonsOverlay.transform);
        creditsContentHolderAnimator.SetPosition(new Vector3(0, 0, SIDE_OVERLAY_Z_VALUE - 1));
    }

    private BackgroundTrianglesRenderer GetBackgroundRenderer()
    {
        if (m_backgroundRenderer == null)
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundTrianglesRenderer>();

        return m_backgroundRenderer;
    }
}