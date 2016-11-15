using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioMenuMain : MonoBehaviour
{

    /**
     * Audio Mixer
     * */
    public AudioMixer masterMixer;
    public static  readonly string MASTER_VOL = "MasterVol", GAME_VOL = "GameVol", UI_VOL = "UiVol", PROF_VOL = "ProfVol", BACKGROUND_VOL = "BackgroundVol", FIRST_START = "FirstStart";

    public Toggle toggleMaster, toggleGame, toggleUi, toggleProf, toggleBackground;
    public Slider sliderMaster, sliderGame, sliderUi, sliderProf, sliderBackground;
    private static readonly float MIN_VAL = 0f, MAX_VAL = 1f;
    public GameObject panel;
    private CameraFollowPlayer cam;
    private AudioSource buttonClick;

    void Start()
    {

        if (PlayerPrefs.GetFloat(FIRST_START) == 0)
        {
            PlayerPrefs.SetFloat(MASTER_VOL, 1);
            PlayerPrefs.SetFloat(GAME_VOL, 1);
            PlayerPrefs.SetFloat(UI_VOL, 1);
            PlayerPrefs.SetFloat(PROF_VOL, 1);
            PlayerPrefs.SetFloat(BACKGROUND_VOL, 1);
            PlayerPrefs.SetFloat(FIRST_START, 1);

        } else
        {
            SetBackgroundSoundLevel(PlayerPrefs.GetFloat(BACKGROUND_VOL));
            sliderBackground.value = PlayerPrefs.GetFloat(BACKGROUND_VOL);
            SetGameSoundLevel(PlayerPrefs.GetFloat(GAME_VOL));
            sliderGame.value = PlayerPrefs.GetFloat(GAME_VOL);
            SetMasterSoundLevel(PlayerPrefs.GetFloat(MASTER_VOL));
            sliderMaster.value = PlayerPrefs.GetFloat(MASTER_VOL);
            SetProfSoundLevel(PlayerPrefs.GetFloat(PROF_VOL));
            sliderProf.value = PlayerPrefs.GetFloat(PROF_VOL);
            SetUiSoundLevel(PlayerPrefs.GetFloat(UI_VOL));
            sliderProf.value = PlayerPrefs.GetFloat(UI_VOL);

        }


        cam = FindObjectOfType<CameraFollowPlayer>();
    }

    public void SetMasterSoundLevel(float level)
    {
        toggleMaster.isOn = level == MIN_VAL ? false : true;
        //sliderMaster.value = level;
        float dB = ConvertLevelToDB(level);
        //slider position should be between 0-1
        PlayerPrefs.SetFloat(MASTER_VOL, level);
         //mixer should be set in dB (-80-20 or so).
        masterMixer.SetFloat(MASTER_VOL, dB);
        buttonClick = GetComponent<AudioSource>();
        //Debug.Log("Master: " + level);
    }

    public void PlayButtonClickSound()
    {
        buttonClick.Play();
    }

    public void SetGameSoundLevel(float level)
    {
        toggleGame.isOn = level == MIN_VAL ? false : true;
        //sliderGame.value = level; //stackoverflow here!
        float dB = ConvertLevelToDB(level);
        //slider position should be between 0-1
        PlayerPrefs.SetFloat(GAME_VOL, level);
        //mixer should be set in dB (-80-20 or so).
        masterMixer.SetFloat(GAME_VOL, dB);
        //Debug.Log("Game: " + level);
    }

    public void SetUiSoundLevel(float level)
    {
        toggleUi.isOn = level == MIN_VAL ? false : true;
        //sliderUi.value = level;
        float dB = ConvertLevelToDB(level);
        //slider position should be between 0-1
        PlayerPrefs.SetFloat(UI_VOL, level);
        //mixer should be set in dB (-80-20 or so).
        masterMixer.SetFloat(UI_VOL, dB);
        //Debug.Log("UI: " + level);
    }

    public void SetProfSoundLevel(float level)
    {
        toggleProf.isOn = level == MIN_VAL ? false : true;
        //sliderProf.value = level;
        float dB = ConvertLevelToDB(level);
        //slider position should be between 0-1
        PlayerPrefs.SetFloat(PROF_VOL, level);
        //mixer should be set in dB (-80-20 or so).
        masterMixer.SetFloat(PROF_VOL, dB);
        //Debug.Log("Prof: " + level);
    }

    public void SetBackgroundSoundLevel(float level)
    {
        toggleBackground.isOn = level == MIN_VAL ? false : true;
        //sliderBackground.value = level;
        float dB = ConvertLevelToDB(level);
        //slider position should be between 0-1
        PlayerPrefs.SetFloat(BACKGROUND_VOL, level);
        //mixer should be set in dB (-80-20 or so).
        masterMixer.SetFloat(BACKGROUND_VOL, dB);
        //Debug.Log("Back: " + level);
    }

    public void MuteMaster()
    {
        bool isUnmuted = toggleMaster.isOn;
        //Debug.Log("MuteMaster: " + isUnmuted);
        float level = !isUnmuted ? MIN_VAL : MAX_VAL * 0.8f;

        SetMasterSoundLevel(level);
        /*
        SetBackgroundSoundLevel(level);
        SetGameSoundLevel(level);
        SetProfSoundLevel(level);
        SetUiSoundLevel(level);
        */
    }

    public void MuteGameSoundLevel()
    {
        float level = toggleGame.isOn ? MAX_VAL : MIN_VAL;
        SetGameSoundLevel(level);
        //Debug.Log("Mute Game: " + level);
    }

    public void MuteUiSoundLevel()
    {
        float level = toggleUi.isOn ? MAX_VAL : MIN_VAL;
        SetUiSoundLevel(level);
        //Debug.Log("Mute UI: " + level);
    }

    public void MuteProfSoundLevel()
    {
        float level = toggleProf.isOn ? MAX_VAL : MIN_VAL;
        SetProfSoundLevel(level);
        //Debug.Log("Mute Prof: " + level);
    }

    public void MuteBackgroundSoundLevel()
    {
        float level = toggleBackground.isOn ? MAX_VAL : MIN_VAL;
        SetBackgroundSoundLevel(level);
        //Debug.Log("Mute Back: " + level);
    }

    public void ToggleMenu()
    {
        // Fix slider drag an drop audio panel.
        if (cam != null)
        {
            cam.EnableMovement(panel.activeSelf);
        }
        StartCoroutine(CloseAfterTime());
    }

    IEnumerator CloseAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(buttonClick.clip.length); //Nullpointer: NullReferenceException: Object reference not set to an instance of an object
        /*
        NullReferenceException: Object reference not set to an instance of an object
        AudioMenuMain+<CloseAfterTime>c__Iterator20.MoveNext () (at Assets/Scripts/UI/AudioMenu/AudioMenuMain.cs:161)
        UnityEngine.MonoBehaviour:StartCoroutine(IEnumerator)
        AudioMenuMain:ToggleMenu() (at Assets/Scripts/UI/AudioMenu/AudioMenuMain.cs:155)
        UnityEngine.EventSystems.EventSystem:Update()
        */
        // Then close panel.
        panel.SetActive(!panel.activeSelf);
    }

    float ConvertLevelToDB(float level)
    {
        float dB = 0;

        if (level != 0)
            dB = 20.0f * Mathf.Log10(level);
        else
            dB = -144.0f;
        //Debug.Log("level=" + level + " --> db=" + dB);
        return dB;
    }

    float ConvertDBToLevel(float db)
    {
        float level = 0;
        level = Mathf.Pow(10, db / 20f);
        //Debug.Log("db=" + db + " --> level=" + level);
        return level;
    }
}
