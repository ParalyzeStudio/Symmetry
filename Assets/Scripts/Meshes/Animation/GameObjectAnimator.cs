using UnityEngine;

public class GameObjectAnimator : ValueAnimator
{
    protected Vector3 m_pivotPoint; //the pivot point of this game object
    protected Vector3 m_pivotPointPosition;

    public virtual void Awake()
    {
        m_pivotPoint = new Vector3(0.5f, 0.5f, 0.5f);
        m_pivotPointPosition = this.transform.localPosition;
    }

    protected virtual Vector3 GetGameObjectSize()
    {
        BoundingBoxCalculator optionsPanelBBoxCalculator = this.gameObject.GetComponent<BoundingBoxCalculator>();
        if (optionsPanelBBoxCalculator != null)
            return optionsPanelBBoxCalculator.m_bbox.size;
        else
            return this.transform.localScale;
    }

    /**
     * Update the pivot point without changing the object position
     * With this operation only the pivot point and its position are updated
     * **/
    public void UpdatePivotPoint(Vector3 pivotPoint)
    {
        m_pivotPoint = pivotPoint;

        Vector3 objectLocalPosition = this.transform.localPosition;
        Vector3 objectCenterToPivotPoint = new Vector3(m_pivotPoint.x - 0.5f, m_pivotPoint.y - 0.5f, m_pivotPoint.z - 0.5f);
        objectCenterToPivotPoint.Scale(GetGameObjectSize());
        objectCenterToPivotPoint = this.transform.rotation * objectCenterToPivotPoint;
        m_pivotPointPosition = objectLocalPosition + objectCenterToPivotPoint;
    }

    /**
     * Update the pivot point position without changing the object position
     * With this operation only the pivot point and its position are updated
     * **/
    public void UpdatePivotPointPosition(Vector3 pivotPointPosition)
    {
        m_pivotPointPosition = pivotPointPosition;

        Vector3 objectLocalPosition = this.transform.localPosition;
        Vector3 objectCenterToPivotPoint = m_pivotPointPosition - objectLocalPosition;
        Vector3 objectSize = GetGameObjectSize();
        Vector3 invObjectSize = new Vector3(1 / objectSize.x, 1 / objectSize.y, 1 / objectSize.z);
        objectCenterToPivotPoint.Scale(invObjectSize);
        objectCenterToPivotPoint = Quaternion.Inverse(this.transform.rotation) * objectCenterToPivotPoint;
        m_pivotPoint = new Vector3(0.5f, 0.5f, 0.5f) + objectCenterToPivotPoint;
    }

    /**
     * Set the position of the object by translating it by the difference between old pivot point position and new pivot point position
     * **/
    public void MoveObjectBySettingPivotPointPosition(Vector3 newPivotPointPosition)
    {
        Vector3 deltaPosition = newPivotPointPosition - m_pivotPointPosition;
        m_pivotPointPosition = newPivotPointPosition;
        this.transform.localPosition += deltaPosition;
    }

    public override void OnPositionChanged(Vector3 newPosition)
    {
        Vector3 deltaPosition = newPosition - m_pivotPointPosition;
        m_pivotPointPosition += deltaPosition;
        this.transform.localPosition += deltaPosition;
    }

    public override void OnScaleChanged(Vector3 newScale)
    {
        //update the size of the game object
        this.transform.localScale = newScale;

        //translate the object for it can scale around pivot point
        Vector3 pivotPointToObjectCenter = new Vector3(0.5f - m_pivotPoint.x, 0.5f - m_pivotPoint.y, 0.5f - m_pivotPoint.z);
        pivotPointToObjectCenter.Scale(newScale);
        pivotPointToObjectCenter = this.transform.rotation * pivotPointToObjectCenter;
        this.transform.localPosition = m_pivotPointPosition + pivotPointToObjectCenter;
    }

    public override void OnRotationChanged(float newAngle, Vector3 axis)
    {
        //rotate the object itself
        Quaternion rotation = Quaternion.AngleAxis(newAngle, axis);
        this.gameObject.transform.rotation = rotation;

        //Calculate the new position of the object relatively to its pivot point
        Vector3 pivotPointToObjectCenter = new Vector3(0.5f - m_pivotPoint.x, 0.5f - m_pivotPoint.y, 0.5f - m_pivotPoint.z);
        pivotPointToObjectCenter.Scale(GetGameObjectSize());
        pivotPointToObjectCenter = rotation * pivotPointToObjectCenter; //rotate this vector
        this.gameObject.transform.localPosition = m_pivotPointPosition + pivotPointToObjectCenter;
    }

    public override void OnOpacityChanged(float fNewOpacity)
    {
        base.OnOpacityChanged(fNewOpacity);

        ValueAnimator[] childAnimators = this.gameObject.GetComponentsInChildren<ValueAnimator>();
        for (int i = 0; i != childAnimators.Length; i++)
        {
            if (childAnimators[i] != this)
                childAnimators[i].OnOpacityChanged(fNewOpacity);
        }
    }
}
