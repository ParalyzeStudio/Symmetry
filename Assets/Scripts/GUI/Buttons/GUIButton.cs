using UnityEngine;

public class GUIButton : MonoBehaviour
{
    public GameObject m_skinObject;
    protected UVQuad m_skin;

    public Vector2 m_size { get; set; }
    public Vector2 m_touchArea { get; set; }

    //Color that will tint the skin texture
    public Color m_skinTintColor { get; set; }

    public enum ButtonState
    {
        PRESSED = 1,
        IDLE,
        DISABLED
    }

    public ButtonState m_state { get; set; }

    private GUIManager m_guiManager;
    private SceneManager m_sceneManager;
    private CallFuncHandler m_callFuncHandler;
    private SoundManager m_soundManager;
    private LevelManager m_levelManager;
    private PersistentDataManager m_persistentDataManager;

    public enum GUIButtonID
    {
        NONE = 0,

        //Interface buttons
        ID_OPTIONS_BUTTON = 1,
        ID_CREDITS_BUTTON,
        ID_MUSIC_BUTTON,
        ID_SOUND_BUTTON,
        ID_RESET_BUTTON,
        ID_CLOSE_OVERLAY_BUTTON,
        ID_PAUSE_BUTTON,
        ID_RETRY_BUTTON,
        ID_HINTS_BUTTON,
        ID_BACK_TO_LEVELS_BUTTON,
        ID_CHAPTER_SELECTION_ARROW_PREVIOUS,
        ID_CHAPTER_SELECTION_ARROW_NEXT,        
        ID_UNSTACK_SYMMETRY,

        //Action buttons
        ID_AXIS_SYMMETRY_TWO_SIDES,
        ID_AXIS_SYMMETRY_ONE_SIDE,
        ID_POINT_SYMMETRY,
        ID_MOVE_SHAPE,
        ID_OPERATION_ADD,
        ID_OPERATION_SUBSTRACT,
        ID_COLOR_FILTER,

        //temporary debug
        ID_DEBUG_SKIP_LEVEL,
        ID_DEBUG_LEVEL1,
        ID_DEBUG_LEVEL2,
        ID_DEBUG_LEVEL3
    }

    public GUIButtonID m_ID;

    /**
     * Init this button mesh with optional material
     * **/
    public virtual void Init(Material skinMaterial = null)
    {
        m_skin = m_skinObject.GetComponent<UVQuad>();
        m_skin.Init(skinMaterial);

        TexturedQuadAnimator skinAnimator = m_skin.GetComponent<TexturedQuadAnimator>();
        skinAnimator.SetPosition(new Vector3(0, 0, -2));
        skinAnimator.SetColor(Color.white); //default white tint color
    }

    /**
     * Set a specific color to tint the button skin
     * **/
    public void SetTintColor(Color color)
    {
        TexturedQuadAnimator skinAnimator = m_skin.GetComponent<TexturedQuadAnimator>();
        skinAnimator.SetColor(color);
    }

    /**
     * Set the size to all elements in this button
     * **/
    public virtual void SetSize(Vector2 size)
    {
        m_size = size;
        m_touchArea = size;

        m_skin.GetComponent<TexturedQuadAnimator>().SetScale(GeometryUtils.BuildVector3FromVector2(size, 1));
    }

    /**
     * Specify a rectangle that can be different from m_size and that serves as a touch collider
     * **/
    public void SetTouchArea(Vector2 touchArea)
    {
        m_touchArea = touchArea;
    }

    /**
     * Tells if this button contains the specified point
     * **/
    public bool ContainsPoint(Vector2 point)
    {
        float minX = this.transform.position.x - 0.5f * m_touchArea.x;
        float maxX = this.transform.position.x + 0.5f * m_touchArea.x;
        float minY = this.transform.position.y - 0.5f * m_touchArea.y;
        float maxY = this.transform.position.y + 0.5f * m_touchArea.y;

        return (point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY);
    }

    /**
     * Set a material to the skin Quad
     * **/
    public void SetSkinMaterial(Material material)
    {
        MeshRenderer skinRenderer = m_skinObject.GetComponent<MeshRenderer>();
        skinRenderer.sharedMaterial = material;
        skinRenderer.sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Clamp;
    }

    /**
     * Callback called when button is pressed for the first time
     * **/
    public virtual bool OnPress()
    {
        if (m_state == ButtonState.IDLE)
        {
            m_state = ButtonState.PRESSED;
            return true;
        }

        //Tint the skin (which color is often white) a bit darker
        m_skin.SetTintColor(new Color(0.95f, 0.95f, 0.95f, 1.0f));

        return false;
    }

    /**
     * Callback called immediately after the button has been released
     * **/
    public virtual bool OnRelease()
    {
        if (m_state == ButtonState.PRESSED)
        {
            m_state = ButtonState.IDLE;
            return true;
        }

        //Reset to the default skin color
        m_skin.SetTintColor(Color.white);

        return false;
    }

