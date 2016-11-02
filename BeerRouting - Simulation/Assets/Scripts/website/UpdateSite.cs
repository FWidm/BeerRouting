using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Networking;
using System.Collections.Generic;

public class UpdateSite : MonoBehaviour {
    public const string gameType = "simulation";
    //public const string URI = "chernobog.dd-dns.de/update.php";
    public const string URI = "http://requestb.in/1nkiima1";
    void Start()
    {
        Debug.Log("UpdateSite >> Start");
        UpdateWebsite();
    }



    public static void UpdateWebsite()
    {
        string name = PlayerPrefs.GetString("name");
        //SendRequest
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("ps", name);
        param.Add("type", gameType);
        Debug.Log("UpdateSite >> params="+param);

        UnityWebRequest request = UnityWebRequest.Post(URI, param);
        request.Send();
        if (request.isDone)
        {
            Debug.Log("UpdateSite >> request sent & is done");
        }
    }
}
