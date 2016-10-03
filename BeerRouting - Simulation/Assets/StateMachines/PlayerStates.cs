using UnityEngine;
using System.Collections;
using System.Configuration;

public class PlayerStates : StateMachineBehaviour
{

    LevelController levelController;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();

        if (stateInfo.IsName("Walk"))
        {
            levelController.OnStartPlayerWalking();
            //disable movement of the cam when the prof is visible
            CameraFollowPlayer camera = FindObjectOfType<CameraFollowPlayer>();
            if (camera != null)
            {
                camera.EnableMovement(false);
            }
        }
        if (stateInfo.IsName("Jump"))
        {
            MovementScript ms = FindObjectOfType<MovementScript>();
            if (ms != null)
                ms.StopJumping();
        }
        if (stateInfo.IsName(("Idle")))
        {
            if (levelController != null)
                if (levelController.levelStart)
                {
                    levelController.levelStart = false;

                }
                else
                    levelController.OnEnterPlayerIdle();
        }
        if (stateInfo.IsName("DrinkBeer"))
        {
            FindObjectOfType<PlayerController>().SetDrinking(true);
        }
        if (stateInfo.IsName("ThrowBeer"))
        {
            FindObjectOfType<PlayerController>().StopThrowBeer();
        }
    }

    //OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();

        if (stateInfo.IsName("Walk"))
        {
            levelController.OnStopPlayerWalking();
            // Disable movement of the cam when the prof is visible.
            CameraFollowPlayer camera = FindObjectOfType<CameraFollowPlayer>();
            ProfessorController professorController = FindObjectOfType<ProfessorController>();
            if (camera != null /*&& !professorController.IsVisible()*/)
            {
                camera.EnableMovement(true);
            }
        }

        if (stateInfo.IsName("DrinkBeer"))
        {
            levelController.OnStopPlayerDrink();
        }
    }
}
