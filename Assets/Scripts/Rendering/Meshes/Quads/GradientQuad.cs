using UnityEngine;

public class GradientQuad : BaseQuad
{
    public Material m_radialGradientMaterial;

    /**
     * Generate a mesh with radial gradient on it
     * **/
    public void InitRadial(Color innerColor, Color outerColor, float radius)
    {
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
