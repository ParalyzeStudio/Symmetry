using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    private List<Level> m_levels;
    public Level m_currentLevel { get; set; }

    public void Awake()
    {
        m_levels = new List<Level>();
        m_currentLevel = null;
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

            //Parse contours
            XMLNodeList contoursNodeList = levelNode.GetNodeList("contours>0>contour");
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

                level.m_shapes.Add(shape);
            }

            m_levels.Add(level);
        }
    }

    public void SetCurrentLevelByNumber(int iLevelNumber)
    {
        if (iLevelNumber < m_levels.Count + 1)
        {
            m_currentLevel = m_levels[iLevelNumber - 1];
        }
    }
}
