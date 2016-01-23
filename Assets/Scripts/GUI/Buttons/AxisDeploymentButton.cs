using UnityEngine;

public class AxisDeploymentButton : GUIButton
{
    private Axis m_axis; //the axis bound to this button
    public Material m_deployAxisSkinMaterial;

    public GameObject m_numberTextObject; //the number that appears below the skin

    public void BuildForNumber(int number)
    {
        //skin
        Init(m_deployAxisSkinMaterial);

        //number
        TextMesh numberText = m_numberTextObject.GetComponent<TextMesh>();
        numberText.text = number.ToString();

        Disable();
    }

    /**
     * Make this button active
     * **/
    public void EnableForAxis(Axis axis)
    {
        m_axis = axis;
    }

    /**
     * Make this button inactive
     * **/
    public void Disable()
    {
        m_axis = null;
    }

    public override void OnClick()
    {
        Debug.Log("AxisDeploymentButton::OnClick");
    }
}
