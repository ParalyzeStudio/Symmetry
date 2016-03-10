using UnityEngine;

public class SymmetryPoint : MonoBehaviour
{
    public GameObject m_arrowsObject;
    public GameObject m_circleObject;

    private TexturedQuadAnimator m_circleAnimator;
    private TexturedQuadAnimator m_arrowsAnimator;

    private Grid.GridAnchor m_snappedAnchor;
    private bool m_circleDetached;

    public void Build()
    {
        UVQuad arrowsQuad = m_arrowsObject.GetComponent<UVQuad>();
        arrowsQuad.Init();

        UVQuad circleQuad = m_circleObject.GetComponent<UVQuad>();
        circleQuad.Init();

        m_circleAnimator = m_circleObject.GetComponent<TexturedQuadAnimator>();
        m_arrowsAnimator = m_arrowsObject.GetComponent<TexturedQuadAnimator>();

        //initialize the symmetrizer component
        PointSymmetrizer symmetrizer = this.GetComponent<PointSymmetrizer>();
        symmetrizer.Init();
    }

    public bool ContainsPointer(Vector2 pointerLocation)
    {
        Vector2 pointSize = m_arrowsObject.GetComponent<MeshRenderer>().bounds.size;

        return pointerLocation.x >= this.gameObject.transform.position.x - 0.5f * pointSize.x &&
               pointerLocation.x <= this.gameObject.transform.position.x + 0.5f * pointSize.x &&
               pointerLocation.y >= this.gameObject.transform.position.y - 0.5f * pointSize.y &&
               pointerLocation.y <= this.gameObject.transform.position.y + 0.5f * pointSize.y;
    }

    public void SnapToClosestGridAnchor(Grid grid, Vector2 pointerLocation)
    {
        Grid.GridAnchor closestAnchor = grid.GetClosestGridAnchorForWorldPosition(pointerLocation);
        if (closestAnchor == null) //we got out of grid bounds and could not find an anchor
        {
            SetCirclePosition(pointerLocation);
            m_snappedAnchor = null;
        }
        else
        {
            if (closestAnchor != m_snappedAnchor)
            {
                m_snappedAnchor = closestAnchor;
                Vector3 anchorWorldPosition = GeometryUtils.BuildVector3FromVector2(m_snappedAnchor.m_localPosition, 0) + grid.transform.position;
                SetCirclePosition(anchorWorldPosition);
            }
        }
    }

    public void DetachCircle()
    {
        if (!m_circleDetached)
        {
            m_circleDetached = true;
            m_circleAnimator.SetParentTransform(null);
        }
    }

    public void ResetCircle()
    {
        m_circleDetached = false;
        m_circleAnimator.SetParentTransform(this.transform);
        m_circleAnimator.SetPosition(m_arrowsAnimator.GetPosition());
    }

    public void ReleaseCircle()
    {
        if (m_snappedAnchor != null) //perform point symmetry
        {
            Symmetrizer pointSymmetrizer = (Symmetrizer)this.gameObject.GetComponent<Symmetrizer>();
            pointSymmetrizer.Symmetrize();
        }
        else
        {
            ResetCircle();
        }
    }

    public Vector3 GetCirclePosition()
    {
        return m_circleAnimator.GetPosition();
    }

    public void SetCirclePosition(Vector2 position)
    {
        //set the z-position of the circle the same as axes
        m_circleAnimator.SetPosition(GeometryUtils.BuildVector3FromVector2(position, m_circleDetached ? GameScene.AXES_Z_VALUE : 0));
    }

    public GridPoint GetCircleGridPosition()
    {
        if (m_snappedAnchor == null)
            throw new System.Exception("Symmetry point is not anchored");

        return m_snappedAnchor.m_gridPosition;
    }

    public void OnPerformSymmetry()
    {
        Destroy(m_circleAnimator.gameObject);
    }
}