using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public List<Level> m_levels;

    public LevelManager()
    {
        m_levels = new List<Level>();
    }

    public void ParseLevelsFile()
    {
        TextAsset levelsFile = (TextAsset) Resources.Load("levels");

        XMLParser xmlParser = new XMLParser();
        XMLNode rootNode = xmlParser.Parse(levelsFile.text);

        XMLNodeList levelsNodeList = rootNode.GetNodeList("levels>0>level");
        foreach (XMLNode levelNode in levelsNodeList)
        {
            Level level = new Level();
            string levelName = levelNode.GetValue("@name");
            Debug.Log(levelName);

            XMLNodeList contoursNodeList = levelNode.GetNodeList("contours>0>contour");
            foreach (XMLNode contourNode in contoursNodeList)
            {
                Contour contour = new Contour();
                XMLNodeList contourPointsNodeList = contourNode.GetNodeList("point");
                foreach (XMLNode contourPointsNode in contourPointsNodeList)
                {
                    string strContourPointLine = contourPointsNode.GetValue("@line");
                    string strContourPointColumn = contourPointsNode.GetValue("@column");

                    int contourPointLine = int.Parse(strContourPointLine);
                    int contourPointColumn = int.Parse(strContourPointColumn);

                    contour.m_points.Add(new Vector2(contourPointLine, contourPointColumn));
                }
                level.m_contours.Add(contour);
            }

            m_levels.Add(level);
        }
    }
}
