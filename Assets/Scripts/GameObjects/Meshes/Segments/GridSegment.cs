using UnityEngine;

/**
 * Class to render a segment with both endpoints as grid coordinates points (column, line)
 * **/
public class GridSegment : Segment
{
    //coordinates of start and end point of this segment in grid coordinates (column, line)
    public Vector2 m_startPointGrid;
    public Vector2 m_endPointGrid;
    //store previous values of m_startPointGrid and m_endPointGrid to check if a change occured
    protected Vector2 m_prevStartPointGrid;
    protected Vector2 m_prevEndPointGrid;

    protected override void Start()
    {
        base.Start();
        m_prevStartPointGrid = new Vector2(-1, -1);
        m_prevEndPointGrid = new Vector2(-1, -1);
    }

    protected override void Update()
    {
        if (m_prevStartPointGrid != m_startPointGrid ||
            m_prevEndPointGrid != m_endPointGrid)
        {
            m_prevStartPointGrid = m_startPointGrid;
            m_prevEndPointGrid = m_endPointGrid;

            GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
            m_startPoint = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(m_startPointGrid);
            m_endPoint = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(m_endPointGrid);   
        }

        base.Update();
    }
}

