using UnityEngine;
using System.Collections.Generic;

public class TouchManager : MonoBehaviour
{
    public const float MOVE_EPSILON = 0.5f;

    public bool m_touchDeactivated { get; set; }

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
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
    }

    /**
     * Handles the touches/click on nodes to drag them properly or on scene to slide it
     * -1 touch: drag a node or slide the scene
     * -2 touches: zoom in/out the scene
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
            touchLocation = Camera.main.ScreenToWorldPoint(touch.position);

            if (!m_selected)
            {
                Rect touchAreaRect = new Rect();
                Vector2 position = transform.position;
                touchAreaRect.position = position - 0.5f * m_touchArea;
                touchAreaRect.width = m_touchArea.x;
                touchAreaRect.height = m_touchArea.y;
                if (touchAreaRect.Contains(touchLocation))
                {
                    OnPointerDown(touchLocation);
                }
            }
            else
            {
                Vector2 delta = Vector2.zero;
                OnPointerMove(touchLocation, ref delta);
            }
            m_prevPointerLocation = touchLocation;
        }
        else if (Input.touchCount == 0)
        {
            if (m_selected)
                OnPointerUp();
        }
        //TODO handle the case of 2 touches
        else if (Input.touchCount == 2)
        {

        }
#elif UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR //devices with mouse

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

                    ////try to process the event on the grid itself
                    GridTouchHandler gridTouchHandler = gameScene.m_grid.GetComponent<GridTouchHandler>();
                    gridTouchHandler.ProcessPointerEvent(pointerLocation, eventType);

                    //or the shapes
                    //List<GameObject> shapeObjects = gameScene.m_shapes.m_shapesObjects;
                    //for (int iShapeIdx = 0; iShapeIdx != shapeObjects.Count; iShapeIdx++)
                    //{
                    //    ShapeTouchHandler shapeTouchHandler = shapeObjects[iShapeIdx].GetComponent<ShapeTouchHandler>();
                    //    shapeTouchHandler.ProcessPointerEvent(pointerLocation, eventType);
                    //}
                }
            }
        }
    }

    public SceneManager GetSceneManager()
    {
        if (m_sceneManager == null)
            m_sceneManager = GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>();
        return m_sceneManager;
    }

    public GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        return m_guiManager;
    }
}
