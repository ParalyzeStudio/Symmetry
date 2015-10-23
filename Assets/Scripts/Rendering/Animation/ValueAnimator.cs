﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ValueAnimator : MonoBehaviour 
{
    public enum InterpolationType
    {
        LINEAR = 1,
        SINUSOIDAL = 2,
        CUSTOM = 3 //use this for custom interpolation, but we have to redefine the UpdatePosition method with the appropriate interpolation function
    }

    public enum ColorMode
    {
        RGB = 1,
        TSB = 2
    }

    private List<ValueAnimator> m_childAnimators; 

    //Variables to handle fading
    protected bool m_fading;
    public float m_opacity;
    protected float m_fromOpacity;
    protected float m_toOpacity;
    protected float m_fadingDuration;
    protected float m_fadingDelay;
    protected float m_fadingElapsedTime;
    protected InterpolationType m_fadingInterpolationType;
    protected bool m_destroyObjectOnFinishFading;

    //Variables to handle scaling
    protected bool m_scaling;
    protected Vector3 m_scale;
    protected Vector3 m_fromScale;
    protected Vector3 m_toScale;
    protected float m_scalingDuration;
    protected float m_scalingDelay;
    protected float m_scalingElapsedTime;
    protected InterpolationType m_scalingInterpolationType;

    //Variables to handle translating
    protected bool m_translating;
    protected Vector3 m_position;
    protected Vector3 m_fromPosition;
    protected Vector3 m_toPosition;
    protected Vector3 m_translationDirection;
    protected float m_translationLength;
    protected float m_translatingDuration;
    protected float m_translatingDelay;
    protected float m_translatingElapsedTime;
    protected InterpolationType m_translatingInterpolationType;
    protected bool m_destroyObjectOnFinishTranslating;

    //Variables to handle rotating
    protected bool m_rotating;
    protected Vector3 m_rotationAxis;
    protected float m_angle;
    protected float m_fromAngle;
    protected float m_toAngle;
    protected float m_rotatingDuration;
    protected float m_rotatingDelay;
    protected float m_rotatingElapsedTime;
    protected InterpolationType m_rotatingInterpolationType;

    //Variables to handle color variation
    protected bool m_colorChanging;
    public Color m_color;
    protected Color m_fromColor;
    protected Color m_toColor;
    protected float m_colorChangingDuration;
    protected float m_colorChangingDelay;
    protected float m_colorChangingElapsedTime;
    protected InterpolationType m_colorChangingInterpolationType;

    //DEBUG Variables to handle tint, saturation and brightness
    public Vector3 m_TSB;
    private Vector3 m_prevTSB;

    //Store previous values to change them dynamically in inspector
    protected float m_prevOpacity;
    protected Color m_prevColor;
    protected ColorMode m_prevColorMode;

    //Global instances to prevent calls to FindGameObjectWithTag and GetComponent<>
    protected GUIManager m_guiManager;
    protected BackgroundTrianglesRenderer m_backgroundRenderer;
    protected LevelManager m_levelManager;
    protected SceneManager m_sceneManager;

    public virtual void Awake()
    {
        m_opacity = 1;
        m_prevOpacity = 1;
        m_color = Color.black;
        m_prevColor = m_color;
    }


    private void CreateChildAnimatorsListIfNull()
    {
        if (m_childAnimators == null)
            m_childAnimators = new List<ValueAnimator>();
    }

    public void AddChildAnimator(ValueAnimator animator)
    {
        CreateChildAnimatorsListIfNull();
        m_childAnimators.Add(animator);
    }

    public void RemoveChildAnimator(ValueAnimator animator)
    {
        if (m_childAnimators != null)
            m_childAnimators.Remove(animator);
    }

    /**
     * Set this animator and the related GameObject as a child of the object passed as parameter
     * **/
    public void SetParentTransform(Transform parentTransform)
    {
        this.gameObject.transform.parent = parentTransform;
        ValueAnimator parentAnimator = parentTransform.gameObject.GetComponent<ValueAnimator>();
        if (parentAnimator != null)
            parentAnimator.AddChildAnimator(this);
    }

    /**
     * Remove this animator from the list of its parent chid animators
     * **/
    public void DetachFromParent()
    {
        Transform parentTransform = this.gameObject.transform.parent;
        ValueAnimator parentAnimator = parentTransform.gameObject.GetComponent<ValueAnimator>();
        if (parentAnimator != null)
            parentAnimator.RemoveChildAnimator(this);
    }

    public void FadeTo(float toOpacity, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyObjectOnFinish = false)
    {
        if (m_opacity == toOpacity)
            return;

        m_fading = true;
        m_fromOpacity = m_opacity;
        m_toOpacity = toOpacity;
        m_fadingDuration = duration;
        m_fadingDelay = delay;
        m_fadingElapsedTime = 0;
        m_fadingInterpolationType = interpolType;
        m_destroyObjectOnFinishFading = bDestroyObjectOnFinish;
    }

    public void ScaleTo(Vector3 toScale, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_scaling = true;
        m_fromScale = m_scale;
        m_toScale = toScale;
        m_scalingDuration = duration;
        m_scalingDelay = delay;
        m_scalingElapsedTime = 0;
        m_scalingInterpolationType = interpolType;
    }

    public void TranslateTo(Vector3 toPosition, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR, bool bDestroyOnFinish = false)
    {
        m_translating = true;
        m_fromPosition = m_position;
        m_toPosition = toPosition;
        m_translationLength = (m_toPosition - m_fromPosition).magnitude;
        m_translationDirection = (m_toPosition - m_fromPosition);
        m_translationDirection /= m_translationLength;        
        m_translatingDuration = duration;
        m_translatingDelay = delay;
        m_translatingElapsedTime = 0;
        m_translatingInterpolationType = interpolType;
        m_destroyObjectOnFinishTranslating = bDestroyOnFinish;
    }

    public void RotateTo(float toAngle, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_rotating = true;
        m_fromAngle = m_angle;
        m_toAngle = toAngle;
        m_rotatingDuration = duration;
        m_rotatingDelay = delay;
        m_rotatingElapsedTime = 0;
        m_rotatingInterpolationType = interpolType;
    }

    public void ColorChangeTo(Color toColor, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_colorChanging = true;
        m_fromColor = m_color;
        m_toColor = toColor;
        m_colorChangingDuration = duration;
        m_colorChangingDelay = delay;
        m_colorChangingElapsedTime = 0;
        m_colorChangingInterpolationType = interpolType;
    }

    public void RotateToAroundAxis(float toAngle, Vector3 axis, float duration, float delay = 0.0f, InterpolationType interpolType = InterpolationType.LINEAR)
    {
        m_rotationAxis = axis;
        RotateTo(toAngle, duration, delay, interpolType);
    }

    public virtual void SetOpacity(float fOpacity, bool bPassOnChildren = true)
    {
        fOpacity = Mathf.Clamp(fOpacity, 0, 1);

        m_opacity = fOpacity;
        m_prevOpacity = fOpacity;
        m_color.a = fOpacity;
        m_prevColor.a = fOpacity;
        OnOpacityChanged();

        if (bPassOnChildren && m_childAnimators != null)
        {
            for (int i = 0; i != m_childAnimators.Count; i++)
            {
                ValueAnimator childAnimator = m_childAnimators[i];
                if (childAnimator != this)
                {
                    childAnimator.SetOpacity(fOpacity, false); //do not pass to this object's children because they are already in the list of childAnimators
                }
            }
        }
    }

    public virtual void SetScale(Vector3 scale)
    {
        m_scale = scale;
        OnScaleChanged();
    }

    public virtual void SetPosition(Vector3 position)
    {
        m_position = position;
        OnPositionChanged();
    }

    public virtual Vector3 GetPosition()
    {
        return m_position;
    }

    public virtual void SetRotationAngle(float angle)
    {
        m_angle = angle;
        OnRotationChanged();
    }

    public virtual void SetRotationAxis(Vector3 axis)
    {
        m_rotationAxis = axis;
    }

    public virtual void SetColor(Color color)
    {
        m_color = color;
        m_prevColor = color;
        m_opacity = color.a;
        m_prevOpacity = color.a;

        //clamp color channels values between 0 and 1
        //m_color.r = Mathf.Clamp(m_color.r, 0, 1);
        //m_color.g = Mathf.Clamp(m_color.g, 0, 1);
        //m_color.b = Mathf.Clamp(m_color.b, 0, 1);
        //m_color.a = Mathf.Clamp(m_color.a, 0, 1);

        OnColorChanged();
    }

    /**
     * Set the color channels 
     * **/
    public virtual void SetColorChannels(Vector3 channels, ColorMode colorMode = ColorMode.RGB)
    {
        if (colorMode == ColorMode.RGB)
            SetColor(new Color(channels.x, channels.y, channels.z, m_color.a));
        else if (colorMode == ColorMode.TSB)
            SetColor(ColorUtils.GetRGBAColorFromTSB(new Vector3(channels.x, channels.y, channels.z), m_color.a));
    }

    public virtual void SetTSB(Vector3 tsb)
    {
        m_TSB = tsb;        
        m_prevTSB = m_TSB;
        SetColorChannels(tsb, ColorMode.TSB);
    }

    public virtual void IncOpacity(float deltaOpacity)
    {
        float fOpacity = m_opacity + deltaOpacity;
        SetOpacity(fOpacity);
    }

    public virtual void IncScale(Vector3 deltaScale)
    {
        Vector3 fScale = m_scale + deltaScale;
        SetScale(fScale);
    }

    public virtual void IncPosition(Vector3 deltaPosition)
    {
        Vector3 fPosition = m_position + deltaPosition;
        SetPosition(fPosition);
    }

    public virtual void IncRotationAngle(float deltaAngle)
    {
        float fAngle = m_angle + deltaAngle;
        SetRotationAngle(fAngle);
    }

    public virtual void IncColor(Color deltaColor)
    {
        Color color = m_color + deltaColor;
        SetColor(color);
    }

    public virtual void OnOpacityChanged()
    {
        
    }

    public virtual void OnPositionChanged()
    {

    }

    public virtual void OnScaleChanged()
    {

    }

    public virtual void OnRotationChanged()
    {

    }

    public virtual void OnColorChanged()
    {

    }

    public virtual void OnFinishFading()
    {
        if (m_destroyObjectOnFinishFading)
        {
            OnPreDestroyObject();
            Destroy(this.gameObject);
        }
    }

    public virtual void OnFinishTranslating()
    {
        if (m_destroyObjectOnFinishTranslating)
        {
            OnPreDestroyObject();
            Destroy(this.gameObject);
        }
    }

    public virtual void OnFinishScaling()
    {

    }

    public virtual void OnFinishRotating()
    {

    }

    public virtual void OnFinishColorChanging()
    {

    }

    /**
     * Return the color (RGB channels + opacity) of this animator
     * **/
    //public Color GetFullColor()
    //{
    //    Color color = Color.black;
    //    if (m_colorMode == ColorMode.RGB)
    //        color = new Color(m_color.x, m_color.y, m_color.z, m_opacity);
    //    else if (m_colorMode == ColorMode.TSB)
    //        color = ColorUtils.GetRGBAColorFromTSB(m_color, m_opacity);

    //    return color;
    //}

    protected virtual void UpdateOpacity(float dt)
    {
        if (m_fading)
        {
            bool inDelay = (m_fadingElapsedTime < m_fadingDelay);
            m_fadingElapsedTime += dt;
            if (m_fadingElapsedTime >= m_fadingDelay)
            {
                if (inDelay) //we were in delay previously
                {
                    dt = m_fadingElapsedTime - m_fadingDelay;
                }
                float effectiveElapsedTime = m_fadingElapsedTime - m_fadingDelay;
                float deltaOpacity = 0;
                float opacityVariation = m_toOpacity - m_fromOpacity;
                if (m_fadingInterpolationType == InterpolationType.LINEAR)
                    deltaOpacity = dt / m_fadingDuration * opacityVariation;
                else if (m_fadingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaOpacity = opacityVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_fadingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_fadingDuration)));

                if (effectiveElapsedTime > m_fadingDuration)
                {
                    SetOpacity(m_toOpacity);
                    m_fading = false;
                    OnFinishFading();
                }
                else
                    IncOpacity(deltaOpacity);
            }
        }
    }

    protected virtual void UpdatePosition(float dt)
    {
        if (m_translating)
        {
            bool inDelay = (m_translatingElapsedTime < m_translatingDelay);
            m_translatingElapsedTime += dt;
            if (m_translatingElapsedTime >= m_translatingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_translatingElapsedTime - m_translatingDelay;
                float effectiveElapsedTime = m_translatingElapsedTime - m_translatingDelay;
                Vector3 deltaPosition = Vector3.zero;
                Vector3 positionVariation = m_toPosition - m_fromPosition;
                if (m_translatingInterpolationType == InterpolationType.LINEAR)
                    deltaPosition = dt / m_translatingDuration * positionVariation;
                else if (m_translatingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaPosition = positionVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_translatingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_translatingDuration)));

                if (effectiveElapsedTime > m_translatingDuration)
                {
                    SetPosition(m_toPosition);
                    m_translating = false;
                    OnFinishTranslating();
                }
                else
                    IncPosition(deltaPosition);
            }
        }
    }

    protected void UpdateRotation(float dt)
    {
        if (m_rotating)
        {
            bool inDelay = (m_rotatingElapsedTime < m_rotatingDelay);
            m_rotatingElapsedTime += dt;
            if (m_rotatingElapsedTime > m_rotatingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_rotatingElapsedTime - m_rotatingDelay;
                float effectiveElapsedTime = m_rotatingElapsedTime - m_rotatingDelay;
                float deltaAngle = 0;
                float angleVariation = m_toAngle - m_fromAngle;
                if (m_rotatingInterpolationType == InterpolationType.LINEAR)
                    deltaAngle = dt / m_rotatingDuration * angleVariation;
                else if (m_rotatingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaAngle = angleVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_rotatingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_rotatingDuration)));

                if (effectiveElapsedTime > m_rotatingDuration)
                {
                    SetRotationAngle(m_toAngle);
                    m_rotating = false;
                    OnFinishRotating();
                }
                else
                    IncRotationAngle(deltaAngle);
            }
        }
    }

    protected virtual void UpdateScale(float dt)
    {
        if (m_scaling)
        {
            bool inDelay = (m_scalingElapsedTime < m_scalingDelay);
            m_scalingElapsedTime += dt;
            if (m_scalingElapsedTime > m_scalingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_scalingElapsedTime - m_scalingDelay;
                float effectiveElapsedTime = m_scalingElapsedTime - m_scalingDelay;
                Vector3 deltaScale = Vector3.zero;
                Vector3 scaleVariation = m_toScale - m_fromScale;
                if (m_scalingInterpolationType == InterpolationType.LINEAR)
                    deltaScale = dt / m_scalingDuration * scaleVariation;
                else if (m_scalingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaScale = scaleVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_scalingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_scalingDuration)));

                if (effectiveElapsedTime > m_scalingDuration)
                {
                    SetScale(m_toScale);
                    m_scaling = false;
                    OnFinishScaling();
                }
                else
                    IncScale(deltaScale);
            }
        }
    }

    protected virtual void UpdateColor(float dt)
    {
        if (m_colorChanging)
        {
            bool inDelay = (m_colorChangingElapsedTime < m_colorChangingDelay);
            m_colorChangingElapsedTime += dt;
            if (m_colorChangingElapsedTime > m_colorChangingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_colorChangingElapsedTime - m_colorChangingDelay;
                float effectiveElapsedTime = m_colorChangingElapsedTime - m_colorChangingDelay;
                Vector4 deltaColor = Vector3.zero;
                Vector4 colorVariation = m_toColor - m_fromColor;
                if (m_colorChangingInterpolationType == InterpolationType.LINEAR)
                    deltaColor = dt / m_colorChangingDuration * colorVariation;
                else if (m_colorChangingInterpolationType == InterpolationType.SINUSOIDAL)
                    deltaColor = colorVariation * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / (2 * m_colorChangingDuration)) - Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / (2 * m_colorChangingDuration)));

                if (effectiveElapsedTime > m_colorChangingDuration)
                {
                    SetColor(m_toColor);
                    m_colorChanging = false;
                    OnFinishColorChanging();
                }
                else
                    IncColor(deltaColor);
            }
        }
    }

    protected virtual void Update()
    {
        float dt = Time.deltaTime;

        //when we modify the opacity value directly in the inspector in edit mode
        if (m_prevOpacity != m_opacity)
        {
            SetOpacity(m_opacity);
            return;
        }

        if (m_prevColor != m_color)
        {
            SetColor(m_color);
            return;
        }

        if (m_prevTSB != m_TSB)
        {
            SetTSB(m_TSB);
            return;
        }

        //update values that have to be modified through time
        UpdateOpacity(dt);
        UpdatePosition(dt);
        UpdateRotation(dt);
        UpdateScale(dt);
        UpdateColor(dt);
    }

    /**
     * Call this method before destroying an object
     * It will remove the animator of 'this' object from its parent child animator list
     * **/
    public void OnPreDestroyObject()
    {
        ValueAnimator parentAnimator = this.transform.parent.GetComponent<ValueAnimator>();
        if (parentAnimator != null)
            parentAnimator.RemoveChildAnimator(this);
    }

    /**
     * Getters for global instances
     * **/
    public GUIManager GetGUIManager()
    {
        if (m_guiManager == null)
            m_guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();

        return m_guiManager;
    }

    public BackgroundTrianglesRenderer GetBackgroundRenderer()
    {
        if (m_backgroundRenderer == null)
            m_backgroundRenderer = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<BackgroundTrianglesRenderer>();

        return m_backgroundRenderer;
    }

    public LevelManager GetLevelManager()
    {
        if (m_levelManager == null)
            m_levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

        return m_levelManager;
    }

    public SceneManager GetSceneManager()
    {
        if (m_sceneManager == null)
            m_sceneManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneManager>();

        return m_sceneManager;
    }
}