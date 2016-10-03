using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine.UI;
using UnityEngine.Assertions;

/// <summary>
/// **********************************************************************
/// *********** Processing steps *****************************************
/// 1) Start: Initializing selected game mode.
/// 2) PerformMoveOnPath: Called every time a user clicks on a path.
///     Determines status for current hop. Calls methods in GameManager depending
///     on status to perform actions. The methods of the game manager are implemented
///     depending on the game mode, i.e. the different status like ValidHop, ForbiddenHop, WrongHop, etc. 
///     are handled depending on the current game mode.
///     => Every hop has a "ArrivedAtTarget" event which is handled depending on the game mode. This
///         can include displaying objects like path costs, etc. Not every game mode requires actions in
///         this case, so its optional to handle this event. However, it is required to handle the run finished
///         case in the "ArrivedAtTarget" event for all game modes. The handling of the run finished case is splitted
///         in two parts which are described in the following point.
/// 3) Special Case: Run Finished Handling.
///     => In PerformMoveOnPath: Actions that are directly executed at the beginning of the hop, i.e.
///                                 when the player starts walking. This includes updating the player object to indicate
///                                 success or failure as well as updating the player's score. It is also checked whether 
///                                 there is a next run or whether the level is already finished.
///     => In ArrivedAtTarget: If the level is not yet marked as finished, i.e. there is a further run, the next run needs to be
///                             prepared and started depending on the current game mode. Every game mode needs to provide an
///                             implementation for this case.
/// </summary>
public class GameMovementManager : MonoBehaviour, MovementManagerInterface
{
    public bool isLogEnabled = true;

    // List of start and end points which need to be handled in the level.
    public List<GameObject> destinationPoints;
    public GameObject startRouter;

    // Specifies which router can be disabled in the level.
    public GameObject disableRouter;

    // Indicates in which run the specified router gets disabled.
    public int disableInRun = -1;

    // Indicates the current game mode, i.e. the routing approach.
    private LevelProperties.GameType gameType;

    // The GameManager contains the logic for execution of a run of the corresponding routing approach.
    private GameManagerInterface gameManager;

    // Reference to the movement script of the player.
    private MovementScript movementScript;

    // A reference to the score beer script.
    private ScoreBeer scoreBeer;

    // The Player controller reference to control the appearance of the player at runtime.
    private PlayerController playerController;

    // A reference to the ScoreTextScript for displaying pop up information about current score events.
    private ScoreTextScript scoreTexts;

    // Indicates the index of the current run.
    private int runIndex = 0;

    // The status that was returned by the game manager for the last move.
    private GameStatus lastMoveStatus;

    // Indicates whether an error is recovered in an error recovery phase.
    private bool errorRecovered;

    // Caches the last path the player has performed an action on.
    private PathScript lastPath;

    // Indicates whether the player should be forced to walk back due to a failed quick time event.
    private bool walkBackOnQteFailure = false;

    // Inidcates whether the player is currently recovering from a qte failure.
    private bool isRecoveringFromQteFailue = false;

    // Reference to the blocker game objects.
    private GameObject blockers;

    #region UniformCostRelevantFields

    // UI Texts for routing table used in Uniform Cost game mode.
    private RoutingTableUI routingTableUI;

    #endregion UniformCostRelevantFields

    #region HotPotatoRelevantFields

    // Specifies the path from a router to an autonomous system in the HotPotato
    private PathScript autonomousSystemHotPotatoPath;

    // Reference to the timer that is relevant for the game mode HotPotato.
    private HotPotatoTimer timer;

    // The timer GUI text field.
    private Text timerGUI;

    // Indicates whether the pre load move is currently performed in the HotPotato game mode.
    private bool performsPreLoadMoveOnHotPotato;

    // All the game objects of room barkeepers in the current level.
    private GameObject[] roomBarkeepers;

    #endregion HotPotatoRelevantFields

    //Log the GameMovement of one level
    private bool logToFile=true;
    private LevelLogging levelLogging;

    private SoundManager soundManager;

    void Awake()
    {
        // Let blockers disappear.
        blockers = GameObject.FindGameObjectWithTag("RouterBlockers");
        if (blockers != null)
            blockers.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        if (isLogEnabled)
            Debug.Log("Start GameMovementManager.");

        soundManager = FindObjectOfType<SoundManager>();

        Assert.IsTrue(disableInRun < destinationPoints.Count, "disableInRun will never be reached because it has to be smaller than the amount of runs.");
        Assert.IsTrue(disableInRun != 0, "Can't disable a router in run 0. Please provide a number greater than 0.");
        runIndex = 0;

        errorRecovered = false;

        // Get reference to the movement script.
        movementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementScript>();

        // Get reference to the ScoreTextManager.
        GameObject scoreTextGO = GameObject.FindGameObjectWithTag("ScoreTextManager");
        scoreTexts = scoreTextGO.GetComponent<ScoreTextScript>();

        scoreBeer = FindObjectOfType<ScoreBeer>();
        playerController = FindObjectOfType<PlayerController>();

        LevelProperties levelProperties = FindObjectOfType<LevelProperties>();

        //Set the Levelscore Maximum
        gameType = levelProperties.gameType;
        levelProperties.SetLevelMaxScore(20 * (destinationPoints.Count));
        // Set max score for this level.
        int maxScore = levelProperties.levelMaxScore;
        FindObjectOfType<ScoreBeer>().SetMaxScore(maxScore);

        // Register for arrival at target position event of movementScript.
        movementScript.SubscribeToTargetPositionArrivedEvent(ArrivedAtTargetPosition);

        // Initialize game mode.
        switch (gameType)
        {
            case LevelProperties.GameType.Greedy:
                initializeGreedyGameMode();
                break;
            case LevelProperties.GameType.UniformCost:
                initializeUniformCostGameMode();
                break;
            case LevelProperties.GameType.HotPotato:
                initializeHotPotatoGameMode();
                break;
        }

        // Register for quick time events.
        QuickTimeScript qteScript = FindObjectOfType<QuickTimeScript>();
        if (qteScript != null)
        {
            qteScript.SubscribeToQteFailedEvent(QteFailure);
            qteScript.SubscribeToQteSucceededEvent(QteSuccess);
        }
        if (logToFile)
        {
            Debug.Log("Logging to file!");
            levelLogging = new LevelLogging(levelProperties.levelName, " MaxScore=" + levelProperties.levelMaxScore + ", Type=" + levelProperties.gameType);
        }
    }

