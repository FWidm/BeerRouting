using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Networking;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class UpdateSite : MonoBehaviour
{
    public const string gameType = "sim";
    public const string URI = "http://134.60.51.204/scripts/getProgressUpdate.php";
    //    public const string URI = "http://requestb.in/1nkiima1";
    public string information
    {
        get;
        set;
    }

    public delegate void PostRequestCallback(string data);


    public string UpdateWebsite(PostRequestCallback callback)
    {
        StartCoroutine(PostRequest(callback));
        return information;
    }

    IEnumerator  PostRequest(PostRequestCallback callback)
    {
        string name = PlayerPrefs.GetString("name");
        //SendRequest
        Debug.Log("UpdateSite >> Start"); 

        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("ps", name);
        param.Add("vr", gameType);
        Debug.Log("UpdateSite >> params=" + param);

        UnityWebRequest www = UnityWebRequest.Post(URI, param);
        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                information = "Seite wurde aktualisiert, beende das Spiel und aktualisiere die Seite im Browser.";
                Debug.Log("UpdateSite >> request sent & is done");
            }
            else
            {
                information = "Verbindung zur Seite konnte nicht hergestellt werden. " +
                "Falls es nach erneutem Versuchen nicht funktioniert, meldet euch bei uns (beercraterouting@lists.uni-ulm.de).";
                Debug.Log("UpdateSite >> request failed code=" + www.responseCode);
            }
            callback(information);
        }
    }


}
