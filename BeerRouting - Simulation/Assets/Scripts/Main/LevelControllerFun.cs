using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LevelControllerFun : LevelController
{
    private GreedyManager greedyManager;
    private int finishedRuns;

    void Start()
    {
        // Init components.
        greedyManager = FindObjectOfType<GreedyManager>();
        showNormalProfOnStart = false;
        StartCoroutine(ShowProfessorBeerGlasAfterTime());
        finishedRuns = 0;
        FindObjectOfType<PlayerController>().SetSunglasses(true);
    }

    IEnumerator ShowProfessorBeerGlasAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(1);
        // Then show professor.        
        professorController.ShowBeer(true, 4);
    }

    public override void OnStopProfessorDisappear()
    {
        // Do nothing special here.
    }

    public override void OnEnterPlayerIdle()
    {
        // If all routers are handled completely, finish level.
        if (movementManager.GetGameStatus() == GameStatus.LevelFinished)
        {
            FinishLevel();
            return;
        } else if(levelProperties.levelName == "Greedy - Level 3" && movementManager.GetGameStatus() == GameStatus.RunFinished) {
            finishedRuns++;
            if (finishedRuns == 4)
            {
                professorController.NextSequence();
                professorController.ShowBeer(true, 4);
            }
        }

        // Check if move was false.
        CheckFalseMoves();
    }

    private void CheckFalseMoves()
    {
        switch (movementManager.GetGameStatus())
        {
            case GameStatus.InvalidHop:
                // Player made no consequential error.
                // Remember last sequence and state.
                lastSequence = professorController.GetCurrentSequenceId();
                lastState = professorController.GetCurrentStateId();
                professorController.SetSequenceAndState(-2, 0);
                professorController.ShowAngry(true);
                break;
            case GameStatus.ErrorRecoveryHop:
                // Check if player has not recovered the error.
                if (!movementManager.IsErrorRecovered())
                {
                    // Player made a consequential error.
                    professorController.SetSequenceAndState(-3, 0);
                    professorController.ShowAngry(true);
                }
                break;
        }
    }

    public override string GetCurrentRouterName()
    {
        return greedyManager.GetCurrentPlayerPosition().routerName;
    }

    public override void OnPathClicked(PathScript path)
    {
        clickedPath = path;
        if (debugging)
            Debug.Log("LevelController: OnPathClicked - GameStatus: " + movementManager.GetGameStatus().ToString());

        switch (movementManager.GetGameStatus())
        {
            case GameStatus.ForbiddenHop:
                // Remember last sequence and state.
                lastSequence = professorController.GetCurrentSequenceId();
                lastState = professorController.GetCurrentStateId();
                // Show professor with HOP_UNREACHABLE message.
                professorController.SetSequenceAndState(-1, 0);
                professorController.ShowAngry(true);
                break;
            case GameStatus.InvalidHop:
                prevRouterName = clickedPath.from.GetComponent<RouterScript>().routerName;
                break;
                // Handle other errors in OnStopPlayerWalking() method.
        }
    }

    public override void OnProfessorButtonClick()
    {
        // Show professor with money here.
        professorController.ShowBeer(true, 4);
    }
}