    /// <summary>
    /// Performs a move action along the selected path. The semantics
    /// of the move depend on the chosen game mode. 
    /// </summary>
    /// <param name="path">The selected path on which the player 
    ///     will perform the move action.</param>
    public void PerformMoveOnPath(PathScript path)
    {
        String pos = "CurrentPos=" + gameManager.GetCurrentPlayerPosition().routerName + ", ";
        // Determine the status of the hop depending on the game mode implementation.
        GameStatus hopStatus = gameManager.IsValidHop(path);
        lastMoveStatus = hopStatus;

        if (hopStatus != GameStatus.ForbiddenHop)
            lastPath = path;

        if (isLogEnabled)
            Debug.Log(hopStatus);


        switch (hopStatus)
        {
            case GameStatus.ValidHop:
                if (isLogEnabled)
                    Debug.Log("Its a valid hop.");

                // Special case in hot potato game mode.
                if (gameType == LevelProperties.GameType.HotPotato)
                {
                    handleValidHopHotPotato(path);     // Needs to be called before PerformHop.
                }

                movePlayerAlongPath(path);  // Needs to be called before PerformHop.

                gameManager.PerformHop(path);
                movementScript.ControlPuddleSpawning(true);

                playerController.SetMouth(1);
                break;
            case GameStatus.InvalidHop:
                if (isLogEnabled)
                    Debug.Log("Its an invalid hop.");

                movePlayerAlongPath(path);  // Needs to be called before PerformWrongHop.

                gameManager.PerformWrongHop(path);

                playerController.SetMouth(2);

                if (isLogEnabled)
                    Debug.Log("Remove 5 points.");

                scoreBeer.UpdateScore(-5);
                scoreTexts.DisplayScoreText(ScoreText.Minus5);
                break;
            case GameStatus.RunFinished:
                if (isLogEnabled)
                    Debug.Log("The run is finished.");
                movePlayerAlongPath(path);      // Needs to be called before PerformHop.

                gameManager.PerformHop(path);

                // Handle run finished. Note: Next run not yet started.
                // Next run will start if the player has arrived at the destination.
                switch (gameType)
                {
                    case LevelProperties.GameType.HotPotato:
                        handleHotPotatoRunFinished(path);
                        break;
                    case LevelProperties.GameType.UniformCost:
                        handleUniformCostRunFinished(path);
                        break;
                    default:
                        // Default run finished.
                        performDefaultRunFinishedHandling();
                        break;
                }

                // Check if there is a further run.
                // If there is no further run, mark level as finished, so that the professor appears.
                // <= 1 because there is only a next run if there are 2 routers left. 
                // If level is marked as finished, no further run will be started in ArrivedAtTargetPosition.
                if (destinationPoints.Count <= 1)
                {
                    if (isLogEnabled)
                        Debug.Log("Level is finished.");

                    lastMoveStatus = GameStatus.LevelFinished;

                    //writeFileLog
                    if (logToFile)
                    {
                        if (levelLogging != null)
                        {
                            String s = "level finished in run=" + runIndex + ", current score=" + scoreBeer.GetScore() + ", saving levelstate.";
                            //write to file
                            levelLogging.AppendLine(s);
                            //                            levelLogging.WriteFile();
                        }
                    }
                }

                movementScript.ControlPuddleSpawning(true);
                break;
            case GameStatus.ForbiddenHop:
                if (isLogEnabled)
                    Debug.Log("The hop is forbidden.");
                break;
            case GameStatus.ErrorRecoveryHop:
                if (isLogEnabled)
                    Debug.Log("Its an error recovery hop.");
                movePlayerAlongPath(path);

                errorRecovered = gameManager.PerformErrorRecoveryHop(path);

                if (!errorRecovered)
                {
                    scoreBeer.UpdateScore(-5);
                    scoreTexts.DisplayScoreText(ScoreText.Minus5);
                }

                break;
        }

        //writeFileLog
        if (logToFile)
        {
            if (levelLogging != null)
            {
                String s = pos + " path=" + path.name + ", hopStatus=" + hopStatus;
                levelLogging.AppendLine(s);
            }
        }
    }

