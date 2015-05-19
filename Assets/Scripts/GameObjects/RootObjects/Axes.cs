using UnityEngine;
using System.Collections.Generic;

public class Axes : MonoBehaviour
{
    public List<GameObject> m_childrenAxes { get; set; }
    public GameObject m_axisPfb;
    public Material m_axisSegmentMaterial;
    public Material m_axisSegmentMaterialInstance { get; set; }

    public GameObject m_circlePfb;
    public Material m_circleMaterial;
    private Material m_circleMaterialInstance;

    public void Awake()
    {
        m_childrenAxes = new List<GameObject>();
    }

    public void Start()
    {
        m_axisSegmentMaterialInstance = (Material)Instantiate(m_axisSegmentMaterial);
        m_circleMaterialInstance = (Material)Instantiate(m_circleMaterial);
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
        ColorCircleMesh circleMesh = clonedCircle.GetComponent<ColorCircleMesh>();
        circleMesh.Init(m_circleMaterialInstance);

        CircleAnimator circleAnimator = clonedCircle.GetComponent<CircleAnimator>();
        circleAnimator.SetInnerRadius(0);
        circleAnimator.SetThickness(2);
        circleAnimator.SetNumSegments(64);
        circleAnimator.SetColor(Color.black);

        circleAnimator.SetInnerRadius(0);
        circleAnimator.AnimateRadiusTo(20, 0.3f, 0.0f, ValueAnimator.InterpolationType.LINEAR, true);
        circleAnimator.SetOpacity(1);
        circleAnimator.FadeTo(0, 0.3f);
    }
}
