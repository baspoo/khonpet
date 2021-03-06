using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Btn : MonoBehaviour
{

    public UnityEngine.UI.Button button => m_button;
    UnityEngine.UI.Button m_button;
   
    public Animation animation => m_animation;
    Animation m_animation;

    public SoundClick soundClick;
    public UnityEngine.UI.Image icon;
    public UnityEngine.UI.Image cooldown;
    public UnityEngine.UI.Text text;
    public Color[] color;
    public bool isToggle;
    public enum SoundClick 
    {
        none,click,select,close, claim
    }


    private void Awake()
    {
        m_button = GetComponent<UnityEngine.UI.Button>();
        m_animation = GetComponent<Animation>();
    }
    void Handle() 
    {
        if (soundClick != SoundClick.none)
        {
            if (soundClick == SoundClick.click) Sound.Play(Sound.playlist.click);
            if (soundClick == SoundClick.select) Sound.Play(Sound.playlist.select);
            if (soundClick == SoundClick.close) Sound.Play(Sound.playlist.close);
            if (soundClick == SoundClick.claim) Sound.Play(Sound.playlist.claim);
        }
        if (m_animation != null)
        {
            m_animation.Stop();
            m_animation.Play(m_animation.clip.name);
        }
    }




    System.Action<bool> onToggle;
    public void InitToggle(System.Action<bool> onToggle , bool start = false)
    {
        this.onToggle = onToggle;
        this.isToggle = start;
        icon.enabled = this.isToggle;
    }
    public void PickToggle()
    {
        OnToggle();
    }
    public bool OnToggle()
    {
        this.isToggle = !this.isToggle;
        icon.enabled = this.isToggle;
        onToggle?.Invoke(this.isToggle);
        return this.isToggle;
    }




    public void OnBtn()
    {
        Handle();
    }
    public void OpenURL(string path)
    {
        Handle();
        Utility.Web.GotoUrl(path);
    }
    public void OpenURL(GameObject path)
    {
        Handle();
        Utility.Web.GotoUrl(path.name);
    }







    public void StartCooldown( )
    {
        StartCoroutine(IEcoodown());
    }
    IEnumerator IEcoodown() {
        button.interactable = false;
        cooldown.enabled = true;
        float percent = 1.0f;
        while (percent > 0.0f) 
        {
            yield return new WaitForEndOfFrame();
            percent -= Time.deltaTime * 0.05f;
            cooldown.fillAmount = percent;
        }
        yield return new WaitForEndOfFrame();
        button.interactable = true;
        cooldown.enabled = false;
    }
    

}
