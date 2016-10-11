using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelControllerDijkstra : LevelController
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
        // Method not needed here.
    }

    public override void OnEnterPlayerIdle()
    {
        // If all routers are handled completely, show professor with star speech bubble.
        if (dijkstraManager.GetAmountOfUndiscoveredRouters() == 0)
        {
            FinishLevel();
            return;
        }
        if (dijkstraManager.GetCurrentMove() != null)
        {
            // Check if move was false.
            CheckFalseMoves();
        }
    }

    private bool CheckFalseMoves()
    {
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
