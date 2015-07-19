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
    public GameObject m_hexagonPfb;

    //buttons prefabs
    public GameObject m_GUIDiamondShapeButtonPfb; //the prefab to create a diamond-shaped gui button with default skin, background and shadow
    public GameObject m_GUISkinOnlyButtonPfb; //the prefab to create a gui button with default skin only (no background, no shadow)
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    public GameObject m_pauseWindowPfb; //the prefab needed to instantiate the pause window
    public GameObject m_GUIFramePfb; //the prefab containing a background frame
    public GameObject m_backButtonPfb; //the prefab for the back button

    //public GameObject m_optionsWindow { get; set; } //the actual options window
    public GameObject m_sideButtonsOverlay { get; set; }
    public bool m_sideButtonsOverlayDisplayed { get; set; }

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
    public Material m_skinBack;
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
    public Material m_skinMenu;
    public Material m_skinColorChange;
    public Material m_GUIButtonBackground;
    public Material m_GUIButtonShadow;

    public Color[] m_framesColors;

    /**
     * Init some variables
     * **/
    public void Init()
    {
        //m_optionsWindow = null;
        BuildFrames();
    }

    /**
     * Create a GUI Button object with the specified 
     * -ID
     * -size
     * -background color if applicable
     * -shadow color if applicable
     * -in case a material could not be determined for the ID,
     *  force eventually a material for the skin (if the button has several materials for instance force one of them)
     * **/
    public GameObject CreateGUIButtonForID(GUIButton.GUIButtonID iID, Vector2 size, Color backgroundColor, Color shadowColor, Material forcedSkinMaterial = null)
    {
        GameObject buttonObject;
        if (false) //todo list diamond shapes button IDs here
        {
            buttonObject = (GameObject)Instantiate(m_GUIDiamondShapeButtonPfb);
        }
        else
            buttonObject = (GameObject)Instantiate(m_GUISkinOnlyButtonPfb);
            

        GUIButton button = buttonObject.GetComponent<GUIButton>();
        button.Init(forcedSkinMaterial);
        button.m_ID = iID;

        //Set the size of the button
        button.SetSize(size);

        //Set the color for background and shadow if applicable
        button.SetBackgroundColor(backgroundColor);
        button.SetShadowColor(shadowColor);
        
        //Materials for background and shadow
        Material clonedBackgroundMaterial = Instantiate(m_GUIButtonBackground);
        Material clonedShadowMaterial = Instantiate(m_GUIButtonShadow);

        //Set the relevant skin material for the specified button ID
        if (forcedSkinMaterial == null)
            button.SetSkinMaterial(GetClonedSkinMaterialForID(iID));
        button.SetBackgroundMaterial(clonedBackgroundMaterial);
        button.SetShadowMaterial(clonedShadowMaterial);

        //add an animator
        buttonObject.AddComponent<GameObjectAnimator>();

        return buttonObject;
    }

    public Material GetClonedSkinMaterialForID(GUIButton.GUIButtonID iID)
    {
        Material skinMaterial = null;
        if (iID == GUIButton.GUIButtonID.ID_BACK_BUTTON)
            skinMaterial = m_skinBack;
        else if (iID == GUIButton.GUIButtonID.ID_BACK_TO_LEVELS_BUTTON)
            skinMaterial = m_skinLevels;
        else if (iID == GUIButton.GUIButtonID.ID_CLOSE_OVERLAY_BUTTON)
            skinMaterial = m_skinCloseOverlay;
        else if (iID == GUIButton.GUIButtonID.ID_COLOR_CHANGE)
            skinMaterial = m_skinColorChange;
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
            skinMaterial = m_skinMenu;
        else if (iID == GUIButton.GUIButtonID.ID_RETRY_BUTTON)
            skinMaterial = m_skinRetry;
        else if (iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS || iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT)
            skinMaterial = m_skinSelectionArrow;
        else
            skinMaterial = m_skinOptions;

        return Instantiate(skinMaterial);
    }

    ///**
    // * Shows the options window on top of the current scene
    // * **/
    //public void ShowOptionsWindow()
    //{
    //    if (m_optionsWindow == null)
    //    {
    //        m_optionsWindow = (GameObject)Instantiate(m_optionsWindowPfb);
    //        m_optionsWindow.transform.parent = this.gameObject.transform;

    //        GUIButton[] childButtons = m_optionsWindow.GetComponentsInChildren<GUIButton>();
    //        for (int iButtonIdx = 0; iButtonIdx != childButtons.Length; iButtonIdx++)
    //        {
    //            GUIButton childButton = childButtons[iButtonIdx];

    //            if (childButton.m_ID == GUIButton.GUIButtonID.ID_SOUND_BUTTON)
    //            {
    //                SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    //                childButton.Init(soundManager.m_soundActive ? childButton.m_skinOnMaterial : childButton.m_skinOffMaterial);
    //            }
    //            else if (childButton.m_ID == GUIButton.GUIButtonID.ID_MUSIC_BUTTON)
    //            {
    //                SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    //                childButton.Init(soundManager.m_musicActive ? childButton.m_skinOnMaterial : childButton.m_skinOffMaterial);
    //            }
    //        }
    //    }
    //}

    ///**
    // * Dismisses the options window
    // * **/
    //public void DismissOptionsWindow()
    //{
    //    if (m_optionsWindow != null)
    //    {
    //        Destroy(m_optionsWindow);
    //        m_optionsWindow = null;
    //    }
    //}

    ///**
    // * Shows the pause window on top of the current scene
    // * **/
    //public void ShowPauseWindow()
    //{
    //    if (m_pauseWindow == null)
    //    {
    //        m_pauseWindow = (GameObject)Instantiate(m_pauseWindowPfb);
    //        m_pauseWindow.transform.parent = this.gameObject.transform;

    //        GUIButton[] childButtons = m_pauseWindow.GetComponentsInChildren<GUIButton>();
    //        for (int iButtonIdx = 0; iButtonIdx != childButtons.Length; iButtonIdx++)
    //        {
    //            GUIButton interfaceButton = childButtons[iButtonIdx];

    //            if (interfaceButton.m_ID == GUIButton.GUIButtonID.ID_SOUND_BUTTON)
    //            {
    //                SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    //                interfaceButton.Init(soundManager.m_soundActive ? interfaceButton.m_skinOnMaterial : interfaceButton.m_skinOffMaterial);
    //            }
    //            else if (interfaceButton.m_ID == GUIButton.GUIButtonID.ID_MUSIC_BUTTON)
    //            {
    //                SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    //                interfaceButton.Init(soundManager.m_musicActive ? interfaceButton.m_skinOnMaterial : interfaceButton.m_skinOffMaterial);
    //            }
    //            else
    //                interfaceButton.Init();
    //        }
    //    }
    //}

    ///**
    // * Dismisses the pause window
    // * **/
    //public void DismissPauseWindow()
    //{
    //    if (m_pauseWindow != null)
    //    {
    //        Destroy(m_pauseWindow);
    //        m_pauseWindow = null;
    //    }
    //}

    ///**
    // * Tells if the options window is displayed
    // * **/
    //public bool IsOptionsWindowShown()
    //{
    //    return m_optionsWindow != null;
    //}

    ///**
    // * Tells if the pause window is displayed
    // * **/
    //public bool IsPauseWindowShown()
    //{
    //    return m_pauseWindow != null;
    //}

    /**
     * Build frames that will be animated through scenes
     * **/
    public void BuildFrames()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
        GameObject topFrame = (GameObject)Instantiate(m_GUIFramePfb);
        GameObject middleFrame = (GameObject)Instantiate(m_GUIFramePfb);

        topFrame.transform.parent = framesHolder.transform;
        middleFrame.transform.parent = framesHolder.transform;

        //set the height of those frames to zero
        GameObjectAnimator topFrameAnimator = topFrame.GetComponent<GameObjectAnimator>();
        GameObjectAnimator middleFrameAnimator = middleFrame.GetComponent<GameObjectAnimator>();
        topFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));
        middleFrameAnimator.SetScale(new Vector3(screenSize.x, 0, 1));

        //init material for both frames
        middleFrame.GetComponent<TintColorMaterialAssignment>().InitMeshRendererMaterial();
        topFrame.GetComponent<TintColorMaterialAssignment>().InitMeshRendererMaterial();

        //set the color
        topFrameAnimator.SetColor(m_framesColors[0]);
        middleFrameAnimator.SetColor(m_framesColors[1]);
    }

    ///**
    // * Animate frames for the specified menu scene
    // * **/
    //public void AnimateFrames(SceneManager.DisplayContent contentToDisplay, float fDelay = 0.0f)
    //{
    //    return; //disable

    //    GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
    //    GUIFrameAnimator[] framesAnimators = framesHolder.GetComponentsInChildren<GUIFrameAnimator>();
    //    GUIFrameAnimator topFrameAnimator = framesAnimators[0];
    //    GUIFrameAnimator middleFrameAnimator = framesAnimators[1];

    //    Vector2 screenSize = ScreenUtils.GetScreenSize();

    //    if (contentToDisplay == SceneManager.DisplayContent.MENU)
    //    {
    //        //framesHolder.transform.position = new Vector3(0, 0, -5);
           
    //        //GameObject axesHolder = GameObject.FindGameObjectWithTag("MainMenuAxes");
    //        //float distanceToScreenTopBorder = 0.5f * screenSize.y - axesHolder.transform.position.y;
    //        //float topFrameHeight = 2 * distanceToScreenTopBorder;

            
    //        //topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //        //topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
    //        //Vector3 fromScale = new Vector3(screenSize.x, 0, 1);
    //        //Vector3 toScale = new Vector3(screenSize.x, topFrameHeight, 1);
    //        //topFrameAnimator.SetScale(fromScale);
    //        //topFrameAnimator.ScaleTo(toScale, 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

    //        //float middleFrameHeight = topFrameHeight + 0.23f * screenSize.y;
      
    //        //middleFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //        //middleFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, 0));
    //        //fromScale = new Vector3(screenSize.x, 0, 1);
    //        //toScale = new Vector3(screenSize.x, middleFrameHeight, 1);
    //        //middleFrameAnimator.SetScale(fromScale);
    //        //middleFrameAnimator.ScaleTo(toScale, 1.2f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
    //    }
    //    else if (contentToDisplay == SceneManager.DisplayContent.CHAPTERS || contentToDisplay == SceneManager.DisplayContent.LEVELS)
    //    {
    //        //animate top frame
    //        topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //        topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
    //        Vector3 topFrameSize = topFrameAnimator.GetGameObjectSize();
    //        topFrameAnimator.ScaleTo(new Vector3(screenSize.x, 0.144f * screenSize.y, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);

    //        //animate middle frame
    //        middleFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //        middleFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
    //        Vector3 middleFrameSize = middleFrameAnimator.GetGameObjectSize();
    //        middleFrameAnimator.ScaleTo(new Vector3(screenSize.x, 0, middleFrameSize.z), 0.6f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
    //    }
    //    else if (contentToDisplay == SceneManager.DisplayContent.LEVEL_INTRO)
    //    {
    //        //animate top frame
    //        topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //        topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
    //        Vector3 topFrameSize = topFrameAnimator.GetGameObjectSize();
    //        topFrameAnimator.ScaleTo(new Vector3(screenSize.x, screenSize.y, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
    //    }
    //    else if (contentToDisplay == SceneManager.DisplayContent.GAME)
    //    {
    //        //animate top frame
    //        topFrameAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
    //        topFrameAnimator.SetPosition(new Vector3(0, 0.5f * screenSize.y, -1));
    //        Vector3 topFrameSize = topFrameAnimator.GetGameObjectSize();
    //        topFrameAnimator.ScaleTo(new Vector3(screenSize.x, 0.144f * screenSize.y, topFrameSize.z), 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
    //        topFrameAnimator.ColorChangeTo(new Color(0.69f, 0.24f, 0.24f, 1), 0.8f, fDelay); 
    //    }
    //}

    /**
     * Show options, credits button on the left side of the screen
     * **/
    public void ShowSideButtons(bool bAnimated = true, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();
        BackgroundTrianglesRenderer bgRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        //Credits button
        GameObject creditsButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CREDITS_BUTTON,
                                                              new Vector2(128.0f, 128.0f),
                                                              ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
                                                              Color.black);
        creditsButtonObject.name = "CreditsButton";

        creditsButtonObject.transform.parent = this.transform;

        GameObjectAnimator creditsButtonAnimator = creditsButtonObject.GetComponent<GameObjectAnimator>();
        
        float creditsButtonXPosition = bgRenderer.m_triangleHeight - 0.5f * screenSize.x;
        float creditsButtonYPosition = bgRenderer.GetNearestTriangleToScreenYPosition(-0.39f * screenSize.y, 0, 0).GetCenter().y;
        Vector3 creditsButtonFinalPosition = new Vector3(creditsButtonXPosition, creditsButtonYPosition, SIDE_OVERLAY_Z_VALUE - 1);

        //Options button
        GameObject optionsButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_OPTIONS_BUTTON,
                                                              new Vector2(128.0f, 128.0f),
                                                              ColorUtils.GetColorFromRGBAVector4(new Color(255.0f, 0, 0, 255.0f)),
                                                              Color.black);
        optionsButtonObject.name = "OptionsButton";

        optionsButtonObject.transform.parent = this.transform;

        GameObjectAnimator optionsButtonAnimator = optionsButtonObject.GetComponent<GameObjectAnimator>();

        float optionsButtonXPosition = bgRenderer.m_triangleHeight - 0.5f * screenSize.x;
        float optionsButtonYPosition = creditsButtonYPosition + 2 * bgRenderer.m_triangleEdgeLength;
        Vector3 optionsButtonFinalPosition = new Vector3(optionsButtonXPosition, optionsButtonYPosition, SIDE_OVERLAY_Z_VALUE - 1);

        if (bAnimated)
        {
            Vector3 creditsButtonFromPosition = new Vector3(-0.5f * bgRenderer.m_triangleHeight - 0.5f * screenSize.x, creditsButtonYPosition, SIDE_OVERLAY_Z_VALUE - 1);
            creditsButtonAnimator.SetPosition(creditsButtonFromPosition);
            creditsButtonAnimator.TranslateTo(creditsButtonFinalPosition, 0.8f, fDelay + 0.4f, ValueAnimator.InterpolationType.SINUSOIDAL);

            Vector3 optionsButtonFromPosition = new Vector3(-0.5f * bgRenderer.m_triangleHeight - 0.5f * screenSize.x, optionsButtonYPosition, SIDE_OVERLAY_Z_VALUE - 1);
            optionsButtonAnimator.SetPosition(optionsButtonFromPosition);
            optionsButtonAnimator.TranslateTo(optionsButtonFinalPosition, 0.8f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else
        {
            creditsButtonAnimator.SetPosition(creditsButtonFinalPosition);
            optionsButtonAnimator.SetPosition(optionsButtonFinalPosition);
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
        m_sideButtonsOverlay.transform.parent = this.transform;
        
        GameObjectAnimator overlayAnimator = m_sideButtonsOverlay.AddComponent<GameObjectAnimator>();
        overlayAnimator.SetPosition(new Vector3(0, -screenSize.y, SIDE_OVERLAY_Z_VALUE));
        overlayAnimator.TranslateTo(new Vector3(0, 0, SIDE_OVERLAY_Z_VALUE), overlayTranslationDuration, 0.0f, ValueAnimator.InterpolationType.SINUSOIDAL);

        //background
        GameObject overlayBackgroundObject = (GameObject)Instantiate(m_colorQuadPfb);
        overlayBackgroundObject.name = "Background";
        overlayBackgroundObject.transform.parent = m_sideButtonsOverlay.transform;

        ColorQuad overlayBackgroundQuad = overlayBackgroundObject.GetComponent<ColorQuad>();
        overlayBackgroundQuad.InitQuadMesh();
        overlayBackgroundQuad.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(m_transpPositionColorMaterial);

        ColorQuadAnimator overlayBackgroundAnimator = overlayBackgroundObject.GetComponent<ColorQuadAnimator>();
        overlayBackgroundAnimator.SetPosition(Vector3.zero);
        overlayBackgroundAnimator.SetColor(ColorUtils.GetColorFromRGBAVector4(new Vector4(11, 12, 19, 247)));
        overlayBackgroundAnimator.SetScale(GeometryUtils.BuildVector3FromVector2(screenSize, 1));

        //separation line below title
        GameObject separationLineObject = (GameObject)Instantiate(m_colorQuadPfb);
        separationLineObject.name = "SeparationLine";
        separationLineObject.transform.parent = m_sideButtonsOverlay.transform;

        ColorQuad separationLineQuad = separationLineObject.GetComponent<ColorQuad>();
        separationLineQuad.InitQuadMesh();
        separationLineQuad.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(m_transpPositionColorMaterial);

        ColorQuadAnimator separationLineAnimator = separationLineObject.GetComponent<ColorQuadAnimator>();
        separationLineAnimator.SetPosition(new Vector3(0, 0.29f * screenSize.y, -1));
        separationLineAnimator.SetScale(new Vector3(0.73f * screenSize.x, 4, 1));
        separationLineAnimator.SetColor(Color.white);

        //close button
        GameObject closeButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_CLOSE_OVERLAY_BUTTON,
                                                            new Vector2(128, 128),
                                                            Color.black,
                                                            Color.black);
        closeButtonObject.name = "CloseButton";
        closeButtonObject.transform.parent = m_sideButtonsOverlay.transform;
        closeButtonObject.transform.localPosition = new Vector3(133 - 0.5f * screenSize.x, 0.5f * screenSize.y - 106, -1);


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
            overlayTitleObject.transform.parent = m_optionsContentHolder.transform;
            overlayTitleTextMesh.text = LanguageUtils.GetTranslationForTag("options");           
        }
        //credits content
        else if (displayedContent == OverlayDisplayedContent.CREDITS)
        {
            BuildCreditsContent();
            m_displayedContentHolder = m_creditsContentHolder;
            overlayTitleObject.transform.parent = m_creditsContentHolder.transform;
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
        m_optionsContentHolder.transform.parent = m_sideButtonsOverlay.transform;
        GameObjectAnimator optionsContentHolderAnimator = m_optionsContentHolder.AddComponent<GameObjectAnimator>();
        optionsContentHolderAnimator.SetPosition(new Vector3(0, 0, SIDE_OVERLAY_Z_VALUE - 1));

        //sound button
        Vector3 soundButtonPosition = new Vector3(-horizontalGapBetweenButtons, - 0.06f * screenSize.x, -1);
        GameObject soundButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_SOUND_BUTTON, new Vector2(100.0f, 100.0f), Color.black, Color.black);
        soundButtonObject.name = "SoundButton";
        soundButtonObject.transform.parent = m_optionsContentHolder.transform;
        GameObjectAnimator soundButtonAnimator = soundButtonObject.GetComponent<GameObjectAnimator>();
        soundButtonAnimator.SetPosition(soundButtonPosition);

        //music button
        Vector3 musicButtonPosition = new Vector3(0, -0.06f * screenSize.x, -1);
        GameObject musicButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_MUSIC_BUTTON, new Vector2(128.0f, 128.0f), Color.black, Color.black);
        musicButtonObject.name = "MusicButton";
        musicButtonObject.transform.parent = m_optionsContentHolder.transform;
        GameObjectAnimator musicButtonAnimator = musicButtonObject.GetComponent<GameObjectAnimator>();
        musicButtonAnimator.SetPosition(musicButtonPosition);

        //reset button
        Vector3 resetButtonPosition = new Vector3(horizontalGapBetweenButtons, -0.06f * screenSize.x, -1);
        GameObject resetButtonObject = CreateGUIButtonForID(GUIButton.GUIButtonID.ID_RESET_BUTTON, new Vector2(100.0f, 100.0f), Color.black, Color.black);
        resetButtonObject.name = "ResetButton";
        resetButtonObject.transform.parent = m_optionsContentHolder.transform;
        GameObjectAnimator resetButtonAnimator = resetButtonObject.GetComponent<GameObjectAnimator>();
        resetButtonAnimator.SetPosition(resetButtonPosition); 

        //Build hexagons around every button skin
        Material transpWhiteMaterial = Instantiate(m_transpPositionColorMaterial);
        GameObject soundButtonHexagon = (GameObject)Instantiate(m_hexagonPfb);
        soundButtonHexagon.name = "SoundButtonHexagon";
        soundButtonHexagon.transform.parent = m_optionsContentHolder.transform;
        HexagonMesh hexaMesh = soundButtonHexagon.GetComponent<HexagonMesh>();
        hexaMesh.Init(transpWhiteMaterial);
        HexagonMeshAnimator hexaMeshAnimator = soundButtonHexagon.GetComponent<HexagonMeshAnimator>();
        hexaMeshAnimator.SetInnerRadius(96);
        hexaMeshAnimator.SetOuterRadius(102, true);
        hexaMeshAnimator.SetColor(Color.white);
        hexaMeshAnimator.SetPosition(soundButtonPosition);

        GameObject musicButtonHexagon = (GameObject)Instantiate(m_hexagonPfb);
        musicButtonHexagon.name = "MusicButtonHexagon";
        musicButtonHexagon.transform.parent = m_optionsContentHolder.transform;
        hexaMesh = musicButtonHexagon.GetComponent<HexagonMesh>();
        hexaMesh.Init(transpWhiteMaterial);
        hexaMeshAnimator = musicButtonHexagon.GetComponent<HexagonMeshAnimator>();
        hexaMeshAnimator.SetInnerRadius(96);
        hexaMeshAnimator.SetOuterRadius(102, true);
        hexaMeshAnimator.SetColor(Color.white);
        hexaMeshAnimator.SetPosition(musicButtonPosition);

        GameObject resetButtonHexagon = (GameObject)Instantiate(m_hexagonPfb);
        resetButtonHexagon.name = "ResetButtonHexagon";
        resetButtonHexagon.transform.parent = m_optionsContentHolder.transform;
        hexaMesh = resetButtonHexagon.GetComponent<HexagonMesh>();
        hexaMesh.Init(transpWhiteMaterial);
        hexaMeshAnimator = resetButtonHexagon.GetComponent<HexagonMeshAnimator>();
        hexaMeshAnimator.SetInnerRadius(96);
        hexaMeshAnimator.SetOuterRadius(102, true);
        hexaMeshAnimator.SetColor(Color.white);
        hexaMeshAnimator.SetPosition(resetButtonPosition);

        //Text labels
        //music
        GameObject musicTextObject = (GameObject)Instantiate(m_textMeshPfb);
        musicTextObject.name = "MusicText";
        musicTextObject.transform.parent = m_optionsContentHolder.transform;

        musicTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("music");

        TextMeshAnimator musicTextAnimator = musicTextObject.GetComponent<TextMeshAnimator>();
        musicTextAnimator.SetPosition(musicButtonPosition - new Vector3(0, 190.0f, 0));
        musicTextAnimator.SetFontHeight(40);
        musicTextAnimator.SetColor(Color.white);

        //sound
        GameObject soundTextObject = (GameObject)Instantiate(m_textMeshPfb);
        soundTextObject.name = "SoundText";
        soundTextObject.transform.parent = m_optionsContentHolder.transform;

        soundTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("sound");

        TextMeshAnimator soundTextAnimator = soundTextObject.GetComponent<TextMeshAnimator>();
        soundTextAnimator.SetPosition(soundButtonPosition - new Vector3(0, 190.0f, 0));
        soundTextAnimator.SetFontHeight(40);
        soundTextAnimator.SetColor(Color.white);

        //reset
        GameObject resetTextObject = (GameObject)Instantiate(m_textMeshPfb);
        resetTextObject.name = "ResetText";
        resetTextObject.transform.parent = m_optionsContentHolder.transform;

        resetTextObject.GetComponent<TextMesh>().text = LanguageUtils.GetTranslationForTag("reset");

        TextMeshAnimator resetTextAnimator = resetTextObject.GetComponent<TextMeshAnimator>();
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
        m_creditsContentHolder.transform.parent = m_sideButtonsOverlay.transform;
        GameObjectAnimator creditsContentHolderAnimator = m_creditsContentHolder.AddComponent<GameObjectAnimator>();
        creditsContentHolderAnimator.SetPosition(new Vector3(0, 0, SIDE_OVERLAY_Z_VALUE - 1));
    }

    /**
     * Shows the back button that is shared across scenes
     * If it's already shown do nothing
     * **/
    public void ShowBackButton(float fDelay = 0.0f)
    {
        GUIButton backButton = GUIButton.FindInObjectChildrenForID(this.gameObject, GUIButton.GUIButtonID.ID_BACK_BUTTON);
        if (backButton == null)
        {
            Vector2 screenSize = ScreenUtils.GetScreenSize();

            GameObject backButtonObject = this.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_BACK_BUTTON,
                                                                    new Vector2(128.0f, 128.0f),
                                                                    Color.black,
                                                                    Color.black);
            backButtonObject.name = "BackButton";

            backButtonObject.transform.parent = this.gameObject.transform;
            backButtonObject.transform.localPosition = new Vector3(-0.5f * screenSize.x + 110.0f, 0.5f * screenSize.y - 90.0f, -20.0f);

            //Fade in button
            GameObjectAnimator backButtonAnimator = backButtonObject.GetComponent<GameObjectAnimator>();
            backButtonAnimator.SetOpacity(0);
            backButtonAnimator.FadeTo(1, 0.5f, fDelay);
        }
    }

    /**
     * Dismisses the back button
     * **/
    public void DismissBackButton()
    {
        //GUIButton backButton = GUIButton.FindInObjectChildrenForID(this.gameObject, GUIButton.GUIButtonID.ID_BACK_BUTTON);
        //if (backButton != null)
        //    Destroy(backButton.gameObject);
    }
}