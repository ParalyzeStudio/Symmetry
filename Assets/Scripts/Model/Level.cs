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

    //Constraints on axes
    public const int CONSTRAINT_SYMMETRY_AXIS_HORIZONTAL = 1;
    public const int CONSTRAINT_SYMMETRY_AXIS_VERTICAL = 2;
    public const int CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_LEFT = 4;
    public const int CONSTRAINT_SYMMETRY_AXIS_DIAGONAL_RIGHT = 8;

    private int m_constraints;
    public int Constraints
    {
        get
        {
            return m_constraints;
        }
    }

    //available actions
    public const int ACTION_ONE_SIDED_SYMMETRY = 1;
    public const int ACTION_TWO_SIDED_SYMMETRY = 2;
    public const int ACTION_POINT_SYMMETRY = 4;
    public const int ACTION_MOVE_SHAPE = 8;

    private int m_actions;
    public int Actions
    {
        get
        {
            return m_actions;
        }
    }

    public Level()
    {
        m_outlines = new List<DottedOutline>();
        m_initialShapes = new List<Shape>();

        m_gridMinNumLines = 0;
        m_gridMinNumColumns = 0;
        m_gridExactNumLines = 0;
        m_gridExactNumColumns = 0;

        m_statusDirty = true;
    }

    public void AddConstraintFromFile(string constraint)
    {
        int constraintValue = (int) this.GetType().GetField(constraint).GetValue(null);
        m_constraints |= constraintValue;
    }

    public void AddActionFromFile(string action)
    {
        int actionValue = (int)this.GetType().GetField(action).GetValue(null);
        m_actions |= actionValue;
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
