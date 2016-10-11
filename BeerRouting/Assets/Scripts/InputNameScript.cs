using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputNameScript : MonoBehaviour {
    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    // Use this for initialization
    void Start () {
        string name=PlayerPrefs.GetString("name");
        if(name!=null && name.Length > 0)
        {
            this.name = name;
            Debug.Log("Found PlayerPrefs(name)=" + name);
            InputField inputFieldCo = this.gameObject.GetComponent<InputField>();
            inputFieldCo.text = name;

        }
    }
	
	// Update is called once per frame
	public void InputEnd (string n) {
        Debug.Log("Input ended string="+n);
        PlayerPrefs.SetString("name", n);
        PlayerPrefs.Save();
	}
}
