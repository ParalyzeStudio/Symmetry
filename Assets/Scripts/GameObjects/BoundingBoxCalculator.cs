using UnityEngine;

public class BoundingBoxCalculator : MonoBehaviour
{
    public Bounds m_bbox { get; set; }

    public void Start()
    {
        InvalidateBounds();
    }

    /**
     * Invalidates the value of m_bbox by recalculating the bounds of this game object
     * **/
    public void InvalidateBounds()
    {
        Transform[] childTransforms = this.gameObject.GetComponentsInChildren<Transform>();
        if (childTransforms.Length > 1) //this gameobject has children transforms
        {
            Bounds bounds = new Bounds();
            bool bBoundsSet = false;
            for (int iChildTransformIndex = 0; iChildTransformIndex != childTransforms.Length; iChildTransformIndex++)
            {
                Transform childTransform = childTransforms[iChildTransformIndex];
                if (childTransform.gameObject == this.gameObject)
                    continue;

                if (!bBoundsSet) //Set the first bounds to this child object bounds
                {
                    bounds = CalculateDirectChildBounds(childTransform);
                    bBoundsSet = true;
                }
                else
                {
                    Bounds expandBounds = CalculateDirectChildBounds(childTransform);
                    bounds.Encapsulate(expandBounds);
                }
            }

            m_bbox = bounds;
        }
        else
            m_bbox = new Bounds(this.transform.localPosition, this.transform.localScale);
    }

    /**
     * Calculate the bounds of a direct child of this gameobject.transform
     * **/
    private Bounds CalculateDirectChildBounds(Transform directChildTransform)
    {
        Transform[] childTransforms = directChildTransform.GetComponentsInChildren<Transform>();
        if (childTransforms.Length > 1) //this gameobject has children transforms
        {
            Bounds bounds = new Bounds();
            bool bBoundsSet = false;
            for (int iChildTransformIndex = 0; iChildTransformIndex != childTransforms.Length; iChildTransformIndex++)
            {
                Transform childTransform = childTransforms[iChildTransformIndex];

                if (childTransform.gameObject == directChildTransform.gameObject)
                    continue;

                if (!bBoundsSet)//Set the first bounds to this child object bounds
                {
                    bounds = CalculateDirectChildBounds(childTransform);
                    bBoundsSet = true;
                }
                else
                {
                    Bounds expandBounds = CalculateDirectChildBounds(childTransform);
                    bounds.Encapsulate(expandBounds);
                }
            }

            return bounds;
        }
        else
        {
            Vector3 objectSize = directChildTransform.localScale;
            Transform parent = this.transform.parent;
            while (parent != null)
            {
                objectSize.Scale(parent.transform.localScale);
                parent = parent.transform.parent;
            }
            return new Bounds(directChildTransform.transform.localPosition, objectSize);
        }            
    }
}
