using UnityEngine;

public class ActionButton : GUIButton
{
    public const float ACTION_BUTTON_Z_VALUE = -10.0f;

    //shared prefabs
    public GameObject m_colorQuadPfb;
    public GameObject m_circleMeshPfb;
    public GameObject m_textMeshPfb;

    protected GameObject m_background;
    protected UVQuad m_shadow;
    protected GameObject m_buttonNameObject;

    //variables to handle the update of the button name
    private bool m_buttonNameUpdatePending;
    private float m_buttonNameUpdateElapsedTime;
    private float m_buttonNameUpdateDuration;

    public enum Location
    {
        TOP,
        MIDDLE,
        BOTTOM
    }
    private Location m_location;

    public void Init(Material skinMaterial, Color tintColor, Location location)
    {
        base.Init(skinMaterial, tintColor);
        m_location = location;

        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //Set the default button ID
        SetDefaultButtonID();

        GameObjectAnimator buttonAnimator = this.GetComponent<GameObjectAnimator>();
        buttonAnimator.SetPosition(new Vector3(-100.0f - 0.5f * screenSize.x, GetYPositionForLocation(location), ACTION_BUTTON_Z_VALUE));

        Material plainWhiteMaterial = GetGUIManager().m_plainWhiteMaterial;

        //Build the background
        m_background = new GameObject("Background");
        m_background.transform.parent = this.transform;

        GameObjectAnimator backgroundAnimator = m_background.AddComponent<GameObjectAnimator>();
        backgroundAnimator.SetPosition(Vector3.zero);

        //diamond
        float diamondSize = 150;
        float diamondThickness = 10.0f;

        GameObject diamondMeshObject = (GameObject)Instantiate(m_circleMeshPfb);
        diamondMeshObject.transform.parent = m_background.transform;
        
        CircleMesh diamondMesh = diamondMeshObject.GetComponent<CircleMesh>();
        diamondMesh.Init(plainWhiteMaterial);

        CircleMeshAnimator diamondMeshAnimator = diamondMeshObject.GetComponent<CircleMeshAnimator>();
        diamondMeshAnimator.SetNumSegments(4, false); //diamond = 4 edges        
        diamondMeshAnimator.SetOuterRadius(0.5f * diamondSize, false);
        diamondMeshAnimator.SetInnerRadius(0.5f * diamondSize - diamondThickness, true); //thickness: 14
        diamondMeshAnimator.SetPosition(Vector3.zero);
        diamondMeshAnimator.SetColor(tintColor);

        //segment
        GameObject segmentObject = (GameObject)Instantiate(m_colorQuadPfb);
        segmentObject.transform.parent = m_background.transform;

        ColorQuad segmentQuad = segmentObject.GetComponent<ColorQuad>();
        segmentQuad.Init(plainWhiteMaterial);

        ColorQuadAnimator segmentQuadAnimator = segmentObject.GetComponent<ColorQuadAnimator>();
        segmentQuadAnimator.UpdatePivotPoint(new Vector3(1.0f, 0.5f, 0.5f));
        segmentQuadAnimator.SetPosition(new Vector3(-0.5f * (diamondSize - diamondThickness), 0, 0));
        segmentQuadAnimator.SetScale(new Vector3(100.0f, 7.0f, 1)); //set the segment long enough
        segmentQuadAnimator.SetColor(tintColor);

        //TODO build the shadow
    }

    /**
     * Set the default button ID when level starts
     * **/
    private void SetDefaultButtonID()
    {
        if (m_location == Location.TOP)
            m_ID = GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES;
        else if (m_location == Location.MIDDLE)
            m_ID = GUIButtonID.ID_OPERATION_ADD;
        else
            m_ID = GUIButtonID.ID_COLOR_FILTER;
    }

