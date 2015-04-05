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
            InvalidatePoints();
        }
    }

    public void SetAngle(float fNewAngle)
    {
        if (m_angle != fNewAngle)
        {
            m_angle = fNewAngle;
            this.transform.rotation = Quaternion.Euler(0, 0, m_angle);
            InvalidatePoints();
        }
    }

    public void SetStartPoint(Vector2 startPoint)
    {
        m_startPoint = startPoint;
        InvalidatePositionLengthAndAngle();
    }

    public void SetEndPoint(Vector2 endPoint)
    {
        m_endPoint = endPoint;
        InvalidatePositionLengthAndAngle();
    }

    /**
     * Update the segment points after we manually changed the length or the angle of it
     * **/
    protected void InvalidatePoints()
    {
        Vector2 center = this.transform.localPosition;
        Vector2 segmentDirection = new Vector2(Mathf.Cos(m_angle * Mathf.Deg2Rad), Mathf.Sin(m_angle * Mathf.Deg2Rad));
        m_startPoint = center - 0.5f * m_length * segmentDirection;
        m_endPoint = center + 0.5f * m_length * segmentDirection;

        //prevent UpdateEndpoints() from being called by setting previous values directly at current values
        m_prevStartPoint = m_startPoint;
        m_prevEndPoint = m_endPoint;
    }

    /**
     * Update the segment position, angle and length after we modified its points
     * **/
    protected void InvalidatePositionLengthAndAngle()
    {
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

    /**
     * Update segment points from the inspector 
     * **/
    protected void UpdatePoints()
    {
        if (m_prevStartPoint != m_startPoint || m_prevEndPoint != m_endPoint)
        {
            m_prevStartPoint = m_startPoint;
            m_prevEndPoint = m_endPoint;

            InvalidatePositionLengthAndAngle();
        }
    }

    /**
     * Update the thickness of the segment in the inspector. This method is normally never called at runtime
     * **/
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
        UpdatePoints();
        UpdateThickness();
        UpdateTextureRange();
        UpdateTextureWrapMode();
        base.Update();
    }
}
