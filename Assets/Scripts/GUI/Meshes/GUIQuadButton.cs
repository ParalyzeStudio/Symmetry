using UnityEngine;

public class GUIQuadButton : UVQuad
{
    public enum ButtonState
    {
        PRESSED = 1,
        IDLE,
        DISABLED
    }

    public ButtonState m_state { get; set; }

    protected override void Awake()
    {
        base.Awake();
        m_textureRange = new Vector4(0, 0, 1, 1);
        m_textureWrapMode = TextureWrapMode.Clamp;
        m_state = ButtonState.IDLE;
    }

    protected override void Update()
    {
        //do nothing in the update not even executing the parent class code
        //because buttons use a (0,0,1,1) texture range that can't be modified
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