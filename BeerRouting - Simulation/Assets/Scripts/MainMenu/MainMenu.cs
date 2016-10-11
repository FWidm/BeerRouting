using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Texture backgroundTexture;

    public GUIStyle buttonExit;
    public GUIStyle buttonCheatSheet;
    public GUIStyle buttonPlay;

    public GUIStyle buttonTutorial;
    public int levelTutorial;


    public GUIStyle buttonLevel1;
    public int level1;

    public bool isAllAudioMuted = false;
    public bool soundUI = false;
    public bool soundGameplay = false;

    private float levelButtonSize;
    private float guiButtonSize;

    private float sectionTutorials = 0f;
    private float sectionLevels = 0.25f;
    private float sectionSettings = 0.75f;

    private float marginWidth;
    private float marginHeight;
    private float offsetY;

    private int numberOfIconsInRow = 10;

    public GameObject modalDialogue;
    public GUIStyle labelStyleTutorials;
    public GUIStyle labelStyleLevels;
    public GUIStyle labelStyleSettings;

    public GUIStyle guiStyleSliderUiSound1;
    public GUIStyle guiStyleSliderUiSound2;

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);

        levelButtonSize = ((Screen.height / numberOfIconsInRow)/5)*4;
        marginHeight = ((Screen.height / numberOfIconsInRow) / 5)*1;
        offsetY = (Screen.height / numberOfIconsInRow)*4;
        guiButtonSize = levelButtonSize;
        marginWidth = (Screen.width  - numberOfIconsInRow * levelButtonSize) / (numberOfIconsInRow-1)+marginHeight;

        //SectionTutorials

        GUI.Label(new Rect(Screen.width * sectionTutorials + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * -2) + (levelButtonSize * 0), levelButtonSize, levelButtonSize),
            "Tutorials", labelStyleTutorials);
        if (GUI.Button(new Rect(Screen.width * sectionTutorials + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 1) + (levelButtonSize * 0), levelButtonSize, levelButtonSize),
            "", buttonPlay))
        {
            LoadScene(level1);
        }
        /*
        if (GUI.Button(new Rect(Screen.width * sectionTutorials+ (marginWidth * 2) + (levelButtonSize * 1), offsetY + (marginHeight * 1) + (levelButtonSize * 0), levelButtonSize, levelButtonSize),
            "", buttonTutorial))
        {
            LoadScene(level1);
        }
        */

        //SectionLevels
        GUI.Label(new Rect(Screen.width * sectionLevels + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * -2) + (levelButtonSize * 0), levelButtonSize, levelButtonSize),
            "Levels", labelStyleLevels);
        //First row
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 1) + (levelButtonSize * 0), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 2) + (levelButtonSize * 1), offsetY + (marginHeight * 1) + (levelButtonSize * 0), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 3) + (levelButtonSize * 2), offsetY + (marginHeight * 1) + (levelButtonSize * 0), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 4) + (levelButtonSize * 3), offsetY + (marginHeight * 1) + (levelButtonSize * 0), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }

        //Second row
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 2) + (levelButtonSize * 1), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 2) + (levelButtonSize * 1), offsetY + (marginHeight * 2) + (levelButtonSize * 1), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 3) + (levelButtonSize * 2), offsetY + (marginHeight * 2) + (levelButtonSize * 1), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 4) + (levelButtonSize * 3), offsetY + (marginHeight * 2) + (levelButtonSize * 1), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }

        //Third row
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 3) + (levelButtonSize * 2), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 2) + (levelButtonSize * 1), offsetY + (marginHeight * 3) + (levelButtonSize * 2), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 3) + (levelButtonSize * 2), offsetY + (marginHeight * 3) + (levelButtonSize * 2), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 4) + (levelButtonSize * 3), offsetY + (marginHeight * 3) + (levelButtonSize * 2), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }

        //Fourth row
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 4) + (levelButtonSize * 3), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 2) + (levelButtonSize * 1), offsetY + (marginHeight * 4) + (levelButtonSize * 3), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 3) + (levelButtonSize * 2), offsetY + (marginHeight * 4) + (levelButtonSize * 3), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }
        if (GUI.Button(new Rect(Screen.width * sectionLevels + (marginWidth * 4) + (levelButtonSize * 3), offsetY + (marginHeight * 4) + (levelButtonSize * 3), levelButtonSize, levelButtonSize), "", buttonPlay))
        {
            LoadScene(level1);
        }

        // SectionSettings
        GUI.Label(new Rect(Screen.width * sectionSettings + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * -2) + (levelButtonSize * 0), levelButtonSize, levelButtonSize),
        "Settings", labelStyleSettings);
        if (GUI.Button(new Rect(Screen.width * sectionSettings + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 1), guiButtonSize, guiButtonSize), "", buttonCheatSheet))
        {
            LoadScene(1);
        }
        if (GUI.Toggle(new Rect(Screen.width * sectionSettings + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 6), guiButtonSize, guiButtonSize), soundUI, "GUI Sound"))
        {
            soundUI = !soundUI;
            if (!soundUI)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
            print("mute1: " + soundUI);
        }
        if (GUI.Toggle(new Rect(Screen.width * sectionSettings + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 7), guiButtonSize, guiButtonSize),soundGameplay, "Gameplay Sound"))
        {
            soundGameplay = !soundGameplay;
            if (!soundGameplay)
            {
                AudioListener.volume = 0;
            } else
            {
                AudioListener.volume = 1;
            }
            print("mute2: " + soundGameplay);

        }
         GUI.HorizontalSlider(new Rect(Screen.width * sectionSettings + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 8), guiButtonSize, guiButtonSize), 0.5f, 0.0f, 1.0f);
        

        if (GUI.Button(new Rect(Screen.width * sectionSettings + (marginWidth * 1) + (levelButtonSize * 0), offsetY + (marginHeight * 16), guiButtonSize, guiButtonSize), "", buttonExit))
        {
            Application.Quit();
        }


    }

    public void LoadScene(int level)
    {
        //Application.LoadLevel(level);
        SceneManager.LoadScene(level);
    }

    public void ToggleAudio()
    {
        AudioListener.pause = !isAllAudioMuted;
        isAllAudioMuted = !isAllAudioMuted;
    }
}
