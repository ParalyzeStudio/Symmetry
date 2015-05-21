﻿using UnityEngine;
using System.Collections.Generic;

public class Shapes : MonoBehaviour
{
    public GameObject m_shapePfb; //prefab to instantiate a shape
    public List<GameObject> m_shapesObjects { get; set; } //list of children shapes
    public List<PositionColorMaterial> m_materials { get; set; }

    public Shape m_translatedShape; //the shape that is being translated by the user if applicable
    public List<GameObject> m_intersectionShapesObjects { get; set; } //the temporary list of shapes resulting from the intersection of the shape 
                                                               //being moved shape and other shapes during translation

    public Material m_transpColorMaterial;

    public void Awake()
    {
        m_shapesObjects = new List<GameObject>();
        m_materials = new List<PositionColorMaterial>();
        m_intersectionShapesObjects = new List<GameObject>();
        m_translatedShape = null;
    }

    /**
     * Build a shape game object from shape data (contour/triangles, holes, color)
     * **/
    public GameObject CreateShapeObjectFromData(Shape shapeData, bool bIntersectionShape = false)
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
        shapeRenderer.Render(/*true, */ShapeRenderer.RenderFaces.DOUBLE_SIDED);

        if (bIntersectionShape)
            AddIntersectionShapeObject(clonedShapeObject);
        else
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
        m_shapesObjects.Add(shape);
    }

    /**
     * Remove a shape GameObject from the list of shapes
     * **/
    public void RemoveShapeObject(GameObject shape)
    {
        for (int shapeIndex = 0; shapeIndex != m_shapesObjects.Count; shapeIndex++)
        {
            if (m_shapesObjects[shapeIndex] == shape)
            {
                m_shapesObjects.Remove(shape);
                return;
            }
        }
    }

    /**
     * Remove all shape GameObjects from the list of shapes
     * **/
    public void ClearShapeObjects()
    {
        m_shapesObjects.Clear();
    }

    /**
     * Add a shape GameObject to the list of shapes
     * **/
    public void AddIntersectionShapeObject(GameObject shape)
    {
        m_intersectionShapesObjects.Add(shape);
    }

    /**
     * Add a shape material to the list of materials
     * **/
    public PositionColorMaterial CreateAndAddMaterialForColor(Color color)
    {
        //if not, just add it
        Material clonedMaterial = (Material)Instantiate(m_transpColorMaterial);
        PositionColorMaterial pcMaterial = new PositionColorMaterial(color, clonedMaterial);
        m_materials.Add(pcMaterial);

        return pcMaterial;
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

        //material does not exist, create it
        return CreateAndAddMaterialForColor(color);
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

    /**
     * Check if this shape overlaps a shape of a different color and invalidate the list of intersection shapes if necessary
     * **/
    public void InvalidateIntersectionShapes()
    {
        int iIntersectionShapeIdx = 0;
        for (int iShapeIdx = 0; iShapeIdx != m_shapesObjects.Count; iShapeIdx++)
        {
            GameObject shapeObject = m_shapesObjects[iShapeIdx];

            Shape shape = shapeObject.GetComponent<ShapeRenderer>().m_shape;

            if (shape == m_translatedShape)
                continue;
            else
            {
                if (m_translatedShape.OverlapsShape(shape))
                {     
                    if (!shape.m_color.Equals(m_translatedShape.m_color))
                    {
                        //List<Shape> intersectionShapes = ClippingBooleanOperations.ShapesIntersection(m_translatedShape, shape);

                        //Color intersectionShapeColor = 0.5f * (m_translatedShape.m_color + shape.m_color);

                        //for (int i = 0; i != intersectionShapes.Count; i++)
                        //{
                        //    Shape intersectionShape = intersectionShapes[i];
                        //    intersectionShape.m_color = intersectionShapeColor;
                        //    InsertIntersectionShapeAtIndex(intersectionShape, iIntersectionShapeIdx);
                        //    iIntersectionShapeIdx++;
                        //}
                    }
                }
            }
        }
    }

    /**
     * Build a game object to render the intersection shape (index >= list.Count) 
     * or reuse an existing object (index < list.Count)
     * **/
    public void InsertIntersectionShapeAtIndex(Shape shape, int index)
    {
        if (index >= m_intersectionShapesObjects.Count)
        {
            CreateShapeObjectFromData(shape, true);
        }
        else
        {
            GameObject recycledShapeObject = m_intersectionShapesObjects[index];

            MeshRenderer meshRenderer = recycledShapeObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = GetMaterialForColor(shape.m_color).m_material;

            ShapeAnimator shapeAnimator = recycledShapeObject.GetComponent<ShapeAnimator>();
            shapeAnimator.m_color = shape.m_color;

            ShapeRenderer shapeRenderer = recycledShapeObject.GetComponent<ShapeRenderer>();
            shapeRenderer.m_shape = shape;
            shapeRenderer.Render(/*true, */ShapeRenderer.RenderFaces.DOUBLE_SIDED);
        }
    }

    /**
     * Crops the end of the list after the specified index and destroy all dismissed objects
     * **/
    public void TrimIntersectionsShapesAfterIndex(int trimIndex)
    {
        if (trimIndex == m_intersectionShapesObjects.Count - 1) //no element after trimIndex
            return;

        for (int i = trimIndex + 1; i != m_intersectionShapesObjects.Count; i++)
        {
            //Destroy(m_intersectionShapesObjects[i]);
            m_intersectionShapesObjects[i].GetComponent<MeshFilter>().sharedMesh = null;
        }
    }
}
