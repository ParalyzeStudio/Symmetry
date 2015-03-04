using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject m_segmentPfb;
    public GameObject m_GUIFramePfb;
    public GameObject m_titlePartPfb;
    public Material[] m_framesMaterials;
    public Material[] m_titlePartsMaterials;

    /**
     * Shows MainMenu with or without animation
     * **/
    public void Show(bool bAnimated)
    {
        ShowTitle(bAnimated);
        ShowFrames(bAnimated);
        ShowAxes(bAnimated, 1.5f);
        //ShowButtons(bAnimated, 2.0f);
    }

    public void Dismiss()
    {
        this.gameObject.transform.localPosition = new Vector3(0, 0, 100);
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

        //Set zero scale so they are invisible at start
        clonedTopLeft.transform.localScale = Vector3.zero;
        clonedTopRight.transform.localScale = Vector3.zero;
        clonedBottomRight.transform.localScale = Vector3.zero;
        clonedBottomLeft.transform.localScale = Vector3.zero;

        //Animate top left part
        FixedTextureSizeAnimator topLeftAnimator = clonedTopLeft.GetComponent<FixedTextureSizeAnimator>();
        topLeftAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        topLeftAnimator.MoveObjectBySettingPivotPointPosition(topLeftPartLocalPosition);
        topLeftAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);
        topLeftAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, 1.5f);
        topLeftAnimator.TranslateFromTo(topLeftPartLocalPosition, topLeftPartLocalPosition + new Vector3(0, 128, 0), 0.5f, 1.5f);

        //Animate top right part
        FixedTextureSizeAnimator topRightAnimator = clonedTopRight.GetComponent<FixedTextureSizeAnimator>();
        topRightAnimator.UpdatePivotPoint(new Vector3(0.5f, 1.0f, 0.5f));
        topRightAnimator.MoveObjectBySettingPivotPointPosition(topRightPartLocalPosition);
        topRightAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);
        topRightAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, 1.8f);
        topRightAnimator.TranslateFromTo(topRightPartLocalPosition, topRightPartLocalPosition + new Vector3(0, 128, 0), 0.5f, 1.8f);

        //Animate bottom right part
        FixedTextureSizeAnimator bottomRightAnimator = clonedBottomRight.GetComponent<FixedTextureSizeAnimator>();
        bottomRightAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.0f, 0.5f));
        bottomRightAnimator.MoveObjectBySettingPivotPointPosition(bottomRightPartLocalPosition);
        bottomRightAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);
        bottomRightAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, 1.5f);
        bottomRightAnimator.TranslateFromTo(bottomRightPartLocalPosition, bottomRightPartLocalPosition - new Vector3(0, 128, 0), 0.5f, 1.5f);

        //Animate bottom left part
        FixedTextureSizeAnimator bottomLeftAnimator = clonedBottomLeft.GetComponent<FixedTextureSizeAnimator>();
        bottomLeftAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.0f, 0.5f));
        bottomLeftAnimator.MoveObjectBySettingPivotPointPosition(bottomLeftPartLocalPosition);
        bottomLeftAnimator.m_textureSize = new Vector2(titlePartWidth, titlePartHeight);
        bottomLeftAnimator.ScaleFromTo(new Vector3(titlePartWidth, 0, 1.0f), new Vector3(titlePartWidth, titlePartHeight, 1.0f), 0.5f, 1.8f);
        bottomLeftAnimator.TranslateFromTo(bottomLeftPartLocalPosition, bottomLeftPartLocalPosition - new Vector3(0, 128, 0), 0.5f, 1.8f);
    }

    public void ShowFrames(bool bAnimated, float fDelay = 0.0f)
    {
        GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
        framesHolder.transform.position = new Vector3(0, 0, -5);

        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;
        GameObject axesHolder = GameObject.FindGameObjectWithTag("MainMenuAxes");
        float distanceToScreenTopBorder = 0.5f * screenSize.y - axesHolder.transform.position.y;
        float topFrameHeight = 2 * distanceToScreenTopBorder;

        GameObject topFrame = (GameObject) Instantiate(m_GUIFramePfb);
        topFrame.transform.parent = framesHolder.transform;
        topFrame.transform.localScale = new Vector3(screenSize.x, topFrameHeight, 1);
        topFrame.transform.localPosition = new Vector3(0, 0.5f * (screenSize.y - topFrameHeight), 0);
        topFrame.GetComponent<MeshRenderer>().material = m_framesMaterials[0];

        float middleFrameRatio = 0.5f; //the ratio of middle frame relatively to bottom frame
        float bottomFrameRatio = 1 - middleFrameRatio;
        GameObject middleFrame = (GameObject)Instantiate(m_GUIFramePfb);
        middleFrame.transform.parent = framesHolder.transform;
        float middleFrameHeight = middleFrameRatio * (screenSize.y - topFrameHeight);
        middleFrame.transform.localScale = new Vector3(screenSize.x, middleFrameHeight, 1);
        middleFrame.transform.localPosition = new Vector3(0, 0.5f * screenSize.y - topFrameHeight - 0.5f * middleFrameHeight, 0);
        middleFrame.GetComponent<MeshRenderer>().material = m_framesMaterials[1];

        GameObject bottomFrame = (GameObject)Instantiate(m_GUIFramePfb);
        bottomFrame.transform.parent = framesHolder.transform;
        float bottomFrameHeight = bottomFrameRatio * (screenSize.y - topFrameHeight);
        bottomFrame.transform.localScale = new Vector3(screenSize.x, bottomFrameHeight, 1);
        bottomFrame.transform.localPosition = new Vector3(0, 0.5f * screenSize.y - topFrameHeight - middleFrameHeight - 0.5f * bottomFrameHeight, 0);
        bottomFrame.GetComponent<MeshRenderer>().material = m_framesMaterials[2];
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
        if (bAnimated)
        {
            Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

            GameObject optionsPanel = GameObject.FindGameObjectWithTag("OptionsPanel");
            GameObject creditsPanel = GameObject.FindGameObjectWithTag("CreditsPanel");

            GameObjectAnimator optionsPanelAnimator = optionsPanel.GetComponent<GameObjectAnimator>();
            GameObjectAnimator creditsPanelAnimator = creditsPanel.GetComponent<GameObjectAnimator>();

            Vector3 optionsPanelFromPosition = new Vector3(optionsPanel.transform.position.x, -0.5f * screenSize.y - 175.0f, optionsPanel.transform.position.z);
            Vector3 creditsPanelFromPosition = new Vector3(creditsPanel.transform.position.x, -0.5f * screenSize.y - 175.0f, creditsPanel.transform.position.z);
            Vector3 optionsPanelToPosition = new Vector3(optionsPanel.transform.position.x, -0.5f * screenSize.y + 175.0f, optionsPanel.transform.position.z);
            Vector3 creditsPanelToPosition = new Vector3(creditsPanel.transform.position.x, -0.5f * screenSize.y + 175.0f, creditsPanel.transform.position.z);

            optionsPanel.transform.localPosition = optionsPanelFromPosition;
            creditsPanel.transform.localPosition = creditsPanelFromPosition;
            optionsPanelAnimator.TranslateFromTo(optionsPanelFromPosition, optionsPanelToPosition, 0.4f, fDelay);
            creditsPanelAnimator.TranslateFromTo(creditsPanelFromPosition, creditsPanelToPosition, 0.4f, fDelay + 0.15f);
        }
    }

    public void ShowTapToPlay()
    {
        //GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
    }
}

