using UnityEngine;

public class TranspQuadOpacityAnimator : GameObjectAnimator
{
    public override void OnOpacityChanged(float fNewOpacity)
    {
        base.OnOpacityChanged(fNewOpacity);

        Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        Color color = material.GetColor("_Color");
        color.a = fNewOpacity;
        material.SetColor("_Color", color);
    }
}

