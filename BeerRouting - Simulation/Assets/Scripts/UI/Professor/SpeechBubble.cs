using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeechBubble : MonoBehaviour
{
    public Image imageBubble;
    public Image imageStar1;
    public Image imageStar2;
    public Image imageStar3;
    public Sprite starFull;
    public Sprite starEmpty;
    public Text textNormal;
    public Text textLevel;
    public Text textScore;
    public Button buttonPrev;
    public Button buttonNext;
    public Button buttonOk;

    private ProfessorController professorController;
    private float imageHeight = 299f;
    // private float minImageWidth = 350f;
    // private float maxImageWidth = 1580f;
    private float textHeight = 200f;
    private AudioSource audioBtnClick;
    private AudioSource audioBubbleAppear;
    private AudioSource audioBubbleDisappear;
    private AudioSource audioStarAppear;

    private LevelController levelController;
    private TutorialControllerHotPotato tutHotPototoCtrl;
    private TutorialControllerGreedy tutGreedyCtrl;
    private LevelControllerPlatform levelControllerPlatform;

    private System.Collections.Generic.Dictionary<int, SpeechBubbleState> states;
    private SpeechBubbleState state;
    private bool lastState;
    private int score;
    private int numberOfStars;
    private bool animateScore;
    private int currentScore;
    private Vector2 buttonOkPos;

    private int maxScore =-1;

    void Awake()
    {
        buttonOkPos = buttonOk.transform.localPosition;
        professorController = FindObjectOfType<ProfessorController>();
    }

    // Use this for initialization
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioBtnClick = audioSources[0];
        audioBubbleAppear = audioSources[1];
        audioBubbleDisappear = audioSources[2];
        audioStarAppear = audioSources[3];
        buttonPrev.gameObject.SetActive(false);
        buttonOk.gameObject.SetActive(false);
        SetState(0);

        // Init current level controller.
        levelController = LevelController.GetCurrentLevelController();
        tutHotPototoCtrl = FindObjectOfType<TutorialControllerHotPotato>();
        tutGreedyCtrl = FindObjectOfType<TutorialControllerGreedy>();
        levelControllerPlatform = FindObjectOfType<LevelControllerPlatform>();

        gameObject.SetActive(false);
        lastState = false;
        imageStar1.gameObject.SetActive(false);
        imageStar2.gameObject.SetActive(false);
        imageStar3.gameObject.SetActive(false);
        textLevel.gameObject.SetActive(false);
        textScore.gameObject.SetActive(false);
        animateScore = false;
        currentScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) == true)
        {
            if (buttonPrev.IsActive())
                OnClickPrev();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) == true)
        {
            if (buttonNext.IsActive())
            {
                OnClickNext();
            }
            else if (buttonOk.IsActive())
            {
                OnClickOk();
            }
        }

        if (animateScore)
        {
            if (currentScore < score && currentScore < 100)
            {
                currentScore++;
            }
            else
            {
                animateScore = false;
            }
            Debug.Log("levelProp score = "+maxScore);
            textScore.text = "Du hast "+currentScore.ToString() + " % von möglichen "+maxScore+" Punkten erreicht!";
        }
    }

    public void SetState(int id)
    {
        state = (SpeechBubbleState)states[id];
        if (state != null)
        {
            // Update speech bubble text placeholders.
            state.UpdatePlaceholders();

            // Resize speech bubble image to state size.
            RectTransform rtImage = (RectTransform)imageBubble.transform;
            rtImage.sizeDelta = new Vector2(state.width + 30, imageHeight);

            // Resize speech bubble text to fit in new size.
            RectTransform rtText = (RectTransform)textNormal.transform;
            float textWidth = state.width - 200;
            rtText.sizeDelta = new Vector2(textWidth, textHeight);
            textNormal.text = state.text;

            // Move ok button to the mittle of the speech bubble if there is only one state.
            if (states.Values.Count == 1)
            {
                buttonOk.transform.localPosition = new Vector2(85, -125);
                buttonPrev.gameObject.SetActive(false);
                buttonNext.gameObject.SetActive(false);
            }
            else
            {
                buttonOk.transform.localPosition = buttonOkPos;
                buttonPrev.gameObject.SetActive(true);
                buttonNext.gameObject.SetActive(true);
                buttonPrev.interactable = false;
            }

            // Disable previous button if it's the first state of the sequence.
            if (state.id == 0)
            {
                buttonPrev.interactable = false;
            }
            else
            {
                buttonPrev.interactable = true;
            }

            if (state.id == states.Values.Count - 1)
            {
                // Hide next button and show ok button.
                buttonNext.gameObject.SetActive(false);
                buttonOk.gameObject.SetActive(true);
            }
            else
            {
                // Show next button and hide ok button.
                buttonNext.gameObject.SetActive(true);
                buttonOk.gameObject.SetActive(false);
            }
        }
    }

    public void SetStates(System.Collections.Generic.Dictionary<int, SpeechBubbleState> states)
    {
        this.states = states;
        SetState(0);
    }

    public void OnClickPrev()
    {
        if (state.id > 0)
        {
            // Play button click sound.
            audioBtnClick.Play();
            // Go to previous speech bubble state.
            SetState(state.id - 1);
            // Start professor speaking.
            StartCoroutine(PlayVoiceAfterTime());
        }

        NextPrevEvent();
    }

    public void OnClickNext()
    {
        if (state.id < states.Values.Count - 1)
        {
            // Play button click sound.
            audioBtnClick.Play();
            // Go to next speech bubble state.
            SetState(state.id + 1);
            // Show previous button.
            buttonPrev.gameObject.SetActive(true);
            // Start professor speaking.
            StartCoroutine(PlayVoiceAfterTime());
        }

        NextPrevEvent();
    }

    private void NextPrevEvent()
    {
        if (tutHotPototoCtrl != null)
        {
            tutHotPototoCtrl.OnSpeechBubble();
        }
        else if (tutGreedyCtrl != null)
        {
            tutGreedyCtrl.OnSpeechBubble();
        }
    }

    public void OnClickOk()
    {
        if (levelControllerPlatform != null)
        {
            if (lastState)
                levelControllerPlatform.OnLevelComplete();
            else
                levelControllerPlatform.UpdateProfessor();
        }
        else
        {
            if (lastState)
            {
                levelController.OnLevelComplete();
            }
            else if (state.id == states.Values.Count - 1)
            {
                // Stop professor speaking.
                professorController.StopVoice();
                levelController.UpdateProfessor();
            }
        }
    }

    public void SetVisible(bool visible)
    {
        if (visible)
        {
            // Play speech bubble appearing sound.
            audioBubbleAppear.Play();
            // Make speech bubble visible.
            transform.localScale = new Vector3(1, 1, 1);
            // Update speech bubble state placeholders.
            state.UpdatePlaceholders();
            textNormal.text = state.text;

            if (lastState)
            {
                // Show stars on last state.
                StartCoroutine(ShowStarsAfterTime());
            }
            else
            {
                // No professor voice on last state.
                StartCoroutine(PlayVoiceAfterTime());
            }
        }
        else
        {
            // Play speech bubble disappearing sound.
            audioBubbleDisappear.Play();
            // Make speech bubble invisible but not inactive.
            transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public SpeechBubbleState GetState()
    {
        return state;
    }

    public void SetLastState()
    {
        lastState = true;

        // Show or hide ui elements.
        buttonPrev.gameObject.SetActive(false);
        buttonNext.gameObject.SetActive(false);
        buttonOk.gameObject.SetActive(false);

        imageStar1.gameObject.SetActive(true);
        imageStar2.gameObject.SetActive(true);
        imageStar3.gameObject.SetActive(true);

        textNormal.gameObject.SetActive(false);
        textLevel.gameObject.SetActive(true);
        textScore.gameObject.SetActive(true);

        // Resize speech bubble image to level state size.
        RectTransform rtImage = (RectTransform)imageBubble.transform;
        rtImage.sizeDelta = new Vector2(515, imageHeight);

        // Move ok button to the bottom of the speech bubble.
        // buttonOk.transform.localPosition = new Vector2(rtImage.sizeDelta.x - 70, -128.4f);

        // Move ok button to the mittle of the speech bubble if there is only one state.
        buttonOk.transform.localPosition = new Vector2(85, -125);

        // Set level name.
        textLevel.text = FindObjectOfType<LevelProperties>().levelName;
        maxScore=FindObjectOfType<LevelProperties>().levelMaxScore;
        // Set level score as stars.
        score = FindObjectOfType<ScoreBeer>().GetScore();
        if (score >= 100)
        {
            numberOfStars = 3;
        }
        else if (score > 65)
        {
            numberOfStars = 2;
        }
        else if (score > 32)
        {
            numberOfStars = 1;
        }
        else
        {
            numberOfStars = 0;
        }

        imageStar1.sprite = starEmpty;
        imageStar2.sprite = starEmpty;
        imageStar3.sprite = starEmpty;
    }

    IEnumerator ShowStarsAfterTime()
    {
        animateScore = true;

        // Wait a second.
        yield return new WaitForSeconds(1);

        for (int i = 0; i < numberOfStars; i++)
        {
            // Play sound and show star.
            audioStarAppear.Play();
            switch (i)
            {
                case 0: imageStar1.sprite = starFull; break;
                case 1: imageStar2.sprite = starFull; break;
                case 2: imageStar3.sprite = starFull; break;
            }
            // Wait until star appear sound is over.
            yield return new WaitForSeconds(audioStarAppear.clip.length);
        }

        // Wait a short time.
        yield return new WaitForSeconds(0.3f);

        // Enable ok button to quit level.
        audioBtnClick.Play();
        buttonOk.gameObject.SetActive(true);
    }

    public int GetNumberOfStars()
    {
        return numberOfStars;
    }

    IEnumerator PlayVoiceAfterTime()
    {
        // Wait a short time.
        yield return new WaitForSeconds(audioBubbleAppear.clip.length + 0.1f);

        // Select appropriate voice length according to the length of the speech bubbles text.
        if (state.text.Length < 40)
        {
            professorController.PlayVoice(0);
        }
        else if (state.text.Length < 80)
        {
            professorController.PlayVoice(1);
        }
        else if (state.text.Length < 100)
        {
            professorController.PlayVoice(2);
        }
        else if (state.text.Length < 140)
        {
            professorController.PlayVoice(3);
        }
        else if (state.text.Length < 170)
        {
            professorController.PlayVoice(4);
        }
        else if (state.text.Length < 210)
        {
            professorController.PlayVoice(4);
        }
        else if (state.text.Length < 230)
        {
            professorController.PlayVoice(4);
        }
        else
        {
            professorController.PlayVoice(7);
        }


    }
}
