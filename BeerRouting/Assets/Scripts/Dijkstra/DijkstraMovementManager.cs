using UnityEngine;
using System.Collections;
using System;

public class DijkstraMovementManager : MonoBehaviour, MovementManagerInterface
{

    // Is logging enabled.
    public bool isLogEnabled;

    // Specifies the starting point of the player and the starting point of the dijkstra algorithm. 
    public GameObject startingRouter;

    // A reference to the DijkstraManager.
    private DijkstraManager dijkstraManager;

    // A reference to the ScoreTextScript for displaying pop up information about current score events.
    private ScoreTextScript scoreTexts;

    // UI Texts for Dijkstra routing table.
    private RoutingTableUI routingTableUI;

    // A reference to the score beer script.
    private ScoreBeer scoreBeer;

    // The Player controller reference to control the appearance of the player at runtime.
    private PlayerController playerController;

    // A reference to the MovementScript.
    private MovementScript movementScript;

    // The path that is currently handled and the result of the validity check for this move.
    private PathScript currentPath;
    private DijkstraStatus currentStatus;

    // Path discovery relevant parameters.
    private bool performsPathDiscovery = false;
    private GameObject originRouter;

    private SoundManager soundManager;

    //Log the GameMovement of one level
    private bool logToFile = true;
    private LevelLogging levelLogging;

    // count things to log
    public int countErrorRecovery, countWrongHop, countNoOp, countUndiscoveredPaths;

    // Use this for initialization
    void Start()
    {
        // Get a reference to the DijkstraManager.
        GameObject dijkstraManagerGO = GameObject.FindGameObjectWithTag("Dijkstra");
        dijkstraManager = dijkstraManagerGO.GetComponent<DijkstraManager>();

        // Get a reference to the SoundManager.
        soundManager = FindObjectOfType<SoundManager>();

        // Get a reference to the ScoreTextManager.
        GameObject scoreTextGO = GameObject.FindGameObjectWithTag("ScoreTextManager");
        scoreTexts = scoreTextGO.GetComponent<ScoreTextScript>();

        // Get a reference to the MovementScript.
        movementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementScript>();

        scoreBeer = FindObjectOfType<ScoreBeer>();
        playerController = FindObjectOfType<PlayerController>();
        LevelProperties lp = FindObjectOfType<LevelProperties>();
        scoreBeer.SetMaxScore(lp.levelMaxScore);    // Set the max score defined in the level properties.

        // Start the dijkstra algorithm.
        dijkstraManager.StartDijkstraAlgorithm(startingRouter.GetComponent<RouterScript>());

        // Perform pre load move.
        movementScript.PerformPreLoadMove(startingRouter);

        movementScript.SubscribeToTargetPositionArrivedEvent(ReachedTargetPoint);

        // UI text fields for dijkstra routing table information.
        routingTableUI = GameObject.FindGameObjectWithTag("RoutingTableUI").GetComponent<RoutingTableUI>();
        routingTableUI.UpdateRoutingTableUI();

        if (logToFile)
        {
            LevelProperties levelProperties = FindObjectOfType<LevelProperties>();

            Debug.Log("Logging to file!");
            levelLogging = new LevelLogging(levelProperties.levelName, " MaxScore=" + levelProperties.levelMaxScore + ", Type=" + levelProperties.gameType, PlayerPrefs.GetString("name"));
        }
    }
    /// <summary>
    /// Appends a string to the internal levelLogging object if it is not null and logging is enabled.
    /// </summary>
    /// <param name="logLine"></param>
    public void WriteToLogAppendScore(String logLine)
    {
        if (logToFile)
        {
            if (levelLogging != null)
            {
                levelLogging.AppendLine(logLine + ";\tScore% = " + scoreBeer.GetScore());
            }
        }
    }

    /// <summary>
    /// Appends a string to the internal levelLogging object if it is not null and logging is enabled.
    /// </summary>
    /// <param name="logLine"></param>
    public void WriteToLog(String logLine)
    {
        if (logToFile)
        {
            if (levelLogging != null)
            {
                levelLogging.AppendLine(logLine);
            }
        }
    }

