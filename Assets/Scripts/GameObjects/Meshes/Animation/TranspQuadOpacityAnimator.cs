using UnityEngine;

public class TranspQuadOpacityAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        if (material != null)
        {
            Color color = material.GetColor("_Color");
            color.a = fOpacity;
            material.SetColor("_Color", color);
        }
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        if (material != null)
        {
            material.SetColor("_Color", color);
        }
    }
}

