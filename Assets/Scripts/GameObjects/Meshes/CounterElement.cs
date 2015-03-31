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

    //materials used for each of the above game objects MeshRenderers
    public Material m_overlayMaterial;
    public Material m_skinOnMaterial;
    public Material m_skinOffMaterial;
    public Material m_shadowMaterial;

    public enum CounterElementStatus
    {
        DONE, //an action has been realised and this element is marked as done
        CURRENT, //display an overlay for the CURRENT element
        WAITING //this element is waiting for previous ones to complete
    };

    public CounterElementStatus m_status { get; set; }
    private CounterElementStatus m_prevStatus;

    public void Init()
    {
        m_status = CounterElementStatus.WAITING;
        m_prevStatus = m_status;
        InitSkinAndShadow();
    }

    private void InitSkinAndShadow()
    {
        m_skin = Instantiate(m_baseQuadElementPfb);
        m_skin.transform.parent = this.gameObject.transform;
        TranspQuadOpacityAnimator skinAnimator = m_skin.GetComponent<TranspQuadOpacityAnimator>();
        skinAnimator.SetRotationAxis(Vector3.forward);
        skinAnimator.SetRotationAngle(45);
        skinAnimator.SetScale(new Vector3(40.0f, 40.0f, 1.0f));
        skinAnimator.SetPosition(new Vector3(0, 6, SKIN_Z_VALUE));
        SetStatus(CounterElementStatus.CURRENT);

        m_shadow = Instantiate(m_baseQuadElementPfb);
        m_shadow.transform.parent = this.gameObject.transform;
        TranspQuadOpacityAnimator shadowAnimator = m_shadow.GetComponent<TranspQuadOpacityAnimator>();
        shadowAnimator.SetRotationAxis(Vector3.forward);
        shadowAnimator.SetRotationAngle(45);
        shadowAnimator.SetScale(new Vector3(40.0f, 40.0f, 1.0f));
        shadowAnimator.SetPosition(new Vector3(0, -6, SHADOW_Z_VALUE));
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
            TranspQuadOpacityAnimator overlayAnimator = m_overlay.GetComponent<TranspQuadOpacityAnimator>();
            overlayAnimator.SetRotationAxis(Vector3.forward);
            overlayAnimator.SetRotationAngle(45);
            overlayAnimator.SetScale(new Vector3(18.0f, 18.0f, 1.0f));
            overlayAnimator.SetPosition(new Vector3(0, 6, OVERLAY_Z_VALUE));
        }
        else if (status == CounterElementStatus.DONE)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = m_skinOnMaterial;
        }
        else if (status == CounterElementStatus.WAITING)
        {
            m_skin.GetComponent<MeshRenderer>().sharedMaterial = m_skinOffMaterial;
        }

        m_prevStatus = m_status;
    }
}
