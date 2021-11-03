using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awake : MonoBehaviour
{
   




    void OnEnable()
    {

        OnAwake();

    }


    public void OnAwake() 
    {
        var ParticleSystem = GetComponent<ParticleSystem>();
        if (ParticleSystem != null)
        {
            ParticleSystem.Stop();
            ParticleSystem.Play();
        }

    }


}