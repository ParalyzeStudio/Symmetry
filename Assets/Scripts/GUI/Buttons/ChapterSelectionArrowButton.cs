using UnityEngine;

/**
 * Class that inherits from GUIButton and that represents the chapter selection arrow
 * **/
public class ChapterSelectionArrowButton : GUIButton
{
    public GameObject m_arrowObject; //the small arrow that indicates the direction left or right
    private UVQuad m_arrow;
    private TexturedQuadAnimator m_arrowAnimator;

    //variables to handle arrow translation animation
    private bool m_arrowTranslating;
    private Vector3 m_arrowTranslationPointA;
    private Vector3 m_arrowTranslationPointB;
    private float m_arrowTranslationLength;
    private bool m_arrowTranslationDirection; //left = 0 or right = 1
    private float m_arrowTranslationDuration;
    private float m_arrowTranslationElapsedTime;


    public override void Init(Material skinMaterial = null)
    {
        base.Init(skinMaterial);

        m_arrow = m_arrowObject.GetComponent<UVQuad>();
        m_arrow.Init(null); //material is already specified inside prefab, just build the mesh here

        m_arrowAnimator = m_arrow.GetComponent<TexturedQuadAnimator>();
        m_arrowAnimator.SetColor(Color.white); //default white tint color
        m_arrowAnimator.SetScale(new Vector3(128, 256, 1));

        TexturedQuadAnimator skinAnimator = m_skinObject.GetComponent<TexturedQuadAnimator>();
        m_size = new Vector2(192, 192);
        skinAnimator.SetScale(GeometryUtils.BuildVector3FromVector2(m_size, 1));
        Vector3 skinPosition = skinAnimator.GetPosition();
        float spaceBetweenSkinAndArrow = -40.0f;

        m_arrowTranslating = false;
        m_arrowTranslationLength = 30.0f;
        m_arrowTranslationPointB = skinPosition - new Vector3(0.5f * m_size.x + spaceBetweenSkinAndArrow, 0, 0);
        m_arrowTranslationPointA = m_arrowTranslationPointB - new Vector3(m_arrowTranslationLength, 0, 0);
        m_arrowTranslationDirection = false;

        m_arrowAnimator.SetPosition(m_arrowTranslationPointB);

        StartArrowTranslationAnimation();
    }

    public void StartArrowTranslationAnimation()
    {
        m_arrowTranslating = true;
        m_arrowTranslationDuration = 1.0f;
        m_arrowTranslationElapsedTime = 0;
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        if (m_arrowTranslating)
        {
            m_arrowTranslationElapsedTime += dt;
            if (m_arrowTranslationElapsedTime >= m_arrowTranslationDuration)
            {
                if (!m_arrowTranslationDirection)
                {
                    m_arrowAnimator.SetPosition(m_arrowTranslationPointA);
                    m_arrowTranslationDirection = true;
                }
                else
                {
                    m_arrowAnimator.SetPosition(m_arrowTranslationPointB);
                    m_arrowTranslationDirection = false;
                }

                m_arrowTranslationElapsedTime = 0;
            }
            else
            {
                float dx = (!m_arrowTranslationDirection ? -1 : 1) * dt / m_arrowTranslationDuration * m_arrowTranslationLength;
                m_arrowAnimator.IncPosition(new Vector3(dx, 0, 0));
            }
        }
    }
}
