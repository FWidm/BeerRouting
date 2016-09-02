using UnityEngine;
using System.Collections;
using Pathfinding;

public class ProfessorStates : StateMachineBehaviour
{
    ProfessorController professorController;
    LevelController levelController;


    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        professorController = GameObject.FindWithTag("Professor").GetComponent<ProfessorController>();
        HotPotatoTimer hotpotatoTimer = FindObjectOfType<HotPotatoTimer>();

        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();

        if (stateInfo.IsName("Appear") || stateInfo.IsName("AppearBeer") || stateInfo.IsName("AppearBeerGlass") || stateInfo.IsName("AppearAngry") || stateInfo.IsName("AppearMoney"))
        {
            professorController.SetVisible(true);
            // Hide professor button.
            ProfessorButton professorButton = FindObjectOfType<ProfessorButton>();
            if (professorButton != null)
                professorButton.SetVisible(false);
            // Disable gameplay interaction on current level controller.
            if (levelController != null)
                levelController.SetGameInputEnabled(false);
            //when hotpotato is the game mode and the prof appears, stop timers
            if (hotpotatoTimer != null)
            {
                hotpotatoTimer.PauseTimer();
                //disable movement of the cam when the prof is visible
                CameraFollowPlayer camera = FindObjectOfType<CameraFollowPlayer>();
                if (camera != null)
                {
                    camera.EnableMovement(false);
                }
            }
        }
        else if (stateInfo.IsName("Disappear") || stateInfo.IsName("DisappearBeer") || stateInfo.IsName("DisappearBeerGlass") || stateInfo.IsName("DisappearAngry") || stateInfo.IsName("DisappearMoney"))
        {
            // Hide speech bubble.
            professorController.speechBubble.SetVisible(false);
        }
        else if (stateInfo.IsName("Money") || stateInfo.IsName("Money2") || stateInfo.IsName("Money3"))
        {
            // Reset money animation flag.
            professorController.Money(false);
        }
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        professorController = GameObject.FindWithTag("Professor").GetComponent<ProfessorController>();
        HotPotatoTimer hotpotatoTimer = FindObjectOfType<HotPotatoTimer>();

        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();

        if (stateInfo.IsName("Appear") || stateInfo.IsName("AppearBeer") || stateInfo.IsName("AppearBeerGlass") || stateInfo.IsName("AppearAngry") || stateInfo.IsName("AppearMoney"))
        {
            // Activate speech bubble.
            professorController.speechBubble.gameObject.SetActive(true);
            // Show speech bubble.
            professorController.speechBubble.SetVisible(true);
        }
        else if (stateInfo.IsName("Disappear") || stateInfo.IsName("DisappearBeer") || stateInfo.IsName("DisappearBeerGlass") || stateInfo.IsName("DisappearAngry") || stateInfo.IsName("DisappearMoney"))
        {
            professorController.SetVisible(false);
            // Hide professor button.
            ProfessorButton professorButton = FindObjectOfType<ProfessorButton>();
            if (professorButton != null)
                professorButton.SetVisible(true);
            // Deactivate speech bubble.
            professorController.speechBubble.gameObject.SetActive(false);

            // Enable gameplay interaction on current level controller.
            if (levelController != null)
                levelController.SetGameInputEnabled(true);

            // Invoke OnStopDisappear method on level controller.
            if (levelController != null)
                levelController.OnStopProfessorDisappear();

            // when hotpotato is active, resume timer when prof disappears
            if (hotpotatoTimer != null)
            {
                hotpotatoTimer.ResumeTimer();
            }

            //Enable Camera movement when the prof is gone.
            CameraFollowPlayer camera = FindObjectOfType<CameraFollowPlayer>();
            if (camera != null)
            {
                camera.EnableMovement(true);
            }
        }
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMachineEnter is called when entering a statemachine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
    //
    //}

    // OnStateMachineExit is called when exiting a statemachine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
    //
    //}
}
