using UnityEngine;

public class Chapters : MonoBehaviour
{
    public const int CHAPTERS_PER_GROUP = 4;

    public GameObject m_chapterSlotPfb;

    /**
     * Shows Chapters screen with or without animation
     * **/
    public void Show(bool bAnimated)
    {
        int reachedChapterNumber = 1;
        int chapterGroup = ((reachedChapterNumber - 1) / CHAPTERS_PER_GROUP) + 1;
        ShowChapterSlots(chapterGroup, bAnimated);
    }

    public void ShowChapterSlots(int chapterGroup, bool bAnimated)
    {
        GameObject chaptersHolder = GameObject.FindGameObjectWithTag("ChaptersHolder");

        for (int iChapterSlotIndex = 0; iChapterSlotIndex != CHAPTERS_PER_GROUP; iChapterSlotIndex++)
        {
            GameObject clonedChapterSlot = (GameObject) Instantiate(m_chapterSlotPfb);

            //Find the position of each slot
            BoundingBoxCalculator bboxCalculator = clonedChapterSlot.GetComponent<BoundingBoxCalculator>();
            bboxCalculator.InvalidateBounds();
            Vector3 slotSize = bboxCalculator.m_bbox.size;
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
            slotProperties.m_number = (chapterGroup - 1) * CHAPTERS_PER_GROUP + iChapterSlotIndex + 1;


        }
    }
}

