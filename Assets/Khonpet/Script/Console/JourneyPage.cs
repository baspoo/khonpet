using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyPage : MonoBehaviour
{
    public static JourneyPage instance { get { if (m_instance == null) m_instance = FindObjectOfType<JourneyPage>(); return m_instance; } }
    static JourneyPage m_instance;


    public Transform root;
    public Transform diraction;
    public List<BarObject> bars;
    public Transform t_Time;
    public Transform t_winlose;
    public Transform t_win;
    public Transform t_lose;
    public Awake effHit;
    public UnityEngine.UI.Image timing;
    public UnityEngine.UI.Image focus;
    public UnityEngine.UI.Text textscore;
    public UnityEngine.UI.Text textcombo;
    public UnityEngine.UI.Text textcomboshort;
    public Animation animScore;
    public Btn BtnA,BtnB;
    public Sprite imgBitzA;
    public Sprite imgBitzB;
    public Sprite imgBitzZero;
    public Color colorZero;
    float m_speed;
    System.Action<int> callback;




    void Start()
    {
 
    }


    public void Init()
    {
        m_instance = this;
        OnClose();
    }
    public void OnClose()
    {
        active = false;
        root.gameObject.SetActive(false);
        Sound.PlayBgm(Sound.playlist.bgm_main);
    }

    Coroutine corotine;
    public void OnPlay(System.Action<int> callback)
    {
        this.callback = callback;
        if (corotine != null)
            StopCoroutine(corotine);
        corotine = StartCoroutine(IEStart( ));
    }
    IEnumerator IEStart()
    {
        live = Config.Data.Journey.StartLive;
        score = 0;
        round = 0;
        m_speed = Config.Data.Journey.SpeedStart;
        textcombo.text = "";
        textcomboshort.text = "";
        textscore.text = "0";
        isready = false;

        //** Pet Face Diraction
        PetObj.Current.ChangeDiraction(diraction);

        //wait mainmenu animation hide
        yield return new WaitForSeconds(1.0f);
        Sound.PlayBgm(Sound.playlist.bgm_journey);
        root.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        active = true;
        PetObj.Current.anim.OnAnimForce( PetAnim.AnimState.Walk );
        yield return new WaitForEndOfFrame();
        NextRound();
    }











    public void OnBitzA()
    {
        OnBitz(1);
    }
    public void OnBitzB()
    {
        OnBitz(2);
    }
    void OnBitz(int bitz)
    {
        if (!isready)
            return;


        if (bars[bitzIndex].Value == bitz)
        {
            effHit.transform.position = bars[bitzIndex].gameObject.transform.position;
            effHit.OnAwake();

            bars[bitzIndex].Icon.sprite = imgBitzZero;
            bars[bitzIndex].Icon.color = colorZero;

            if(bitzIndex<Sound.playlist.journey_bitz.Length)
                Sound.Play(Sound.playlist.journey_bitz[bitzIndex]);
            else
                Sound.Play(Sound.playlist.journey_bitz[Sound.playlist.journey_bitz.Length-1]);

            //Match
            bitzIndex++;
            if (bitzIndex >= count)
            {
                //Done
                EndRound(true);
            }
        }
        else 
        {
            //Fail
            EndRound(false);
        }

    }




    void UpdateForcus( )
    {
        if (!isready)
        {
            focus.gameObject.SetActive(false);
            t_Time.gameObject.SetActive(false);
            BtnA.button.interactable = false;
            BtnB.button.interactable = false;
        }
        else 
        {
            focus.gameObject.SetActive(true);
            t_Time.gameObject.SetActive(true);
            BtnA.button.interactable = true;
            BtnB.button.interactable = true;

            focus.gameObject.transform.position = bars[bitzIndex].transform.position;
            //focus.gameObject.transform.position = Vector3.Lerp(focus.gameObject.transform.position, bars[round].gameObject.transform.position, timeRun * 2.5f);
        }
    }

    bool active = false;
    int live;
    int score;
    int basescore => Config.Data.Journey.BaseScore;
    int lowscore => Config.Data.Journey.LowScore;
    int combo;
    int bitzIndex = 0;
    int count = 0;
    int round = 0;
    int[] max = new int[2];
    void NextRound()
    {
        StartCoroutine(IENext());
    }
    IEnumerator IENext() 
    {
        round++;
        bitzIndex = 0;
        timeRun = 0.0f;
        m_speed += Config.Data.Journey.SpeedPlus;
        bars.ForEach(x => { x.Value = 0; x.gameObject.SetActive(false); });
        t_winlose.gameObject.SetActive(false);


        if (round > 0)  { max[0] = 2; max[1] = 4; }
        if (round > 8) { max[0] = 3; max[1] = 5; }
        if (round > 16) { max[0] = 3; max[1] = 7; }
        if (round > 25) { max[0] = 3; max[1] = 11; }
        if (round > 50) { max[0] = 5; max[1] = 11; }
        if (round > 100) { max[0] = 8; max[1] = 11; }
        count = Random.RandomRange(max[0], max[1]);


        for (int i = 0; i < count; i++) 
        {
            var bitz = bars[i];
            bitz.gameObject.SetActive(true);
            bitz.Icon.gameObject.SetActive(false);
            bitz.Icon.color = Color.white;

            if (Random.RandomRange(0, 100) < 50)
            {
                bitz.Value = 1;
                bitz.Icon.sprite = imgBitzA;
            }
            else
            {
                bitz.Value = 2;
                bitz.Icon.sprite = imgBitzB;
            }
        }
        focus.gameObject.transform.position = bars[0].transform.position;
        for (int i = 0; i < count; i++)
        {
            var bitz = bars[i];
            bitz.Icon.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.07f);
        }
       
        yield return new WaitForSeconds(0.25f);
        isready = true;
    }




    void EndRound(bool done)
    {
        StartCoroutine(IEEnd(done));
    }
    IEnumerator IEEnd(bool done)
    {
        isready = false;
        bars.ForEach(x => { x.Value = 0; x.gameObject.SetActive(false); });
        t_winlose.gameObject.SetActive(true);
        t_win.gameObject.SetActive(false);
        t_lose.gameObject.SetActive(false);
        textcombo.gameObject.SetActive(false);
        textcomboshort.text = "";

        if (done)
        {
            //Debug.Log("WINNN!");
            Sound.Play(Sound.playlist.journey_win);
            PetObj.Current.talking.bubble.OnEmo(lowScore? Talking.Bubble.EmoType.FeelingNormal : Talking.Bubble.EmoType.FeelingHappy, 1.5f);
            t_win.gameObject.SetActive(true);



            if (combo == 2) Sound.Play(Sound.playlist.journey_cheer[0]);
            if (combo == 4) Sound.Play(Sound.playlist.journey_cheer[1]);
            if (combo == 6) Sound.Play(Sound.playlist.journey_cheer[2]);
            if (combo == 9) Sound.Play(Sound.playlist.journey_cheer[3]);


            combo++;
            combo = combo.Max(Config.Data.Journey.MaxCombo);
            score += (lowScore ? lowscore : basescore) * combo;

            textcomboshort.text = (combo>1)?$"X{combo}":"";

            textscore.text = score.ToString("#,##0");
            animScore.Stop();
            animScore.Play("awake");

        }
        else
        {
            //Debug.Log("FAIL!");
            Sound.Play(Sound.playlist.journey_miss);
            PetObj.Current.talking.bubble.OnEmo(Talking.Bubble.EmoType.FeelingBad, 1.5f);
            t_lose.gameObject.SetActive(true);
            combo = 0;
            live--;
        }



        yield return new WaitForEndOfFrame();
        textcombo.gameObject.SetActive(combo > 1);
        textcombo.text = $"Combo X{combo}";

        


        yield return new WaitForSeconds(0.85f);


        if (live > 0)
        {
            NextRound();
        }
        else 
        {
            //End Game
            StartCoroutine(EndGame());
        }
    }
    IEnumerator EndGame( )
    {
        yield return new WaitForEndOfFrame();
        PetObj.Current.anim.OnReset();
        this.callback?.Invoke(score);
        OnClose();
    }

    public void OnExit()
    {
        StartCoroutine(EndGame());
    }





    bool isready = false;
    float timeRun;
    float timeOut=5.0f;
    bool lowScore => timing.fillAmount <= 0.3f;
    void Update()
    {

        if (!active)
            return;

        UpdateForcus();
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            BtnA.button.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.X)) 
        {
            BtnB.button.onClick.Invoke();
        }

        if (isready == false)
            return;

        if (timeRun < timeOut)
        {
            timeRun += Time.deltaTime * m_speed;
            timing.fillAmount = (timeOut-timeRun) / timeOut;
        }
        else
        {
            EndRound(false);
        }

    }
}
