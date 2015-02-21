using UnityEngine;

[ExecuteInEditMode]
public class MaterialAssignment : MonoBehaviour
{
    public Material m_material;
    private Material m_oldMaterial;

    public virtual void Update()
    {
        if (m_material != m_oldMaterial)
        {
            m_oldMaterial = m_material;
            Material clonedMaterial = (Material) Instantiate(m_material);
            MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = clonedMaterial;
        }
    }
}