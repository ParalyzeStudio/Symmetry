using UnityEngine;

public class HUDButton : GUIQuadButton
{
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

    public HUDButtonID m_iID { get; set; }

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

        Debug.Log(m_iID);
    }

    private void OnClickVerticalAxis()
    {

    }

    private void OnClickHorizontalAxis()
    {

    }

    private void OnClickStraightAxes()
    {

    }

    private void OnClickLeftDiagonal()
    {

    }

    private void OnClickRightDiagonal()
    {

    }

    private void OnClickDiagonalAxes()
    {

    }

    private void OnClickDiagonalAllAxes()
    {

    }

    private void OnClickShapesButton()
    {

    }
}
