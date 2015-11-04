using UnityEngine;

public class Chapter
{
    public const int LEVELS_COUNT = 16;

    public Level[] m_levels { get; set; }
    public int m_number;

    private Color[] m_themeColors;
    private Vector3[] m_themeTintValues;

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
        if (m_themeColors == null)
        {
            m_themeColors = new Color[4];

            if (m_number == 1)
            {
                m_themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(159, 162, 24, 255));
                m_themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(60, 61, 9, 255));
                m_themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(212, 185, 0, 255));
                m_themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));

                //m_themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(146, 21, 51, 255));
                //m_themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
                //m_themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(255, 47, 125, 255));
                //m_themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
            }
            else if (m_number == 2)
            {
                m_themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(174, 26, 24, 255));
                m_themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(61, 9, 10, 255));
                m_themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
                m_themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
            }
            else if (m_number == 3)
            {
                m_themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(176, 109, 29, 255));
                m_themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(61, 34, 9, 255));
                m_themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
                m_themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
            }
            else if (m_number == 4)
            {
                m_themeColors[0] = ColorUtils.GetColorFromRGBAVector4(new Vector4(155, 175, 29, 255));
                m_themeColors[1] = ColorUtils.GetColorFromRGBAVector4(new Vector4(53, 62, 10, 255));
                m_themeColors[2] = ColorUtils.GetColorFromRGBAVector4(new Vector4(64, 12, 26, 255));
                m_themeColors[3] = ColorUtils.GetColorFromRGBAVector4(new Vector4(59, 15, 29, 255));
            }
        }

        return m_themeColors;
    }

    /**
     * -0: tint color of the action buttons
     * -1: color of the interface buttons contour
     * -2: tint color of the interface buttons
     * **/
    public Vector3[] GetThemeTintValues()
    {
        m_themeTintValues = new Vector3[3];

        if (m_number == 1)
        {
            m_themeTintValues[0] = new Vector3(280.0f, 1.0f, 1.75f);
            m_themeTintValues[1] = new Vector3(5, 0.8f, 2.75f);
            m_themeTintValues[2] = new Vector3(0, 0.75f, 4.5f);
        }
        else if (m_number == 2)
        {
            m_themeTintValues[0] = new Vector3(280.0f, 1.0f, 1.75f);
            m_themeTintValues[1] = new Vector3(5, 0.8f, 2.75f);
            m_themeTintValues[2] = new Vector3(0, 0.75f, 4.5f);
        }
        else if (m_number == 3)
        {
            m_themeTintValues[0] = new Vector3(280.0f, 1.0f, 1.75f);
            m_themeTintValues[1] = new Vector3(5, 0.8f, 2.75f);
            m_themeTintValues[2] = new Vector3(0, 0.75f, 4.5f);
        }
        else if (m_number == 4)
        {
            m_themeTintValues[0] = new Vector3(280.0f, 1.0f, 1.75f);
            m_themeTintValues[1] = new Vector3(5, 0.8f, 2.75f);
            m_themeTintValues[2] = new Vector3(0, 0.75f, 4.5f);
        }

        return m_themeTintValues;
    }

    /**
     * Is this chapter locked
     * **/
    public bool IsLocked()
    {
        LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        if (m_number > 1)
        {
            Chapter previousChapter = levelManager.GetChapterForNumber(this.m_number - 1);

            //chapter is unlocked if at least 75% of previous chapter is done
            int iDoneLevelsCount = 0;
            for (int iLevelIdx = 0; iLevelIdx != previousChapter.m_levels.Length; iLevelIdx++)
            {
                if (previousChapter.m_levels[iLevelIdx] == null)//TODO remove this condition when all chapters are built with 15 levels in each
                    break;
                if (previousChapter.m_levels[iLevelIdx].IsDone())
                    iDoneLevelsCount++;
            }

           return (iDoneLevelsCount / (float)Chapter.LEVELS_COUNT) >= 0.75f;
        }

        return false;
    }
}
