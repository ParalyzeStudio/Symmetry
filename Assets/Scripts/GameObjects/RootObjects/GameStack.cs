using UnityEngine;
using System.Collections.Generic;

/**
 * Hold the elements stacked by the player when drawing axes
 * **/
public class GameStack : MonoBehaviour
{
    private Stack<GameObject> m_elements;

    public GameObject m_stackElementPfb;
    private float m_elementSize; //the size of one element
    private float m_elementSpacing; //the spacing between 2 elements

    private SymmetryStack m_stack; //the underlying data

    /**
     * Build the stack by initiating base elements
     * **/
    public void Build()
    {
        m_elementSize = 64.0f;
        m_elementSpacing = 16.0f;

        //build the list of elements
        m_elements = new Stack<GameObject>();

        //Build the unstack button
        GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
        GameObject unstackButtonObject = guiManager.CreateGUIButtonForID(GUIButton.GUIButtonID.ID_UNSTACK_SYMMETRY, new Vector2(128.0f, 128.0f));
        unstackButtonObject.name = "UnstackButton";

        GameObjectAnimator unstackButtonAnimator = unstackButtonObject.GetComponent<GameObjectAnimator>();
        unstackButtonAnimator.SetParentTransform(this.transform);
        unstackButtonAnimator.SetPosition(new Vector3(0, 64.0f, 0));

        //build the stack data
        m_stack = new SymmetryStack();
    }

    /**
    * Build a stack element for the current top element number and the 'axis' parameter
    * **/
    public void PushAxis(AxisRenderer axis)
    {
        //add new data on top of the stack
        m_stack.StackSymmetry(axis);

        //build and store a new visual element containing the number of the element
        int elementIndex = m_elements.Count;

        GameObject stackElementObject = (GameObject)Instantiate(m_stackElementPfb);
        stackElementObject.name = "StackElement";

        //Set position for this element
        GameObjectAnimator elementAnimator = stackElementObject.GetComponent<GameObjectAnimator>();
        elementAnimator.SetParentTransform(this.transform);
        Vector2 elementPosition = new Vector2(0, -elementIndex * (m_elementSize + m_elementSpacing));
        elementAnimator.SetPosition(elementPosition);

        TextMesh numberTextMesh = stackElementObject.GetComponentInChildren<TextMesh>();
        numberTextMesh.text = (elementIndex + 1).ToString();

        TextMeshAnimator numberAnimator = stackElementObject.GetComponent<TextMeshAnimator>();
        numberAnimator.SetFontHeight(50);
        numberAnimator.SetColor(Color.white);

        m_elements.Push(stackElementObject);
    }

    /**
     * Removes a visual element to the stack with a number for instance
     * **/
    public void Pop()
    {
        GameObject topElement = m_elements.Pop();
        GameObjectAnimator elementAnimator = topElement.GetComponent<GameObjectAnimator>();
        Vector3 toPosition = elementAnimator.GetPosition() + new Vector3(2 * m_elementSize, 0, 0);
        elementAnimator.TranslateTo(toPosition, 0.5f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);

        //unstack the top element from symmetry data
        m_stack.UnstackSymmetryGroup();
    }      

    /**
     * Show the button to unstack elements
     * **/
    private void ShowUnstackButton()
    {

    }

    /**
     * Hide the button to unstack elements
     * **/
    private void HideUnstackButton()
    {

    }
}