using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    public static Play instance { get { if (m_instance == null) m_instance = FindObjectOfType<Play>(); return m_instance; } }
    static Play m_instance;

    public enum PlayType
    {
       None, Ball, Memory, Guess, Dance
    }
    public enum PlayAction
    {
        None, Playing ,Win, Lose
    }
    public Transform Root;
    public SeletePopup seletePopup;
    [System.Serializable]
    public class SeletePopup
    {
        public Transform tRoot;
        public Transform tPosition;
        public Btn btnBall;
        public Btn btnMemory;
        public Btn btnGuess;
        public Btn btnDance;

        public void Open(bool open) 
        {
            tRoot.gameObject.SetActive(open);
            if (open)
            {

                var level = PetData.Current.Lv;
                void Updatebtn(Btn btn , PlayType type) 
                {
                    var data = Store.instance.FindPlay(type);
                    btn.button.interactable = (level.CurrentLevel >= data.Lv);
                    if (btn.button.interactable)
                    {
                        btn.text.text = "";
                    }
                    else 
                    {
                        btn.text.text = $"Lv.{data.Lv}";
                    }
                }
                tPosition.position = MainmenuPage.instance.consoleZone.btnPlay.transform.position;
                Updatebtn(btnBall, PlayType.Ball);
                Updatebtn(btnMemory, PlayType.Memory);
                Updatebtn(btnGuess, PlayType.Guess);
                Updatebtn(btnDance, PlayType.Dance);
                
            }
        }
    }



    System.Action<PlayAction> m_action;
    public void OnPlay(System.Action<PlayAction> action)
    {
        m_action = action;
        seletePopup.Open(true);
    }
    public void OnSelectPopup(string type)
    {
        seletePopup.Open(false);
        if (string.IsNullOrEmpty(type))
        {
            m_action?.Invoke(PlayAction.None);
        }
        else 
        {
            m_action?.Invoke( PlayAction.Playing );
            switch (type) 
            {
                case "Ball": Ball(); break;
                case "Memory": Memory(); break;
                case "Guess": Guess(); break;
                case "Dance": break;
            }
        }
    }




    BallPage ballpage;
    void Ball() 
    {
        if (ballpage == null)
        {
            var data = Store.instance.FindPlay(PlayType.Ball);
            ballpage = data.Root.Create(Root).GetComponent<BallPage>();
        }
        ballpage.Init((pass)=> {
            m_action?.Invoke(pass? PlayAction.Win : PlayAction.Lose);
        });
    }

    MemoryPage mem;
    void Memory()
    {
        if (mem == null)
        {
            var data = Store.instance.FindPlay(PlayType.Memory);
            mem = data.Root.Create(Root).GetComponent<MemoryPage>();
        }
        mem.Init((pass) => {
            m_action?.Invoke(pass ? PlayAction.Win : PlayAction.Lose);
        });
    }

    QuickRandomPage quickRandomPage;
    void Guess()
    {
        if (quickRandomPage == null)
        {
            var data = Store.instance.FindPlay(PlayType.Guess);
            quickRandomPage = data.Root.Create(Root).GetComponent<QuickRandomPage>();
        }
        quickRandomPage.Init((pass) => {
            m_action?.Invoke(pass ? PlayAction.Win : PlayAction.Lose);
        });
    }
    void Dance()
    {

    }




}
