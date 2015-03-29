using UnityEngine;

public class GUIFrameAnimator : GameObjectAnimator
{
    public override void SetColor(Color color)
    {
        base.SetColor(color);

        TintColorMaterialAssignment tintColorComponent = this.gameObject.GetComponent<TintColorMaterialAssignment>();
        tintColorComponent.SetTintColor(color);
    }
}