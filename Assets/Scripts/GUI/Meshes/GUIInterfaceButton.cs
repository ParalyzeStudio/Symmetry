﻿using UnityEngine;
using System;

/**
 * Button used in menus or in game when player has clicked a HUDButton such as options
 * **/
public class GUIInterfaceButton : GUIQuadButton
{
    public float m_touchAreaRadius = 65.0f;

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
        ID_HINTS_BUTTON,
        ID_BACK_TO_LEVELS_BUTTON
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

    public void SetSize(Vector2 size)
    {
        Transform[] childTransforms = this.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i != childTransforms.Length; i++)
        {
            if (childTransforms[i] != this.transform)
                childTransforms[i].transform.localScale = GeometryUtils.BuildVector3FromVector2(size, 1);
        }
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
        Debug.Log("m_ID: " + m_ID);

        if (m_ID == GUIInterfaceButtonID.ID_OPTIONS_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            guiManager.ShowOptionsWindow();
        }
        else if (m_ID == GUIInterfaceButtonID.ID_CREDITS_BUTTON)
        {
            UnityEngine.Debug.Log("OnClick Credits");
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
        else if (m_ID == GUIInterfaceButtonID.ID_CLOSE_BUTTON)
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
        else if (m_ID == GUIInterfaceButtonID.ID_PAUSE_BUTTON)
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            guiManager.ShowPauseWindow();
        }
        else if (m_ID == GUIInterfaceButtonID.ID_RETRY_BUTTON)
        {
            Debug.Log("RETRY");
        }
        else if (m_ID == GUIInterfaceButtonID.ID_HINTS_BUTTON)
        {
            Debug.Log("HINTS");
        }
    }
}
