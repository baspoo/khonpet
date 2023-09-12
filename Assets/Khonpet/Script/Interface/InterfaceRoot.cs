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



    public bool IsReady => m_Ready;
    bool m_Ready;

    public void Init()
    {
        if (string.IsNullOrEmpty(Playing.instance.playingData.NickName))
        {
            var userName = SystemInfo.deviceName;
            if(userName == null)
                userName = "USS-"+Random.RandomRange(11111111,99999999).ToString();
            Playing.instance.UpdateDisplayName(userName);
            Active();

            //World.instance.HideWorld(true);
            //popup.displayName.Open((displayName) =>
            //{
            //    World.instance.HideWorld(false);
            //    Active();
            //});
        }
        else 
        {
            Active();
        }
    }
    void Active() 
    {
        m_Ready = true;
        mainmenu.Init();
        popup.Init();
    }



    public void Loading(bool active)
    {
        loading.gameObject.SetActive(active);
    }

    public void Error(string header, string message)
    {
        PopupPage.instance.message.Open(header, message, false, true).HideBtnClose();
        instance.Loading(false);
    }


}
