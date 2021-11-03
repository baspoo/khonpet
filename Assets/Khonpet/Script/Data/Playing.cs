using System.Collections;
using System.Collections.Generic;
using UnityEngine;





[System.Serializable]
public class PlayingData
{
    public string NickName;



}






public class Playing : MonoBehaviour
{
    public static Playing instance { get { if (m_instance == null) m_instance = FindObjectOfType<Playing>(); return m_instance; } }
    static Playing m_instance;
    public PlayingData playing;



    public void Save()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(playing);
        PlayerPrefs.SetString("playing", json);
    }
    public void Load()
    {
        var json = PlayerPrefs.GetString("playing");
        if (!string.IsNullOrEmpty(json)) playing = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayingData>(json);
        else playing = new PlayingData();
    }


}


