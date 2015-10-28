using UnityEngine;

public class TitleLetterAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        TitleLetter letter = this.GetComponent<TitleLetter>();
        letter.SetForegroundOpacity(fOpacity);
        letter.SetShadowOpacity(fOpacity);
    }
}
