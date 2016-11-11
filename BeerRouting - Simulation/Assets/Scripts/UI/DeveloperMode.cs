using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeveloperMode : MonoBehaviour
{
    int devCount = 7;
    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
        float mouseDistanceToText = Vector3.Distance(this.transform.position, Input.mousePosition);
        //Debug.Log(mouseDistanceToText);
        if (Input.GetMouseButtonUp(0) && mouseDistanceToText > 10 && mouseDistanceToText < 70)
        {
            devCount--;
            Debug.Log("Soon dev " + devCount);
            if (devCount <= 0)
            {
                //unlock all
                GameState gs = FindObjectOfType<GameState>();
                //TODO: unlock all levels if needed: gs.UnlockAllLevelsCompletely();
                // Go to main menu.
                SceneManager.LoadScene(0);
            }
        }
    }
}
