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

    public GameObject BuildAxis(GridPoint startPosition)
    {
        GameObject newAxis = (GameObject)Instantiate(m_axisPfb);
        GameObjectAnimator axisAnimator = newAxis.GetComponent<GameObjectAnimator>();
        axisAnimator.SetParentTransform(this.transform);
        axisAnimator.SetPosition(Vector3.zero);

        //Set the symmetry type
        Symmetrizer symmetrizer = newAxis.GetComponent<Symmetrizer>();
        symmetrizer.Init();
        GameScene parentScene = this.transform.parent.gameObject.GetComponent<GameScene>();
        symmetrizer.SetSymmetryTypeFromActionButtonID(parentScene.GetActionButtonID(ActionButton.Location.TOP));

        //Build and render the axis once
        AxisRenderer axisRenderer = newAxis.GetComponent<AxisRenderer>();
        axisRenderer.BuildElements(startPosition);

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

    ///**
    // * Return the axes whose type is either DYNAMIC_SNAPPED or DYNAMIC_UNSNAPPED
    // * **/
    //public List<AxisRenderer> GetDynamicAxes()
    //{
    //    List<AxisRenderer> dynamicAxes = new List<AxisRenderer>(); //dynamicAxes Count is likely to be 1 so set no capacity here
    //    for (int i = 0; i != m_childrenAxes.Count; i++)
    //    {
    //        AxisRenderer axis = m_childrenAxes[i].GetComponent<AxisRenderer>();
    //        if (axis.m_type == AxisRenderer.AxisType.DYNAMIC_SNAPPED ||
    //            axis.m_type == AxisRenderer.AxisType.DYNAMIC_UNSNAPPED)
    //        {
    //            dynamicAxes.Add(axis);
    //        }
    //    }

    //    return dynamicAxes;
    //}

    /**
     * Return the axis the player is currently drawing
     * **/
    public AxisRenderer GetAxisBeingDrawn()
    {
        for (int i = 0; i != m_childrenAxes.Count; i++)
        {
            AxisRenderer axis = m_childrenAxes[i].GetComponent<AxisRenderer>();
            //stop at the first dynamic axis as th
            if (axis.m_type == AxisRenderer.AxisType.DYNAMIC_SNAPPED ||
                axis.m_type == AxisRenderer.AxisType.DYNAMIC_UNSNAPPED)
            {
                return axis;
            }
        }

        return null;
    }

    /**
     * Simple animation with circle scaling up and fading out when an axis snaps
     * **/
    //public void LaunchSnapCircleAnimation(Vector2 position)
    //{
    //    GameObject clonedCircle = (GameObject)Instantiate(m_circlePfb);
    //    clonedCircle.transform.localPosition = GeometryUtils.BuildVector3FromVector2(position, -10);
    //    CircleMesh circleMesh = clonedCircle.GetComponent<CircleMesh>();
    //    circleMesh.Init(m_circleMaterial);

    //    CircleMeshAnimator circleAnimator = clonedCircle.GetComponent<CircleMeshAnimator>();
    //    circleAnimator.SetNumSegments(64, false);
    //    circleAnimator.SetInnerRadius(0, false);
    //    circleAnimator.SetOuterRadius(2, true);
    //    circleAnimator.SetColor(Color.white);

    //    circleAnimator.AnimateInnerRadiusTo(20, 0.3f, 0.0f, ValueAnimator.InterpolationType.LINEAR);
    //    circleAnimator.AnimateOuterRadiusTo(22, 0.3f, 0.0f, ValueAnimator.InterpolationType.LINEAR);
    //    circleAnimator.SetOpacity(1);
    //    circleAnimator.FadeTo(0, 0.3f, 0, ValueAnimator.InterpolationType.LINEAR, true);
    //}

    /**
     * Dismiss the axes holder (when a level ends for instance)
     * **/
    public void Dismiss(float fDuration = 0.5f, float fDelay = 0.0f, bool bDestroyOnFinish = true)
    {
        GameObjectAnimator axesAnimator = this.GetComponent<GameObjectAnimator>();
        axesAnimator.FadeTo(0, fDuration, fDelay, ValueAnimator.InterpolationType.LINEAR, bDestroyOnFinish);
    }
}
