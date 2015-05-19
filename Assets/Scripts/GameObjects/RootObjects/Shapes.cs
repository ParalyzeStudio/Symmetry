using UnityEngine;
using System.Collections.Generic;

public class Shapes : MonoBehaviour
{
    public GameObject m_shapePfb; //prefab to instantiate a shape
    public List<GameObject> m_shapesObj { get; set; } //list of children shapes
    public List<PositionColorMaterial> m_materials { get; set; }

    public Material m_transpColorMaterial;

    public void Awake()
    {
        m_shapesObj = new List<GameObject>();
        m_materials = new List<PositionColorMaterial>();
    }

    /**
     * Build a shape game object from shape data (contour/triangles, holes, color)
     * **/
    public GameObject CreateShapeObjectFromData(Shape shapeData)
    {
        GameObject clonedShapeObject = (GameObject)Instantiate(m_shapePfb);
        clonedShapeObject.transform.parent = this.gameObject.transform;
        clonedShapeObject.transform.localPosition = Vector3.zero;

        MeshRenderer meshRenderer = clonedShapeObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = GetMaterialForColor(shapeData.m_color).m_material;

        ShapeAnimator shapeAnimator = clonedShapeObject.GetComponent<ShapeAnimator>();
        shapeAnimator.m_color = shapeData.m_color;

        ShapeRenderer shapeRenderer = clonedShapeObject.GetComponent<ShapeRenderer>();
        shapeRenderer.m_shape = shapeData;
        shapeRenderer.Render(true, ShapeRenderer.RenderFaces.DOUBLE_SIDED);

        AddShapeObject(clonedShapeObject);

        return clonedShapeObject;
    }

    public void DestroyShape(GameObject shape)
    {
        Destroy(shape);
    }

    /**
     * Add a shape GameObject to the list of shapes
     * **/
    public void AddShapeObject(GameObject shape)
    {
        m_shapesObj.Add(shape);
    }

    /**
     * Remove a shape GameObject from the list of shapes
     * **/
    public void RemoveShapeObject(GameObject shape)
    {
        for (int shapeIndex = 0; shapeIndex != m_shapesObj.Count; shapeIndex++)
        {
            if (m_shapesObj[shapeIndex] == shape)
            {
                m_shapesObj.Remove(shape);
                return;
            }
        }
    }

    /**
     * Remove all shape GameObjects from the list of shapes
     * **/
    public void ClearShapeObjects()
    {
        m_shapesObj.Clear();
    }

    /**
     * Add a shape material to the list of materials
     * **/
    public void CreateAndAddMaterialForColor(Color color)
    {
        //First check if the material is not already listed
        if (GetMaterialForColor(color) != null)
            return;

        //if not, just add it
        Material clonedMaterial = (Material)Instantiate(m_transpColorMaterial);
        PositionColorMaterial pcMaterial = new PositionColorMaterial(color, clonedMaterial);
        m_materials.Add(pcMaterial);
    }

    /**
     * Remove a shape material from the list of materials
     * **/
    public void RemoveMaterialForColor(Color color)
    {
        for (int iMaterialIdx = 0; iMaterialIdx != m_materials.Count; iMaterialIdx++)
        {
            PositionColorMaterial material = m_materials[iMaterialIdx];
            if (material.m_color.Equals(color))
            {
                m_materials.Remove(material);
                return;
            }
        }
    }

    /**
     * Get the shape material with the specified color
     * **/
    public PositionColorMaterial GetMaterialForColor(Color color)
    {
        for (int iMaterialIdx = 0; iMaterialIdx != m_materials.Count; iMaterialIdx++)
        {
            PositionColorMaterial material = m_materials[iMaterialIdx];
            if (material.m_color.Equals(color))
            {
                return material;
            }
        }

        return null;
    }

    /**
     * Clear the list of materials
     * **/
    public void ClearMaterials()
    {
        m_materials.Clear();
    }

    /**
     * Calls recursively Shape.Fusion() on the shape passed as parameter and then on the shape resulting from previous fusion
     * This way we are sure that the initial shape is fusionned to every shape that overlapped it at the beginning
     * **/
    public static void PerformFusionOnShape(Shape shape)
    {
        Shape resultingShape = shape;
        do
        {
            resultingShape = resultingShape.Fusion();
        }
        while (resultingShape != null);
    }
}
