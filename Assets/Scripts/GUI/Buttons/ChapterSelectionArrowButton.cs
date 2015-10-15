using UnityEngine;

/**
 * Class that inherits from GUIButton and that represents the chapter selection arrow
 * **/
public class ChapterSelectionArrowButton : GUIButton
{
    public GameObject m_arrowObject; //the small arrow that indicates the direction left or right
    private UVQuad m_arrow;
    private TexturedQuadAnimator m_arrowAnimator;
    private bool m_activeState; //the state of the button (active/inactive)
    public Material m_chapterSelectionArrowTriangleMaterial;
    public Material m_chapterSelectionArrowHexagonMaterial;

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
        base.Init(Instantiate(m_chapterSelectionArrowHexagonMaterial));

        m_arrow = m_arrowObject.GetComponent<UVQuad>();
        m_arrow.Init(Instantiate(m_chapterSelectionArrowTriangleMaterial)); //material is already specified inside prefab, just build the mesh here

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

        SetState(false);
    }

    public void StartArrowTranslationAnimation()
    {
        m_arrowTranslating = true;
        m_arrowTranslationDuration = 1.0f;
        m_arrowTranslationElapsedTime = 0;
    }

    /**
     * Switch between active/inactive mode by setting a new state to the button
     * true = active
     * false = inactive
     * **/
    public void SetState(bool bNewState)
    {
        if (m_activeState == bNewState)
            return;

        m_activeState = bNewState;

        GameObjectAnimator arrowAnimator = this.GetComponent<GameObjectAnimator>();

        if (bNewState)
        {
            StartArrowTranslationAnimation();
            arrowAnimator.SetOpacity(0);
            arrowAnimator.FadeTo(1.0f, 0.5f);
        }
        else
        {
            m_arrowTranslating = false;
            arrowAnimator.FadeTo(0.0f, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, false);
        }
    }

    public override void OnClick()
    {
        SceneManager sceneManager = GetSceneManager();
        Chapters chapters = (Chapters)sceneManager.m_currentScene;

        if (m_ID == GUIButtonID.ID_CHAPTER_SELECTION_ARROW_PREVIOUS && chapters.DecrementChapterIndex() ||
            m_ID == GUIButtonID.ID_CHAPTER_SELECTION_ARROW_NEXT && chapters.IncrementChapterIndex())
        {
            chapters.OnClickSelectionArrow();

            //dismiss old chapter slot information
            //chapters.DismissChapterSlot(false, false, false);
            //update background gradient and chapter slot backgorund color
            //chapters.UpdateBackgroundGradient();
            //chapters.UpdateChapterSlotBackgroundColor();
            ////update chapter slot information when opacity is 0
            //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(chapters.UpdateChapterSlotInformation), 0.5f);
            ////wait a bit and then display the new information
            //GetCallFuncHandler().AddCallFuncInstance(new CallFuncHandler.CallFunc(chapters.ShowChapterSlotInformation), 0.8f);
        }     
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
