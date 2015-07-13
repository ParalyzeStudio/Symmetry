using UnityEngine;

/**
 * Class that handles GUI on menus only
 * **/
public class GUIManager : MonoBehaviour
{
    public const float SIDE_BUTTONS_Z_VALUE = -10.0f;

    public GameObject m_colorQuadPfb;
    public Material m_transpPositionColorMaterial;
    public GameObject m_GUIDiamondShapeButtonPfb; //the prefab to create a diamond-shaped gui button with default skin, background and shadow
    public GameObject m_GUISkinOnlyButtonPfb; //the prefab to create a gui button with default skin only (no background, no shadow)
    public GameObject m_optionsWindowPfb; //the prefab needed to instantiate the options window
    public GameObject m_pauseWindowPfb; //the prefab needed to instantiate the pause window
    public GameObject m_GUIFramePfb; //the prefab containing a background frame
    public GameObject m_backButtonPfb; //the prefab for the back button
    //public GameObject m_optionsWindow { get; set; } //the actual options window
    public GameObject m_sideButtonsOverlay { get; set; }
    public GameObject m_pauseWindow { get; set; } //the pause menu we can access from game

    //which side button is selected when showing the overlay
    public GUIButton.GUIButtonID m_selectedSideButtonID { get; set; }

    //side buttons overlay elements
    public GameObject m_sideButtonsOverlayTitlePfb;
    private GameObject m_optionsContentHolder;
    private GameObject m_creditsContentHolder;

    //Materials for GUI buttons    
    public Material m_skinBack;
    public Material m_skinSelectionArrow;
    public Material m_skinCredits;
    public Material m_skinLevels;
    public Material m_skinOptions;
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

        if (iID == GUIButton.GUIButtonID.ID_OPTIONS_BUTTON ||
            iID == GUIButton.GUIButtonID.ID_CREDITS_BUTTON ||
            iID == GUIButton.GUIButtonID.ID_BACK_BUTTON ||
            iID == GUIButton.GUIButtonID.ID_HINTS_BUTTON ||
            iID == GUIButton.GUIButtonID.ID_RETRY_BUTTON ||
            iID == GUIButton.GUIButtonID.ID_MENU_BUTTON ||
            iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS ||
            iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT)
        {
            buttonObject = (GameObject)Instantiate(m_GUISkinOnlyButtonPfb);
        }
        else
            buttonObject = (GameObject)Instantiate(m_GUIDiamondShapeButtonPfb);

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
        else if (iID == GUIButton.GUIButtonID.ID_CLOSE_BUTTON)
            ;
        else if (iID == GUIButton.GUIButtonID.ID_COLOR_CHANGE)
            skinMaterial = m_skinColorChange;
        else if (iID == GUIButton.GUIButtonID.ID_CREDITS_BUTTON)
            skinMaterial = m_skinCredits;
        else if (iID == GUIButton.GUIButtonID.ID_HINTS_BUTTON)
            skinMaterial = m_skinHints;
        else if (iID == GUIButton.GUIButtonID.ID_OPTIONS_BUTTON)
            skinMaterial = m_skinOptions;
        else if (iID == GUIButton.GUIButtonID.ID_MENU_BUTTON)
            skinMaterial = m_skinMenu;
        else if (iID == GUIButton.GUIButtonID.ID_RETRY_BUTTON)
            skinMaterial = m_skinRetry;
        else if (iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS || iID == GUIButton.GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT)
            skinMaterial = m_skinSelectionArrow;

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

    /**
     * Shows the pause window on top of the current scene
     * **/
    public void ShowPauseWindow()
    {
        if (m_pauseWindow == null)
        {
            m_pauseWindow = (GameObject)Instantiate(m_pauseWindowPfb);
            m_pauseWindow.transform.parent = this.gameObject.transform;

            GUIButton[] childButtons = m_pauseWindow.GetComponentsInChildren<GUIButton>();
            for (int iButtonIdx = 0; iButtonIdx != childButtons.Length; iButtonIdx++)
            {
                GUIButton interfaceButton = childButtons[iButtonIdx];

                if (interfaceButton.m_ID == GUIButton.GUIButtonID.ID_SOUND_BUTTON)
                {
                    SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
                    interfaceButton.Init(soundManager.m_soundActive ? interfaceButton.m_skinOnMaterial : interfaceButton.m_skinOffMaterial);
                }
                else if (interfaceButton.m_ID == GUIButton.GUIButtonID.ID_MUSIC_BUTTON)
                {
                    SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
                    interfaceButton.Init(soundManager.m_musicActive ? interfaceButton.m_skinOnMaterial : interfaceButton.m_skinOffMaterial);
                }
                else
                    interfaceButton.Init();
            }
        }
    }

    /**
     * Dismisses the pause window
     * **/
    public void DismissPauseWindow()
    {
        if (m_pauseWindow != null)
        {
            Destroy(m_pauseWindow);
            m_pauseWindow = null;
        }
    }

    ///**
    // * Tells if the options window is displayed
    // * **/
    //public bool IsOptionsWindowShown()
    //{
    //    return m_optionsWindow != null;
    //}

    /**
     * Tells if the pause window is displayed
     * **/
    public bool IsPauseWindowShown()
    {
        return m_pauseWindow != null;
    }

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
        Vector3 creditsButtonFinalPosition = new Vector3(creditsButtonXPosition, creditsButtonYPosition, SIDE_BUTTONS_Z_VALUE);

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
        Vector3 optionsButtonFinalPosition = new Vector3(optionsButtonXPosition, optionsButtonYPosition, SIDE_BUTTONS_Z_VALUE);

