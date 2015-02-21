using UnityEngine;

public class GameObjectAnimator : ValueAnimator
{
    private Vector3 m_pivotPoint; //the pivot point of this game object
    private Vector3 m_pivotPointPosition;

    public void Awake()
    {
        m_pivotPoint = new Vector3(0.5f, 0.5f, 0.5f);
        m_pivotPointPosition = this.transform.localPosition;
    }

    private Vector3 GetGameObjectSize()
    {
        BoundingBoxCalculator optionsPanelBBoxCalculator = this.gameObject.GetComponent<BoundingBoxCalculator>();
        if (optionsPanelBBoxCalculator != null)
            return optionsPanelBBoxCalculator.m_bbox.size;
        else
            return this.transform.localScale;
    }

    public void UpdatePivotPoint(Vector2 pivotPoint)
    {
        m_pivotPoint = pivotPoint;

        Vector3 objectLocalPosition = this.transform.localPosition;
        Vector3 objectCenterToPivotPoint = new Vector3(m_pivotPoint.x - 0.5f, m_pivotPoint.y - 0.5f, m_pivotPoint.z - 0.5f);
        objectCenterToPivotPoint.Scale(this.transform.localScale);
        objectCenterToPivotPoint = this.transform.rotation * objectCenterToPivotPoint;
        m_pivotPointPosition = objectLocalPosition + objectCenterToPivotPoint;
    }

    public override void OnPositionChanged(Vector3 newPosition)
    {
        Vector3 deltaPosition = newPosition - this.transform.localPosition;
        this.transform.localPosition = newPosition;
        m_pivotPointPosition += deltaPosition;
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
}
