using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameState : MonoBehaviour
{
    /// <summary>
    /// Indicates whether logging statements should be printed. 
    /// </summary>
    private bool isLoggingEnabled = false;

    /// <summary>
    /// Holds the states of the various levels of the game.
    /// Maps level states to level ids.
    /// </summary>
    private Dictionary<int, LevelState> levels;

    /// <summary>
    /// The name of the file that keeps the game state in a persistent manner.
    /// </summary>
    private const string FileName = "BeerRoutingGameData_Sim.bin";
    private const string DebugFile = "JSONDebug.txt";
    private const string ErrorLogFile = "ErrorLog.txt";

    private string saveDebugFilePath = string.Empty;
    private string saveGameStateFilePath = string.Empty;
    private string errorLogFilePath = string.Empty;

    void Awake()
    {
        levels = new Dictionary<int, LevelState>();

        // Set up the paths to the txt files.
        // Get the path to the application data folder.
        // Application.persistentDataPath should work on every plattform, except Web.
        string appDataFolder = Application.persistentDataPath;
        saveDebugFilePath = appDataFolder + "/BeerRoutingData/" + DebugFile;
        saveGameStateFilePath = appDataFolder + "/BeerRoutingData/" + FileName;
        errorLogFilePath = appDataFolder + "/ErrorLog/" + ErrorLogFile;

        // Create directory if not already there.
        createDirectory();

        // Create files for saving the data if they not already exist.
        createSaveFiles();

        if (isLoggingEnabled)
        {
            Debug.Log("JsonFilePath: " + saveGameStateFilePath);
            Debug.Log("DebugFilePath: " + saveDebugFilePath);
            Debug.Log("ErrorLogFilePath: " + errorLogFilePath);
        }

        // UnlockAllLevelsCompletely();
    }

    // Called on Startup.
    void Start()
    {
        if (isLoggingEnabled)
            Debug.Log("GameState script is started in scene: " + SceneManager.GetActiveScene().name);

        if (isLoggingEnabled)
        {
            // Store to debug file.
            using (FileStream fs = new FileStream(saveDebugFilePath, FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("");
                    writer.WriteLine("------");
                    writer.WriteLine("Start debug log file for scene: " + SceneManager.GetActiveScene().name + " -> " + System.DateTime.Now.ToString("yyyy-MM-dd (HH:mm:ss:ffff)"));
                    writer.WriteLine("");
                }
            }
        }

        // Read the current game state.
        ReadGameState();
    }

    public void UnlockAllLevels()
    {
        for (int i = 0; i < 24; i++)
        {
            LevelState levelState = new LevelState(i, 1, 45);
            AddOrReplaceLevelState(levelState);
        }
        StoreGameState();
    }

    public void UnlockAllLevelsCompletely()
    {
        for (int i = 0; i < 24; i++)
        {
            LevelState levelState = new LevelState(i, 3, 100);
            AddOrReplaceLevelState(levelState);
        }
        StoreGameState();
    }

    /// <summary>
    /// Resets all levels
    /// </summary>
    public void ResetGameState()
    {
        levels.Clear();
        LevelState levelState = new LevelState(0, 0, 0);
        AddOrReplaceLevelState(levelState);
        StoreGameState();
        // Go to main menu.
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Creates the directory for storing the game data if it hasn't been already created.
    /// If directory is already there, no action is taken.
    /// </summary>
    private void createDirectory()
    {
        // Create directory BeerRoutingData if it is not already there.
        try
        {
            string appDataFolder = Application.persistentDataPath;
            string directoryBeerRoutingData = appDataFolder + "/BeerRoutingData";
            if (!Directory.Exists(directoryBeerRoutingData))
            {
                Directory.CreateDirectory(directoryBeerRoutingData);

                if (isLoggingEnabled)
                    Debug.Log("GameState: Created the directory BeerRoutingData.");
            }

            string directoryErrorLogs = appDataFolder + "/ErrorLog";
            if (!Directory.Exists(directoryErrorLogs))
            {
                Directory.CreateDirectory(directoryErrorLogs);

                if (isLoggingEnabled)
                    Debug.Log("GameState: Created the directory ErrorLog.");
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("Error occurred in createDirectory function: ");
            Debug.LogError(ex.Message);

            writeTextToErrorLog("Error message: " + ex.Message + ". " + Environment.NewLine +
                "StackTrace: " + ex.StackTrace);
        }
    }

    /// <summary>
    /// Creates the files for storing the game data and the debug information. 
    /// The files are only created if they are not already there.
    /// </summary>
    private void createSaveFiles()
    {
        // Create files if they are not already there.
        try
        {
            if (!File.Exists(saveGameStateFilePath))
            {
                File.Create(saveGameStateFilePath);
                if (isLoggingEnabled)
                    Debug.Log("GameState: Created file for saving the game state.");
                //Show Help screen
                SceneManager.LoadScene(1);

               
            }
            if (!File.Exists(saveDebugFilePath))
            {
                File.Create(saveDebugFilePath);

                if (isLoggingEnabled)
                    Debug.Log("GameState: Created the file for the debug.");
            }
            if (!File.Exists(errorLogFilePath))
            {
                File.Create(errorLogFilePath);

                if (isLoggingEnabled)
                    Debug.Log("GameState: Created the file for the error logs.");
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("Error occurred in createSaveFiles function: ");
            Debug.LogError(ex.Message);

            writeTextToErrorLog("Error message: " + ex.Message + ". " + Environment.NewLine +
                "StackTrace: " + ex.StackTrace);
        }
    }

    /// <summary>
    /// Adds an LevelState instance to the corresponding list if there is no
    /// instance of LevelState for this level, otherwise replaces the currently 
    /// stored LevelState instance.
    /// </summary>
    /// <param name="levelState">The LevelState instance that contains the state of the level.</param>
    public void AddOrReplaceLevelState(LevelState levelState)
    {
        bool replaced = true;
        if (levels.ContainsKey(levelState.LevelId))
        {
            // Replace existing entry.
            levels[levelState.LevelId] = levelState;
        }
        else
        {
            // Add as a new entry.
            replaced = false;
            levels.Add(levelState.LevelId, levelState);
        }

        if (isLoggingEnabled)
        {
            // Store to debug file.
            using (FileStream fs = new FileStream(saveDebugFilePath, FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd (HH:mm:ss:ffff)") + "| scene: " + SceneManager.GetActiveScene().name + " | Replaced: " + levelState + "? " + replaced + ".");
                }
            }
        }
    }

    /// <summary>
    /// Get the list of all LevelState levels that are currently stored in the GameState.
    /// </summary>
    /// <returns>A list of LevelState instances.</returns>
    public List<LevelState> GetLevelStates()
    {
        return levels.Values.ToList<LevelState>();
    }

    /// <summary>
    /// Returns the level state of the level that is identified by the specified level name.
    /// </summary>
    /// <param name="levelId">The id of the level whose state should be extracted.</param>
    /// <returns>The level state as an instance of LevelState, or null if level state could not be extracted.</returns>
    public LevelState GetLevelStateByLevelId(int levelId)
    {
//        if (isLoggingEnabled)
//            Debug.Log("levels.ContainsKey: " + levelId + " " + levels.ContainsKey(levelId));
        if (levels.ContainsKey(levelId))
        {            
            return levels[levelId];
        }
        return null;
    }

    /// <summary>
    /// Stores the current game state to file.
    /// </summary>
    public void StoreGameState()
    {
        if (isLoggingEnabled)
            Debug.Log("Start writing game state into file.");

        // Store the current level states as a json string.
        LevelState[] levelStates = levels.Values.ToArray<LevelState>();
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("[");
        for (int i = 0; i < levelStates.Length; i++)
        {
            sb.Append(JsonUtility.ToJson(levelStates[i]));
            if (i != (levelStates.Length - 1))
            {
                sb.AppendLine(",");
            }
            else
            {
                sb.AppendLine("");
            }
        }
        sb.Append("]");

        // Store to file.
        using (FileStream fs = new FileStream(saveGameStateFilePath, FileMode.Truncate))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                string base64String = Convert.ToBase64String(stringBytes);

                try
                {
                    formatter.Serialize(fs, base64String);
                }
                catch (ArgumentNullException ex)
                {
                    Debug.LogError("GameState: Argument null exception during serialization of game state." + ex.Message);

                    writeTextToErrorLog("Error message: " + ex.Message + " StackTrace is: " + ex.StackTrace);
                }
                catch (SerializationException serEx)
                {
                    Debug.LogError("GameState: SerializationException during serialization of game state." + serEx.Message);

                    writeTextToErrorLog("Error message: " + serEx.Message + " StackTrace is: " + serEx.StackTrace);
                }
                catch (SecurityException secEx)
                {
                    Debug.LogError("GameState: SecurityException during serialization. User has not the required rights. " + secEx.Message);

                    writeTextToErrorLog("Error message: " + secEx.Message + " StackTrace is: " + secEx.StackTrace);
                }

                if (isLoggingEnabled)
                    Debug.Log("GameState: Stored json string in file. The json file was: " + sb.ToString());
            }
        }

        // Store to debug file.
        if (isLoggingEnabled)
        {
            using (FileStream fs = new FileStream(saveDebugFilePath, FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("");
                    writer.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd (HH:mm:ss:ffff)") + "| scene: " + (SceneManager.GetActiveScene().name) + " | Write: ");
                    writer.WriteLine("\t -> JSON: " + sb.ToString());
                    writer.WriteLine("");
                }
            }  
        }

    }

    /// <summary>
    /// Retreive the current state of the game from the stored state files.
    /// </summary>
    public void ReadGameState()
    {
        // Read from file.
        using (FileStream fs = new FileStream(saveGameStateFilePath, FileMode.Open))
        {
            // Check if file is empty. If not, read content.
            if (fs.Length != 0)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                string jsonString = null;
                try
                {
                    jsonString = formatter.Deserialize(fs) as string;
                }
                catch (ArgumentNullException ex)
                {
                    Debug.LogError("GameState: Argument null exception during deserialization of game state." + ex.Message);

                    writeTextToErrorLog("Error message: " + ex.Message + " StackTrace is: " + ex.StackTrace);
                }
                catch (SerializationException serEx)
                {
                    Debug.LogError("GameState: SerializationException during deserialization of game state." + serEx.Message);

                    writeTextToErrorLog("Error message: " + serEx.Message + " StackTrace is: " + serEx.StackTrace);
                }
                catch (SecurityException secEx)
                {
                    Debug.LogError("GameState: SecurityException during deserialization. User has not the required rights. " + secEx.Message);

                    writeTextToErrorLog("Error message: " + secEx.Message + " StackTrace is: " + secEx.StackTrace);
                }

                if (jsonString == null)
                {
                    Debug.Log("Couldn't read the json string from file.");
                    return;
                }

                byte[] stringBytes = Convert.FromBase64String(jsonString);
                jsonString = Encoding.UTF8.GetString(stringBytes, 0, stringBytes.Length);

                // Parse json string to level states.
                foreach (var jsonLine in jsonString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string line = jsonLine;
                    if (line == "[" || line == "]")
                    {
                        continue;
                    }

                    line = line.Trim();
                    if (line.EndsWith(","))
                    {
                        line = line.Substring(0, line.Length - 1);
                    }

                    if (isLoggingEnabled)
                        Debug.Log("GameState: Current read json line is: " + line);

                    // Parse json line.
                    LevelState levelState = JsonUtility.FromJson<LevelState>(line);
                    AddOrReplaceLevelState(levelState);
                }
            }
            else
            {
                if (isLoggingEnabled)
                    Debug.Log("GameState: File is empty.");
            }
        }

        if (isLoggingEnabled)
        {
            // Store to debug file.
            Debug.Log("GameState: Game data loaded. The current list of level states has " + levels.Values.ToList().Count + " elements.");

            using (FileStream fs = new FileStream(saveDebugFilePath, FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.WriteLine("");
                    foreach (var levelState in levels.Values)
                    {
                        writer.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd (HH:mm:ss:ffff)") + "| scene: " + SceneManager.GetActiveScene().name + " | Read << " + levelState);
                    }
                    writer.WriteLine("");
                }
            }
        }
    }

    /// <summary>
    /// Writes a text to the error log file.
    /// </summary>
    /// <param name="text">The text.</param>
    private void writeTextToErrorLog(string text)
    {
        // Store to error log file.
        using (FileStream fs = new FileStream(errorLogFilePath, FileMode.Append))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine("Error in scene: " + SceneManager.GetActiveScene().name + " -> " + System.DateTime.Now.ToString("yyyy-MM-dd (HH:mm:ss:ffff)"));
                writer.WriteLine(" -> " + text);
                writer.WriteLine("");
            }
        }
    }
}

