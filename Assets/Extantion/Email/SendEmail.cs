
 using UnityEngine;
 using System.Collections;
 using System;
 using System.Net;
 using System.Net.Mail;
 using System.Net.Security;
 using System.Security.Cryptography.X509Certificates;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class SendEmail : MonoBehaviour
{
    public string ToEmail;
    public string DisplayName;
    public string Subject;
    public void Send()
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("application.info.broadcast@gmail.com", DisplayName );
        mail.To.Add(ToEmail);
        mail.Subject = Subject;
        mail.Body = html();
        mail.IsBodyHtml = true;
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("application.info.broadcast@gmail.com", "ljxegavbhertujvm") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");

    }


    
    public TextAsset temp;
    public string image_cover_url;
    public string header;
    public string[] messages;
    public string btn_url;
    public string btn_name;
    public string foot;
    public string footmessage;

    public string html() {
        string original = temp.text;





        original = original.Replace("#@image_cover_url#", image_cover_url);
        original = original.Replace("#@header#", header);


        string message = "";
        foreach (var msg in messages) {
            message += msg + " <br>";
        }
        original = original.Replace("#@message#", message);


        original = original.Replace("#@btn_url#", btn_url);
        original = original.Replace("#@btn_name#", btn_name);


        original = original.Replace("#@foot#", foot);
        original = original.Replace("#@footmessage#", footmessage);



        return original;
    }


}

















#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SendEmail))]
[System.Serializable]
public class SendEmailUI : Editor
{

    string filename = "emailpreview.html";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var mail = (SendEmail)target;



        if (GUILayout.Button("Send")) 
        {
            mail.Send();
        }
        if (GUILayout.Button("Preview"))
        {
            System.IO.File.WriteAllText(filename, mail.html());
            Application.OpenURL(filename);
        }

       if (System.IO.File.Exists(filename)) {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Remove Preview"))
            {
                System.IO.File.Delete(filename);
            }
            GUI.backgroundColor = Color.white;
        }


    }
}
#endif