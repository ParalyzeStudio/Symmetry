using UnityEngine;

/**
 * Button used in menus or in game when player has clicked a HUDButton such as options
 * **/
public class GUIInterfaceButton : GUIQuadButton
{
    public Material[] m_materials;

    public override bool OnPress()
    {
        return base.OnPress();
    }

    public override bool OnRelease()
    {
        return base.OnRelease();
    }

    public override void OnClick()
    {
        string tag = this.gameObject.tag;
        if (tag.Equals("OptionsButton"))
        {
            GUIManager guiManager = GameObject.FindGameObjectWithTag("GUIManager").GetComponent<GUIManager>();
            guiManager.ShowOptionsWindow();
        }
        else if (tag.Equals("CreditsButton"))
        {
            Debug.Log("OnClick Credits");
        }
        else if (tag.Equals("MusicButton"))
        {
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleMusic();

            MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            Material onMaterial = m_materials[0];
            Material offMaterial = m_materials[1];
            meshRenderer.material = soundManager.m_musicActive ? onMaterial : offMaterial;
        }
        else if (tag.Equals("SoundButton"))
        {
            SoundManager soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
            soundManager.ToggleSound();

            MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            Material onMaterial = m_materials[0];
            Material offMaterial = m_materials[1];
            meshRenderer.material = soundManager.m_soundActive ? onMaterial : offMaterial;
        }
    }
}