    /// <summary>
    /// The method is called every time the player arrives at its target position in a player move.
    /// </summary>
    public void ArrivedAtTargetPosition()
    {
        if (isLogEnabled)
            Debug.Log("Arrived at target position.");

        // Handle QTE failures.
        if (walkBackOnQteFailure && !isRecoveringFromQteFailue)
        {
            if (isLogEnabled)
                Debug.Log("Case walkBackOnQteFailure.");

            // Force player to walk back on qte failure.
            movePlayerAlongPath(lastPath);
            // Set is recovering from qte failure.
            isRecoveringFromQteFailue = true;
            // Reset walkBackOnQteFailure.
            walkBackOnQteFailure = false;
            return;
        }
        else if (isRecoveringFromQteFailue)
        {
            if (isLogEnabled)
                Debug.Log("Case isRecoveringFromQteFailue");

            // Reset recovery mode.
            isRecoveringFromQteFailue = false;

            // Force the player to walk to the player position again from which he
            // was forced to move away in the walkBackOnQteFailure case.
            RouterScript currentRouter = gameManager.GetCurrentPlayerPosition();
            forceMoveToRouter(currentRouter);

            // Reactivate all GlassShard scripts.
            GlassShardTriggerQTE[] scripts = FindObjectsOfType<GlassShardTriggerQTE>();
            foreach (var script in scripts)
            {
                script.ActivateCollider();
            }
            return;
        }
        else
        {
            // Reactivate all GlassShard scripts.
            GlassShardTriggerQTE[] scripts = FindObjectsOfType<GlassShardTriggerQTE>();
            foreach (var script in scripts)
            {
                script.ActivateCollider();
            }
        }

        // Handling arrived at target position depending on game mode.
        switch (gameType)
        {
            case LevelProperties.GameType.UniformCost:
                // Handle the arrived at target position event in uniform cost game mode.
                handleArrivedAtTargetPositionUniformCost();
                break;
            case LevelProperties.GameType.HotPotato:
                // Special case:
                //Check whether the player has walked to an autonomous system. If this is the case, 
                // the player is forced to return to the previous router.
                if (autonomousSystemHotPotatoPath != null)
                {
                    // Handle the special case.
                    handlePlayerArrivedAtAutonomousSystem();
                    // Need to abort here, player will return to previous router before taking
                    // any further actions.
                    return;
                }

                // Handle the arrived at target position event in hot potato game mode.
                handleArrivedAtTargetPositionHotPotato();
                break;
            default:
                break;
        }

        // Run finished case.
        if (lastMoveStatus == GameStatus.RunFinished)
        {
            //writeFileLog
            if (logToFile)
            {
                if (levelLogging != null)
                {
                    String s = "run=" + runIndex + ", current score=" + scoreBeer.GetScore() + " finished, starting next run.";
                    levelLogging.AppendLine(s);
                }
            }

            movementScript.resetPuddles();
            runIndex++;



            if (isLogEnabled)
                Debug.Log("Destination Points count is: " + destinationPoints.Count);

            // There is a next run. Prepare and start next run.
            if (destinationPoints.Count > 0)
            {
                //TODO: Log optimal path for hot potato/greedy
                //writeFileLog
                if (logToFile)
                {
                    if (levelLogging != null)
                    {
                        String s = "Preparing next run. Optimal Path=" + null;
                        levelLogging.AppendLine(s);
                    }
                }
                // Check if a router needs to be disabled for the next run.
                if (disableInRun != -1 && disableInRun == runIndex)
                {
                    if (disableRouter != null)
                    {
                        // Disable the router.
                        disableRouter.GetComponent<RouterScript>().Disabled = true;

                        // Let blockers appear.
                        if (blockers != null)
                            blockers.SetActive(true);

                    }
                    PathScript[] paths = gameManager.GetAllPathScripts();
                    foreach (var path in paths)
                    {
                        RouterScript from = path.from.GetComponent<RouterScript>();
                        RouterScript to = path.to.GetComponent<RouterScript>();
                        if (from.Disabled || to.Disabled)
                        {
                            path.Disabled = true;
                            // Hide path element.
                            path.gameObject.SetActive(false);
                        }
                    }
                }

                switch (gameType)
                {
                    case LevelProperties.GameType.Greedy:
                        // Start the next run in greedy game mode.
                        prepareAndStartNextRunGreedy();
                        break;
                    case LevelProperties.GameType.UniformCost:
                        // Prepare next run in uniform cost game mode and start it.
                        prepareAndStartNextRunUniformCost();
                        break;
                    case LevelProperties.GameType.HotPotato:
                        // Check if level finished, if not start new run.
                        prepareAndStartNextRunHotPotato();
                        break;
                    default:
                        if (isLogEnabled)
                            Debug.Log("Performing default action. Starting next run.");

                        // Start run.
                        startNextRun();

                        // Disable highlight on source router.
                        startRouter.GetComponent<RouterScript>().DisableHighlight();
                        // Highlight end router.
                        destinationPoints[0].GetComponent<RouterScript>().HighlightRouter();
                        break;
                }

                if (isLogEnabled)
                    Debug.Log("New run started: Start router is: " + startRouter + ", dest point is: " + destinationPoints[0]);

            }

        }
        movementScript.ControlPuddleSpawning(false);

        // Run finished case.
        if (lastMoveStatus == GameStatus.RunFinished || lastMoveStatus == GameStatus.LevelFinished)
        {
            // Play sound if on run complete.
            if (soundManager != null)
                soundManager.PlaySound(SoundManager.SoundType.RunComplete);
        }
    }

    #region UniformCostGameMode

