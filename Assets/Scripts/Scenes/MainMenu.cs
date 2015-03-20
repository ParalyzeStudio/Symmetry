using UnityEngine;

public class MainMenu : GUIScene
{
    public GameObject m_segmentPfb;
    public GameObject m_titlePartPfb;
    public Material[] m_titlePartsMaterials;

    /**
     * Shows MainMenu with or without animation
     * **/
    public override void Show(bool bAnimated, float fDelay = 0.0f)
    {
        base.Show(bAnimated, fDelay);
        GameObjectAnimator menuAnimator = this.GetComponent<GameObjectAnimator>();
        //menuAnimator.OnOpacityChanged(1);
        menuAnimator.SetOpacity(1);
        ShowTitle(bAnimated, fDelay);
        ShowAxes(bAnimated, 1.5f + fDelay);
        ShowButtons(bAnimated, 2.0f + fDelay);
        ShowTapToPlay(bAnimated ? 2.0f + fDelay : 0);

        if (!bAnimated)
        {
            //menuAnimator.OnOpacityChanged(0);
            //menuAnimator.FadeFromTo(0, 1, 0.5f, 1.0f);
            menuAnimator.SetOpacity(0);
            menuAnimator.FadeTo(1, 0.5f, 1.0f);
            GameObject tapToPlayObject = GameObject.FindGameObjectWithTag("TapToPlay");
            TextMeshAnimator tapToPlayAnimator = tapToPlayObject.GetComponent<TextMeshAnimator>();
            tapToPlayAnimator.SetCyclingPaused(true);
            ShowTapToPlay(1.0f);
        }
    }

    public override void Dismiss(float fDuration, float fDelay = 0.0f)
    {
        TextMeshAnimator tapToPlayAnimator = GameObject.FindGameObjectWithTag("TapToPlay").GetComponent<TextMeshAnimator>();
        tapToPlayAnimator.SetCyclingPaused(true);

        base.Dismiss(fDuration, fDelay);
    }

    public override void OnSceneDismissed()
    {
        base.OnSceneDismissed();
    }

    public void ShowTitle(bool bAnimated, float fDelay = 0.0f)
    {
        GameObject titleObject = GameObject.FindGameObjectWithTag("Title");

        float titlePartWidth = 512.0f;
        float titlePartHeight = 128.0f;

        Vector3 topLeftPartLocalPosition = new Vector3(-283, 0, 0);
        Vector3 topRightPartLocalPosition = new Vector3(284, 0, 0);
        Vector3 bottomRightPartLocalPosition = new Vector3(256, 0, 0);
        Vector3 bottomLeftPartLocalPosition = new Vector3(-256, 0, 0);

        GameObject clonedTopLeft = (GameObject) Instantiate(m_titlePartPfb);
        GameObject clonedTopRight = (GameObject)Instantiate(m_titlePartPfb);
        GameObject clonedBottomRight = (GameObject)Instantiate(m_titlePartPfb);
        GameObject clonedBottomLeft = (GameObject)Instantiate(m_titlePartPfb);

        //Assign titles holder as their parent
        clonedTopLeft.transform.parent = titleObject.transform;
        clonedTopRight.transform.parent = titleObject.transform;
        clonedBottomRight.transform.parent = titleObject.transform;
        clonedBottomLeft.transform.parent = titleObject.transform;

        //Assign correct material for each part
        clonedTopLeft.GetComponent<MeshRenderer>().material = m_titlePartsMaterials[0];
        clonedTopRight.GetComponent<MeshRenderer>().material = m_titlePartsMaterials[1];
        clonedBottomRight.GetComponent<MeshRenderer>().material = m_titlePartsMaterials[2];
        clonedBottomLeft.GetComponent<MeshRenderer>().material = m_titlePartsMaterials[3];

        //set position for each part
        FixedTextureSizeAnimator topLeftAnimator = clonedTopLeft.GetComponent<FixedTextureSizeAnimator>();
        topLeftAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        //topLeftAnimator.MoveObjectBySettingPivotPointPosition(bAnimated ? topLeftPartLocalPosition : topLeftPartLocalPosition  + new Vector3(0, 128, 0));
        topLeftAnimator.SetPosition(bAnimated ? topLeftPartLocalPosition : topLeftPartLocalPosition + new Vector3(0, 128, 0));
        topLeftAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);

