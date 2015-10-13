using UnityEngine;
using System.Collections.Generic;

public class AxisRenderer : MonoBehaviour
{
    public const float SWEEP_LINE_SPEED = 500.0f;
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
    private Shapes m_shapesHolder;

    //Ribbon
    public GameObject m_ribbonPfb;
    public Material m_ribbonMaterial;
    private RibbonMesh m_ribbonMesh;
    public RibbonMesh Ribbon
    {
        get
        {
            return m_ribbonMesh;
        }
    }

    //Sweeping lines that reveal shapes. Axis and sweeping line are parallel so define this line with two points 
    //that are the projections of both axis points A and B and a translation direction
    public class SweepingLine
    {
        private Vector2 m_pointA; //the projection of the axis point A on this line
        public Vector2 PointA
        {
            get
            {
                return m_pointA;
            }
        }
        
        private Vector2 m_pointB; //the projection of the axis point B on this line
        public Vector2 PointB
        {
            get
            {
                return m_pointB;
            }
        }

        private Vector2 m_translationDirection;
        public Vector2 TranslationDirection
        {
            get
            {
                return m_translationDirection;
            }
        }

        public SweepingLine(Vector2 pointA, Vector2 pointB, Vector2 translationDirection) 
        { 
            m_pointA = pointA; 
            m_pointB = pointB;
            m_translationDirection = translationDirection;
        }

        public void Translate(float dx) { m_pointA += dx * m_translationDirection; m_pointB += dx * m_translationDirection; }
    }

    public SweepingLine m_leftSweepingLine { get; set; } //line that will sweep the area on the 'left' of the axis or null if not applicable
    public SweepingLine m_rightSweepingLine { get; set; } //line that will sweep the area on the 'right' of the axis or null if not applicable
    private bool m_sweepingLeft;
    private bool m_sweepingRight;

    public GameObject m_sweepLinePfb;
    private GameObject m_debugLeftSweepLineObject;
    private GameObject m_debugRightSweepLineObject;

    ////Circle that grow and sweep an area
    //public struct SweepingCircle
    //{
    //    public Vector2 m_center;
    //    public float m_radius;
    //}

    //public SweepingLine m_sweepingCircle;

    public void Awake()
    {
        //m_buildStatus = BuildStatus.NOTHING;
        m_axisSegment = null;
        m_endpoint1 = null;
        m_endpoint2 = null;
        m_snappedAnchor = null;

        m_leftSweepingLine = null;
        m_rightSweepingLine = null;
        m_sweepingLeft = false;
        m_sweepingRight = false;
    }

