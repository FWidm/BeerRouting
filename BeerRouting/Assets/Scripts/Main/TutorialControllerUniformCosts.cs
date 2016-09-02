using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialControllerUniformCosts : LevelController
{
    private UniformCostManager uniformCostsManager;

    void Start()
    {
        // Init components.
        uniformCostsManager = FindObjectOfType<UniformCostManager>();
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
        // If all routers are handled completely, show professor with star speech bubble.
        if (professorController.GetCurrentSequenceId() == 11 && movementManager.GetGameStatus() == GameStatus.LevelFinished)
        {
            FinishLevel();
        }
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
        if (seqId == 0 || seqId == 1 || seqId == 2 || seqId == 3)
        {
            // If next router was reached, show professor with next sequence.
            if (movementManager.GetGameStatus().Equals(GameStatus.ValidHop))
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
        else if (movementManager.GetGameStatus().Equals(GameStatus.ValidHop))
        {
            string rName = GetCurrentRouterName();
            // If specific sequence and router was reached, show professor with next sequence.
            if ((seqId == 7 && rName.Equals("B")) || (seqId == 8 && rName.Equals("A")) || (seqId == 9 && rName.Equals("F")))
            {
                professorController.NextSequence();
                professorController.Show(true);
            }

        }
        else if (seqId == 4 || seqId == 5 || seqId == 6 || seqId == 10)
        {
            // If target router was reached, show professor with next sequence.
            if (movementManager.GetGameStatus().Equals(GameStatus.RunFinished) || movementManager.GetGameStatus() == GameStatus.LevelFinished)
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
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
