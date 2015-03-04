using UnityEngine;

public class Chapters : MonoBehaviour
{
    public const int CHAPTERS_PER_GROUP = 4;
    public Color CHAPTER_GROUP_1_BASE_COLOR { get { return new Color(1, 0, 0, 1); } }
    public Color CHAPTER_GROUP_2_BASE_COLOR { get { return new Color(0, 0, 1, 1); } }
    public Color CHAPTER_GROUP_3_BASE_COLOR { get { return new Color(0, 1, 0, 1); } }

    public GameObject m_chapterSlotPfb;
    private GameObject[] m_chapterSlots;

    private int m_chapterGroup;

    private float m_showTitleDelay;
    private float m_showTitleElpasedTime;
    private bool m_showTitleCallRunning;
    private float m_showChaptersDelay;
    private float m_showChaptersElpasedTime;
    private bool m_showChaptersCallRunning;

    /**
     * Shows Chapters screen with or without animation
     * **/
    public void Show(bool bAnimated)
    {
        int reachedChapterNumber = 1;
        m_chapterGroup = ((reachedChapterNumber - 1) / CHAPTERS_PER_GROUP) + 1;
        m_showTitleCallRunning = false;
        m_showChaptersCallRunning = false;
        ShowTitle(bAnimated, 5.0f);
        ShowChapterSlots(bAnimated, 1.0f);
    }

    public void Dismiss()
    {
        this.gameObject.transform.localPosition = new Vector3(0, 0, 100);
    }

    public void ShowTitle(bool bAnimated, float fDelay = 0.0f)
    {
        if (bAnimated)
        {
            if (!m_showTitleCallRunning)
            {
                m_showTitleCallRunning = true;
                m_showTitleDelay = fDelay;
                return;
            }
            else
            {
                m_showTitleCallRunning = false;
            }
        }
        else
        {
            
        }
    }

    public void ShowChapterSlots(bool bAnimated, float fDelay = 0.0f)
    {
        GameObject chaptersHolder = GameObject.FindGameObjectWithTag("ChaptersHolder");

        m_chapterSlots = new GameObject[4];
        Vector3 slotSize = Vector3.zero;
        for (int iChapterSlotIndex = 0; iChapterSlotIndex != CHAPTERS_PER_GROUP; iChapterSlotIndex++)
        {
            GameObject clonedChapterSlot = (GameObject) Instantiate(m_chapterSlotPfb);
            m_chapterSlots[iChapterSlotIndex] = clonedChapterSlot;

            //Find the position of each slot
            BoundingBoxCalculator bboxCalculator = clonedChapterSlot.GetComponent<BoundingBoxCalculator>();
            bboxCalculator.InvalidateBounds();
            if (slotSize == Vector3.zero)
                slotSize = bboxCalculator.m_bbox.size;
            Vector3 slotLocalPosition;
            if (iChapterSlotIndex == 0)
                slotLocalPosition = new Vector3(-0.5f * slotSize.x, 0.5f * slotSize.y, 0);
            else if (iChapterSlotIndex == 1)
                slotLocalPosition = new Vector3(0.5f * slotSize.x, 0.5f * slotSize.y, 0);
            else if (iChapterSlotIndex == 2)
                slotLocalPosition = new Vector3(-0.5f * slotSize.x, -0.5f * slotSize.y, 0);
            else
                slotLocalPosition = new Vector3(0.5f * slotSize.x, -0.5f * slotSize.y, 0);

            clonedChapterSlot.transform.parent = chaptersHolder.transform;
            clonedChapterSlot.transform.localPosition = slotLocalPosition;

            //Set the correct number on child text mesh and correct color on skin
            ChapterSlot slotProperties = clonedChapterSlot.GetComponent<ChapterSlot>();
            slotProperties.m_number = (m_chapterGroup - 1) * CHAPTERS_PER_GROUP + iChapterSlotIndex + 1;

            //Set the correct color
            Color baseColor;
            if (m_chapterGroup == 1)
                baseColor = CHAPTER_GROUP_1_BASE_COLOR;
            else if (m_chapterGroup == 2)
                baseColor = CHAPTER_GROUP_2_BASE_COLOR;
            else
                baseColor = CHAPTER_GROUP_3_BASE_COLOR;

            slotProperties.m_color = ColorUtils.DarkenColor(baseColor, 0.18f * iChapterSlotIndex);   
        }

        if (bAnimated)
        {
            m_showChaptersCallRunning = true;
            m_showChaptersDelay = fDelay;

            //TintColorMaterialAssignment slotSkinTintColor = chapterSlots[0].GetComponentInChildren<TintColorMaterialAssignment>();
            //TextMeshAnimator numberTextAnimator = chapterSlots[0].GetComponentInChildren<TextMeshAnimator>();
            //slotSkinTintColor.m_tintColor = 

            GameObjectAnimator slotAnimatorTest = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
            slotAnimatorTest.OnOpacityChanged(0);

            GameObjectAnimator slotAnimator = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(0, 0.5f, 0.5f));
            slotAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0, 0.5f * slotSize.y, 0));
            slotAnimator.OnRotationChanged(180, new Vector3(0, 1, 0));

            slotAnimator = m_chapterSlots[2].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.5f));
            slotAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0, -0.5f * slotSize.y, 0));
            slotAnimator.OnRotationChanged(180, new Vector3(0, -1, 0));

            slotAnimator = m_chapterSlots[3].GetComponent<GameObjectAnimator>();
            slotAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
            slotAnimator.MoveObjectBySettingPivotPointPosition(new Vector3(0.5f * slotSize.y, 0, 0));
            slotAnimator.OnRotationChanged(180, new Vector3(1, 0, 0));
        }        
    }

    public void ShowChapterSlotsWithAnimation()
    {
        m_showChaptersCallRunning = false;

        //Animate slot 2
        GameObjectAnimator slotAnimator = m_chapterSlots[1].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateFromToAroundAxis(180, 0, new Vector3(0, 1, 0), 0.3f);

        TextMeshAnimator numberTextAnimator = m_chapterSlots[1].GetComponentInChildren<TextMeshAnimator>();
        numberTextAnimator.OnOpacityChanged(0);
        numberTextAnimator.FadeFromTo(0, 1, 0.5f, 0.3f);

        //Animate slot 3
        slotAnimator = m_chapterSlots[2].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateFromToAroundAxis(180, 0, new Vector3(0, -1, 0), 0.3f, 0.4f);

        numberTextAnimator = m_chapterSlots[2].GetComponentInChildren<TextMeshAnimator>();
        numberTextAnimator.OnOpacityChanged(0);
        numberTextAnimator.FadeFromTo(0, 1, 0.5f, 0.4f);

        //Animate slot 4
        slotAnimator = m_chapterSlots[3].GetComponent<GameObjectAnimator>();
        slotAnimator.RotateFromToAroundAxis(180, 0, new Vector3(1, 0, 0), 0.3f, 0.2f);

        numberTextAnimator = m_chapterSlots[3].GetComponentInChildren<TextMeshAnimator>();
        numberTextAnimator.OnOpacityChanged(0);
        numberTextAnimator.FadeFromTo(0, 1, 0.5f, 0.2f);
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        if (m_showTitleCallRunning)
        {
            m_showTitleElpasedTime += dt;
            if (m_showTitleElpasedTime >= m_showTitleDelay)
            {
                ShowTitle(true);
            }
        }

        if (m_showChaptersCallRunning)
        {
            m_showChaptersElpasedTime += dt;
            if (m_showChaptersElpasedTime >= m_showChaptersDelay)
            {
                ShowChapterSlotsWithAnimation();
            }
        }
    }
}

