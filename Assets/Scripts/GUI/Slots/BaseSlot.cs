using UnityEngine;

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
    public virtual void Dismiss(float fDuration, bool bDestroyOnFinish = true)
    {
        DismissSlotBackground(fDuration, true);
        DismissSlotContour(fDuration, true);
        DismissSlotInformation(fDuration, true);

        if (bDestroyOnFinish)
            Destroy(this.gameObject, fDuration);
    }

    /**
    * Show the slot hexagon background 
    * **/
    public virtual void ShowSlotBackground()
    {
        
    }

    /**
    * Show the slot hexagon contour 
    * **/
    public virtual void ShowSlotContour()
    {

    }

    /**
     * Show the slot hexagon background 
     * **/
    public virtual void ShowSlotInformation()
    {

    }

    /**
     * Fade out background with eventually destroying the object at zero opacity
     * **/
    public virtual void DismissSlotBackground(float fDuration, bool bDestroyOnFinish = true)
    {

    }

    /**
     * Fade out contour with eventually destroying the object at zero opacity
     * **/
    public virtual void DismissSlotContour(float fDuration, bool bDestroyOnFinish = true)
    {

    }

    /**
     * Fade out information with eventually destroying the object at zero opacity
     * **/
    public virtual void DismissSlotInformation(float fDuration, bool bDestroyOnFinish = true)
    {
        
    }
}
