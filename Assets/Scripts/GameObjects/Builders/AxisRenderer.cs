﻿using UnityEngine;
using System.Collections.Generic;

public class AxisRenderer : MonoBehaviour
{
    public GameObject m_axisSegmentPfb;
    public GameObject m_symmetryAxisEndpointPfb;
    public GameObject m_axisSegment { get; set; } //the segment joining two endpoints
    public GameObject m_endpoint1 { get; set; } //the first endpoint of this axis
    public GameObject m_endpoint2 { get; set; } //the second endpoint of this axis
    public Vector2 m_endpoint1GridPosition { get; set; } //the grid position of the first endpoint
    public Vector2 m_endpoint2GridPosition { get; set; } //the grid position of the second endpoint

    public enum BuildStatus
    {
        NOTHING, //nothing has been built yet
        FIRST_ENDPOINT_SET, //player has set the first endpoint only
        SECOND_ENDPOINT_SET, //player has set both endpoints
        BUILDING_SEGMENT, //segment is being drawn
        DONE_BUILDING //everything has been set properly
    };

    public BuildStatus m_buildStatus { get; set; } //is the axis built

    public void Awake()
    {
        m_buildStatus = BuildStatus.NOTHING;
    }

    /**
     * Build and endpoint at a certain location on the grid
     * If the second endpoint is set the axis will be drawn immediately
     * **/
    public void BuildEndpointAtGridPosition(Vector2 gridPosition)
    {
        GameObject gridObject = GameObject.FindGameObjectWithTag("Grid");
        GridBuilder gridBuilder = (GridBuilder)gridObject.GetComponent<GridBuilder>();
        Vector2 worldPosition = gridBuilder.GetWorldCoordinatesFromGridCoordinates(gridPosition);

        if (m_endpoint1 == null) //no endpoints set yet, initiate the first one
        {
            m_endpoint1 = (GameObject)Instantiate(m_symmetryAxisEndpointPfb);
            m_endpoint1.transform.parent = this.transform; //set this endpoint as a child of the axis itself
            m_endpoint1.transform.localPosition = GeometryUtils.BuildVector3FromVector2(worldPosition, 0);
            m_endpoint1GridPosition = gridPosition;
            m_buildStatus = BuildStatus.FIRST_ENDPOINT_SET;

            SceneManager sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
            GameScene gameScene = (GameScene)sceneManager.m_currentScene;
            if (gameScene.m_selectedActionButton != null)
            {
                Symmetrizer.SymmetryType symmetryType = this.gameObject.GetComponent<Symmetrizer>().GetSymmetryTypeForActionTag(gameScene.m_activeActionTag);
                gridBuilder.RenderConstraintAnchors(m_endpoint1GridPosition, symmetryType);
            }
        }
        else
        {
            if (!gridPosition.Equals(m_endpoint1GridPosition))
            {
                m_endpoint2 = (GameObject)Instantiate(m_symmetryAxisEndpointPfb);
                m_endpoint2.transform.parent = this.transform; //set this endpoint as a child of the axis itself
                m_endpoint2.transform.localPosition = GeometryUtils.BuildVector3FromVector2(worldPosition, 0);
                m_endpoint2GridPosition = gridPosition;
                m_buildStatus = BuildStatus.SECOND_ENDPOINT_SET;
                BuildAxisSegment();

                gridBuilder.RemoveConstraintAnchors();
            }
        }
    }

    /**
     * Build the segment joining the two endpoints of this axis
     * **/
    public void BuildAxisSegment()
    {
        m_axisSegment = (GameObject)Instantiate(m_axisSegmentPfb);
        m_axisSegment.transform.parent = this.transform;
        AxisSegment segment = m_axisSegment.GetComponent<AxisSegment>();
        segment.m_startPointGrid = m_endpoint1GridPosition;
        segment.m_endPointGrid = m_endpoint2GridPosition;
        m_buildStatus = BuildStatus.BUILDING_SEGMENT;
        m_buildStatus = BuildStatus.DONE_BUILDING;

        OnFinishBuildAxisSegment();
    }

    /**
     * Callback that is called when axis segment has being drawn
     * **/
    public void OnFinishBuildAxisSegment()
    {
        Symmetrizer symmetrizer = this.gameObject.GetComponent<Symmetrizer>();
        symmetrizer.SymmetrizeByAxis();
    }

    public Vector2 GetAxisCenterInWorldCoordinates()
    {
        GridBuilder gridBuilder = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridBuilder>();

        Vector2 gridAxisCenter = 0.5f * (m_endpoint1GridPosition + m_endpoint2GridPosition);
        Vector2 worldAxisCenter = gridBuilder.GetWorldCoordinatesFromGridCoordinates(gridAxisCenter);

        return worldAxisCenter;
    }

    public Vector2 GetAxisDirection()
    {
        Vector2 axisDirection = m_endpoint2GridPosition - m_endpoint1GridPosition;
        axisDirection.Normalize();
        return axisDirection;
    }
}
