using UnityEngine;
using System.Collections;

public class ToggleBeer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
    public void DisplayBeer(bool display)
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        if (display)
        {
            SetActive(true, false);
            pc.SetThrowDisabled(false);
        }
        else
        {
            SetActive(false,true);
            pc.SetThrowDisabled(true);
        }
    }

    private void SetActive(bool beerActive, bool otherActive)
    {
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.name.Contains("arm_left"))
            {
                GameObject handLeft = child.transform.GetChild(0).gameObject;
                foreach (Transform _child in handLeft.transform)
                {
                    //Debug.Log(child.name);
                    if (_child.name.StartsWith("CanvasBeer"))
                    {
                        _child.gameObject.SetActive(beerActive);
                    }
                    if (_child.name.StartsWith("Other"))
                    {
                        _child.gameObject.SetActive(otherActive);
                        //Debug.Log("set placeholder to " + otherActive);

                    }
                }
            }
        }
    }
}
