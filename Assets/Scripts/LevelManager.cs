using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public const int CHAPTERS_COUNT = 8;
    public const int LEVELS_PER_CHAPTER = 15;

    public Chapter[] m_chapters { get; set; }
    public Level m_debugLevel1 { get; set; }
    public Level m_debugLevel2 { get; set; }
    public Level m_debugLevel3 { get; set; }

    public Level m_currentLevel { get; set; }
    public Chapter m_currentChapter { get; set; }

    public void Awake()
    {
        m_chapters = new Chapter[CHAPTERS_COUNT];
        m_currentLevel = null;
    }

    public void ParseAllLevels()
    {
        for (int iChapterNumber = 1; iChapterNumber != CHAPTERS_COUNT + 1; iChapterNumber++)
        {
            Chapter builtChapter = BuildChapterFromXml(iChapterNumber);
            m_chapters[iChapterNumber - 1] = builtChapter;
        }

        //parse debug level
        m_debugLevel1 = ParseLevelFile(null, 1, true);
        m_debugLevel2 = ParseLevelFile(null, 2, true);
        m_debugLevel3 = ParseLevelFile(null, 3, true);
    }

    public Chapter BuildChapterFromXml(int iChapterNumber)
    {
        Chapter chapter = new Chapter(iChapterNumber);

        for (int iLevelIdx = 0; iLevelIdx != chapter.m_levels.Length; iLevelIdx++)
        {
            Level parsedLevel = ParseLevelFile(chapter, iLevelIdx + 1);
            if (parsedLevel != null)
                chapter.m_levels[iLevelIdx] = parsedLevel;
        }

        return chapter;
    }

    public Level ParseLevelFile(Chapter chapter, int iLevelNumber, bool bDebugLevel = false)
    {
        string levelFilename;
        if (bDebugLevel)
            levelFilename = "Levels/debug_level" + iLevelNumber;
        else
            levelFilename = "Levels/Chapter" + chapter.m_number + "/level_" + iLevelNumber;
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
        level.m_chapterRelativeNumber = int.Parse(levelNumber);
        level.m_parentChapter = chapter;
        level.m_name = levelName;

        //Parse counter information
        XMLNode counterNode = levelNode.GetNode("counter>0");
        string strMaxActions = counterNode.GetValue("@maxActions");
        level.m_maxActions = int.Parse(strMaxActions);

        //Parse stack information
        XMLNode stackNode = levelNode.GetNode("stack>0");
        if (stackNode != null)
        {
            level.m_symmetriesStackable = true;
            level.m_symmetryStackSize = int.Parse(stackNode.GetValue("@size"));
        }
        else
            level.m_symmetriesStackable = false;

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

        string strMaxGridSpacing = gridNode.GetValue("@maxGridSpacing");
        level.m_maxGridSpacing = (strMaxGridSpacing == null) ? 0 : float.Parse(strMaxGridSpacing);

        //Parse dotted outlines
        XMLNodeList dottedOutlinesNodeList = levelNode.GetNodeList("dottedOutlines>0>dottedOutline");
        if (dottedOutlinesNodeList != null)
        {
            level.m_outlines.Capacity = dottedOutlinesNodeList.Count;
            foreach (XMLNode outlineNode in dottedOutlinesNodeList)
            {
                DottedOutline outline = new DottedOutline();

                //Contour
                XMLNodeList contourPointsNodeList = outlineNode.GetNodeList("contour>0>point");
                foreach (XMLNode contourPointNode in contourPointsNodeList)
                {
                    string strContourPointLine = contourPointNode.GetValue("@line");
                    string strContourPointColumn = contourPointNode.GetValue("@column");

                    float contourPointLine = float.Parse(strContourPointLine);
                    float contourPointColumn = float.Parse(strContourPointColumn);

                    int scaleValue = GridPoint.DEFAULT_SCALE_PRECISION;
                    int scaledContourPointLine = (int)(contourPointLine * scaleValue);
                    int scaledContourPointColumn = (int)(contourPointColumn * scaleValue);

                    GridPoint outlineContourPoint = new GridPoint(scaledContourPointColumn, scaledContourPointLine, false);

                    outline.m_contour.Add(outlineContourPoint);
                }

                //Holes
                XMLNodeList holesNodeList = outlineNode.GetNodeList("holes>0>hole");
                if (holesNodeList != null)
                {
                    foreach (XMLNode holeNode in holesNodeList)
                    {
                        Contour holePoints = new Contour();
                        XMLNodeList holePointsNodeList = holeNode.GetNodeList("point");
                        holePoints.Capacity = holePointsNodeList.Count;

                        foreach (XMLNode holePointNode in holePointsNodeList)
                        {
                            string strHolePointLine = holePointNode.GetValue("@line");
                            string strHolePointColumn = holePointNode.GetValue("@column");

                            float holePointLine = float.Parse(strHolePointLine);
                            float holePointColumn = float.Parse(strHolePointColumn);

                            int scaleValue = GridPoint.DEFAULT_SCALE_PRECISION;
                            int scaledHolePointLine = (int)(holePointLine * scaleValue);
                            int scaledHolePointColumn = (int)(holePointColumn * scaleValue);

                            GridPoint outlineHolePoint = new GridPoint(scaledHolePointColumn, scaledHolePointLine, false);

                            holePoints.Add(outlineHolePoint);
                        }

                        outline.m_holes.Add(holePoints);
                    }
                }

                //Finally add the filled outline to the oulines list
                level.m_outlines.Add(outline);
            }
        }

        //Parse shapes
        XMLNodeList shapesNodeList = levelNode.GetNodeList("shapes>0>shape");
        if (shapesNodeList != null)
        {
            level.m_initialShapes.Capacity = shapesNodeList.Count;
            foreach (XMLNode shapeNode in shapesNodeList)
            {
                Shape shape = new Shape();
                //Get the color of the shape (either RBG or TSB)
                string strShapeColor = shapeNode.GetValue("@color");
                if (strShapeColor != null)
                {
                    string[] strSplitColor = strShapeColor.Split(new char[] { ',' });
                    Color shapeColor = new Color();
                    shapeColor.r = float.Parse(strSplitColor[0]);
                    shapeColor.g = float.Parse(strSplitColor[1]);
                    shapeColor.b = float.Parse(strSplitColor[2]);
                    shapeColor.a = 1;
                    shape.m_color = shapeColor;
                }

                string strTintShapeColor = shapeNode.GetValue("@tint");
                if (strTintShapeColor != null)
                {                    
                    shape.m_tint = float.Parse(strTintShapeColor);
                    shape.CalculateColorFromTSBValues();
                }

                //Contour
                XMLNodeList contourPointsNodeList = shapeNode.GetNodeList("contour>0>point");
                foreach (XMLNode contourPointNode in contourPointsNodeList)
                {
                    string strContourPointLine = contourPointNode.GetValue("@line");
                    string strContourPointColumn = contourPointNode.GetValue("@column");

                    float contourPointLine = float.Parse(strContourPointLine);
                    float contourPointColumn = float.Parse(strContourPointColumn);
                    
                    int scaleValue = GridPoint.DEFAULT_SCALE_PRECISION;
                    int scaledContourPointLine = (int)(contourPointLine * scaleValue);
                    int scaledContourPointColumn = (int)(contourPointColumn * scaleValue);

                    GridPoint shapeContourPoint = new GridPoint(scaledContourPointColumn, scaledContourPointLine, false);

                    shape.m_contour.Add(shapeContourPoint);
                }

                //Holes
                XMLNodeList holesNodeList = shapeNode.GetNodeList("holes>0>hole");
                if (holesNodeList != null)
                {
                    foreach (XMLNode holeNode in holesNodeList)
                    {
                        Contour holePoints = new Contour();
                        XMLNodeList holePointsNodeList = holeNode.GetNodeList("point");
                        holePoints.Capacity = holePointsNodeList.Count;

                        foreach (XMLNode holePointNode in holePointsNodeList)
                        {
                            string strHolePointLine = holePointNode.GetValue("@line");
                            string strHolePointColumn = holePointNode.GetValue("@column");

                            float holePointLine = float.Parse(strHolePointLine);
                            float holePointColumn = float.Parse(strHolePointColumn);

                            int scaleValue = GridPoint.DEFAULT_SCALE_PRECISION;
                            int scaledHolePointLine = (int)(holePointLine * scaleValue);
                            int scaledHolePointColumn = (int)(holePointColumn * scaleValue);

                            GridPoint shapeHolePoint = new GridPoint(scaledHolePointColumn, scaledHolePointLine, false);

                            holePoints.Add(shapeHolePoint);
                        }

                        shape.m_holes.Add(holePoints);
                    }
                }

                level.m_initialShapes.Add(shape);
            }
        }

        //Parse axis constraints
        XMLNodeList constraintsNodeList = levelNode.GetNodeList("constraints>0>constraint");
        if (constraintsNodeList != null)
        {
            level.m_axisConstraints.Capacity = constraintsNodeList.Count;
            foreach (XMLNode actionNode in constraintsNodeList)
            {
                string actionTag = actionNode.GetValue("@tag");
                level.m_axisConstraints.Add(actionTag);
            }
        }

        return level;
    }

    /**
     * Sets the m_currentLevel variable by providing its absolute number
     * **/
    public void SetCurrentLevelByAbsoluteNumber(int iLevelNumber)
    {
        if (iLevelNumber < m_currentChapter.m_levels.Length + 1)
        {
            m_currentLevel = m_currentChapter.m_levels[iLevelNumber - 1];
        }
    }

    /**
     * Sets the m_currentLevel variable by providing its chapter relative number
     * **/
    public void SetLevelOnCurrentChapter(int iLevelRelativeNumber)
    {
        if (iLevelRelativeNumber < m_currentChapter.m_levels.Length + 1)
        {
            m_currentLevel = m_currentChapter.m_levels[iLevelRelativeNumber - 1];
        }
    }

    /**
     * For debug purposes use the debug level instead of a regular level
     * **/
    public void SetCurrentLevelAsDebugLevel(int iDebugLevelNumber)
    {
        if (iDebugLevelNumber == 1)
            m_currentLevel = m_debugLevel1;
        else if (iDebugLevelNumber == 2)
            m_currentLevel = m_debugLevel2;
        else
            m_currentLevel = m_debugLevel3;
    }

    /**
     * For debug purposes test if current loaded level is the debug level
     * **/
    public int IsCurrentLevelDebugLevel()
    {
        if (m_currentLevel == m_debugLevel1)
            return 1;
        else if (m_currentLevel == m_debugLevel2)
            return 2;
        else if (m_currentLevel == m_debugLevel3)
            return 3;
        else
            return -1;
    }

    /**
     * Gets the m_currentLevel variable by asking for its chapter number and chapter relative level number
     * **/
    public Level GetLevelForChapterNumberAndLevelRelativeNumber(int iChapterNumber, int iLevelRelativeNumber)
    {
        return m_chapters[iChapterNumber - 1].m_levels[iLevelRelativeNumber - 1];
    }

    /**
     * Sets the m_currentChapter variable 
     * **/
    public void SetCurrentChapterByNumber(int iChapterNumber)
    {
        m_currentChapter = m_chapters[iChapterNumber - 1];
    }

    /**
     * Gets the chapter whose number is passed as parameter
     * **/
    public Chapter GetChapterForNumber(int iChapterNumber)
    {
        return m_chapters[iChapterNumber - 1];
    }

    /**
     * Returns the absolute level number for relative level number inside current chapter
     * **/
    public int GetAbsoluteLevelNumberForCurrentChapterAndLevel(int iChapterRelativeLevelNumber)
    {
        return m_currentChapter.m_number * LEVELS_PER_CHAPTER + iChapterRelativeLevelNumber;
    }
}
