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

    float loop=0.0f;
    public void LoopStart()
    {
        loop= anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        Debug.Log(loop);
    }
    public void LoopEnd( )
    {
        if(loop != 0.0f) 
        { 
            anim.Play("AnimForce", -1, loop);
            Debug.Log("end");
        }
    }
    public void OnReset()
    {
        loop = 0.0f;
    }


    public void Take(string take) 
    {
        if (m_action.ContainsKey(take))
        {
            m_action[take]?.Invoke();
        }
    }
}
