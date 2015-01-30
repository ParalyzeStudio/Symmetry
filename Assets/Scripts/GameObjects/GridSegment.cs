using UnityEngine;

/**
 * Class to render a segment with both endpoints as grid coordinates points (column, line)
 * **/
public class GridSegment : UVQuad
{
    //properties of the quad
    protected float m_length;
    public float m_thickness;
    private float m_prevThickness;

    //coordinates of start and end point of this segment in grid coordinates (column, line)
    public Vector2 m_startPointGrid;
    public Vector2 m_endPointGrid;
    //store previous values of m_startPointGrid and m_endPointGrid to check if a change occured
    protected Vector2 m_prevStartPointGrid;
    protected Vector2 m_prevEndPointGrid;

    protected override void Start()
    {
        base.Start();
        m_prevThickness = 0.0f;
        m_prevStartPointGrid = new Vector2(-1, -1);
        m_prevEndPointGrid = new Vector2(-1, -1);
    }

    protected override void Update()
    {
        if (m_prevStartPointGrid != m_startPointGrid ||
            m_prevEndPointGrid != m_endPointGrid)
        {
            m_prevStartPointGrid = m_startPointGrid;
            m_prevEndPointGrid = m_endPointGrid;

            GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
            GridBuilder gridBuilder = gridObject.GetComponent<GridBuilder>();
            Vector2 startPoint = gridBuilder.GetWorldCoordinatesFromGridCoordinates(m_startPointGrid);
            Vector2 endPoint = gridBuilder.GetWorldCoordinatesFromGridCoordinates(m_endPointGrid);
            m_length = (endPoint - startPoint).magnitude;

            //set the correct position
            Vector2 segmentCenter = (startPoint + endPoint) / 2.0f;
            this.transform.localPosition = GeometryUtils.BuildVector3FromVector2(segmentCenter, 0);

            //set the correct rotation
            float fRotationAngleRad = Mathf.Atan2((endPoint.y - startPoint.y), (endPoint.x - startPoint.x));
            this.transform.rotation = Quaternion.Euler(0, 0, fRotationAngleRad * Mathf.Rad2Deg);

            //set the length
            this.transform.localScale = new Vector3(m_length, m_thickness, this.transform.localScale.z);
            
            //update the texture range
            UpdateTextureRange();

            base.Update();
        }

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
}

