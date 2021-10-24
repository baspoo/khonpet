using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{

    public IEnumerator Init()
    {

        // PetData
        PetData.Init(()=> { });
        // Information
        StartCoroutine(Information.instance.Init());

        while (!PetData.Done || !Information.instance.isDone) yield return new WaitForEndOfFrame();



        // Pet SetCurrent
        PetData.SetCurrent(Information.instance.ContractAddress, Information.instance.TokenId);
        // FirebaseService Init
        FirebaseService.instance.Init(PetData.Current.ID, () => { });
        // NFTService Init
        StartCoroutine(NFTService.instance.Init(Information.instance.ContractAddress, Information.instance.TokenId));
        // BundleService Init
        StartCoroutine(BundleService.instance.Init());

        while (!FirebaseService.instance.IsDone ||
            !NFTService.instance.IsDone ||
            !BundleService.instance.IsDone) yield return new WaitForEndOfFrame();


        Debug.Log("Initialize Done.");

        //----------------------------------------------------------------------------------------------------
        // [Instance Game Client]
        //----------------------------------------------------------------------------------------------------

       
        World.instance.Init();
        PetActivity.Init();
        while (!PetActivity.IsReady) yield return new WaitForEndOfFrame();
        ConsoleActivity.Init();
        AirActivity.Init();
        ChatActivity.Init();
        InterfaceRoot.instance.Init();
    }


}
