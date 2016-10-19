using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBeer : MonoBehaviour
{

    public Image imageBeerFull;
    public Text textBeerScore;

    private float minStatus = 0.0f;
    private float maxStatus = 1f;
    private float status;
    private float nextStatus;
    private float score = 0f;
    private float maxScore = 100f;
    private float minScore = 0f;

    // Use this for initialization
    void Start()
    {
        // Initially the glas is empty.
        status = minStatus;
        nextStatus = status;
        imageBeerFull.fillAmount = status;
        textBeerScore.text = score + "%";
    }

    // Update is called once per frame
    void Update()
    {
        // Update beer level step by step until it meets the current score.
        if (nextStatus > status)
        {
            status += 0.05f * Time.deltaTime;
            if (status > nextStatus)
            {
                status = nextStatus;
            }
            imageBeerFull.fillAmount = status;
        }
        if (nextStatus < status)
        {
            status -= 0.05f * Time.deltaTime;
            if (status < nextStatus)
            {
                status = nextStatus;
            }
            imageBeerFull.fillAmount = status;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus) == true)
        {
            UpdateScore(10);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) == true)
        {
            UpdateScore(-10);
        }
    }

    public void UpdateScore(int amount)
    {
        // Add or remove amount from current score.
        score += amount;
        int statusChange = amount;
        if (Mathf.Abs(score) < amount)
        {
            statusChange = Mathf.RoundToInt(Mathf.Abs(score));
        }
        nextStatus = nextStatus + (maxStatus - minStatus) * (statusChange / maxScore);

        if (nextStatus > maxStatus)
        {
            nextStatus = maxStatus;
        }
        else if (nextStatus < minStatus)
        {
            nextStatus = minStatus;
        }

        // Update text score immediately.
        int textScore;       
        textScore = Mathf.RoundToInt((score / maxScore) * 100);
        // Ensure bounds 0 - 100%.
        if (score > maxScore)
        {
            textScore = 100;
        }
        if (score < minScore)
        {
            textScore = 0;
        }
        textBeerScore.text = textScore + "%";
    }

    public void SetMaxScore(int maxScore)
    {
        this.maxScore = maxScore;
    }

    public int GetScore()
    {
        return Mathf.RoundToInt((score / maxScore) * 100);
    }
}
