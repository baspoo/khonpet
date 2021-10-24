using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{

    public static Store instance { get { if (m_instance == null) m_instance = FindObjectOfType<Store>(); return m_instance;  } }
    static Store m_instance;



    public PetAsset Pet;
    [System.Serializable]
    public class PetAsset 
    {
        public RuntimeAnimatorController animatorController;
    }

    public List<AirActivity.AirData> AirDatas;



}
