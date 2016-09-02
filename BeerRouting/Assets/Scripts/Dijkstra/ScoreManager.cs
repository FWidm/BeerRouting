using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    public int score = 0;

    private Text uiText;

	// Use this for initialization
	void Start () {
        GameObject uiScoreText = GameObject.FindGameObjectWithTag("Score");
        uiText = uiScoreText.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        // Set the score text.
        uiText.text = "Score: " + score;
	}

    /// <summary>
    /// Updates the score by adding up a specified value onto the current score.
    /// </summary>
    /// <param name="value">The value that will be added to the score.</param>
    public void UpdateScoreByValue(int value)
    {
        if ((score + value) >= 0)
        {
            score += value;
        }
        else
        {
            score = 0;
        }
    }
}
