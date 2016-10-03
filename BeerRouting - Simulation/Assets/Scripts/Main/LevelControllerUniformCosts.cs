using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LevelControllerUniformCosts : LevelController
{
    private UniformCostManager uniformCostsManager;

    void Start()
    {
        // Init components.
        uniformCostsManager = FindObjectOfType<UniformCostManager>();
        // Show mobile specific text on Android devices.
        if (levelProperties.levelName == "Uniform Cost - Level 3" && Application.platform == RuntimePlatform.Android)
        {
            professorController.NextSequence();
        }
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
        return uniformCostsManager.GetComponent<GameManagerInterface>().GetCurrentPlayerPosition().routerName;
    }

    public override void OnProfessorButtonClick()
    {
        // Show normal professor here.
        professorController.Show(true);
    }
}
