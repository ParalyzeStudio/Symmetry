using UnityEngine;
using System.Collections;

public class OutlineSegment : GridSegment 
{
    protected override void UpdateTextureRange()
    {
        Texture tex = this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        float texWidth = tex.width;
        this.m_textureRange = new Vector4(0, 0, m_length / texWidth, 1);
    }

    protected override void UpdateTextureWrapMode()
    {
        GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
    }
}
