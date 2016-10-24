using UnityEngine;
using System.Collections;

public class ExitMenu : MonoBehaviour
{
    public GameObject panel;
    public AudioSource buttonClick;
    public AudioSource successSound;

    void Start()
    {
    }

    /// <summary>
    /// Sends the files to our sftp.
    /// </summary>
    public void OnSend()
    {
        buttonClick.Play();
       
//        ToggleMenu();
        SFTPAccess sftp = FindObjectOfType<SFTPAccess>();
        bool sftpSuccess = sftp.UploadSurveyLogs();
        Debug.Log("SuccessClip=" + successSound.clip);
        if (sftpSuccess && !successSound.isPlaying)
        {
            successSound.Play();
        }
        //Application.Quit();

        //sftp do et nao
    }

    public void OnExit()
    {
        buttonClick.Play();
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