    /**
     * Build both endpoints and segment
     * **/
    public void BuildElements(Vector2 startPosition)
    {
        GameScene gameScene = GetGameScene();

        //Set axis endpoints coordinates (both world and grid ones)
        m_endpoint1GridPosition = startPosition;
        m_endpoint2GridPosition = startPosition;

        m_endpoint1Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(startPosition);
        m_endpoint2Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(startPosition);

        //One material per axis
        Material axisMaterial = m_plainWhiteMaterial = Instantiate(m_plainWhiteMaterial);

        //segment
        GameObject axisSegmentObject = (GameObject)Instantiate(m_axisSegmentPfb);
        axisSegmentObject.transform.parent = this.transform;
        axisSegmentObject.name = "AxisSegment";
        m_axisSegment = axisSegmentObject.GetComponent<AxisSegment>();
        m_axisSegment.Build(m_endpoint1Position, m_endpoint2Position, DEFAULT_AXIS_THICKNESS, m_plainWhiteMaterial, Color.black);

        //endpoint 1
        m_endpoint1 = (GameObject)Instantiate(m_circleMeshPfb);
        m_endpoint1.transform.parent = this.gameObject.transform;
        m_endpoint1.name = "AxisEndpoint1";
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
        m_endpoint2.name = "AxisEndpoint2";
        CircleMesh endpoint2Mesh = m_endpoint2.GetComponent<CircleMesh>();
        endpoint2Mesh.Init(axisMaterial);
        CircleMeshAnimator endpoint2Animator = m_endpoint2.GetComponent<CircleMeshAnimator>();
        endpoint2Animator.SetNumSegments(6, false);
        endpoint2Animator.SetInnerRadius(0, false);
        endpoint2Animator.SetOuterRadius(8, true);
        endpoint2Animator.SetColor(Color.white);
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));

        //ribbon
        CreateRibbon();
    }

    /**
     * Renders the axis between 2 points using grid coordinates
     * **/
    public void Render(Vector2 pointA, Vector2 pointB, bool bGridPoints)
    {
        //pointA = new Vector2(5, 6);
        //pointB = new Vector2(6, 5);
        //bGridPoints = true;

        GameScene gameScene = GetGameScene();

        if (bGridPoints)
        {
            m_endpoint1GridPosition = pointA;
            m_endpoint2GridPosition = pointB;

            m_endpoint1Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(pointA);
            m_endpoint2Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(pointB);

            //Debug.Log("endpoint1:" + m_endpoint1Position + " endpoint2:" + m_endpoint2Position);
        }
        else
        {
            m_endpoint1Position = pointA;
            m_endpoint2Position = pointB;

            m_endpoint1GridPosition = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointA);
            m_endpoint2GridPosition = gameScene.m_grid.GetPointGridCoordinatesFromWorldCoordinates(pointB);

            //Debug.Log("endpoint1:" + m_endpoint1Position + " endpoint2:" + m_endpoint2Position);
        }

        //Set correct points coordinates for segment
        m_axisSegment.SetPointA(m_endpoint1Position, false);
        m_axisSegment.SetPointB(m_endpoint2Position, true);
        
        //Set correct position for both endpoints
        GameObjectAnimator endpoint1Animator = m_endpoint1.GetComponent<GameObjectAnimator>();
        GameObjectAnimator endpoint2Animator = m_endpoint2.GetComponent<GameObjectAnimator>();
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));

        RenderRibbon();
    }

    /**
     * Create the ribbon object with related mesh
     * **/
    private void CreateRibbon()
    {
        GameObject ribbonObject = (GameObject) Instantiate(m_ribbonPfb);
        ribbonObject.transform.parent = this.transform;
        ribbonObject.name = "Ribbon";

        m_ribbonMesh = ribbonObject.GetComponent<RibbonMesh>();
        m_ribbonMesh.Init();

        //Set a simple material on the ribbon
        MeshRenderer ribbonRenderer = ribbonObject.GetComponent<MeshRenderer>();
        ribbonRenderer.material = Instantiate(m_ribbonMaterial);

        //Set the color of the ribbon
        RibbonAnimator ribbonAnimator = ribbonObject.GetComponent<RibbonAnimator>();
        ribbonAnimator.SetColor(new Color(1, 1, 1, 0.5f));
        ribbonAnimator.SetPosition(Vector3.zero);
    }

    /**
     * Try to snap the second endpoint to a grid anchor if the distance to it is small enough
     * **/
    public bool TryToSnapAxisEndpointToClosestAnchor()
    {
        if (m_snappedAnchor != null)
            return false;

        GameScene gameScene = GetGameScene();
        GameObject[] gridAnchors = gameScene.m_grid.m_anchors;

        for (int anchorIndex = 0; anchorIndex != gridAnchors.Length; anchorIndex++)
        {
            GameObject snapAnchor = gridAnchors[anchorIndex];

            Vector2 snapAnchorGridPosition = gameScene.m_grid.GetAnchorGridCoordinatesForAnchorIndex(anchorIndex);
            Vector2 snapAnchorWorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(snapAnchorGridPosition);

            float fDistanceToAnchor = (m_endpoint2Position - snapAnchorWorldPosition).magnitude;

            //do not snap on the current snapped anchor or on the anchor under the first axis endpoint
            if (snapAnchor != m_snappedAnchor && snapAnchorGridPosition != m_endpoint1GridPosition)
            {
                if (fDistanceToAnchor <= m_snapDistance) //we can snap the anchor
                {
                    m_snappedAnchor = snapAnchor;
                    //Render(m_endpoint1Position, snapAnchorPosition, false);
                    Render(m_endpoint1GridPosition, snapAnchorGridPosition, true);

                    //gameScene.m_axes.LaunchSnapCircleAnimation(snapAnchorPosition);

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
        Vector2 snappedAnchorPosition2 = m_snappedAnchor.GetComponent<GameObjectAnimator>().GetPosition();
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
        m_ribbonMesh.CalculateRibbonForAxis(this);
    }

    /**
     * Split ribbon in two sub-shapes for each side of the axis
     * **/
    public void SplitRibbon(Symmetrizer.SymmetryType symmetryType)
    {
        m_ribbonMesh.SplitByAxis(symmetryType);
    }

    /**
     * Launch sweeping lines from the current axis
     * **/
    public void LaunchSweepingLines()
    {
        if (m_leftSweepingLine != null)
            m_sweepingLeft = true;
        if (m_rightSweepingLine != null)
            m_sweepingRight = true;
    }

    /**
     * Callback used when a symmetry is performed on the grid
     * **/
    public void OnPerformSymmetry(Symmetrizer.SymmetryType symmetryType)
    {
        Vector2 axisDirection = GetAxisDirection();

        //Create and animate sweeping lines
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_TWO_SIDES)
        {
            Vector2 clockwiseAxisNormal = GetAxisNormal();
            
            float axisAngle = Mathf.Atan2(axisDirection.y, axisDirection.x) * Mathf.Rad2Deg;
            
            //m_leftSweepingLine = new SweepingLine(m_endpoint1Position, m_endpoint2Position, -clockwiseAxisNormal);
            m_rightSweepingLine = new SweepingLine(m_endpoint1Position, m_endpoint2Position, clockwiseAxisNormal);

            //debug objects
            //Quaternion sweepLineRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), GeometryUtils.BuildVector3FromVector2(GetAxisDirection(), 0));
            Quaternion sweepLineRotation = Quaternion.AngleAxis(axisAngle, Vector3.forward);
            //m_debugLeftSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
            //m_debugLeftSweepLineObject.transform.parent = this.transform;
            //m_debugLeftSweepLineObject.transform.localRotation = sweepLineRotation;
            //m_debugLeftSweepLineObject.transform.localPosition = GetAxisCenterInWorldCoordinates();
            m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
            m_debugRightSweepLineObject.transform.parent = this.transform;
            m_debugRightSweepLineObject.transform.localRotation = sweepLineRotation;
            m_debugRightSweepLineObject.transform.localPosition = GetAxisCenterInWorldCoordinates();
        }
        else if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_ONE_SIDE)
        {
            m_rightSweepingLine = new SweepingLine(m_endpoint1Position, m_endpoint2Position, GetAxisNormal());
            Quaternion sweepLineRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), GeometryUtils.BuildVector3FromVector2(GetAxisDirection(), 0));
            m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
            m_debugRightSweepLineObject.transform.localRotation = sweepLineRotation;
            m_debugRightSweepLineObject.transform.localPosition = GetAxisCenterInWorldCoordinates();

            m_leftSweepingLine = null;
        }

        //Assign a sweeping line to each dynamic shape
        List<Shape> allShapes = GetShapesHolder().m_shapes;
        for (int i = 0; i != allShapes.Count; i++)
        {
            Shape shape = allShapes[i];
            ShapeMesh shapeMesh = shape.m_parentMesh;
            if (MathUtils.Determinant(m_endpoint1Position, m_endpoint2Position, shape.GetBarycentre()) >= 0) //on the 'left' of the axis
                shapeMesh.m_sweepingLine = m_leftSweepingLine;
            else //on the 'right' of the axis
                shapeMesh.m_sweepingLine = m_rightSweepingLine;
        }

        //Destroy ribbon
        Destroy(m_ribbonMesh.gameObject);

        //Start sweeping
        LaunchSweepingLines();
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

    private Shapes GetShapesHolder()
    {
        if (m_shapesHolder == null)
            m_shapesHolder = GetGameScene().GetComponentInChildren<Shapes>();

        return m_shapesHolder;
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        if (m_sweepingLeft)
        {
            m_leftSweepingLine.Translate(dt * SWEEP_LINE_SPEED);

            Vector2 sweepingLinePosition = 0.5f * (m_leftSweepingLine.PointA + m_leftSweepingLine.PointB);
            float sqrDistanceFromAxis = (sweepingLinePosition - GetAxisCenterInWorldCoordinates()).sqrMagnitude;

            if (sqrDistanceFromAxis < 1000000) //TMP delete sweeping line when far enough (out of screen)
            {
                //move the debug object
                Vector2 debugLeftSweepLinePosition = 0.5f * (m_leftSweepingLine.PointA + m_leftSweepingLine.PointB);
                m_debugLeftSweepLineObject.transform.localPosition = debugLeftSweepLinePosition;
                GetShapesHolder().SweepDynamicShapesWithLine(m_leftSweepingLine, false);
            }
            else
            {
                m_leftSweepingLine = null;
                m_sweepingLeft = false;
                Destroy(m_debugLeftSweepLineObject);
                m_debugLeftSweepLineObject = null;
            }
        }
        if (m_sweepingRight)
        {
            m_rightSweepingLine.Translate(dt * SWEEP_LINE_SPEED);

            Vector2 sweepingLinePosition = 0.5f * (m_rightSweepingLine.PointA + m_rightSweepingLine.PointB);
            float sqrDistanceFromAxis = (sweepingLinePosition - GetAxisCenterInWorldCoordinates()).sqrMagnitude;

            if (sqrDistanceFromAxis < 1000000) //TMP delete sweeping line when far enough (out of screen)
            {
                //move the debug object
                Vector3 debugRightSweepLinePosition = 0.5f * (m_rightSweepingLine.PointA + m_rightSweepingLine.PointB);
                m_debugRightSweepLineObject.transform.localPosition = debugRightSweepLinePosition;
                GetShapesHolder().SweepDynamicShapesWithLine(m_rightSweepingLine, true);
            }
            else
            {
                m_rightSweepingLine = null;
                m_sweepingRight = false;
                Destroy(m_debugRightSweepLineObject);
                m_debugRightSweepLineObject = null;
            }
        }
    }
}