    /// <summary>
    /// Initializes the uniform costs game mode. Sets up all required references
    /// and startst the first run.
    /// </summary>
    private void initializeUniformCostGameMode()
    {
        if (isLogEnabled)
            Debug.Log("Starting uniform game mode.");

        // Reference to the game manager.
        gameManager = GameObject.FindGameObjectWithTag("UniformCosts").GetComponent<GameManagerInterface>();

        if (destinationPoints != null && destinationPoints.Count > 0)
        {
            // Move player to start router with pre load move.
            movementScript.PerformPreLoadMove(startRouter);

            GameTuple currentRun = new GameTuple(startRouter, destinationPoints[0]);
            destinationPoints[0].GetComponent<RouterScript>().HighlightRouter();

            // Start first run.
            gameManager.Start(currentRun);

            // Display the path costs.
            PathScript[] pathScripts = gameManager.GetAllPathScripts();
            foreach (PathScript pathScript in pathScripts)
            {
                if (pathScript.from == startRouter)
                {
                    pathScript.DisplayPathCosts();
                }
            }

            // Get reference to the routing table.
            // UI text fields for uniform cost routing table information.
            routingTableUI = GameObject.FindGameObjectWithTag("RoutingTableUI").GetComponent<RoutingTableUI>();
            routingTableUI.UpdateRoutingTableUniformCosts();
        }
    }

    /// <summary>
    /// Performs actions if a moving action of the player is finished. Displays 
    /// the path costs of all paths that arise from the current player position.
    /// Updates the Routing table.
    /// </summary>
    private void handleArrivedAtTargetPositionUniformCost()
    {
        // Display the path costs of all paths on the current active router.
        RouterScript activeRouter = gameManager.GetCurrentPlayerPosition();
        PathScript[] pathScripts = gameManager.GetAllPathScripts();
        if (GetGameStatus() == GameStatus.ValidHop || GetGameStatus() == GameStatus.RunFinished)
        {
            foreach (PathScript pathScript in pathScripts)
            {
                if (pathScript.from == activeRouter.gameObject)
                {
                    pathScript.DisplayPathCosts();
                }
            }
        }

        // Update the routing table.
        routingTableUI.UpdateRoutingTableUniformCosts();
    }

    /// <summary>
    /// Prepares the next run in the uniform cost game mode. 
    /// Highlights the target router, displays the path costs
    /// for the paths that arise from the start router.
    /// </summary>
    private void prepareAndStartNextRunUniformCost()
    {
        if (isLogEnabled)
            Debug.Log("Starting a new run in uniform cost game mode.");

        // Start the next run first.
        startNextRun();

        // Disable highlight on source router.
        startRouter.GetComponent<RouterScript>().DisableHighlight();

        // Highlight end router.
        destinationPoints[0].GetComponent<RouterScript>().HighlightRouter();

        // Update the routing table.
        routingTableUI.UpdateRoutingTableUniformCosts();

        // Display path costs for paths arising from the start router.
        PathScript[] pathScripts = gameManager.GetAllPathScripts();
        foreach (PathScript pathScript in pathScripts)
        {
            if (pathScript.from == startRouter)
            {
                pathScript.DisplayPathCosts();
            }
            else
            {
                pathScript.ResetPathcost();
            }
        }
    }

    /// <summary>
    /// Handles the uniform cost run finished.
    /// </summary>
    /// <param name="path">Path.</param>
    private void handleUniformCostRunFinished(PathScript path)
    {
        // Setze alle kürzesten Pfadkosten zu den Routern zurück.
        RouterScript[] routers = gameManager.GetAllRouterScripts();
        foreach (var router in routers)
        {
            router.SetPriority(int.MaxValue);
        }

        // Make Bierio smile :)
        playerController.SetMouth(0);

        if (isLogEnabled)
            Debug.Log("Added 20 points.");

        // Update score.
        scoreBeer.UpdateScore(20);
        scoreTexts.DisplayScoreText(ScoreText.Plus20);
    }

    #endregion UniformCostGameMode

    #region GreedyGameMode

    /// <summary>
    /// Initializes the game mode Greedy. Sets up all references
    /// and starts the first run.
    /// </summary>
    private void initializeGreedyGameMode()
    {
        if (isLogEnabled)
            Debug.Log("Start the greedy game mode.");

        // Reference to the game manager.
        gameManager = GameObject.FindGameObjectWithTag("Greedy").GetComponent<GameManagerInterface>();

        if (destinationPoints != null && destinationPoints.Count > 0)
        {
            // Move player to start router with pre load move.
            movementScript.PerformPreLoadMove(startRouter);

            GameTuple currentRun = new GameTuple(startRouter, destinationPoints[0]);
            destinationPoints[0].GetComponent<RouterScript>().HighlightRouter();

            // Start first run.
            gameManager.Start(currentRun);

            // Display the new heuristic values.
            RouterScript[] routerScripts = gameManager.GetAllRouterScripts();
            foreach (RouterScript routerScript in routerScripts)
            {
                routerScript.DisplayGreedyHeuristic();
            }
        }
    }

    /// <summary>
    /// Prepares the next run for the greedy game mode. Highlights the new
    /// target router and displays the new greedy heuristics.
    /// </summary>
    private void prepareAndStartNextRunGreedy()
    {
        if (isLogEnabled)
            Debug.Log("Starting a new run in greedy game mode.");

        // Start next run first.
        startNextRun();

        // Disable highlight on source router.
        startRouter.GetComponent<RouterScript>().DisableHighlight();

        // Highlight end router.
        destinationPoints[0].GetComponent<RouterScript>().HighlightRouter();

        // Display the new heuristic values.
        RouterScript[] routerScripts = gameManager.GetAllRouterScripts();
        foreach (RouterScript routerScript in routerScripts)
        {
            routerScript.ResetRouterText();
            routerScript.DisplayGreedyHeuristic();
        }
    }