        if (bAnimated)
        {
            Vector3 creditsButtonFromPosition = new Vector3(-0.5f * bgRenderer.m_triangleHeight - 0.5f * screenSize.x, creditsButtonYPosition, SIDE_BUTTONS_Z_VALUE);
            creditsButtonAnimator.SetPosition(creditsButtonFromPosition);
            creditsButtonAnimator.TranslateTo(creditsButtonFinalPosition, 0.8f, fDelay + 0.4f, ValueAnimator.InterpolationType.SINUSOIDAL);

            Vector3 optionsButtonFromPosition = new Vector3(-0.5f * bgRenderer.m_triangleHeight - 0.5f * screenSize.x, optionsButtonYPosition, SIDE_BUTTONS_Z_VALUE);
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
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        m_sideButtonsOverlay = new GameObject("SideButtonsOverlay");
        m_sideButtonsOverlay.transform.parent = this.transform;
        GameObjectAnimator overlayAnimator = m_sideButtonsOverlay.AddComponent<GameObjectAnimator>();

        //background
        GameObject overlayBackgroundObject = (GameObject)Instantiate(m_colorQuadPfb);
        overlayBackgroundObject.transform.parent = m_sideButtonsOverlay.transform;
        overlayBackgroundObject.transform.localScale = GeometryUtils.BuildVector3FromVector2(screenSize, 1);

        ColorQuad overlayBackgroundQuad = overlayBackgroundObject.GetComponent<ColorQuad>();
        overlayBackgroundQuad.InitQuadMesh();
        overlayBackgroundQuad.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(m_transpPositionColorMaterial);

        ColorQuadAnimator overlayBackgroundAnimator = overlayBackgroundObject.GetComponent<ColorQuadAnimator>();
        overlayBackgroundAnimator.SetColor(Color.black);        

        //separation line below title
        GameObject separationLineObject = (GameObject)Instantiate(m_colorQuadPfb);
        separationLineObject.transform.parent = m_sideButtonsOverlay.transform;
        separationLineObject.transform.localPosition = new Vector3(0, 0.21f * screenSize.y, -1);
        separationLineObject.transform.localScale = new Vector3(0.73f * screenSize.x, 8, 1);

        ColorQuad separationLineQuad = overlayBackgroundObject.GetComponent<ColorQuad>();
        overlayBackgroundQuad.InitQuadMesh();
        overlayBackgroundQuad.GetComponent<MeshRenderer>().sharedMaterial = Instantiate(m_transpPositionColorMaterial);

        ColorQuadAnimator separationLineAnimator = overlayBackgroundObject.GetComponent<ColorQuadAnimator>();
        overlayBackgroundAnimator.SetColor(Color.white);

        //build content holders
        m_optionsContentHolder = new GameObject("OptionsContentHolder");
        m_optionsContentHolder.transform.parent = m_sideButtonsOverlay.transform;
        m_optionsContentHolder.AddComponent<GameObjectAnimator>();

        m_creditsContentHolder = new GameObject("CreditsContentHolder");
        m_creditsContentHolder.transform.parent = m_sideButtonsOverlay.transform;
        m_creditsContentHolder.AddComponent<GameObjectAnimator>();

        ShowOverlayContent(false);
    }

    /**
     * Show the content for the currently selected side button ID
     * **/
    public void ShowOverlayContent(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Build title
        GameObject overlayTitleObject = (GameObject)Instantiate(m_sideButtonsOverlayTitlePfb);        
        overlayTitleObject.transform.localPosition = new Vector3(0, 0.38f * screenSize.y, -1);

        TextMesh overlayTitleTextMesh = overlayTitleObject.GetComponent<TextMesh>();
        TextMeshAnimator overlayTitleAnimator = overlayTitleObject.GetComponent<TextMeshAnimator>();
        overlayTitleAnimator.SetColor(Color.white);

        GameObject contentHolder = null;
        if (m_selectedSideButtonID == GUIButton.GUIButtonID.ID_OPTIONS_BUTTON)
        {
            contentHolder = m_optionsContentHolder;
            overlayTitleObject.transform.parent = m_optionsContentHolder.transform;
            overlayTitleTextMesh.text = LanguageUtils.GetTranslationForTag("options");
        }
        else if (m_selectedSideButtonID == GUIButton.GUIButtonID.ID_CREDITS_BUTTON)
        {
            contentHolder = m_creditsContentHolder;
            overlayTitleObject.transform.parent = m_creditsContentHolder.transform;
            overlayTitleTextMesh.text = LanguageUtils.GetTranslationForTag("about");
        }

        if (bAnimated)
        {
            GameObjectAnimator contentHolderAnimator = contentHolder.GetComponent<GameObjectAnimator>();
            contentHolderAnimator.SetPosition(new Vector3(screenSize.x, 0, 0));
            contentHolderAnimator.TranslateTo(Vector3.zero, 0.5f, 0.0f);
        }
    }

    /**
     * Dismiss the overlay content currently displayed
     * **/
    public void DismissCurrentlyDisplayedOverlayContent()
    {
        if (m_selectedSideButtonID == GUIButton.GUIButtonID.ID_CREDITS_BUTTON)
        {
            GameObjectAnimator contentHolderAnimator = m_creditsContentHolder.GetComponent<GameObjectAnimator>();
            contentHolderAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        }
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