using UnityEngine;

public class GUIMainMenuButton : GUIQuadButton
{
    public override void OnPress()
    {

    }

    public override void OnRelease()
    {

    }

    public override void OnClick()
    {
        string tag = this.gameObject.tag;
        if (tag.Equals("OptionsButton"))
            ;
    }
}
