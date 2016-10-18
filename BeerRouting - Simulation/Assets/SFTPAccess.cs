using UnityEngine;
using System.Collections;


using System;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Renci.SshNet.Common;

public class SFTPAccess : MonoBehaviour {
    private string gameTypeString = "sim";
    public Boolean debug = true;
    public string name = "beerrouting";
    public string password;
    public void UploadSurveyLogs()
    {
        if(password==null || password.Length < 1)
        {
            Debug.LogError("sftpaccess >> No Password specified!");
            return;
        }
        using (var client = new SftpClient("chernobog.dd-dns.de", name, password))
        {
            client.Connect();
            Debug.Log("Is connected? " + client.IsConnected);

            string baseReadWriteDir = "/surveylog/";
            string playerName = PlayerPrefs.GetString("name");

            //check if /$name exists, if not create it, then switch the workdir
            client.ChangeDirectory(baseReadWriteDir);
            if(!client.Exists(playerName))
                client.CreateDirectory(playerName);
            client.ChangeDirectory(playerName);
            //check if /$name/$type exists, if not create it, then switch the workdir
            if (!client.Exists(gameTypeString))
                client.CreateDirectory(gameTypeString);
            client.ChangeDirectory(gameTypeString);
            
            Console.WriteLine("Changed directory to {0}", baseReadWriteDir);

            //store logs


            string logDir = Application.persistentDataPath + "/SurveyLog/";
            string[] files = Directory.GetFiles(logDir);
            foreach (var uploadFile in files)
            {
                using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                {
                    client.BufferSize = 4 * 1024; // bypass Payload error large files
                    client.UploadFile(fileStream, Path.GetFileName(uploadFile), true);
                }
            }

            if (debug)
            {
                List<SftpFile> filesInDir = client.ListDirectory(client.WorkingDirectory).ToList();
                Debug.Log("No. of uploaded files in the dir: " + files.Length);
                Debug.Log("No. of files in the dir: " + filesInDir.Count);

            }

            client.Disconnect();
        }

    }


}

