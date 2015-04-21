using UnityEngine;
using System.Collections.Generic;

public class AxisRenderer : MonoBehaviour
{
    public GameObject m_axisSegmentPfb;
    public GameObject m_symmetryAxisEndpointPfb;
    public GameObject m_circlePfb;
    public GameObject m_axisSegment { get; set; } //the segment joining two endpoints
    public GameObject m_endpoint1 { get; set; } //the first endpoint of this axis
    public GameObject m_endpoint2 { get; set; } //the second endpoint of this axis
    public Vector2 m_endpoint1Position { get; set; } //the world position of the first endpoint
    public Vector2 m_endpoint2Position { get; set; } //the world position of the second endpoint
    public Vector2 m_endpoint1GridPosition { get; set; } //the grid position of the first endpoint
    public Vector2 m_endpoint2GridPosition { get; set; } //the grid position of the second endpoint

    private GameObject m_snappedAnchor; //the current anchor the second axis endpoint has been snapped on
    public float m_snapDistance;

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

    /**
     * Try to snap the second endpoint to a grid anchor if the distance to it is small enough ( <= m_snapDistance)
     * **/
    public bool TryToSnapAxisEndpointToClosestAnchor()
    {
        GameScene gameScene = this.transform.parent.transform.parent.gameObject.GetComponent<GameScene>();
        List<GameObject> gridAnchors = gameScene.m_grid.m_anchors;

        for (int anchorIndex = 0; anchorIndex != gridAnchors.Count; anchorIndex++)
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

                    LaunchSnapCircleAnimation(snapAnchorPosition);

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

    /**
     * Simple animation with circle scaling up and fading out
     * **/
    public void LaunchSnapCircleAnimation(Vector2 position)
    {
        GameObject clonedCircle = (GameObject) Instantiate(m_circlePfb);
        //clonedCircle.transform.parent = this.transform;
        clonedCircle.transform.localPosition = GeometryUtils.BuildVector3FromVector2(position, -10);

        CircleAnimator circleAnimator = clonedCircle.GetComponent<CircleAnimator>();
        circleAnimator.SetInnerRadius(0);
        circleAnimator.SetThickness(2);
        circleAnimator.SetNumSegments(64);
        circleAnimator.SetColor(Color.black);

        circleAnimator.SetInnerRadius(0);
        circleAnimator.AnimateRadiusTo(20, 0.3f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        circleAnimator.SetOpacity(1);
        circleAnimator.FadeTo(0, 0.3f);
    }

    public void FindConstrainedDirection(Vector2 pointerLocation, out Vector2 constrainedDirection, out float projectionLength)
    {
        //Find all possible directions for our axis
        GameScene gameScene = this.transform.parent.transform.parent.gameObject.GetComponent<GameScene>();
        List<Vector2> directions = gameScene.GetDirectionsForSymmetryActiveActionTag();

        float maxDotProduct = float.MinValue;
        constrainedDirection = Vector2.zero;
        projectionLength = 0;
        for (int iDirectionIndex = 0; iDirectionIndex != directions.Count; iDirectionIndex++)
        {
            Vector2 axisEndpoint1ToPointer = pointerLocation - m_endpoint1Position;
            float axisEndpoint1ToPointerDistance = axisEndpoint1ToPointer.magnitude; //store vector length before normalizing it
            axisEndpoint1ToPointer.Normalize();

            float dotProduct = Vector2.Dot(axisEndpoint1ToPointer, directions[iDirectionIndex]);
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                constrainedDirection = directions[iDirectionIndex];
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
}
