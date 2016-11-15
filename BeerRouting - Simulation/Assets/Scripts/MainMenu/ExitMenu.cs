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
    public GameObject sendButton;

    private string defaultText = "";
    UpdateSite.PostRequestCallback callback;

    void Start()
    {
        defaultText = changeMessage.GetComponent<Text>().text;
        callback = UpdateGuiText;
    }

    void UpdateGuiText(string data)
    {
        Text guiText = changeMessage.GetComponent<Text>();
        guiText.text += "\r\n" + data;
        RectTransform rTransform = changeMessage.GetComponent<RectTransform>();
        rTransform.sizeDelta = new Vector2(rTransform.sizeDelta.x, guiText.preferredHeight);

    }

    /// <summary>
    /// Sends the files to our sftp.
    /// </summary>
    public void OnSend()
    {
        buttonClick.Play();
        Text guiText = changeMessage.GetComponent<Text>();
        guiText.text = "Sende Daten... (Das kann etwas dauern)";
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
                guiText.text = "Daten sind hochgeladen:\r\n";
                guiText.text += sftp.GetDirectoryContent();
                RectTransform rTransform = changeMessage.GetComponent<RectTransform>();
                rTransform.sizeDelta = new Vector2(rTransform.sizeDelta.x, guiText.preferredHeight);
                //TODO: check if it is called.
                UpdateSite updateSite = GetComponent<UpdateSite>();
                updateSite.UpdateWebsite(callback);
                Debug.Log("Setting guitext after website call");
                guiText.text += updateSite.information;
                sendButton.SetActive(false);
            }

        }
        catch (Exception e)
        {
            guiText.text = "Verbindung konnte nicht hergestellt werden, bitte nochmal versuchen.\r\nSollte die Verbindung nicht hergestellt werden können, nimm Kontakt mit uns auf: beercraterouting@lists.uni-ulm.de.";
            Debug.Log("Size after=" + guiText.preferredWidth);
            Debug.Log("Exitmenu >> got exception e=" + e); 
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
        //TODO Delete progress?
        //DeleteProgess();
        Application.Quit();

    }

    public void DeleteProgess(){
        string FileName = "BeerRoutingGameData.bin";
        string appDataFolder = Application.persistentDataPath;
        string saveGameStateFilePath = appDataFolder + "/BeerRoutingData/" + FileName;
        if (File.Exists(saveGameStateFilePath))
            {
                File.Delete(saveGameStateFilePath);
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
        changeMessage.GetComponent<Text>().text = defaultText;


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
