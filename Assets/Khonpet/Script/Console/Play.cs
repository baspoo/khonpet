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
        public BarObject btnBall;
        public BarObject btnMemory;
        public BarObject btnGuess;
        public BarObject btnDance;

        public void Open(bool open) 
        {
            tRoot.gameObject.SetActive(open);
            if (open)
            {

                var level = PetData.PetInspector.Lv;
                void Updatebtn(BarObject btn , PlayType type) 
                {
                    var data = Store.instance.FindPlay(type);
                    btn.Btn.interactable = data.IsActive(level.CurrentLevel);// (level.CurrentLevel >= data.Lv);

                    btn.Name.gameObject.SetActive(!btn.Btn.interactable);
                    btn.Count.gameObject.SetActive(btn.Btn.interactable);

                    btn.Name.text = $"Lv.{data.Lv}";
                    btn.Count.text = $"{Mathf.Abs(data.Energy)}";

                    if(btn.Btn.interactable)
                        btn.Btn.interactable = PetData.PetInspector.GetStat(Pet.StatType.Energy) >= (Mathf.Abs(data.Energy)) || Setting.instance.debug.isUnLimitPlay;

                }
                tPosition.position = MainmenuPage.instance.consoleZone.btnPlay.transform.position;
                Updatebtn(btnBall, PlayType.Ball);
                Updatebtn(btnMemory, PlayType.Memory);
                Updatebtn(btnGuess, PlayType.Guess);
                Updatebtn(btnDance, PlayType.Dance);
                
            }
        }
    }


    PlayType currentPlayType;
    System.Action<PlayType,PlayAction> m_action;
    public void OnPlay(System.Action<PlayType,PlayAction> action)
    {
        m_action = action;
        seletePopup.Open(true);
    }
    public void OnSelectPopup(string type)
    {
        seletePopup.Open(false);
        if (string.IsNullOrEmpty(type))
        {
            m_action?.Invoke(currentPlayType,PlayAction.None);
        }
        else 
        {
            m_action?.Invoke(currentPlayType, PlayAction.Playing );
            switch (type) 
            {
                case "Ball": Ball(); break;
                case "Memory": Memory(); break;
                case "Guess": Guess(); break;
                case "Dance": Dance(); break;
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
            m_action?.Invoke(PlayType.Ball, pass ? PlayAction.Win : PlayAction.Lose);
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
            m_action?.Invoke( PlayType.Memory , pass ? PlayAction.Win : PlayAction.Lose);
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
            m_action?.Invoke(PlayType.Guess, pass ? PlayAction.Win : PlayAction.Lose);
        });
    }

    DancePage dancePage;
    void Dance()
    {
        if (dancePage == null)
        {
            var data = Store.instance.FindPlay(PlayType.Dance);
            dancePage = data.Root.Create(Root).GetComponent<DancePage>();
        }
        dancePage.Init((pass) => {
            m_action?.Invoke(PlayType.Dance, pass ? PlayAction.Win : PlayAction.Lose);
        });
    }




}
