using UnityEngine;

public class CounterElement : MonoBehaviour
{
    public const float SHADOW_Z_VALUE = 1.0f;
    public const float SKIN_Z_VALUE = 0.0f;
    public const float OVERLAY_Z_VALUE = -1.0f;

    public GameObject m_baseQuadElementPfb;

    private GameObject m_overlay; //the overlay that is displayed on the current action
    private GameObject m_skin; //the skin of this element
    private GameObject m_shadow; //the shadow behind the skin

    //colors
    private Color m_shadowColor;
    private Color m_skinColor;
    private Color m_overlayColor;

    //materials used for each of the above game objects MeshRenderers
    private Material m_shadowMaterial;
    private Material m_skinOnMaterial;
    private Material m_skinOffMaterial;
    private Material m_overlayMaterial;

    public enum CounterElementStatus
    {
        DONE, //an action has been realised and this element is marked as done
        CURRENT, //display an overlay for the CURRENT element
        WAITING //this element is waiting for previous ones to complete
    };

    public CounterElementStatus m_status { get; set; }
    private CounterElementStatus m_prevStatus;

    public void Init(Material shadowMaterial, Material skinOnMaterial, Material skinOffMaterial, Material overlayMaterial)
    {
        //Set cloned materials shared among counter elements to global variables
        m_shadowMaterial = shadowMaterial;
        m_skinOnMaterial = skinOnMaterial;
        m_skinOffMaterial = skinOffMaterial;
        m_overlayMaterial = overlayMaterial;

        m_status = CounterElementStatus.WAITING;
        m_prevStatus = m_status;
        InitElementsColors();
        InitSkinAndShadow();
    }

    public void InitElementsColors()
    {
        m_shadowColor = new Color(0,0,0,1);
        m_skinColor = new Color(1,1,1,1);
        m_overlayColor = new Color(0,0,0,1);
    }

    private void InitSkinAndShadow()
    {
        m_skin = Instantiate(m_baseQuadElementPfb);
        m_skin.transform.parent = this.gameObject.transform;
        ColorQuadAnimator skinAnimator = m_skin.GetComponent<ColorQuadAnimator>();
        skinAnimator.SetRotationAxis(Vector3.forward);
        skinAnimator.SetRotationAngle(45);
        skinAnimator.SetScale(new Vector3(40.0f, 40.0f, 1.0f));
        skinAnimator.SetPosition(new Vector3(0, 6, SKIN_Z_VALUE));
        skinAnimator.SetColor(m_skinColor);
        SetStatus(CounterElementStatus.WAITING);

        m_shadow = Instantiate(m_baseQuadElementPfb);
        m_shadow.transform.parent = this.gameObject.transform;
        ColorQuadAnimator shadowAnimator = m_shadow.GetComponent<ColorQuadAnimator>();
        shadowAnimator.SetRotationAxis(Vector3.forward);
        shadowAnimator.SetRotationAngle(45);
        shadowAnimator.SetScale(new Vector3(40.0f, 40.0f, 1.0f));
        shadowAnimator.SetPosition(new Vector3(0, -6, SHADOW_Z_VALUE));
        shadowAnimator.SetColor(m_shadowColor);
        m_shadow.GetComponent<MeshRenderer>().sharedMaterial = m_shadowMaterial;
    }

    public void SetStatus(CounterElementStatus status)
    {
        m_status = status;

        //check previous status and remove overlay if CounterElementStatus.CURRENT
        if (m_prevStatus == CounterElementStatus.CURRENT)
        {
            Destroy(m_overlay);
        }

        //Set correct materials depending on the status
        if (status == CounterElementStatus.CURRENT)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = m_skinOnMaterial;

            m_overlay = Instantiate(m_baseQuadElementPfb);
            m_overlay.transform.parent = this.transform;
            m_overlay.GetComponent<MeshRenderer>().sharedMaterial = m_overlayMaterial;
            TexturedQuadAnimator overlayAnimator = m_overlay.GetComponent<TexturedQuadAnimator>();
            overlayAnimator.SetRotationAxis(Vector3.forward);
            overlayAnimator.SetRotationAngle(45);
            overlayAnimator.SetScale(new Vector3(18.0f, 18.0f, 1.0f));
            overlayAnimator.SetPosition(new Vector3(0, 6, OVERLAY_Z_VALUE));
            overlayAnimator.SetColor(m_overlayColor);
        }
        else if (status == CounterElementStatus.DONE)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = m_skinOnMaterial;
            if (m_overlay != null)
            {
                Destroy(m_overlay);
                m_overlay = null;
            }
        }
        else if (status == CounterElementStatus.WAITING)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = m_skinOffMaterial;
        }

        m_prevStatus = m_status;
    }
}
