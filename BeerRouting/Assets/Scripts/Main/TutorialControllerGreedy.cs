using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TutorialControllerGreedy : LevelController
{
    private GreedyManager greedyManager;
    private CameraPan cameraPan;
    public List<GameObject> cameraPositions;

    void Start()
    {
        // Init components.
        greedyManager = FindObjectOfType<GreedyManager>();
        cameraPan = FindObjectOfType<CameraPan>();
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
        // Get a beer at the end of the first sequence.
        if (professorController.GetCurrentSequenceId() == 0 && professorController.GetCurrentStateId() == 1)
        {
            gameInputEnabled = false;
            professorController.ShowBeer(true, 3);
            professorController.NextSequence();
        }

        // If all routers are handled completely, finish level.
        if (professorController.GetCurrentSequenceId() == 6 && movementManager.GetGameStatus() == GameStatus.LevelFinished)
        {
            FinishLevel();
        }

    }

    public void OnSpeechBubble()
    {
        int stateId = professorController.GetCurrentStateId();
        int sequenceId = professorController.GetCurrentSequenceId();

            if (sequenceId == 0 && stateId == 1)
                cameraPan.ZoomIn(cameraPositions[0].transform.position, 0.1f, 4, 6.5f);
            else if (sequenceId == 2 && stateId == 1)
                cameraPan.ZoomIn(cameraPositions[1].transform.position, 0.1f, 4, 6.5f);
            else if (sequenceId == 3 && stateId == 1)
                cameraPan.ZoomIn(cameraPositions[2].transform.position, 0.1f, 4, 7.5f);
            else if (sequenceId == 4 && stateId == 1)
                cameraPan.ZoomIn(cameraPositions[3].transform.position, 0.1f, 4, 7.5f);
            else if (sequenceId == 5 && stateId == 1)
                cameraPan.ZoomIn(cameraPositions[4].transform.position, 0.1f, 4, 7.0f);
    }

    public override void OnEnterPlayerIdle()
    {
        // Check if move was false.
        if (CheckFalseMoves())
        {
            // Check valid moves if no errors have occured.
            CheckValidMoves();
        }
    }


    private bool CheckFalseMoves()
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
                return false;
            case GameStatus.ErrorRecoveryHop:
                // Check if player has not recovered the error.
                if (!movementManager.IsErrorRecovered())
                {
                    // Player made a consequential error.
                    professorController.SetSequenceAndState(-3, 0);
                    professorController.ShowAngry(true);
                }
                return false;
        }

        // No errors occured.
        return true;
    }

    private void CheckValidMoves()
    {
        int seqId = professorController.GetCurrentSequenceId();
        if (seqId == 1 || seqId == 2 || seqId == 3 || seqId == 4 || seqId == 5)
        {
            // If a target router was reached, show professor with next sequence.
            if (movementManager.GetGameStatus().Equals(GameStatus.RunFinished) || movementManager.GetGameStatus().Equals(GameStatus.LevelFinished))
            {
                professorController.NextSequence();
                professorController.ShowBeer(true, 4);
            }
        }
    }

    public override string GetCurrentRouterName()
    {
        return greedyManager.GetCurrentPlayerPosition().routerName;
    }

    public override void OnProfessorButtonClick()
    {
        // Show professor with money here.
        professorController.ShowBeer(true, 4);
    }
}
