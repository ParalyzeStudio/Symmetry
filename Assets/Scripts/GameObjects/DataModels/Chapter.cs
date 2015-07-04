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

    /**
     * Get the colors that will be used to draw elements for this specific chapters
     * -[0-1]: inner and outer colors for bg radial gradient
     * -2: color of central item background
     * -3: color of the progress bar background
     * **/
    public Color[] GetThemeColors()
    {
        Color[] themeColors = new Color[4];

        if (m_number == 1)
        {
            themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(146, 21, 51, 255));
            themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
            themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(255, 47, 125, 255));
            themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
        }
        else if (m_number == 2)
        {
            themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(174, 26, 24, 255));
            themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(61, 9, 10, 255));
            themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
            themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
        }
        else if (m_number == 3)
        {
            themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(176, 109, 29, 255));
            themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(61, 34, 9, 255));
            themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
            themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
        }
        else if (m_number == 4)
        {
            themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(155, 175, 29, 255));
            themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(53, 62, 10, 255));
            themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
            themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
        }

        return themeColors;
    }

    /**
     * Is this chapter locked
     * **/
    public bool IsLocked()
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        if (m_number > 1)
        {
            Chapter previousChapter = levelManager.GetChapterForNumber(this.m_number - 1);

            //chapter is unlocked if at least 75% of previous chapter is done
            int iDoneLevelsCount = 0;
            for (int iLevelIdx = 0; iLevelIdx != previousChapter.m_levels.Length; iLevelIdx++)
            {
                if (previousChapter.m_levels[iLevelIdx].IsDone())
                    iDoneLevelsCount++;
            }

           return (iDoneLevelsCount / (float)Chapter.LEVELS_COUNT) >= 0.75f;
        }

        return false;
    }
}
