using UnityEngine;

/**
 * Use this class to render a segment with one color and possible rounded endpoints
 * **/
public class Segment : MonoBehaviour
{
    protected Vector3 m_pointA; //first point of the segment
    protected Vector3 m_pointB; //second point of the segment
    protected float m_thickness; //segment thickness
    protected int m_numSegmentsPerHalfCircle; //number of segments used to draw each half circle on rounded endpoints. Set to 0 to have a rectangle segment
    protected Material m_material; //the material used to render this segment

    //controlling length and angle of the segment manually
    protected float m_length;
    protected float m_angle;

    public const int DEFAULT_NUM_SEGMENTS_PER_HALF_CIRCLE = 16;

    //parent segment tree if this segment is part of a group and child node
    public SegmentTree m_parentTree { get; set; }
    public SegmentTreeNode m_endTreeNode { get; set; }

    //mesh arrays
    protected Mesh m_segmentMesh;
    protected Vector3[] m_meshVertices;
    protected int[] m_meshIndices;
    protected bool m_meshVerticesDirty;
    protected bool m_meshIndicesDirty;
    private int m_prevNumVertices;

    /**
     * Renders the segment with rounded endpoints
     * **/
    protected virtual void RenderInternal(bool bUpdateVertices = true, bool bUpdateIndices = true)
    {
        //First set the position of the segment and define mesh coordinates for pointA and pointB
        Vector3 segmentPosition = 0.5f * (m_pointA + m_pointB);
        this.gameObject.transform.localPosition = segmentPosition;

        //Calculate the distance between pointA and pointB to determine their mesh coordinates
        m_length = (m_pointB - m_pointA).magnitude;
        Vector3 localPointA = new Vector3(-0.5f * m_length, 0, segmentPosition.z);
        Vector3 localPointB = new Vector3(0.5f * m_length, 0, segmentPosition.z);

        //Then find the angle between pointA and pointB and apply rotation to the segment object
        m_angle = Mathf.Atan2(m_pointB.y - m_pointA.y, m_pointB.x - m_pointA.x);
        this.gameObject.transform.rotation = Quaternion.AngleAxis(m_angle * Mathf.Rad2Deg, Vector3.forward);

        //Build the actual mesh if it doesnt exist
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        InitSegmentMesh();        

        if (bUpdateVertices)
        {
            int numVertices = (m_numSegmentsPerHalfCircle <= 1) ? 4 : 2 * (m_numSegmentsPerHalfCircle + 1);
            if (m_meshVertices == null || numVertices != m_prevNumVertices)
            {
                m_meshVertices = new Vector3[numVertices];
                m_prevNumVertices = numVertices;
            }

            //Build vertices
            //left vertices
            m_meshVertices[0] = localPointA - new Vector3(0, 0.5f * m_thickness, 0);
            m_meshVertices[1] = localPointA + new Vector3(0, 0.5f * m_thickness, 0);
            for (int i = 2; i != numVertices / 2; i++)
            {
                float vertexAngle = Mathf.PI / 2.0f + i * Mathf.PI / (float)m_numSegmentsPerHalfCircle;
                m_meshVertices[i] = 0.5f * m_thickness * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);
                m_meshVertices[i] += localPointA;
            }

            //right vertices
            m_meshVertices[numVertices / 2] = localPointB - new Vector3(0, 0.5f * m_thickness, 0);
            m_meshVertices[numVertices / 2 + 1] = localPointB + new Vector3(0, 0.5f * m_thickness, 0);
            for (int i = numVertices / 2 + 2; i != numVertices; i++)
            {
                int offsetIndex = i - numVertices / 2 - 2;
                float vertexAngle = Mathf.PI / 2.0f - offsetIndex * Mathf.PI / (float)m_numSegmentsPerHalfCircle;
                m_meshVertices[i] = 0.5f * m_thickness * new Vector3(Mathf.Cos(vertexAngle), Mathf.Sin(vertexAngle), 0);
                m_meshVertices[i] += localPointB;
            }

            m_meshVerticesDirty = true;

            //build triangles
            if (bUpdateIndices)
            {
                int numTriangles = numVertices - 2;
                m_meshIndices = new int[3 * numTriangles];

                if (numTriangles > 2)
                {
                    //left rounded endpoint
                    for (int i = 0; i != (numTriangles / 2 - 1); i++)
                    {
                        m_meshIndices[3 * i] = 0;
                        m_meshIndices[3 * i + 1] = i + 2;
                        m_meshIndices[3 * i + 2] = i + 1;
                    }

                    //right rounded endpoint
                    for (int i = (numTriangles / 2 - 1); i != (numTriangles - 2); i++)
                    {
                        int offsetIndex = i - (numTriangles / 2 - 1);
                        m_meshIndices[3 * i] = numVertices / 2;
                        m_meshIndices[3 * i + 1] = numVertices / 2 + offsetIndex + 1;
                        m_meshIndices[3 * i + 2] = numVertices / 2 + offsetIndex + 2;
                    }

                    //central segment
                    m_meshIndices[3 * (numTriangles - 2)] = 0;
                    m_meshIndices[3 * (numTriangles - 2) + 1] = 1;
                    m_meshIndices[3 * (numTriangles - 2) + 2] = numVertices / 2 + 1;
                    m_meshIndices[3 * (numTriangles - 1)] = 0;
                    m_meshIndices[3 * (numTriangles - 1) + 1] = numVertices / 2 + 1;
                    m_meshIndices[3 * (numTriangles - 1) + 2] = numVertices / 2;
                }
                else
                {
                    //only the central segment
                    m_meshIndices[0] = 0;
                    m_meshIndices[1] = 1;
                    m_meshIndices[2] = 3;
                    m_meshIndices[3] = 0;
                    m_meshIndices[4] = 3;
                    m_meshIndices[5] = 2;
                }

                m_meshIndicesDirty = true;
            }
        }
    }

    /**
     * Creates the segment mesh if not done already
     * **/
    private void InitSegmentMesh()
    {
        if (m_segmentMesh == null)
        {
            m_segmentMesh = new Mesh();
            m_segmentMesh.name = "SegmentMesh";
            GetComponent<MeshFilter>().sharedMesh = m_segmentMesh;
        }
    }

    /**
     * Transform points from 
     * **/
    public void TransformPointsFromGridCoordinatesToWorldCoordinates(bool bTransformPointA = true, bool bTransformPointB = true)
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;

        if (bTransformPointA)
            m_pointA = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointA);
        if (bTransformPointB)
            m_pointB = gameScene.m_grid.GetPointWorldCoordinatesFromGridCoordinates(m_pointB);
    }

    public virtual void Build(Vector3 pointA, Vector3 pointB, float thickness, Material material, bool bGridPoints, int numSegmentsPerHalfCircle = DEFAULT_NUM_SEGMENTS_PER_HALF_CIRCLE)
    {
        InitBasicVariables(pointA, pointB, thickness, material, bGridPoints, numSegmentsPerHalfCircle);
        RenderInternal(); //builds the mesh
    }

    public void InitBasicVariables(Vector3 pointA, Vector3 pointB, float thickness, Material material, bool bGridPoints, int numSegmentsPerHalfCircle = DEFAULT_NUM_SEGMENTS_PER_HALF_CIRCLE)
    {
        m_pointA = pointA;
        m_pointB = pointB;
        m_thickness = thickness;
        m_numSegmentsPerHalfCircle = numSegmentsPerHalfCircle;

        if (bGridPoints) //this is a grid segment, this means the player is on the game scene
            TransformPointsFromGridCoordinatesToWorldCoordinates();

        SegmentAnimator segmentAnimator = this.gameObject.GetComponent<SegmentAnimator>();
        if (segmentAnimator != null)
        {
            segmentAnimator.m_pointAPosition = pointA;
            segmentAnimator.m_pointBPosition = pointB;
        }

        MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null && material != null)
        {
            meshRenderer.sharedMaterial = material;
        }
    }

    /**
     * Set new coordinates for pointA. Set bGridPoint to true is the passed pointB is in grid coordinates
     * **/
    public virtual void SetPointA(Vector2 pointA, bool bGridPoint = false, bool bRenderSegment = true)
    {
        m_pointA = pointA;

        if (bGridPoint)
            TransformPointsFromGridCoordinatesToWorldCoordinates(true, false);

        if (bRenderSegment)
            RenderInternal(true, false);
    }

    /**
     * Set new coordinates for pointB. Set bGridPoint to true is the passed pointB is in grid coordinates
     * **/
    public virtual void SetPointB(Vector2 pointB, bool bGridPoint = false, bool bRenderSegment = true)
    {
        m_pointB = pointB;

        if (bGridPoint)            
            TransformPointsFromGridCoordinatesToWorldCoordinates(false, true);

        if (bRenderSegment)
            RenderInternal(true, false);
    }

    /**
     * Set new thickness for the segment
     * **/
    public virtual void SetThickness(float thickness)
    {
        m_thickness = thickness;
        RenderInternal(true, false);
    }

    /**
     * Set new number of segments for each rounded endpoint
     * **/
    public void SetNumSegmentsPerHalfCircle(int numSegmentsPerHalfCircle)
    {
        RenderInternal(true, true);
    }

    /**
     * Set manually the length of the segment around the z-axis
     * **/
    public void SetLength(float fLength)
    {
        if (m_length != fLength)
        {
            //if (m_isGridSegment)
            //{
            //    GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;

            //    float gridWorldRatio = gameScene.m_grid.GetGridWorldRatio();
            //    fLength /= gridWorldRatio;
            //}

            m_length = fLength;
            InvalidateEndpoints();
        }
    }

    /**
     * Set manually the angle of the segment around the z-axis
     * **/
    public void SetAngle(float fAngle)
    {
        if (m_angle != fAngle)
        {
            m_angle = fAngle;
            InvalidateEndpoints();
        }
    }

    /**
     * Update the segment points after we manually changed the length or the angle of it
     * **/
    private void InvalidateEndpoints()
    {
        Vector2 center = this.transform.localPosition;
        Vector2 segmentDirection = new Vector2(Mathf.Cos(m_angle * Mathf.Deg2Rad), Mathf.Sin(m_angle * Mathf.Deg2Rad));
        m_pointA = center - 0.5f * m_length * segmentDirection;
        m_pointB = center + 0.5f * m_length * segmentDirection;

        RenderInternal(true, false);
    }

    ///**
    //* Update the segment position, angle and length after we modified its points
    //* **/
    //protected void InvalidatePositionLengthAndAngle()
    //{
    //    //set the length
    //    m_length = (m_pointB - m_pointA).magnitude;

    //    if (m_isGridSegment)
    //    {
    //        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>().m_currentScene;

    //        float gridWorldRatio = gameScene.m_grid.GetGridWorldRatio();
    //        m_length /= gridWorldRatio;
    //    }

    //    //Set the center
    //    Vector2 center = (m_pointA + m_pointB) / 2.0f;
    //    this.transform.localPosition = GeometryUtils.BuildVector3FromVector2(center, 0);

    //    //set the angle
    //    float fRotationAngleRad = Mathf.Atan2((m_pointB.y - m_pointA.y), (m_pointB.x - m_pointA.x));
    //    m_angle = fRotationAngleRad * Mathf.Rad2Deg;
    //    this.transform.rotation = Quaternion.Euler(0, 0, m_angle);

    //    //set the size
    //    this.transform.localScale = new Vector3(m_length, m_thickness, this.transform.localScale.z);
    //}

    public virtual void Update()
    {
        if (m_segmentMesh != null)
        { 
            if (m_meshVerticesDirty)
            {
                m_segmentMesh.vertices = m_meshVertices;
                m_meshVerticesDirty = false;
            }
            if (m_meshIndicesDirty)
            {
                m_segmentMesh.triangles = m_meshIndices;
                m_meshIndicesDirty = false;
            }
        }
    }
}
