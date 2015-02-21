using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject m_segmentPfb;
    public GameObject m_GUIFramePfb;
    public Material[] m_framesMaterials;

    /**
     * Shows MainMenu with or without animation
     * **/
    public void Show(bool bAnimated)
    {
        ShowFrames(bAnimated, 0.0f);
        ShowAxes(bAnimated, 1.5f);
        ShowButtons(bAnimated, 2.0f);
    }

    public void Dismiss()
    {
        GameObject rootNode = GameObject.FindGameObjectWithTag("GUIMainMenu");
        rootNode.SetActive(false);
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
            Vector3 optionsPanelToPosition = new Vector3(optionsPanel.transform.position.x, -0.5f * screenSize.y + 175.0f, -200);
            Vector3 creditsPanelToPosition = new Vector3(creditsPanel.transform.position.x, -0.5f * screenSize.y + 175.0f, creditsPanel.transform.position.z);

            optionsPanel.transform.localPosition = optionsPanelFromPosition;
            creditsPanel.transform.localPosition = creditsPanelFromPosition;
            optionsPanelAnimator.TranslateFromTo(optionsPanelFromPosition, optionsPanelToPosition, 0.4f, fDelay);
            creditsPanelAnimator.TranslateFromTo(creditsPanelFromPosition, creditsPanelToPosition, 0.4f, fDelay + 0.15f);

            optionsPanelAnimator.UpdatePivotPoint(new Vector3(0,0,0.5f));
            optionsPanelAnimator.RotateFromToAroundAxis(0, 360, new Vector3(0,0,1), 5.0f, 2 * fDelay);
        }
    }

    public void ShowTapToPlay()
    {
        //GameObject framesHolder = GameObject.FindGameObjectWithTag("FramesHolder");
    }
}

