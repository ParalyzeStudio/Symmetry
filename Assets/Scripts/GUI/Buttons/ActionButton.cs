using UnityEngine;
using System.Collections.Generic;

public class ActionButton : GUIButton
{
    //shared prefabs
    public GameObject m_colorQuadPfb;
    public GameObject m_texQuadPfb;
    public GameObject m_textMeshPfb;
    public Material m_transpColorMaterial;

    //protected GameObject m_background;
    protected UVQuad m_background;
    protected GameObject m_buttonNameObject;

    //variables to handle the update of the button name
    private bool m_buttonNameUpdatePending;
    private float m_buttonNameUpdateElapsedTime;
    private float m_buttonNameUpdateDuration;

    private List<GameObject> m_childButtons;

    public enum GroupID
    {
        MAIN_ACTIONS,
        CLIP_OPERATION,
        COLOR_FILTERING
    }
    public GroupID m_groupID { get; set; }

    //the list of IDs this button is cycling on
    public GUIButton.GUIButtonID[] m_childIDs { get; set; }
    public GUIButton.GUIButtonID m_currentChildID { get; set; }
    public int m_currentChildIndex { get; set; }

    /**
     * Build an action buttons with a title and one or more sub elements
     * The position is given by the position of the title
     * **/
    public void Init(GroupID groupID, GUIButton.GUIButtonID[] childIDs)
    {
        m_groupID = groupID;
        m_childIDs = childIDs;

        Vector2 screenSize = ScreenUtils.GetScreenSize();

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
        Debug.Log("titleHeight:" + titleHeight);

        float titleBottomMargin = 10.0f;

        //Build sub elements
        m_childButtons = new List<GameObject>(childIDs.Length);
        float subElementWidth = 242.0f;
        float subElementHeight = 80.0f;
        for (int i = 0; i != childIDs.Length; i++)
        {
            GameObject subElement = BuildSubElement(childIDs[i], subElementWidth);
            
            GameObjectAnimator elementAnimator = subElement.GetComponent<GameObjectAnimator>();
            elementAnimator.SetParentTransform(this.transform);
            Vector3 subElementPosition = new Vector3(0, -(i + 0.5f) * subElementHeight - 0.5f * titleHeight - titleBottomMargin, 0);
            elementAnimator.SetPosition(subElementPosition);
        }

        //Add separation lines
        Material lineMaterial = Instantiate(m_transpColorMaterial);
        float lineThickness = 2.0f;
        for (int i = 0; i != childIDs.Length; i++)
        {
            Vector3 elementPosition = m_childButtons[i].transform.localPosition;
            Vector3 lineSize = new Vector3(subElementWidth, lineThickness, 1);
            Vector3 linePosition = elementPosition - new Vector3(0, 0.5f * subElementHeight, 0);

            BuildSeparationLine(lineSize, linePosition, lineMaterial);

            if (i == 0)
            {
                linePosition = elementPosition + new Vector3(0, 0.5f * subElementHeight, 0);
                BuildSeparationLine(lineSize, linePosition, lineMaterial);
            }
        }

        ////set the first ID as default
        //m_ID = m_childIDs[0];
        //Material defaultSkinMaterial = GetGUIManager().GetClonedSkinMaterialForID(m_ID);

        //base.Init(defaultSkinMaterial);
        //TexturedQuadAnimator skinAnimator = m_skin.GetComponent<TexturedQuadAnimator>();
        //skinAnimator.SetPosition(new Vector3(-18, 0, -2));
        //skinAnimator.SetColor(Color.white);

        //GameObjectAnimator buttonAnimator = this.GetComponent<GameObjectAnimator>();
        //buttonAnimator.SetPosition(new Vector3(-128.0f - 0.5f * screenSize.x, GetYPositionForLocation(location), ACTION_BUTTON_Z_VALUE));

        ////Build the background
        //GameObject buttonBackgroundObject = Instantiate(m_texQuadPfb);
        //buttonBackgroundObject.name = "Background";

        //m_background = buttonBackgroundObject.GetComponent<UVQuad>();
        //m_background.Init(m_actionButtonFrameMaterial);

        //LevelManager levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

        //TexturedQuadAnimator backgroundAnimator = m_background.GetComponent<TexturedQuadAnimator>();
        //backgroundAnimator.SetParentTransform(this.transform);
        //backgroundAnimator.SetScale(new Vector2(256, 256));
        //backgroundAnimator.SetColorChannels(levelManager.m_currentChapter.GetThemeTintValues()[0], ValueAnimator.ColorMode.TSB);
        //backgroundAnimator.SetPosition(new Vector3(0, 0, -1));
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
     * Display this action button on the scene
     * **/
    public void Show(float fDelay = 0.0f)
    {
        
    }

    /**
     * Show the button name object when button first appear
     * **/
    public void ShowButtonName()
    {
        m_buttonNameObject = (GameObject)Instantiate(m_textMeshPfb);
        m_buttonNameObject.name = "Name";

        TextMesh nameTextMesh = m_buttonNameObject.GetComponent<TextMesh>();
        nameTextMesh.text = GetButtonName();

        Vector3 nameTextMeshPosition = new Vector3(0, -115, 0);
        TextMeshAnimator nameAnimator = m_buttonNameObject.GetComponent<TextMeshAnimator>();
        nameAnimator.SetParentTransform(this.transform);
        nameAnimator.SetFontHeight(24);
        nameAnimator.SetColor(Color.white);
        nameAnimator.SetPosition(nameTextMeshPosition - new Vector3(0.0f, 10.0f, 0.0f));
        nameAnimator.TranslateTo(nameTextMeshPosition, 0.3f, 0.5f);
        nameAnimator.SetOpacity(0);
        nameAnimator.FadeTo(1.0f, 0.3f, 0.5f);
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
     * Switch to next button child ID if possible
     * **/
    public void UpdateButtonToNextID()
    {
        if (m_childIDs.Length > 1)
        {
            if (m_currentChildIndex == m_childIDs.Length - 1)
                m_currentChildIndex = 0;
            else
                m_currentChildIndex++;

            UpdateButtonID(m_childIDs[m_currentChildIndex]);
        }
    }

    /**
     * Update the ID of this button
     * **/
    public void UpdateButtonID(GUIButton.GUIButtonID iID)
    {
        m_ID = iID;
        UpdateSkinForCurrentButtonID(true, 0.5f, 0.5f);
        UpdateButtonName();
    }

    /**
     * Update the skin for the currently active ID
     * **/
    private void UpdateSkinForCurrentButtonID(bool bAnimated, float fDuration, float fDelay = 0.0f)
    {        
        SetSkinMaterial(GetGUIManager().GetClonedSkinMaterialForID(m_ID));

        if (bAnimated)
        {
            TexturedQuadAnimator skinAnimator = m_skin.GetComponent<TexturedQuadAnimator>();
            skinAnimator.SetOpacity(0);
            skinAnimator.FadeTo(1.0f, fDuration, fDelay);
        }
    }

    /**
     * Update the text inside the button name object by fading out the current one and fading in the new one
     * **/
    private void UpdateButtonName()
    {
        m_buttonNameUpdatePending = true;
        m_buttonNameUpdateElapsedTime = 0;
        m_buttonNameUpdateDuration = 0.75f;

        TextMeshAnimator buttonNameAnimator = m_buttonNameObject.GetComponent<TextMeshAnimator>();
        buttonNameAnimator.FadeTo(0.0f, 0.25f, 0.0f);
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

    public override void OnClick()
    {
        UpdateButtonToNextID();
    }

    /**
     * Update loop
     * **/
    public void Update()
    {
        float dt = Time.deltaTime;

        if (m_buttonNameUpdatePending)
        {            
            if (m_buttonNameUpdateElapsedTime > m_buttonNameUpdateDuration)
            {
                m_buttonNameUpdatePending = false;

                TextMesh buttonNameTextMesh = m_buttonNameObject.GetComponent<TextMesh>();
                buttonNameTextMesh.text = GetButtonName();

                TextMeshAnimator buttonNameAnimator = m_buttonNameObject.GetComponent<TextMeshAnimator>();
                buttonNameAnimator.FadeTo(1.0f, 0.25f);
            }
            else
                m_buttonNameUpdateElapsedTime += dt;
        }
    }
}
