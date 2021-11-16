using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talking : MonoBehaviour
{
    public static Talking instance { get { if (m_instance == null) m_instance = FindObjectOfType<Talking>(); return m_instance; } }
    static Talking m_instance;



    public Bubble bubble;
    [System.Serializable]
    public class Bubble 
    {
        public Transform Talk;
        public Transform TalkEating;
        public Transform TalkSleep;
        public SpriteRenderer TalkIcon;
        void Show()
        {
            Hide();
            Talk.position = PetObj.Current.body.talk.position;
            Talk.gameObject.SetActive(true);
        }
        public void Hide()
        {
            Talk.gameObject.SetActive(false);
            TalkEating.gameObject.SetActive(false);
            TalkSleep.gameObject.SetActive(false);
            TalkIcon.gameObject.SetActive(false);
        }
        public void OnIcon(Sprite spr)
        {
            Show();
            TalkIcon.sprite = spr;
            TalkIcon.gameObject.SetActive(true);
        }
        public void OnEatting()
        {
            Show();
            TalkEating.gameObject.SetActive(true);
        }
        public void OnSleep()
        {
            Show();
            TalkSleep.gameObject.SetActive(true);
        }
    }





    public Message message;
    [System.Serializable]
    public class Message
    {
        public Transform Talk;
        public SpriteRenderer TalkIcon;
        public Animation anim;
        public Sprite GoodJob;
        public Sprite Great;
        public Sprite Bad;
        public enum MessageType { goodjob, great, bad}
        public void Show(MessageType type)
        {
            switch (type)
            {
                case MessageType.goodjob:
                    TalkIcon.sprite = GoodJob;
                    break;
                case MessageType.great:
                    TalkIcon.sprite = Great;
                    break;
                case MessageType.bad:
                    TalkIcon.sprite = Bad;
                    break;
            }
            Talk.gameObject.SetActive(true);
            anim.Stop();
            anim.Play(anim.clip.name);
        }
        public void Hide()
        {
            Talk.gameObject.SetActive(false);
        }
    }









}
