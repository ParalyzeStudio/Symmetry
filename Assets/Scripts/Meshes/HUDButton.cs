using UnityEngine;

public class HUDButton : GUIQuadButton
{
    public override void OnPress()
    {
        base.OnPress();
        Debug.Log(this.transform.parent.tag);
    }

    public override void OnClick()
    {
        base.OnClick();
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
