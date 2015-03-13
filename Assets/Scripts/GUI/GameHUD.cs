using UnityEngine;
using System.Collections.Generic;

public class GameHUD : MonoBehaviour
{
    public List<HUDButton> m_actionButtons { get; set; }
    public HUDButton m_selectedActionButton { get; set; } //the button the player has selected, null if none is selected
    public List<HUDButton> m_interfaceButtons { get; set; }

    public GameObject m_horAxisPfb;
    public GameObject m_vertAxisPfb;
    public GameObject m_straightAxesPfb;
    public GameObject m_leftDiagonalAxisPfb;
    public GameObject m_rightDiagonalAxisPfb;
    public GameObject m_diagonalsAxesPfb;
    public GameObject m_allAxesPfb;

    //Action tags
    public const string ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL = "SYMMETRY_AXIS_HORIZONTAL";
    public const string ACTION_TAG_SYMMETRY_AXIS_VERTICAL = "SYMMETRY_AXIS_VERTICAL";
    public const string ACTION_TAG_SYMMETRY_AXES_STRAIGHT = "SYMMETRY_AXES_STRAIGHT";
    public const string ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT = "SYMMETRY_AXIS_DIAGONAL_LEFT";
    public const string ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT = "SYMMETRY_AXIS_DIAGONAL_RIGHT";
    public const string ACTION_TAG_SYMMETRY_AXES_DIAGONALS = "SYMMETRY_AXES_DIAGONALS";
    public const string ACTION_TAG_SYMMETRY_AXES_ALL = "SYMMETRY_AXES_ALL";

    public string m_activeActionTag { get; set; }    

    /**
     * Build the GUI top banner
     * **/
    public void BuildForLevel(int iLevelNumber)
    {
        m_activeActionTag = null;
        BuildActionButtonsForLevel(iLevelNumber);
        BuildInterfaceButtons();
    }

    /**
     * Creates the buttons for drawing/moving elements onto grid
     * **/
    public void BuildActionButtonsForLevel(int iLevelNumber)
    {
        m_actionButtons = new List<HUDButton>();

        GameObject backgroundObject = GameObject.FindGameObjectWithTag("Background");
        Vector2 screenSize = backgroundObject.GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        LevelManager levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        List<string> actionTags = levelManager.m_currentLevel.m_actionButtonsTags;

        float gapBetweenButtons = 20.0f;
        float distanceToScreenLeftSide = 60.0f;
        float distanceToScreenTopSide = 30.0f;
        Vector2 actionButtonSize = new Vector2(128.0f, 128.0f);
        for (int iTagIndex = 0; iTagIndex != actionTags.Count; iTagIndex++)
        {
            //calculate the local position of the button
            int numberOfActionButtons = m_actionButtons.Count;
            float xPosition = -0.5f * screenSize.x + 
                              distanceToScreenLeftSide + 
                              numberOfActionButtons * actionButtonSize.x +
                              numberOfActionButtons * gapBetweenButtons +
                              0.5f * actionButtonSize.x;
            float yPosition = -distanceToScreenTopSide;
            Vector2 buttonLocalPosition = new Vector2(xPosition, yPosition);

            string tag = actionTags[iTagIndex];
            GameObject clonedButton  = null;
            if (tag.Equals(ACTION_TAG_SYMMETRY_AXES_ALL))
            {
                clonedButton = (GameObject)Instantiate(m_allAxesPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                HUDButton hudButton = clonedButton.GetComponentInChildren<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_ALL_AXES;
                hudButton.m_type = HUDButton.HUDButtonType.ACTION;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL))
            {
                clonedButton = (GameObject)Instantiate(m_horAxisPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                HUDButton hudButton = clonedButton.GetComponentInChildren<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_HORIZONTAL;
                hudButton.m_type = HUDButton.HUDButtonType.ACTION;
            }
            else if (tag.Equals(ACTION_TAG_SYMMETRY_AXIS_VERTICAL))
            {
                clonedButton = (GameObject)Instantiate(m_vertAxisPfb);
                clonedButton.transform.parent = this.gameObject.transform;
                clonedButton.transform.localPosition = buttonLocalPosition;
                HUDButton hudButton = clonedButton.GetComponentInChildren<HUDButton>();
                hudButton.m_ID = HUDButton.HUDButtonID.ID_SYMMETRY_AXIS_VERTICAL;
                hudButton.m_type = HUDButton.HUDButtonType.ACTION;
            }

            if (clonedButton != null)
                m_actionButtons.Add(clonedButton.GetComponentInChildren<HUDButton>());
        }
    }

    /**
     * Create the pause, retry buttons and stuff
     * **/
    public void BuildInterfaceButtons()
    {
        m_interfaceButtons = new List<HUDButton>();
    }
}
