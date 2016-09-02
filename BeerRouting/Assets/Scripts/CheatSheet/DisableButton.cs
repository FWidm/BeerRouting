using UnityEngine;
using System.Collections;

public class DisableButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        string name = this.name;

        // Toggles which button is shown on desktop or android
        if (Application.platform == RuntimePlatform.Android)
        {
            if (name == "ButtonAlt")
            {
                this.gameObject.SetActive(false);
            }
            else if (name == "ButtonNavi")
            {
                this.gameObject.SetActive(true);
            }
        }
        else
        {
            if (name == "ButtonNavi")
            {
                this.gameObject.SetActive(false);
            }
            else if (name == "ButtonNavi")
            {
                this.gameObject.SetActive(true);
            }
        }
    }
	
}
