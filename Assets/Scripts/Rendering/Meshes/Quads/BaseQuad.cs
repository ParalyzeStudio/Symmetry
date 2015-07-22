using UnityEngine;

public class BaseQuad : MonoBehaviour
{
    public virtual void InitQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "BaseQuad";

        GetComponent<MeshFilter>().sharedMesh = mesh;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-0.5f, 0.5f, 0.0f); //top-left
        vertices[1] = new Vector3(0.5f, 0.5f, 0.0f); //top-right
        vertices[2] = new Vector3(-0.5f, -0.5f, 0.0f); //bottom-left
        vertices[3] = new Vector3(0.5f, -0.5f, 0.0f); //bottom-right
        mesh.vertices = vertices;

        int[] indices = new int[6] { 0, 1, 2, 3, 2, 1 };
        mesh.triangles = indices;

        Vector3[] normals = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
        mesh.normals = normals;
    }

    public void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().sharedMaterial = material;
    }
}
