using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelControllerPlatform : MonoBehaviour
{

    private ScoreBeer scoreBeer;
    private ScoreTextScript scoreTexts;
    private ProfessorController professorController;
    private GameObject player;
    private LevelProperties levelProperties;
    private GameState gameState;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerPlatform");
    }

    // Use this for initialization
    void Start()
    {
        levelProperties = FindObjectOfType<LevelProperties>();
        scoreBeer = FindObjectOfType<ScoreBeer>();
        // Get reference to the ScoreTextManager.
        GameObject scoreTextGO = GameObject.FindGameObjectWithTag("ScoreTextManager");
        scoreTexts = scoreTextGO.GetComponent<ScoreTextScript>();
        professorController = FindObjectOfType<ProfessorController>();
        if (!ApplicationState.levelRestarted)
        {
            StartCoroutine(ShowProfessorAfterTime());
            player.SetActive(false);
        }
        gameState = FindObjectOfType<GameState>();
		// Calculate max score.
		int maxScore = determineMaxScore();

		FindObjectOfType<ScoreBeer>().SetMaxScore(maxScore);
    }

    public void LoadScene(int level)
    {
        StartCoroutine(LoadSceneAfterTime(level, 0.062f));
    }

    IEnumerator LoadSceneAfterTime(int level, float time)
    {
        // Wait until button click sound is over.
        yield return new WaitForSeconds(time);
        // Then load scene.
        SceneManager.LoadScene(level);
    }

    public void AddPoints(ScoreText score)
    {
        scoreTexts.DisplayScoreText(score);

        switch (score)
        {
            case ScoreText.Plus5:
                scoreBeer.UpdateScore(5);
                break;
            case ScoreText.Plus10:
                scoreBeer.UpdateScore(10);
                break;
            case ScoreText.Plus15:
                scoreBeer.UpdateScore(15);
                break;
            case ScoreText.Plus20:
                scoreBeer.UpdateScore(20);
                break;
        }
    }

    public void LevelFailed()
    {
        StartCoroutine(LoadSceneAfterTime(26, 2.5f));
        GameObject go = GameObject.FindGameObjectWithTag("BonusLevelFailed");
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            child.SetActive(true);
        }
        FindObjectOfType<PlayerPlatformController>().Kill();
        ApplicationState.levelRestarted = true;
    }

    public void OnGoal()
    {
        GameObject go = GameObject.FindGameObjectWithTag("BonusLevelFinished");
        for (int i = 0; i < go.transform.childCount; i++)
        {
            GameObject child = go.transform.GetChild(i).gameObject;
            child.SetActive(true);
        }

        Platformer_Movement pm = FindObjectOfType<Platformer_Movement>();
        if (pm != null)
            pm.LevelFinished();

        FindObjectOfType<PlayerPlatformController>().Drink();
        StartCoroutine(StopPlayerAfterTime());
        professorController.ShowStars();
    }

    public void UpdateProfessor()
    {
        professorController.ShowBeer(false, 2);
        player.SetActive(true);
        player.GetComponentInChildren<PlayerPlatformController>().StartBeerThrowing();
    }

    IEnumerator ShowProfessorAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(1.5f);
        // Then show professor.
        professorController.ShowBeer(true, 2);
    }

    public Platformer_Movement GetPlayerMovementScript()
    {
        return player.GetComponentInChildren<Platformer_Movement>();
    }

    public void OnLevelComplete()
    {
        // Save level state.
        SaveLevelState();

        // Go to main menu.
        SceneManager.LoadScene(0);
    }

    private void SaveLevelState()
    {
        // Get current level state.
        LevelState levelState;
        int score = scoreBeer.GetScore();
        int numberOfStars = professorController.speechBubble.GetNumberOfStars();
        int levelId = levelProperties.levelId;
        levelState = new LevelState(levelId, numberOfStars, score);

        // Get saved level state.
        LevelState savedState = gameState.GetLevelStateByLevelId(levelId);

        // Only save new level state if result is better.
        if (savedState != null)
        {
            if (savedState.PlayerScore >= score)
            {
                return;
            }
        }

        gameState.AddOrReplaceLevelState(levelState);
        gameState.StoreGameState();

        // gameState.AddLevelState();
    }


    IEnumerator StopPlayerAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(1.0f);
        // Disable player movement.
        GetPlayerMovementScript().BlockMovement(true);
    }

	/// <summary>
	/// Determines the max score for the level.
	/// </summary>
	/// <returns>The max score.</returns>
	private int determineMaxScore()
	{
		int maxScore = 0;
		Platformer_Collectibles[] collectibles = GameObject.FindObjectsOfType<Platformer_Collectibles> ();
		foreach (var collectible in collectibles) {
			string collectibleName = collectible.gameObject.name.ToLower();
			if (collectibleName.Contains ("point")) {
				maxScore += 5;
			} else if (collectibleName.Contains ("sunglasses")) {
				maxScore += 20;
			} else if (collectibleName.Contains ("money")) {
				maxScore += 10;
			}
		}
		return maxScore;
	}
}
