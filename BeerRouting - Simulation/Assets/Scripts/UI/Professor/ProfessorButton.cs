using UnityEngine;
using System.Collections;

public class ProfessorButton : MonoBehaviour {

    private ProfessorController professorController;
    private AudioSource audioBtnClick;

	// Use this for initialization
	void Start () {
        audioBtnClick = GetComponent<AudioSource>();
        professorController = FindObjectOfType<ProfessorController>();
    }

    public void OnClick()
    {
        // Play button click sound.
        audioBtnClick.Play();
        // Show professor.
        LevelController.GetCurrentLevelController().OnProfessorButtonClick();
    }	

    public void SetVisible(bool visible)
    {
        if (visible)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