    /// <summary>
    /// Moves the player along the specified path.
    /// The player always moves from the router where he is currently located to the other router to which the path is linked.
    /// </summary>
    /// <param name="path">The path on which the player object is moved.</param>
    private void movePlayerAlongPath(PathScript path)
    {
        if (path.from == dijkstraManager.GetCurrentPlayerPosition())
        {
            movementScript.MovePlayer(path.to);
        }
        else
        {
            movementScript.MovePlayer(path.from);
        }
    }

    /// <summary>
    /// Performs the path discovery move along the specified path, i.e. the player moves 
    /// towards the destination router and spwans puddles on the way. After the player has reached
    /// the target position, the player moves back along the path to the origin router.
    /// </summary>
    /// <param name="path">The path on which the path discovery should be performed.</param>
    private void performPathDiscoveryMove(PathScript path)
    {
        if (isLogEnabled)
            Debug.Log("DijkstraMovementManager: Perform path discovery on path " + path.name);

        movementScript.ControlPuddleSpawning(true);
        performsPathDiscovery = true;

        if (path.from == dijkstraManager.GetCurrentPlayerPosition())
        {
            originRouter = path.from;
            movementScript.MovePlayer(path.to);
        }
        else
        {
            originRouter = path.to;
            movementScript.MovePlayer(path.from);
        }
    }

    /// <summary>
    /// Handles the event which is fired if the player has reached the target point.
    /// </summary>
    private void ReachedTargetPoint()
    {
        // Special case: Path Discovery.
        if (performsPathDiscovery && originRouter != null)
        {
            if (isLogEnabled)
                Debug.Log("DijkstraMovementManager: PathDiscovery: Need to move player back to origin router.");

            performsPathDiscovery = false;
            movementScript.ControlPuddleSpawning(false);

            // Move player back to origin router.
            movementScript.MovePlayer(originRouter);

            originRouter = null;
        }
        else
        {
            // Update the routing table.
            routingTableUI.UpdateRoutingTableUI();

            // Show the path costs.
            if (currentPath != null && currentPath.IsDiscovered() == true)
            {
                currentPath.DisplayPathCosts();

                // Display path costs also for the way back.
                dijkstraManager.GetInversePath(currentPath).DisplayPathCosts();
                //writeFileLog

            }

            // If it is a valid discovery move, check if the current working router is completely handled now.
            if (currentStatus == DijkstraStatus.VALID_HOP_DISCOVERY || currentStatus == DijkstraStatus.VALID_HOP)
            {
                bool isHandled = dijkstraManager.IsCurrentWorkingRouterHandledCompletely();
                if (isHandled)
                {
                    dijkstraManager.GetCurrentPlayerPosition().GetComponent<RouterScript>().HighlightRouter();
                    if (soundManager != null)
                        soundManager.PlaySound(SoundManager.SoundType.RunComplete);
                }

            }
        }
    }

