using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{
   
    IEnumerator Start()
    {


        PetData.Init(()=> { });
        while (!PetData.Done) yield return new WaitForEndOfFrame();

        

    }


}
