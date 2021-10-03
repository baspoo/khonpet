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



}
