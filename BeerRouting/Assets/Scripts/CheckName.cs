using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckName : MonoBehaviour {

	void Start () {
        string name = PlayerPrefs.GetString("name");
        Debug.Log("Found name=" + name );
        if(name!=null && name.Length > 0)
        {
            Text txtUi=this.gameObject.GetComponent<Text>();
            txtUi.text = "Hallo, " + name + "!";
        }
    }

}