    #endregion GreedyGameMode

    #region HotPotatoGameMode

    /// <summary>
    /// Initializes the hot potato game mode. Starts the first run.
    /// </summary>
    private void initializeHotPotatoGameMode()
    {
        if (isLogEnabled)
            Debug.Log("Starting the hot potato game mode.");

        // Reference to the game manager.
        gameManager = GameObject.FindGameObjectWithTag("HotPotato").GetComponent<GameManagerInterface>();
        // Reference to the timer and timerGUI.
        timer = GameObject.FindGameObjectWithTag("HotPotatoTimer").GetComponent<HotPotatoTimer>();
        timerGUI = GameObject.FindGameObjectWithTag("HotPotatoTimerGUI").GetComponent<Text>();

        if (destinationPoints != null && destinationPoints.Count > 0)
        {
            // Move player to start router with pre load move.
            movementScript.PerformPreLoadMove(startRouter);
            performsPreLoadMoveOnHotPotato = true;

            GameTuple currentRun = new GameTuple(startRouter, destinationPoints[0]);
            destinationPoints[0].GetComponent<RouterScript>().HighlightRouter();

            // Start first run.
            gameManager.Start(currentRun);
        }

        // Initializes room barkeepers.
        initializeRoomBarkeepers();

        // Determine bar keepers of first target AS.
        List<GameObject> responsibleBarkeepers = getRoomBarkeepersForAS(destinationPoints[0].GetComponent<RouterScript>());
        // Activate waving for these barkeepers.
        foreach (GameObject barkeeper in responsibleBarkeepers)
        {
            barkeeper.GetComponent<RoomBarkeeperController>().Wave();
        }

        // Start a timer run using the next run context.
        startNextRunTimerRun();
        // At start of the level, pause the timer, due to the professor.
        timer.PauseTimer();
    }

    /// <summary>
    /// Initalizes the room barkeepers for the current level.
    /// </summary>
    private void initializeRoomBarkeepers()
    {
        if (isLogEnabled)
            Debug.Log("Initializing the room barkeepers.");

        roomBarkeepers = GameObject.FindGameObjectsWithTag("RoomBarkeeper");

        foreach (GameObject roomBarkeeper in roomBarkeepers)
        {
            // Set normal mouth at the beginning.
            roomBarkeeper.GetComponent<RoomBarkeeperController>().SetMouth(1);

            if (isLogEnabled)
            {
                Debug.Log("Set the mouth to standard for roomBarkeeper: " + roomBarkeeper.name);
            }
        }
    }

    /// <summary>
    /// Is called if the timer has reached the provided 
    /// maximum time for the hop. The user did not choose an action.
    /// A random router will be selected as the next hop.
    /// </summary>
    private void TimeUpHandling()
    {
        if (isLogEnabled)
            Debug.Log("Timer has fired. User has not performed an action in time.");
        TutorialControllerHotPotato tutorialController = FindObjectOfType<TutorialControllerHotPotato>();
        if (tutorialController != null)
            tutorialController.SetTimeUp(true);

        HopBasedHotPotatoManager hpManager = gameManager as HopBasedHotPotatoManager;
        if (hpManager != null)
        {
            RouterScript currentNode = hpManager.GetCurrentPlayerPosition();
            // Extract neighbour routers of currently active router.
            List<RouterScript> neighbours = hpManager.ExpandNode(currentNode);

            // Chose a random neighbour.
            int randomNeighbourIndex = UnityEngine.Random.Range(0, neighbours.Count);
            if (neighbours.Count > 0)
            {
                RouterScript nextRouter = neighbours[randomNeighbourIndex];

                // Extract the path for the hop.
                PathScript path = hpManager.GetPathBetweenNodes(currentNode, nextRouter);

                // Call the normal handling method for a move on this path.
                PerformMoveOnPath(path);
            }
        }
    }

    /// <summary>
    /// Is called in the specified update interval. Updates the 
    /// timer GUI.
    /// </summary>
    private void updateTimerGui()
    {
        if (!timerGUI.gameObject.activeSelf)
            timerGUI.gameObject.SetActive(true);

        // Update timer gui.
        printOnTimerGui(timer.CurrentTime);
    }

    /// <summary>
    /// Prints the provided time onto the timerGUI.
    /// </summary>
    /// <param name="time">The time to display.</param>
    private void printOnTimerGui(float time)
    {
        // Print the current time value.
        timerGUI.text = string.Format("{0:0.0#}", time);
    }

    /// <summary>
    /// Starts a new timer run.
    /// </summary>
    private void startStandardTimerRun()
    {
        if (timer == null || timerGUI == null)
            return;

        // Start the timer for the next hop.
        timer.CallNewTimer(TimeUpHandling, timer.standardHopTime);
        // Start the update of the timer GUI.
        timer.SubscribeToGuiTimer(updateTimerGui, timer.standardIntervalGUITimer);

        // Activate timer gui.
        printOnTimerGui(timer.standardHopTime);
    }

    /// <summary>
    /// Starts a timer run for the next run.
    /// </summary>
    private void startNextRunTimerRun()
    {
        if (timer == null || timerGUI == null)
            return;

        // Start the timer for the next hop.
        timer.CallNewTimer(TimeUpHandling, timer.hopTimeOnNewRun);
        // Start the update of the timer GUI.
        timer.SubscribeToGuiTimer(updateTimerGui, timer.standardIntervalGUITimer);

        // Activate timer gui.
        printOnTimerGui(timer.hopTimeOnNewRun);
    }

