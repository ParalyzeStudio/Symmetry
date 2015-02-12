using UnityEngine;
using System.Collections.Generic;

public class GameHUD : MonoBehaviour
{
    public List<HUDButton> m_buttons;

    public GameObject m_horAxisPfb;
    public GameObject m_vertAxisPfb;
    public GameObject m_straightAxesPfb;
    public GameObject m_leftDiagonalAxisPfb;
    public GameObject m_rightDiagonalAxisPfb;
    public GameObject m_diagonalsAxesPfb;
    public GameObject m_allAxesPfb;

    /**
     * Creates the buttons for drawing/moving elements onto grid
     * 
     * **/
    public void BuildActionButtonsForLevel(int iLevelNumber)
    {
        GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");
        Vector2 screenSize = backgroundObject.GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        List<string> actionTags = levelManager.m_currentLevel.m_actionButtonsTags;

        float gapBetweenButtons = 20.0f;
        float distanceToScreenLeftSide = 0.0f;
        Vector2 actionButtonSize = new Vector2(128.0f, 128.0f);
        for (int iTagIndex = 0; iTagIndex != actionTags.Count; iTagIndex++)
        {
            //calculate the local position of the button
            int numberOfActionButtons = m_buttons.Count;
            float xPosition = -0.5f * screenSize.x + 
                              distanceToScreenLeftSide + 
                              numberOfActionButtons * actionButtonSize.x +
                              numberOfActionButtons * gapBetweenButtons +
                              0.5f * actionButtonSize.x;
            Vector2 buttonLocalPosition = new Vector2(xPosition, 0);

            string tag = actionTags[iTagIndex];
            GameObject clonedButton  = null;
            if (tag.Equals("SYMMETRY_AXES_ALL"))
            {
                clonedButton = (GameObject)Instantiate(m_allAxesPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
            }
            else if (tag.Equals("SYMMETRY_AXIS_HORIZONTAL"))
            {
                clonedButton = (GameObject)Instantiate(m_horAxisPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
            }
            else if (tag.Equals("SYMMETRY_AXIS_VERTICAL"))
            {
                clonedButton = (GameObject)Instantiate(m_vertAxisPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
            }

            if (clonedButton != null)
                m_buttons.Add(clonedButton.GetComponentInChildren<HUDButton>());
        }
    }

    /**
     * Create the pause, retry buttons and stuff
     * **/
    public void BuildInterfaceButtons()
    {

    }
}
