using UnityEngine;
using System.Collections.Generic;
using System;

public class Shapes : MonoBehaviour
{
    public const float SHAPES_OPACITY = 0.5f;

    public GameObject m_shapePfb; //prefab to instantiate a shape
    //public List<GameObject> m_staticShapeObjects { get; set; } //list of shapes currently set in the grid and triangulated
    //public List<GameObject> m_dynamicShapeObjects { get; set; } //list of shapes waiting to be rendered cell by cell and possibly clipped against static shapes

    public List<Shape> m_staticShapes { get; set; } //list of shapes currently set in the grid and triangulated
    public List<Shape> m_dynamicShapes { get; set; } //list of shapes waiting to be rendered cell by cell and possibly clipped against static shapes

    //public GameObject m_mainOverlappingShapeObject { get; set; } //the subject shape on which the clipping operations are performed on
    //public List<GameObject> m_substitutionShapeObjects { get; set; } //the list of shapes that will replace the actual shapes that are overlapping
    //public List<GameObject> m_overlappingShapeObjects { get; set; } //the list shape objects that are currently overlapping and needs to be masked

    public void Awake()
    {
        //m_staticShapeObjects = new List<GameObject>();
        //m_dynamicShapeObjects = new List<GameObject>();
        //m_mainOverlappingShapeObject = null;

        m_staticShapes = new List<Shape>();
        m_dynamicShapes = new List<Shape>();

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
        clonedShapeObject.transform.parent = this.gameObject.transform;
        clonedShapeObject.transform.localPosition = Vector3.zero;

        ShapeMesh shapeMesh = clonedShapeObject.GetComponent<ShapeMesh>();
        shapeMesh.Init();
        shapeMesh.SetShapeData(shapeData);
        shapeMesh.Render(bCellRendering);

        if (!bCellRendering)
        {
            ShapeAnimator shapeAnimator = clonedShapeObject.GetComponent<ShapeAnimator>();
            shapeAnimator.SetColor(shapeData.m_color);
            shapeAnimator.SetOpacity(0);
            shapeAnimator.FadeTo(SHAPES_OPACITY, 0.3f);
        }

        if (bCellRendering)
            m_dynamicShapes.Add(shapeData);
        else
            m_staticShapes.Add(shapeData);

        //if (bSubstitutionShape)
        //    AddSubstitutionShapeObject(clonedShapeObject);
        //else
        //    AddShapeObject(clonedShapeObject);

        return clonedShapeObject;
    }

    public void DestroyShape(GameObject shape)
    {
        Destroy(shape);
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
        for (int i = 0; i != m_dynamicShapes.Count; i++)
        {
            ShapeMesh shapeMesh = m_dynamicShapes[i].m_parentMesh;
            shapeMesh.SweepCellsWithLine(line, bLeftSide);
        }
    }

