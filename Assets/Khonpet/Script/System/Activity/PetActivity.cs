using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetActivity 
{


    public static bool IsReady => m_petObj != null;

    static PetObj m_petObj;
    public static void Init( )
    {

        ResourcesHandle.Load(PetData.Current.ID,"pet", ResourcesHandle.FileType.prefab, (obj) => {
            if (obj != null) 
            {
                m_petObj = GameObject.Instantiate((GameObject)obj,World.instance.transform).GetComponent<PetObj>();
            }
            else Debug.LogError($"petobj = null!");
        });

    }








}
