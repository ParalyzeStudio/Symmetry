﻿using UnityEngine;
using System.Collections.Generic;

public class AxisRenderer : MonoBehaviour
{
    public const float DEFAULT_AXIS_THICKNESS = 8.0f;

    //Shared prefabs
    public Material m_plainWhiteMaterial;
    public GameObject m_axisSegmentPfb;
    public GameObject m_circleMeshPfb;

    public AxisSegment m_axisSegment { get; set; } //the segment joining two endpoints
    public GameObject m_endpoint1 { get; set; } //the first endpoint of this axis
    public GameObject m_endpoint2 { get; set; } //the second endpoint of this axis
    public Vector2 m_endpoint1Position { get; set; } //the world position of the first endpoint
    public Vector2 m_endpoint2Position { get; set; } //the world position of the second endpoint
    public Vector2 m_endpoint1GridPosition { get; set; } //the grid position of the first endpoint
    public Vector2 m_endpoint2GridPosition { get; set; } //the grid position of the second endpoint

    private GameObject m_snappedAnchor; //the current anchor the second axis endpoint has been snapped on
    public float m_snapDistance;

    private GameScene m_gameScene;

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
        m_snappedAnchor = null;
    }

    /**
     * Build both endpoints and segment
     * **/
    public void BuildElements(Vector2 startPosition, bool bGridPoints)
    {
        GameScene gameScene = GetGameScene();

        //Set axis endpoints coordinates (both world and grid ones)
        if (bGridPoints)
        {
            m_endpoint1GridPosition = startPosition;
            m_endpoint2GridPosition = startPosition;

            m_endpoint1Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(startPosition);
            m_endpoint2Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(startPosition);
        }
        else
        {
            m_endpoint1Position = startPosition;
            m_endpoint2Position = startPosition;

            m_endpoint1GridPosition = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(startPosition);
            m_endpoint2GridPosition = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(startPosition);
        }

        //One material per axis
        Material axisMaterial = m_plainWhiteMaterial = Instantiate(m_plainWhiteMaterial);

        //segment
        GameObject axisSegmentObject = (GameObject)Instantiate(m_axisSegmentPfb);
        m_axisSegment = axisSegmentObject.GetComponent<AxisSegment>();
        m_axisSegment.Build(m_endpoint1GridPosition, m_endpoint2GridPosition, DEFAULT_AXIS_THICKNESS, m_plainWhiteMaterial, Color.black);
        m_axisSegment.transform.parent = this.gameObject.transform;

        //endpoint 1
        m_endpoint1 = (GameObject)Instantiate(m_circleMeshPfb);
        m_endpoint1.transform.parent = this.gameObject.transform;
        CircleMesh endpoint1Mesh = m_endpoint1.GetComponent<CircleMesh>();
        endpoint1Mesh.Init(axisMaterial);
        CircleMeshAnimator endpoint1Animator = m_endpoint1.GetComponent<CircleMeshAnimator>();
        endpoint1Animator.SetNumSegments(6, false);
        endpoint1Animator.SetInnerRadius(0, false);
        endpoint1Animator.SetOuterRadius(8, true);
        endpoint1Animator.SetColor(Color.white);
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));

        //endpoint 2
        m_endpoint2 = (GameObject)Instantiate(m_circleMeshPfb);
        m_endpoint2.transform.parent = this.gameObject.transform;
        CircleMesh endpoint2Mesh = m_endpoint2.GetComponent<CircleMesh>();
        endpoint2Mesh.Init(axisMaterial);
        CircleMeshAnimator endpoint2Animator = m_endpoint2.GetComponent<CircleMeshAnimator>();
        endpoint2Animator.SetNumSegments(6, false);
        endpoint2Animator.SetInnerRadius(0, false);
        endpoint2Animator.SetOuterRadius(8, true);
        endpoint2Animator.SetColor(Color.white);
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));
    }

    /**
     * Renders the axis between 2 points. Set bGridPoints to true if points are in grid coordinates
     * **/
    public void Render(Vector2 pointA, Vector2 pointB, bool bGridPoints)
    {
        GameScene gameScene = GetGameScene();

        if (bGridPoints)
        {
            m_endpoint1GridPosition = pointA;
            m_endpoint2GridPosition = pointB;

            m_endpoint1Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(pointA);
            m_endpoint2Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(pointB);
        }
        else
        {
            m_endpoint1Position = pointA;
            m_endpoint2Position = pointB;
                       
            m_endpoint1GridPosition = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointA);
            m_endpoint2GridPosition = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointB);
        }

        //Set correct points coordinates for segment
        m_axisSegment.SetPointA(m_endpoint1Position, false, false);
        m_axisSegment.SetPointB(m_endpoint2Position, false, true);
        
        //Set correct position for both endpoints
        GameObjectAnimator endpoint1Animator = m_endpoint1.GetComponent<GameObjectAnimator>();
        GameObjectAnimator endpoint2Animator = m_endpoint2.GetComponent<GameObjectAnimator>();
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));
    }

    /**
     * Try to snap the second endpoint to a grid anchor if the distance to it is small enough ( <= m_snapDistance)
     * **/
    public bool TryToSnapAxisEndpointToClosestAnchor()
    {
        GameScene gameScene = GetGameScene();
        GameObject[] gridAnchors = gameScene.m_grid.m_anchors;

        for (int anchorIndex = 0; anchorIndex != gridAnchors.Length; anchorIndex++)
        {
            GameObject snapAnchor = gridAnchors[anchorIndex];

            Vector2 snapAnchorGridPosition = gameScene.m_grid.GetAnchorGridCoordinatesForAnchorIndex(anchorIndex);

            Vector2 snapAnchorPosition = snapAnchor.transform.position;
            float fDistanceToAnchor = (m_endpoint2Position - snapAnchorPosition).magnitude;

            //do not snap on the current snapped anchor or on the anchor under the first axis endpoint
            if (snapAnchor != m_snappedAnchor && snapAnchorGridPosition != m_endpoint1GridPosition)
            {
                if (fDistanceToAnchor <= m_snapDistance) //we can snap the anchor
                {
                    m_snappedAnchor = snapAnchor;
                    Render(m_endpoint1Position, snapAnchorPosition, false);

                    gameScene.m_axes.LaunchSnapCircleAnimation(snapAnchorPosition);

                    return true;
                }
            }
        }

        return false;
    }

    /**
     * Try to unsnap a the axis second endpoint if the distance to it is big enough ( > m_snapDistance)
     * **/
    public void TryToUnsnap(Vector2 pointerLocation)
    {
        Vector2 snappedAnchorPosition = m_snappedAnchor.transform.position;
        float fDistanceFromMouseToAnchor = (pointerLocation - snappedAnchorPosition).magnitude;
        if (fDistanceFromMouseToAnchor > m_snapDistance)
        {
            m_snappedAnchor = null;
            Render(m_endpoint1Position, pointerLocation, false);
        }
    }

    public bool isAxisSnapped()
    {
        return m_snappedAnchor != null;
    }

    public void FindConstrainedDirection(Vector2 pointerLocation, out Vector2 constrainedDirection, out float projectionLength)
    {
        //Find all possible directions for our axis
        GameScene gameScene = this.transform.parent.transform.parent.gameObject.GetComponent<GameScene>();
        List<Vector2> constrainedDirections = gameScene.m_constrainedDirections;

        float maxDotProduct = float.MinValue;
        constrainedDirection = Vector2.zero;
        projectionLength = 0;
        for (int iDirectionIndex = 0; iDirectionIndex != constrainedDirections.Count; iDirectionIndex++)
        {
            Vector2 axisEndpoint1ToPointer = pointerLocation - m_endpoint1Position;
            float axisEndpoint1ToPointerDistance = axisEndpoint1ToPointer.magnitude; //store vector length before normalizing it
            axisEndpoint1ToPointer.Normalize();

            float dotProduct = Vector2.Dot(axisEndpoint1ToPointer, constrainedDirections[iDirectionIndex]);
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                constrainedDirection = constrainedDirections[iDirectionIndex];
                projectionLength = axisEndpoint1ToPointerDistance;
            }
        }
    }

    /**
     * Renders the ribbon that indicates which parts of shapes will be symmetrized
     * **/
    public void RenderRibbon()
    {

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

    public GameScene GetGameScene()
    {
        if (m_gameScene == null)
        {
            m_gameScene = this.transform.parent.transform.parent.gameObject.GetComponent<GameScene>();
        }

        return m_gameScene;
    }
}