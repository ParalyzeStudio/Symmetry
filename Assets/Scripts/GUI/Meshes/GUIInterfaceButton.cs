using UnityEngine;

/**
 * Button used in menus or in game when player has clicked a HUDButton such as options
 * **/
public class GUIInterfaceButton : GUIQuadButton
{
    public enum GUIInterfaceButtonID
    {
        ID_OPTIONS_BUTTON = 1,
        ID_CREDITS_BUTTON,
        ID_MUSIC_BUTTON,
        ID_SOUND_BUTTON,
        ID_BACK_BUTTON,
        ID_CLOSE_BUTTON,
        ID_PAUSE_BUTTON,
        ID_RETRY_BUTTON,
        ID_HINTS_BUTTON
    }

    public GUIInterfaceButtonID m_ID;

    public Material[] m_materials;

    public static GUIInterfaceButton FindInObjectChildrenForID(GameObject parentObject, GUIInterfaceButtonID id)
    {
        GUIInterfaceButton[] interfaceButtonsChildren = parentObject.GetComponentsInChildren<GUIInterfaceButton>();
        for (int iButtonIdx = 0; iButtonIdx != interfaceButtonsChildren.Length; iButtonIdx++)
        {
            if (interfaceButtonsChildren[iButtonIdx].m_ID == id)
            {
                return interfaceButtonsChildren[iButtonIdx];
            }
        }
        return null;
    }

    public override bool OnPress()
    {
        return base.OnPress();
    }

    public override bool OnRelease()
    {
        return base.OnRelease();
    }

    public override void OnClick()
    {
        if (m_ID == GUIInterfaceButtonID.ID_OPTIONS_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            guiManager.ShowOptionsWindow();
        }
        else if (m_ID == GUIInterfaceButtonID.ID_CREDITS_BUTTON)
        {
            Debug.Log("OnClick Credits");
        }
        else if (m_ID == GUIInterfaceButtonID.ID_MUSIC_BUTTON)
        {
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleMusic();

            MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            Material onMaterial = m_materials[0];
            Material offMaterial = m_materials[1];
            meshRenderer.material = soundManager.m_musicActive ? onMaterial : offMaterial;
        }
        else if (m_ID == GUIInterfaceButtonID.ID_SOUND_BUTTON)
        {
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleSound();

            MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            Material onMaterial = m_materials[0];
            Material offMaterial = m_materials[1];
            meshRenderer.material = soundManager.m_soundActive ? onMaterial : offMaterial;
        }
        else if (m_ID == GUIInterfaceButtonID.ID_BACK_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            GUIManager.DisplayContent displayedContent = guiManager.m_displayedContent;
            if (displayedContent == GUIManager.DisplayContent.CHAPTERS)
            {
                guiManager.SwitchDisplayedContent(GUIManager.DisplayContent.MENU, false, 0.0f, 0.5f);
                guiManager.DismissBackButton();
            }
            else if (displayedContent == GUIManager.DisplayContent.LEVELS)
            {
                guiManager.SwitchDisplayedContent(GUIManager.DisplayContent.CHAPTERS, false, 0.0f, 0.5f);
            }
        }
        else if (m_ID == GUIInterfaceButtonID.ID_CLOSE_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            if (guiManager.IsOptionsWindowShown())
            {
                guiManager.DismissOptionsWindow();
            }
        }
    }
}
