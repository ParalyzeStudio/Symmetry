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

    private HUDButton FindButtonContainingPointerLocation(Vector2 pointerLocation)
    {
        List<HUDButton> buttons = this.gameObject.GetComponent<GameHUD>().m_actionButtons;
        for (int iButtonIndex = 0; iButtonIndex != buttons.Count; iButtonIndex++)
        {
            HUDButton button = buttons[iButtonIndex];
            Vector2 buttonPosition = button.gameObject.transform.position;
            Vector2 buttonSize = button.gameObject.transform.localScale;
            Rect buttonRect = new Rect(-0.5f * buttonSize.x,
                                       -0.5f * buttonSize.y,
                                       buttonSize.x,
                                       buttonSize.y);

            Vector2 localPointerLocation = pointerLocation - buttonPosition;
            localPointerLocation.y = -localPointerLocation.y; //reverse the y coordinates to match the rect axes system
            if (buttonRect.Contains(localPointerLocation))
            {
                return button;
            }
        }

        return null;
    }

    private void ReleaseAllActionButtons(List<HUDButton> exceptButtons)
    {
        List<HUDButton> buttons = this.gameObject.GetComponent<GameHUD>().m_actionButtons;
        if (exceptButtons.Count == 0)
        {
            for (int iButtonIndex = 0; iButtonIndex != buttons.Count; iButtonIndex++)
            {
                buttons[iButtonIndex].OnRelease();
            }
        }
        else
        {
            for (int iButtonIndex = 0; iButtonIndex != buttons.Count; iButtonIndex++)
            {
                HUDButton button = buttons[iButtonIndex];
                for (int iExceptButtonIndex = 0; iExceptButtonIndex != exceptButtons.Count; iExceptButtonIndex++)
                {
                    HUDButton exceptButton = exceptButtons[iExceptButtonIndex];
                    if (button != exceptButton)
                        buttons[iButtonIndex].OnRelease();
                }               
            }
        }        
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        HUDButton button = FindButtonContainingPointerLocation(pointerLocation);
        if (button)
            button.OnPress();
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
       if (!base.OnPointerMove(pointerLocation, ref delta))
            return false;

        HUDButton button = FindButtonContainingPointerLocation(pointerLocation);
        if (button != null)
            button.OnPress();
        else
        {
            //Release previously pressed button by releasing everything except the selected actionButton
            HUDButton selectedActionButton = this.gameObject.GetComponent<GameHUD>().m_selectedActionButton;
            if (selectedActionButton != null)
            {
                List<HUDButton> releaseExceptButtons = new List<HUDButton>();
                releaseExceptButtons.Add(selectedActionButton);
                ReleaseAllActionButtons(releaseExceptButtons);
            }
        }
        
        return true;
    }

    protected override void OnPointerUp()
    {
        base.OnPointerUp();

        //Check if OnPointerUp() has been called on an action button
        GameHUD gameHUD = this.gameObject.GetComponent<GameHUD>();
        List<HUDButton> buttons = this.gameObject.GetComponent<GameHUD>().m_actionButtons;
        for (int iButtonIndex = 0; iButtonIndex != buttons.Count; iButtonIndex++)
        {
            HUDButton button = buttons[iButtonIndex];
            if (button.m_state == GUIQuadButton.ButtonState.PRESSED) //this button has been pressed
            {
                if (gameHUD.m_selectedActionButton == null) //no action button was selected previously 
                {
                    button.OnClick();
                    gameHUD.m_selectedActionButton = button;
                }
                else
                {
                    if (gameHUD.m_selectedActionButton != button)
                    {
                        gameHUD.m_selectedActionButton.OnRelease(); //release the previously selected button
                        button.OnClick();
                        gameHUD.m_selectedActionButton = button;
                    }
                }
            }
        }
    }
}
