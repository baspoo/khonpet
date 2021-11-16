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
                m_petObj = GameObject.Instantiate((GameObject)obj,World.instance.PetPosition).GetComponent<PetObj>();
                m_petObj.transform.localPosition = Vector3.zero;
                m_petObj.transform.localScale = Vector3.one;
                m_petObj.Init();
            }
            else Debug.LogError($"petobj = null!");
        });

    }








}
