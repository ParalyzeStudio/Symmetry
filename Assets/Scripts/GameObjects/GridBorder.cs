using UnityEngine;

public class GridBorder
{
    public Grid.GridBoxEdgeLocation m_location { get; set; }
    private GameObject m_lineObject; //the line that is used to render this border
    private Vector3 m_relaxedPosition; //when no shape is intersecting the border, the line is a bit off the grid
    private Vector3 m_contractedPosition; //when a shape is intersecting the border, the line is exactly on the outer grid points

    private bool m_relaxed;
    
    public GridBorder(Grid.GridBoxEdgeLocation location, GameObject lineObject, Vector3 relaxedPosition, Vector3 contractedPosition)
    {
        m_location = location;
        m_lineObject = lineObject;
        m_relaxedPosition = relaxedPosition;
        m_contractedPosition = contractedPosition;
    }

    public void Contract()
    {
        if (!m_relaxed)
            return;

        GameObjectAnimator lineAnimator = m_lineObject.GetComponent<GameObjectAnimator>();
        lineAnimator.TranslateTo(m_contractedPosition, 0.2f);

        m_relaxed = false;
    }

    public void Relax()
    {
        if (m_relaxed)
            return;

        GameObjectAnimator lineAnimator = m_lineObject.GetComponent<GameObjectAnimator>();
        lineAnimator.TranslateTo(m_relaxedPosition, 0.2f);

        m_relaxed = true;
    }
}
