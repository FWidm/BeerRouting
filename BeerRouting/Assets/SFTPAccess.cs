using UnityEngine;
using System.Collections;


using System;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Renci.SshNet.Common;

public class SFTPAccess : MonoBehaviour
{
    private string gameTypeString = "comic";
    public Boolean debug = true;
    public string username = "beerrouting";
    public string password;
    public string host= "134.60.51.204";

    public bool UploadSurveyLogs()
    {
        if (password == null || password.Length < 1)
        {
            if (debug)
                Debug.LogError("sftpaccess >> No Password specified!");
            return false;
        }
        using (var client = new SftpClient(host, username, password))
        {
            if (debug)
                Debug.Log("OperationTimeout=" + client.OperationTimeout);
            client.Connect();
            if (debug)
                Debug.Log("Is connected? " + client.IsConnected);

            string baseReadWriteDir = "/surveylog/";
            string playerName = PlayerPrefs.GetString("name");

            //check if /$name exists, if not create it, then switch the workdir
            client.ChangeDirectory(baseReadWriteDir);
            if (!client.Exists(playerName))
                client.CreateDirectory(playerName);
            client.ChangeDirectory(playerName);
            //check if /$name/$type exists, if not create it, then switch the workdir
            if (!client.Exists(gameTypeString))
                client.CreateDirectory(gameTypeString);
            client.ChangeDirectory(gameTypeString);
            
            if (debug)
                Debug.Log("Changed directory to " + baseReadWriteDir);
            // no. of files currently
            int inherentNoOfFiles = client.ListDirectory(client.WorkingDirectory).Count();

            //store logs


            string logDir = Application.persistentDataPath + "/SurveyLog/";
            string[] files = Directory.GetFiles(logDir);
            foreach (var uploadFile in files)
            {
                if (debug)
                    Debug.Log("Filename=" + uploadFile + " | contains typeString=" + gameTypeString + "? " + uploadFile.ToUpper().Contains(gameTypeString.ToUpper()));
                if (uploadFile.ToUpper().Contains(gameTypeString.ToUpper()))
                {
                    using (var fileStream = new FileStream(uploadFile, FileMode.Open))
                    {
                        client.BufferSize = 4 * 1024; // bypass Payload error large files
                        client.UploadFile(fileStream, Path.GetFileName(uploadFile), true);
                    }
                }
            }

            if (debug)
                Debug.Log(client.GetStatus(client.WorkingDirectory));
            int differenceOfFileNumbers = client.ListDirectory(client.WorkingDirectory).Count() - inherentNoOfFiles;
            if (debug)
                Debug.Log("Successful? diffInFiles=" + differenceOfFileNumbers);
            if (differenceOfFileNumbers >= 0)
            {
                client.Disconnect();
                return true;
            }
            client.Disconnect();
            return false;

        }

    }

    public string GetDirectoryContent()
    {
        if (password == null || password.Length < 1)
        {
            if (debug)
                Debug.LogError("sftpaccess >> No Password specified!");
            return null;
        }
        using (var client = new SftpClient(host, username, password))
        {
            client.Connect();
            if (debug)
                Debug.Log("Is connected? " + client.IsConnected);

            string baseReadWriteDir = "/surveylog/";
            string playerName = PlayerPrefs.GetString("name");

            //check if /$name exists, if not create it, then switch the workdir
            client.ChangeDirectory(baseReadWriteDir);
            if (!client.Exists(playerName))
                client.CreateDirectory(playerName);
            client.ChangeDirectory(playerName);
            //check if /$name/$type exists, if not create it, then switch the workdir
            if (!client.Exists(gameTypeString))
                client.CreateDirectory(gameTypeString);
            client.ChangeDirectory(gameTypeString);

            Console.WriteLine("Changed directory to {0}", baseReadWriteDir);
            // no. of files currently
            List<SftpFile> files = client.ListDirectory(client.WorkingDirectory).ToList();
            client.Disconnect();
            String ret = "";
            foreach (var item in files)
            {
                if (debug)
                    Debug.Log(">>" + item);
                if (!item.Name.StartsWith("."))
                    ret += "> " + item.Name + "\r\n";
            }
            return ret;
        }

    }
}

