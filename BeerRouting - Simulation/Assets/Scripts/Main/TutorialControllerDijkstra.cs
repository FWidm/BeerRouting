using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialControllerDijkstra : LevelController
{
    private DijkstraManager dijkstraManager;
    private int prevErrorSeq;

    // Use this for initialization
    void Start()
    {
        // Init components.
        dijkstraManager = FindObjectOfType<DijkstraManager>();
    }

    public override void OnPathClicked(PathScript path)
    {
        clickedPath = path;

        Debug.Log("DijkstraStatus: " + dijkstraManager.GetCurrentMove().Status);
        switch (dijkstraManager.GetCurrentMove().Status)
        {
            case DijkstraStatus.HOP_UNREACHABLE:
                // Remember last sequence and state.
                lastSequence = professorController.GetCurrentSequenceId();
                lastState = professorController.GetCurrentStateId();
                // Show professor with HOP_UNREACHABLE message.
                professorController.SetSequenceAndState(-1, 0);
                professorController.ShowAngry(true);
                break;
            case DijkstraStatus.WRONG_HOP:
                prevRouterName = clickedPath.from.GetComponent<RouterScript>().routerName;
                break;
            case DijkstraStatus.UNDISCOVERED_PATHS:
                prevRouterName = clickedPath.from.GetComponent<RouterScript>().routerName;
                break;
        }
    }

    public override void OnStopProfessorDisappear()
    {
        // Get a beer at the end of the first sequence.
        if (professorController.GetCurrentSequenceId() == 0 && professorController.GetCurrentStateId() == 3)
        {
            gameInputEnabled = false;
            professorController.ShowBeer(true, 0);
            professorController.NextSequence();
        }
        // Check if level is finished and professor is gone.
        if (professorController.GetCurrentSequenceId() == 8 && professorController.GetCurrentStateId() == 2)
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
        if (dijkstraManager.GetCurrentMove() == null)
            return true;

        if (dijkstraManager.GetCurrentMove().Status.Equals(DijkstraStatus.UNDISCOVERED_PATHS))
        {
            // Player made no consequential error.
            prevRouterName = dijkstraManager.GetPreviousMove().Source.routerName;
            // Remember last sequence and state.
            lastSequence = professorController.GetCurrentSequenceId();
            lastState = professorController.GetCurrentStateId();
            professorController.SetSequenceAndState(-2, 0);
            prevErrorSeq = -2;
            professorController.ShowAngry(true);
            return false;
        }

        if (dijkstraManager.GetCurrentMove().Status.Equals(DijkstraStatus.ERROR_RECOVERY))
        {
            // Player made a consequential error.
            if (dijkstraManager.IsErrorRecovery())
            {
                if (prevErrorSeq == -2)
                    professorController.SetSequenceAndState(-3, 0);
                else
                    professorController.SetSequenceAndState(-5, 0);
                professorController.ShowAngry(true);
            }
            return false;
        }

        if (dijkstraManager.GetCurrentMove().Status.Equals(DijkstraStatus.WRONG_HOP))
        {
            // Player made no consequential error.
            prevRouterName = dijkstraManager.GetPreviousMove().Source.routerName;
            // Remember last sequence and state.
            lastSequence = professorController.GetCurrentSequenceId();
            lastState = professorController.GetCurrentStateId();
            professorController.SetSequenceAndState(-4, 0);
            prevErrorSeq = -4;
            professorController.ShowAngry(true);
            return false;
        }

        // No error occured.
        return true;
    }

    private void CheckValidMoves()
    {
        if (professorController.GetCurrentSequenceId() == 1 && professorController.GetCurrentStateId() == 2)
        {
            // If first path was discovered, show professor with next sequence.
            if (dijkstraManager.GetCurrentMove().Status.Equals(DijkstraStatus.VALID_HOP_DISCOVERY))
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
        else if (professorController.GetCurrentSequenceId() == 2 && professorController.GetCurrentStateId() == 2)
        {
            // If second path was discovered, show professor with next sequence.
            if (!dijkstraManager.IsCurrentWorkingRouterHandledCompletely())
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
        else if (professorController.GetCurrentSequenceId() == 3 && professorController.GetCurrentStateId() == 2)
        {
            // If first router is handled completely, show professor with next sequence.
            if (dijkstraManager.IsCurrentWorkingRouterHandledCompletely())
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
        else if (professorController.GetCurrentSequenceId() == 4 && professorController.GetCurrentStateId() == 3)
        {
            // If correctly moved to second hop, show professor with next sequence.
            if (dijkstraManager.GetCurrentMove().Status.Equals(DijkstraStatus.VALID_HOP))
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
        else if (professorController.GetCurrentSequenceId() == 5 && professorController.GetCurrentStateId() == 0)
        {
            // If second router is handled completely, show professor with next sequence.
            if (dijkstraManager.IsCurrentWorkingRouterHandledCompletely())
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
        else if (professorController.GetCurrentSequenceId() == 6 && professorController.GetCurrentStateId() == 4)
        {
            // If only one router is left, show professor with next sequence.
            if (dijkstraManager.GetAmountOfUndiscoveredRouters() == 1)
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
        else if (professorController.GetCurrentSequenceId() == 7 && professorController.GetCurrentStateId() == 1)
        {
            // If all routers are handled completely, show professor with next sequence.
            if (dijkstraManager.GetAmountOfUndiscoveredRouters() == 0)
            {
                professorController.NextSequence();
                professorController.Show(true);
            }
        }
    }

    public override string GetCurrentRouterName()
    {
        return dijkstraManager.GetCurrentPlayerPosition().GetComponent<RouterScript>().routerName;
    }

    public override void OnProfessorButtonClick()
    {
        // Show normal professor here.
        professorController.Show(true);
    }
}
