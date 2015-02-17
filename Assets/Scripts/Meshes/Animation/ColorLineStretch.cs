using UnityEngine;

public class ColorLineStretch : MonoBehaviour
{
    private float m_duration;
    private float m_delay;
    private float m_diffLength;

    public Vector2 m_anchorPoint { get; set; }

    public void Awake()
    {
        m_anchorPoint = new Vector2(0.5f, 0.5f);
    }

    public void StretchToLength(float fLength, float fDuration, float fDelay = 0.0f)
    {
        m_duration = fDuration;
        m_delay = fDelay;
        float fStartLength = this.gameObject.transform.localScale.x;
        m_diffLength = fLength - fStartLength;
    }

    public void Update()
    {
        float dt = Time.deltaTime;

        float dLength = dt / m_duration * m_diffLength;
    

    }
}

