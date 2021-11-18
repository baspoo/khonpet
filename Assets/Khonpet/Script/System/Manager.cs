using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{


    public static Manager mg { get { if (m_instance == null) m_instance = FindObjectOfType<Manager>(); return m_instance; } }
    static Manager m_instance;







    public Initialize init;
    public InterfaceRoot ui;
    public FirebaseService firebase;
    public NFTService nft;
    public BundleService bundle;
    public LoaderService loader;
    public Setting setting;





    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InterfaceRoot.instance.Loading(true);
        yield return StartCoroutine(init.Init());
        InterfaceRoot.instance.Loading(false);
    }
}
