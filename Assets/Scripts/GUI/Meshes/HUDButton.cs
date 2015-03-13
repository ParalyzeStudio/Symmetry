using UnityEngine;

public class HUDButton : GUIQuadButton
{
    private GameHUD m_gameHUD;

    //Ids for HUD buttons
    //Action buttons
    public enum HUDButtonID
    {
        ID_SYMMETRY_AXIS_HORIZONTAL = 1,
        ID_SYMMETRY_AXIS_VERTICAL,
        ID_SYMMETRY_STRAIGHT_AXES,
        ID_SYMMETRY_AXIS_DIAGONAL_LEFT,
        ID_SYMMETRY_AXIS_DIAGONAL_RIGHT,
        ID_SYMMETRY_DIAGONAL_AXES,
        ID_SYMMETRY_ALL_AXES
    }

    public HUDButtonID m_ID { get; set; }

    public enum HUDButtonType
    {
        ACTION,
        INTERFACE
    }

    public HUDButtonType m_type { get; set; }

    protected override void Start()
    {
        base.Start();
        m_gameHUD = GameObject.FindGameObjectWithTag("GameHUD").GetComponent<GameHUD>();
    }

    public override bool OnPress()
    {
        if (!base.OnPress())
            return false;

        //Move the skin by 6 pixels right and bottom
        GameObject buttonSkin = this.gameObject;
        buttonSkin.transform.localPosition += new Vector3(6, -6, 0);

        return true;
    }

    public override bool OnRelease()
    {
        if (!base.OnRelease())
            return false;

        //Move the skin by 6 pixels left and top
        GameObject buttonSkin = this.gameObject;
        buttonSkin.transform.localPosition -= new Vector3(6, -6, 0);

        return true;
    }

    public override void OnClick()
    {
        base.OnClick();

        switch (m_ID)
        {
            case HUDButtonID.ID_SYMMETRY_ALL_AXES:
                OnClickAllAxes();
                break;
            case HUDButtonID.ID_SYMMETRY_AXIS_HORIZONTAL:
                OnClickHorizontalAxis();
                break;
            case HUDButtonID.ID_SYMMETRY_AXIS_VERTICAL:
                OnClickVerticalAxis();
                break;
            case HUDButtonID.ID_SYMMETRY_STRAIGHT_AXES:
                OnClickStraightAxes();
                break;
            case HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_LEFT:
                OnClickLeftDiagonalAxis();
                break;
            case HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_RIGHT:
                OnClickRightDiagonalAxis();
                break;
            case HUDButtonID.ID_SYMMETRY_DIAGONAL_AXES:
                OnClickDiagonalAxes();
                break;
            default:
                break;
        }

        if (m_type == HUDButtonType.ACTION)
        {
            string actionTag = GetActionTagForButtonID(m_ID);

            GameObject axesObject = GameObject.FindGameObjectWithTag("Axes");
            AxesHolder axesHolder = axesObject.GetComponent<AxesHolder>();
            GameObject axisUnderConstruction = axesHolder.GetAxisBeingBuilt();
            if (axisUnderConstruction != null)
            {
                Vector2 axisFirstEndpointGridPosition = axisUnderConstruction.GetComponent<AxisRenderer>().m_endpoint1GridPosition;

                GridBuilder gridBuilder = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridBuilder>();
                Symmetrizer.SymmetryType symmetryType = axisUnderConstruction.GetComponent<Symmetrizer>().GetSymmetryTypeForActionTag(actionTag);
                gridBuilder.RenderConstraintAnchors(axisFirstEndpointGridPosition, symmetryType);
            }
        }
    }

    private void OnClickVerticalAxis()
    {
        m_gameHUD.m_activeActionTag = GameHUD.ACTION_TAG_SYMMETRY_AXIS_VERTICAL;
    }

    private void OnClickHorizontalAxis()
    {
        m_gameHUD.m_activeActionTag = GameHUD.ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL;
    }

    private void OnClickStraightAxes()
    {
        m_gameHUD.m_activeActionTag = GameHUD.ACTION_TAG_SYMMETRY_AXES_STRAIGHT;
    }

    private void OnClickLeftDiagonalAxis()
    {
        m_gameHUD.m_activeActionTag = GameHUD.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT;
    }

    private void OnClickRightDiagonalAxis()
    {
        m_gameHUD.m_activeActionTag = GameHUD.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT;
    }

    private void OnClickDiagonalAxes()
    {
        m_gameHUD.m_activeActionTag = GameHUD.ACTION_TAG_SYMMETRY_AXES_DIAGONALS;
    }

    private void OnClickAllAxes()
    {
        m_gameHUD.m_activeActionTag = GameHUD.ACTION_TAG_SYMMETRY_AXES_ALL;
    }

    private void OnClickShapesButton()
    {

    }

    private string GetActionTagForButtonID(HUDButtonID iID)
    {
        if (iID == HUDButtonID.ID_SYMMETRY_ALL_AXES)
            return GameHUD.ACTION_TAG_SYMMETRY_AXES_ALL;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_LEFT)
            return GameHUD.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_RIGHT)
            return GameHUD.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_HORIZONTAL)
            return GameHUD.ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_VERTICAL)
            return GameHUD.ACTION_TAG_SYMMETRY_AXIS_VERTICAL;
        else if (iID == HUDButtonID.ID_SYMMETRY_STRAIGHT_AXES)
            return GameHUD.ACTION_TAG_SYMMETRY_AXES_STRAIGHT;
        else if (iID == HUDButtonID.ID_SYMMETRY_DIAGONAL_AXES)
            return GameHUD.ACTION_TAG_SYMMETRY_AXES_DIAGONALS;
        else
            return string.Empty;
    }
}