    /// <summary>
    /// Cancels the timer run.
    /// </summary>
    private void cancelTimerRun()
    {
        if (timer == null || timerGUI == null)
            return;

        // Cancel timer gui update events.
        timer.UnsubscribeFromGuiTimer(updateTimerGui);
        // Cancel also the timer.
        timer.CancelTimerRun(TimeUpHandling);

        // Hide timer gui.
        timerGUI.text = "";
    }

    /// <summary>
    /// Handles the valid hop status for the HotPotato game mode. Cancels the current timer run. Checks whether the 
    /// player is heading towards an autonomous system. If this is the case, it is defined
    /// that the player should walk back the path after arriving at the AS.
    /// </summary>
    /// <param name="path">The currently selected path.</param>
    private void handleValidHopHotPotato(PathScript path)
    {
        // Cancel the current timer run.
        cancelTimerRun();

        // TODO: Eigentlich reicht es hier nur auf path.to zu prüfe, da ein AS keinen Rückweg hat.
        RouterScript targetRouter = null;
        // Check to which router the user is heading.
        if (path.from == gameManager.GetCurrentPlayerPosition().gameObject)
        {
            targetRouter = path.to.GetComponent<RouterScript>();
        }
        else
        {
            targetRouter = path.from.GetComponent<RouterScript>();
        }

        if (targetRouter.isAutonomousSystem)
        {
            // We are heading to an AS, so we need to return to active router afterwards.
            autonomousSystemHotPotatoPath = path;
        }
    }

    /// <summary>
    /// Handles the event that is fired when the player has finished a movement process.
    /// Starts a new timer run if the current run is not finished.
    /// </summary>
    private void handleArrivedAtTargetPositionHotPotato()
    {
        // Case: Arrived at target position but not run finished.
        if (gameManager.getGameStatus() != GameStatus.RunFinished)
        {
            // Start timer if its not the pre load move.
            if (performsPreLoadMoveOnHotPotato)
            {
                performsPreLoadMoveOnHotPotato = false;
            }
            else
            {
                startStandardTimerRun();
            }
        }
    }

    /// <summary>
    /// Handles the special case there the player has arrived at an autonomous system.
    /// Forces the player to walk back to the last router. Indicates whether it was the 
    /// target AS using the room barkeepers.
    /// </summary>
    private void handlePlayerArrivedAtAutonomousSystem()
    {
        // Force player to walk back to the previous router.
        movePlayerAlongPath(autonomousSystemHotPotatoPath);
        gameManager.SetCurrentPlayerPosition(autonomousSystemHotPotatoPath.from.GetComponent<RouterScript>());
        autonomousSystemHotPotatoPath = null;

        if (isLogEnabled)
            Debug.Log("In handlePlayerArrivedAtAutonomousSystem: LastMoveStatus is: " + lastMoveStatus);

        // Inidcate that correct AS has been reached.
        if (lastMoveStatus == GameStatus.RunFinished ||
            lastMoveStatus == GameStatus.LevelFinished)
        {
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            GameObject roomBarkeeper = getClosestRoomBarkeeper(playerPos);

            if (roomBarkeeper != null)
            {
                if (isLogEnabled)
                    Debug.Log("In handlePlayerArrivedAtAutonomousSystem: Set room barkeeper " + roomBarkeeper.name + " to status happy.");

                // Barkeeper happy.
                roomBarkeeper.GetComponent<RoomBarkeeperController>().SetMouth(0);
                roomBarkeeper.GetComponent<RoomBarkeeperController>().IdleWithBeer();
                // Reset status after 5 seconds.
                StartCoroutine(ResetBarkeeperStatus(roomBarkeeper, 10.0f));
            }
        }
        else // Not the correct AS.
        {
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            GameObject roomBarkeeper = getClosestRoomBarkeeper(playerPos);
            lastMoveStatus = GameStatus.InvalidHop;

            if (roomBarkeeper != null)
            {
                if (isLogEnabled)
                    Debug.Log("In handlePlayerArrivedAtAutonomousSystem: Set room barkeeper " + roomBarkeeper.name + " to status angry.");

                // Barkeeper angry.
                roomBarkeeper.GetComponent<RoomBarkeeperController>().SetMouth(2);
                // Reset status after 5 seconds.
                StartCoroutine(ResetBarkeeperStatus(roomBarkeeper, 10.0f));
            }
        }
    }

    /// <summary>
    /// Returns the game object of the room barkeeper which is closes to the
    /// player position at that time.
    /// </summary>
    /// <param name="playerPos">The player's position.</param>
    /// <returns>The game object of the closest room barkeeper, or null
    ///     if no room barkeeper could be found.</returns>
    private GameObject getClosestRoomBarkeeper(Vector3 playerPos)
    {
        float minDistance = float.MaxValue;
        int minIndex = -1;

        if (isLogEnabled)
            Debug.Log("Started getClosestRoomBarkeeper: Player pos is: " + playerPos);

        if (roomBarkeepers != null && roomBarkeepers.Length > 0)
        {
            for (int i = 0; i < roomBarkeepers.Length; i++)
            {
                float currentDistance = Vector3.Distance(roomBarkeepers[i].transform.position, playerPos);
                if (currentDistance < minDistance)
                {
                    if (isLogEnabled)
                        Debug.Log("new min distance is: " + currentDistance);

                    minDistance = currentDistance;
                    minIndex = i;
                }
            }

            // Extract Barkeeper with minDistance.
            if (minIndex != -1)
            {
                GameObject roomBarkeeper = roomBarkeepers[minIndex];

                if (isLogEnabled)
                    Debug.Log("Closes room barkeeper is: " + roomBarkeeper.name);

                return roomBarkeeper;
            }
        }
        return null;
    }

