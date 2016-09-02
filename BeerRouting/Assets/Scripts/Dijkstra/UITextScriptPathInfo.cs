using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextScriptPathInfo : MonoBehaviour {

    private static Text shortestPathInfo;
 
	void Awake () {
        shortestPathInfo = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Set the UI text element shortestPathInfo.
    /// </summary>
    /// <param name="info">The text that will be displayed.</param>
    public static void SetShortestPathInfo(string info)
    {
        shortestPathInfo.text = info;
    }
}
