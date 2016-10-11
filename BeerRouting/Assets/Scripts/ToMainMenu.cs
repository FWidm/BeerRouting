using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToMainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void LoadScene(int level)
    {
        SceneManager.LoadScene(level);

    }
}

