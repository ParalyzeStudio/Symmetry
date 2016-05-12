using UnityEngine;
using System.Collections.Generic;

public class ActionButton : GUIButton
{
    private ActionMenu m_parentMenu;

    //shared prefabs
    //public GameObject m_texQuadPfb;
    //public GameObject m_textMeshPfb;

    /**
     * Build an action button
     * **/
    public void Init(ActionMenu parentMenu, Material skinMaterial = null)
    {
        m_parentMenu = parentMenu;
        base.Init(skinMaterial); 
    }

    /**
     * Display this action button on the scene
     * **/
    public void Show(float fDelay = 0.0f)
    {
        
    }

    /**
     * Fades out and remove this button from scene
     * **/
    public void Dismiss(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator buttonAnimator = this.GetComponent<GameObjectAnimator>();
        buttonAnimator.FadeTo(0.0f, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }

    /**
     * Get the name of the displayed button ID
     * **/
    public string GetButtonName()
    {
        string buttonName = null;
        switch (m_ID)
        {
            case GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES:
                buttonName = LanguageUtils.GetTranslationForTag("action_symmetry_two_sides");
                break;
            case GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE:
                buttonName = LanguageUtils.GetTranslationForTag("action_symmetry_one_side");
                break;
            case GUIButtonID.ID_POINT_SYMMETRY:
                buttonName = LanguageUtils.GetTranslationForTag("action_point_symmetry");
                break;
            case GUIButtonID.ID_MOVE_SHAPE:
                buttonName = LanguageUtils.GetTranslationForTag("action_move_shape");
                break;
            case GUIButtonID.ID_OPERATION_ADD:
                buttonName = LanguageUtils.GetTranslationForTag("action_operation_addition");
                break;
            case GUIButtonID.ID_OPERATION_SUBSTRACT:
                buttonName = LanguageUtils.GetTranslationForTag("action_operation_substraction");
                break;
            case GUIButtonID.ID_COLOR_FILTER:
                buttonName = LanguageUtils.GetTranslationForTag("action_color_filter");
                break;
            default:
                buttonName = "DEFAULT";
                break;
        }

        if (buttonName == null)
            buttonName = "EMPTY NAME";

        //Add lines if button name is too long
        int lettersPerLine = 6;
        int currentLineLength = 0;
        string[] splitName = buttonName.Split();

        if (splitName.Length > 1)
        {
            buttonName = "";

            for (int i = 0; i != splitName.Length; i++)
            {
                string word = splitName[i];
                if (word.Length + currentLineLength > lettersPerLine)
                {
                    buttonName += "\n";
                    buttonName += word;
                    currentLineLength = word.Length;
                }
                else
                {
                    buttonName += " ";
                    buttonName += word;
                    currentLineLength += (word.Length + 1); //adds a space
                }
            }
        }

        return buttonName;
    }

    public override bool ContainsPoint(Vector2 point)
    {        
        Vector2 globalPosition = this.transform.position;

        float minX = globalPosition.x - 0.5f * m_touchArea.x;
        float maxX = globalPosition.x + 0.5f * m_touchArea.x;
        float minY = globalPosition.y - 0.5f * m_touchArea.y;
        float maxY = globalPosition.y + 0.5f * m_touchArea.y;

        return (point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY);
    }

    public override void OnClick()
    {
        GameScene gameScene = (GameScene)GetSceneManager().m_currentScene;

        if (m_ID == GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE || m_ID == GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES)
        {
            Axis axis = new Axis();
            axis.m_pointA = m_parentMenu.m_relatedAnchor.m_gridPosition;
            axis.m_state = Axis.AxisState.UNDER_CONSTRUCTION;
            gameScene.m_axesHolder.BuildAxisRenderer(axis);
            axis.m_parentRenderer.InitializeRendering();
            axis.m_type = (m_ID == GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE) ? Axis.AxisType.SYMMETRY_AXES_ONE_SIDE : Axis.AxisType.SYMMETRY_AXES_TWO_SIDES;
            
            gameScene.DisplayAvailableAnchorsForAxis(axis.m_pointA);
            gameScene.m_currentAction = GameScene.Action.DRAWING_AXIS;
        }
        else if (m_ID == GUIButtonID.ID_POINT_SYMMETRY)
        {
            Debug.Log("Build point symmetry");
            gameScene.m_currentAction = GameScene.Action.DRAWING_POINT_SYMMETRY;
        }

        else if (m_ID == GUIButtonID.ID_CANCEL_ACTION_MENU)
        {
            gameScene.m_currentAction = GameScene.Action.NONE;

            //Destroy the overlay
        }

        gameScene.DismissActionMenu();
    }
}
