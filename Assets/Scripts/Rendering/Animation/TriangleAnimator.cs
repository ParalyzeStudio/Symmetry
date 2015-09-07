using UnityEngine;

public class TriangleAnimator : GameObjectAnimator
{
    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        TriangleMesh triangleMesh = this.GetComponent<TriangleMesh>();
        triangleMesh.SetColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        TriangleMesh triangleMesh = this.GetComponent<TriangleMesh>();
        triangleMesh.SetColor(m_color);
    }
}