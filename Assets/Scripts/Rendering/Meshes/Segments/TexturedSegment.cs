using UnityEngine;
using System;

public class TexturedSegment : Segment
{
    protected Vector2[] m_meshUVs;
    protected bool m_meshUVsDirty;

    public void Build(Vector3 pointA, Vector3 pointB, float thickness, Material material, Color tintColor, bool bGridPoints, TextureWrapMode texWrapMode = TextureWrapMode.Repeat)
    {
        InitBasicVariables(pointA, pointB, thickness, material, bGridPoints, 0);

        if (GetComponent<MeshRenderer>().sharedMaterial.mainTexture == null)
            throw new Exception("Material has no texture set on it");

        RenderInternal();
        SetTextureWrapMode(texWrapMode);

        TexturedSegmentAnimator segmentAnimator = this.GetComponent<TexturedSegmentAnimator>();
        segmentAnimator.SetColor(tintColor);
    }

    protected virtual void RenderInternal(bool bUpdateVertices = true, bool bUpdateIndices = true, bool bUpdateUVs = true)
    {
        base.RenderInternal(bUpdateVertices, bUpdateIndices);

        if (bUpdateUVs)
            UpdateUVs();

    }

    protected virtual void UpdateUVs()
    {
        Texture tex = this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        if (tex == null)
            return;

        float texWidth = tex.width;
        float texHeight = tex.height;
        float segmentToTextureRatio = m_thickness / texHeight;
        Vector4 textureRange = new Vector4(0, 0, m_length / (texWidth * segmentToTextureRatio), 1);

        m_meshUVs = new Vector2[4];
        m_meshUVs[0] = new Vector2(textureRange.x, textureRange.y); //bottom-left
        m_meshUVs[1] = new Vector2(textureRange.x, textureRange.y + textureRange.w); //top-left
        m_meshUVs[2] = new Vector2(textureRange.x + textureRange.z, textureRange.y); //bottom-right
        m_meshUVs[3] = new Vector2(textureRange.x + textureRange.z, textureRange.y + textureRange.w); //top-right

        m_meshUVsDirty = true;
    }

    public void SetTextureWrapMode(TextureWrapMode texWrapMode)
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial.mainTexture.wrapMode = texWrapMode;
    }

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

    public override void Update()
    {
        base.Update();

        if (m_meshUVsDirty)
        {
            m_segmentMesh.uv = m_meshUVs;
            m_meshUVsDirty = false;
        }
    }
}