    /**
     * Clip a shape against all the static shapes
     * **/
    public void ClipAgainstStaticShapes(Shape subjShape)
    {
        List<Shape> clipShapes = new List<Shape>(4); //build a list with big enough capacity to store result of clipping on subjShape
        clipShapes.Add(subjShape);

        for (int i = 0; i != m_staticShapes.Count; i++)
        {
            Shape staticShape = m_staticShapes[i];

            //bool bOverlapAtLeastOneStaticShape = false;
            List<Shape> clippedDifferenceShapes = new List<Shape>(10);
            List<Shape> clippedIntersectionShapes = new List<Shape>(10);
            for (int j = 0; j != clipShapes.Count; j++)
            {
                Shape clipShape = clipShapes[j];
                if (clipShape.OverlapsShape(staticShape))
                {
                    //bOverlapAtLeastOneStaticShape = true;

                    List<Shape> differenceShapes = ClippingBooleanOperations.ShapesOperation(clipShape, staticShape, ClipperLib.ClipType.ctDifference);
                    clippedDifferenceShapes.AddRange(differenceShapes);
                    List<Shape> intersectionShapes = new List<Shape>(5);
                    if (differenceShapes.Count == 0) //no difference, so intersection is the full clipShape
                        intersectionShapes.Add(clipShape);
                    else
                        intersectionShapes.AddRange(ClippingBooleanOperations.ShapesOperation(clipShape, staticShape, ClipperLib.ClipType.ctIntersection));

                    clippedIntersectionShapes.AddRange(intersectionShapes);

                    //Color
                    for (int iShapeIdx = 0; iShapeIdx != differenceShapes.Count; iShapeIdx++)
                    {
                        Shape differenceShape = differenceShapes[iShapeIdx];
                        differenceShape.m_color = clipShape.m_color; //same color as original
                    }
                    for (int iShapeIdx = 0; iShapeIdx != intersectionShapes.Count; iShapeIdx++)
                    {
                        Shape intersectionShape = intersectionShapes[iShapeIdx];
                        intersectionShape.m_color = 0.5f * (clipShape.m_color + staticShape.m_color); //mid color of intersected shapes
                    }

                    ////TODO create and add clipped static shape objects
                    //for (int iStaticShapeIdx = 0; iStaticShapeIdx != clippedStaticShapes.Count; iStaticShapeIdx++)
                    //{
                    //    AddStaticShape(clippedStaticShapes[iStaticShapeIdx]);
                    //    CreateShapeObjectFromData(clippedStaticShapes[iStaticShapeIdx], false);
                    //}

                    //TODO remove clipShape from clipShapes and add each element of dynamicShapes instead
                    //clipShapes.Remove(clipShape);
                    //clipShapes.AddRange(dynamicShapes);

                    ////TODO create and add new dynamic shape objects
                    //for (int iDynShapeIdx = 0; iDynShapeIdx != dynamicShapes.Count; iDynShapeIdx++)
                    //{
                    //    GameObject dynShapeObject = CreateShapeObjectFromData(dynamicShapes[iDynShapeIdx], true);
                    //    m_dynamicShapeObjects.Add(dynShapeObject);
                    //}

                    //TODO Calculate intersection shapes and add them to dynamic shape objects
                }
            }

            clipShapes.Clear();
            clipShapes.AddRange(clippedDifferenceShapes);
            clipShapes.AddRange(clippedIntersectionShapes);

            //if (!bOverlapAtLeastOneStaticShape) //no clipping done just add it to the dynamic shape list
            //{
            //    CreateShapeObjectFromData(clipShape, false);
            //    AddDynamicShape(clipShape);
            //}
        }

        //TODO make clipShapes as dynamic shapes
        for (int i = 0; i != clipShapes.Count; i++)
        {
            clipShapes[i].Triangulate();
            CreateShapeObjectFromData(clipShapes[i], true);
        }

        ////TODO make clipShapes as static shapes
        //for (int i = 0; i != clipShapes.Count; i++)
        //{
        //    CreateShapeObjectFromData(clipShapes[i], false);
        //}
    }

    /**
     * Add a shape to the static shapes array
     * **/
    public void AddStaticShape(Shape shape)
    {
        for (int i = 0; i != m_staticShapes.Count; i++)
        {
            if (shape == m_staticShapes[i])
                return;
        }

        m_staticShapes.Add(shape);
    }

    /**
     * Remove a shape from the static shapes array
     * **/
    public void RemoveStaticShape(Shape shape)
    {
        for (int shapeIndex = 0; shapeIndex != m_staticShapes.Count; shapeIndex++)
        {
            if (m_staticShapes[shapeIndex] == shape)
            {
                m_staticShapes.Remove(shape);
                return;
            }
        }
    }

    /**
     * Add a shape to the dynamic shapes array
     * **/
    public void AddDynamicShape(Shape shape)
    {
        for (int i = 0; i != m_dynamicShapes.Count; i++)
        {
            if (shape == m_dynamicShapes[i])
                return;
        }

        m_dynamicShapes.Add(shape);
    }

    /**
     * Remove a shape from the dynamic shapes array
     * **/
    public void RemoveDynamicShape(Shape shape)
    {
        for (int shapeIndex = 0; shapeIndex != m_dynamicShapes.Count; shapeIndex++)
        {
            if (m_dynamicShapes[shapeIndex] == shape)
            {
                m_dynamicShapes.Remove(shape);
                return;
            }
        }
    }
}