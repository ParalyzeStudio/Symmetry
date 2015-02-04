using UnityEngine;

public class MenuTouchHandler : TouchHandler
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

    protected override void OnClick()
    {
        Vector2 clickLocation = m_prevPointerLocation;

        Vector2 m_designScreenSize = m_gameController.m_designScreenSize;

        ////Check if we clicked a button first to swallow touch
        //Options Button
        GameObject optionsBtn = GameObject.FindGameObjectWithTag("OptionsButton");
        if (optionsBtn != null)
        {
            float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(optionsBtn.transform.position) - clickLocation).magnitude;
            if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
            {
                OnClickOptions();
                return; //swallow the touch by returning
            }
        }

        //Credits Button
        GameObject creditsBtn = GameObject.FindGameObjectWithTag("CreditsButton");
        if (creditsBtn != null)
        {
            float clickLocationToButtonDistance = (GeometryUtils.BuildVector2FromVector3(creditsBtn.transform.position) - clickLocation).magnitude;
            if (clickLocationToButtonDistance <= m_circleButtonsTouchAreaRadius)
            {
                OnClickCredits();
                return; //swallow the touch by returning
            }
        }

        //transform the click location in screen rect coordinates
        clickLocation += new Vector2(0.5f * m_designScreenSize.x, 0.5f * m_designScreenSize.y);
        clickLocation.y = -clickLocation.y + m_designScreenSize.y;

        Rect tapToPlayAreaRect = new Rect();
        Vector2 position = transform.position;

        tapToPlayAreaRect.width = m_designScreenSize.x;
        tapToPlayAreaRect.height = 0.78f * m_designScreenSize.y;
        tapToPlayAreaRect.position = Vector3.zero;

        if (tapToPlayAreaRect.Contains(clickLocation))
        {
            OnClickTapToPlay();
        }
    }

    public void OnClickTapToPlay()
    {
        Application.LoadLevel("test_scene");
        m_gameController.BuildAndShowLevelsMenu();
    }

    public void OnClickOptions()
    {
        Debug.Log("Click Options");
    }

    public void OnClickCredits()
    {
        Debug.Log("Click Credits");
    }
}
