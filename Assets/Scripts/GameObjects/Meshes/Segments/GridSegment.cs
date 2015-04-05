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

    public void SetStartPointInGridCoordinates(Vector2 startPoint)
    {
        m_startPointGrid = startPoint;
        m_prevStartPointGrid = startPoint;

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 worldStartPoint = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(startPoint);
        SetStartPoint(worldStartPoint);
    }

    public void SetEndPointInGridCoordinates(Vector2 endPoint)
    {
        m_endPointGrid = endPoint;
        m_prevEndPointGrid = endPoint;

        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        Vector2 worldEndPoint = gameScene.m_grid.GetWorldCoordinatesFromGridCoordinates(endPoint);
        SetEndPoint(worldEndPoint);
    }

    protected override void Update()
    {
        if (m_prevStartPointGrid != m_startPointGrid ||
            m_prevEndPointGrid != m_endPointGrid)
        {
            SetStartPointInGridCoordinates(m_startPointGrid);
            SetEndPointInGridCoordinates(m_endPointGrid);  
        }

        base.Update();
    }
}

