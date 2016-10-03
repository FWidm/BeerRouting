using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class ScoreTextScript : MonoBehaviour
{

    #region Sprites

    private Sprite minus5ScoreTextImage;
    private Sprite minus10ScoreTextImage;
    private Sprite minus15ScoreTextImage;
    private Sprite minus20ScoreTextImage;
    private Sprite plus0ScoreTextImage;
    private Sprite plus5ScoreTextImage;
    private Sprite plus10ScoreTextImage;
    private Sprite plus15ScoreTextImage;
    private Sprite plus20ScoreTextImage;

    #endregion Sprites

    /// <summary>
    /// Defines the time the score text is actually displayed before being hidden again.
    /// </summary>
    public float destroyDelayTimeInSeconds;

    /// <summary>
    /// Defines the scale increment by that the scale of the image object is increased
    /// in every frame.
    /// </summary>
    public float scaleIncrementPerFrame;

    /// <summary>
    /// A reference to the Image object on the Canvas of the scene.
    /// </summary>
    private Image scoreTextImage;

    void Awake()
    {
        // Load sprites of text scores.
        minus5ScoreTextImage = Resources.Load<Sprite>("Scores/ScoreMinus5");
        minus10ScoreTextImage = Resources.Load<Sprite>("Scores/ScoreMinus10");
        minus15ScoreTextImage = Resources.Load<Sprite>("Scores/ScoreMinus15");
        minus20ScoreTextImage = Resources.Load<Sprite>("Scores/ScoreMinus20");
        plus0ScoreTextImage = Resources.Load<Sprite>("Scores/ScorePlus0");
        plus5ScoreTextImage = Resources.Load<Sprite>("Scores/ScorePlus5");
        plus10ScoreTextImage = Resources.Load<Sprite>("Scores/ScorePlus10");
        plus15ScoreTextImage = Resources.Load<Sprite>("Scores/ScorePlus15");
        plus20ScoreTextImage = Resources.Load<Sprite>("Scores/ScorePlus20");
    }

    // Use this for initialization
    void Start()
    {
        // The image is identified by the tag ScoreTextCanvasImg.
        scoreTextImage = GameObject.FindGameObjectWithTag("ScoreTextCanvasImg").GetComponent<Image>();
    }

    /// <summary>
    /// Displays a score text on the screen. The ScoreText parameter
    /// defines which of the different score text objects will be displayed.
    /// </summary>
    /// <param name="scoreText">Specifies which score text is shown.</param>
    public void DisplayScoreText(ScoreText scoreText)
    {
        Sprite currentSprite;

        if (scoreTextImage == null)
        {
            Debug.LogError("ScoreTextImage is null.");
            return;
        }

        switch (scoreText)
        {
            case ScoreText.Minus5:
                currentSprite = minus5ScoreTextImage;
                break;
            case ScoreText.Minus10:
                currentSprite = minus10ScoreTextImage;
                break;
            case ScoreText.Minus15:
                currentSprite = minus15ScoreTextImage;
                break;
            case ScoreText.Minus20:
                currentSprite = minus20ScoreTextImage;
                break;
            case ScoreText.Plus0:
                currentSprite = plus0ScoreTextImage;
                break;
            case ScoreText.Plus5:
                currentSprite = plus5ScoreTextImage;
                break;
            case ScoreText.Plus10:
                currentSprite = plus10ScoreTextImage;
                break;
            case ScoreText.Plus15:
                currentSprite = plus15ScoreTextImage;
                break;
            case ScoreText.Plus20:
                currentSprite = plus20ScoreTextImage;
                break;
            default:
                currentSprite = null;
                break;
        }
        // Set image width, height and sprite according to current spirte.
        scoreTextImage.sprite = currentSprite;
        RectTransform rtScoreTextImage = (RectTransform)scoreTextImage.transform;
        rtScoreTextImage.sizeDelta = new Vector2(currentSprite.rect.width, currentSprite.rect.height);

        // Set the scale to 0. 
        scoreTextImage.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);

        // Set the transparency to 1, so that the sprite becomes visible.
        scoreTextImage.color = new Vector4(scoreTextImage.color.r, scoreTextImage.color.g, scoreTextImage.color.b, 1.0f);

        // Start scale the score text up over time.
        StartCoroutine("ScaleUpScoreText");

        // Hide the score text after the specified delay.
        StartCoroutine("HideScoreTextAfterDelay");
    }

    /// <summary>
    /// Scales up the image of the currently displayed score text over time.
    /// That means the score text is incresing in size over time until it reaches 
    /// the full scaling factor.
    /// </summary>
    IEnumerator ScaleUpScoreText()
    {
        // Do while the scale is not 1.0f in x and y direction.
        while (scoreTextImage != null &&
               scoreTextImage.transform.localScale.x <= 1.0f &&
               scoreTextImage.transform.localScale.y <= 1.0f)
        {
            // Scale up the score text in every iteration.
            scoreTextImage.transform.localScale += new Vector3(scaleIncrementPerFrame, scaleIncrementPerFrame, 0.0f);

            // yield return pauses the execution of the coroutine until the next frame.
            yield return null;
        }
    }

    /// <summary>
    /// Waits the specified delay time and then sets the currently displayed
    /// score text image to invisible.
    /// </summary>
    IEnumerator HideScoreTextAfterDelay()
    {
        // Wait the specified delay time before executing the following code.
        yield return new WaitForSeconds(destroyDelayTimeInSeconds);

        if (scoreTextImage != null)
        {
            // Set the scale to 0. 
            scoreTextImage.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);
            // Set the alpha value of the image to 0, so that it becomes invisible.
            // Vector4 currentColor = scoreTextCanvasImage.color;
            // scoreTextCanvasImage.color = new Vector4(currentColor.x, currentColor.y, currentColor.z, 0.0f);
        }
    }

}

/// <summary>
/// Enum, welches mögliche ScoreText Objekte definiert.
/// </summary>
public enum ScoreText
{
    Minus5,
    Minus10,
    Minus15,
    Minus20,
    Plus5,
    Plus10,
    Plus15,
    Plus20,
    Plus0
}
