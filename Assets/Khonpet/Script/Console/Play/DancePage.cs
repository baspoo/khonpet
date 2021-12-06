using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancePage : MonoBehaviour
{


    public List<DanceObj> Objs;
    public Transform Born;
    public List<DanceData> DanceDatas;
    public List<Btn> Btns;
    [System.Serializable]
    public class DanceData
    {
        public int Index;
        public Color Color;
        public Color ArrowColor;
        public Vector3 Rotate;
        public Transform EndPoint;
        public Awake Eff;
    }

    public Setting setting;
    [System.Serializable]
    public class Setting
    {
        public float PaddingRange;
        public float[] MatchRanges;
        public float[] GenarationsTime;
        public float DiscountGenarationsTime;
        public int Count;
        public float Speed;
        public float SpeedAwake;
        public float SpeedPlus;
    }
    System.Action<bool> m_done;
    int point;
    bool active = false;


    public void Init(System.Action<bool> done)
    {
        m_done = done;
        gameObject.SetActive(true);
        StartCoroutine(engine());
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    float genTimes_min;
    float genTimes_max;
    IEnumerator engine()
    {
        point = 0;
        genTimes_min = setting.GenarationsTime[0];
        genTimes_max = setting.GenarationsTime[1];
        setting.Speed = setting.SpeedAwake;
        Objs.ForEach(x => {
            x.Dispose();
        });
        DanceDatas.ForEach(x => {
            x.Eff.gameObject.SetActive(false);
        });

        var count = setting.Count;
        active = true;
        Sound.PlayBgm(Sound.playlist.bgm_dance);
        yield return new WaitForSeconds(1.0f);
        PetObj.Current?.anim.OnAnimForce(PetAnim.AnimState.Dance);

        while (count!=0) 
        {
            count--;
            yield return new WaitForSeconds(Random.RandomRange(genTimes_min, genTimes_max));
            var obj = Objs.Find(x=>!x.IsRuning);
            var data = DanceDatas[DanceDatas.Count.Random()];
            obj.Init(this, data);
        }


        PetObj.Current?.anim.OnReset();

        yield return new WaitForSeconds(2.5f);
        var win = point > (setting.Count / 2);
        Sound.PlayBgm(Sound.playlist.bgm_main);



        PetObj.Current.talking.bubble.OnEmo(win ? Talking.Bubble.EmoType.FeelingHappy : Talking.Bubble.EmoType.FeelingBad, 1.5f);
        if (win)
        {
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
            Sound.Play(Sound.playlist.yeahh);
        }
        active = false;
        m_done?.Invoke(win);
        Close();
    }


    public void OnEnter(GameObject btn)
    {
        int index = -1;
        foreach (var obj in Objs) 
        {
            if (obj.IsRuning) 
            {
                Debug.Log($"   {btn.name}  {obj.Index}");
                if (btn.name == obj.Index.ToString()) 
                {
                    if (obj.OnCheck())
                        index = obj.Index;
                }
            }
        }

        if(index != -1) Sound.Play(Sound.playlist.dance_bitz[index-1]);
        else Sound.Play(Sound.playlist.click);
    }

    public void OnMatched( )
    {
        point++;
    }


    void Update()
    {

        if (!active)
            return;


        setting.Speed += Time.deltaTime * setting.SpeedPlus;
        genTimes_max -= Time.deltaTime * setting.DiscountGenarationsTime;


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Btns[0].button.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Btns[1].button.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Btns[2].button.onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Btns[3].button.onClick.Invoke();
        }


    }








}
