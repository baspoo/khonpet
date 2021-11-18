using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awake : MonoBehaviour
{


    public bool StopOnAwake;


    void OnEnable()
    {
        if (!StopOnAwake)
            OnAwake();
        else 
        {
            var ParticleSystem = GetComponent<ParticleSystem>();
            if (ParticleSystem != null)
            {
                ParticleSystem.Stop();
            }
        }

    }


    public void OnAwake() 
    {
        gameObject.SetActive(true);
        var ParticleSystem = GetComponent<ParticleSystem>();
        if (ParticleSystem != null)
        {
            ParticleSystem.Stop();
            ParticleSystem.Play();
        }

    }


}
