using UnityEngine;
using System.Collections.Generic;

public class ShapeAnimator : GameObjectAnimator
{
    public override void SetOpacity(float opacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(opacity, false); //only children a shape can have are cells, do not pass opacity to them because they are likely to not being built yet

        ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
        shapeMesh.SetTintColor(m_color);
        shapeMesh.RefreshMesh(); //refresh the mesh instantly
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        ShapeMesh shapeMesh = this.gameObject.GetComponent<ShapeMesh>();
        shapeMesh.SetTintColor(m_color);
        shapeMesh.RefreshMesh(); //refresh the mesh instantly
    }
}