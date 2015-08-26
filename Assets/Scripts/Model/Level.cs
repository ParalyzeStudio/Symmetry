using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{
    public int m_chapterRelativeNumber { get; set; } //the number of this level
    public Chapter m_parentChapter;
    public string m_name { get; set; }
    public List<DottedOutline> m_outlines { get; set; }
    public List<Shape> m_initialShapes { get; set; }
    public List<string> m_axisConstraints { get; set; }
    public int m_maxActions { get; set; }
    public int m_gridMinNumLines { get; set; }
    public int m_gridMinNumColumns { get; set; }
    public int m_gridExactNumLines { get; set; }
    public int m_gridExactNumColumns { get; set; }
    public float m_maxGridSpacing { get; set; }

    public Level()
    {
        m_outlines = new List<DottedOutline>();
        m_initialShapes = new List<Shape>();
        m_axisConstraints = new List<string>();

        m_gridMinNumLines = 0;
        m_gridMinNumColumns = 0;
        m_gridExactNumLines = 0;
        m_gridExactNumColumns = 0;
    }

    public bool IsDone()
    {
        PersistentDataManager persistentDataManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PersistentDataManager>();
        return persistentDataManager.IsLevelDone((m_parentChapter.m_number - 1) * Chapter.LEVELS_COUNT + m_chapterRelativeNumber);
    }
}
