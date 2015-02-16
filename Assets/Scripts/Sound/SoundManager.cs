using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool m_soundActive { get; set; }
    public bool m_musicActive { get; set; }

    public void Awake()
    {
        m_soundActive = true;
        m_musicActive = true;
    }

    public void ToggleMusic()
    {
        m_musicActive = !m_musicActive;
    }

    public void ToggleSound()
    {
        m_soundActive = !m_soundActive;
    }
}