    /// <summary>
    /// Extracts the game objects of the room barkeepers which are responsible for the 
    /// sepcified AS.
    /// </summary>
    /// <param name="autonomousSystem">The autonomous system for which the bar keepers should be returned.</param>
    /// <returns>A list of game objects.</returns>
    private List<GameObject> getRoomBarkeepersForAS(RouterScript autonomousSystem)
    {
        Assert.IsNotNull(roomBarkeepers, "Room barkeepers is null. Can't extract barkeepers for AS.");
        Assert.IsFalse(roomBarkeepers.Length == 0, "Warning: There is no room barkeeper in the array of room barkeeper game objects.");
        Assert.IsTrue(autonomousSystem.isAutonomousSystem, "Warning in getRoomBarkeepersForAS: The passed RoutersScript is not an autonomous system.");

        float distanceThreshold = 5.0f;

        List<GameObject> responsibleBarkeepers = new List<GameObject>();

        // Get the collider of the "autonoumousSystem" GameObject.
        Collider2D coll = autonomousSystem.gameObject.GetComponent<Collider2D>();
        if (coll != null)
        {
            // Compare the position of the room barkeeper against the closest collider point of the router.
            // Do this to determine whether a barkeeper belongs to an AS.
            foreach (GameObject roomBarkeeper in roomBarkeepers)
            {
                // Calculate the closest points on the collider's border towards the current position of the player.
                Vector3 borderPoint = coll.bounds.ClosestPoint(roomBarkeeper.transform.position);
                if (borderPoint != null)
                {
                    Vector2 xyPosRoomBarkeeper = new Vector2(roomBarkeeper.transform.position.x, roomBarkeeper.transform.position.y);
                    Vector2 xyPosClosestPoint = new Vector2(borderPoint.x, borderPoint.y);
                    float distanceRoomBarkeeperToBorder = Vector2.Distance(xyPosRoomBarkeeper, xyPosClosestPoint);

                    if (isLogEnabled)
                    {
                        Debug.Log("The room keeper position is: " + xyPosRoomBarkeeper + " and the closest border point is: " + xyPosClosestPoint);
                        Debug.Log("The calculated distance is: " + distanceRoomBarkeeperToBorder + " for barkeeper: " + roomBarkeeper.name);
                    }

                    if (distanceRoomBarkeeperToBorder < distanceThreshold)
                    {
                        // Add the barkeeper to the responsible barkeepers of this AS.
                        responsibleBarkeepers.Add(roomBarkeeper);
                    }
                }
            }
        }

        return responsibleBarkeepers;
    }

    /// <summary>
    /// Coroutine for setting back the status of the specified room barkeeper game object.
    /// </summary>
    /// <param name="barkeeper">The room barkeeper.</param>
    /// <param name="delay">The delay in seconds after that the status will be set back to default.</param>
    IEnumerator ResetBarkeeperStatus(GameObject barkeeper, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (barkeeper != null)
        {
            // Set mouth back to normal.
            barkeeper.GetComponent<RoomBarkeeperController>().SetMouth(1);
            barkeeper.GetComponent<RoomBarkeeperController>().IdleNoBeer();
        }
    }

    /// <summary>
    /// Handles the hot potato run finished. Updates
    /// the player's score. Cancles the current timer run. 
    /// <param name="path">The path to the destination.</param>
    /// </summary>
    private void handleHotPotatoRunFinished(PathScript path)
    {
        // Cancel the current timer run.
        cancelTimerRun();

        // Set the path as the final path to an AS.
        autonomousSystemHotPotatoPath = path;

        if (gameManager.GetType() == typeof(HopBasedHotPotatoManager))
        {
            HopBasedHotPotatoManager hotPotatoManager = gameManager as HopBasedHotPotatoManager;
            if (hotPotatoManager != null)
            {
                // Get the amount of hops of the optimal path.
                int optimalHopCount = hotPotatoManager.GetOptimalPathLength();
                // Get the amount of hops the player has performed for this run.
                int playerHopCount = hotPotatoManager.GetCurrentPlayerPathLength();

                if (optimalHopCount == playerHopCount)
                {
                    // 100 %.
                    scoreBeer.UpdateScore(20);
                    scoreTexts.DisplayScoreText(ScoreText.Plus20);
                }
                else
                {
                    int difference = playerHopCount - optimalHopCount;
                    if (difference == 1)
                    {
                        // 75 %.
                        scoreBeer.UpdateScore(15);
                        scoreTexts.DisplayScoreText(ScoreText.Plus15);
                    }
                    else if (difference == 2)
                    {
                        // 50 %.
                        scoreBeer.UpdateScore(10);
                        scoreTexts.DisplayScoreText(ScoreText.Plus10);
                    }
                    else if (difference == 3)
                    {
                        // 25 %.
                        scoreBeer.UpdateScore(5);
                        scoreTexts.DisplayScoreText(ScoreText.Plus5);
                    }
                    else
                    {
                        // You failed.
                        playerController.SetMouth(2);

                        // 0%
                        scoreTexts.DisplayScoreText(ScoreText.Plus0);
                    }
                }
            }

            // Disable the highlighting to prepare the next run.
            gameManager.GetCurrentPlayerPosition().DisableHighlight();

            // Stop player waving at this AS.
            List<GameObject> responsibleBarkeepers = getRoomBarkeepersForAS(gameManager.GetCurrentPlayerPosition());
            foreach (GameObject barkeeper in responsibleBarkeepers)
            {
                RoomBarkeeperController rbc = barkeeper.GetComponent<RoomBarkeeperController>();
                if (rbc.IsWaving())
                {
                    // Deactivate waving.
                    rbc.IdleNoBeer();
                }
            }
        }
    }

