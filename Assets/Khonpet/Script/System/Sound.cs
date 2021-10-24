using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    static Sound instance { get { if (m_instance == null) m_instance = FindObjectOfType<Sound>(); return m_instance; } }
    static Sound m_instance;
    public AudioSource audiosource;



    public static Playlist playlist => instance.m_playlist;
    public Playlist m_playlist;
    [System.Serializable]
    public class Playlist 
    {
        public AudioClip bgm_main;
        public AudioClip bgm_dance;
        public AudioClip click;
        public AudioClip select;
        public AudioClip close;
    }
   


    






    public static void Play(AudioClip clip)
    {
        if(clip!=null)
            instance.audiosource.PlayOneShot(clip);
    }
    public static void PlayBgm(AudioClip clip)
    {
        instance.audiosource.clip = clip;
        instance.audiosource.Stop();
        instance.audiosource.Play();
    }




    


}
