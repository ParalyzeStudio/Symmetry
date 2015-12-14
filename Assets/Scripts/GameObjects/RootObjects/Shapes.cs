using UnityEngine;
using System.Collections.Generic;
using System;

public class Shapes : MonoBehaviour
{
    public const float SHAPES_OPACITY = 1.0f;

    public GameObject m_shapePfb; //prefab to instantiate a shape
    //public List<GameObject> m_staticShapeObjects { get; set; } //list of shapes currently set in the grid and triangulated
    //public List<GameObject> m_dynamicShapeObjects { get; set; } //list of shapes waiting to be rendered cell by cell and possibly clipped against static shapes

    //public List<Shape> m_staticShapes { get; set; } //list of shapes currently set in the grid and triangulated
    //public List<Shape> m_dynamicShapes { get; set; } //list of shapes waiting to be rendered cell by cell and possibly clipped against static shapes
    public List<Shape> m_shapes { get; set; }

    //public GameObject m_mainOverlappingShapeObject { get; set; } //the subject shape on which the clipping operations are performed on
    //public List<GameObject> m_substitutionShapeObjects { get; set; } //the list of shapes that will replace the actual shapes that are overlapping
    //public List<GameObject> m_overlappingShapeObjects { get; set; } //the list shape objects that are currently overlapping and needs to be masked

    public void Awake()
    {
        //m_staticShapeObjects = new List<GameObject>();
        //m_dynamicShapeObjects = new List<GameObject>();
        //m_mainOverlappingShapeObject = null;

        //m_staticShapes = new List<Shape>();
        //m_dynamicShapes = new List<Shape>();
        m_shapes = new List<Shape>();

        //m_substitutionShapeObjects = new List<GameObject>();
        //m_overlappingShapeObjects = new List<GameObject>();
        //m_overlappingShapeObjects.Capacity = 16;
    }

    /**
     * Build a shape game object from shape data (contour/triangles, holes, color)
     * **/
    public GameObject CreateShapeObjectFromData(Shape shapeData, bool bCellRendering/*, bool bSubstitutionShape = false*/)
    {
        GameObject clonedShapeObject = (GameObject)Instantiate(m_shapePfb);

        ShapeMesh shapeMesh = clonedShapeObject.GetComponent<ShapeMesh>();
        shapeMesh.Init();
        shapeMesh.SetShapeData(shapeData);
        shapeMesh.Render(bCellRendering);

        ShapeAnimator shapeAnimator = clonedShapeObject.GetComponent<ShapeAnimator>();
        shapeAnimator.SetParentTransform(this.transform);
        shapeAnimator.SetPosition(Vector3.zero);
        shapeAnimator.SetColor(shapeData.m_color);

        m_shapes.Add(shapeData);

        //if (bSubstitutionShape)
        //    AddSubstitutionShapeObject(clonedShapeObject);
        //else
        //    AddShapeObject(clonedShapeObject);

        return clonedShapeObject;
    }

    /**
     * Destroy the object holding the mesh with relevant shape data
     * **/
    public void DestroyShapeObjectForShape(Shape shape, float delay = 0.0f)
    {
        Destroy(shape.m_parentMesh.gameObject, delay);
    }

    /**
     * Add a shape GameObject to the list of shapes
     * **/
    //public void AddShapeObject(GameObject shapeObject)
    //{
    //    m_staticShapeObjects.Add(shapeObject);
    //}

    /**
     * Remove a shape GameObject from the list of shapes
     * **/
    //public void RemoveShapeObject(GameObject shapeObject)
    //{
    //    for (int shapeIndex = 0; shapeIndex != m_staticShapeObjects.Count; shapeIndex++)
    //    {
    //        if (m_staticShapeObjects[shapeIndex] == shapeObject)
    //        {
    //            m_staticShapeObjects.Remove(shapeObject);
    //            return;
    //        }
    //    }
    //}

    /**
     * Add an shape object to the overlapping shape objects list
     * **/
    //public void AddOverlappingShapeObject(GameObject shapeObject)
    //{
    //    for (int i = 0; i != m_overlappingShapeObjects.Count; i++)
    //    {
    //        if (shapeObject == m_overlappingShapeObjects[i])
    //            return;
    //    }

