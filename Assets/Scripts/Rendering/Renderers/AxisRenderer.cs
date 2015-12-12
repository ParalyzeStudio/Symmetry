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
    public GridPoint m_pointA { get; set; } //the position of the first endpoint
    public GridPoint m_pointB { get; set; } //the position of the second endpoint

    private Grid.GridAnchor m_snappedAnchor; //the current anchor the second axis endpoint has been snapped on
    public float m_snapDistance;

    private GameScene m_gameScene;

    //Type of the axis
    public enum AxisType
    {
        STATIC_PENDING, //axis is waiting for the player to unstack this symmetry
        STATIC_DONE, //player has finished drawing the axis and symmetry has been done
        DYNAMIC_UNSNAPPED, //player is currently drawing the axis but it is not snapped to a grid anchor
        DYNAMIC_SNAPPED,  //same but this time axis is snapped
        HINT //axis is displayed when the player has requested some help
    }

    public AxisType m_type { get; set; }

    //Strip
    public GameObject m_stripPfb;
    public Material m_stripMaterial;
    public Strip m_stripData { get; set; } //the points defining the contour of this strip
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

    /**
     * Build both endpoints and segment
     * **/
    public void BuildElements(GridPoint pointA, GridPoint pointB)
    {
        GameScene gameScene = GetGameScene();
        Color axisTintColor = gameScene.GetLevelManager().m_currentChapter.GetThemeColors()[4];

        m_pointA = pointA;
        m_pointB = pointB;

        Vector3 endpoint1WorldPosition = GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointA);
        Vector3 endpoint2WorldPosition = GetGameScene().m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointB);

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
        m_axisSegment.Build(endpoint1WorldPosition, endpoint2WorldPosition, DEFAULT_AXIS_THICKNESS, axisMaterial, axisTintColor);

        //endpoint 1
        m_endpoint1 = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint1.name = "AxisEndpoint1";
        UVQuad endpoint1Mesh = m_endpoint1.GetComponent<UVQuad>();
        endpoint1Mesh.Init(endpointMaterial);
        TexturedQuadAnimator endpoint1Animator = m_endpoint1.GetComponent<TexturedQuadAnimator>();
        endpoint1Animator.SetParentTransform(this.transform);
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint1WorldPosition, 0));
        endpoint1Animator.SetScale(endpointSize);
        endpoint1Animator.SetColor(axisTintColor);

        //endpoint 1 outer contour
        m_endpoint1Circle = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint1Circle.name = "AxisEndpoint1Circle";
        UVQuad endpoint1CircleMesh = m_endpoint1Circle.GetComponent<UVQuad>();
        endpoint1CircleMesh.Init(endpointOuterContourMaterial);
        TexturedQuadAnimator endpoint1CircleAnimator = m_endpoint1Circle.GetComponent<TexturedQuadAnimator>();
        endpoint1CircleAnimator.SetParentTransform(this.transform);
        endpoint1CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint1WorldPosition, 0));
        endpoint1CircleAnimator.SetScale(endpointOuterContourSize);
        endpoint1CircleAnimator.SetColor(axisTintColor);

        //endpoint 2
        m_endpoint2 = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint2.name = "AxisEndpoint2";
        UVQuad endpoint2Mesh = m_endpoint2.GetComponent<UVQuad>();
        endpoint2Mesh.Init(endpointMaterial);
        TexturedQuadAnimator endpoint2Animator = m_endpoint2.GetComponent<TexturedQuadAnimator>();
        endpoint2Animator.SetParentTransform(this.transform);
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint2WorldPosition, 0));
        endpoint2Animator.SetScale(endpointSize);
        endpoint2Animator.SetColor(axisTintColor);

        //endpoint 2 circle
        m_endpoint2Circle = (GameObject)Instantiate(m_texQuadPfb);
        m_endpoint2Circle.name = "AxisEndpoint2Circle";
        UVQuad endpoint2CircleMesh = m_endpoint2Circle.GetComponent<UVQuad>();
        endpoint2CircleMesh.Init(endpointOuterContourMaterial);
        TexturedQuadAnimator endpoint2CircleAnimator = m_endpoint2Circle.GetComponent<TexturedQuadAnimator>();
        endpoint2CircleAnimator.SetParentTransform(this.transform);
        endpoint2CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint2WorldPosition, 0));
        endpoint2CircleAnimator.SetScale(endpointOuterContourSize);
        endpoint2CircleAnimator.SetColor(axisTintColor);

        //strip
        CreateStrip();
    }

    /**
     * Renders the axis between 2 points using grid coordinates
     * **/
    public void Render(GridPoint pointA, GridPoint pointB)
    {
        GameScene gameScene = GetGameScene();

        m_pointA = pointA;
        m_pointB = pointB;

        Vector2 endpoint1WorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(pointA);
        Vector2 endpoint2WorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(pointB);        

        //Set correct points coordinates for segment
        m_axisSegment.SetPointA(endpoint1WorldPosition, false);
        m_axisSegment.SetPointB(endpoint2WorldPosition, true);
        
        //Set correct position for both endpoints
        GameObjectAnimator endpoint1Animator = m_endpoint1.GetComponent<GameObjectAnimator>();        
        GameObjectAnimator endpoint2Animator = m_endpoint2.GetComponent<GameObjectAnimator>();
        GameObjectAnimator endpoint1CircleAnimator = m_endpoint1Circle.GetComponent<GameObjectAnimator>();
        GameObjectAnimator endpoint2CircleAnimator = m_endpoint2Circle.GetComponent<GameObjectAnimator>();
        endpoint1Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint1WorldPosition, 0));
        endpoint2Animator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint2WorldPosition, 0));
        endpoint1CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint1WorldPosition, 0));
        endpoint2CircleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(endpoint2WorldPosition, 0));

        //render strip if axis is not too small
        if ((endpoint1WorldPosition - endpoint2WorldPosition).sqrMagnitude > 10.0f)
            RenderStrip();
        else
            m_stripMesh.Hide();
    }

    /**
     * Create the strip object with related mesh
     * **/
    public void CreateStrip(bool bCreateMesh = true)
    {
        //initialize the strip data
        m_stripData = new Strip(this);
        m_stripData.CalculateContour();

        //build the strip mesh
        if (bCreateMesh)
        {
            GameObject stripObject = (GameObject)Instantiate(m_stripPfb);
            stripObject.name = "Strip";

            m_stripMesh = stripObject.GetComponent<StripMesh>();
            m_stripMesh.Init(m_stripData, Instantiate(m_stripMaterial));

            //Set the color of the strip
            StripAnimator stripAnimator = stripObject.GetComponent<StripAnimator>();
            stripAnimator.SetParentTransform(this.transform);
            stripAnimator.SetColor(new Color(1, 1, 1, 0.5f));
            stripAnimator.SetPosition(Vector3.zero);
        }
    }

    /**
     * Try to snap the second endpoint of the axis to a grid anchor if the distance between the current touch and anchor is the smallest
     * **/
    public bool SnapAxisEndpointToClosestAnchor(Vector2 pointerLocation)
    {
        GameScene gameScene = GetGameScene();

        Grid.GridAnchor closestAnchor = gameScene.m_grid.GetClosestGridAnchorForWorldPosition(pointerLocation);
        if (closestAnchor == null) //we got out of grid bounds and could not find an anchor
            return false;

        if (closestAnchor != m_snappedAnchor)
        {
            m_snappedAnchor = closestAnchor;
            if (m_pointA == m_snappedAnchor.m_gridPosition)
            {
                m_type = AxisType.DYNAMIC_UNSNAPPED;
            }
            else
            {
                m_type = AxisType.DYNAMIC_SNAPPED;
            }
            Render(m_pointA, closestAnchor.m_gridPosition);
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
        Vector2 endpoint1WorldPosition = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointA);
        for (int iDirectionIndex = 0; iDirectionIndex != constrainedDirections.Count; iDirectionIndex++)
        {
            Vector2 axisEndpoint1ToPointer = pointerLocation - endpoint1WorldPosition;
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
     * Renders the strip that indicates which parts of grid elements will be symmetrized
     * **/
    public void RenderStrip()
    {
        m_stripData.CalculateContour();
        m_stripMesh.Render();
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
        GridPoint axisDirection = GetDirection();
        Grid grid = GetGameScene().m_grid;

        Vector2 endpoint1WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointA);
        Vector2 endpoint2WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointB);
        Vector2 worldAxisCenter = GetWorldCenter();

        //Create and animate sweeping lines
        GridPoint clockwiseAxisNormal = GetNormal();
        Vector2 normalizedClockwiseAxisNormal = clockwiseAxisNormal / clockwiseAxisNormal.magnitude;
        if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_TWO_SIDES)
        {
            float axisAngle = Mathf.Atan2(axisDirection.Y, axisDirection.X) * Mathf.Rad2Deg;

            m_leftSweepingLine = new SweepingLine(endpoint1WorldPosition, endpoint2WorldPosition, -normalizedClockwiseAxisNormal);
            m_rightSweepingLine = new SweepingLine(endpoint1WorldPosition, endpoint2WorldPosition, normalizedClockwiseAxisNormal);

            //debug objects
            m_debugLeftSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
            m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);            

            GameObjectAnimator debugLeftSweepLineAnimator = m_debugLeftSweepLineObject.GetComponent<GameObjectAnimator>();
            debugLeftSweepLineAnimator.SetParentTransform(this.transform);
            debugLeftSweepLineAnimator.SetRotationAxis(Vector3.forward);
            debugLeftSweepLineAnimator.SetRotationAngle(axisAngle);
            debugLeftSweepLineAnimator.SetPosition(worldAxisCenter);
            GameObjectAnimator debugRightSweepLineAnimator = m_debugRightSweepLineObject.GetComponent<GameObjectAnimator>();
            debugRightSweepLineAnimator.SetParentTransform(this.transform);
            debugRightSweepLineAnimator.SetRotationAxis(Vector3.forward);
            debugRightSweepLineAnimator.SetRotationAngle(axisAngle);
            debugRightSweepLineAnimator.SetPosition(worldAxisCenter);
        }
        else if (symmetryType == Symmetrizer.SymmetryType.SYMMETRY_AXES_ONE_SIDE)
        {
            float axisAngle = Mathf.Atan2(axisDirection.Y, axisDirection.X) * Mathf.Rad2Deg;

            m_rightSweepingLine = new SweepingLine(endpoint1WorldPosition, endpoint2WorldPosition, normalizedClockwiseAxisNormal);
            m_debugRightSweepLineObject = (GameObject)Instantiate(m_sweepLinePfb);
            m_debugRightSweepLineObject.name = "RightSweepLine";

            GameObjectAnimator debugRightSweepLineAnimator = m_debugRightSweepLineObject.GetComponent<GameObjectAnimator>();
            debugRightSweepLineAnimator.SetParentTransform(this.transform);
            debugRightSweepLineAnimator.SetRotationAxis(Vector3.forward);
            debugRightSweepLineAnimator.SetRotationAngle(axisAngle);
            debugRightSweepLineAnimator.SetPosition(worldAxisCenter);

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
                    if (MathUtils.Determinant(m_pointA, m_pointB, shape.GetBarycentre()) >= 0) //shape is on the 'left' of the axis
                        shapeMesh.m_sweepingLine = m_leftSweepingLine;
                    else //on the 'right' of the axis
                        shapeMesh.m_sweepingLine = m_rightSweepingLine;
                }
            }
        }

        //Destroy strip
        Destroy(m_stripMesh.gameObject);

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

    /**
     * Return the coordinates of the middle of the axis
     * **/
    public GridPoint GetCenter()
    {
        return 0.5f * (m_pointA + m_pointB);
    }

    /**
    * Return the world coordinates of the middle of this axis
    * **/
    public Vector2 GetWorldCenter()
    {
        Grid grid = GetGameScene().m_grid;
        Vector2 endpoint1WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointA);
        Vector2 endpoint2WorldPosition = grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointB);
        Vector2 axisWorldCenter = 0.5f * (endpoint1WorldPosition + endpoint2WorldPosition);
        return axisWorldCenter;
    }

    /**
    * Return the direction of this axis without normalizing it
    * **/
    public GridPoint GetDirection()
    {
        GridPoint axisDirection = m_pointB - m_pointA;
        return axisDirection;
    }

    /**
     * Return the normal of this axis without normalizing it
     * Can choose between cw or ccw order
     * **/
    public GridPoint GetNormal(bool bClockwiseOrder = true)
    {
        GridPoint axisDirection = GetDirection();
        if (bClockwiseOrder)
            return new GridPoint(axisDirection.Y, -axisDirection.X, false);
        else
            return new GridPoint(-axisDirection.Y, axisDirection.X, false);
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
            float sqrDistanceFromAxis = (sweepingLinePosition - GetWorldCenter()).sqrMagnitude;

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
            float sqrDistanceFromAxis = (sweepingLinePosition - GetWorldCenter()).sqrMagnitude;

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
