using UnityEngine;
using System.Collections.Generic;

public class AxisRenderer : MonoBehaviour
{
    public const float SWEEP_LINE_SPEED = 300.0f;
    public const float DEFAULT_AXIS_THICKNESS = 8.0f;

    public Axis m_axisData { get; set; }

    //Shared prefabs
    public Material m_plainWhiteMaterial;
    public GameObject m_texQuadPfb;
    public GameObject m_axisSegmentPfb;

    //materials
    public Material m_endpointMaterial;
    public Material m_endpointOuterContourMaterial;
    public Material m_indicatingArrowMaterial;

    public AxisSegment m_axisSegment { get; set; } //the segment joining two endpoints
    public GameObject m_endpoint1 { get; set; } //the first endpoint of this axis
    public GameObject m_endpoint2 { get; set; } //the second endpoint of this axis
    private GameObjectAnimator m_leftIndicatingArrowAnimator;
    private GameObjectAnimator m_rightIndicatingArrowAnimator;

    private Grid.GridAnchor m_snappedAnchor; //the current anchor the second axis endpoint has been snapped on
    public float m_snapDistance;

    private GameScene m_gameScene;

    //Strip
    public GameObject m_stripPfb;
    public Material m_stripMaterial;
    public StripMesh m_stripMesh { get; set; }

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

    public void SetAxisData(Axis axis)
    {
        m_axisData = axis;
        axis.m_parentRenderer = this;
    }

    ///**
    // * Build endpoints, segment, strip, indicating arrows and deploy button
    // * **/
    //public void BuildElements()
    //{
    //    Vector3 endpoint1WorldPosition = GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointA);
    //    Vector3 endpoint2WorldPosition = GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointB);

    //    //segment
    //    BuildAxisSegment(endpoint1WorldPosition, endpoint2WorldPosition);

    //    //endpoints
    //    BuildEndpointA(endpoint1WorldPosition);
    //    BuildEndpointB(endpoint2WorldPosition);

    //    //strip        
    //    CreateStrip();

    //    //indicating arrows
    //    BuildIndicatingArrows();
    //}

    /**
    * Call this method when creating a new axis with only one point at disposal
    **/
    public void InitializeRendering()
    {
        BuildEndpointA();
    }

    /**
    * Call this method to finalize the creation of the axis when setting the second endpoint
    **/
    public void FinalizeRendering()
    {
        BuildEndpointB();
        BuildAxisSegment();
        BuildIndicatingArrows();
        CalculateStrip();
        CreateStrip();
    }

