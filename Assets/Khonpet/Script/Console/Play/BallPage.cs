using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPage : MonoBehaviour
{

    PetObj pet => PetObj.Current;
    public Transform ballposition;
    public Animation anim;
    public AnimCallback animcallback;
    public UnityEngine.UI.Image uiImage;
    public Awake awake;
    public GameObject gate;

    private void Awake()
    {
        //Init();  ballwelcome
    }

    System.Action<bool> m_done;
    public void Init(System.Action<bool> done)
    {
        gameObject.SetActive(true);
        awake.gameObject.SetActive(false);
        ballposition.position = PetObj.Current.body.head.position;

        m_done = done;
        count = 0;
        point = 0;
        uiImage.fillAmount = 0.0f;
        StartCoroutine(Welcome());
       
    }
    IEnumerator Welcome()
    {
        anim.Stop();
        anim.Play("ballwelcome");
        yield return  new WaitForSeconds(1.0f);
        isready = true;
        StartCoroutine(Anim());
    }
    public void Close() 
    {
        gameObject.SetActive(false);
    }
    IEnumerator Anim()
    {
        bool isKick = false;
        pet.anim.Animcallback.AddAction("ballkick", () =>
        {
            isKick = true;
            anim.Stop();
            anim.Play("ball");

        });
        while (true) 
        {
            isKick = false;
            pet.anim.OnAnimForce(PetAnim.AnimState.BallKick);
            while (!isKick) yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(anim.clip.length);
        }
    }




    public float speed = 1.0f;
    float runtime = 0.0f;
    private void Update()
    {
        if (isready)
        {
            runtime += Time.deltaTime * speed;
            if (runtime >= 1.0f)
            {
                runtime = 0.0f;
            }
            uiImage.fillAmount = runtime;
        }

    }
    public int MaxCount;
    public int PassPoint;
    public float ShackVolume;
    int count =0;
    int point = 0;
    bool isready = false;
    public float[] range = new float[2];
    public void OnKick()
    {
        if(isready)
            StartCoroutine(OnReset());
    }

    IEnumerator OnReset() 
    {
        count++;
        if (runtime >= range[0] && runtime <= range[1])
        {
            point++;
            Debug.Log($"Wow [{runtime}]");
            Sound.Play(Sound.playlist.match);
            Talking.instance.petTalk.ShowHeader(Talking.PetTalk.HeaderType.goodjob);
            awake.OnAwake();
        }
        else
        {
            Debug.Log($"Fail [{runtime}]");
            Sound.Play(Sound.playlist.fail);
            Talking.instance.petTalk.ShowHeader(Talking.PetTalk.HeaderType.bad);
        }
        iTween.ShakePosition(gate, Vector3.one * ShackVolume , 0.25f);
        isready = false;
        runtime = 0.0f;
        yield return new WaitForSeconds(0.5f);
        if (count < MaxCount && point < PassPoint)
            isready = true;
        else 
        {
            yield return new WaitForSeconds(1.0f);
            Close();

            var win = point >= PassPoint;

            PetObj.Current.talking.bubble.OnEmo(win ? Talking.Bubble.EmoType.FeelingHappy : Talking.Bubble.EmoType.FeelingBad , 1.5f);
            if (win)
            {
                PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.GoodJob);
                Sound.Play(Sound.playlist.yeahh);
            }

            m_done?.Invoke(win);
        }
    }

}
