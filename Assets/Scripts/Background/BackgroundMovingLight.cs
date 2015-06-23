using UnityEngine;

/**
 * A point that moves along a path where every sample is determined at execution time
 * **/
public class BackgroundMovingLight : UVQuad
{
    public const float POINT_LIGHT_Z_VALUE = -1.0f;

    public Material m_pointLightMaterial;

    //public GameObject m_attachedObject { get; set; } //a GameObject that moves accordingly to this point
    public float m_pointSpeed { get; set; } //the speed of the point
    public Vector2 m_pointDirection { get; set; }
    public Vector2 m_startPoint { get; set; } //the start point of the path segment the point is currently covering
    public Vector2 m_currentPoint { get; set; } //current coordinates of the point
    public float m_segmentLength { get; set; } //the length of the path segment that the point has to cover before changing direction

    public bool m_finished { get; set; } //did this light reach one of the screen border
    public bool m_evacuated { get; set; } //does this light needs to be evacuated from parent renderer lights list

    public override void InitQuadMesh()
    {
        base.InitQuadMesh();

        GetComponent<MeshFilter>().sharedMesh.name = "PointLightMesh";
        //GetComponent<MeshRenderer>().sharedMaterial = Instantiate(m_pointLightMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = m_pointLightMaterial; //do not clone the material for each point light for the moment
        GetComponent<TexturedQuadAnimator>().SetColor(Color.white);
    }

    public void Update()
    {
        float dt = Time.deltaTime;
        
        float coveredLength = (m_currentPoint - m_startPoint).magnitude;
        if (coveredLength > m_segmentLength)
        {
            if (m_finished)
            {
                m_evacuated = true;
                return;
            }

            m_startPoint += (m_pointDirection * m_segmentLength); //new segment start point becomes the previous segment target point
            m_currentPoint = m_startPoint;

            Vector2 screenSize = ScreenUtils.GetScreenSize();

            //check if current point has reached one of the screen borders
            if (m_currentPoint.x <= -0.5f * screenSize.x ||
                m_currentPoint.x >= 0.5f * screenSize.x ||
                m_currentPoint.y <= -0.5f * screenSize.y ||
                m_currentPoint.y >= 0.5f * screenSize.y)
            {
                m_finished = true;
            }
            else
            {
                //Randomly choose a new direction
                float randomValue = Random.value;

                //if (randomValue <= 1 / 3.0f) //rotate the direction by -PI / 3
                //    m_pointDirection = Quaternion.AngleAxis(-60, Vector3.forward) * m_pointDirection;
                //else if (randomValue > 1 / 3.0f && randomValue <= 2 / 3.0f) //rotate the direction by PI / 3
                //    m_pointDirection = Quaternion.AngleAxis(60, Vector3.forward) * m_pointDirection;
                //else keep the same direction
            }
        }
        else
            m_currentPoint += dt * m_pointSpeed * m_pointDirection;

        this.transform.localPosition = GeometryUtils.BuildVector3FromVector2(m_currentPoint, POINT_LIGHT_Z_VALUE);
        //m_attachedObject.transform.localPosition = m_currentPoint;
    }
}
