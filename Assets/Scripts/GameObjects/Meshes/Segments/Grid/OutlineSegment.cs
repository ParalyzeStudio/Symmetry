using UnityEngine;
using System.Collections;

public class OutlineSegment : TexturedGridSegment 
{
    public const float DEFAULT_OUTLINE_SEGMENT_THICKNESS = 8.0f;

    public void Build(Vector2 gridPointA, Vector2 gridPointB, Color tintColor, float thickness = DEFAULT_OUTLINE_SEGMENT_THICKNESS)
    {
        base.Build(gridPointA, gridPointB, thickness, null, Color.black, 0); //material is already set int the prefab so pass null to the function
        UpdateUVs();
        SetColor(tintColor);
    }

    protected override void UpdateUVs()
    {
        Texture tex = this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        if (tex == null)
            return;

        float texWidth = tex.width;
        float texHeight = tex.height;
        float segmentToTextureRatio = m_thickness / texHeight;
        Vector4 textureRange = new Vector4(0, 0, m_length / (texWidth * segmentToTextureRatio), 1);

        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(textureRange.x, textureRange.y); //bottom-left
        uvs[1] = new Vector2(textureRange.x, textureRange.y + textureRange.w); //top-left
        uvs[2] = new Vector2(textureRange.x + textureRange.z, textureRange.y); //bottom-right
        uvs[3] = new Vector2(textureRange.x + textureRange.z, textureRange.y + textureRange.w); //top-right

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;

        GetComponent<MeshFilter>().sharedMesh.uv = uvs;
    }
}
