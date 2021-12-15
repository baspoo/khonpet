using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Information : MonoBehaviour
{
    public static Information instance { get { if (m_instance == null) m_instance = FindObjectOfType<Information>(); return m_instance; } }
    static Information m_instance;




    public bool isDone;
      public bool IsWidescreen => 
        (Setting.instance.device.screen == Setting.Device.ScreenType.auto)?Screen.width > Screen.height : 
        (Setting.instance.device.screen == Setting.Device.ScreenType.wide);

    public bool IsMobile;
    public string ContractAddress;
    public string TokenId;
    public URLParameters.Parameters Parameters;
   

    public IEnumerator Init( )
    {
     
       
        yield return new WaitForEndOfFrame();

        // IsMobile
        var checking_html = false;
        #if UNITY_WEBGL && !UNITY_EDITOR
                HtmlCallback.IsMobile((r) => { 
                    IsMobile = r; 
                   checking_html = true;
                });
        #else
            IsMobile = Setting.instance.device.isForceMobile? true : false;
            checking_html = true;
        #endif






        // URL Parameters
        var checking_url = false;
        URLParameters.Instance.Request((parameters) => {
            
            Parameters = parameters;
            var id = parameters.SearchParams.Get("id");
            if (id.notnull())
            {
                if (id.Contains("/"))
                {
                    var address = id.Split('/');
                    ContractAddress = address[0];
                    TokenId = address[1];
                }
                else 
                {
                    ContractAddress = id;
                    TokenId = "";
                }
            }
            checking_url = true;
        });




        while (!checking_html || !checking_url) yield return new WaitForEndOfFrame();
        isDone = true;
    
    
    }


}