    //    m_overlappingShapeObjects.Add(shapeObject);
    //}

    /**
     * Clear all indices from the list of overlapping shape objects
     * **/
    //public void ClearOverlappingShapeObjects(bool bDestroyShapeObjects = false)
    //{
    //    if (bDestroyShapeObjects)
    //    {
    //        for (int i = 0; i != m_overlappingShapeObjects.Count; i++)
    //        {
    //            Destroy(m_overlappingShapeObjects[i]);
    //        }
    //    }

    //    m_overlappingShapeObjects.Clear();
    //}

    /**
     * Tells if this shape object is currently overlapping with another one
     * **/
    //public bool IsOverlappingShapeObject(GameObject shapeObject)
    //{
    //    for (int i = 0; i != m_overlappingShapeObjects.Count; i++)
    //    {
    //        if (shapeObject == m_overlappingShapeObjects[i])
    //            return true;
    //    }

    //    return false;
    //}

    /**
     * Add a shape GameObject to the list of substitution shapes
     * **/
    //public void AddSubstitutionShapeObject(GameObject shapeObject)
    //{
    //    m_substitutionShapeObjects.Add(shapeObject);
    //}

    /**
     * Remove all shape GameObjects from the list of substitution shapes
     * **/
    //public void ClearSubstitutionShapeObjects(bool bDestroysShapeObjects = false)
    //{
    //    if (bDestroysShapeObjects)
    //    {
    //        for (int i = 0; i != m_substitutionShapeObjects.Count; i++)
    //        {
    //            Destroy(m_substitutionShapeObjects[i]);
    //        }
    //    }

    //    m_substitutionShapeObjects.Clear();
    //}

    /**
     * If two or more shape objects overlap
     * **/
    //public void InvalidateOverlappingAndSubstitutionShapes()
    //{
    //    ClearOverlappingShapeObjects();
    //    ClearSubstitutionShapeObjects(true);

    //    //Build lists to hold temporary shapes
    //    List<Shape> intersectionShapes = new List<Shape>();
    //    List<Shape> resultingShapes1 = new List<Shape>();
    //    List<Shape> resultingShapes2 = new List<Shape>();

    //    for (int iShapeIdx = 0; iShapeIdx != m_staticShapeObjects.Count; iShapeIdx++)
    //    {
    //        GameObject shapeObject = m_staticShapeObjects[iShapeIdx];

    //        Shape translatedShape = m_mainOverlappingShapeObject.GetComponent<ShapeMesh>().m_shapeData;
    //        Shape shape = shapeObject.GetComponent<ShapeMesh>().m_shapeData;

    //        if (shapeObject == m_mainOverlappingShapeObject)
    //            continue;
    //        else
    //        {
    //            if (translatedShape.OverlapsShape(shape))
    //            {
    //                AddOverlappingShapeObject(m_mainOverlappingShapeObject);
    //                AddOverlappingShapeObject(shapeObject);

    //                //Compute intersection shapes
    //                intersectionShapes.AddRange(ClippingBooleanOperations.ShapesOperation(translatedShape, shape, ClipperLib.ClipType.ctIntersection));

    //                //Compute resulting shapes from original shape 1
    //                if (resultingShapes1.Count == 0)
    //                {
    //                    resultingShapes1.AddRange(ClippingBooleanOperations.ShapesOperation(translatedShape, shape, ClipperLib.ClipType.ctDifference));
    //                }
    //                else
    //                {
    //                    List<Shape> newResultingShapes1 = new List<Shape>();
    //                    newResultingShapes1.Capacity = resultingShapes1.Count; //newResultingShapes1.Count >= resultingShapes1.Count so set capacity accordingly
    //                    for (int iResultingShapeIdx = 0; iResultingShapeIdx != resultingShapes1.Count; iResultingShapeIdx++)
    //                    {
    //                        Shape resultingShape1 = resultingShapes1[iResultingShapeIdx];
    //                        newResultingShapes1.AddRange(ClippingBooleanOperations.ShapesOperation(resultingShape1, shape, ClipperLib.ClipType.ctDifference));
    //                    }

