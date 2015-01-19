using UnityEngine;
using System.Collections;


/**
 * Base Class that handles touches or clicks on the scene
 * **/
public class TouchHandler : MonoBehaviour
{
    public const float MOVE_EPSILON = 0.5f;

    //public Vector2 m_touchArea;
    protected bool m_selected;
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
    protected bool m_mouseButtonPressed;
#endif
    protected Vector2 m_prevPointerLocation;

    public virtual void Awake()
    {
        m_selected = false;
        m_mouseButtonPressed = false;
    }

    public virtual void Start()
    {

    }

    /**
     * Virtual method that checks if a point is inside the touch area of the object
     * **/
    protected virtual bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return false;
    }

    /**
     * Player touched this object
     * **/
    protected virtual void OnPointerDown(Vector2 pointerLocation)
    {
        m_selected = true;
        m_prevPointerLocation = pointerLocation;
    }

    /**
     * Player moved the pointer with object selected
     * **/
    protected virtual bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (!m_selected)
            return false;

        delta = pointerLocation - m_prevPointerLocation;
        if (delta.sqrMagnitude < MOVE_EPSILON)
            return false;

        return true;
    }

    /**
     * Player released the pointer
     * **/
    protected virtual void OnPointerUp()
    {
        if (m_selected && IsPointerLocationContainedInObject(m_prevPointerLocation))
            OnClick();

        m_selected = false;
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        m_mouseButtonPressed = false;
#endif
    }

    /**
     * Player clicked on this object
     * **/
    protected virtual void OnClick()
    {

    }

    /**
     * Handles the touches/click on nodes to drag them properly or on scene to slide it
     * -1 touch: drag a node or slide the scene
     * -2 touches: zoom in/out the scene
     * **/
    void Update()
    {
        Vector2 touchLocation;
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
            touchLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!m_mouseButtonPressed) //first press = OnPointerDown
            {
                m_mouseButtonPressed = true;
                if (IsPointerLocationContainedInObject(touchLocation))
                {                    
                    OnPointerDown(touchLocation);
                }
                //Rect touchAreaRect = new Rect();
                //Vector2 position = transform.position;
                //touchAreaRect.position = position - 0.5f * m_touchArea;
                //touchAreaRect.width = m_touchArea.x;
                //touchAreaRect.height = m_touchArea.y;
                //if (touchAreaRect.Contains(touchLocation))
                //{
                //    OnPointerDown(touchLocation);
                //}
            }
            else //other presses = OnPointerMove
            {
                Vector2 delta = Vector2.zero;
                OnPointerMove(touchLocation, ref delta);
            }

            m_prevPointerLocation = touchLocation;
        }
        else
        {
            if (m_mouseButtonPressed) //use the m_mouseButtonPressed to call the OnPointerUp() method only once
            {
                OnPointerUp();
            }
        }
#endif
    }
}
