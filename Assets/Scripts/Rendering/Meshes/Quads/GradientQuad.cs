using UnityEngine;

[ExecuteInEditMode]
public class GradientQuad : BaseQuad
{
    public Material m_radialGradientMaterial;

    public Color m_radialInnerColor;
    public Color m_radialOuterColor;
    private Color m_prevRadialInnerColor;
    private Color m_prevRadialOuterColor;

    /**
     * Generate a mesh with radial gradient on it
     * **/
    public void InitRadial(Color innerColor, Color outerColor, float radius)
    {
        m_radialInnerColor = innerColor;
        m_prevRadialInnerColor = innerColor;
        m_radialOuterColor = outerColor;
        m_prevRadialOuterColor = outerColor;

        base.InitQuadMesh();
        GetComponent<MeshFilter>().sharedMesh.name = "RadialGradientQuad";

        Material gradientMaterial = Instantiate(m_radialGradientMaterial);
        gradientMaterial.SetColor("_InnerColor", innerColor);
        gradientMaterial.SetColor("_OuterColor", outerColor);
        gradientMaterial.SetFloat("_Radius", radius);
        GetComponent<MeshRenderer>().sharedMaterial = gradientMaterial;

    }

    public void InitLinear(Color startColor, Color endColor)
    {
        base.InitQuadMesh();
    }
}
