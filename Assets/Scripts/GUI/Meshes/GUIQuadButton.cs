using UnityEngine;

public class GUIQuadButton : MonoBehaviour
{
    public UVQuad m_skin { get; set; }
    public ColorQuad m_shadow { get; set; }

    public enum ButtonState
    {
        PRESSED = 1,
        IDLE,
        DISABLED
    }

    public ButtonState m_state { get; set; }

    public void Start()
    {
        Transform[] children = this.GetComponentsInChildren<Transform>();
        m_skin = children[1].gameObject.GetComponent<UVQuad>();
        m_shadow = (children.Length > 2) ? children[1].gameObject.GetComponent<ColorQuad>() : null;

        m_skin.SetTextureRange(new Vector4(0,0,1,1));
        m_skin.SetTextureWrapMode(TextureWrapMode.Clamp);
        TexturedQuadAnimator skinAnimator = m_skin.GetComponent<TexturedQuadAnimator>();
        skinAnimator.SetColor(Color.white);
    }

    //public override void InitQuadMesh()
    //{
    //    m_textureRange = new Vector4(0, 0, 1, 1);
    //    m_textureWrapMode = TextureWrapMode.Clamp;
    //    m_state = ButtonState.IDLE;

    //    base.InitQuadMesh();
    //}

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