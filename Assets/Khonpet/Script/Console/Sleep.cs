using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    public static Sleep instance { get { if (m_instance == null) m_instance = FindObjectOfType<Sleep>(); return m_instance; } }
    static Sleep m_instance;






    public void Init()
    {
        m_instance = this;
    }
    Coroutine corotine;
    public void OnPlay(System.Action callback)
    {
        if (corotine != null)
            StopCoroutine(corotine);
        corotine = StartCoroutine(Play(callback));
    }
    IEnumerator Play(System.Action callback)
    {
        float remaining = 8.0f;
        PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Sleep);
        Talking.instance.bubble.OnSleep();
        InterfaceRoot.instance.mainmenu.main.OnActive(false);
        yield return new WaitForSeconds(remaining);
        InterfaceRoot.instance.mainmenu.main.OnActive(true);
        PetObj.Current.anim.OnReset();
        Talking.instance.bubble.Hide();

        //** End
        callback?.Invoke();

    }


}
