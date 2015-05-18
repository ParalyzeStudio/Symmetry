using UnityEngine;

public class UVQuad : BaseQuad
{
    public Vector4 m_textureRange { get; set; }
    public TextureWrapMode m_textureWrapMode { get; set; }

    public override void InitQuadMesh()
    {
        base.InitQuadMesh();

        //set UVs
        Vector2[] uvs = new Vector2[4];
        GetComponent<MeshFilter>().sharedMesh.uv = uvs;
        UpdateUVs();

        //Set default texture range
        SetTextureRange(new Vector4(0,0,1,1));

        //set default wrap mode to CLAMP
        SetTextureWrapMode(TextureWrapMode.Clamp);

        //set default tint color to white
        TexturedQuadAnimator quadAnimator = this.GetComponent<TexturedQuadAnimator>();
        quadAnimator.SetColor(Color.white);
    }

    protected void UpdateUVs()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Vector2[] uvs = mesh.uv;

        uvs[0] = new Vector2(m_textureRange.x, m_textureRange.y + m_textureRange.w); //top-left
        uvs[1] = new Vector2(m_textureRange.x + m_textureRange.z, m_textureRange.y + m_textureRange.w); //top-right
        uvs[2] = new Vector2(m_textureRange.x, m_textureRange.y); //bottom-left
        uvs[3] = new Vector2(m_textureRange.x + m_textureRange.z, m_textureRange.y); //bottom-right

        mesh.uv = uvs;
    }

    public void SetTextureRange(Vector4 textureRange)
    {
        m_textureRange = textureRange;
        UpdateUVs();
    }

    /**
     * Switch between CLAMP and REPEAT wrap modes
     * **/
    public void SetTextureWrapMode(TextureWrapMode texWrapMode)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial.mainTexture.wrapMode = m_textureWrapMode;
    }

    /**
     * Set the color that will tint the base texture
     * **/
    public void SetTintColor(Color color)
    {
        Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        if (material != null)
        {
            material.SetColor("_Color", color);
        }
    }
}

