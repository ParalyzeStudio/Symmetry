using UnityEngine;

public class TexturedQuadAnimator : GameObjectAnimator
{
    public override void Awake()
    {
        base.Awake();
        m_color = Color.white;
        m_prevColor = m_color;
    }

    public override void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        base.SetOpacity(fOpacity, bPassOnChildren);

        UVQuad texturedQuad = this.GetComponent<UVQuad>();
        texturedQuad.SetTintColor(m_color);
    }

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        UVQuad texturedQuad = this.GetComponent<UVQuad>();
        texturedQuad.SetTintColor(m_color);
    }

    //public override void SetTSB(Vector3 tsb)
    //{
    //    base.SetTSB(tsb);

    //    UVQuad texturedQuad = this.GetComponent<UVQuad>();
    //    texturedQuad.SetTSB(tsb);
    //}
}

