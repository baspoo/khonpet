using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleService : MonoBehaviour
{

    public static BundleService instance { get { if (m_instance == null) m_instance = FindObjectOfType<BundleService>(); return m_instance; } }
    static BundleService m_instance;

    public bool IsDone { get; private set; }


    public AssetBundle PetBundle;


    public IEnumerator Init( )
    {
        yield return new WaitForEndOfFrame();
        ResourcesHandle.Init( PetData.Current.ID , (r) => {
            IsDone = r;
        });
    }


}
