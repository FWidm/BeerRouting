using UnityEngine;
using System;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;

public class GmailScript : MonoBehaviour
{
    string _sender = "";
    string _password = "";

    public GmailScript()
    {
        _sender = "uulmbeerrouting@gmail.com";
        _password = "12uulmbeerrouting34";
    }

    void Start()
    {
        string appDataDir = Application.persistentDataPath;
        string logDir = appDataDir + "/SurveyLog";
        string[] fileEntries = Directory.GetFiles(logDir);
        Debug.Log("LogDir=" + logDir);
        foreach (string s in fileEntries)
        {
            Debug.Log("File=" + s);
        }
        SendEmail("uulmbeerrouting@gmail.com", "hello", "World!", fileEntries);
    }

    private void SendEmail(string recipient, string subject, string message, string[] fileEntries)
    {
        //Hardcoded recipient email and subject and body of the mail


        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        //SMTP server can be changed for gmail, yahoomail, etc., just google it up


        client.Port = 25;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(_sender, _password);
        client.EnableSsl = true;
        client.Credentials = (System.Net.ICredentialsByHost)credentials;

        try
        {
            var mail = new MailMessage(_sender.Trim(), recipient.Trim());
            mail.Subject = subject;
            mail.Body = message;
            for (int i = 0; i < fileEntries.Length; i++)
            {
                mail.Attachments.Add(new Attachment(fileEntries[i]));
            }

            Debug.Log("Attachment is now Online");
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            client.Send(mail);
            Debug.Log("Success");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw ex;
        }
    }
}
