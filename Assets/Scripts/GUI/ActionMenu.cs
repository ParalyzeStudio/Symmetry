using System.Collections.Generic;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    public Grid.GridAnchor m_relatedAnchor { get; set; }

    //some sprite overlay to indicate which grid anchor has been used to show this menu
    public GameObject m_anchorOverlayPfb; 
    private GameObject m_anchorOverlayObject;

    /**
    * Build this menu. If no action is available return false
    **/
    public bool Build(GameScene parentScene, Vector3 position, Grid.GridAnchor relatedAnchor)
    {
        Vector2 gridPosition = parentScene.m_grid.transform.position;
        Vector2 anchorPosition = gridPosition + relatedAnchor.m_localPosition;

        m_relatedAnchor = relatedAnchor;

        //set correct position
        this.GetComponent<GameObjectAnimator>().SetPosition(position);

        //Build the child action buttons
        int availableActions = parentScene.GetLevelManager().m_currentLevel.Actions;
        bool bTwoSidedSymmetry = (availableActions & Level.ACTION_TWO_SIDED_SYMMETRY) > 0;
        bool bPointSymmetry = (availableActions & Level.ACTION_POINT_SYMMETRY) > 0;

        List<GUIButton.GUIButtonID> actionIDs = new List<GUIButton.GUIButtonID>();
        if (bTwoSidedSymmetry)
        {
            actionIDs.Add(GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_ONE_SIDE);
            actionIDs.Add(GUIButton.GUIButtonID.ID_AXIS_SYMMETRY_TWO_SIDES);
        }
        if (bPointSymmetry)
            actionIDs.Add(GUIButton.GUIButtonID.ID_POINT_SYMMETRY);

        if (actionIDs.Count == 0)
            return false;

        Vector2 actionButtonSize = new Vector2(128, 128);
        float verticalMargin = 50.0f;
        for (int i = 0; i != actionIDs.Count; i++)
        {
            GameObject buttonObject = parentScene.GetGUIManager().CreateActionButtonForID(this, actionIDs[i], actionButtonSize);

            Vector3 buttonPosition;
            if (actionIDs.Count % 2 == 0)
            {
                buttonPosition = new Vector3(0, (actionButtonSize.y + verticalMargin) * (i - actionIDs.Count / 2 + 0.5f), 0);
            }
            else
            {
                buttonPosition = new Vector3(0, (actionButtonSize.y + verticalMargin) * (i - actionIDs.Count / 2), 0);
            }

            GameObjectAnimator buttonAnimator = buttonObject.GetComponent<GameObjectAnimator>();
            buttonAnimator.SetParentTransform(this.transform);
            buttonAnimator.SetPosition(buttonPosition);
        }

        //build the cancel button
        GameObject cancelButtonObject = parentScene.GetGUIManager().CreateActionButtonForID(this, GUIButton.GUIButtonID.ID_CANCEL_ACTION_MENU, actionButtonSize);

        GameObjectAnimator cancelButtonAnimator = cancelButtonObject.GetComponent<GameObjectAnimator>();
        cancelButtonAnimator.SetParentTransform(this.transform);
        cancelButtonAnimator.SetPosition(new Vector3(0,400,0));

        //show an overlay on the grid anchor we clicked
        ShowAnchorOverlayAtPosition(new Vector3(anchorPosition.x, anchorPosition.y, GameScene.ACTION_MENU_Z_VALUE));

        return true;
    }

    private void ShowAnchorOverlayAtPosition(Vector3 position)
    {
        m_anchorOverlayObject = (GameObject)Instantiate(m_anchorOverlayPfb);
        m_anchorOverlayObject.name = "AnchorOverlay";

        UVQuad anchorOverlay = m_anchorOverlayObject.GetComponent<UVQuad>();
        anchorOverlay.Init();

        GameObjectAnimator overlayAnimator = m_anchorOverlayObject.GetComponent<GameObjectAnimator>();
        overlayAnimator.SetParentTransform(this.transform);
        Debug.Log(this.name + ":" + this.transform.localPosition);
        overlayAnimator.SetPosition(position - this.transform.position);
        overlayAnimator.SetScale(new Vector3(64, 64, 1));
    }
}
