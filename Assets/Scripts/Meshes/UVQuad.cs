using UnityEngine;

[ExecuteInEditMode]
public class UVQuad : MonoBehaviour
{
    public Vector4 m_textureRange;
    public TextureWrapMode m_textureWrapMode;
    private bool m_isTextured; //is this quad textured. If not we don't need to update UVs and wrap mode

    //private variables to prevent unity from calling update functions for nothing
    private Vector4 m_prevTextureRange;
    private TextureWrapMode m_prevTextureWrapMode;

    protected virtual void Awake()
    {
        m_prevTextureRange = new Vector4(0,0,0,0);
        m_prevTextureWrapMode = m_textureWrapMode;
    }

    protected virtual void Start()
    {
        Texture quadMainTexture = this.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        m_isTextured = (quadMainTexture != null);
        InitQuadMesh();
    }

    /**
     * Init a quad mesh (2 triangles, 4 vertices, 6 indices) and set the correct UVs to be able to repeat or clamp a texture over the whole quad
     * **/
    protected void InitQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "UVQuad";

        GetComponent<MeshFilter>().sharedMesh = mesh;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-0.5f, 0.5f, 0.0f); //top-left
        vertices[1] = new Vector3(0.5f, 0.5f, 0.0f); //top-right
        vertices[2] = new Vector3(-0.5f, -0.5f, 0.0f); //bottom-left
        vertices[3] = new Vector3(0.5f, -0.5f, 0.0f); //bottom-right
        mesh.vertices = vertices;

        int[] indices = new int[6] { 0, 1, 2, 3, 2, 1 };
        mesh.triangles = indices;

        //uvs are likely to change on every frame, create a function and call it in the Update() method (also useful for updating the scene in Unity in Edit mode)
        UpdateUVs(true);

        Vector3[] normals = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
        mesh.normals = normals;
    }

    ///**
    // * Set the world position of this quad anchor point and the quad world position accordingly
    // * **/
    //public void SetPosition(Vector2 position)
    //{
    //    m_position = position;
    //    Vector2 quadSize = this.transform.localScale;
    //    Vector2 anchorPointToQuadCenter = new Vector2(0.5f, 0.5f) - m_anchorPoint;
    //    anchorPointToQuadCenter.Scale(quadSize);

    //    this.transform.localPosition = position + anchorPointToQuadCenter;
    //}

    /**
     * Simply update the UVs array
     * **/
    protected virtual void UpdateUVs(bool bForceUpdate)
    {
        if (!m_isTextured)
            return;

        if (!bForceUpdate && m_textureRange.Equals(m_prevTextureRange)) //same value do not update
            return;

        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(m_textureRange.x, m_textureRange.y + m_textureRange.w); //top-left
        uvs[1] = new Vector2(m_textureRange.x + m_textureRange.z, m_textureRange.y + m_textureRange.w); //top-right
        uvs[2] = new Vector2(m_textureRange.x, m_textureRange.y); //bottom-left
        uvs[3] = new Vector2(m_textureRange.x + m_textureRange.z, m_textureRange.y); //bottom-right

        GetComponent<MeshFilter>().sharedMesh.uv = uvs;

        m_prevTextureRange = m_textureRange;
    }

    /**
     * Switch between CLAMP and REPEAT wrap modes
     * **/
    protected void UpdateWrapMode(bool bForceUpdate)
    {
        if (!m_isTextured)
            return;

        if (!bForceUpdate && m_textureWrapMode.Equals(m_prevTextureWrapMode)) //same value do not update
            return;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial.mainTexture.wrapMode = m_textureWrapMode;

        m_prevTextureWrapMode = m_textureWrapMode;
    }

    protected virtual void Update()
    {
        UpdateUVs(false);
        UpdateWrapMode(false);
    }
}

