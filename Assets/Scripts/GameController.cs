using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private LevelManager m_levelManager; //variable to store the levelManager that is used in the update loop
    private SceneManager m_sceneManager; //same thing for the scene manager
    private GUIManager m_guiManager; //same thing for the GUI manager
    private PersistentDataManager m_persistentDataManager;
    private BackgroundTrianglesRenderer m_backgroundRenderer;

    public Vector2 m_designScreenSize; //the design screen size that can be set in the inspector

    public bool m_finishingLevelVictory { get; set; }
    public float m_finishLevelVictoryElapsedTime { get; set; }
    public float m_finishLevelVictoryDelay { get; set; }

    public Material m_debugMaterial;

    public enum GameStatus
    {
        RUNNING,
        PAUSED,
        VICTORY,
        DEFEAT,
        FINISHED //the level is done and player is waiting for current level to restart or next level to start
    };

    private GameStatus m_gameStatus;

    protected void Awake()
    {
        m_levelManager = null;
        m_sceneManager = null;
        m_guiManager = null;
        m_persistentDataManager = null;
        m_backgroundRenderer = null;
    }

    //private void PerformTestCallFunc()
    //{
    //    Debug.Log("PerformTestCallFunc");
    //    CallFuncHandler callFuncHandler = this.GetComponent<CallFuncHandler>();
    //    callFuncHandler.AddCallFuncInstance(new CallFuncHandler.CallFunc(TestCallFunc), 5.0f);
    //}

    //private void TestCallFunc()
    //{
    //    Debug.Log("CallFunc");
    //}

    protected void Start()
    {
        //Set up background fill color
        //GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");
        //ColorQuad fillQuad = backgroundObject.GetComponentInChildren<ColorQuad>();
        //fillQuad.InitQuadMesh();
        //ColorQuadAnimator fillQuadAnimator = backgroundObject.GetComponentInChildren<ColorQuadAnimator>();
        //fillQuadAnimator.SetColor(0.12f * Color.white);
        //fillQuadAnimator.gameObject.transform.localScale = ScreenUtils.GetScreenSize();

        //parse xml levels files
        GetLevelManager().ParseAllLevels();

        //Init persistent data on first application launch
        GetPersistentManager().CreateAndInitAllLevelData();

        //Init the GUIManager
        GetGUIManager().Init();

        BackgroundTrianglesRenderer bgRenderer = GetBackgroundRenderer();
        bgRenderer.Init();
        bgRenderer.gameObject.transform.localPosition = new Vector3(0, 0, BackgroundTrianglesRenderer.BACKGROUND_TRIANGLES_Z_VALUE);

        m_levelManager.m_currentChapter = m_levelManager.m_chapters[0];
        ShowMainMenu();
        //DebugShowChapters();
        //DebugShowLevels(1);
        //DebugShowSpecificLevel(1, 1, false);
        //m_sceneManager.ShowContent(SceneManager.DisplayContent.LEVELS, true, 2.0f);

        //TouchHandler.s_touchDeactivated = false;

        m_finishingLevelVictory = false;

        //PerformTestCallFunc();
    }

    public void ShowMainMenu()
    {
        GetSceneManager().ShowContent(SceneManager.DisplayContent.MENU, true, 0.5f);

        GetGUIManager().ShowSideButtons(true, 1.0f);
    }

    /**
     * Tmp method to jump directly to chapters scene
     * **/
    public void DebugShowChapters()
    {
        GetSceneManager().ShowContent(SceneManager.DisplayContent.CHAPTERS, true, 0.5f);
    }

    /**
     * Tmp method to jump directly to levels scene
     * **/
    public void DebugShowLevels(int iChapterNumber)
    {
        GetLevelManager().SetCurrentChapterByNumber(iChapterNumber);

        GetBackgroundRenderer().Offset(2 * ScreenUtils.GetScreenSize().y);

        GetSceneManager().ShowContent(SceneManager.DisplayContent.LEVELS, true, 0.5f);
    }

    /**
     * Tmp method to jump directly to game scene (with level intro scene optionally)
     * **/
    public void DebugShowSpecificLevel(int iChapterNumber, int iLevelNumber, bool bShowLevelIntro = true)
    {
        LevelManager levelManager = GetLevelManager();
        levelManager.SetCurrentChapterByNumber(iChapterNumber);
        levelManager.SetLevelOnCurrentChapter(iLevelNumber);

        GetBackgroundRenderer().Offset(2 * ScreenUtils.GetScreenSize().y);

        GetSceneManager().ShowContent(bShowLevelIntro ? SceneManager.DisplayContent.LEVEL_INTRO : SceneManager.DisplayContent.GAME, true, 0.5f);
    }

    protected void Update()
    {
        return;
        if (m_sceneManager.m_displayedContent == SceneManager.DisplayContent.GAME)
        {
            GameStatus gameStatus = GetGameStatus();
            if (gameStatus == GameStatus.VICTORY || gameStatus == GameStatus.DEFEAT)
            {
                EndLevel(gameStatus);
                m_gameStatus = GameStatus.FINISHED;
            }
            else if (m_gameStatus == GameStatus.FINISHED)
            {
                if (m_finishingLevelVictory)
                {
                    float dt = Time.deltaTime;
                    m_finishLevelVictoryElapsedTime += dt;
                    if (m_finishLevelVictoryElapsedTime >= m_finishLevelVictoryDelay)
                    {
                        OnFinishEndingLevelVictory();
                        Debug.Log("OnFinishEndingLevelVictory");
                        m_finishingLevelVictory = false;
                    }
                }
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
        return false; //TODO remove this line

        //First we check if one of the shapes intersects a contour
        GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;

        if (!gameScene.m_isShown)
            return false;

        List<GameObject> allShapeObjects = gameScene.m_shapes.m_shapesObjects;
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
                    if (outline.ContainsGridPoint(shape.m_gridTriangles[0].GetCenter()))
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
        return false; //TODO remove this line

        GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;

        if (!gameScene.m_isShown)
            return false;

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
            GameScene gameScene = (GameScene) GetSceneManager().m_currentScene;
            gameScene.m_grid.Dismiss(2.0f, 1.0f);
            gameScene.m_outlines.Dismiss(true, 2.0f, 1.0f);
            gameScene.DismissInterfaceButtons(2.0f, 1.0f);

            LevelManager levelManager = GetLevelManager();
            int currentLevelNumber = levelManager.m_currentLevel.m_chapterRelativeNumber;
            if (currentLevelNumber < LevelManager.LEVELS_PER_CHAPTER - 1)
            {
                levelManager.SetLevelOnCurrentChapter(levelManager.m_currentLevel.m_chapterRelativeNumber + 1);

                m_finishingLevelVictory = true;
                m_finishLevelVictoryElapsedTime = 0.0f;
                m_finishLevelVictoryDelay = 3.0f;
            }
            else
            {
                //TODO go to next chapter by showing chapter menu for instance
            }

            //Save the status of this level to preferences
            GetPersistentManager().SetLevelDone(currentLevelNumber);

        }
        else if (gameStatus == GameStatus.DEFEAT)
        {
            Debug.Log("EndLevel DEFEAT");
            //GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, false, 0.0f, 0.5f, 0.5f); //restart the level
        }
    }

    /**
     * Function called when all scene elements have been faded out except shapes (victory)
     * Time to switch scene and go to next level or next chapter (if level 16 has been reached)
     * **/
    public void OnFinishEndingLevelVictory()
    {
        GetSceneManager().SwitchDisplayedContent(SceneManager.DisplayContent.LEVEL_INTRO, true, 5.0f); //restart the level
    }

    public SceneManager GetSceneManager()
    {
        if (m_sceneManager == null)
            m_sceneManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();

        return m_sceneManager;
    }

    public GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

        return m_guiManager;
    }

    public LevelManager GetLevelManager()
    {
        if (m_levelManager == null)
            m_levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

        return m_levelManager;
    }

    public PersistentDataManager GetPersistentManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PersistentDataManager>();

        return m_persistentDataManager;
    }

    public BackgroundTrianglesRenderer GetBackgroundRenderer()
    {
        if (m_backgroundRenderer == null)
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        return m_backgroundRenderer;
    }
}
