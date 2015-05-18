﻿using UnityEngine;

public class GUIQuadButton : MonoBehaviour
{
    public UVQuad m_skin { get; set; }
    public UVQuad m_shadow { get; set; }

    public enum ButtonState
    {
        PRESSED = 1,
        IDLE,
        DISABLED
    }

    public ButtonState m_state { get; set; }

    public void Init(Material skinMaterial = null)
    {
        Transform[] children = this.GetComponentsInChildren<Transform>();
        m_skin = (children.Length > 1) ? children[1].gameObject.GetComponent<UVQuad>() : null;
        m_shadow = (children.Length > 2) ? children[2].gameObject.GetComponent<UVQuad>() : null;

        if (skinMaterial != null)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = skinMaterial;
        }

        if (m_skin != null)
        m_skin.InitQuadMesh();

        if (m_shadow != null)
            m_shadow.InitQuadMesh();
    }

    /**
     * Set the size of this object by applying it to all its children
     * **/
    public void SetSize(Vector2 size)
    {
        Transform[] childTransforms = this.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i != childTransforms.Length; i++)
        {
            if (childTransforms[i] != this.transform)
                childTransforms[i].transform.localScale = GeometryUtils.BuildVector3FromVector2(size, 1);
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

    }
}