    /**
     * Player clicked the GUI buttons
     * **/
    public virtual void OnClick()
    {
        GUIManager guiManager = GetGUIManager();
        guiManager.m_selectedSideButtonID = m_ID;

        if (m_ID == GUIButtonID.ID_OPTIONS_BUTTON)
        {
            if (!guiManager.m_sideButtonsOverlayDisplayed)
                guiManager.ShowSideButtonsOverlay();
            else
            {
                guiManager.SwitchOverlayContent(GUIManager.OverlayDisplayedContent.OPTIONS);
            }
        }
        else if (m_ID == GUIButtonID.ID_CREDITS_BUTTON)
        {
            if (!guiManager.m_sideButtonsOverlayDisplayed)
                guiManager.ShowSideButtonsOverlay();
            else
            {
                guiManager.SwitchOverlayContent(GUIManager.OverlayDisplayedContent.CREDITS);
            }
        }
        else if (m_ID == GUIButtonID.ID_MUSIC_BUTTON)
        {
            SoundManager soundManager = GetSoundManager();
            soundManager.ToggleMusic();

            PersistentDataManager persistentDataManager = GetPersistentDataManager();
            persistentDataManager.SetMusicStatus(soundManager.m_musicActive);
        }
        else if (m_ID == GUIButtonID.ID_SOUND_BUTTON)
        {
            SoundManager soundManager = GetSoundManager();
            soundManager.ToggleSound();

            PersistentDataManager persistentDataManager = GetPersistentDataManager();
            persistentDataManager.SetMusicStatus(soundManager.m_soundActive);
        }
        else if (m_ID == GUIButtonID.ID_CLOSE_OVERLAY_BUTTON)
        {
            guiManager.DismissSideButtonsOverlay();
        }
        else if (m_ID == GUIButtonID.ID_PAUSE_BUTTON)
        {
            Debug.Log("CLICK PAUSE");

            guiManager.ShowSideButtonsOverlay();
        }
        else if (m_ID == GUIButtonID.ID_RETRY_BUTTON)
        {
            SceneManager sceneManager = GetSceneManager();
            sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false);

            //the Show() method will be called in the next frame due to the way CallFuncHandler works.
            //Thus we can set the boolean in this frame and be sure it will be set before Show() is actually called
            LevelIntro levelIntroPendingScene = (LevelIntro)sceneManager.m_pendingScene;
            levelIntroPendingScene.m_loadingLevelIntroFromRetry = true;
        }
        else if (m_ID == GUIButtonID.ID_HINTS_BUTTON)
        {
            Debug.Log("CLICK HINTS");
        }
        else if (m_ID == GUIButtonID.ID_BACK_TO_LEVELS_BUTTON)
        {
            Debug.Log("CLICK LEVELS");
            SceneManager sceneManager = GetSceneManager();
            sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, false, 1.0f);

            //GUIManager guiManager = GetGUIManager();
            //guiManager.DismissPauseWindow();
        }
        else if (m_ID == GUIButtonID.ID_UNSTACK_SYMMETRY)
        {
            GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;
            gameScene.m_gameStack.Pop();
        }
        else if (m_ID == GUIButtonID.ID_DEBUG_SKIP_LEVEL)
        {
            Debug.Log("ID_DEBUG_SKIP_LEVEL");
            GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;
            gameScene.EndLevel(GameScene.GameStatus.VICTORY);
        }
        else if (m_ID == GUIButtonID.ID_DEBUG_LEVEL1)
        {
            Debug.Log("ID_DEBUG_LEVEL");
            LevelManager levelManager = GetLevelManager();
            levelManager.SetCurrentLevelAsDebugLevel(1);
            Levels levels = (Levels)GetSceneManager().m_currentScene;
            levels.OnClickLevelSlot(-1);
        }
        else if (m_ID == GUIButtonID.ID_DEBUG_LEVEL2)
        {
            Debug.Log("ID_DEBUG_LEVEL");
            LevelManager levelManager = GetLevelManager();
            levelManager.SetCurrentLevelAsDebugLevel(2);
            Levels levels = (Levels)GetSceneManager().m_currentScene;
            levels.OnClickLevelSlot(-2);
        }
        else if (m_ID == GUIButtonID.ID_DEBUG_LEVEL3)
        {
            Debug.Log("ID_DEBUG_LEVEL");
            LevelManager levelManager = GetLevelManager();
            levelManager.SetCurrentLevelAsDebugLevel(3);
            Levels levels = (Levels)GetSceneManager().m_currentScene;
            levels.OnClickLevelSlot(-3);
        }  
    }

    protected GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

        return m_guiManager;
    }

    protected SceneManager GetSceneManager()
    {
        if (m_sceneManager == null)
            m_sceneManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();

        return m_sceneManager;
    }

    protected SoundManager GetSoundManager()
    {
        if (m_soundManager == null)
            m_soundManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SoundManager>();

        return m_soundManager;
    }

    protected LevelManager GetLevelManager()
    {
        if (m_levelManager == null)
            m_levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

        return m_levelManager;
    }

    protected CallFuncHandler GetCallFuncHandler()
    {
        if (m_callFuncHandler == null)
            m_callFuncHandler = GameObject.FindGameObjectWithTag("GameController").GetComponent<CallFuncHandler>();

        return m_callFuncHandler;
    }

    protected PersistentDataManager GetPersistentDataManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PersistentDataManager>();

        return m_persistentDataManager;
    }
}
