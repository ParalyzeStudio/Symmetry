using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public const int LEVELS_COUNT = 90;

    private Level[] m_levels;
    public Level m_currentLevel { get; set; }

    public void Awake()
    {
        m_levels = new Level[LEVELS_COUNT];
        //nullify elements in this array
        for (int i = 0; i != LEVELS_COUNT; i++)
        {
            m_levels[i] = null;
        }
        m_currentLevel = null;
    }

    public void ParseAllLevels()
    {
        for (int iLevelNumber = 1; iLevelNumber != LEVELS_COUNT + 1; iLevelNumber++)
        {
            Level level = ParseLevelFile(iLevelNumber);
            m_levels[iLevelNumber - 1] = level;
        }
    }

    public Level ParseLevelFile(int iLevelNumber)
    {
        string levelFilename = "level_" + iLevelNumber;
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
        if (iLevelNumber < m_levels.Length + 1)
        {
            m_currentLevel = m_levels[iLevelNumber - 1];
        }
    }
}