    /// <summary>
    /// Handles the complete Movement for any Dijkstra level - is checked if "isDijkstra" is set to true.
    /// </summary>
    /// <param name="destination">The path for which the hop will be evaluated.</param>
    public void CheckDijkstra(PathScript destination)
    {
        // Check if it is a valid hop.
        DijkstraStatus status = dijkstraManager.IsValidHop(destination);
        currentStatus = status;

        if (isLogEnabled)
            Debug.Log("DijkstraMovementManager: IsValidHop returned: Status Code: " + status);

        switch (status)
        {
            case DijkstraStatus.ERROR_RECOVERY:
                if (isLogEnabled)
                    Debug.Log("DijkstraMovementManager: In error recovery mode.");

                // Perform error recovery hop.
                movePlayerAlongPath(destination);
                ErrorRecoveryStatus erStatus = dijkstraManager.PerformErrorRecoveryHop(destination);

                if (erStatus == ErrorRecoveryStatus.ERROR_NOT_RECOVERED)
                {
                    // TODO change score to scorebeer
                    // Display -5 score points on screen and update the score.
                    scoreTexts.DisplayScoreText(ScoreText.Minus5);
                    scoreBeer.UpdateScore(-5);
                    countErrorRecovery++;
                    WriteToLogAppendScore("Error Recovery;\tDid not recover from Error, ErrorRecoveryStatus=" + erStatus);

                }
                else
                {
                    WriteToLogAppendScore("Error Recovery;\tRecovered from Error, ErrorRecoveryStatus=" + erStatus);
                }

                break;
            case DijkstraStatus.UNDISCOVERED_PATHS:
                if (isLogEnabled)
                    Debug.Log("DijkstraMovementManager: You need to perform path disvocery on every path from this router first.");

                // TODO Unterscheidung Tutorial oder in game.
                movePlayerAlongPath(destination);
                dijkstraManager.PerformWrongHop(destination);

                // TODO change score to scorebeer
                // Show -10 score points on screen and update the score.
                scoreTexts.DisplayScoreText(ScoreText.Minus10);
                scoreBeer.UpdateScore(-10);
                countUndiscoveredPaths++;
                WriteToLogAppendScore("Undiscovered Paths;\tPlayer did not perform the complete discovery, destination=" + destination);

                // Change players mouth to angry.
                playerController.SetMouth(2);
                break;
            case DijkstraStatus.WRONG_HOP:
                if (isLogEnabled)
                    Debug.Log("DijkstraMovementManager: The dijkstra algorithm prohibits this hop.");

                // TODO Unterscheidung Tutorial oder in game.
                movePlayerAlongPath(destination);
                dijkstraManager.PerformWrongHop(destination);

                // TODO change score to scorebeer
                // Show -20 score points on screen and update the score.
                scoreTexts.DisplayScoreText(ScoreText.Minus20);
                scoreBeer.UpdateScore(-20);
                countWrongHop++;
                WriteToLogAppendScore("Wrong Hop;\tPlayer did perform a wrong hop. destination=" + destination);

                // Change players mouth to angry.
                playerController.SetMouth(2);
                break;
            case DijkstraStatus.HOP_UNREACHABLE:

                if (isLogEnabled)
                    Debug.Log("DijkstraMovementManager: It is an invalid hop.");
                WriteToLogAppendScore("Wrong Hop;\tPlayer did perform a wrong hop. destination=" + destination);

                break;
            case DijkstraStatus.NOP:
                countNoOp++;
                WriteToLogAppendScore("No Operation;\tPlayer travels through completely handled destination=" + destination);

                // Start moving the player object.
                movePlayerAlongPath(destination);

                // Perform the hop in the dijkstra algorithm.
                dijkstraManager.PerformHop(destination);
                break;
            case DijkstraStatus.VALID_HOP:

                if (isLogEnabled)
                    Debug.Log("DijkstraMovementManager: Performing the hop to the other router.");

                // Start moving the player object.
                movePlayerAlongPath(destination);

                // TODO change score to scorebeer
                // Display +10 points and update the score.
                scoreTexts.DisplayScoreText(ScoreText.Plus10);
                scoreBeer.UpdateScore(10);

                // Perform the hop in the dijkstra algorithm.
                dijkstraManager.PerformHop(destination);

                // Change players mouth to happy.
                playerController.SetMouth(0);

                WriteToLogAppendScore("Valid Hop;\tPlayer did perform a valid hop. destination=" + destination);

                break;
            case DijkstraStatus.VALID_HOP_DISCOVERY:

                if (isLogEnabled)
                    Debug.Log("DijkstraMovementManager: Performing the path discovery.");

                // Start path discovery.
                performPathDiscoveryMove(destination);

                // TODO change score to scorebeer
                // Display +5 points and update the score.
                scoreTexts.DisplayScoreText(ScoreText.Plus5);
                scoreBeer.UpdateScore(5);

                // Change players mouth to normal.
                playerController.SetMouth(1);

                // Perform a path discovery step in the dijkstra algorithm.
                dijkstraManager.PerformPathDiscovery(destination);

                WriteToLogAppendScore("Valid Hop Discovery;\tPlayer did perform a valid hop. destination=" + destination);

                break;
        }
    }

    /// <summary>
    /// Evaluates whether the move along this path is valid in the current state of the dijkstra run.
    /// If it is, the move is performed.
    /// </summary>
    /// <param name="path">The path on which a player move should be performed.</param>
    public void PerformMoveOnPath(PathScript path)
    {
        currentPath = path;
        CheckDijkstra(path);
    }
}
