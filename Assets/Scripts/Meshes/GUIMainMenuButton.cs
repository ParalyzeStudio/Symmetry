using UnityEngine;

public class GUIMainMenuButton : GUIQuadButton
{
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
        string tag = this.gameObject.tag;
        if (tag.Equals("OptionsButton"))
            ;
    }
}