    //                    resultingShapes1 = newResultingShapes1;
    //                }


    //                //Compute resulting shapes from original shape 2
    //                resultingShapes2.AddRange(ClippingBooleanOperations.ShapesOperation(shape, translatedShape, ClipperLib.ClipType.ctDifference));
    //            }
    //        }
    //    }

    //    InvalidateOpacityOnShapeObjects();

    //    ////Render temporary shape objects
    //    for (int i = 0; i != resultingShapes1.Count; i++)
    //    {
    //        CreateShapeObjectFromData(resultingShapes1[i], true);
    //    }

    //    for (int i = 0; i != resultingShapes2.Count; i++)
    //    {
    //        CreateShapeObjectFromData(resultingShapes2[i], true);
    //    }

    //    for (int i = 0; i != intersectionShapes.Count; i++)
    //    {
    //        CreateShapeObjectFromData(intersectionShapes[i], true);
    //    }
    //}

    /**
     * Set opacity to 0 if shape object is an overlapping object, otherwise set to SHAPES_OPACITY 
     * **/
    //public void InvalidateOpacityOnShapeObjects()
    //{
    //    for (int iShapeIdx = 0; iShapeIdx != m_staticShapeObjects.Count; iShapeIdx++)
    //    {
    //        GameObject shapeObject = m_staticShapeObjects[iShapeIdx];
    //        ShapeAnimator shapeAnimator = shapeObject.GetComponent<ShapeAnimator>();
    //        shapeAnimator.SetOpacity(IsOverlappingShapeObject(shapeObject) ? 0 : SHAPES_OPACITY);
    //    }
    //}

    /**
     * Call this when clipping operations are performed on a target shape object
     * **/
    //public void InitClippingOperationsOnShapeObject(GameObject shapeObject)
    //{
    //    m_mainOverlappingShapeObject = shapeObject;
    //}

    /**
     * Call this when clipping operations are done (for instance player ended translating shape or symmetry has been done)
     * **/
    //public void FinalizeClippingOperations()
    //{
    //    //Transfer substitution shape objects to normal shape objects list
    //    for (int i = 0; i != m_substitutionShapeObjects.Count; i++)
    //    {
    //        m_staticShapeObjects.Add(m_substitutionShapeObjects[i]);
    //    }

    //    //clean up the list of substition shape objects
    //    ClearSubstitutionShapeObjects();

    //    //clean up the list of overlapping shape objects and remove them from the normal shape objects list too
    //    for (int i = 0; i != m_overlappingShapeObjects.Count; i++)
    //    {
    //        m_staticShapeObjects.Remove(m_overlappingShapeObjects[i]);
    //    }
    //    ClearOverlappingShapeObjects(true);

    //    //Set the translated shape object to null
    //    m_mainOverlappingShapeObject = null;
    //}

    //-------------------------------------------------------------------//
    //-------------------------------------------------------------------//
    //---------------------        SEPARATION        --------------------//
    //-------------------------------------------------------------------//
    //-------------------------------------------------------------------//

    /**
    * Sweep dynamic shapes and reveal their cells if they have just crossed the sweeping line and are now on the correct 'side'
    * **/
    public void SweepDynamicShapesWithLine(AxisRenderer.SweepingLine line, bool bLeftSide)
    {
        for (int i = 0; i != m_shapes.Count; i++)
        {
            Shape shape = m_shapes[i];
            if (shape.IsDynamic() && shape.m_parentMesh.m_sweepingLine == line) //only sweep shapes that reference the same line
            {
                shape.m_parentMesh.SweepCellsWithLine(line, bLeftSide);
            }
        }
    }

    /**
     * Delete and remove from shapes list all shapes in the shapes set parameter
     * **/
    public void DeleteShapesSet(HashSet<Shape> shapesToDelete)
    {        
        for (int i = 0; i != m_shapes.Count; i++)
        {      
            Shape shape = m_shapes[i];
            if (shapesToDelete.Contains(shape))
            {
                m_shapes.Remove(shape);
                DestroyShapeObjectForShape(shape);
                i--;
            }
        }
    }
}