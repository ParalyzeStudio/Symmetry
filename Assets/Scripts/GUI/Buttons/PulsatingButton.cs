using UnityEngine;

public class PulsatingButton : MonoBehaviour
{
    public GameObject m_circleMeshPfb;
    public GameObject m_texQuadPfb;
    public Material m_transpColorMaterial;
    public Material m_glowMaterial;

    //size of the button
    private float m_size;
    public float Size
    {
        get
        {
            return m_size;
        }
    }

    //pulsations
    private const float PULSATION_PERIOD = 1.0f;
    private const float PULSE_TIMESPAN = 3.0f;
    private float m_fadingHexagonsGenerationTimeInterval;
    private float m_fadingHexagonsGenerationElapsedTime;
    private bool m_pulsating;

    public void Build(float size, Color color, float thickness = 30.0f, float pulsationPeriod = PULSATION_PERIOD)
    {
        //Vector2 screenSize = ScreenUtils.GetScreenSize();
        m_size = size;
        
        ////Center hexagon
        //GameObject button = (GameObject)Instantiate(m_circleMeshPfb);
        //button.name = "PulsatingButton";

        ////Set the correct material on it
        //Material hexagonMaterial = Instantiate(m_transpColorMaterial);

        ////Build the mesh
        //CircleMesh hexaMesh = button.GetComponent<CircleMesh>();
        //hexaMesh.Init(hexagonMaterial);

        //CircleMeshAnimator hexaMeshAnimator = button.GetComponent<CircleMeshAnimator>();
        //hexaMeshAnimator.SetParentTransform(this.transform);
        //hexaMeshAnimator.SetPosition(new Vector3(0, 0, 0));
        //hexaMeshAnimator.SetNumSegments(6, false);
        //hexaMeshAnimator.SetInnerRadius(outerRadius - thickness, true);
        //hexaMeshAnimator.SetOuterRadius(outerRadius, true);
        //hexaMeshAnimator.SetColor(color);

        //init some variables        
        m_pulsating = true;
        m_fadingHexagonsGenerationElapsedTime = pulsationPeriod; //the first hexagon will start fading after some delay (0.5 sec)
        m_fadingHexagonsGenerationTimeInterval = pulsationPeriod;
    }

    public void StopPulsating()
    {
        m_pulsating = false;
    }

    /**
     * Launch a new fading hexagon from the blurry contour of this chapter slot
     * **/
    private void LaunchFadingHexagon()
    {
        GameObject fadingHexagonObject = (GameObject)Instantiate(m_texQuadPfb); //instantiate a second contour that will serve as a fading object
        fadingHexagonObject.name = "FadingHexagon";

        UVQuad fadingHexagon = fadingHexagonObject.GetComponent<UVQuad>();
        fadingHexagon.Init(Instantiate(m_glowMaterial));

        Vector3 fadingHexagonSize = new Vector3(m_size, m_size, 1);

        TexturedQuadAnimator fadingHexagonAnimator = fadingHexagonObject.GetComponent<TexturedQuadAnimator>();
        fadingHexagonAnimator.SetParentTransform(this.transform);
        fadingHexagonAnimator.SetScale(fadingHexagonSize);
        fadingHexagonAnimator.SetPosition(new Vector3(0, 0, -1)); //set the contour above hexagon
        fadingHexagonAnimator.SetOpacity(1); //for the moment hide the fading hexagon

        //launch animation on hexagon
        Vector3 scaleToSize = new Vector3(2.0f * m_size, 2.0f * m_size, 1);
        fadingHexagonAnimator.ScaleTo(scaleToSize, PULSE_TIMESPAN);
        fadingHexagonAnimator.FadeTo(0.0f, PULSE_TIMESPAN, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
    }

    public void Update()
    {
        if (m_pulsating)
        {
            float dt = Time.deltaTime;

            if (m_fadingHexagonsGenerationElapsedTime > m_fadingHexagonsGenerationTimeInterval)
            {
                LaunchFadingHexagon();
                m_fadingHexagonsGenerationElapsedTime = 0.0f;
            }
            else
                m_fadingHexagonsGenerationElapsedTime += dt;
        }
    }
}
