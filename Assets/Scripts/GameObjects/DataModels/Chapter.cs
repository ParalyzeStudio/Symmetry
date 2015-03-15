using UnityEngine;

public class Chapter
{
    public const int LEVELS_COUNT = 16;

    public Level[] m_levels { get; set; }
    public int m_number;

    public Chapter()
    {
        m_levels = new Level[LEVELS_COUNT];
        m_number = 0;
    }

    public Chapter(int iChapterNumber) : this()
    {
        m_number = iChapterNumber;
    }
}
