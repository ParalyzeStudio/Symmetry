﻿using UnityEngine;

public class BaseSlot : MonoBehaviour
{
    protected GUIScene m_parentScene;
    public GameObject m_background;
    public GameObject m_contour;

    public void Init(GUIScene parentScene)
    {
        m_parentScene = parentScene;
    }

    /**
     * Show the whole slot
     * **/
    public virtual void Show()
    {
        ShowSlotBackground();
        ShowSlotContour();
        ShowSlotInformation();
    }

    /**
     * Dismiss and remove the whole slot
     * **/
    public virtual void Dismiss()
    {
        DismissSlotBackground(true);
        DismissSlotContour(true);
        DismissSlotInformation(true);
    }

    /**
    * Show the slot hexagon background 
    * **/
    protected virtual void ShowSlotBackground()
    {
        
    }

    /**
    * Show the slot hexagon contour 
    * **/
    protected virtual void ShowSlotContour()
    {

    }

    /**
     * Show the slot hexagon background 
     * **/
    protected virtual void ShowSlotInformation()
    {

    }

    /**
     * Fade out background with eventually destroying the object at zero opacity
     * **/
    protected virtual void DismissSlotBackground(bool bDestroyOnFinish)
    {

    }

    /**
     * Fade out contour with eventually destroying the object at zero opacity
     * **/
    protected virtual void DismissSlotContour(bool bDestroyOnFinish)
    {

    }

    /**
     * Fade out information with eventually destroying the object at zero opacity
     * **/
    protected virtual void DismissSlotInformation(bool bDestroyOnFinish)
    {
        
    }
}
