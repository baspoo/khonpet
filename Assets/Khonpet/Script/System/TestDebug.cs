using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class TestDebug : MonoBehaviour
{
    [TextArea]
    public string json;



    public class Pet
    {
        public Status status;
        public long time;
        [System.Serializable]
        public class Status
        {
            public string token;
            public long lastupdate;
            public Dictionary<string, long> like = new Dictionary<string, long>();
            public Dictionary<string, long> star = new Dictionary<string, long>();
        }

        public Dictionary<string, Chat> chats = new Dictionary<string, Chat>();
        [System.Serializable]
        public class Chat
        {
            public long date;
            public string name;
            public string message;
            public int code;
        }
    }

    void Start()
    {
        //var g = JsonConvert.DeserializeObject<Pet>(json);
        Debug.Log(100.KiloFormat());
        Debug.Log(1000.KiloFormat());
        Debug.Log(10000.KiloFormat());
        Debug.Log(100000.KiloFormat());
        Debug.Log(1000000.KiloFormat());
        Debug.Log(10000000.KiloFormat());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
