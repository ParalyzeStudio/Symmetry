using UnityEngine;

public class SegmentAnimator : GameObjectAnimator
{
    //Variables to handle pointA position
    protected bool m_translatingPointA;
    public Vector3 m_pointAPosition { get; set; }
    protected Vector3 m_pointAFromPosition;
    protected Vector3 m_pointAToPosition;
    protected float m_translatingPointADuration;
    protected float m_translatingPointADelay;
    protected float m_translatingPointAElapsedTime;
    protected InterpolationType m_translatingPointAInterpolationType;

    //Variables to handle pointB position
    protected bool m_translatingPointB;
    public Vector3 m_pointBPosition { get; set; }
    protected Vector3 m_pointBFromPosition;
    protected Vector3 m_pointBToPosition;
    protected float m_translatingPointBDuration;
    protected float m_translatingPointBDelay;
    protected float m_translatingPointBElapsedTime;
    protected InterpolationType m_translatingPointBInterpolationType;
    
    /***
     * Actions on pointA
     * ***/
    public void TranslatePointATo(Vector3 toPosition, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_translatingPointA = true;
        m_pointAFromPosition = m_pointAPosition;
        m_pointAToPosition = toPosition;
        m_translatingPointADuration = duration;
        m_translatingPointADelay = delay;
        m_translatingPointAElapsedTime = 0;
        m_translatingPointAInterpolationType = interpolType;
    }

    public virtual void SetPointAPosition(Vector3 position)
    {
        m_pointAPosition = position;
        Segment segment = this.gameObject.GetComponent<Segment>();
        segment.SetPointA(position);
        OnPointAPositionChanged();
    }

    public virtual void IncPointAPosition(Vector3 deltaPosition)
    {
        Vector3 fPosition = m_pointAPosition + deltaPosition;
        SetPointAPosition(fPosition);
    }

    public virtual void OnPointAPositionChanged()
    {

    }

    public virtual void OnFinishTranslatingPointA()
    {

    }

    protected virtual void UpdatePointAPosition(float dt)
    {
        if (m_translatingPointA)
        {
            bool inDelay = (m_translatingPointAElapsedTime < m_translatingPointADelay);
            m_translatingPointAElapsedTime += dt;
            if (m_translatingPointAElapsedTime >= m_translatingPointADelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_translatingPointAElapsedTime - m_translatingPointADelay;
                float effectiveElapsedTime = m_translatingPointAElapsedTime - m_translatingPointADelay;
                Vector3 deltaPosition = Vector3.zero;
                Vector3 positionVariation = m_pointAToPosition - m_pointAFromPosition;
                if (m_translatingPointAInterpolationType == InterpolationType.LINEAR)
                    deltaPosition = dt / m_translatingPointADuration * positionVariation;
                else if (m_translatingPointAInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaPosition = positionVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_translatingPointADuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_translatingPointADuration)));

                if (effectiveElapsedTime > m_translatingPointADuration)
                {
                    SetPointAPosition(m_pointAToPosition);
                    m_translatingPointA = false;
                    OnFinishTranslatingPointA();
                }
                else
                    IncPointAPosition(deltaPosition);
            }
        }
    }

    /***
     * Actions on pointB
     * ***/
    public void TranslatePointBTo(Vector3 toPosition, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_translatingPointB = true;
        m_pointBFromPosition = m_pointBPosition;
        m_pointBToPosition = toPosition;
        m_translatingPointBDuration = duration;
        m_translatingPointBDelay = delay;
        m_translatingPointBElapsedTime = 0;
        m_translatingPointBInterpolationType = interpolType;
    }

    public virtual void SetPointBPosition(Vector3 position)
    {
        m_pointBPosition = position;
        Segment segment = this.gameObject.GetComponent<Segment>();
        segment.SetPointB(position);
        OnPointBPositionChanged();
    }

    public virtual void IncPointBPosition(Vector3 deltaPosition)
    {
        Vector3 fPosition = m_pointBPosition + deltaPosition;
        SetPointBPosition(fPosition);
    }

    public virtual void OnPointBPositionChanged()
    {

    }

    public virtual void OnFinishTranslatingPointB()
    {
        Segment segment = this.GetComponent<Segment>();
        SegmentTreeNode endNode = segment.m_endTreeNode;
        if (endNode != null && endNode.m_active)
        {
            segment.m_parentTree.BuildSegmentsForNode(endNode, true);
            endNode.m_active = false; //deactivate this node as it already started emitting its children segments
        }
    }

    protected virtual void UpdatePointBPosition(float dt)
    {
        if (m_translatingPointB)
        {
            bool inDelay = (m_translatingPointBElapsedTime < m_translatingPointBDelay);
            m_translatingPointBElapsedTime += dt;
            if (m_translatingPointBElapsedTime >= m_translatingPointBDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_translatingPointBElapsedTime - m_translatingPointBDelay;
                float effectiveElapsedTime = m_translatingPointBElapsedTime - m_translatingPointBDelay;
                Vector3 deltaPosition = Vector3.zero;
                Vector3 positionVariation = m_pointBToPosition - m_pointBFromPosition;
                if (m_translatingPointBInterpolationType == InterpolationType.LINEAR)
                    deltaPosition = dt / m_translatingPointBDuration * positionVariation;
                else if (m_translatingPointBInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaPosition = positionVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_translatingPointBDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_translatingPointBDuration)));

                if (effectiveElapsedTime > m_translatingPointBDuration)
                {
                    SetPointBPosition(m_pointBToPosition);
                    m_translatingPointB = false;
                    OnFinishTranslatingPointB();
                }
                else
                    IncPointBPosition(deltaPosition);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        float dt = Time.deltaTime;

        //update values that have to be modified through time
        UpdatePointAPosition(dt);
        UpdatePointBPosition(dt);
    }
}
