using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{

    public IEnumerator Init()
    {

        // PetData
        Playing.instance.Init();
        PetData.Init(()=> { });
        Language.Init(() => { });
        Config.Init(() => { });

        // Information
        StartCoroutine(Information.instance.Init());

        while (!PetData.Done || !Language.Done || !Config.Done || !Information.instance.isDone) yield return new WaitForEndOfFrame();





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



      
       

        PetActivity.Init();
        while (!PetActivity.IsReady) yield return new WaitForEndOfFrame();
        Sound.Init();
        World.instance.Init();
        AirActivity.Init();
        Chat.instance.Init();
        InterfaceRoot.instance.Init();
        ConsoleActivity.Init();
    }


}
