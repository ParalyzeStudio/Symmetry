using UnityEngine;

public class GUIDiamondShapeButton : GUIButton
{
    public override void Init(Material skinMaterial = null)
    {
        base.Init(skinMaterial);

        //rotate background and shadow by 45 degrees
        m_background.transform.gameObject.transform.localRotation = Quaternion.AngleAxis(45, Vector3.forward);
        m_shadow.transform.gameObject.transform.localRotation = Quaternion.AngleAxis(45, Vector3.forward);

        //displace the skin and background 3 pixels up and the shadow 3 pixels down
        Vector3 skinLocalPosition = m_skin.transform.localPosition;
        Vector3 backgroundLocalPosition = m_background.transform.localPosition;
        Vector3 shadowLocalPosition = m_shadow.transform.localPosition;

        skinLocalPosition += new Vector3(0, 3, 0);
        backgroundLocalPosition += new Vector3(0, 3, 0);
        shadowLocalPosition += new Vector3(0, -3, 0);

        m_skin.transform.localPosition = skinLocalPosition;
        m_background.transform.localPosition = backgroundLocalPosition;
        m_shadow.transform.localPosition = shadowLocalPosition;
    }

    public override void SetSize(Vector2 size)
    {
        m_size = size;

        //width and height of the diamond-shaped button are the length of the diagonal of it
        m_background.transform.localScale = GeometryUtils.BuildVector3FromVector2(size / Mathf.Sqrt(2), 1);
        m_shadow.transform.localScale = GeometryUtils.BuildVector3FromVector2(size / Mathf.Sqrt(2), 1);
        m_skin.transform.localScale = GeometryUtils.BuildVector3FromVector2(size, 1);
    }

    /**
     * Set the background color
     * **/
    public override void SetBackgroundColor(Color backgroundColor)
    {
        m_background.gameObject.GetComponent<ColorQuadAnimator>().SetColor(backgroundColor);
    }

    /**
     * Set the shadow color
     * **/
    public override void SetShadowColor(Color shadowColor)
    {
        m_shadow.gameObject.GetComponent<ColorQuadAnimator>().SetColor(shadowColor);
    }

    public override bool OnPress()
    {
        if (!base.OnPress())
            return false;

        //Move the skin and background by 6 pixels down
        m_skin.transform.localPosition += new Vector3(0, -6, 0);
        m_background.transform.localPosition += new Vector3(0, -6, 0);

        return true;
    }

    public override bool OnRelease()
    {
        if (!base.OnRelease())
            return false;

        //Move the skin and background by 6 pixels top
        m_skin.transform.localPosition += new Vector3(0, 6, 0);
        m_background.transform.localPosition += new Vector3(0, 6, 0);

        return true;
    }
}
