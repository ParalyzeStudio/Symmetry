using UnityEngine;

public class BackgroundTriangleAnimator : ValueAnimator
{
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
                float effectiveElapsedTime = m_translatingElapsedTime - m_translatingDelay;
                Vector3 deltaPosition = Vector3.zero;
                Vector3 positionVariation = m_toPosition - m_fromPosition;

                //sin(t*PI/T - PI/2)
                deltaPosition = positionVariation * 0.5f * (Mathf.Sin(effectiveElapsedTime * Mathf.PI / m_translatingDuration - Mathf.PI / 2.0f) -
                                                            Mathf.Sin((effectiveElapsedTime - dt) * Mathf.PI / m_translatingDuration - Mathf.PI / 2.0f));

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

    public override void IncPosition(Vector3 deltaPosition)
    {
        m_position += deltaPosition;
        BackgroundTrianglesRenderer renderer = this.gameObject.GetComponent<BackgroundTrianglesRenderer>();
        renderer.Offset(deltaPosition.y);
    }

    public override void SetPosition(Vector3 position)
    {
        Vector3 deltaPosition = position - m_position;
       
        IncPosition(deltaPosition);
    }

    public override void OnFinishTranslating()
    {
        
    }
}
