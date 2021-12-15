using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clean : MonoBehaviour
{
    public static Clean instance { get { if (m_instance == null) m_instance = FindObjectOfType<Clean>(); return m_instance; } }
    static Clean m_instance;




    public Transform Root;
    public Animation[] AnimPoos;
    public AnimCallback AnimCallback;
    public void Init()
    {
        m_instance = this;
        StartCoroutine(PooActive());
        OnClose();
    }



    List<Animation> pooActive = new List<Animation>();
    int pooPoint => (Pet.Static.MaxStat- cleanstat) / 3;
    int cleanstat => PetData.PetInspector.GetStat(Pet.StatType.Cleanliness);
    bool ishavePool => pooPoint >= 10;
    bool iscleaned = false;
    IEnumerator PooActive( )
    {
        iscleaned = false;
        foreach (var anim in AnimPoos)
            anim.gameObject.SetActive(false);


        var poo = pooPoint;
        if (poo >= 10) 
        {
            AnimPoos[0].gameObject.SetActive(true);
            pooActive.Add(AnimPoos[0]);
        }
        if (poo >= 20)
        {
            AnimPoos[1].gameObject.SetActive(true);
            pooActive.Add(AnimPoos[1]);
        }
        if (poo >= 30)
        {
            AnimPoos[2].gameObject.SetActive(true);
            pooActive.Add(AnimPoos[2]);
        }



        yield return new WaitForSeconds(1.00f);
        while (ishavePool && !iscleaned) 
        {
            pooActive[Random.RandomRange(0, pooActive.Count )].Play("poo_dance");
            yield return new WaitForSeconds(Random.RandomRange(3.0f,6.0f));
        }
    }

    IEnumerator CleanPoo()
    {
        iscleaned = true;
        foreach (var anim in pooActive)
            anim.Play("poo_clean");
        yield return new WaitForSeconds(2.00f);
        foreach (var anim in pooActive)
            anim.gameObject.SetActive(false);
    }


    Coroutine corotine;
    public void OnPlay(System.Action callback)
    {
        Root.gameObject.SetActive(true);
        if (corotine != null)
            StopCoroutine(corotine);
        corotine = StartCoroutine(Play(callback));
    }


    IEnumerator Play(System.Action callback)
    {
        bool isAnimDone = false;

        //** Food Sprite Start
        AnimCallback.ClearAction();
        AnimCallback.AddAction("start", () => {

        });
        AnimCallback.AddAction("full", () => {
            PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.LikeLove);
            isAnimDone = true;
        });

        Sound.Play(Sound.playlist.clean);

        StartCoroutine(CleanPoo());


        //** wait
        while (!isAnimDone) yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1.00f);
       

        //** End
        callback?.Invoke();
        OnClose();
    }
    public void OnClose()
    {
        Root.gameObject.SetActive(false);
    }



}
