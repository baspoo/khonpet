using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceRoot : MonoBehaviour
{
    public static InterfaceRoot instance { get { if (m_instance == null) m_instance = FindObjectOfType<InterfaceRoot>(); return m_instance; } }
    static InterfaceRoot m_instance;

    public Transform loading;
    public MainmenuPage mainmenu;
    public ScreenCanvas screenCanvas;
    public PopupPage popup;








    public void Init()
    {
        mainmenu.Init();
        popup.Init();

        if (string.IsNullOrEmpty(Playing.instance.playingData.NickName)) 
        {
            popup.displayName.Open((displayName)=> {
               
            });
        }

    }

    public void Loading(bool active)
    {
        loading.gameObject.SetActive(active);
    }




}
