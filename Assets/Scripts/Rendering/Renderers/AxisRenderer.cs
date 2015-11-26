using UnityEngine;
using System.Collections.Generic;

public class AxisRenderer : MonoBehaviour
{
    public const float SWEEP_LINE_SPEED = 300.0f;
    public const float DEFAULT_AXIS_THICKNESS = 8.0f;

    //Shared prefabs
    public Material m_plainWhiteMaterial;
    public GameObject m_texQuadPfb;
    public GameObject m_axisSegmentPfb;

    //materials
    public Material m_endpointMaterial;
    public Material m_endpointOuterContourMaterial;

    public AxisSegment m_axisSegment { get; set; } //the segment joining two endpoints
    public GameObject m_endpoint1 { get; set; } //the first endpoint of this axis
    public GameObject m_endpoint2 { get; set; } //the second endpoint of this axis
    public GameObject m_endpoint1Circle { get; set; }
    public GameObject m_endpoint2Circle { get; set; }
    public Vector2 m_endpoint1Position { get; set; } //the world position of the first endpoint
    public Vector2 m_endpoint2Position { get; set; } //the world position of the second endpoint
    public Vector2 m_endpoint1GridPosition { get; set; } //the grid position of the first endpoint
    public Vector2 m_endpoint2GridPosition { get; set; } //the grid position of the second endpoint

    private Grid.GridAnchor m_snappedAnchor; //the current anchor the second axis endpoint has been snapped on
    public float m_snapDistance;

    private GameScene m_gameScene;

    //Type of the axis
    public enum AxisType
    {
        STATIC, //player has finished drawing the axis and symmetry has been done
        DYNAMIC_UNSNAPPED, //player is currently drawing the axis but it is not snapped to a grid anchor
        DYNAMIC_SNAPPED,  //same but this time axis is snapped
        HINT //axis is displayed when the player has requested some help
    }

    public AxisType m_type { get; set; }

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
        Color axisTintColor = gameScene.GetLevelManager().m_currentChapter.GetThemeColors()[4];

        //Set axis endpoints coordinates (both world and grid ones)
        m_endpoint1GridPosition = startPosition;
        m_endpoint2GridPosition = startPosition;

        m_endpoint1Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(startPosition);
        m_endpoint2Position = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(startPosition);

        //One material per axis
        Material axisMaterial = Instantiate(m_plainWhiteMaterial);
        Material endpointMaterial = Instantiate(m_endpointMaterial);
        Material endpointOuterContourMaterial = Instantiate(m_endpointOuterContourMaterial);

        //dimensions of each element
        Vector3 endpointSize = new Vector3(64, 64, 1);
        Vector3 endpointOuterContourSize = new Vector3(128, 128, 1);

        //segment
        GameObject axisSegmentObject = (GameObject)Instantiate(m_axisSegmentPfb);
        GameObjectAnimator axisSegmentAnimator = axisSegmentObject.GetComponent<GameObjectAnimator>();
        axisSegmentAnimator.SetParentTransform(this.transform);
        axisSegmentObject.name = "AxisSegment";
        m_axisSegment = axisSegmentObject.GetComponent<AxisSegment>();
        m_axisSegment.Build(m_endpoint1Position, m_endpoint2Position, DEFAULT_AXIS_THICKNESS, axisMaterial, axisTintColor);

