using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private LevelManager m_levelManager; //variable to store the levelManager that is used in the update loop
    private SceneManager m_sceneManager; //same thing for the scene manager

    public Vector2 m_designScreenSize; //the design screen size that can be set in the inspector

    public enum GameStatus
    {
        RUNNING,
        PAUSED,
        VICTORY,
        DEFEAT,
        FINISHED //the level is done and player is waiting for current level to restart or next level to start
    };

    private GameStatus m_gameStatus;

    protected void Start()
    {
        m_levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        m_sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();

        //set correct size for background
        GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");
        backgroundObject.GetComponent<BackgroundAdaptativeSize>().InvalidateSize();

        //parse xml levels files
        m_levelManager.ParseAllLevels();

        //m_sceneManager.ShowContent(SceneManager.DisplayContent.MENU, true, 2.0f);
        m_levelManager.m_currentChapter = m_levelManager.m_chapters[0];
        //m_sceneManager.ShowContent(SceneManager.DisplayContent.MENU, true, 2.0f);
        m_sceneManager.ShowContent(SceneManager.DisplayContent.LEVELS, true, 2.0f);

        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.Init();
        //guiManager.AnimateFrames(SceneManager.DisplayContent.MENU, 2.3f);
        guiManager.AnimateFrames(SceneManager.DisplayContent.LEVELS, 2.3f);

        TouchHandler.s_touchDeactivated = false;
    }

    protected void Update()
    {
        if (m_sceneManager.m_displayedContent == SceneManager.DisplayContent.GAME)
        {
            GameStatus gameStatus = GetGameStatus();
            if (gameStatus == GameStatus.VICTORY || gameStatus == GameStatus.DEFEAT)
            {
                EndLevel(gameStatus);
                m_gameStatus = GameStatus.FINISHED;
            }
        }
    }    

    /**
     * Returns the current status of the game
     * **/
    public GameStatus GetGameStatus()
    {
        if (m_gameStatus == GameStatus.DEFEAT || m_gameStatus == GameStatus.VICTORY || m_gameStatus == GameStatus.FINISHED)
            return m_gameStatus;

        bool victory = IsVictory();
        if (victory)
            m_gameStatus = GameStatus.VICTORY;
        else
        {
            bool defeat = IsDefeat();
            if (defeat)
                m_gameStatus = GameStatus.DEFEAT;
            else //neither victory nor defeat, keep playing
                m_gameStatus = GameStatus.RUNNING;
        }

        return m_gameStatus;
    }

    /**
     * Checks if the contour is filled exactly
     * Calculate the sum of the areas of contours and compare it to the area occupied by all the shapes
     * **/
    public bool IsVictory()
    {
        //First we check if one of the shapes intersects a contour
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;

        List<GameObject> allShapeObjects = gameScene.m_shapes.m_shapesObj;
        List<DottedOutline> allContours = gameScene.m_outlines.m_outlinesList;
        float shapesArea = 0;
        for (int iShapeIndex = 0; iShapeIndex != allShapeObjects.Count; iShapeIndex++)
        {
            Shape shape = allShapeObjects[iShapeIndex].GetComponent<ShapeRenderer>().m_shape;
            bool shapeInsideOutline = false;
            for (int iOutlineIndex = 0; iOutlineIndex != allContours.Count; iOutlineIndex++)
            {
                DottedOutline outline = allContours[iOutlineIndex];
                if (shape.IntersectsOutline(outline)) //we check if this shape intersects an outline
                {
                    return false;
                }
                else //if not we check if this shape is inside an outline
                {
                    if (outline.ContainsGridPoint(shape.m_gridTriangles[0].GetBarycentre()))
                    {
                        shapeInsideOutline = true;
                        break;
                    }
                }
            }

            if (!shapeInsideOutline)
                return false;

            shapesArea += shape.m_area;
        }

        //Debug.Log("1: NO SHAPE/OUTLINE INTERSECTION");

        //finally we check if the sum of the areas of all shapes is equal to the sum of the areas of all contours
        float contoursArea = 0;
        for (int iContourIndex = 0; iContourIndex != allContours.Count; iContourIndex++)
        {
            DottedOutline contour = allContours[iContourIndex];
            contoursArea += contour.m_area;
        }

        //Debug.Log("contoursArea:" + contoursArea);
        //Debug.Log("shapesArea:" + shapesArea);

        //if (contoursArea == shapesArea)
        //    Debug.Log("3: SAME AREA");

        return (contoursArea == shapesArea);
    }

    /**
     * Simply checks if counter is at maximum fill, because we know that conditions of victory have already been checked out negatively at this point
     * **/
    public bool IsDefeat()
    {
        GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;

        bool bDefeat = gameScene.m_counter.isFull();
        if (bDefeat)
            Debug.Log("DEFEAT");
        return gameScene.m_counter.isFull();
    }

    /**
     * If victory:
     * -Ends the current level by fading out the grid and contours and disabling touch
     * -After a few seconds launch next level
     * If defeat:
     * -Restart the level
     * **/
    public void EndLevel(GameStatus gameStatus)
    {        
        if (gameStatus == GameStatus.VICTORY)
        {
            Debug.Log("EndLevel VICTORY");
            GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
            gameScene.m_grid.Dismiss(1.0f);
            gameScene.m_outlines.Dismiss(1.0f);
        }
        else if (gameStatus == GameStatus.DEFEAT)
        {
            Debug.Log("EndLevel DEFEAT");
            //m_sceneManager.SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false, 0.0f, 0.5f, 0.5f); //restart the level
        }
    }
}
