using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialControllerHotPotato : LevelController
{

    private HopBasedHotPotatoManager hotPotatoManager;
    private bool timeUp;
    private CameraPan cameraPan;
    public List<GameObject> cameraPositions;

    // Use this for initialization
    void Start()
    {
        // Init components.
        hotPotatoManager = FindObjectOfType<HopBasedHotPotatoManager>();
        cameraPan = FindObjectOfType<CameraPan>();
        timeUp = false;
        showNormalProfOnStart = false;
        StartCoroutine(ShowProfessorMoneyAfterTime());
        StartCoroutine(MoneyAfterTime());
    }

    IEnumerator ShowProfessorMoneyAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(1);
        // Then show professor.        
        professorController.ShowMoney(true);
    }

    IEnumerator MoneyAfterTime()
    {
        yield return new WaitForSeconds(Random.Range(6, 10));
        if (professorController.IsVisible())
        {
            // Play the professors money animation once.
            professorController.Money(true);
        }
        // Start this method again.
        StartCoroutine(MoneyAfterTime());
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

    public void OnSpeechBubble()
    {
        int stateId = professorController.GetCurrentStateId();
        int sequenceId = professorController.GetCurrentSequenceId();

        if (sequenceId == 0)
        {
            if (stateId == 8)
            {
                cameraPan.ZoomIn(cameraPositions[0].transform.position, 0.2f, 3, 100);
            }
            else
            if (stateId == 3)
            {
                cameraPan.ZoomOut(FindObjectOfType<BackgroundDimensions>().transform.position, 0.2f);
            }
        }
        else if (sequenceId == 1 && stateId == 1)
        {
            Vector3 pos2 = cameraPositions[1].transform.position;
            pos2.y = pos2.y + 1.5f;
            cameraPan.ZoomIn(pos2, 0.2f, 3, 100);
        }
        else if (sequenceId == 2 && stateId == 1)
        {
            cameraPan.ZoomIn(cameraPositions[2].transform.position, 0.2f, 3, 100);
        }
        else if (sequenceId == 3 && stateId == 1)
        {
            cameraPan.ZoomIn(cameraPositions[0].transform.position, 0.2f, 3, 100);
        }
    }

    public override void OnStopProfessorDisappear()
    {
        // If all routers are handled completely, finish level.
        if (movementManager.GetGameStatus() == GameStatus.LevelFinished)
        {
            FinishLevel();
            return;
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

    private void CheckValidMoves()
    {
        int seqId = professorController.GetCurrentSequenceId();
        if (seqId == 0 || seqId == 1 || seqId == 2 || seqId == 3 || seqId == 4)
        {
            // If a target barkeeper was reached, show professor with next sequence.
            if (movementManager.GetGameStatus().Equals(GameStatus.RunFinished) || movementManager.GetGameStatus().Equals(GameStatus.LevelFinished))
            {
                professorController.NextSequence();
                professorController.ShowMoney(true);
            }
        }
    }

    private bool CheckFalseMoves()
    {
        bool wrongBarkeeper = false;
        switch (movementManager.GetGameStatus())
        {
            case GameStatus.InvalidHop:
                wrongBarkeeper = true;
                break;
        }

        if (wrongBarkeeper && timeUp)
        {
            timeUp = false;
            // Player made no consequential error.
            // Remember last sequence and state.
            lastSequence = professorController.GetCurrentSequenceId();
            lastState = professorController.GetCurrentStateId();
            professorController.SetSequenceAndState(-4, 0);
            professorController.ShowAngry(true);
            return false;
        }
        else if (timeUp)
        {
            timeUp = false;
            // Player made no consequential error.
            // Remember last sequence and state.
            lastSequence = professorController.GetCurrentSequenceId();
            lastState = professorController.GetCurrentStateId();
            professorController.SetSequenceAndState(-3, 0);
            professorController.ShowAngry(true);
            return false;
        }
        else if (wrongBarkeeper)
        {
            // Player made no consequential error.
            // Remember last sequence and state.
            lastSequence = professorController.GetCurrentSequenceId();
            lastState = professorController.GetCurrentStateId();
            professorController.SetSequenceAndState(-2, 0);
            professorController.ShowAngry(true);
            return false;
        }

        // No error occured.
        return true;
    }

    public override string GetCurrentRouterName()
    {
        return hotPotatoManager.GetCurrentPlayerPosition().routerName;
    }

    public void SetTimeUp(bool isTimeUp)
    {
        timeUp = isTimeUp;
    }

    public override void OnProfessorButtonClick()
    {
        // Show professor with money here.
        professorController.ShowMoney(true);
    }
}
