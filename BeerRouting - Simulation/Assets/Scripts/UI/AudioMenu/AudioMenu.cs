using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMenu : MonoBehaviour
{
    
    private GameObject goProfessorSprites, goSpeechBubble;
    public GameObject goStatusBar;
    private AudioSource[] asProfessorSprites, asSpeechBubble, asStatusBar;
    public Toggle toggleAll, toggleProf, toggleButtons;
    public Slider sliderProf, sliderButtons;
    public Button buttonClose;
    public GameObject panel;

    private float ppProfessorSprites = 1, ppSpeechBubble = 1, ppStatusBar = 1;
    private bool ppToggleAll = false, ppToggleProf = false, ppToggleStatusBar = false;
    private readonly string PROFESSOR = "Vol_ProfessorSprites_sim", STATUS_BAR = "Vol_StatusBar_sim", TOGGLE_ALL = "TOGGLE_All_sim", TOGGLE_PROF = "TOGGLE_Professsor_sim", TOGGLE_BUTTONS = "TOGGLE_StatusBar_sim";
    private CameraFollowPlayer cam;

    void Awake()
    {
        goProfessorSprites = GameObject.FindGameObjectWithTag("Professor");
        goSpeechBubble = GameObject.FindGameObjectWithTag("SpeechBubble");
        cam = FindObjectOfType<CameraFollowPlayer>();
    }

    // Use this for initialization
    void Start()
    {
        ppProfessorSprites = PlayerPrefs.GetFloat(PROFESSOR);
        ppStatusBar = PlayerPrefs.GetFloat(STATUS_BAR);
        ppToggleAll = PlayerPrefs.GetInt(TOGGLE_ALL) > 0;
        ppToggleProf = PlayerPrefs.GetInt(TOGGLE_PROF) > 0;
        ppToggleStatusBar = PlayerPrefs.GetInt(TOGGLE_BUTTONS) > 0;
        Debug.Log("ProfSprites=" + goProfessorSprites + " -> has audio? " + goProfessorSprites.GetComponents<AudioSource>());
        asProfessorSprites = goProfessorSprites.GetComponents<AudioSource>();
        asSpeechBubble = goSpeechBubble.GetComponents<AudioSource>();
        asStatusBar = goStatusBar.GetComponents<AudioSource>();

        sliderProf.onValueChanged.AddListener(delegate
            {
                ValueChangeCheckProf();
            });
        sliderButtons.onValueChanged.AddListener(delegate
            {
                ValueChangeCheckButtons();
            });
        toggleAll.onValueChanged.AddListener(delegate
            {
                SwitchAudioAll();
            });
        toggleButtons.onValueChanged.AddListener(delegate
            {
                SetButtonsVolume();
            });
        toggleProf.onValueChanged.AddListener(delegate
            {
                SwitchAudioProfessor(ppProfessorSprites);
            });

        sliderProf.value = ppProfessorSprites;
        sliderButtons.value = ppStatusBar;
        toggleAll.isOn = ppToggleAll;
        toggleProf.isOn = ppToggleProf;
        toggleButtons.isOn = ppToggleStatusBar;

        SetButtonsVolume();
        SwitchAudioProfessor(ppProfessorSprites);
    }

    public void ValueChangeCheckProf()
    {
        if (sliderProf.value > 0)
        {
            toggleProf.isOn = true;

        }
        else
        {
            toggleProf.isOn = false;
        }
        SwitchAudioProfessor(sliderProf.value);
        PlayerPrefs.SetFloat(PROFESSOR, sliderProf.value);
        PlayerPrefs.SetInt(TOGGLE_PROF, toggleProf.isOn ? 1 : 0);
    }

    public void ValueChangeCheckButtons()
    {
        if (sliderButtons.value > 0)
        {
            toggleButtons.isOn = true;
        }
        else
        {
            toggleButtons.isOn = false;
        }
        SwitchAudioButtons(sliderButtons.value);
        PlayerPrefs.SetFloat(STATUS_BAR, sliderButtons.value);
        PlayerPrefs.SetInt(TOGGLE_BUTTONS, toggleButtons.isOn ? 1 : 0);
    }

    
    public void SwitchAudioProfessor()
    {
        SwitchAudioSource(asProfessorSprites, !toggleProf.isOn);
        PlayerPrefs.SetInt(TOGGLE_PROF, toggleProf.isOn ? 1 : 0);
    }

    private void SwitchAudioProfessor(float vol)
    {
        if (toggleProf.isOn && !toggleAll.isOn)
        {
            SwitchAudioSource(asProfessorSprites, vol);
        }
        else
        {
            SwitchAudioProfessor();
        }
        
    }

    public void SwitchAudioButtons()
    {
        SwitchAudioSource(asStatusBar, ppStatusBar);
        SwitchAudioSource(asSpeechBubble, ppSpeechBubble);
        PlayerPrefs.SetInt(TOGGLE_BUTTONS, toggleButtons.isOn ? 1 : 0);
    }

    private void SetButtonsVolume()
    {
        if (!toggleButtons.isOn)
        {
            SwitchAudioSource(asStatusBar, true);
            SwitchAudioSource(asSpeechBubble, true);
        }
        else if (!toggleAll.isOn)
        {
            SwitchAudioSource(asStatusBar, sliderButtons.value);
            SwitchAudioSource(asSpeechBubble, sliderButtons.value);
        }
        PlayerPrefs.SetFloat(STATUS_BAR, sliderButtons.value);
        PlayerPrefs.SetInt(TOGGLE_BUTTONS, toggleButtons.isOn ? 1 : 0);
    }


    private void SwitchAudioButtons(float vol)
    {
        if (toggleButtons.isOn && !toggleAll.isOn)
        {
            SwitchAudioSource(asStatusBar, vol);
            SwitchAudioSource(asSpeechBubble, vol);
            PlayerPrefs.SetInt(TOGGLE_BUTTONS, toggleButtons.isOn ? 1 : 0);
        }
        else
        {
            SwitchAudioButtons();
        }
    }


    public void SwitchAudioAll()
    {
        toggleButtons.isOn = !toggleAll.isOn;
        toggleProf.isOn = !toggleAll.isOn;
        PlayerPrefs.SetInt(TOGGLE_ALL, (toggleAll.isOn ? 1 : 0));
        if (toggleAll.isOn)
        {
            SwitchAudioSource(asProfessorSprites, true);
            SwitchAudioSource(asSpeechBubble, true);
            SwitchAudioSource(asStatusBar, true);
        }
        else
        {
            SwitchAudioSource(asProfessorSprites, PlayerPrefs.GetFloat(PROFESSOR));
            SwitchAudioSource(asSpeechBubble, PlayerPrefs.GetFloat(STATUS_BAR));
            SwitchAudioSource(asStatusBar, PlayerPrefs.GetFloat(STATUS_BAR));
        }
    }



    private void SwitchAudioSource(AudioSource[] audios, bool isMute)
    {
        foreach (AudioSource audio in audios)
        {
            if (!isMute)
            {
                audio.volume = 1;
            }
            else
            {
                audio.volume = 0;
            }
        }
    }

    private void SwitchAudioSource(AudioSource[] audios, float vol)
    {
        foreach (AudioSource audio in audios)
        {
            audio.volume = vol;
        }
    }

    public void ToggleMenu()
    {
        // Fix slider drag an drop audio panel.
        // cam.EnableMovement(panel.activeSelf);
        panel.SetActive(!panel.activeSelf);
    }
}
