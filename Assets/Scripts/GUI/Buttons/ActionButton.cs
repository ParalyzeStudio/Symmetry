using UnityEngine;
using System.Collections.Generic;

public class ActionButton : GUIButton
{
    //shared prefabs
    public GameObject m_colorQuadPfb;
    public GameObject m_texQuadPfb;
    public GameObject m_textMeshPfb;
    public Material m_transpColorMaterial;

    private List<GameObject> m_childButtons;
    private Vector2 m_childButtonSize;

    public enum GroupID
    {
        MAIN_ACTIONS,
        CLIP_OPERATION,
        COLOR_FILTERING
    }
    public GroupID m_groupID { get; set; }

    //the list of IDs this button is cycling on
    public GUIButton.GUIButtonID[] m_childIDs { get; set; }
    public int m_currentChildIndex { get; set; }
    public ColorQuadAnimator m_selectedChildOverlayAnimator;

    /**
     * Build an action buttons with a title and one or more sub elements
     * The position is given by the position of the title
     * **/
    public void Init(GroupID groupID, GUIButton.GUIButtonID[] childIDs, float width)
    {
        m_groupID = groupID;
        m_childIDs = childIDs;

        Vector2 screenSize = ScreenUtils.GetScreenSize();
        m_childButtonSize = new Vector2(width, 90.0f);

        //Build group title
        GameObject titleObject = (GameObject)Instantiate(m_textMeshPfb);
        titleObject.name = "Title";

        TextMesh titleMesh = titleObject.GetComponent<TextMesh>();
        titleMesh.text = GetTitleForID(groupID);
        titleMesh.text = "Actions";

        TextMeshAnimator titleAnimator = titleObject.GetComponent<TextMeshAnimator>();
        titleAnimator.SetParentTransform(this.transform);
        titleAnimator.SetFontHeight(20);
        titleAnimator.SetColor(Color.white);
        titleAnimator.SetPosition(Vector3.zero);

        float titleHeight = titleObject.GetComponent<MeshRenderer>().bounds.size.y;

        float titleBottomMargin = 10.0f;

        //Build sub elements
        m_childButtons = new List<GameObject>(childIDs.Length);
        for (int i = 0; i != childIDs.Length; i++)
        {
            GameObject subElement = BuildSubElement(childIDs[i], m_childButtonSize.x);
            
            GameObjectAnimator elementAnimator = subElement.GetComponent<GameObjectAnimator>();
            elementAnimator.SetParentTransform(this.transform);
            Vector3 subElementPosition = new Vector3(0, -(i + 0.5f) * m_childButtonSize.y - 0.5f * titleHeight - titleBottomMargin, 0);
            elementAnimator.SetPosition(subElementPosition);
        }

        //Add separation lines
        Material lineMaterial = Instantiate(m_transpColorMaterial);
        float lineThickness = 2.0f;
        for (int i = 0; i != childIDs.Length; i++)
        {
            Vector3 elementPosition = m_childButtons[i].transform.localPosition;
            Vector3 lineSize = new Vector3(m_childButtonSize.x, lineThickness, 1);
            Vector3 linePosition = elementPosition - new Vector3(0, 0.5f * m_childButtonSize.y, 0);

            BuildSeparationLine(lineSize, linePosition, lineMaterial);

            if (i == 0)
            {
                linePosition = elementPosition + new Vector3(0, 0.5f * m_childButtonSize.y, 0);
                BuildSeparationLine(lineSize, linePosition, lineMaterial);
            }
        }

        //set the first ID as default
        UpdateSelectedChildID(0, false);
    }

    /**
     * Build one element containing an icon and the corresponding text
     * **/
    private GameObject BuildSubElement(GUIButton.GUIButtonID elementID, float width)
    {
        Vector3 buttonIconSize = new Vector3(64, 64, 1);

        //holder
        GameObject childButtonObject = new GameObject("ChildButton");
        m_childButtons.Add(childButtonObject);
        childButtonObject.AddComponent<GameObjectAnimator>();

        //icon
        GameObject iconObject = (GameObject)Instantiate(m_texQuadPfb);
        iconObject.name = "Icon";

        UVQuad icon = iconObject.GetComponent<UVQuad>();
        Material iconMaterial = GetGUIManager().GetClonedSkinMaterialForID(elementID);
        icon.Init(iconMaterial);

        TexturedQuadAnimator iconAnimator = iconObject.GetComponent<TexturedQuadAnimator>();
        iconAnimator.SetParentTransform(childButtonObject.transform);
        iconAnimator.SetColor(Color.white);
        iconAnimator.SetScale(buttonIconSize);
        iconAnimator.SetPosition(new Vector3(-0.27f * width, 0, 0));

        //text
        GameObject textObject = (GameObject)Instantiate(m_textMeshPfb);
        textObject.name = "Text";

        TextMesh text = textObject.GetComponent<TextMesh>();
        text.text = "draw axis";

        TextMeshAnimator textAnimator = textObject.GetComponent<TextMeshAnimator>();
        textAnimator.SetParentTransform(childButtonObject.transform);
        textAnimator.SetColor(Color.white);
        textAnimator.SetFontHeight(20);
        textAnimator.SetPosition(new Vector3(0.15f * width, 0, 0));

        return childButtonObject;
    }

