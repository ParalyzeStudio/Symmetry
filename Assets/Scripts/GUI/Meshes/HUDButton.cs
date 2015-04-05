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
        ID_SYMMETRY_ALL_AXES,
        ID_MOVE_SHAPE
    }

    public HUDButtonID m_ID { get; set; }

    protected override void Start()
    {
        //MeshRenderer skinRenderer = this.gameObject.GetComponentsInChildren<MeshRenderer>()[0];
        //Texture quadMainTexture = skinRenderer.sharedMaterial.mainTexture;
        //m_isTextured = (quadMainTexture != null);
        //InitQuadMesh();
    }

    public void SetSize(Vector2 size)
    {
        Transform[] childTransforms = this.gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i != childTransforms.Length; i++)
        {
            if (childTransforms[i] != this.transform)
                childTransforms[i].transform.localScale = GeometryUtils.BuildVector3FromVector2(size, 1);
        }
    }

    public override bool OnPress()
    {
        if (!base.OnPress())
            return false;

        //Move the skin by 6 pixels right and bottom
        Transform skinTransform = this.gameObject.GetComponentsInChildren<Transform>()[1];
        skinTransform.localPosition += new Vector3(6, -6, 0);

        return true;
    }

    public override bool OnRelease()
    {
        if (!base.OnRelease())
            return false;

        //Move the skin by 6 pixels left and top
        Transform skinTransform = this.gameObject.GetComponentsInChildren<Transform>()[1];
        skinTransform.localPosition -= new Vector3(6, -6, 0);

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
            case HUDButtonID.ID_MOVE_SHAPE:
                OnClickMoveShape();
                break;
            default:
                break;
        }

        string actionTag = GetActionTagForButtonID(m_ID);

        //Axes axes = GetGameScene().GetComponentInChildren<Axes>();
        //GameObject axisUnderConstruction = axes.GetAxisBeingBuilt();
        //if (axisUnderConstruction != null)
        //{
        //    Vector2 axisFirstEndpointGridPosition = axisUnderConstruction.GetComponent<AxisRenderer>().m_endpoint1GridPosition;

        //    GameScene gameScene = (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
        //    Symmetrizer.SymmetryType symmetryType = axisUnderConstruction.GetComponent<Symmetrizer>().GetSymmetryTypeForActionTag(actionTag);
        //    gameScene.m_grid.RenderConstraintAnchors(axisFirstEndpointGridPosition, symmetryType);
        //}
    }

    private void OnClickVerticalAxis()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_SYMMETRY_AXIS_VERTICAL;
    }

    private void OnClickHorizontalAxis()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL;
    }

    private void OnClickStraightAxes()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_SYMMETRY_AXES_STRAIGHT;
    }

    private void OnClickLeftDiagonalAxis()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT;
    }

    private void OnClickRightDiagonalAxis()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT;
    }

    private void OnClickDiagonalAxes()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_SYMMETRY_AXES_DIAGONALS;
    }

    private void OnClickAllAxes()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_SYMMETRY_AXES_ALL;
    }

    private void OnClickMoveShape()
    {
        GetGameScene().m_activeActionTag = GameScene.ACTION_TAG_MOVE_SHAPE;
    }

    private string GetActionTagForButtonID(HUDButtonID iID)
    {
        if (iID == HUDButtonID.ID_SYMMETRY_ALL_AXES)
            return GameScene.ACTION_TAG_SYMMETRY_AXES_ALL;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_LEFT)
            return GameScene.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_LEFT;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_DIAGONAL_RIGHT)
            return GameScene.ACTION_TAG_SYMMETRY_AXIS_DIAGONAL_RIGHT;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_HORIZONTAL)
            return GameScene.ACTION_TAG_SYMMETRY_AXIS_HORIZONTAL;
        else if (iID == HUDButtonID.ID_SYMMETRY_AXIS_VERTICAL)
            return GameScene.ACTION_TAG_SYMMETRY_AXIS_VERTICAL;
        else if (iID == HUDButtonID.ID_SYMMETRY_STRAIGHT_AXES)
            return GameScene.ACTION_TAG_SYMMETRY_AXES_STRAIGHT;
        else if (iID == HUDButtonID.ID_SYMMETRY_DIAGONAL_AXES)
            return GameScene.ACTION_TAG_SYMMETRY_AXES_DIAGONALS;
        else if (iID == HUDButtonID.ID_MOVE_SHAPE)
            return GameScene.ACTION_TAG_MOVE_SHAPE;
        else
            return string.Empty;
    }

    private GameScene GetGameScene()
    {
        return (GameScene)GameObject.FindGameObjectWithTag("Scenes").GetComponent<SceneManager>().m_currentScene;
    }
}
