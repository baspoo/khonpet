using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using SimpleFirebaseUnity;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class FirebaseService : MonoBehaviour
{

    public static FirebaseService instance { get { if (m_instance == null) m_instance = FindObjectOfType<FirebaseService>(); return m_instance; } }
    static FirebaseService m_instance;


    Pet pet;
    [System.Serializable]
    public class Pet
    {
        public Status status = new Status();
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



    public PresetData Preset;
    [System.Serializable]
    public class PresetData
    {
        public long Like => ((instance.pet.status.like!=null) ? instance.pet.status.like.Sum(x => x.Value) : 0) + ClientLike;
        public long ClientLike;
        

        public long Star => ((instance.pet.status.star != null) ? instance.pet.status.star.Sum(x => x.Value) : 0) + ClientStar;
        public long ClientStar;

        public Dictionary<string, Pet.Chat> chats => instance.pet.chats;



    }





    public string petID;
    long time;
    public string userID => SystemInfo.deviceUniqueIdentifier;
    public bool IsDone { get; private set; }
    public System.Action<Dictionary<string, Pet.Chat>> onChatUpdate;

    Firebase firebase;
    FirebaseQueue firebaseQueue;
    Utility.TimeServer timeserver = new Utility.TimeServer();
    System.Action done;
    public void Init(string petID, System.Action done)
    {
        this.petID = petID;
        this.done = done;
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {



        // Create a FirebaseQueue
        firebaseQueue = new FirebaseQueue(true, 3, 1f);
        firebase = Firebase.CreateNew($"{"https://"}{"khonpet-default-rtdb"}.firebaseio.com/{"pets"}/{petID}", "AIzaSyBbUqrPWBZ7PSNjqnbEViSENFMaBU6-uYs");



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
            pet.chats = JsonConvert.DeserializeObject<Dictionary<string, Pet.Chat>>(snapshot.RawJson);
            onChatUpdate?.Invoke(pet.chats);
        };
        observer.Start();




        bool timeDone = false;
        GetTime(() => { timeDone = true; });
        while (!timeDone) yield return new WaitForEndOfFrame();



        bool petDone = false;
        GetPet((pet) => { this.pet = pet; petDone = true; });
        while (!petDone) yield return new WaitForEndOfFrame();



        ChatVerify(pet);



        IsDone = true;
        done?.Invoke();
    }





    public void GetTime(System.Action done)
    {
        //** GetTime **
        Debug.Log("GetTime");
        Firebase lastUpdate = firebase.Child("time", true);
        lastUpdate.OnSetSuccess = (sender, snap) =>
        {
            long timeStamp = snap.Value<long>();
            var dateTime = Firebase.TimeStampToDateTime(timeStamp);
            time = timeStamp;
            Debug.Log($"[OK-GetTime] Raw Json: {snap.RawJson}");
            Debug.Log($"[OK-GetTime] Datetime: {dateTime}");
            timeserver.Init(dateTime);
            done?.Invoke();
        };
        lastUpdate.SetValue(Firebase.SERVER_VALUE_TIMESTAMP, true);
    }


    public class XX
    {
        public Status status;
        public long time;
        [System.Serializable]
        public class Status
        {
            public string token;
            public long lastupdate;
            //public Dictionary<int, long> like = new Dictionary<int, long>();
            //public Dictionary<int, long> star = new Dictionary<int, long>();
        }
    }
    public void GetPet(System.Action<Pet> done)
    {
        //** GetPet **
        firebase.OnGetSuccess = (sender, snapshot) => {
            Debug.Log($"[OK-GetPet] Raw Json: " + snapshot.RawJson);
            pet = JsonConvert.DeserializeObject<Pet>(snapshot.RawJson);
            ChatVerify(pet);
            done?.Invoke(pet);
        };
        firebase.GetValue();

    }


    void ChatVerify(Pet pet)
    {
        int max = 50;
        Utility.TimeServer dif = new Utility.TimeServer();
        foreach (var chat in new Dictionary<string, Pet.Chat>(pet.chats).OrderBy(x=>x.Value.date))
        {
            if (max >= 0)
            {
                max--;
            }
            else 
            {
                pet.chats.Remove(chat.Key);
            }
        }
        Firebase updatechat = firebase.Child("chats", true);
        updatechat.UpdateValue(pet.chats.SerializeToJson());
    }





    public enum ValueKey
    {
        star,
        like
    }


    public List<stock> stocks = new List<stock>();
    public class stock {
        public ValueKey key;
        public int plus;
    }

    public void AddValue(ValueKey key, int plus )
    {
        var send = new stock()
        {
            key = key,
            plus = plus,
        };

        if (stocks.Count == 0)
        {
            Debug.Log("SendValue");
            SendValue(send);
        }
        else 
        {
           
            stocks.Add(send);
            Debug.Log($"Stocks {stocks.Count}");
        }
    }
    void SendValue(stock send) {

        //** Add Value **
        var index = Random.RandomRange(1, 100);
        Firebase add = firebase.Child($"status/{send.key.ToString()}/k-{index}", false);
        add.OnGetSuccess = (sender, snap) =>
        {
            var val = snap.Value<long>();
            val = val + send.plus;
            add.SetValue(val, FirebaseParam.Empty.AccesToken(snap.Value<long>().ToString()));

            if (stocks.Count != 0)
            {
                SendValue(stocks[0]);
                stocks.RemoveAt(0);
            }
        };
        add.GetValue();
    }

    public enum ChatCode
    {
        none = 100,
        message = 101,
        star = 102,
        like = 103,
        lvup = 104,
        food = 105,
        play = 106,
        clean = 107
        
    }
    public void PushChat( string name , string message , ChatCode code )
    {

        Pet.Chat chat = new Pet.Chat();
        chat.date = timeserver.Unix;
        chat.code = (int)code;
        chat.name = name;
        chat.message = message;

        //** Push Chat **
        Firebase put = firebase.Child("chats", true);
        put.OnPushSuccess = (sender, snap) =>
        {
            Debug.Log($"[OK-PushChat]");
        };
        put.Push(chat.SerializeToJson(), true);
    }



    public void UpdatePet(Pet pet)
    {
        var json = ToJson(pet);
        firebase.UpdateValue(json);
    }





    string ToJson(object myObject)
    {
        return JsonConvert.SerializeObject(myObject, Newtonsoft.Json.Formatting.None,
         new JsonSerializerSettings
         {
             NullValueHandling = NullValueHandling.Ignore
         });
    }





    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("AddValue");
            AddValue(ValueKey.star, 1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            AddValue( ValueKey.star, 5);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            AddValue(ValueKey.star, 20);
        }

    }




}