    private void BuildSeparationLine(Vector3 size, Vector3 position, Material lineMaterial)
    {
        GameObject lineObject = (GameObject)Instantiate(m_colorQuadPfb);
        lineObject.name = "Line";

        ColorQuad line = lineObject.GetComponent<ColorQuad>();
        line.Init(lineMaterial);

        ColorQuadAnimator lineAnimator = lineObject.GetComponent<ColorQuadAnimator>();
        lineAnimator.SetParentTransform(this.transform);
        lineAnimator.SetColor(Color.white);
        lineAnimator.SetScale(size);
        lineAnimator.SetPosition(position);
    }

    private string GetTitleForID(GroupID id)
    {
        switch (id)
        {
            case GroupID.MAIN_ACTIONS:
                return LanguageUtils.GetTranslationForTag("main_actions");
            case GroupID.CLIP_OPERATION:
                return LanguageUtils.GetTranslationForTag("clip_operations");
            case GroupID.COLOR_FILTERING:
                return LanguageUtils.GetTranslationForTag("color_filtering");
            default:
                return "no title";
        }
    }

    /**
     * Return the button ID of the currently selected child
     * **/
    public GUIButtonID GetSelectedChildID()
    {
        return m_childIDs[m_currentChildIndex];
    }

    /**
     * Display a white overlay over the currently selected child item
     * **/
    private void UpdateSelectedChildID(int newChildIndex, bool bAnimated = false)
    {
        m_currentChildIndex = newChildIndex;

        if (m_selectedChildOverlayAnimator == null)
            BuildSelectedChildOverlay();

        Vector3 overlayFinalPosition = m_childButtons[newChildIndex].transform.localPosition;
        overlayFinalPosition.z = -1; //set the overlay above all other elements
        if (bAnimated)
            m_selectedChildOverlayAnimator.TranslateTo(overlayFinalPosition, 0.3f);
        else
            m_selectedChildOverlayAnimator.SetPosition(overlayFinalPosition);
    }

    /**
     * Build a rectangle that will be rendered over the selected child button
     * **/
    private void BuildSelectedChildOverlay()
    {
        GameObject childOverlayObject = (GameObject)Instantiate(m_colorQuadPfb);
        childOverlayObject.name = "Overlay";

        ColorQuad childOverlay = childOverlayObject.GetComponent<ColorQuad>();
        childOverlay.Init(Instantiate(m_transpColorMaterial));

        m_selectedChildOverlayAnimator = childOverlayObject.GetComponent<ColorQuadAnimator>();
        m_selectedChildOverlayAnimator.SetParentTransform(this.transform);
        m_selectedChildOverlayAnimator.SetScale(new Vector3(m_childButtonSize.x, m_childButtonSize.y, 1));
        m_selectedChildOverlayAnimator.SetColor(new Color(1, 1, 1, 0.25f));
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
        float actionButtonHeight = m_childButtons.Count * m_childButtonSize.y + 30.0f; //title (around 30 units)
        Vector2 buttonTouchAreaSize = new Vector2(0.83f * m_childButtonSize.x, actionButtonHeight);
        Vector2 buttonTouchAreaPosition = GeometryUtils.BuildVector2FromVector3(this.transform.position) - new Vector2(0, 0.5f * actionButtonHeight);

        float minX = buttonTouchAreaPosition.x - 0.5f * m_childButtonSize.x;
        float maxX = buttonTouchAreaPosition.x + 0.33f * m_childButtonSize.x;
        float minY = buttonTouchAreaPosition.y - 0.5f * buttonTouchAreaSize.y;
        float maxY = buttonTouchAreaPosition.y + 0.5f * buttonTouchAreaSize.y;

        return (point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY);
    }

    public override void OnClick()
    {
        int newChildIndex = (m_currentChildIndex == m_childIDs.Length - 1) ? 0 : m_currentChildIndex + 1;
        UpdateSelectedChildID(newChildIndex, true);
    }
}
