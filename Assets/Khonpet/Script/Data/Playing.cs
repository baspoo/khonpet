using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif




[System.Serializable]
public class PlayingData
{
    public static PlayingData Current => Playing.instance.playing;

    public string NickName;
    public string UserID;
    public long UnixCreatedAt;
    public long UnixLastUpdate;

    public List<PetPlaying> PetPlayings;
    [System.Serializable]
    public class PetPlaying
    {
        public string PetID;
    }
}






public static class PlayingDataTools
{
    public static void AddFood( this PlayingData.PetPlaying petplaying , int value )
    {

    }
}

















































public class Playing : MonoBehaviour
{
    public static Playing instance { get { if (m_instance == null) m_instance = FindObjectOfType<Playing>(); return m_instance; } }
    static Playing m_instance;

    public PlayingData playing => m_playing;
    public PlayingData.PetPlaying petplaying => m_petplaying;

    [SerializeField]
    PlayingData m_playing;
    PlayingData.PetPlaying m_petplaying;


    const string m_key = "playing";

    public void Init()
    {
        // load playing
        Load();

        // get pet
        m_petplaying = m_playing.PetPlayings.Find(x=>x.PetID == PetData.Current.ID);
        if (m_petplaying == null) 
        {
            // new pet
            m_petplaying = new PlayingData.PetPlaying() { PetID = PetData.Current.ID };
            m_playing.PetPlayings.Add(m_petplaying);
        }
    }
    public void Save()
    {
        m_playing.UnixLastUpdate = Utility.TimeServer.DateTimeToUnixTimeStamp(System.DateTime.Now);
        if (m_playing.UnixCreatedAt == 0)
            m_playing.UnixCreatedAt = m_playing.UnixLastUpdate;
        if (!m_playing.UserID.notnull())
            m_playing.UserID = $"{SystemInfo.deviceName}-{SystemInfo.deviceUniqueIdentifier}-{m_playing.UnixCreatedAt}-{Random.RandomRange(11111,99999)}";
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(m_playing);
        PlayerPrefs.SetString(m_key, json);
    }
    public void Load()
    {
        m_playing = GetLocalStore();
    }
    public PlayingData GetLocalStore()
    {
        var json = PlayerPrefs.GetString(m_key);
        if (!string.IsNullOrEmpty(json)) return Newtonsoft.Json.JsonConvert.DeserializeObject<PlayingData>(json);
        else return new PlayingData();
    }
    public void Clear()
    {
        m_playing = new PlayingData();
        PlayerPrefs.DeleteKey(m_key);
    }
}

























#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(Playing))]
[System.Serializable]
public class PlayingUI : Editor
{


    Playing plaging => (Playing)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(30);
        if (GUILayout.Button("Load"))
        {
            plaging.Load();
        }
        if (GUILayout.Button("Save"))
        {
            var local = plaging.GetLocalStore();
            if (local.UnixLastUpdate == plaging.playing.UnixLastUpdate)
                plaging.Save();
            else
                Debug.LogError("Can't Save This Playing Data Is <color=red>Not Update.</color>");
        }
        if (GUILayout.Button("Clear"))
        {
            plaging.Clear();
        }
    }
}
#endif