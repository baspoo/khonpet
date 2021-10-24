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
    public enum SoundClick 
    {
        none,click,select,close
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
        }
        if (m_animation != null)
        {
            m_animation.Stop();
            m_animation.Play(m_animation.clip.name);
        }
    }
  










    public void OnBtn()
    {
        Handle();
    }
    public void OpenURL(string path)
    {
        Handle();
        Application.OpenURL(path);
    }
    public void OpenURL(GameObject path)
    {
        Handle();
        Application.OpenURL(path.name);
    }





 


   
    

}
