using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCallback : MonoBehaviour
{
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
    public void Take(string take) 
    {
        if (m_action.ContainsKey(take))
        {
            m_action[take]?.Invoke();
        }
    }
}
