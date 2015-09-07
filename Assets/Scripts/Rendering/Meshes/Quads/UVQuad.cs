using UnityEngine;

public class UVQuad : BaseQuad
{
    public Vector4 m_textureRange { get; set; }
    public TextureWrapMode m_textureWrapMode { get; set; }

    public void Init(Material material = null)
    {
        if (material != null)
            SetMaterial(material);

        InitQuadMesh();
    }

    protected override void InitQuadMesh()
    {
        base.InitQuadMesh();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh.name = "UVQuad";

        //set UVs
        Vector2[] uvs = new Vector2[4];
        meshFilter.sharedMesh.uv = uvs;
        UpdateUVs();

        //Set default texture range
        SetTextureRange(new Vector4(0, 0, 1, 1));

        //set default wrap mode to CLAMP
        SetTextureWrapMode(TextureWrapMode.Clamp);
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
        m_textureWrapMode = texWrapMode;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer.sharedMaterial != null && renderer.sharedMaterial.mainTexture != null)
            renderer.sharedMaterial.mainTexture.wrapMode = texWrapMode;
    }

    /**
     * Set the color that will tint the base texture
     * **/
    public void SetTintColor(Color color)
    {
        Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        if (material != null)
        {
            material.SetColor("_TintColor", color);
        }
    }

    ///**
    // * Set the tint, saturation, brightness of this textured quad all at once
    // * **/
    //public void SetTSB(Vector3 tsb)
    //{
    //    SetTint(tsb.x);
    //    SetSaturation(tsb.y);
    //    SetBrightness(tsb.z);
    //}

    ///**
    // * Set the tint of this this textured quad
    // * **/
    //public void SetTint(float tint)
    //{
    //    Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    //    if (material != null)
    //    {
    //        material.SetFloat("_Tint", tint);
    //    }
    //}

    ///**
    // * Set the saturation of this this textured quad
    // * **/
    //public void SetSaturation(float saturation)
    //{
    //    Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    //    if (material != null)
    //    {
    //        material.SetFloat("_Saturation", saturation);
    //    }
    //}

    ///**
    // * Set the brightness of this this textured quad
    // * **/
    //public void SetBrightness(float brightness)
    //{
    //    Material material = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    //    if (material != null)
    //    {
    //        material.SetFloat("_Brightness", brightness);
    //    }
    //}
}

