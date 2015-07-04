using UnityEngine;

public class GUIButton : MonoBehaviour
{
    protected BaseQuad m_shadow;
    protected BaseQuad m_background;
    protected UVQuad m_skin;

    public Vector2 m_size { get; set; }

    //In case button has to change textures between its on/off states
    public Material m_skinOnMaterial;
    public Material m_skinOffMaterial;

    public enum ButtonState
    {
        PRESSED = 1,
        IDLE,
        DISABLED
    }

    public ButtonState m_state { get; set; }

    public enum GUIButtonID
    {
        //Interface buttons
        ID_OPTIONS_BUTTON = 1,
        ID_CREDITS_BUTTON,
        ID_MUSIC_BUTTON,
        ID_SOUND_BUTTON,
        ID_BACK_BUTTON,
        ID_CLOSE_BUTTON,
        ID_MENU_BUTTON,
        ID_RETRY_BUTTON,
        ID_HINTS_BUTTON,
        ID_BACK_TO_LEVELS_BUTTON,
        ID_CHAPTER_SELECTION_ARROW_PREVIOUS,
        ID_CHAPTER_SELECTION_ARROW_NEXT,

        //Action buttons
        ID_COLOR_CHANGE,
        ID_MODE_CHANGE
    }

    public GUIButtonID m_ID;

    public virtual void Init(Material skinMaterial = null)
    {
        BaseQuad[] childrenQuads = this.GetComponentsInChildren<BaseQuad>();
        m_skin = (childrenQuads.Length > 0) ? (UVQuad) childrenQuads[0] : null;
        m_background = (childrenQuads.Length > 1) ? childrenQuads[1] : null;
        m_shadow = (childrenQuads.Length > 2) ? childrenQuads[2] : null;

        if (skinMaterial != null)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = skinMaterial;
        }

        if (m_skin != null)
        {
            m_skin.InitQuadMesh();
            m_skin.transform.localPosition = new Vector3(0,0,-2);
        }

        if (m_background != null)
        {
            m_background.InitQuadMesh();
            m_background.transform.localPosition = new Vector3(0,0,-1);
        }

        if (m_shadow != null)
        {
            m_shadow.InitQuadMesh();
            m_shadow.transform.localPosition = new Vector3(0,0,0);
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

        Transform[] childTransforms = this.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i != childTransforms.Length; i++)
        {
            if (childTransforms[i] != this.transform)
                childTransforms[i].transform.localScale = GeometryUtils.BuildVector3FromVector2(size, 1);
        }
    }

    /**
     * Set the background color
     * **/
    public virtual void SetBackgroundColor(Color backgroundColor)
    {

    }

    /**
     * Set the shadow color
     * **/
    public virtual void SetShadowColor(Color shadowColor)
    {

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
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /**
     * Set a material to the background Quad
     * **/
    public void SetBackgroundMaterial(Material material)
    {
        if (m_background != null)
            m_background.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    /**
     * Set a material to the shadow Quad
     * **/
    public void SetShadowMaterial(Material material)
    {
        if (m_shadow != null)
            m_shadow.GetComponent<MeshRenderer>().sharedMaterial = material;
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
        return false;
    }

    /**
     * Player clicked the GUI buttons
     * **/
    public virtual void OnClick()
    {
        if (m_ID == GUIButtonID.ID_OPTIONS_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            guiManager.ShowOptionsWindow();
        }
        else if (m_ID == GUIButtonID.ID_CREDITS_BUTTON)
        {
            UnityEngine.Debug.Log("OnClick Credits");
        }
        else if (m_ID == GUIButtonID.ID_MUSIC_BUTTON)
        {
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleMusic();

            PersistentDataManager persistentDataManager = GameObject.FindGameObjectWithTag("PersistentDataManager").GetComponent<PersistentDataManager>();
            persistentDataManager.SetMusicStatus(soundManager.m_musicActive);

            MeshRenderer skinMeshRenderer = m_skin.gameObject.GetComponent<MeshRenderer>();
            Material onMaterial = m_skinOnMaterial;
            Material offMaterial = m_skinOffMaterial;
            skinMeshRenderer.material = soundManager.m_musicActive ? onMaterial : offMaterial;
        }
        else if (m_ID == GUIButtonID.ID_SOUND_BUTTON)
        {
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleSound();

            PersistentDataManager persistentDataManager = GameObject.FindGameObjectWithTag("PersistentDataManager").GetComponent<PersistentDataManager>();
            persistentDataManager.SetMusicStatus(soundManager.m_soundActive);

            MeshRenderer skinMeshRenderer = m_skin.gameObject.GetComponent<MeshRenderer>();
            Material onMaterial = m_skinOnMaterial;
            Material offMaterial = m_skinOffMaterial;
            skinMeshRenderer.material = soundManager.m_soundActive ? onMaterial : offMaterial;
        }
        else if (m_ID == GUIButtonID.ID_BACK_BUTTON)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            SceneManager.DisplayContent displayedContent = sceneManager.m_displayedContent;
            if (displayedContent == SceneManager.DisplayContent.CHAPTERS)
            {
                sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.MENU, false, 0.0f, 0.5f, 0.5f);
                guiManager.DismissBackButton();
            }
            else if (displayedContent == SceneManager.DisplayContent.LEVELS)
            {
                sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.CHAPTERS, false, 0.0f, 0.5f, 0.5f);
            }
        }
        else if (m_ID == GUIButtonID.ID_CLOSE_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            if (guiManager.IsOptionsWindowShown())
            {
                guiManager.DismissOptionsWindow();
            }
            else if (guiManager.IsPauseWindowShown())
            {
                guiManager.DismissPauseWindow();
            }
        }
        else if (m_ID == GUIButtonID.ID_MENU_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            guiManager.ShowPauseWindow();
        }
        else if (m_ID == GUIButtonID.ID_RETRY_BUTTON)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false, 0.0f, 0.5f, 0.5f);
        }
        else if (m_ID == GUIButtonID.ID_HINTS_BUTTON)
        {
            Debug.Log("HINTS");
        }
        else if (m_ID == GUIButtonID.ID_BACK_TO_LEVELS_BUTTON)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVELS, false, 0.0f, 1.0f, 0.0f);

            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            guiManager.DismissPauseWindow();
        }
        else if (m_ID == GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            Chapters chapters = (Chapters) sceneManager.m_currentScene;

            if (chapters.DecrementChapterIndex())
            {
                chapters.DismissAndDestroyCentralItemInformation();
                chapters.BuildAndShowCentralItemInformation(true, 0.5f, 0.5f);
                chapters.UpdateBackgroundGradient();
            }
        }
        else if (m_ID == GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT)
        {
            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            Chapters chapters = (Chapters)sceneManager.m_currentScene;

            if (chapters.IncrementChapterIndex())
            {
                chapters.DismissAndDestroyCentralItemInformation();
                chapters.BuildAndShowCentralItemInformation(true, 0.5f, 0.5f);
                chapters.UpdateBackgroundGradient();
            }
        }
    }
}
