using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class HtmlCallback : MonoBehaviour
{

    // Unity Send To Html
    [DllImport("__Internal")]
    private static extern void OnHtmlPing(int code);

    public static void onHtmlPing(int code)
    {
        OnHtmlPing(code);

        // Add On Html Script
        // function HtmlPing( code )  {  }
    }




    [DllImport("__Internal")]
    private static extern void OnHtmlMessage(int code, string str);

    public static void onHtmlMessage(int code, string str)
    {
        OnHtmlMessage(code, str);

        // Add On Html Script
        // function HtmlMessage( code , str )  {  }
    }

















    // Html Send To Unity
    // unityInstance.SendMessage(  <gameobject.name> , <function>, <parms> );
    // unityInstance.SendMessage('HtmlCallback', 'Callback', 'string message');

    public void Callback( string str)
    {
        
    }









    //Check Mobile
    static System.Action<bool> onmobilecallback;
    public static void IsMobile(System.Action<bool> mobilecallback)
    {
        onmobilecallback = mobilecallback;
        OnHtmlMessage(0, "ismobile");
    }
    public void IsMobileCallback(string str)
    {
        onmobilecallback?.Invoke(str=="1");
    }









    //Goto Url
    public static void GotoUrl(string url)
    {
        OnHtmlMessage(1, url);
    }


    //GotoPet
    public static void GotoPet(string contect)
    {
        OnHtmlMessage(3, contect);
    }






    //Check Mobile
    static System.Action<string> onPopupInputMessage;
    public static void PopupInputMessage(string header , System.Action<string> callback )
    {
        onPopupInputMessage = callback;
        OnHtmlMessage(2, header );
    }
    public void PopupInputMessageCallback(string str)
    {
        onPopupInputMessage?.Invoke(str);
    }

}
