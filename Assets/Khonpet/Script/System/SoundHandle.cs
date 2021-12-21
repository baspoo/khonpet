using UnityEngine;

public class SoundHandle : UnityEngine.MonoBehaviour
{
    AudioSource[] AudioSources = null;
    void OnEnable()
    {
        OnUpdate();
    }
    public void OnUpdate()
    {
        if (AudioSources == null)
            AudioSources = gameObject.GetComponents<AudioSource>();
        foreach (var audio in AudioSources)
            audio.enabled = Playing.instance.playingData.IsSfx;
    }
    public static void RefreshAll()
    {
        foreach (var soundHandle in FindObjectsOfType<SoundHandle>())
            soundHandle.OnUpdate();
    }
}
