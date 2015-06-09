using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BackgroundAdaptativeSize : MonoBehaviour 
{
    private Vector2 m_designScreenSize; //the size of the background as it was designed in photoshop
    private float m_previousCameraSize;
    public Vector2 m_screenSizeInUnits { get; set; }

    void Awake()
    {
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        GameController gameController = gameControllerObject.GetComponent<GameController>();
        m_designScreenSize = gameController.m_designScreenSize;
        m_previousCameraSize = 0; //initialization will be done in the first update call
    }

    void Start()
    {
        InvalidateSize();
    }

    /***
     *  Background do not scale when camera size changes so counter that effect by
     *  recaluting the new local scale of this quad object every frame
     ***/
    public void InvalidateSize()
    {
        float fCameraSize = Camera.main.orthographicSize;
        if (fCameraSize != m_previousCameraSize)
        {
            m_previousCameraSize = fCameraSize;
            float fScreenWidth = (float)Screen.width;
            float fScreenHeight = (float)Screen.height;
            float fDesignScreenRatio = m_designScreenSize.x / m_designScreenSize.y;
            float fScreenRatio = fScreenWidth / fScreenHeight;
            float fScreenHeightInUnits = 2.0f * fCameraSize;
            float fScreenWidthInUnits = fScreenRatio * fScreenHeightInUnits;

            float fBackgroundWidthInUnits, fBackgroundHeightInUnits;
            if (fScreenRatio <= fDesignScreenRatio)
            {
                fBackgroundHeightInUnits = fScreenHeightInUnits; //background image exactly fits the height of the screen 
                fBackgroundWidthInUnits = fDesignScreenRatio * fBackgroundHeightInUnits; //we scale the width (borders are cropped)
            }
            else
            {
                fBackgroundWidthInUnits = fScreenWidthInUnits; //background image exactly fits the width of the screen
                fBackgroundHeightInUnits = fBackgroundWidthInUnits / fDesignScreenRatio; //we scale the height (borders are cropped)
            }

            m_screenSizeInUnits = new Vector2(fScreenWidthInUnits, fScreenHeightInUnits);
            //this.transform.localScale = new Vector3(fBackgroundWidthInUnits, fBackgroundHeightInUnits, 1);
        }
    }

	public void Update() 
    {
        InvalidateSize();
	}
}
