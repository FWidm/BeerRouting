using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

	public void LoadScene(int level)
    {
        //Application.LoadLevel(level);
        SceneManager.LoadScene(level);
    }

    public void ExitApplicationOnClick()
    {
        Application.Quit();
    }
}
