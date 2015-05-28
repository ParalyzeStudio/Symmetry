﻿using UnityEngine;
using System.Collections.Generic;
using System;

public class Shapes : MonoBehaviour
{
    public const float SHAPES_OPACITY = 1.0f;

    public GameObject m_shapePfb; //prefab to instantiate a shape
    public List<GameObject> m_shapesObjects { get; set; } //list of children shapes
    public List<PositionColorMaterial> m_materials { get; set; }

    public GameObject m_translatedShapeObject; //the shape that is being translated by the user if applicable
    public List<GameObject> m_substitutionShapeObjects; //the list of shapes that will replace the actual shapes that are overlapping
    public List<GameObject> m_overlappingShapeObjects; //the list shape objects that are currently overlapping and needs to be masked

    public Material m_transpColorMaterial;

    public void Awake()
    {
        m_shapesObjects = new List<GameObject>();
        m_materials = new List<PositionColorMaterial>();
        m_translatedShapeObject = null;
        m_substitutionShapeObjects = new List<GameObject>();
        m_overlappingShapeObjects = new List<GameObject>();
        m_overlappingShapeObjects.Capacity = 16;
    }

    /**
     * Build a shape game object from shape data (contour/triangles, holes, color)
     * **/
    public GameObject CreateShapeObjectFromData(Shape shapeData, bool bSubstitutionShape = false)
    {
        GameObject clonedShapeObject = (GameObject)Instantiate(m_shapePfb);
        clonedShapeObject.transform.parent = this.gameObject.transform;
        clonedShapeObject.transform.localPosition = Vector3.zero;

        MeshRenderer meshRenderer = clonedShapeObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = GetMaterialForColor(shapeData.m_color).m_material;

        ShapeRenderer shapeRenderer = clonedShapeObject.GetComponent<ShapeRenderer>();
        shapeRenderer.m_shape = shapeData;
        shapeRenderer.Render(ShapeRenderer.RenderFaces.DOUBLE_SIDED);

        ShapeAnimator shapeAnimator = clonedShapeObject.GetComponent<ShapeAnimator>();
        shapeAnimator.SetColor(shapeData.m_color);

        if (bSubstitutionShape)
            AddSubstitutionShapeObject(clonedShapeObject);
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
    public void AddShapeObject(GameObject shapeObject)
    {
        m_shapesObjects.Add(shapeObject);
    }

    /**
     * Remove a shape GameObject from the list of shapes
     * **/
    public void RemoveShapeObject(GameObject shapeObject)
    {
        for (int shapeIndex = 0; shapeIndex != m_shapesObjects.Count; shapeIndex++)
        {
            if (m_shapesObjects[shapeIndex] == shapeObject)
            {
                m_shapesObjects.Remove(shapeObject);
                return;
            }
        }
    }

    /**
     * Add an shape object to the overlapping shape objects list
     * **/
    public void AddOverlappingShapeObject(GameObject shapeObject)
    {
        for (int i = 0; i != m_overlappingShapeObjects.Count; i++)
        {
            if (shapeObject == m_overlappingShapeObjects[i])
                return;
        }

        m_overlappingShapeObjects.Add(shapeObject);
    }

    /**
     * Clear all indices from the list of overlapping shape objects
     * **/
    public void ClearOverlappingShapeObjects(bool bDestroyShapeObjects = false)
    {
        if (bDestroyShapeObjects)
        {
            for (int i = 0; i != m_overlappingShapeObjects.Count; i++)
            {
                Destroy(m_overlappingShapeObjects[i]);
            }
        }

        m_overlappingShapeObjects.Clear();
    }

    /**
     * Tells if this shape object is currently overlapping with another one
     * **/
    public bool IsOverlappingShapeObject(GameObject shapeObject)
    {
        for (int i = 0; i != m_overlappingShapeObjects.Count; i++)
        {
            if (shapeObject == m_overlappingShapeObjects[i])
                return true;
        }

        return false;
    }

    /**
     * Add a shape GameObject to the list of substitution shapes
     * **/
    public void AddSubstitutionShapeObject(GameObject shapeObject)
    {
        m_substitutionShapeObjects.Add(shapeObject);
    }

    /**
     * Remove all shape GameObjects from the list of substitution shapes
     * **/
    public void ClearSubstitutionShapeObjects(bool bDestroysShapeObjects = false)
    {
        if (bDestroysShapeObjects)
        {
            for (int i = 0; i != m_substitutionShapeObjects.Count; i++)
            {
                Destroy(m_substitutionShapeObjects[i]);
            }
        }

        m_substitutionShapeObjects.Clear();
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
     * If two or more shape objects overlap, 
     * **/
    public void InvalidateOverlappingAndSubstitutionShapes()
    {
        ClearOverlappingShapeObjects();
        ClearSubstitutionShapeObjects(true);

        //Build lists to hold temporary shapes
        List<Shape> intersectionShapes = new List<Shape>();
        List<Shape> resultingShapes1 = new List<Shape>();
        List<Shape> resultingShapes2 = new List<Shape>();

        for (int iShapeIdx = 0; iShapeIdx != m_shapesObjects.Count; iShapeIdx++)
        {
            GameObject shapeObject = m_shapesObjects[iShapeIdx];

            Shape translatedShape = m_translatedShapeObject.GetComponent<ShapeRenderer>().m_shape;
            Shape shape = shapeObject.GetComponent<ShapeRenderer>().m_shape;

            if (shapeObject == m_translatedShapeObject)
                continue;
            else
            {
                if (translatedShape.OverlapsShape(shape))
                {
                    AddOverlappingShapeObject(m_translatedShapeObject);
                    AddOverlappingShapeObject(shapeObject);

                    //Compute intersection shapes
                    intersectionShapes.AddRange(ClippingBooleanOperations.ShapesOperation(translatedShape, shape, ClipperLib.ClipType.ctIntersection));

                    //Compute resulting shapes from original shape 1
                    if (resultingShapes1.Count == 0)
                    {
                        resultingShapes1.AddRange(ClippingBooleanOperations.ShapesOperation(translatedShape, shape, ClipperLib.ClipType.ctDifference));
                    }
                    else
                    {
                        List<Shape> newResultingShapes1 = new List<Shape>();
                        newResultingShapes1.Capacity = resultingShapes1.Count; //newResultingShapes1.Count >= resultingShapes1.Count so set capacity accordingly
                        for (int iResultingShapeIdx = 0; iResultingShapeIdx != resultingShapes1.Count; iResultingShapeIdx++)
                        {
                            Shape resultingShape1 = resultingShapes1[iResultingShapeIdx];
                            newResultingShapes1.AddRange(ClippingBooleanOperations.ShapesOperation(resultingShape1, shape, ClipperLib.ClipType.ctDifference));
                        }

                        resultingShapes1 = newResultingShapes1;
                    }


                    //Compute resulting shapes from original shape 2
                    resultingShapes2.AddRange(ClippingBooleanOperations.ShapesOperation(shape, translatedShape, ClipperLib.ClipType.ctDifference));
                }
            }
        }

        InvalidateOpacityOnShapeObjects();
        
        ////Render temporary shape objects
        for (int i = 0; i != resultingShapes1.Count; i++)
        {
            CreateShapeObjectFromData(resultingShapes1[i], true);
        }

        for (int i = 0; i != resultingShapes2.Count; i++)
        {
            CreateShapeObjectFromData(resultingShapes2[i], true);
        }

        for (int i = 0; i != intersectionShapes.Count; i++)
        {
            CreateShapeObjectFromData(intersectionShapes[i], true);
        }
    }

    /**
     * 
     * **/
    public void InvalidateOpacityOnShapeObjects()
    {
        for (int iShapeIdx = 0; iShapeIdx != m_shapesObjects.Count; iShapeIdx++)
        {
            GameObject shapeObject = m_shapesObjects[iShapeIdx];
            ShapeAnimator shapeAnimator = shapeObject.GetComponent<ShapeAnimator>();
            shapeAnimator.SetOpacity(IsOverlappingShapeObject(shapeObject) ? 0 : SHAPES_OPACITY);
        }
    }
}
