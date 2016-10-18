using UnityEngine;
using System.Collections;

public class ExitMenu : MonoBehaviour {
    public GameObject panel;
    private AudioSource buttonClick;

    void Start()
    {
        buttonClick = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Sends the files to our sftp.
    /// </summary>
    public void OnSend()
    {
        ToggleMenu();
        SFTPAccess sftp = FindObjectOfType<SFTPAccess>();
        sftp.UploadSurveyLogs();
        Application.Quit();

        //sftp do et nao
    }

    public void OnExit()
    {
        ToggleMenu();
        Application.Quit();

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
