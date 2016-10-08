using UnityEngine;
using System.Collections;

public class ProfessorButton : MonoBehaviour {

    private ProfessorController professorController;
    private AudioSource audioBtnClick;
    private int buttonClickCount;

	// Use this for initialization
	void Start () {
        audioBtnClick = GetComponent<AudioSource>();
        professorController = FindObjectOfType<ProfessorController>();
        buttonClickCount = 0;
    }
    public int GetClickCount() {
        return buttonClickCount;
    }

    public void OnClick()
    {
        // Play button click sound.
        audioBtnClick.Play();
        // Show professor.
        LevelController.GetCurrentLevelController().OnProfessorButtonClick();
        buttonClickCount++;
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