    private void BuildEndpointA()
    {
        if (m_endpoint1 == null)
        {
            m_endpoint1 = BuildEndpointAtPosition(GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointA));
            m_endpoint1.name = "AxisEndpoint1";
        }
    }

    private void BuildEndpointB()
    {
        if (m_endpoint2 == null)
        {
            m_endpoint2 = BuildEndpointAtPosition(GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointB));
            m_endpoint2.name = "AxisEndpoint2";
        }
    }

    private GameObject BuildEndpointAtPosition(Vector2 position)
    {
        Material endpointMaterial = Instantiate(m_endpointMaterial);
        Material endpointOuterContourMaterial = Instantiate(m_endpointOuterContourMaterial);
        Color axisTintColor = GetGameScene().GetLevelManager().m_currentChapter.GetThemeColors()[4];

        //dimensions of each element
        Vector3 endpointSize = new Vector3(64, 64, 1);
        Vector3 endpointOuterContourSize = new Vector3(128, 128, 1);

        //endpoint
        GameObject endpoint = (GameObject)Instantiate(m_texQuadPfb);
        UVQuad endpointMesh = endpoint.GetComponent<UVQuad>();
        endpointMesh.Init(endpointMaterial);
        TexturedQuadAnimator endpointAnimator = endpoint.GetComponent<TexturedQuadAnimator>();
        endpointAnimator.SetParentTransform(this.transform);
        endpointAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(position, 0));
        endpointAnimator.SetScale(endpointSize);
        endpointAnimator.SetColor(axisTintColor);

        //endpoint outer contour
        GameObject endpointCircle = (GameObject)Instantiate(m_texQuadPfb);
        endpointCircle.name = "AxisEndpoint1Circle";
        UVQuad endpointCircleMesh = endpointCircle.GetComponent<UVQuad>();
        endpointCircleMesh.Init(endpointOuterContourMaterial);
        TexturedQuadAnimator endpoint1CircleAnimator = endpointCircle.GetComponent<TexturedQuadAnimator>();
        endpoint1CircleAnimator.SetParentTransform(endpoint.transform);
        endpoint1CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(position, 0));
        endpoint1CircleAnimator.SetScale(endpointOuterContourSize);
        endpoint1CircleAnimator.SetColor(axisTintColor);

        return endpoint;
    }

    private void BuildAxisSegment()
    {
        Material axisMaterial = Instantiate(m_plainWhiteMaterial);
        Color axisTintColor = GetGameScene().GetLevelManager().m_currentChapter.GetThemeColors()[4];

        //segment
        GameObject axisSegmentObject = (GameObject)Instantiate(m_axisSegmentPfb);
        GameObjectAnimator axisSegmentAnimator = axisSegmentObject.GetComponent<GameObjectAnimator>();
        axisSegmentAnimator.SetParentTransform(this.transform);
        axisSegmentObject.name = "AxisSegment";
        m_axisSegment = axisSegmentObject.GetComponent<AxisSegment>();
        Vector3 pointAWorldPosition = GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointA);
        Vector3 pointBWorldPosition = GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointB);
        m_axisSegment.Build(pointAWorldPosition, pointBWorldPosition, DEFAULT_AXIS_THICKNESS, axisMaterial, axisTintColor);
    }

    private void BuildIndicatingArrows()
    {
        Material arrowMaterial = Instantiate(m_indicatingArrowMaterial);
        Color arrowColor = GetGameScene().GetLevelManager().m_currentChapter.GetThemeColors()[4];
        Vector3 arrowSize = new Vector3(64, 64, 1);

        GameObject rightIndicatingArrowObject = (GameObject)Instantiate(m_texQuadPfb);
        rightIndicatingArrowObject.name = "IndicatingArrow";
        UVQuad rightIndicatingArrow = rightIndicatingArrowObject.GetComponent<UVQuad>();
        rightIndicatingArrow.Init(arrowMaterial);
        m_rightIndicatingArrowAnimator = rightIndicatingArrowObject.GetComponent<TexturedQuadAnimator>();
        m_rightIndicatingArrowAnimator.SetParentTransform(this.transform);
        m_rightIndicatingArrowAnimator.SetScale(arrowSize);
        m_rightIndicatingArrowAnimator.SetColor(arrowColor);
        m_rightIndicatingArrowAnimator.SetOpacity(0);

        if (m_axisData.m_type == Axis.AxisType.SYMMETRY_AXES_TWO_SIDES)
        {
            GameObject leftIndicatingArrowObject = (GameObject)Instantiate(m_texQuadPfb);
            leftIndicatingArrowObject.name = "IndicatingArrow";
            UVQuad leftIndicatingArrow = leftIndicatingArrowObject.GetComponent<UVQuad>();
            leftIndicatingArrow.Init(arrowMaterial);
            m_leftIndicatingArrowAnimator = leftIndicatingArrowObject.GetComponent<TexturedQuadAnimator>();
            m_leftIndicatingArrowAnimator.SetParentTransform(this.transform);
            m_leftIndicatingArrowAnimator.SetScale(arrowSize);
            m_leftIndicatingArrowAnimator.SetColor(arrowColor);
            m_leftIndicatingArrowAnimator.SetOpacity(0);
        }
    }

    /**
     * Renders the axis between 2 points using grid coordinates
     * **/
    //public void Render()
    //{
    //    GameScene gameScene = GetGameScene();

    //    Vector2 endpoint1WorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointA);
    //    Vector2 endpoint2WorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointB);        

    //    //Set correct points coordinates for segment
    //    m_axisSegment.SetPointA(endpoint1WorldPosition, false);
    //    m_axisSegment.SetPointB(endpoint2WorldPosition, true);

    //    //Set correct position for both endpoints
    //    GameObjectAnimator endpoint1Animator = m_endpoint1.GetComponent<GameObjectAnimator>();        
    //    GameObjectAnimator endpoint2Animator = m_endpoint2.GetComponent<GameObjectAnimator>();
    //    //GameObjectAnimator endpoint1CircleAnimator = m_endpoint1Circle.GetComponent<GameObjectAnimator>();
    //    //GameObjectAnimator endpoint2CircleAnimator = m_endpoint2Circle.GetComponent<GameObjectAnimator>();
    //    endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint1WorldPosition, 0));
    //    endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint2WorldPosition, 0));
    //    //endpoint1CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint1WorldPosition, 0));
    //    //endpoint2CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint2WorldPosition, 0));

    //    //render strip if axis is not too small
    //    if ((endpoint1WorldPosition - endpoint2WorldPosition).sqrMagnitude > 10.0f)
    //        RenderStrip();
    //    else
    //        m_stripMesh.Hide();

    //    GridPoint pointA = m_axisData.m_pointA;
    //    GridPoint pointB = m_axisData.m_pointB;
    //    if (pointA != pointB)
    //    {
    //        float axisAngle = Mathf.Atan2(pointB.Y - pointA.Y, pointB.X - pointA.X) * Mathf.Rad2Deg;
    //        Vector2 normalizedAxisDirection = (pointB - pointA).NormalizeAsVector2();
    //        Vector3 cwAxisNormal = new Vector3(normalizedAxisDirection.y, -normalizedAxisDirection.x, 0);

    //        float distanceFromAxis = 50.0f;

    //        Vector3 rightArrowPosition = 0.5f * (endpoint1WorldPosition + endpoint2WorldPosition);
    //        rightArrowPosition += distanceFromAxis * cwAxisNormal;

    //        m_rightIndicatingArrowAnimator.SetRotationAxis(Vector3.forward);
    //        m_rightIndicatingArrowAnimator.SetRotationAngle(axisAngle - 90);
    //        m_rightIndicatingArrowAnimator.SetPosition(rightArrowPosition);
    //        m_rightIndicatingArrowAnimator.SetOpacity(1);

    //        if (m_axisData.m_type == Axis.AxisType.SYMMETRY_AXES_TWO_SIDES)
    //        {
    //            Vector3 leftArrowPosition = 0.5f * (endpoint1WorldPosition + endpoint2WorldPosition);
    //            leftArrowPosition -= distanceFromAxis * cwAxisNormal;

    //            m_leftIndicatingArrowAnimator.SetRotationAxis(Vector3.forward);
    //            m_leftIndicatingArrowAnimator.SetRotationAngle(axisAngle + 90);
    //            m_leftIndicatingArrowAnimator.SetPosition(leftArrowPosition);
    //            m_leftIndicatingArrowAnimator.SetOpacity(1);
    //        }
    //    }
    //    else //hide some elements, just display the first endpoint
    //    {
    //        if (m_leftIndicatingArrowAnimator != null)
    //            m_leftIndicatingArrowAnimator.SetOpacity(0);
    //        m_rightIndicatingArrowAnimator.SetOpacity(0);
    //    }
    //}

    private void CalculateStrip()
    {
        m_axisData.CalculateStripContour();
    }

    /**
     * Create the strip object with related mesh
     * **/
    private void CreateStrip()
    {
        if (m_axisData.Strip == null)
            return;

        //build the strip mesh
        GameObject stripObject = (GameObject)Instantiate(m_stripPfb);
        stripObject.name = "Strip";

        m_stripMesh = stripObject.GetComponent<StripMesh>();
        m_stripMesh.Init(m_axisData.Strip, Instantiate(m_stripMaterial));

        //Set the color of the strip
        StripAnimator stripAnimator = stripObject.GetComponent<StripAnimator>();
        stripAnimator.SetParentTransform(this.transform);
        stripAnimator.SetColor(new Color(1, 1, 1, 0.5f));
        stripAnimator.SetPosition(Vector3.zero);

        //render it
        RenderStrip();
    }

    /**
     * Try to snap the second endpoint of the axis to a grid anchor if the distance between the current touch and anchor is the smallest
     * **/
    //public bool SnapAxisEndpointToClosestAnchor(Vector2 pointerLocation)
    //{
    //    GameScene gameScene = GetGameScene();

    //    Grid.GridAnchor closestAnchor = gameScene.m_grid.GetClosestGridAnchorForWorldPosition(pointerLocation);
    //    if (closestAnchor == null) //we got out of grid bounds and could not find an anchor
    //        return false;

    //    if (closestAnchor != m_snappedAnchor)
    //    {
    //        m_snappedAnchor = closestAnchor;
    //        if (m_axisData.m_pointA == m_snappedAnchor.m_gridPosition)
    //        {
    //            m_axisData.m_state = Axis.AxisState.DYNAMIC_UNSNAPPED;
    //        }
    //        else
    //        {
    //            m_axisData.m_state = Axis.AxisState.DYNAMIC_SNAPPED;
    //        }
    //        m_axisData.m_pointB = closestAnchor.m_gridPosition;
    //        Render();

    //        return true;
    //    }

    //    return false;
    //}

    /**
     * Find the direction (among all constrained directions) of the axis the player is currently drawing
     * **/
    //public void FindConstrainedDirection(Vector2 pointerLocation, out Vector2 constrainedDirection, out float projectionLength)
    //{
    //    //Find all possible directions for our axis
    //    GameScene gameScene = this.transform.parent.transform.parent.gameObject.GetComponent<GameScene>();
    //    List<GridPoint> constrainedDirections = gameScene.m_constrainedDirections;

    //    float maxDotProduct = float.MinValue;
    //    constrainedDirection = Vector2.zero;
    //    projectionLength = 0;
    //    Vector2 endpoint1WorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_axisData.m_pointA);
    //    for (int iDirectionIndex = 0; iDirectionIndex != constrainedDirections.Count; iDirectionIndex++)
    //    {
    //        Vector2 axisEndpoint1ToPointer = pointerLocation - endpoint1WorldPosition;
    //        float axisEndpoint1ToPointerDistance = axisEndpoint1ToPointer.magnitude; //store vector length before normalizing it
    //        axisEndpoint1ToPointer.Normalize();

    //        float dotProduct = Vector2.Dot(axisEndpoint1ToPointer, constrainedDirections[iDirectionIndex]);
    //        if (dotProduct > maxDotProduct)
    //        {
    //            maxDotProduct = dotProduct;
    //            constrainedDirection = constrainedDirections[iDirectionIndex];
    //            projectionLength = axisEndpoint1ToPointerDistance;
    //        }
    //    }
    //}
    
    /**
     * Renders the strip that indicates which parts of grid elements will be symmetrized
     * **/
    public void RenderStrip()
    {
        if (m_axisData.Strip == null)
            return;

        m_stripMesh.Render();
    }   

    /**
     * Launch sweeping lines from the current axis
     * **/
    public void LaunchSweepingLines()
    {
        m_sweepingLeft = (m_axisData.m_type == Axis.AxisType.SYMMETRY_AXES_TWO_SIDES);
        m_sweepingRight = true;
    }

    /**
     * Callback used when a symmetry is performed on the grid
     * **/
    public void OnPerformSymmetry()
    {
        //GridPoint axisDirection = GetDirection();
        //Grid grid = GetGameScene().m_grid;

        //Vector2 endpoint1WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointA);
        //Vector2 endpoint2WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointB);
        //Vector2 worldAxisCenter = GetWorldCenter();

        ////Create and animate sweeping lines
        //GridPoint clockwiseAxisNormal = GetNormal();
        //Vector2 normalizedClockwiseAxisNormal = clockwiseAxisNormal / clockwiseAxisNormal.magnitude;
        //if (m_twoSidedSymmetry)
        //{
        //    float axisAngle = Mathf.Atan2(axisDirection.Y, axisDirection.X) * Mathf.Rad2Deg;

        //    m_leftSweepingLine = new SweepingLine(endpoint1WorldPosition, endpoint2WorldPosition, -normalizedClockwiseAxisNormal);
        //    m_rightSweepingLine = new SweepingLine(endpoint1WorldPosition, endpoint2WorldPosition, normalizedClockwiseAxisNormal);

        //    //debug objects
        //    m_debugLeftSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
        //    m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);            

        //    GameObjectAnimator debugLeftSweepLineAnimator = m_debugLeftSweepLineObject.GetComponent<GameObjectAnimator>();
        //    debugLeftSweepLineAnimator.SetParentTransform(this.transform);
        //    debugLeftSweepLineAnimator.SetRotationAxis(Vector3.forward);
        //    debugLeftSweepLineAnimator.SetRotationAngle(axisAngle);
        //    debugLeftSweepLineAnimator.SetPosition(worldAxisCenter);
        //    GameObjectAnimator debugRightSweepLineAnimator = m_debugRightSweepLineObject.GetComponent<GameObjectAnimator>();
        //    debugRightSweepLineAnimator.SetParentTransform(this.transform);
        //    debugRightSweepLineAnimator.SetRotationAxis(Vector3.forward);
        //    debugRightSweepLineAnimator.SetRotationAngle(axisAngle);
        //    debugRightSweepLineAnimator.SetPosition(worldAxisCenter);
        //}
        //else
        //{
        //    float axisAngle = Mathf.Atan2(axisDirection.Y, axisDirection.X) * Mathf.Rad2Deg;

        //    m_rightSweepingLine = new SweepingLine(endpoint1WorldPosition, endpoint2WorldPosition, normalizedClockwiseAxisNormal);
        //    m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
        //    m_debugRightSweepLineObject.name = "RightSweepLine";

        //    GameObjectAnimator debugRightSweepLineAnimator = m_debugRightSweepLineObject.GetComponent<GameObjectAnimator>();
        //    debugRightSweepLineAnimator.SetParentTransform(this.transform);
        //    debugRightSweepLineAnimator.SetRotationAxis(Vector3.forward);
        //    debugRightSweepLineAnimator.SetRotationAngle(axisAngle);
        //    debugRightSweepLineAnimator.SetPosition(worldAxisCenter);

        //    m_leftSweepingLine = null;
        //}

        ////Assign a sweeping line to each dynamic shape
        //List<Shape> allShapes = GetShapesHolder().m_shapes;
        //for (int i = 0; i != allShapes.Count; i++)
        //{
        //    Shape shape = allShapes[i];
        //    if (shape.IsDynamic()) 
        //    {
        //        ShapeMesh shapeMesh = shape.m_parentMesh;
        //        if (shapeMesh.m_sweepingLine == null) //check if shape is not already swept by another line
        //        {
        //            if (MathUtils.Determinant(m_pointA, m_pointB, shape.GetBarycentre()) >= 0) //shape is on the 'left' of the axis
        //                shapeMesh.m_sweepingLine = m_leftSweepingLine;
        //            else //on the 'right' of the axis
        //                shapeMesh.m_sweepingLine = m_rightSweepingLine;
        //        }
        //    }
        //}

        //Destroy strip
        Destroy(m_stripMesh.gameObject);

        //Fade out and scale up axis endpoint outer contours
        //Vector3 scaleToValue = new Vector3(256, 256, 1);
        //TexturedQuadAnimator endpoint1CircleAnimator = m_endpoint1Circle.GetComponent<TexturedQuadAnimator>();
        //endpoint1CircleAnimator.FadeTo(0.0f, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        //endpoint1CircleAnimator.ScaleTo(scaleToValue, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR);
        //TexturedQuadAnimator endpoint2CircleAnimator = m_endpoint2Circle.GetComponent<TexturedQuadAnimator>();
        //endpoint2CircleAnimator.FadeTo(0.0f, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        //endpoint2CircleAnimator.ScaleTo(scaleToValue, 2.0f, 0.0f, ValueAnimator.InterpolationType.LINEAR);

        //Start sweeping
        //LaunchSweepingLines();
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
            float sqrDistanceFromAxis = (sweepingLinePosition - m_axisData.GetWorldCenter()).sqrMagnitude;

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
            float sqrDistanceFromAxis = (sweepingLinePosition - m_axisData.GetWorldCenter()).sqrMagnitude;

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
