using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talking : MonoBehaviour
{
    public static Talking instance { get { if (m_instance == null) m_instance = FindObjectOfType<Talking>(); return m_instance; } }
    static Talking m_instance;



    Coroutine StartWait(float time, System.Action done) { return StartCoroutine(Wait(time,done)); }
    void StopWait(Coroutine coro) 
    {
        if (coro != null)
            StopCoroutine(coro);
    }
    IEnumerator Wait(float time, System.Action done)
    {
        yield return new WaitForSeconds(time);
        done();
    }
















    public Bubble bubble;
    [System.Serializable]
    public class Bubble 
    {
        public bool IsTalking => m_IsTalking;
        bool m_IsTalking;

        public EmoType emoType => m_emoType;
        EmoType m_emoType;

        Coroutine coro;
        public enum EmoType { FeelingSuper, FeelingHappy, FeelingNormal, FeelingBad ,Eating, Sleep, Love, Full , Boring }
        [System.Serializable]
        public class EmoData
        {
            public EmoType Type;
            public Transform Trans;
            public Sprite Sprite;
            public float Scale;
            public AudioClip sound;
        }

        public Transform Talk;
        public List<EmoData> EmoDatas;
        //public Transform TalkEating;
        //public Transform TalkSleep;
        public SpriteRenderer TalkIcon;
        void Show()
        {
            Hide();
            Talk.position = PetObj.Current.body.talk.position;
            Talk.gameObject.SetActive(true);
            m_IsTalking = true;
        }
        public void Hide(float wait = 0.0f)
        {
            void action() 
            {

                instance.StopWait(coro);
                Talk.gameObject.SetActive(false);
                TalkIcon.gameObject.SetActive(false);
                EmoDatas.ForEach(x => {
                    if(x.Trans!=null)
                        x.Trans.gameObject.SetActive(false);
                });
                m_IsTalking = false;
            }
            if (wait == 0.0f)
            {
                action();
            }
            else 
            {
                coro = instance.StartWait(wait,()=> { action(); });
            }
        }
        public void OnEmo(EmoType emo, float duration = 0.0f)
        {

            if (IsTalking)
                if (m_emoType == emo)
                    return;

            Show();
            m_emoType = emo;
            var data = EmoDatas.Find(x => x.Type == emo);
            if (data.Trans != null)
            {
                data.Trans.gameObject.SetActive(true);
            }
            else 
            {
                TalkIcon.gameObject.SetActive(true);
                TalkIcon.transform.localScale = Vector3.one * data.Scale;
                TalkIcon.sprite = data.Sprite;
            }
            data.sound?.Play();
            if (duration != 0.0f)
                Hide(duration);
        }
    }



    public PetTalk petTalk;
    [System.Serializable]
    public class PetTalk
    {
        public Transform Talk;
        public Animation anim_message;
        public Animation anim_header;
        public UnityEngine.UI.Text Message;
        public UnityEngine.UI.Text Header;
        public float Duration;
        Coroutine coro;
        public void ShowText(string message)
        {
            anim_message.gameObject.SetActive(true);
            anim_header.gameObject.SetActive(false);

            Message.text = message;
            Talk.gameObject.SetActive(true);

            anim_message.Stop();
            anim_message.Play(anim_message.clip.name);

            instance.StopWait(coro);
            coro = instance.StartWait(Duration, () => {
                Hide();
            });
        }

        public enum HeaderType { goodjob, great, bad }
        public void ShowHeader(HeaderType message) {
            switch (message)
            {
                case HeaderType.goodjob:
                    ShowHeader("goodjob!");
                    break;
                case HeaderType.great:
                    ShowHeader("great!");
                    break;
                case HeaderType.bad:
                    ShowHeader("bad!");
                    break;
                default:
                    break;
            }
        }
        public void ShowHeader(string message)
        {
            anim_message.gameObject.SetActive(false);
            anim_header.gameObject.SetActive(true);

            Header.text = message;
            Talk.gameObject.SetActive(true);

            anim_header.Stop();
            anim_header.Play(anim_header.clip.name);

            instance.StopWait(coro);
            coro = instance.StartWait(2.0f, () => {
                Hide();
            });
        }
        public void Hide()
        {
            instance.StopWait(coro);
            Talk.gameObject.SetActive(false);
        }
    }
    public void ForceHidePetTalk() 
    {
        petTalk.Hide();
    }









    //public Message message;
    //[System.Serializable]
    //public class Message
    //{
    //    public Transform Talk;
    //    public SpriteRenderer TalkIcon;
    //    public Animation anim;
    //    public Sprite GoodJob;
    //    public Sprite Great;
    //    public Sprite Bad;
    //    public enum MessageType { goodjob, great, bad}
    //    public void Show(MessageType type)
    //    {
    //        switch (type)
    //        {
    //            case MessageType.goodjob:
    //                TalkIcon.sprite = GoodJob;
    //                break;
    //            case MessageType.great:
    //                TalkIcon.sprite = Great;
    //                break;
    //            case MessageType.bad:
    //                TalkIcon.sprite = Bad;
    //                break;
    //        }
    //        Talk.gameObject.SetActive(true);
    //        anim.Stop();
    //        anim.Play(anim.clip.name);

    //        instance.StopWait();
    //        instance.StartWait(1.65f, () => {
    //            Hide();
    //        });
    //    }
    //    public void Hide()
    //    {
    //        instance.StopWait();
    //        Talk.gameObject.SetActive(false);
    //    }
    //}









}
