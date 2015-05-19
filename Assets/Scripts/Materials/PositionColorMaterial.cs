using UnityEngine;

public class PositionColorMaterial
{
    public Color m_color { get; set; }
    public Material m_material { get; set; }

    public PositionColorMaterial(Color color, Material material)
    {
        m_color = color;
        m_material = material;
    }
}