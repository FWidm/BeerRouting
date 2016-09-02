using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResetMenu : MonoBehaviour
{

    public GameObject panel;
    public GameState gameState;
    private AudioSource buttonClick;

    void Start()
    {
        buttonClick = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Resets al Level and locks all.
    /// </summary>
    public void OnYes()
    {
        ToggleMenu();
        gameState.ResetGameState();        
    }

    /// <summary>
    /// FUN - random function if game states are deleted or not.
    /// </summary>
    public void OnPerhaps()
    {
        if (Random.Range(0, 2) == 1)
        {
            OnYes();
        }
        else
        {
            ToggleMenu();
        }
    }

    /// <summary>
    /// Toggles the image panel.
    /// </summary>
    public void ToggleMenu()
    {
        if (buttonClick != null)
            buttonClick.Play();
        if (!panel.activeSelf)
            panel.SetActive(true);
        else
            StartCoroutine(CloseAfterTime());

    }

    IEnumerator CloseAfterTime()
    {
        // Wait a short time.
        if (buttonClick != null)
            yield return new WaitForSeconds(buttonClick.clip.length);
        // Then close panel.
        panel.SetActive(false);
    }
}
