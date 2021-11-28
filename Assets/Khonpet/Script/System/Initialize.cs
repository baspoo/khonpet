using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{

    public IEnumerator Init()
    {

        // PetData
        PetData.Init(()=> { });
        Language.Init(() => { });
        ConfigData.Init(() => { });

        // Information
        StartCoroutine(Information.instance.Init());

        while (!PetData.Done || !Language.Done || !ConfigData.Done || !Information.instance.isDone) yield return new WaitForEndOfFrame();





        // Pet SetCurrent
        PetData.SetCurrent(Information.instance.ContractAddress, Information.instance.TokenId);
        while (PetData.Current == null) yield return new WaitForEndOfFrame();

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


        Playing.instance.Init();
        PetActivity.Init();
        while (!PetActivity.IsReady) yield return new WaitForEndOfFrame();
        World.instance.Init();
        AirActivity.Init();
        ChatActivity.Init();
        InterfaceRoot.instance.Init();
        ConsoleActivity.Init();
    }


}
