using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Chat : MonoBehaviour
{
    public static Chat instance { get { if (m_instance == null) m_instance = FindObjectOfType<Chat>(); return m_instance; } }
    static Chat m_instance;


    public enum ChatCode
    {
        none = 100,
        message = 101,
        star = 102,
        like = 103,
        lvup = 104,
        petting = 105,
        food = 106,
        highscore = 107
    }



    public Transform root;
    public Transform display;
    public UnityEngine.UI.Image icon;
    public UnityEngine.UI.Button btn;
    public Animation anim;
    public List<BarObject> chatBars;


    public void Init()
    {
        m_ChatDatas = new List<ChatData>();
        max = chatBars.Count;
        foreach (var c in chatBars)
            c.gameObject.SetActive(false);

        if (Information.instance.IsMobile) 
        {
            isDisplay = false;
            root.gameObject.SetActive(false);
        }
        else
            root.gameObject.SetActive(true);

        //foreach (var c in FirebaseService.instance.Preset.chats) 
        //{
        //    AddLocal(c.Value.name, c.Value.message , (ChatCode)c.Value.code);
        //}
        ReciverMessage(FirebaseService.instance.Preset.chats);
        FirebaseService.instance.onChatUpdate = ReciverMessage;


        isDisplay = Playing.instance.playingData.IsMessage;
        MessageDisplay();

    }




    bool isDisplay = true;
    public void Open()
    {
        isDisplay = !isDisplay;
        Playing.instance.Message(isDisplay);
        MessageDisplay();
    }
    void MessageDisplay()
    {
        icon.color = isDisplay ? Color.white : btn.colors.disabledColor;
        display.gameObject.SetActive(isDisplay);
    }



    int max;
    List<ChatData> m_ChatDatas;
    public class ChatData 
    {
        public string Name;
        public string Message;
    }




    public string GetMessage(ChatCode code) 
    {
        string message = Language.Get($"broadcast_msg_{code}");
        return message;
    }




    public void AddLocal(string Name,string message)
    {
        Add(Name, message, ChatCode.none);
    }
    public void AddLocal(string Name, string message, ChatCode code)
    {
       if(code == ChatCode.message) Add(Name, message , ChatCode.none);
       else Add(Name, GetMessage(code), ChatCode.none);
    }



    public void Add(ChatCode code) 
    {
        Add(Playing.instance.playingData.NickName, GetMessage(code) , code);
    }
    public void Add(string Name,string Message, ChatCode Code)
    {
        if (Code == ChatCode.none)
        {
            m_ChatDatas.Add(new ChatData()
            {
                Name = Name,
                Message = Message
            });
            Refresh();
        }
        else 
        {
            if (Code == ChatCode.message)
            {
                FirebaseService.instance.PushChat(Name, Message, Code);
            }
            else 
            {
                FirebaseService.instance.PushChat(Name, string.Empty, Code);
            }
        }
    }









    void Refresh() 
    {


        if (m_ChatDatas.Count > max)
        {
            m_ChatDatas.RemoveAt(0);
        }

        int index = 0;
        for (int i = (m_ChatDatas.Count-1); i >= 0; i--) 
        {
            var data = m_ChatDatas[i];
            var c = chatBars[index];
            c.gameObject.SetActive(true);
            c.Name.text = data.Name;
            c.Content.text = data.Message;
            index++;
        }


        if (!isDisplay)
            return;
        anim.Stop();
        anim.Play("chat");

    }









    int maxStock = 25;
    public List<string> messageStocks;
    void ReciverMessage(Dictionary<string, FirebaseService.Pet.Chat> chats) {
        foreach (var chat in chats.OrderBy(x=>x.Value.date)) 
        {
            if (!messageStocks.Contains(chat.Key))
            {
                AddLocal(chat.Value.name, chat.Value.message, (ChatCode)chat.Value.code);
                messageStocks.Add(chat.Key);
            }
        }
        if (messageStocks.Count > maxStock) 
        {
            (messageStocks.Count - maxStock).Loop(i=> {
                messageStocks.RemoveAt(0);
            });
        }
    }













    int m_d;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            Add("BaspooNE", Random.RandomRange(11111,99999).ToString(), ChatCode.message);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_d++;
            Add("BaspooCO", m_d.ToString(), ChatCode.message);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            FirebaseService.instance.ChatVerify();
        }
#endif
    }

}
