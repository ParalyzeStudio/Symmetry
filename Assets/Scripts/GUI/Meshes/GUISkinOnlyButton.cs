using UnityEngine;

/**
 * Interface button with no background and shadow but only skin
 * **/
public class GUISkinOnlyButton : GUIButton
{
    public override bool OnPress()
    {
        if (!base.OnPress())
            return false;

        //Tint the skin (which color is often white) a bit darker
        m_skin.SetTintColor(new Color(0.95f, 0.95f, 0.95f, 1.0f));

        return true;
    }

    public override bool OnRelease()
    {
        if (!base.OnRelease())
            return false;

        //Reset to the default skin color
        m_skin.SetTintColor(Color.white);

        return true;
    }
}
