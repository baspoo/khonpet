using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    static Sound instance { get { if (m_instance == null) m_instance = FindObjectOfType<Sound>(); return m_instance; } }
    static Sound m_instance;
    public AudioSource bgm;
    public AudioSource sfx;


    public static Playlist playlist => instance.m_playlist;
    public Playlist m_playlist;
    [System.Serializable]
    public class Playlist 
    {
        [Header("BGM")]
        public AudioClip bgm_main;
        public AudioClip bgm_dance;
        public AudioClip bgm_journey;
        [Header("SFX-Interface")]
        public AudioClip click;
        public AudioClip select;
        public AudioClip close;
        public AudioClip claim;
        public AudioClip openpage;
        [Header("SFX-base")]
        public AudioClip stardone;
        public AudioClip move1;
        public AudioClip move2;
        public AudioClip clean;
        public AudioClip yeahh;
        public AudioClip match;
        public AudioClip fail;
        public AudioClip bad;
        [Header("SFX-Play&Journey")]
        public AudioClip[] dance_bitz;
        public AudioClip[] journey_bitz;
        public AudioClip[] journey_cheer;
        public AudioClip journey_win;
        public AudioClip journey_miss;
    }








    public static void Init( )
    {
        PlayBgm(playlist.bgm_main);
    }
    public static void Play(AudioClip clip)
    {
        if(Playing.instance.playingData.IsSfx && clip != null)
            instance.sfx.PlayOneShot(clip);
    }
    public static void PlayBgm(AudioClip clip)
    {
        instance.bgm.Stop();
        if (Playing.instance.playingData.IsBgm) 
        {
            instance.bgm.clip = clip;
            instance.bgm.Play();
        }
    }




    


}
