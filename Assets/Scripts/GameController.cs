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

    public Material m_debugMaterial;

    protected void Awake()
    {
        m_levelManager = null;
        m_sceneManager = null;
        m_guiManager = null;
        m_persistentDataManager = null;
        m_backgroundRenderer = null;
    }

    protected void Start()
    {
        //Set up background fill color
        //GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");
        //ColorQuad fillQuad = backgroundObject.GetComponentInChildren<ColorQuad>();
        //fillQuad.InitQuadMesh();
        //ColorQuadAnimator fillQuadAnimator = backgroundObject.GetComponentInChildren<ColorQuadAnimator>();
        //fillQuadAnimator.SetColor(0.12f * Color.white);
        //fillQuadAnimator.gameObject.transform.localScale = ScreenUtils.GetScreenSize();

        //UnitTests.Init();

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

        //ShowMainMenu();
        //DebugShowChapters();
        //DebugShowLevels(1);
        DebugShowSpecificLevel(2, 6, false);
        //DebugShowDebugLevel(3, false);
        //m_sceneManager.ShowContent(SceneManager.DisplayContent.LEVELS, true, 2.0f);

        //TouchHandler.s_touchDeactivated = false;
    }

    public void ShowMainMenu()
    {
        GetSceneManager().ShowContent(SceneManager.DisplayContent.MENU, 0.5f);

        GetGUIManager().ShowSideButtons(true, 1.0f);
    }

    /**
     * Tmp method to jump directly to chapters scene
     * **/
    public void DebugShowChapters()
    {
        GetSceneManager().ShowContent(SceneManager.DisplayContent.CHAPTERS, 0.5f);
    }

    /**
     * Tmp method to jump directly to levels scene
     * **/
    public void DebugShowLevels(int iChapterNumber)
    {
        GetLevelManager().SetCurrentChapterByNumber(iChapterNumber);

        GetSceneManager().ShowContent(SceneManager.DisplayContent.LEVELS, 0.5f);
    }

    /**
     * Tmp method to jump directly to game scene (with level intro scene optionally)
     * **/
    public void DebugShowSpecificLevel(int iChapterNumber, int iLevelNumber, bool bShowLevelIntro = true)
    {
        LevelManager levelManager = GetLevelManager();
        levelManager.SetCurrentChapterByNumber(iChapterNumber);
        levelManager.SetLevelOnCurrentChapter(iLevelNumber);

        GetSceneManager().ShowContent(bShowLevelIntro ? SceneManager.DisplayContent.LEVEL_INTRO : SceneManager.DisplayContent.GAME, 0.5f);
    }  

    /**
     * Tmp method to jump directly to a debug level (with level intro scene optionally)
     * **/
    public void DebugShowDebugLevel(int iDebugLevelNumber, bool bShowLevelIntro = true)
    {
        if (iDebugLevelNumber > 4)
            iDebugLevelNumber = 4;

        LevelManager levelManager = GetLevelManager();
        levelManager.SetCurrentLevelAsDebugLevel(iDebugLevelNumber);

        GetSceneManager().ShowContent(bShowLevelIntro ? SceneManager.DisplayContent.LEVEL_INTRO : SceneManager.DisplayContent.GAME, 0.5f);
    }

    public SceneManager GetSceneManager()
    {
        if (m_sceneManager == null)
            m_sceneManager = GetComponent<SceneManager>();

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
            m_levelManager = GetComponent<LevelManager>();

        return m_levelManager;
    }

    public PersistentDataManager GetPersistentManager()
    {
        if (m_persistentDataManager == null)
            m_persistentDataManager = GetComponent<PersistentDataManager>();

        return m_persistentDataManager;
    }

    public BackgroundTrianglesRenderer GetBackgroundRenderer()
    {
        if (m_backgroundRenderer == null)
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundTrianglesRenderer>();

        return m_backgroundRenderer;
    }
}