    /**
     * -Rotate the background object
     * -Fade in the shadow
     * -Fade in the skin
     * -translate the whole button from left to right
     * **/
    public void Show()
    {
        Vector2 screenSize = ScreenUtils.GetScreenSize();

        //animate the background
        //float rotationDuration = 0.5f;
        //GameObjectAnimator backgroundAnimator = m_background.GetComponent<GameObjectAnimator>();
        //backgroundAnimator.SetRotationAxis(Vector3.right);
        //backgroundAnimator.SetRotationAngle(90);
        //backgroundAnimator.RotateTo(0, rotationDuration);

        //translate button
        GameObjectAnimator buttonAnimator = this.GetComponent<GameObjectAnimator>();
        float buttonPositionY = buttonAnimator.GetPosition().y;
        Vector3 buttonToPosition = new Vector3(100.0f - 0.5f * screenSize.x, buttonPositionY, ACTION_BUTTON_Z_VALUE);
        buttonAnimator.TranslateTo(buttonToPosition, 0.5f, 0.0f, ValueAnimator.InterpolationType.SINUSOIDAL);

        //Show the name label
        ShowButtonName();
    }

    /**
     * Show the button name object when button first appear
     * **/
    public void ShowButtonName()
    {
        m_buttonNameObject = (GameObject)Instantiate(m_textMeshPfb);
        m_buttonNameObject.name = "Name";
        m_buttonNameObject.transform.parent = this.transform;

        TextMesh nameTextMesh = m_buttonNameObject.GetComponent<TextMesh>();
        nameTextMesh.text = GetButtonNameForID(m_ID);

        Vector3 nameTextMeshPosition = new Vector3(0, -115, 0);
        TextMeshAnimator nameAnimator = m_buttonNameObject.GetComponent<TextMeshAnimator>();
        nameAnimator.SetFontHeight(24);
        nameAnimator.SetColor(Color.white);
        nameAnimator.SetPosition(nameTextMeshPosition - new Vector3(0.0f, 50.0f, 0.0f));
        nameAnimator.TranslateTo(nameTextMeshPosition, 0.5f, 0.5f);
    }

    ///**
    // * -Rotate the background object
    // * -Fade out the shadow
    // * -Fade out the skin
    // * -translate the whole button from right to left
    // * **/
    //public void Dismiss()
    //{

    //}

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
     * Specify the y position for this specific action button
     * **/
    public float GetYPositionForLocation(Location location)
    {
        float verticalSpacesBetweenButtons = 250.0f;

        if (location == Location.TOP)
            return verticalSpacesBetweenButtons;
        else if (location == Location.BOTTOM)
            return -verticalSpacesBetweenButtons;
        else 
            return 0;
    }

    /**
     * Get the name of the displayed button ID
     * **/
    public string GetButtonNameForID(GUIButtonID iID)
    {
        switch (iID)
        {
            case GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES:
                return LanguageUtils.GetTranslationForTag("symmetry_two_sides");
            case GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE:
                return LanguageUtils.GetTranslationForTag("symmetry_one_side");
            case GUIButtonID.ID_POINT_SYMMETRY:
                return LanguageUtils.GetTranslationForTag("symmetry_one_side");
            case GUIButtonID.ID_MOVE_SHAPE:
                return LanguageUtils.GetTranslationForTag("symmetry_one_side");
            case GUIButtonID.ID_OPERATION_ADD:
                return LanguageUtils.GetTranslationForTag("symmetry_one_side");
            case GUIButtonID.ID_OPERATION_SUBSTRACT:
                return LanguageUtils.GetTranslationForTag("symmetry_one_side");
            case GUIButtonID.ID_COLOR_FILTER:
                return LanguageUtils.GetTranslationForTag("symmetry_one_side");
            default:
                return null;
        }
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
                buttonNameTextMesh.text = GetButtonNameForID(m_ID);

                TextMeshAnimator buttonNameAnimator = m_buttonNameObject.GetComponent<TextMeshAnimator>();
                buttonNameAnimator.FadeTo(1.0f, 0.25f);
            }
            else
                m_buttonNameUpdateElapsedTime += dt;
        }
    }
}
