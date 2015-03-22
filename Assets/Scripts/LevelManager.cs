using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public const int CHAPTERS_PER_GROUP = 4;
    public Color CHAPTER_GROUP_1_BASE_COLOR { get { return new Color(1, 0, 0, 1); } }
    public Color CHAPTER_GROUP_2_BASE_COLOR { get { return new Color(0, 0, 1, 1); } }
    public Color CHAPTER_GROUP_3_BASE_COLOR { get { return new Color(0, 1, 0, 1); } }

    public const int CHAPTERS_COUNT = 8;

    public Chapter[] m_chapters { get; set; }

    public Level m_currentLevel { get; set; }
    public Chapter m_currentChapter { get; set; }

    public void Awake()
    {
        m_chapters = new Chapter[CHAPTERS_COUNT];
        m_currentLevel = null;
    }

    public void ParseAllLevels()
    {
        for (int iChapterNumber = 1; iChapterNumber != CHAPTERS_COUNT; iChapterNumber++)
        {
            Chapter builtChapter = BuildChapterFromXml(iChapterNumber);
            m_chapters[iChapterNumber - 1] = builtChapter;
        }
    }

    public Chapter BuildChapterFromXml(int iChapterNumber)
    {
        Chapter chapter = new Chapter(iChapterNumber);

        string chapterDirectoryPath = "Chapter" + iChapterNumber;
        for (int iLevelIdx = 0; iLevelIdx != chapter.m_levels.Length; iLevelIdx++)
        {
            Level parsedLevel = ParseLevelFile(iChapterNumber, iLevelIdx + 1);
            if (parsedLevel != null)
                chapter.m_levels[iLevelIdx] = parsedLevel;
        }

        return chapter;
    }

    public Level ParseLevelFile(int iChapterNumber, int iLevelNumber)
    {
        string levelFilename = "Chapter" + iChapterNumber + "/level_" + iLevelNumber;
        Object levelObjectFile = Resources.Load(levelFilename);
        if (levelObjectFile == null)
            return null;

        TextAsset levelFile = (TextAsset)levelObjectFile;

        XMLParser xmlParser = new XMLParser();
        XMLNode rootNode = xmlParser.Parse(levelFile.text);

        XMLNode levelNode = rootNode.GetNode("level>0");

        Level level = new Level();
        string levelName = levelNode.GetValue("@name");
        string levelNumber = levelNode.GetValue("@number");
        level.m_number = int.Parse(levelNumber);
        level.m_name = levelName;

        //Parse grid information
        XMLNode gridNode = levelNode.GetNode("grid>0");

        string strExactNumLines = gridNode.GetValue("@exactNumLines");
        string strExactNumColumns = gridNode.GetValue("@exactNumColumns");
        level.m_gridExactNumLines = (strExactNumLines == null) ? 0 : int.Parse(strExactNumLines);
        level.m_gridExactNumColumns = (strExactNumColumns == null) ? 0 : int.Parse(strExactNumColumns);

        if (level.m_gridExactNumLines == 0)
        {
            string strMinNumLines = gridNode.GetValue("@minNumLines");
            level.m_gridMinNumLines = (strMinNumLines == null) ? 0 : int.Parse(strMinNumLines);
        }

        if (level.m_gridExactNumColumns == 0)
        {
            string strMinNumColumns = gridNode.GetValue("@minNumColumns");
            level.m_gridMinNumColumns = (strMinNumColumns == null) ? 0 : int.Parse(strMinNumColumns);
        }

        //Parse contours
        XMLNodeList contoursNodeList = levelNode.GetNodeList("contours>0>contour");
        level.m_contours.Capacity = contoursNodeList.Count;
        foreach (XMLNode contourNode in contoursNodeList)
        {
            Contour contour = new Contour();
            string strContourArea = contourNode.GetValue("@area");
            contour.m_area = float.Parse(strContourArea);
            XMLNodeList contourPointsNodeList = contourNode.GetNodeList("point");
            foreach (XMLNode contourPointNode in contourPointsNodeList)
            {
                string strContourPointLine = contourPointNode.GetValue("@line");
                string strContourPointColumn = contourPointNode.GetValue("@column");

                int contourPointLine = int.Parse(strContourPointLine);
                int contourPointColumn = int.Parse(strContourPointColumn);

                contour.m_contour.Add(new Vector2(contourPointColumn, contourPointLine));
            }
            level.m_contours.Add(contour);
        }

        //Parse shapes
        XMLNodeList shapesNodeList = levelNode.GetNodeList("shapes>0>shape");
        level.m_initialShapes.Capacity = shapesNodeList.Count;
        foreach (XMLNode shapeNode in shapesNodeList)
        {
            Shape shape = new Shape();
            //Get the color of the shape
            string strShapeColor = shapeNode.GetValue("@color");
            string[] strSplitColor = strShapeColor.Split(new char[] { ',' });
            Color shapeColor = new Color();
            shapeColor.r = float.Parse(strSplitColor[0]);
            shapeColor.g = float.Parse(strSplitColor[1]);
            shapeColor.b = float.Parse(strSplitColor[2]);
            shapeColor.a = float.Parse(strSplitColor[3]);
            shape.m_color = shapeColor;

            XMLNodeList contourPointsNodeList = shapeNode.GetNodeList("contour>0>point");
            foreach (XMLNode contourPointNode in contourPointsNodeList)
            {
                string strContourPointLine = contourPointNode.GetValue("@line");
                string strContourPointColumn = contourPointNode.GetValue("@column");

                int contourPointLine = int.Parse(strContourPointLine);
                int contourPointColumn = int.Parse(strContourPointColumn);

                shape.m_contour.Add(new Vector2(contourPointColumn, contourPointLine));
            }

            level.m_initialShapes.Add(shape);
        }

        //Parse action buttons tags
        XMLNodeList actionsNodeList = levelNode.GetNodeList("actions>0>action");
        level.m_actionButtonsTags.Capacity = actionsNodeList.Count;
        foreach (XMLNode actionNode in actionsNodeList)
        {
            string actionTag = actionNode.GetValue("@tag");
            level.m_actionButtonsTags.Add(actionTag);
        }

        return level;
    }

    public void SetCurrentLevelByNumber(int iLevelNumber)
    {
        if (iLevelNumber < m_currentChapter.m_levels.Length + 1)
        {
            m_currentLevel = m_currentChapter.m_levels[iLevelNumber - 1];
        }
    }

    public void SetCurrentChapterByNumber(int iChapterNumber)
    {
        m_currentChapter = m_chapters[iChapterNumber - 1];
    }

    public int GetChapterGroupForChapter(Chapter chapter)
    {
        return (chapter.m_number - 1) / 4 + 1;
    }

    public int GetCurrentChapterGroup()
    {
        return GetChapterGroupForChapter(m_currentChapter);
    }

    /**
     * One chapter group containing 4 chapters is associated with a color
     * **/
    public Color GetChapterGroupBaseColor(int iChapterGroup)
    {
        Color baseColor;
        if (iChapterGroup == 1)
            baseColor = CHAPTER_GROUP_1_BASE_COLOR;
        else if (iChapterGroup == 2)
            baseColor = CHAPTER_GROUP_2_BASE_COLOR;
        else
            baseColor = CHAPTER_GROUP_3_BASE_COLOR;

        return baseColor;
    }

    public Color GetCurrentChapterGroupBaseColor()
    {
        return GetChapterGroupBaseColor(m_currentChapter.m_number);
    }
}
