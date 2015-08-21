using UnityEngine;

public class GUIButton : MonoBehaviour
{
    protected UVQuad m_skin;

    public Vector2 m_size { get; set; }

    //In case button has to change textures between its on/off states
    public Material m_skinOnMaterial;
    public Material m_skinOffMaterial;

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
        ID_MENU_BUTTON,
        ID_RETRY_BUTTON,
        ID_HINTS_BUTTON,
        ID_BACK_TO_LEVELS_BUTTON,
        ID_CHAPTER_SELECTION_ARROW_PREVIOUS,
        ID_CHAPTER_SELECTION_ARROW_NEXT,

        //Action buttons
        ID_AXIS_SYMMETRY_TWO_SIDES,
        ID_AXIS_SYMMETRY_ONE_SIDE,
        ID_POINT_SYMMETRY,
        ID_MOVE_SHAPE,
        ID_OPERATION_ADD,
        ID_OPERATION_SUBSTRACT,
        ID_COLOR_FILTER
    }

    public GUIButtonID m_ID;

    public virtual void Init(Material skinMaterial, Color tintColor)
    {
        BaseQuad[] childrenQuads = this.GetComponentsInChildren<BaseQuad>();
        m_skin = (childrenQuads.Length > 0) ? (UVQuad) childrenQuads[0] : null;

        if (m_skin != null)
        {
            m_skin.Init(skinMaterial);

            TexturedQuadAnimator skinAnimator = m_skin.GetComponent<TexturedQuadAnimator>();
            skinAnimator.SetPosition(new Vector3(0,0,-2));
            skinAnimator.SetColor(tintColor);
        }
    }

    public static GUIButton FindInObjectChildrenForID(GameObject parentObject, GUIButtonID id)
    {
        GUIButton[] childButtons = parentObject.GetComponentsInChildren<GUIButton>();
        for (int iButtonIdx = 0; iButtonIdx != childButtons.Length; iButtonIdx++)
        {
            if (childButtons[iButtonIdx].m_ID == id)
            {
                return childButtons[iButtonIdx];
            }
        }

        return null;
    }

    /**
     * Set the size to all elements in this button
     * **/
    public virtual void SetSize(Vector2 size)
    {
        m_size = size;

        m_skin.GetComponent<TexturedQuadAnimator>().SetScale(GeometryUtils.BuildVector3FromVector2(size, 1));
    }

    /**
     * Tint the skin
     * **/
    public void SetSkinTintColor(Color skinTintColor)
    {
        TexturedQuadAnimator skinAnimator = m_skin.GetComponent<TexturedQuadAnimator>();
        skinAnimator.SetColor(skinTintColor);
    }

    /**
     * Tells if this button contains the specified point
     * **/
    public bool ContainsPoint(Vector2 point)
    {
        float minX = this.transform.position.x - 0.5f * m_size.x;
        float maxX = this.transform.position.x + 0.5f * m_size.x;
        float minY = this.transform.position.y - 0.5f * m_size.y;
        float maxY = this.transform.position.y + 0.5f * m_size.y;

        return (point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY);
    }

    /**
     * Set a material to the skin Quad
     * **/
    public void SetSkinMaterial(Material material)
    {
        if (m_skin != null)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = material;
            m_skin.GetComponent<MeshRenderer>().sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Clamp;
        }
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
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
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
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleMusic();

            PersistentDataManager persistentDataManager = GameObject.FindGameObjectWithTag("PersistentDataManager").GetComponent<PersistentDataManager>();
            persistentDataManager.SetMusicStatus(soundManager.m_musicActive);
        }
        else if (m_ID == GUIButtonID.ID_SOUND_BUTTON)
        {
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleSound();

            PersistentDataManager persistentDataManager = GameObject.FindGameObjectWithTag("PersistentDataManager").GetComponent<PersistentDataManager>();
            persistentDataManager.SetMusicStatus(soundManager.m_soundActive);
        }
        else if (m_ID == GUIButtonID.ID_CLOSE_OVERLAY_BUTTON)
        {
            guiManager.DismissSideButtonsOverlay();
        }
        else if (m_ID == GUIButtonID.ID_MENU_BUTTON)
        {
            //GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            //guiManager.ShowPauseWindow();
        }
        else if (m_ID == GUIButtonID.ID_RETRY_BUTTON)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false, 0.0f, 0.5f);
        }
        else if (m_ID == GUIButtonID.ID_HINTS_BUTTON)
        {
            Debug.Log("HINTS");
        }
        else if (m_ID == GUIButtonID.ID_BACK_TO_LEVELS_BUTTON)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, false, 0.0f, 1.0f);

            //GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            //guiManager.DismissPauseWindow();
        }
        else if (m_ID == GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            Chapters chapters = (Chapters) sceneManager.m_currentScene;

            if (chapters.DecrementChapterIndex())
            {
                chapters.DismissChapterSlot(false, true, 0.5f, 0.0f);
                chapters.UpdateBackgroundGradient();
                chapters.ShowChapterSlot(1.0f); //build a new container for diplaying chapter info
            }
        }
        else if (m_ID == GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            Chapters chapters = (Chapters)sceneManager.m_currentScene;

            if (chapters.IncrementChapterIndex())
            {
                chapters.DismissChapterSlot(false, true, 0.5f, 0.0f);
                chapters.UpdateBackgroundGradient();
                chapters.ShowChapterSlot(1.0f); //build a new container for diplaying chapter info
            }
        }
    }

    protected GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

        return m_guiManager;
    }
}
