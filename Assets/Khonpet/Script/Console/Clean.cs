using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clean : MonoBehaviour
{
    public static Clean instance { get { if (m_instance == null) m_instance = FindObjectOfType<Clean>(); return m_instance; } }
    static Clean m_instance;










    public Transform Root;
    public AnimCallback AnimCallback;
    public void Init()
    {
        m_instance = this;
        OnClose();
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
