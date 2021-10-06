using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SimpleFirebaseUnity;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class FirebaseService : MonoBehaviour
{



    public Pet pet;
    [System.Serializable]
    public class Pet
    {
        public Status status;
        public long time;
        [System.Serializable]
        public class Status
        {
            public string token;
            public long lastupdate;
            public Dictionary<int, long> like = new Dictionary<int, long>();
            public Dictionary<int, long> star = new Dictionary<int, long>();
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



    public string petID;
    public string token;
    public long time;



    Firebase firebase;
    FirebaseQueue firebaseQueue;
    void Start()
    {

       

        // Create a FirebaseQueue
        firebaseQueue = new FirebaseQueue(true, 3, 1f);
        firebase = Firebase.CreateNew($"{"https://"}{"khonpet-default-rtdb"}.firebaseio.com/{"pets"}/{petID}", "AIzaSyBbUqrPWBZ7PSNjqnbEViSENFMaBU6-uYs");


        //Get
        firebase.OnGetSuccess = (sender, snapshot) => {
            Debug.Log($"[OK-OnGetSuccess] Raw Json: " + snapshot.RawJson);
            pet = JsonConvert.DeserializeObject<Pet>(snapshot.RawJson);
            Debug.Log($"{pet.time}");
        };
        firebase.OnGetFailed = (sender, error) => {
            Debug.LogError("[ERR-GetFailed ] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };
        //Set
        firebase.OnSetSuccess = (sender, snapshot) => {
            Debug.Log($"[OK-SetSuccess] {sender.FullKey} Raw Json: " + snapshot.RawJson);
        };
        firebase.OnSetFailed = (sender, error) => {
            Debug.LogError("[ERR-SetFailed] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };
        //Update
        firebase.OnUpdateSuccess = (sender, snapshot) => {
            Debug.Log($"[OK-UpdateSuccess] {sender.FullKey} Raw Json: " + snapshot.RawJson);
        };
        firebase.OnUpdateFailed = (sender, error) => {
            Debug.LogError("[ERR-UpdateFailed] Set from key: <" + sender.FullKey + ">, " + error.Message + " (" + (int)error.Status + ")");
        };





        FirebaseObserver observer = new FirebaseObserver(firebase.Child("chats", true), 1f);
        observer.OnChange += (Firebase sender, DataSnapshot snapshot) =>
        {
            Debug.Log($"[OBSERVER] Raw Json: " + snapshot.RawJson);
            //Debug.Log($"[OBSERVER] Raw Json: " + JsonConvert.SerializeObject(snapshot.RawValue));
        };
        observer.Start();


      
    }






    public static string userID => SystemInfo.deviceUniqueIdentifier;

    public string authtoken
    {
        get
        {
            var data = new
            {
                provider = "anonymous",
                uid = userID
            };
            var json = JsonUtility.ToJson(data);
            return json;
        }
    }



   


    string ToJson(object myObject)
    {
        return JsonConvert.SerializeObject(myObject, Newtonsoft.Json.Formatting.None,
         new JsonSerializerSettings
         {
             NullValueHandling = NullValueHandling.Ignore
         });
    }


    public string data,unix;
    Utility.TimeServer t = new Utility.TimeServer();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //** GetTime **
            Debug.Log("GetTime");
            // ".info/serverTimeOffset"
            Firebase lastUpdate = firebase.Child("time", true);
            lastUpdate.OnSetSuccess = (sender, snap) =>
            {
                long timeStamp = snap.Value<long>();
                var dateTime = Firebase.TimeStampToDateTime(timeStamp);
                time = timeStamp;
                Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson}");
                Debug.Log($"[OK-GetTime] Datetime: {dateTime}");


                t.Init(dateTime);

            };
            lastUpdate.SetValue(Firebase.SERVER_VALUE_TIMESTAMP, true);
        }


        data = t.Time.ToLongTimeString();
        unix = t.Unix.ToString();


     






        //** Add Value **
        void addvalue(string key , int plus ) 
        {
            var index = Random.RandomRange(0, 100);
            Firebase add = firebase.Child($"status/{key}/{index}", true);
            add.OnGetSuccess = (sender, snap) =>
            {
                var val = snap.Value<long>();
                Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson} : {val}");
                val = val + plus;
                add.SetValue(val, FirebaseParam.Empty.AccesToken(snap.Value<long>().ToString()));
            };
            add.GetValue();
        }


        if (Input.GetKeyDown(KeyCode.Z))
        {
            addvalue("like",1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            addvalue("like",-1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            addvalue("like", 100);
        }






        if (Input.GetKeyDown(KeyCode.G))
        {
            //** Get **
            firebase.GetValue(  FirebaseParam.Empty.Auth(authtoken) );


            //PublicData publicData = new PublicData();
            //publicData.publicToken = token;
            //var json = ToJson(publicData);
            //Debug.Log(json);
            //firebase.Child("publicData", true).UpdateValue(json, FirebaseParam.Empty.Auth(authtoken));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            //** Init Pet **


            var petData = new Pet();

            //time
            petData.time = time;

            //status
            petData.status = new Pet.Status();
            petData.status.token = token;

            //chats
            petData.chats = new Dictionary<string, Pet.Chat>();

            var json = ToJson(petData.chats);
            firebase.UpdateValue(json, FirebaseParam.Empty.Auth(authtoken));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //** Push Chat **

            Debug.Log("Put");
            Firebase put = firebase.Child("chats", true);
            put.OnPushSuccess = (sender, snap) =>
            {
                //Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson}");
            };
            put.Push(new Pet.Chat()
            {
                date = time,
                code = 101,
                message = "message test " + Random.RandomRange(1111, 9999).ToString(),
                name = "baspoo"

            }.SerializeToJson(), true);
        }






        if (Input.GetKeyDown(KeyCode.D))
        {
            //User userData = new User() { };
            //userData.profile = new User.Profile();
            //userData.uid = userID;
            //userData.token = token;
            //userData.profile.age = 123;
            //var json = ToJson(userData);
            //Debug.Log(json);
            //firebase.Child("userData", true).UpdateValue(json, FirebaseParam.Empty.Auth(authtoken));
        }






        if (Input.GetKeyDown(KeyCode.S))
        {
            //firebase.Child("userData", true).GetValue();
        }




    }




}
