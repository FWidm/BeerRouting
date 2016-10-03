using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SideMenu : MonoBehaviour {

    public bool isAllAudioMuted = false;

    public void ExitApplicationOnClick()
    {
        Application.Quit();
    }

    public void LoadScene(int level)
    {
        StartCoroutine(LoadSceneAfterTime(level));
    }

    public void ToggleAudio()
    {
        AudioListener.pause = !isAllAudioMuted;
        isAllAudioMuted = !isAllAudioMuted;
    }

    IEnumerator LoadSceneAfterTime(int level)
    {
        // Wait until button click sound is over.
        yield return new WaitForSeconds(0.062f);
        // Then load scene.    
        SceneManager.LoadScene(level);
    }
}
