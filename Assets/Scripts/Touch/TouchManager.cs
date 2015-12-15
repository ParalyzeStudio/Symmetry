using UnityEngine;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour
{
    public const float MOVE_EPSILON = 0.5f;

    public bool m_touchDeactivated { get; set; }




#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WINRT || UNITY_BLACKBERRY //touch devices
    protected int m_touchCount;
#elif UNITY_STANDALONE || UNITY_WEBPLAYER
    protected bool m_mouseButtonPressed;
#endif

    public Vector2 m_prevPointerLocation { get; set; }
    public Vector2 m_pointerDeltaLocation { get; set; }

    public enum PointerEventType
    {
        NONE = 0,
        POINTER_DOWN,
        POINTER_MOVE,
        POINTER_UP
    }

    private SceneManager m_sceneManager;
    private GUIManager m_guiManager;

    public void Awake()
    {
        m_sceneManager = null;
        m_guiManager = null;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WINRT || UNITY_BLACKBERRY //touch devices
        m_touchCount = 0;
#elif UNITY_STANDALONE || UNITY_WEBPLAYER
    m_mouseButtonPressed = false;
#endif
    }

    /**
     * Update function to handle touches/clicks
     * **/
    void Update()
    {
        if (m_touchDeactivated)
            return;

        SceneManager sceneManager = GetSceneManager();
        GUIManager guiManager = GetGUIManager();

        PointerEventType eventType = PointerEventType.NONE;
        Vector2 pointerLocation = Vector2.zero;
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WINRT || UNITY_BLACKBERRY //touch devices
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            pointerLocation = Camera.main.ScreenToWorldPoint(touch.position);

            if (m_touchCount == 0) //first press = OnPointerDown
            {
                eventType = PointerEventType.POINTER_DOWN;
                m_prevPointerLocation = pointerLocation;
            }
            else //other presses = OnPointerMove
            {
                m_pointerDeltaLocation = pointerLocation - m_prevPointerLocation;
                if (m_pointerDeltaLocation.sqrMagnitude >= MOVE_EPSILON)
                {
                    eventType = PointerEventType.POINTER_MOVE;
                    m_prevPointerLocation = pointerLocation;
                }
            }
        }
        else if (Input.touchCount == 0)
        {
            if (m_touchCount == 1) //we switched from a press state to a release state
            {
                eventType = PointerEventType.POINTER_UP;
            }
        }
        m_touchCount = Input.touchCount;
#elif UNITY_STANDALONE || UNITY_WEBPLAYER //devices with mouse

        if (Input.GetMouseButton(0))
        {
            pointerLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!m_mouseButtonPressed) //first press = OnPointerDown
            {
                eventType = PointerEventType.POINTER_DOWN;
                m_prevPointerLocation = pointerLocation;
                m_mouseButtonPressed = true;
            }
            else //other presses = OnPointerMove
            {
                m_pointerDeltaLocation = pointerLocation - m_prevPointerLocation;
                if (m_pointerDeltaLocation.sqrMagnitude >= MOVE_EPSILON)
                {
                    eventType = PointerEventType.POINTER_MOVE;
                    m_prevPointerLocation = pointerLocation;
                }
            }
        }
        else
        {
            if (m_mouseButtonPressed) //we switched from a press state to a release state
            {
                eventType = PointerEventType.POINTER_UP;
                m_mouseButtonPressed = false;
            }
        }
#endif

        if (eventType != PointerEventType.NONE)
        {
            bool bProcessed = guiManager.GetComponent<GUITouchHandler>().ProcessPointerEvent(pointerLocation, eventType);

            if (sceneManager.m_displayedContent == SceneManager.DisplayContent.GAME)
            {
                if (!bProcessed) //pointer event has not been processed by GUI, try HUD first
                {
                    GameScene gameScene = (GameScene)sceneManager.m_currentScene;
                    if (gameScene.m_gameStatus == GameScene.GameStatus.RUNNING)
                    {
                        ////try to process the event on the grid itself
                        GridTouchHandler gridTouchHandler = gameScene.m_grid.GetComponent<GridTouchHandler>();
                        gridTouchHandler.ProcessPointerEvent(pointerLocation, eventType);

                        //or the shapes
                        List<Shape> shapes = gameScene.m_shapesHolder.m_shapes;
                        for (int iShapeIdx = 0; iShapeIdx != shapes.Count; iShapeIdx++)
                        {
                            ShapeTouchHandler shapeTouchHandler = shapes[iShapeIdx].m_parentMesh.gameObject.GetComponent<ShapeTouchHandler>();
                            shapeTouchHandler.ProcessPointerEvent(pointerLocation, eventType);
                        }
                    }
                }
            }
        }
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
}
