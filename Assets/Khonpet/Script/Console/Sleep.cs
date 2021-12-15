using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    public static Sleep instance { get { if (m_instance == null) m_instance = FindObjectOfType<Sleep>(); return m_instance; } }
    static Sleep m_instance;




    public Transform Root;
    public UnityEngine.UI.Text txtTime;
    public void Init(System.Action onsleeping)
    {
        m_instance = this;
        if (PetData.PetInspector.IsSleeping)
        {
            onsleeping?.Invoke();
            OnPlay();
        }
        else 
        {
            Root.gameObject.SetActive(false);
        }
    }
    Coroutine corotine;
    //System.Action callback;
    public void OnPlay( )
    {
        if (corotine != null)
            StopCoroutine(corotine);
        corotine = StartCoroutine(Play( ));
    }
    IEnumerator Play( )
    {
        //this.callback = callback;

        PetObj.Current.anim.OnAnimForce(PetAnim.AnimState.Sleep);
        Talking.instance.bubble.OnEmo( Talking.Bubble.EmoType.Sleep );
        InterfaceRoot.instance.mainmenu.main.OnActive(false);

        yield return new WaitForSeconds(0.75f);
        Root.gameObject.SetActive(true);

        var act = PetData.PetInspector.GetActivity(Pet.Activity.StartSleep);
        var last = Utility.TimeServer.TimeStampToDateTime(act.UnixLastActive);

        var remain = Pet.Static.MaxStat - act.Value;
        var timeout = last.AddMinutes(remain);

        while (timeout > System.DateTime.Now) 
        {
            var now = System.DateTime.Now;
            //var t = now - last;
            var t = timeout - now;
            //var duration = 0;
            //var t = System.TimeSpan.FromSeconds(duration);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);
            txtTime.text = answer;
            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForEndOfFrame();
        txtTime.text = "Full Energy";
    }
    public void OnStop( )
    {
        if (corotine != null)
            StopCoroutine(corotine);

        Root.gameObject.SetActive(false);
        InterfaceRoot.instance.mainmenu.main.OnActive(true);
        PetObj.Current.anim.OnReset();
        Talking.instance.bubble.Hide();

        //** End
        PetData.PetInspector.OnSleepComplete();
        ConsoleActivity.OnEnding();
        //callback?.Invoke();
    }

}
