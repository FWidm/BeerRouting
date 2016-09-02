using UnityEngine;
using System.Collections;

public class PlayerPrefAudio : MonoBehaviour {

    public GameObject canvas;
    private AudioSource asButtons;
    private float ppButtons = 1;
    private readonly string TOGGLE_ALL = "TOGGLE_All", TOGGLE_BUTTONS = "TOGGLE_StatusBar", STATUS_BAR = "Vol_StatusBar";
    private bool ppToggleAll = false, ppToggleButtons = true; 

    // Use this for initialization
    void Start () {
        ppButtons = PlayerPrefs.GetFloat(STATUS_BAR);
        ppToggleAll = PlayerPrefs.GetFloat(TOGGLE_ALL) > 0;
        ppToggleButtons = PlayerPrefs.GetFloat(TOGGLE_BUTTONS) > 0;
        asButtons = canvas.GetComponents<AudioSource>()[0];
        SetVolumButtons();
    }

    private void SetVolumButtons()
    {
        if (!ppToggleButtons || ppToggleAll)
        {
                asButtons.volume = 0;
        }
        else 
        {
                asButtons.volume = ppButtons;
        }
    }

}
