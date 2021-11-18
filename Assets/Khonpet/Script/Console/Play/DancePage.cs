using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancePage : MonoBehaviour
{


    public List<DanceObj> Objs;
    public Transform Born;
    public List<DanceData> DanceDatas;
    [System.Serializable]
    public class DanceData
    {
        public int Index;
        public Color Color;
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
        public int Count;
        public float Speed;
    }
    System.Action<bool> m_done;
    int point;



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


    IEnumerator engine()
    {
        point = 0;
        Objs.ForEach(x => {
            x.Dispose();
        });
        DanceDatas.ForEach(x => {
            x.Eff.gameObject.SetActive(false);
        });

        var count = setting.Count;

        Sound.PlayBgm(Sound.playlist.bgm_dance);
        yield return new WaitForSeconds(1.0f);
        PetObj.Current?.anim.OnAnimForce(PetAnim.AnimState.Dance);

        while (count!=0) 
        {
            count--;
            yield return new WaitForSeconds(Random.RandomRange(setting.GenarationsTime[0], setting.GenarationsTime[1]));
            var obj = Objs.Find(x=>!x.IsRuning);
            var data = DanceDatas[DanceDatas.Count.Random()];
            obj.Init(this, data);
        }


        PetObj.Current?.anim.OnReset();

        yield return new WaitForSeconds(2.5f);
        var win = point > (setting.Count / 2);
        Sound.PlayBgm(Sound.playlist.bgm_main);


        m_done?.Invoke(win);
        Close();
    }


    public void OnEnter(GameObject btn)
    {
        foreach (var obj in Objs) 
        {
            if (obj.IsRuning && btn.name == obj.Index.ToString()) 
            {
                obj.OnCheck();
            }
        }
    }

    public void OnMatched( )
    {
        point++;
    }











}
