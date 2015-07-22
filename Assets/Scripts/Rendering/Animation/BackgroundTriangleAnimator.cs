using UnityEngine;

public class BackgroundTriangleAnimator : ValueAnimator
{
    //speed is expressed as v = a * exp(b*t) + c
    //so acceleration is simply acc = a * b * exp(b*t) 
    public float m_accelerationFactor { get; set; } //the factor b

    public override void IncPosition(Vector3 deltaPosition)
    {
        m_position += deltaPosition;
        GetBackgroundRenderer().Offset(deltaPosition.y);
    }

    public override void SetPosition(Vector3 position)
    {
        Vector3 deltaPosition = position - m_position;
       
        IncPosition(deltaPosition);
    }

    public void TranslateTo(Vector3 toPosition, float accelerationFactor, float duration, float delay = 0.0f)
    {
        base.TranslateTo(toPosition, duration, delay, InterpolationType.CUSTOM);
        m_accelerationFactor = accelerationFactor;
    }

    protected override void UpdatePosition(float dt)
    {
        if (m_translating)
        {
            bool inDelay = (m_translatingElapsedTime < m_translatingDelay);
            m_translatingElapsedTime += dt;
            if (m_translatingElapsedTime >= m_translatingDelay)
            {
                if (inDelay) //we were in delay previously
                    dt = m_translatingElapsedTime - m_translatingDelay;
                float t = m_translatingElapsedTime - m_translatingDelay;

                Vector3 deltaPosition;
                //Simplify variables for better reading
                float T = m_translatingDuration / 2.0f;
                float L = m_translationLength;                
                float b = m_accelerationFactor * 2.0f;
                float a = b * L / (2 * (Mathf.Exp(b * T) - 1 - b * T));
                if (t < T)
                {
                    float deltaPositionLength = (a / b) * (Mathf.Exp(b * t) - Mathf.Exp(b * (t - dt))) - a * dt;
                    deltaPosition = deltaPositionLength * m_translationDirection;
                }
                else
                {
                    float deltaPositionLength = -(a / b) * (Mathf.Exp(-b * (t - 2 * T)) - Mathf.Exp(-b * (t - dt - 2 * T))) - a * dt;
                    deltaPosition = deltaPositionLength * m_translationDirection;
                }

                if (t > m_translatingDuration)
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

    public override void OnFinishTranslating()
    {
        
    }
}
