using UnityEngine;

public class ChapterSlotProgressBar : MonoBehaviour
{
    private ChapterSlot m_parentSlot;

    //size of the progress bar
    private Vector2 m_size;

    //elements
    public GameObject m_fill;
    public GameObject m_background;
    public GameObject m_completionText;

    private ColorQuadAnimator m_backgroundQuadAnimator;
    private ColorQuadAnimator m_fillQuadAnimator;
    private TextMeshAnimator m_completionTextAnimator;
    private TextMesh m_completionTextMesh;

    public Material m_transpColorMaterial;    

    public void Init(ChapterSlot parentSlot, Vector2 size, float fontHeight)
    {
        m_parentSlot = parentSlot;

        Chapter parentChapter = parentSlot.m_parentChapter;
        m_size = size;
        float completionPercentage = parentChapter.GetCompletionPercentage();

        //Background
        ColorQuad backgroundQuad = m_background.GetComponent<ColorQuad>();
        backgroundQuad.Init(Instantiate(m_transpColorMaterial));

        m_backgroundQuadAnimator = m_background.GetComponent<ColorQuadAnimator>();
        m_backgroundQuadAnimator.SetScale(GeometryUtils.BuildVector3FromVector2(size, 1));
        m_backgroundQuadAnimator.SetColor(Color.black);

        //Fill
        ColorQuad fillQuad = m_fill.GetComponent<ColorQuad>();
        fillQuad.Init(Instantiate(m_transpColorMaterial));

        m_fillQuadAnimator = m_fill.GetComponent<ColorQuadAnimator>();
        m_fillQuadAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
        m_fillQuadAnimator.SetPosition(new Vector3(-0.5f * size.x, 0, -1));
        //m_fillQuadAnimator.SetScale(new Vector3(completionPercentage * size.x, size.y, 1));
        m_fillQuadAnimator.SetColor(Color.white);

        //Completion text
        m_completionTextMesh = m_completionText.GetComponent<TextMesh>();
        //int iCompletionPercentage = Mathf.RoundToInt(completionPercentage * 100);
        //m_completionTextMesh.text = iCompletionPercentage + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");

        m_completionTextAnimator = m_completionText.GetComponent<TextMeshAnimator>();
        m_completionTextAnimator.SetFontHeight(fontHeight);
        m_completionTextAnimator.UpdatePivotPoint(new Vector3(1.0f, 1.0f, 0.5f));
        //m_completionTextAnimator.SetPosition(new Vector3(0.5f * size.x, -0.5f * size.y - 8.0f, 0));
        m_completionTextAnimator.SetColor(Color.white);
    }

    /**
     * Update the progress bar data for the new given chapter
     * **/
    public void RefreshData()
    {
        float completionPercentage = m_parentSlot.m_parentChapter.GetCompletionPercentage();

        //update fill
        m_fillQuadAnimator.SetScale(new Vector3(completionPercentage * m_size.x, m_size.y, 1));

        //update text
        int iCompletionPercentage = Mathf.RoundToInt(completionPercentage * 100);
        m_completionTextMesh.text = iCompletionPercentage + "% " + LanguageUtils.GetTranslationForTag("progress_bar_completion");

        //reposition the text
        m_completionTextAnimator.SetPosition(new Vector3(0.5f * m_size.x, -0.5f * m_size.y - 8.0f, 0));
    }
}