        FixedTextureSizeAnimator topRightAnimator = clonedTopRight.GetComponent<FixedTextureSizeAnimator>();
        topRightAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        //topRightAnimator.MoveObjectBySettingPivotPointPosition(bAnimated ? topRightPartLocalPosition : topRightPartLocalPosition + new Vector3(0, 128, 0));
        topRightAnimator.SetPosition(bAnimated ? topRightPartLocalPosition : topRightPartLocalPosition + new Vector3(0, 128, 0));
        topRightAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);

        FixedTextureSizeAnimator bottomRightAnimator = clonedBottomRight.GetComponent<FixedTextureSizeAnimator>();
        bottomRightAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.0f, 0.5f));
        //bottomRightAnimator.MoveObjectBySettingPivotPointPosition(bAnimated ? bottomRightPartLocalPosition : bottomRightPartLocalPosition - new Vector3(0, 128, 0));
        bottomRightAnimator.SetPosition(bAnimated ? bottomRightPartLocalPosition : bottomRightPartLocalPosition - new Vector3(0, 128, 0));
        bottomRightAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);

        FixedTextureSizeAnimator bottomLeftAnimator = clonedBottomLeft.GetComponent<FixedTextureSizeAnimator>();
        bottomLeftAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.0f, 0.5f));
        //bottomLeftAnimator.MoveObjectBySettingPivotPointPosition(bAnimated ? bottomLeftPartLocalPosition : bottomLeftPartLocalPosition - new Vector3(0, 128, 0));
        bottomLeftAnimator.SetPosition(bAnimated ? bottomLeftPartLocalPosition : bottomLeftPartLocalPosition - new Vector3(0, 128, 0));
        bottomLeftAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);

        if (bAnimated)
        {
            //Set zero scale so they are invisible at start
            //clonedTopLeft.transform.localScale = Vector3.zero;
            //clonedTopRight.transform.localScale = Vector3.zero;
            //clonedBottomRight.transform.localScale = Vector3.zero;
            //clonedBottomLeft.transform.localScale = Vector3.zero;

            //Animate top left part
            //topLeftAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.5f);
            //topLeftAnimator.TranslateFromTo(topLeftPartLocalPosition, topLeftPartLocalPosition + new Vector3(0, 128, 0), 0.5f, fDelay + 1.5f);
            topLeftAnimator.SetScale(new Vector3(titlePartWidth, 0, 1.0f));
            topLeftAnimator.SetPosition(topLeftPartLocalPosition);
            topLeftAnimator.ScaleTo(new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.5f);
            topLeftAnimator.TranslateTo(topLeftPartLocalPosition + new Vector3(0, 128, 0), 0.5f, fDelay + 1.5f);

            //Animate top right part            
            //topRightAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.8f);
            //topRightAnimator.TranslateFromTo(topRightPartLocalPosition, topRightPartLocalPosition + new Vector3(0, 128, 0), 0.5f, fDelay + 1.8f);
            topRightAnimator.SetScale(new Vector3(titlePartWidth, 0, 1.0f));
            topRightAnimator.SetPosition(topRightPartLocalPosition);
            topRightAnimator.ScaleTo(new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.8f);
            topRightAnimator.TranslateTo(topRightPartLocalPosition + new Vector3(0, 128, 0), 0.5f, fDelay + 1.8f);

            //Animate bottom right part
            //bottomRightAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.5f);
            //bottomRightAnimator.TranslateFromTo(bottomRightPartLocalPosition, bottomRightPartLocalPosition - new Vector3(0, 128, 0), 0.5f, fDelay + 1.5f);
            bottomRightAnimator.SetScale(new Vector3(titlePartWidth, 0, 1.0f));
            bottomRightAnimator.SetPosition(bottomRightPartLocalPosition);
            bottomRightAnimator.ScaleTo(new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.5f);
            bottomRightAnimator.TranslateTo(bottomRightPartLocalPosition - new Vector3(0, 128, 0), 0.5f, fDelay + 1.5f);

            //Animate bottom left part
            //bottomLeftAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.8f);
            //bottomLeftAnimator.TranslateFromTo(bottomLeftPartLocalPosition, bottomLeftPartLocalPosition - new Vector3(0, 128, 0), 0.5f, fDelay + 1.8f);
            bottomLeftAnimator.SetScale(new Vector3(titlePartWidth, 0, 1.0f));
            bottomLeftAnimator.SetPosition(bottomLeftPartLocalPosition);
            bottomLeftAnimator.ScaleTo(new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, fDelay + 1.8f);
            bottomLeftAnimator.TranslateTo(bottomLeftPartLocalPosition - new Vector3(0, 128, 0), 0.5f, fDelay + 1.8f);
        }
    }

    public void ShowAxes(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        GameObject axesHolder = GameObject.FindGameObjectWithTag("MainMenuAxes");
        GameObject horizontalAxisObject = (GameObject)Instantiate(m_segmentPfb);
        GameObject verticalAxisObject = (GameObject)Instantiate(m_segmentPfb);
        Segment horizontalAxis = horizontalAxisObject.GetComponent<Segment>();
        Segment verticalAxis = verticalAxisObject.GetComponent<Segment>();
        SegmentAnimator horizontalAxisAnimator = horizontalAxisObject.GetComponent<SegmentAnimator>();
        SegmentAnimator verticalAxisAnimator = verticalAxisObject.GetComponent<SegmentAnimator>();

        horizontalAxisAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f));
        verticalAxisAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f));

        //add axes to parent holder and set correct local positions
        horizontalAxis.transform.parent = axesHolder.transform;
        verticalAxis.transform.parent = axesHolder.transform;
        horizontalAxis.transform.localPosition = Vector3.zero;
        verticalAxis.transform.localPosition = Vector3.zero;

        //set thickness on axes
        horizontalAxis.m_thickness = 4;
        float distanceToScreenTopBorder = 0.5f * screenSize.y - axesHolder.transform.position.y;
        verticalAxis.m_thickness = 4;

        //set correct angles
        horizontalAxis.SetAngle(0);
        verticalAxis.SetAngle(90);

        if (bAnimated)
        {
            horizontalAxisAnimator.ResizeTo(screenSize.x, 1.0f, fDelay);
            verticalAxisAnimator.ResizeTo(2 * distanceToScreenTopBorder, 1.0f, fDelay);
        }
        else
        {
            horizontalAxis.SetLength(screenSize.x);
            verticalAxis.SetLength(2 * distanceToScreenTopBorder);
        }
    }

    public void ShowButtons(bool bAnimated, float fDelay = 0.0f)
    {
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        GameObject optionsPanel = GameObject.FindGameObjectWithTag("OptionsPanel");
        GameObject creditsPanel = GameObject.FindGameObjectWithTag("CreditsPanel");
        GameObjectAnimator optionsPanelAnimator = optionsPanel.GetComponent<GameObjectAnimator>();
        GameObjectAnimator creditsPanelAnimator = creditsPanel.GetComponent<GameObjectAnimator>();

        Vector3 optionsPanelFinalPosition = new Vector3(optionsPanel.transform.position.x, -0.5f * screenSize.y + 175.0f, optionsPanel.transform.position.z);
        Vector3 creditsPanelFinalPosition = new Vector3(creditsPanel.transform.position.x, -0.5f * screenSize.y + 175.0f, creditsPanel.transform.position.z);

        if (bAnimated)
        {
            Vector3 optionsPanelFromPosition = new Vector3(optionsPanel.transform.position.x, -0.5f * screenSize.y - 175.0f, optionsPanel.transform.position.z);
            Vector3 creditsPanelFromPosition = new Vector3(creditsPanel.transform.position.x, -0.5f * screenSize.y - 175.0f, creditsPanel.transform.position.z);

            //optionsPanelAnimator.MoveObjectBySettingPivotPointPosition(optionsPanelFromPosition);
            //creditsPanelAnimator.MoveObjectBySettingPivotPointPosition(creditsPanelFromPosition);
            //optionsPanelAnimator.TranslateFromTo(optionsPanelFromPosition, optionsPanelFinalPosition, 0.5f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
            //creditsPanelAnimator.TranslateFromTo(creditsPanelFromPosition, creditsPanelFinalPosition, 0.5f, fDelay + 0.15f, ValueAnimator.InterpolationType.SINUSOIDAL);
            optionsPanelAnimator.SetPosition(optionsPanelFromPosition);
            creditsPanelAnimator.SetPosition(creditsPanelFromPosition);
            optionsPanelAnimator.TranslateTo(optionsPanelFinalPosition, 0.5f, fDelay, ValueAnimator.InterpolationType.SINUSOIDAL);
            creditsPanelAnimator.TranslateTo(creditsPanelFinalPosition, 0.5f, fDelay + 0.15f, ValueAnimator.InterpolationType.SINUSOIDAL);
        }
        else
        {
            //optionsPanelAnimator.MoveObjectBySettingPivotPointPosition(optionsPanelFinalPosition);
            //creditsPanelAnimator.MoveObjectBySettingPivotPointPosition(creditsPanelFinalPosition);
            optionsPanelAnimator.SetPosition(optionsPanelFinalPosition);
            creditsPanelAnimator.SetPosition(creditsPanelFinalPosition);
        }
    }

    public void ShowTapToPlay(float fDelay = 0.0f)
    {
        GameObject tapToPlayObject = GameObject.FindGameObjectWithTag("TapToPlay");
        TextMeshAnimator tapToPlayAnimator = tapToPlayObject.GetComponent<TextMeshAnimator>();
        //tapToPlayAnimator.OnOpacityChanged(0);
        tapToPlayAnimator.SetOpacity(0);
        tapToPlayAnimator.SetTextMeshOpacityCycling(2.0f, true, fDelay);
    }
}

