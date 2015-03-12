using UnityEngine;

public class GUITouchHandler : TouchHandler
{
    public float m_circleButtonsTouchAreaRadius;
    private GameController m_gameController;

    public override void Start()
    {
        m_gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    protected override bool IsPointerLocationContainedInObject(Vector2 pointerLocation)
    {
        return true;
    }

    /**
     * Callback when user clicked the screen
     * **/
    protected override void OnClick()
    {
        Vector2 clickLocation = m_prevPointerLocation;
        GUIManager guiManager = this.gameObject.GetComponent<GUIManager>();
        if (guiManager.m_displayedContent == GUIManager.DisplayContent.MENU)
        {
            HandleClickOnMainMenu(clickLocation);
        }
        else if (guiManager.m_displayedContent == GUIManager.DisplayContent.CHAPTERS)
        {
            HandleClickOnChapters(clickLocation);
        }
    }

    /**
     * The user clicked on the tap to play area
     * **/
    public void OnClickTapToPlay()
    {
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        guiManager.SwitchDisplayedContent(GUIManager.DisplayContent.CHAPTERS);
    }

    /**
     * Processes click on main menu scene
     * **/
    public void HandleClickOnMainMenu(Vector2 clickLocation)
    {
        GUIManager guiManager = this.gameObject.GetComponent<GUIManager>();
        Vector2 screenSize = GameObject.FindGameObjectWithTag("Background").GetComponent<BackgroundAdaptativeSize>().m_screenSizeInUnits;

        if (guiManager.IsOptionsWindowShown())
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
            GUIInterfaceButton optionsBtn = GameObject.FindGameObjectWithTag("OptionsPanel").GetComponentInChildren<GUIInterfaceButton>();
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
            GUIInterfaceButton creditsBtn = GameObject.FindGameObjectWithTag("CreditsPanel").GetComponentInChildren<GUIInterfaceButton>();
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
            clickLocation += new Vector2(0.5f * screenSize.x, 0.5f * screenSize.y);
            clickLocation.y = -clickLocation.y + screenSize.y;

            Rect tapToPlayAreaRect = new Rect();

            tapToPlayAreaRect.width = screenSize.x;
            tapToPlayAreaRect.height = 0.25f * screenSize.y;
            tapToPlayAreaRect.position = new Vector2(0, 0.5f * screenSize.y);

            if (tapToPlayAreaRect.Contains(clickLocation))
                OnClickTapToPlay();
        }
    }

    /**
     * Processes click on chapters scene
     * **/
    public void HandleClickOnChapters(Vector2 clickLocation)
    {
        GUIInterfaceButton backBtn = GameObject.FindGameObjectWithTag("BackButton").GetComponent<GUIInterfaceButton>();
        if (backBtn != null)
        {
            float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(backBtn.transform.position) - clickLocation).magnitude;
            if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
            {
                backBtn.OnClick();
                return; //swallow the touch by returning
            }
        }

        Chapters chapters = this.gameObject.GetComponentInChildren<Chapters>();
        GameObject[] chaptersSlots = chapters.m_chapterSlots;
        for (int iSlotIndex = 0; iSlotIndex != chaptersSlots.Length; iSlotIndex++)
        {
            Vector2 slotSize = chaptersSlots[iSlotIndex].GetComponent<GameObjectAnimator>().GetGameObjectSize();
            Vector2 slotPosition = chaptersSlots[iSlotIndex].transform.position;
            if (clickLocation.x <= slotPosition.x + 0.5f * slotSize.x && clickLocation.x >= slotPosition.x - 0.5f * slotSize.x
                &&
                clickLocation.y <= slotPosition.y + 0.5f * slotSize.y && clickLocation.y >= slotPosition.y - 0.5f * slotSize.y)
            {
                chapters.OnClickChapterSlot(iSlotIndex);
            }
        }
    }

    /**
     * Processes click on levels scene
     * **/
    public void HandleClickOnLevels(Vector2 clickLocation)
    {

    }
}