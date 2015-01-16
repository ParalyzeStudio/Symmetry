using UnityEngine;
using System.Collections;

public class ContourSegment : UVQuad 
{
    public const float CONTOUR_THICKNESS = 8.0f;

    //coordinates of start and end point of this segment in grid coordinates (line, column)
    public Vector2 m_startPointGrid;
    public Vector2 m_endPointGrid;
    //coordinates of start and end point of this segment in world coordinates
    private Vector2 m_startPoint;
    private Vector2 m_endPoint;
    //store previous values of m_startPointGrid and m_endPointGrid to check if a change occured
    private Vector2 m_prevStartPointGrid;
    private Vector2 m_prevEndPointGrid;

    public ContourSegment(Vector2 startPointGrid, Vector2 endPointGrid)
    {
        m_startPointGrid = startPointGrid;
        m_endPointGrid = endPointGrid;

        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        GridBuilder gridBuilder = gridObject.GetComponent<GridBuilder>();
        m_startPoint = gridBuilder.GetAnchorWorldCoordinatesFromGridCoordinates(m_startPointGrid);
        m_endPoint = gridBuilder.GetAnchorWorldCoordinatesFromGridCoordinates(m_endPointGrid);

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
            m_startPoint = gridBuilder.GetAnchorWorldCoordinatesFromGridCoordinates(m_startPointGrid);
            m_endPoint = gridBuilder.GetAnchorWorldCoordinatesFromGridCoordinates(m_endPointGrid);

            //set the correct position
            Vector2 bridgeCenter = (m_startPoint + m_endPoint) / 2.0f;
            this.transform.localPosition = MathUtils.BuildVector3FromVector2(bridgeCenter, 0);

            //set the correct rotation
            float fRotationAngleRad = Mathf.Atan2((m_endPoint.y - m_startPoint.y), (m_endPoint.x - m_startPoint.x));
            this.transform.rotation = Quaternion.Euler(0, 0, fRotationAngleRad * Mathf.Rad2Deg);

            //set the length
            float fSegmentLength = (m_endPoint - m_startPoint).magnitude;
            this.transform.localScale = new Vector3(fSegmentLength, CONTOUR_THICKNESS, this.transform.localScale.z);

            //and finally the texture range
            Texture tex = this.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            float texWidth = tex.width;
            this.m_textureRange = new Vector4(0, 0, fSegmentLength / texWidth, 1);

            base.Update();
        }
    }
}
