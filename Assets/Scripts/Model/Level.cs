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
    public bool m_symmetriesStackable { get; set; }
    public int m_symmetryStackSize { get; set; }
    public int m_gridMinNumLines { get; set; }
    public int m_gridMinNumColumns { get; set; }
    public int m_gridExactNumLines { get; set; }
    public int m_gridExactNumColumns { get; set; }
    public float m_maxGridSpacing { get; set; }

    private bool m_statusDirty;
    private bool m_done;

    private PersistentDataManager m_persistentDataManager;

    public Level()
    {
        m_outlines = new List<DottedOutline>();
        m_initialShapes = new List<Shape>();
        m_axisConstraints = new List<string>();

        m_gridMinNumLines = 0;
        m_gridMinNumColumns = 0;
        m_gridExactNumLines = 0;
        m_gridExactNumColumns = 0;

        m_statusDirty = true;
    }

    public void SetAsDone()
    {
        if (!m_done)
        {
            m_done = true;
            GetPersistentDataManager().SetLevelDone((m_parentChapter.m_number - 1) * Chapter.LEVELS_COUNT + m_chapterRelativeNumber);
            m_statusDirty = false;
        }
    }

    public bool IsDone()
    {
        if (!m_statusDirty)
            return m_done;

        //we do not know for sure the status of this level, so check it in persistent data
        m_done = GetPersistentDataManager().IsLevelDone((m_parentChapter.m_number - 1) * Chapter.LEVELS_COUNT + m_chapterRelativeNumber);
        m_statusDirty = false;
        return m_done;
    }

    private PersistentDataManager GetPersistentDataManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PersistentDataManager>();

        return m_persistentDataManager;
    }
}