        //endpoint 1
        m_endpoint1 = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint1.name = "AxisEndpoint1";
        UVQuad endpoint1Mesh = m_endpoint1.GetComponent<UVQuad>();
        endpoint1Mesh.Init(endpointMaterial);
        TexturedQuadAnimator endpoint1Animator = m_endpoint1.GetComponent<TexturedQuadAnimator>();
        endpoint1Animator.SetParentTransform(this.transform);        
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));
        endpoint1Animator.SetScale(endpointSize);
        endpoint1Animator.SetColor(axisTintColor);

        //endpoint 1 outer contour
        m_endpoint1Circle = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint1Circle.name = "AxisEndpoint1Circle";
        UVQuad endpoint1CircleMesh = m_endpoint1Circle.GetComponent<UVQuad>();
        endpoint1CircleMesh.Init(endpointOuterContourMaterial);
        TexturedQuadAnimator endpoint1CircleAnimator = m_endpoint1Circle.GetComponent<TexturedQuadAnimator>();
        endpoint1CircleAnimator.SetParentTransform(this.transform);
        endpoint1CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));
        endpoint1CircleAnimator.SetScale(endpointOuterContourSize);
        endpoint1CircleAnimator.SetColor(axisTintColor);

        //endpoint 2
        m_endpoint2 = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint2.name = "AxisEndpoint2";
        UVQuad endpoint2Mesh = m_endpoint2.GetComponent<UVQuad>();
        endpoint2Mesh.Init(endpointMaterial);
        TexturedQuadAnimator endpoint2Animator = m_endpoint2.GetComponent<TexturedQuadAnimator>();
        endpoint2Animator.SetParentTransform(this.transform);
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));
        endpoint2Animator.SetScale(endpointSize);
        endpoint2Animator.SetColor(axisTintColor);

        //endpoint 2 circle
        m_endpoint2Circle = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint2Circle.name = "AxisEndpoint2Circle";
        UVQuad endpoint2CircleMesh = m_endpoint2Circle.GetComponent<UVQuad>();
        endpoint2CircleMesh.Init(endpointOuterContourMaterial);
        TexturedQuadAnimator endpoint2CircleAnimator = m_endpoint2Circle.GetComponent<TexturedQuadAnimator>();
        endpoint2CircleAnimator.SetParentTransform(this.transform);
        endpoint2CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));
        endpoint2CircleAnimator.SetScale(endpointOuterContourSize);
        endpoint2CircleAnimator.SetColor(axisTintColor);

        //ribbon
        CreateRibbon();

        m_type = AxisType.DYNAMIC_UNSNAPPED;
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
        GameObjectAnimator endpoint1CircleAnimator = m_endpoint1Circle.GetComponent<GameObjectAnimator>();
        GameObjectAnimator endpoint2CircleAnimator = m_endpoint2Circle.GetComponent<GameObjectAnimator>();
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));
        endpoint1CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint1Position, 0));
        endpoint2CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(m_endpoint2Position, 0));

        //render ribbon if axis is not too small
        if ((m_endpoint1Position - m_endpoint2Position).sqrMagnitude > 10.0f)
            RenderRibbon();
        else
            m_ribbonMesh.Hide();

    }

    /**
     * Create the ribbon object with related mesh
     * **/
    private void CreateRibbon()
    {
        GameObject ribbonObject = (GameObject) Instantiate(m_ribbonPfb);
        ribbonObject.name = "Ribbon";

        m_ribbonMesh = ribbonObject.GetComponent<RibbonMesh>();
        m_ribbonMesh.Init(Instantiate(m_ribbonMaterial));

        //Set the color of the ribbon
        RibbonAnimator ribbonAnimator = ribbonObject.GetComponent<RibbonAnimator>();
        ribbonAnimator.SetParentTransform(this.transform);
        ribbonAnimator.SetColor(new Color(1, 1, 1, 0.5f));
        ribbonAnimator.SetPosition(Vector3.zero);
    }

    /**
     * Try to snap the second endpoint of the axis to a grid anchor if the distance between the current touch and anchor is the smallest
     * **/
    public bool SnapAxisEndpointToClosestAnchor(Vector2 pointerLocation)
    {
        //if (m_snappedAnchor != null)
        //    return false;

        GameScene gameScene = GetGameScene();
        Grid.GridAnchor[] gridAnchors = gameScene.m_grid.m_anchors;

        float minDistance = float.MaxValue;
        int minDistanceAnchorIndex = 0;

        for (int anchorIndex = 0; anchorIndex != gridAnchors.Length; anchorIndex++)
        {
            Grid.GridAnchor snapAnchor = gridAnchors[anchorIndex];

            float fSqrDistanceToAnchor = (pointerLocation - snapAnchor.m_worldPosition).sqrMagnitude;
            if (fSqrDistanceToAnchor < minDistance)
            {
                minDistance = fSqrDistanceToAnchor;
                minDistanceAnchorIndex = anchorIndex;
            }           
        }

        Grid.GridAnchor closestAnchor = gridAnchors[minDistanceAnchorIndex];
        if (closestAnchor != m_snappedAnchor)
        {
            m_snappedAnchor = closestAnchor;            
            m_endpoint2Position = m_snappedAnchor.m_worldPosition;
            if (MathUtils.AreVec2PointsEqual(m_endpoint1Position, m_endpoint2Position))
            {
                m_type = AxisType.DYNAMIC_UNSNAPPED;
            }
            else
            {
                m_type = AxisType.DYNAMIC_SNAPPED;
            }
            Render(m_endpoint1Position, m_endpoint2Position, false);
            return true;
        }

        return false;
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
            
            m_leftSweepingLine = new SweepingLine(m_endpoint1Position, m_endpoint2Position, -clockwiseAxisNormal);
            m_rightSweepingLine = new SweepingLine(m_endpoint1Position, m_endpoint2Position, clockwiseAxisNormal);

            //debug objects
            m_debugLeftSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
            m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);

            GameObjectAnimator debugLeftSweepLineAnimator = m_debugLeftSweepLineObject.GetComponent<GameObjectAnimator>();
            debugLeftSweepLineAnimator.SetParentTransform(this.transform);
            debugLeftSweepLineAnimator.SetRotationAxis(Vector3.forward);
            debugLeftSweepLineAnimator.SetRotationAngle(axisAngle);
            debugLeftSweepLineAnimator.SetPosition(GetAxisCenterInWorldCoordinates());
            GameObjectAnimator debugRightSweepLineAnimator = m_debugRightSweepLineObject.GetComponent<GameObjectAnimator>();
            debugRightSweepLineAnimator.SetParentTransform(this.transform);
            debugRightSweepLineAnimator.SetRotationAxis(Vector3.forward);
            debugRightSweepLineAnimator.SetRotationAngle(axisAngle);
            debugRightSweepLineAnimator.SetPosition(GetAxisCenterInWorldCoordinates());
        }
        else if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_ONE_SIDE)
        {
            float axisAngle = Mathf.Atan2(axisDirection.y, axisDirection.x) * Mathf.Rad2Deg;

            m_rightSweepingLine = new SweepingLine(m_endpoint1Position, m_endpoint2Position, GetAxisNormal());
            m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);

            GameObjectAnimator debugRightSweepLineAnimator = m_debugRightSweepLineObject.GetComponent<GameObjectAnimator>();
            debugRightSweepLineAnimator.SetParentTransform(this.transform);
            debugRightSweepLineAnimator.SetRotationAxis(Vector3.forward);
            debugRightSweepLineAnimator.SetRotationAngle(axisAngle);
            debugRightSweepLineAnimator.SetPosition(GetAxisCenterInWorldCoordinates());

            m_leftSweepingLine = null;
        }

        //Assign a sweeping line to each dynamic shape
        List<Shape> allShapes = GetShapesHolder().m_shapes;
        for (int i = 0; i != allShapes.Count; i++)
        {
            Shape shape = allShapes[i];
            if (shape.IsDynamic()) 
            {
                ShapeMesh shapeMesh = shape.m_parentMesh;
                if (shapeMesh.m_sweepingLine == null) //check if shape is not already swept by another line
                {
                    if (MathUtils.Determinant(m_endpoint1Position, m_endpoint2Position, shape.GetBarycentre()) >= 0) //on the 'left' of the axis
                        shapeMesh.m_sweepingLine = m_leftSweepingLine;
                    else //on the 'right' of the axis
                        shapeMesh.m_sweepingLine = m_rightSweepingLine;
                }
            }
        }

        //Destroy ribbon
        Destroy(m_ribbonMesh.gameObject);

        //Fade out and scale up axis endpoint outer contours
        Vector3 scaleToValue = new Vector3(256, 256, 1);
        TexturedQuadAnimator endpoint1CircleAnimator = m_endpoint1Circle.GetComponent<TexturedQuadAnimator>();
        endpoint1CircleAnimator.FadeTo(0.0f, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        endpoint1CircleAnimator.ScaleTo(scaleToValue, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR);
        TexturedQuadAnimator endpoint2CircleAnimator = m_endpoint2Circle.GetComponent<TexturedQuadAnimator>();
        endpoint2CircleAnimator.FadeTo(0.0f, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        endpoint2CircleAnimator.ScaleTo(scaleToValue, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR);

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
        return GetGameScene().m_shapesHolder;
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
