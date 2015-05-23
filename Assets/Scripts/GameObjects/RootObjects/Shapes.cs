using UnityEngine;
using System.Collections.Generic;
using System;

public class Shapes : MonoBehaviour
{
    public GameObject m_shapePfb; //prefab to instantiate a shape
    public List<GameObject> m_shapesObjects { get; set; } //list of children shapes
    public List<PositionColorMaterial> m_materials { get; set; }

    public Shape m_translatedShape; //the shape that is being translated by the user if applicable
    public GameObject[] m_intersectionShapeObjects { get; set; } //the temporary list of shapes resulting from the intersection of the shape 
                                                               //being moved shape and other shapes during translation

    public Material m_transpColorMaterial;

    public void Awake()
    {
        m_shapesObjects = new List<GameObject>();
        m_materials = new List<PositionColorMaterial>();
        m_intersectionShapeObjects = new GameObject[64]; //big enough array
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
        int shapeIndex = FindIntersectionShapesArrayFirstNullObjectIndex();
        m_intersectionShapeObjects[shapeIndex] = shape;
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
                    Vector2 vertex0 = m_translatedShape.m_gridTriangles[0].m_points[0];
                    Debug.Log("vertex0 X:" + vertex0.x + " Y:" + vertex0.y + " area:" + m_translatedShape.m_area);
                    if (!shape.m_color.Equals(m_translatedShape.m_color))
                    {
                        List<Shape> intersectionShapes = ClippingBooleanOperations.ShapesIntersection(m_translatedShape, shape);

                        Color intersectionShapeColor = 0.5f * (m_translatedShape.m_color + shape.m_color);

                        for (int i = 0; i != intersectionShapes.Count; i++)
                        {
                            Shape intersectionShape = intersectionShapes[i];
                            intersectionShape.m_color = intersectionShapeColor;
                            intersectionShape.Triangulate();

                            InsertIntersectionShapeAtIndex(intersectionShape, iIntersectionShapeIdx);
                            iIntersectionShapeIdx++;
                        }
                    }
                }
            }
        }

        if (iIntersectionShapeIdx == 0) //no overlapping, clear the vector of intersection shapes and remove all related objects
            CleanUpIntersectionShapes();
        else
            TrimIntersectionShapesAfterIndex(iIntersectionShapeIdx - 1); //trim the list after the last inserted index
    }

    /**
     * Build a game object to render the intersection shape (index >= list.Count) 
     * or reuse an existing object (index < list.Count)
     * **/
    public void InsertIntersectionShapeAtIndex(Shape shape, int index)
    {
        int intersectionShapesArraySize = GetIntersectionShapesArrayEffectiveSize();

        if (index >= intersectionShapesArraySize)
        {
            GameObject intersectionShapeObject = CreateShapeObjectFromData(shape, true);
            intersectionShapeObject.transform.localPosition = new Vector3(0,0,-200);
        }
        else
        {
            GameObject recycledShapeObject = m_intersectionShapeObjects[index];

            MeshRenderer meshRenderer = recycledShapeObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = GetMaterialForColor(shape.m_color).m_material;

            ShapeAnimator shapeAnimator = recycledShapeObject.GetComponent<ShapeAnimator>();
            shapeAnimator.m_color = shape.m_color;

            ShapeRenderer shapeRenderer = recycledShapeObject.GetComponent<ShapeRenderer>();
            shapeRenderer.m_shape = shape;
            shapeRenderer.Render(ShapeRenderer.RenderFaces.DOUBLE_SIDED);

            recycledShapeObject.transform.localPosition = new Vector3(0, 0, -200);
        }
    }

    /**
     * Crops the end of the list after the specified index and destroy all dismissed objects
     * **/
    public void TrimIntersectionShapesAfterIndex(int trimIndex)
    {
        for (int i = trimIndex + 1; i != m_intersectionShapeObjects.Length; i++)
        {
            GameObject shapeObject = m_intersectionShapeObjects[i];
            if (shapeObject != null)
            {
                Destroy(m_intersectionShapeObjects[i]);
                m_intersectionShapeObjects[i] = null;
            }
        }
    }

    /**
     * Cleans up entirely the array of intersection shapes and delete all relevant objects
     * **/
    public void CleanUpIntersectionShapes()
    {
        TrimIntersectionShapesAfterIndex(-1);
    }

    /**
     * Get the number of non-null object inside the intersection shapes array
     * **/
    public int GetIntersectionShapesArrayEffectiveSize()
    {
        return FindIntersectionShapesArrayFirstNullObjectIndex();
    }

    /**
     * Find the index of the first non-null element in the intersection shapes array
     * **/
    public int FindIntersectionShapesArrayFirstNullObjectIndex()
    {
        int i = 0;

        while (m_intersectionShapeObjects[i] != null)
        {
            i++;
        }

        return i;
    }
}
