using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
using Renci.SshNet.Common;

public class ExitMenu : MonoBehaviour
{
    public GameObject panel;
    public AudioSource buttonClick;
    public AudioSource successSound;
    public GameObject changeMessage;
    private string defaultText = "";
    void Start()
    {
        defaultText = changeMessage.GetComponent<Text>().text;
    }

    /// <summary>
    /// Sends the files to our sftp.
    /// </summary>
    public void OnSend()
    {
        buttonClick.Play();
        Text guiText = changeMessage.GetComponent<Text>();
        guiText.text = "Sende Daten...";
        StartCoroutine(ExecuteAfterTime(.5f));

    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Send();
    }

    private void Send()
    {
        Text guiText = changeMessage.GetComponent<Text>();
        SFTPAccess sftp = FindObjectOfType<SFTPAccess>();
        try
        {
            bool sftpSuccess = sftp.UploadSurveyLogs();
            Debug.Log("SuccessClip=" + successSound.clip);

            if (sftpSuccess && !successSound.isPlaying)
            {
                successSound.Play();
                guiText.text = "Upload finished. Files uploaded are:\r\n";
                Debug.Log("Size b4=" + guiText.preferredWidth);
                guiText.text += sftp.GetDirectoryContent();
                Debug.Log("Size after=" + guiText.preferredWidth);
                RectTransform rTransform = changeMessage.GetComponent<RectTransform>();
                rTransform.sizeDelta = new Vector2(rTransform.sizeDelta.x, guiText.preferredHeight);
            }

        }
        catch (Exception e)
        {
            guiText.text = "SFTP Verbindung abgebrochen, bitte nochmal versuchen.\r\nSollte die Verbindung nicht hergestellt werden können, gibt es weitere Informationen auf der Webseite im Hilfebereich.";
            Debug.Log("Size after=" + guiText.preferredWidth);
            RectTransform rTransform = changeMessage.GetComponent<RectTransform>();
            rTransform.sizeDelta = new Vector2(rTransform.sizeDelta.x, guiText.preferredHeight);
        }
    }

    private int GetLines(string text)
    {
        string[] split = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
        return split.Length;
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
        changeMessage.GetComponent<Text>().text=defaultText;


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
