using UnityEngine;
using System.Collections.Generic;

public class AxisRenderer : MonoBehaviour
{
    public GameObject m_axisSegmentPfb;
    public GameObject m_symmetryAxisEndpointPfb;
    public GameObject m_axisSegment { get; set; } //the segment joining two endpoints
    public GameObject m_endpoint1 { get; set; } //the first endpoint of this axis
    public GameObject m_endpoint2 { get; set; } //the second endpoint of this axis
    public Vector2 m_endpoint1Position { get; set; } //the world position of the first endpoint
    public Vector2 m_endpoint2Position { get; set; } //the world position of the second endpoint
    public Vector2 m_endpoint1GridPosition { get; set; } //the grid position of the first endpoint
    public Vector2 m_endpoint2GridPosition { get; set; } //the grid position of the second endpoint

    //public enum BuildStatus
    //{
    //    NOTHING, //nothing has been built yet
    //    FIRST_ENDPOINT_SET, //player has set the first endpoint only
    //    SECOND_ENDPOINT_SET, //player has set both endpoints
    //    BUILDING_SEGMENT, //segment is being drawn
    //    DONE_BUILDING //everything has been set properly
    //};

    //public BuildStatus m_buildStatus { get; set; } //is the axis built

    public void Awake()
    {
        //m_buildStatus = BuildStatus.NOTHING;
        m_axisSegment = null;
        m_endpoint1 = null;
        m_endpoint2 = null;
    }

    /**
     * Build both endpoints and segment
     * **/
    public void BuildElements()
    {
        m_axisSegment = (GameObject)Instantiate(m_axisSegmentPfb);
        m_axisSegment.transform.parent = this.gameObject.transform;

        m_endpoint1 = (GameObject)Instantiate(m_symmetryAxisEndpointPfb);
        m_endpoint1.transform.parent = this.gameObject.transform;

        m_endpoint2 = (GameObject)Instantiate(m_symmetryAxisEndpointPfb);
        m_endpoint2.transform.parent = this.gameObject.transform;
    }

    /**
     * Renders the axis between 2 points. Set bGridPoints to true if points are in grid coordinates
     * **/
    public void Render(Vector2 pointA, Vector2 pointB, bool bGridPoints)
    {
        if (bGridPoints)
        {
            m_endpoint1GridPosition = pointA;
            m_endpoint2GridPosition = pointB;

            GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
            m_endpoint1Position = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(pointA);
            m_endpoint2Position = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(pointB);
        }
        else
        {
            m_endpoint1Position = pointA;
            m_endpoint2Position = pointB;

            GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
            m_endpoint1GridPosition = gameScene.m_grid.GetGridCoordinatesFromWorldCoordinates(pointA);
            m_endpoint2GridPosition = gameScene.m_grid.GetGridCoordinatesFromWorldCoordinates(pointB);
        }

        //Set correct points coordinates for segment
        AxisSegment axisSegment = m_axisSegment.GetComponent<AxisSegment>();
        axisSegment.SetStartPoint(m_endpoint1Position);
        axisSegment.SetEndPoint(m_endpoint2Position);
        
        //Set correct position for both endpoints
        GameObjectAnimator endpoint1Animator = m_endpoint1.GetComponent<GameObjectAnimator>();
        GameObjectAnimator endpoint2Animator = m_endpoint2.GetComponent<GameObjectAnimator>();
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));
    }

    public Vector2 GetAxisCenterInWorldCoordinates()
    {
        return 0.5f * (m_endpoint1Position + m_endpoint2Position);
    }

    public Vector2 GetAxisCenterInGridCoordinates()
    {
        return 0.5f * (m_endpoint1GridPosition + m_endpoint2GridPosition);
    }

    public Vector2 GetAxisDirection()
    {
        Vector2 axisDirection = m_endpoint2Position - m_endpoint1Position;
        axisDirection.Normalize();
        return axisDirection;
    }

    public Vector2 GetAxisNormal(bool bClockwiseOrder = true)
    {
        Vector2 axisDirection = GetAxisDirection();
        if (bClockwiseOrder)
            return new Vector2(axisDirection.y, -axisDirection.x);
        else
            return new Vector2(-axisDirection.y, axisDirection.x);
    }
}
