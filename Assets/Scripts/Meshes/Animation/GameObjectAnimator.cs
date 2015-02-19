using UnityEngine;

public class GameObjectAnimator : ValueAnimator
{
    public Vector2 m_pivotPoint; //the pivot point of this game object

    public void Awake()
    {
        m_pivotPoint = new Vector2(0.5f, 0.5f);
    }

    public override void OnPositionChanged(Vector3 newPosition)
    {
        this.transform.localPosition = newPosition;
    }

    public override void OnScaleChanged(Vector2 newScale)
    {
        Vector2 objectLocalPosition = this.transform.localPosition;

        //first obtain the pivot position
        Vector2 objectCenterToPivotPoint = new Vector2(m_pivotPoint.x - 0.5f, m_pivotPoint.y - 0.5f);
        objectCenterToPivotPoint.Scale(this.transform.localScale);
        objectCenterToPivotPoint = this.transform.rotation * objectCenterToPivotPoint;
        Vector2 pivotPointPosition = objectLocalPosition + objectCenterToPivotPoint;
        
        //update the size of the game object
        this.transform.localScale = newScale;

        //translate the object for it can scale around pivot point
        Vector2 pivotPointToObjectCenter = new Vector2(0.5f - m_pivotPoint.x, 0.5f - m_pivotPoint.y);
        pivotPointToObjectCenter.Scale(newScale);
        pivotPointToObjectCenter = this.transform.rotation * pivotPointToObjectCenter;       
        objectLocalPosition = pivotPointPosition + pivotPointToObjectCenter;
        this.transform.localPosition = objectLocalPosition;
    }
}
