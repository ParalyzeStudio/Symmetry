using UnityEngine;

public class CounterElement : MonoBehaviour
{
    public GameObject m_texQuadPfb;

    //materials used for each of the above game objects MeshRenderers
    private Material m_filledMaterial;
    private Material m_emptyMaterial;
    private Material m_outerContourMaterial;

    private Color m_color;

    private GameObject m_filledObject;
    private GameObject m_emptyObject;
    private GameObject m_outerContourObject;

    public enum CounterElementStatus
    {
        NONE = 0,
        DONE, //an action has been realised and this element is marked as done
        CURRENT, //display an overlay for the CURRENT element
        WAITING //this element is waiting for previous ones to complete
    };

    public CounterElementStatus m_status { get; set; }

    public void Awake()
    {
        m_status = CounterElementStatus.NONE;
    }

    /**
     * Set up materials to build the counter element for each of the 3 status it may have.
     * DONE: filledMaterial + outerContourMaterial
     * CURRENT: filledMaterial
     * WAITING: emptyMaterial
     * **/
    public void Init(Color color, Material filledMaterial, Material emptyMaterial, Material outerContourMaterial)
    {
        m_color = color;
        m_filledMaterial = filledMaterial;
        m_emptyMaterial = emptyMaterial;
        m_outerContourMaterial = outerContourMaterial;
    }

    private void SetAsDoneElement()
    {
        //build the outer contour element and animate it
        m_outerContourObject = (GameObject)Instantiate(m_texQuadPfb);
        m_outerContourObject.name = "OuterContour";

        UVQuad outerContour = m_outerContourObject.GetComponent<UVQuad>();
        outerContour.Init(m_outerContourMaterial);

        TexturedQuadAnimator outerContourAnimator = m_outerContourObject.GetComponent<TexturedQuadAnimator>();
        outerContourAnimator.SetParentTransform(this.transform);
        outerContourAnimator.SetScale(new Vector3(128, 128, 1));
        outerContourAnimator.SetColor(m_color);
    }

    private void SetAsCurrentElement()
    {
        if (m_status == CounterElementStatus.NONE) //first time we build this element (the first one in the list when game starts)
        {
            //build the filled element
            m_filledObject = (GameObject)Instantiate(m_texQuadPfb);
            m_filledObject.name = "Filled";

            UVQuad filledElement = m_filledObject.GetComponent<UVQuad>();
            filledElement.Init(m_filledMaterial);

            TexturedQuadAnimator filledElementAnimator = m_filledObject.GetComponent<TexturedQuadAnimator>();
            filledElementAnimator.SetParentTransform(this.transform);
            filledElementAnimator.SetScale(new Vector3(64, 64, 1));
            filledElementAnimator.SetColor(m_color);
        }
        else //we just switched from WAITING to CURRENT, so just replace the material
        {
            UVQuad filledElement = m_filledObject.GetComponent<UVQuad>();
            filledElement.SetMaterial(m_filledMaterial);
        }
    }

    private void SetAsWaitingElement()
    {
        //build the empty element
        m_emptyObject = (GameObject)Instantiate(m_texQuadPfb);
        m_emptyObject.name = "Empty";

        UVQuad emptyElement = m_emptyObject.GetComponent<UVQuad>();
        emptyElement.Init(m_emptyMaterial);

        TexturedQuadAnimator emptyElementAnimator = m_emptyObject.GetComponent<TexturedQuadAnimator>();
        emptyElementAnimator.SetParentTransform(this.transform);
        emptyElementAnimator.SetScale(new Vector3(64, 64, 1));
        emptyElementAnimator.SetColor(m_color);
    }

    /**
     * Set the status (CURRENT, DONE or WAITING) and rebuild it if necessary
     * **/
    public void SetStatus(CounterElementStatus status)
    {
        if (status == CounterElementStatus.CURRENT)
        {
            SetAsCurrentElement();
        }
        else if (status == CounterElementStatus.DONE)
        {
            SetAsDoneElement();
        }
        else if (status == CounterElementStatus.WAITING)
        {
            SetAsWaitingElement();
        }

        m_status = status;
    }
}