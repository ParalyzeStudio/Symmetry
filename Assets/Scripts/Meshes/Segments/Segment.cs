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
    public Vector2 m_center { get; set; }

    protected void UpdateEndpoints()
    {
        if (m_prevStartPoint != m_startPoint || m_prevEndPoint != m_endPoint)
        {
            m_prevStartPoint = m_startPoint;
            m_prevEndPoint = m_endPoint;

            //set the length
            m_length = (m_endPoint - m_startPoint).magnitude;

            //Set the center
            m_center = (m_startPoint + m_endPoint) / 2.0f;
            this.transform.localPosition = GeometryUtils.BuildVector3FromVector2(m_center, 0);

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
        UpdateEndpoints();
        UpdateThickness();
        UpdateTextureRange();
        UpdateTextureWrapMode();
        base.Update();
    }
}
