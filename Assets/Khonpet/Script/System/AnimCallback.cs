using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCallback : MonoBehaviour
{
    public Animator anim { get { if (m_anim == null) m_anim = GetComponent<Animator>(); return m_anim; } }
    Animator m_anim;


    Dictionary<string, System.Action> m_action = new Dictionary<string, System.Action>();
    public void AddAction(string take, System.Action action) 
    {
        if (m_action.ContainsKey(take))
        {
            m_action[take] = action;
        }
        else 
        {
            m_action.Add(take, action);
        }
    }
    public void ClearAction()
    {
        m_action = new Dictionary<string, System.Action>();
    }
    float loop=0.0f;
    public void LoopStart()
    {
        loop= anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    public void LoopEnd( )
    {
        if(loop != 0.0f) 
        { 
            anim.Play("AnimForce", -1, loop);
        }
    }
    public void OnReset()
    {
        loop = 0.0f;
    }
    public void ContiuneClip(string clipname)
    {
        var a = GetComponent<Animation>();
        a.Stop();
        a.Play(clipname);
    }

    public void Take(string take) 
    {
        if (m_action.ContainsKey(take))
        {
            m_action[take]?.Invoke();
        }
    }

    public UnityEngine.Events.UnityEvent EventSimple;
    public void Simple( )
    {
        EventSimple?.Invoke();
    }
}
