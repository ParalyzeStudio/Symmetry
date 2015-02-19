using UnityEngine;

public class Segment : UVQuad
{
    public Vector2 m_startPoint;
    private Vector2 m_prevStartPoint;
    public Vector2 m_endPoint;
    private Vector2 m_prevEndPoint;
    public float m_thickness;
    private float m_prevThickness;
    public float m_length { get; set; }
    public float m_angle { get; set; }

    //variables used for resizing segment
    //private bool m_resizing;
    //private Vector2 m_fromStartPoint, m_fromEndPoint;
    //private Vector2 m_toStartPoint, m_toEndPoint;
    //private float m_resizingDuration, m_resizingDelay, m_resizingElapsedTime;

    protected override void Awake()
    {
        base.Awake();
        m_prevStartPoint = new Vector2(-float.MinValue, -float.MinValue);
        m_prevEndPoint = new Vector2(-float.MinValue, -float.MinValue);
        m_prevThickness = 0;
        m_length = 0;
        m_angle = 0;
    }

    public void SetLength(float fNewLength)
    {
        if (m_length != fNewLength)
        {
            m_length = fNewLength;
            this.transform.localScale = new Vector3(m_length, m_thickness, this.transform.localScale.z);
            InvalidateEndpoints();
        }
    }

    public void SetAngle(float fNewAngle)
    {
        if (m_angle != fNewAngle)
        {
            m_angle = fNewAngle;
            this.transform.rotation = Quaternion.Euler(0, 0, m_angle);
            InvalidateEndpoints();
        }
    }

    protected void InvalidateEndpoints()
    {
        Vector2 center = this.transform.localPosition;
        Vector2 segmentDirection = new Vector2(Mathf.Cos(m_angle * Mathf.Deg2Rad), Mathf.Sin(m_angle * Mathf.Deg2Rad));
        m_startPoint = center - 0.5f * m_length * segmentDirection;
        m_endPoint = center + 0.5f * m_length * segmentDirection;

        //prevent UpdateEndpoints() from being called by setting previous values directly at current values
        m_prevStartPoint = m_startPoint;
        m_prevEndPoint = m_endPoint;
    }

    protected void UpdateEndpoints()
    {
        if (m_prevStartPoint != m_startPoint || m_prevEndPoint != m_endPoint)
        {
            m_prevStartPoint = m_startPoint;
            m_prevEndPoint = m_endPoint;

            //set the length
            m_length = (m_endPoint - m_startPoint).magnitude;

            //Set the center
            Vector2 center = (m_startPoint + m_endPoint) / 2.0f;
            this.transform.localPosition = GeometryUtils.BuildVector3FromVector2(center, 0);

            //set the angle
            float fRotationAngleRad = Mathf.Atan2((m_endPoint.y - m_startPoint.y), (m_endPoint.x - m_startPoint.x));
            m_angle = fRotationAngleRad * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0, 0, m_angle);

            //set the size
            this.transform.localScale = new Vector3(m_length, m_thickness, this.transform.localScale.z);
        }
    }

    protected void UpdateThickness()
    {
        if (m_thickness != m_prevThickness)
        {
            m_prevThickness = m_thickness;
            this.transform.localScale = new Vector3(m_length, m_thickness, this.transform.localScale.z);
        }
    }

    protected virtual void UpdateTextureRange()
    {
        m_textureRange = new Vector4(0, 0, 1, 1);
    }

    protected virtual void UpdateTextureWrapMode()
    {
        m_textureWrapMode = TextureWrapMode.Clamp;
    }

    protected override void Update()
    {
        //resize
        //if (m_resizing)
        //{
        //    float dt = Time.deltaTime;
        //    m_resizingElapsedTime += Time.deltaTime;

        //    if (m_resizingElapsedTime >= m_resizingDelay && m_resizingElapsedTime < (m_resizingDuration + m_resizingDelay))
        //    {
        //        Vector2 lineDirection = m_toEndPoint - m_toStartPoint;
        //        lineDirection.Normalize();
        //        float startPointDiff = dt / m_resizingDuration * (m_toStartPoint - m_fromStartPoint).magnitude;
        //        float endPointDiff = dt / m_resizingDuration * (m_toEndPoint - m_fromEndPoint).magnitude;
        //        m_startPoint -= (startPointDiff * lineDirection);
        //        m_endPoint += (endPointDiff * lineDirection);
        //    }
        //    else if (m_resizingElapsedTime >= (m_resizingDuration + m_resizingDelay))
        //    {
        //        m_startPoint = m_toStartPoint;
        //        m_endPoint = m_toEndPoint;
        //        m_resizing = false;
        //    }
        //}

        UpdateEndpoints();
        UpdateThickness();
        UpdateTextureRange();
        UpdateTextureWrapMode();
        base.Update();
    }

    //public void ResizeTo(Vector2 toStartPoint, Vector2 toEndPoint, float fDuration, float fDelay = 0.0f)
    //{
    //    m_resizing = true;
    //    m_fromStartPoint = m_startPoint;
    //    m_fromEndPoint = m_startPoint;
    //    m_toStartPoint = toStartPoint;
    //    m_toEndPoint = toEndPoint;
    //    m_resizingDuration = fDuration;
    //    m_resizingDelay = fDelay;
    //    m_resizingElapsedTime = 0;
    //}
}
