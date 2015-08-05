using UnityEngine;
using System.Collections.Generic;

public class Axes : MonoBehaviour
{
    public List<GameObject> m_childrenAxes { get; set; }
    public GameObject m_axisPfb;

    //snap animation
    public GameObject m_circlePfb;
    public Material m_positionColorMaterial;
    private Material m_circleMaterial;

    public void Awake()
    {
        m_childrenAxes = new List<GameObject>();
    }

    public void Start()
    {
        m_circleMaterial = Instantiate(m_positionColorMaterial);
    }

    public GameObject BuildAxis(Vector2 gridStartPosition)
    {
        GameObject newAxis = (GameObject)Instantiate(m_axisPfb);
        newAxis.transform.parent = this.gameObject.transform;
        newAxis.transform.localPosition = Vector3.zero;

        //Build and render the axis once
        AxisRenderer axisRenderer = newAxis.GetComponent<AxisRenderer>();
        axisRenderer.BuildElements(gridStartPosition, true);

        AddAxis(newAxis);
        return newAxis;
    }

    public void AddAxis(GameObject axis)
    {
        m_childrenAxes.Add(axis);
    }

    public void RemoveAxis(GameObject axis)
    {
        for (int axisIndex = 0; axisIndex != m_childrenAxes.Count; axisIndex++)
        {
            if (m_childrenAxes[axisIndex] == axis)
            {
                m_childrenAxes.Remove(axis);
                return;
            }
        }
    }

    public void ClearAxes()
    {
        m_childrenAxes.Clear();
    }

    public GameObject GetAxisBeingBuilt()
    {
        if (m_childrenAxes.Count > 0)
            return m_childrenAxes[m_childrenAxes.Count - 1];

        return null;
    }

    /**
     * Simple animation with circle scaling up and fading out when an axis snaps
     * **/
    public void LaunchSnapCircleAnimation(Vector2 position)
    {
        GameObject clonedCircle = (GameObject)Instantiate(m_circlePfb);
        //clonedCircle.transform.parent = this.transform;
        clonedCircle.transform.localPosition = GeometryUtils.BuildVector3FromVector2(position, -10);
        CircleMesh circleMesh = clonedCircle.GetComponent<CircleMesh>();
        circleMesh.Init(m_circleMaterial);

        CircleMeshAnimator circleAnimator = clonedCircle.GetComponent<CircleMeshAnimator>();
        circleAnimator.SetNumSegments(64, false);
        circleAnimator.SetInnerRadius(0, false);
        circleAnimator.SetOuterRadius(2, true);
        circleAnimator.SetColor(Color.white);

        circleAnimator.AnimateInnerRadiusTo(20, 0.3f, 0.0f, ValueAnimator.InterpolationType.LINEAR);
        circleAnimator.AnimateOuterRadiusTo(22, 0.3f, 0.0f, ValueAnimator.InterpolationType.LINEAR);
        circleAnimator.SetOpacity(1);
        circleAnimator.FadeTo(0, 0.3f, 0, ValueAnimator.InterpolationType.LINEAR, true);
    }
}
