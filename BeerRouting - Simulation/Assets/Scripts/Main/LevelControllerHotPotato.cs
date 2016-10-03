using UnityEngine;
using System.Collections;

public class LevelControllerHotPotato : LevelController
{

    private HopBasedHotPotatoManager hotPotatoManager;

    // Use this for initialization
    void Start()
    {
        // Init components.
        hotPotatoManager = FindObjectOfType<HopBasedHotPotatoManager>();
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
        }
    }

    public override string GetCurrentRouterName()
    {
        return hotPotatoManager.GetCurrentPlayerPosition().routerName;
    }

    public override void OnProfessorButtonClick()
    {
        // Show professor with money here.
        professorController.ShowMoney(true);
    }
}
