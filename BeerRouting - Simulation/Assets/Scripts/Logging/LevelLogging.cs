using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.IO;

public class LevelLogging
{
    StringBuilder stringBuilder;
    String filePath;
    const string directoryName = "SurveyLog";
    bool isLoggingEnabled = true;

    public LevelLogging(String levelName, String levelInformation)
    {
        string appDataFolder = Application.persistentDataPath;

        String fileName = getTimeStamp().Replace(":",".") + " " + levelName + ".brlog";
        fileName = fileName.Replace(" ", "_");
        Debug.Log("[Levellogging] directory is: "+directoryName);
        filePath = appDataFolder + "/" + directoryName + "/" + fileName;
        stringBuilder = new StringBuilder();
        createDirectory();
        Debug.Log("[LevelLogging] file is:" + filePath);
        string directoryBeerRoutingData = appDataFolder + "/" + directoryName;
        Debug.Log("LevelLogging] dir exists? " + Directory.Exists(directoryBeerRoutingData) + "; file exists?" + File.Exists(filePath));

        using (FileStream fs = new FileStream(filePath, FileMode.Append))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("Level=" + levelInformation + "\t current Time=" + getTimeStamp());
            }
        }
    }

    public LevelLogging(String levelName, String levelInformation, String UserName)
    {
        string appDataFolder = Application.persistentDataPath;

        String fileName = UserName+"_sim_"+levelName+"_"+getTimeStamp().Replace(":","-")+".brlog";
        Debug.Log("[Levellogging] directory is: " + directoryName);
        filePath = appDataFolder + "/" + directoryName + "/" + fileName;
        stringBuilder = new StringBuilder();
        createDirectory();
        Debug.Log("[LevelLogging] file is:" + filePath);
        string directoryBeerRoutingData = appDataFolder + "/" + directoryName;
        Debug.Log("LevelLogging] dir exists? " + Directory.Exists(directoryBeerRoutingData) + "; file exists?" + File.Exists(filePath));

        using (FileStream fs = new FileStream(filePath, FileMode.Append))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("Username=" + UserName + ";\t Level=" + levelInformation + "\t current Time=" + getTimeStamp());
            }
        }
    }

    private String getTimeStamp()
    {
        return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
    }

    /// <summary>
    /// Appends the specified string to the stringBuilder followed by a newline.
    /// </summary>
    /// <param name="s">S.</param>
    public void AppendLine(String s)
    {
//        stringBuilder.AppendLine(getTimeStamp() + ": " + s);
        using (FileStream fs = new FileStream(filePath, FileMode.Append))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(getTimeStamp() + ": " + s);
            }
        }
    }

    /// <summary>
    /// Append the specified string to the stringBuilder.
    /// </summary>
    /// <param name="s">S.</param>
    public void Append(String s)
    {
//        stringBuilder.Append(getTimeStamp() + ": " + s);
        using (FileStream fs = new FileStream(filePath, FileMode.Append))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(getTimeStamp() + ": " + s);
            }
        }
    }


    /// <summary>
    /// Creates the directory for storing the game data if it hasn't been already created.
    /// If directory is already there, no action is taken.
    /// </summary>
    private void createDirectory()
    {
        Debug.Log("Levellogging: Trying to create the directory " + directoryName + ".");

        // Create directory BeerRoutingData if it is not already there.
        try
        {
            string appDataFolder = Application.persistentDataPath;
            string directoryBeerRoutingData = appDataFolder + "/" + directoryName;
            Debug.Log("Levellogging: Complete path of dir=" + directoryBeerRoutingData);
            if (!Directory.Exists(directoryBeerRoutingData))
            {
                Directory.CreateDirectory(directoryBeerRoutingData);

                if (isLoggingEnabled)
                    Debug.Log("Levellogging: Created the directory "+directoryName+".");
            }
                
        }
        catch (IOException ex)
        {
            Debug.LogError("Error occurred in createDirectory function: ");
            Debug.LogError(ex.Message);

            Debug.Log("Error message: " + ex.Message + ". " + Environment.NewLine +
                "StackTrace: " + ex.StackTrace);
        }
    }
}
