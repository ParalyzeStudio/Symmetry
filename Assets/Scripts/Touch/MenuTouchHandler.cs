using UnityEngine;

public class MenuTouchHandler : TouchHandler
{
    public float m_circleButtonsTouchAreaRadius;

    private GameController m_gameController;
    private GUIManager m_guiManager;

    public override void Start()
    {
        m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        m_guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
    }

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return true;
    }

    protected override void OnClick()
    {
        Vector2 clickLocation = m_prevPointerLocation;
        Vector2 m_designScreenSize = m_gameController.m_designScreenSize;

        if (m_guiManager.IsOptionsWindowShown())
        {
            GUIInterfaceButton musicBtn = GameObject.FindGameObjectWithTag("MusicButton").GetComponent<GUIInterfaceButton>();
            if (musicBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(musicBtn.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    musicBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }

            GUIInterfaceButton soundBtn = GameObject.FindGameObjectWithTag("SoundButton").GetComponent<GUIInterfaceButton>();
            if (soundBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(soundBtn.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    soundBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }
        }
        else
        {
            ////Check if we clicked a button first to swallow touch
            //Options Button
            GUIInterfaceButton optionsBtn = GameObject.FindGameObjectWithTag("OptionsButton").GetComponent<GUIInterfaceButton>();
            if (optionsBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(optionsBtn.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    optionsBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }

            //Credits Button
            GUIInterfaceButton creditsBtn = GameObject.FindGameObjectWithTag("CreditsButton").GetComponent<GUIInterfaceButton>();
            if (creditsBtn != null)
            {
                float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(creditsBtn.transform.position) - clickLocation).magnitude;
                if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
                {
                    creditsBtn.OnClick();
                    return; //swallow the touch by returning
                }
            }

            //transform the click location in screen rect coordinates
            clickLocation += new Vector2(0.5f * m_designScreenSize.x, 0.5f * m_designScreenSize.y);
            clickLocation.y = -clickLocation.y + m_designScreenSize.y;

            Rect tapToPlayAreaRect = new Rect();

            tapToPlayAreaRect.width = m_designScreenSize.x;
            tapToPlayAreaRect.height = 0.78f * m_designScreenSize.y;
            tapToPlayAreaRect.position = Vector3.zero;

            if (tapToPlayAreaRect.Contains(clickLocation))
            {
                OnClickTapToPlay();
            }
        }
    }

    public void OnClickTapToPlay()
    {
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.SwitchDisplayedContent(GUIManager.DisplayContent.CHAPTERS);
    }
}