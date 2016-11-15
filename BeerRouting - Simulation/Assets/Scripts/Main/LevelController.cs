using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class LevelController : MonoBehaviour
{
    protected GameMovementManager movementManager;
    //	private GameManagerInterface gameManager;

    protected ProfessorController professorController;
    protected LevelProperties levelProperties;
    protected GameState gameState;

    protected int lastSequence;
    protected int lastState;
    protected PathScript clickedPath;

    protected string prevRouterName;
    protected bool gameInputEnabled;
    protected bool playerWalking;
    protected bool showNormalProfOnStart;

    public bool levelStart
    {
        get;
        set;
    }

    public bool debugging = false;

    // Use this for initialization
    void Awake()
    {
        // Init components.
        professorController = FindObjectOfType<ProfessorController>();
        levelProperties = FindObjectOfType<LevelProperties>();
        gameState = FindObjectOfType<GameState>();
        movementManager = FindObjectOfType<GameMovementManager>();

        // Don't show professor button on tutorial start.
        FindObjectOfType<ProfessorButton>().SetVisible(false);
        // Wait a few seconds on level start, then show professor.
        StartCoroutine(ShowProfessorAfterTime());
        // Disable game play input on tutorial start.
        gameInputEnabled = false;
        playerWalking = false;
        showNormalProfOnStart = true;
    }

    public void UpdateProfessor()
    {
        // Hide professor at the end of a sequence.
        professorController.Show(false);
        professorController.ShowBeer(false, 0);
        professorController.ShowAngry(false);
        professorController.ShowMoney(false);

        // Set last professor sequence and state after errors were shown.
        int sId = professorController.GetCurrentSequenceId();
        if (sId < 0)
        {
            professorController.SetSequenceAndState(lastSequence, lastState);
        }
    }

    public void OnStopPlayerDrink()
    {
        professorController.ShowStars();
    }

    public void FinishLevel()
    {

        if (debugging)
            Debug.Log("Level finished.");
        professorController.ShowStars();

        DijkstraMovementManager DMM = FindObjectOfType<DijkstraMovementManager>();
        if (DMM != null)
        {
            DMM.WriteToLogAppendScore("Level Finished; Player has finished the level");
            ProfessorButton pButton = FindObjectOfType<ProfessorButton>();
            OnClickRoutingTable rButton = FindObjectOfType<OnClickRoutingTable>();
            DMM.WriteToLog("Summary; Printing the Summary of stats for this run:");
            DMM.WriteToLog("");
            DMM.WriteToLog("Buttons; Professor Button clicks= " + pButton.GetClickCount());
            DMM.WriteToLog("Buttons; Routing Table Button clicks=" + rButton.GetClickCount());
            //errors
            DMM.WriteToLog("Errors; #Error Recovery= " + DMM.countErrorRecovery);
            DMM.WriteToLog("Errors; #NOPs = " + DMM.countNoOp);
            DMM.WriteToLog("Errors; #Wrong Hops= " + DMM.countWrongHop);
            DMM.WriteToLog("Errors; #UndiscoveredPaths= " + DMM.countUndiscoveredPaths);
            //sound -> AudioMenuMain     private readonly string MASTER_VOL = "MasterVol", GAME_VOL = "GameVol", UI_VOL = "UiVol", PROF_VOL = "ProfVol", BACKGROUND_VOL = "BackgroundVol";
            DMM.WriteToLog("Sound; Master: " + PlayerPrefs.GetFloat(AudioMenuMain.MASTER_VOL));
            DMM.WriteToLog("Sound; Game: " + PlayerPrefs.GetFloat(AudioMenuMain.GAME_VOL));
            DMM.WriteToLog("Sound; UI: " + PlayerPrefs.GetFloat(AudioMenuMain.UI_VOL));
            DMM.WriteToLog("Sound; Professor: " + PlayerPrefs.GetFloat(AudioMenuMain.PROF_VOL));
            DMM.WriteToLog("Sound; Background: " + PlayerPrefs.GetFloat(AudioMenuMain.BACKGROUND_VOL));




        }
        // TODO Move and zoom camera to player!?

        // Play beer drink animation after a short time.
        //StartCoroutine(DrinkBeerAfterTime());
    }

    public void OnLevelComplete()
    {
        if (debugging)
            Debug.Log("LevelController: OnLevelComplete called. Start saving state.");

        // Save level state.
        SaveLevelState();

        // Go to main menu.
        SceneManager.LoadScene(2);
    }

    private void SaveLevelState()
    {
        if (debugging)
        {
            Debug.Log("LevelController: In SaveLevelState.");
        }

        // Get current level state.
        LevelState levelState;
        int score = FindObjectOfType<ScoreBeer>().GetScore();
        int numberOfStars = professorController.speechBubble.GetNumberOfStars();
        int levelId = levelProperties.levelId;
        levelState = new LevelState(levelId, numberOfStars, score);

        // Get saved level state.
        LevelState savedState = gameState.GetLevelStateByLevelId(levelId);

        // Only save new level state if result is better.
        if (savedState != null)
        {
            if (savedState.PlayerScore >= score)
            {
                return;
            }
        }

        if (debugging)
            Debug.Log("LevelController: Replace that entry and write the state new.");

        gameState.AddOrReplaceLevelState(levelState);
        gameState.StoreGameState();

        // gameState.AddLevelState();
    }

    public void OnStartPlayerWalking()
    {
        playerWalking = true;
        gameInputEnabled = false;
    }

    public string GetClickedPathToName()
    {
        return clickedPath.to.GetComponent<RouterScript>().routerName;
    }

    public string GetClickedPathFromName()
    {
        return clickedPath.from.GetComponent<RouterScript>().routerName;
    }

    IEnumerator DrinkBeerAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(0.5f);
        // Then play drink beer animation.
        FindObjectOfType<PlayerController>().Drink();
    }

    IEnumerator ShowProfessorAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(1);
        // Then show professor.
        if (showNormalProfOnStart)
            professorController.Show(true);
    }

    public bool IsGameInputEnabled()
    {
        return gameInputEnabled;
    }

    public void SetGameInputEnabled(bool enabled)
    {
        gameInputEnabled = enabled;
    }

    public void SetPlayerWalking(bool walking)
    {
        playerWalking = walking;
    }

    public bool IsPlayerWalking()
    {
        return playerWalking;
    }

    public string GetPrevRouterName()
    {
        return prevRouterName;
    }

    public static LevelController GetCurrentLevelController()
    {
        // Init current level controller.
        LevelController levelController = null;
        levelController = FindObjectOfType<TutorialControllerDijkstra>();
        if (levelController == null)
            levelController = FindObjectOfType<TutorialControllerGreedy>();
        if (levelController == null)
            levelController = FindObjectOfType<TutorialControllerUniformCosts>();
        if (levelController == null)
            levelController = FindObjectOfType<TutorialControllerHotPotato>();
        if (levelController == null)
            levelController = FindObjectOfType<LevelControllerDijkstra>();
        if (levelController == null)
            levelController = FindObjectOfType<LevelControllerGreedy>();
        if (levelController == null)
            levelController = FindObjectOfType<LevelControllerUniformCosts>();
        if (levelController == null)
            levelController = FindObjectOfType<LevelControllerHotPotato>();
        return levelController;
    }

    public abstract void OnProfessorButtonClick();

    // Implement method behaviour in specific level controller subclass.
    public abstract void OnPathClicked(PathScript path);

    public abstract string GetCurrentRouterName();

    public abstract void OnStopProfessorDisappear();

    public abstract void OnEnterPlayerIdle();

    public void OnStopPlayerWalking()
    {
        playerWalking = false;
        if (!professorController.IsVisible())
        { 
            gameInputEnabled = true;
        }
    }
}