    /// <summary>
    /// Evaluates whether the level is finished, or whether a new run needs to be started.
    /// In the later case, the new run is initialized and started.
    /// </summary>
    private void prepareAndStartNextRunHotPotato()
    {
        // Start next run. Start router is the router where the player is located.
        startRouter = gameManager.GetCurrentPlayerPosition().gameObject;
        // Remove currently handled run from the list of start and end points.
        destinationPoints.RemoveAt(0);

        if (destinationPoints.Count > 0)
        {
            // Perform highlighting.
            destinationPoints[0].GetComponent<RouterScript>().HighlightRouter();

            // Start next run.
            GameTuple nextRun = new GameTuple(startRouter, destinationPoints[0]);
            gameManager.Start(nextRun);

            // Setze letzten Status von RunFinished zurück auf Valid hop.
            // Falls das nicht gemacht wird, wird der Status falsch angezeigt, sollte man
            // direkt nochmals zum benachbarten AS laufen.
            // lastMoveStatus = GameStatus.ValidHop;

            // Start waving of barkeepers at target AS.
            List<GameObject> responsibleBarkeepers = getRoomBarkeepersForAS(destinationPoints[0].GetComponent<RouterScript>());
            foreach (GameObject barkeeper in responsibleBarkeepers)
            {
                barkeeper.GetComponent<RoomBarkeeperController>().Wave();
            }

            // Start timer.
            startNextRunTimerRun();
        }
        else
        {
            if (isLogEnabled)
                Debug.Log("Level is finished.");

            lastMoveStatus = GameStatus.LevelFinished;
        }
    }

    #endregion HotPotatoGameMode

    #region QteEvents

    /// <summary>
    /// Callback function that is called if a qte event
    /// was not handled successfully.
    /// </summary>
    void QteFailure()
    {
        // Only force the player to walk back if he is not already recovering from the qte failure.
        if (!isRecoveringFromQteFailue)
        {
            // Force player to walk back.
            walkBackOnQteFailure = true;

            scoreBeer.UpdateScore(-5);
            scoreTexts.DisplayScoreText(ScoreText.Minus5);

            // Abort the current movement process.
            movementScript.AbortCurrentMovementProcess();
        }
        // else do nothing.
    }

    /// <summary>
    /// Callback function that is called if a qte event
    /// was handled successfully.
    /// </summary>
    void QteSuccess()
    {
        // Do not mark it as successful if player is currently recovering from the qte failure.
        if (!isRecoveringFromQteFailue)
        {
            walkBackOnQteFailure = false;
            isRecoveringFromQteFailue = false;
        }
    }

    #endregion QteEvents

    /// <summary>
    /// Provides a default handling procedure for the run finished 
    /// status. Simply updates the player's score. If a game mode doesn't need
    /// a special handling of the run finished state, this method can be used.
    /// </summary>
    private void performDefaultRunFinishedHandling()
    {
        // Make Bierio smile :)
        playerController.SetMouth(0);

        if (isLogEnabled)
            Debug.Log("Added 20 points.");

        // Update score.
        scoreBeer.UpdateScore(20);
        scoreTexts.DisplayScoreText(ScoreText.Plus20);
    }

    /// <summary>
    /// Starts the next run. The run is started for the current
    /// game mode.
    /// </summary>
    private void startNextRun()
    {
        // Start next run.
        startRouter = destinationPoints[0];
        // Remove currently handled run from the list of start and end points.
        destinationPoints.RemoveAt(0);

        if (destinationPoints.Count > 0)
        {
            GameTuple nextRun = new GameTuple(startRouter, destinationPoints[0]);

            // Start next run.
            gameManager.Start(nextRun);
        }
        else
        {
            if (isLogEnabled)
                Debug.Log("Level is finished.");

            lastMoveStatus = GameStatus.LevelFinished;
        }
    }

    /// <summary>
    /// Moves the player along the specified path.
    /// The player always moves from the router where he is currently located to the other router to which the path is linked.
    /// </summary>
    /// <param name="path">The path on which the player object is moved.</param>
    private void movePlayerAlongPath(PathScript path)
    {
        if (path.from == gameManager.GetCurrentPlayerPosition().gameObject)
        {
            if (isLogEnabled)
                Debug.Log("Call moveplayer with to: " + path.to.gameObject.GetComponent<RouterScript>());
            movementScript.MovePlayer(path.to);
        }
        else
        {
            if (isLogEnabled)
                Debug.Log("Call moveplayer with from: " + path.from.gameObject);
            movementScript.MovePlayer(path.from);
        }
    }

    /// <summary>
    /// Forces the player to move to the target router.
    /// </summary>
    /// <param name="target">Target router.</param>
    private void forceMoveToRouter(RouterScript target)
    {
        if (isLogEnabled)
            Debug.Log("Call forceMoveToRouter with target: " + target.gameObject);
        movementScript.MovePlayer(target.gameObject);
    }

    public GameStatus GetGameStatus()
    {
        return lastMoveStatus;
    }

    public bool IsErrorRecovered()
    {
        return errorRecovered;
    }
}
