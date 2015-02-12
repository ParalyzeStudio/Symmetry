using UnityEngine;
using System.Collections.Generic;

public class HUDTouchHandler : TouchHandler
{
    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        //HUD top banner is 180 units tall
        float bannerHeight = 180.0f;
        Vector2 designScreenSize = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().m_designScreenSize;
        return pointerLocation.y >= (0.5f * designScreenSize.y - bannerHeight);
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        m_prevPointerLocation = pointerLocation;
        m_pointerDownPointerLocation = pointerLocation;

        List<HUDButton> buttons = this.gameObject.GetComponent<GameHUD>().m_buttons;
        for (int iButtonIndex = 0; iButtonIndex != buttons.Count; iButtonIndex++)
        {
            HUDButton button = buttons[iButtonIndex];
            Vector2 buttonPosition = button.gameObject.transform.position;
            Vector2 buttonSize = button.gameObject.transform.localScale;
            //Rect buttonRect = new Rect(buttonPosition.x - 0.5f * buttonSize.x,
            //                           buttonPosition.y + 0.5f * buttonSize.y,
            //                           buttonSize.x,
            //                           buttonSize.y);
            Rect buttonRect = new Rect(-0.5f * buttonSize.x,
                                       -0.5f * buttonSize.y,
                                       buttonSize.x,
                                       buttonSize.y);

            Vector2 localPointerLocation = pointerLocation - buttonPosition;
            localPointerLocation.y = -localPointerLocation.y; //reverse the y coordinates to match the rect axes system
            if (buttonRect.Contains(localPointerLocation))
            {
                button.OnPress();
                break; //swallow the touch
            }
        }
    }
}
