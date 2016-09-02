using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProfessorController : MonoBehaviour
{

    public Animator animator;
    public SpeechBubble speechBubble;
    public SpeechBubbleSequences sequences;

    private ProfessorButton professorButton;
    private Image speechBubbleImage;
    private Text speechBubbleText;
    private AudioSource[] audioGetBeer;
    private AudioSource audioVoice;

    private SpeechBubbleSequence sequence;
    private bool showBeer;
    private bool blink;
    private System.Random rnd;
    private int stopAtSample;
    private ArrayList audioSequence;
    private bool visible;

    // Use this for initialization
    void Start()
    {
        showBeer = false;
        blink = false;
        rnd = new System.Random();
        audioGetBeer = GetComponents<AudioSource>();
        audioVoice = audioGetBeer[2];
        sequence = sequences.GetSequence(0);
        speechBubble.SetStates(sequence.states);
        professorButton = FindObjectOfType<ProfessorButton>();
        //Debug.Log("Prof");
        //foreach(SpeechBubbleSequence  s in sequences.sequences.Values)
        //{
        //    foreach(SpeechBubbleState state in s.states.Values)
        //    {
        //        Debug.Log(">>>>" + state.Text + " from " + s.gameObject.name);
        //    }
        //}
    }

    public void ReReadStates()
    {
        Debug.Log(sequences);
    }
    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.N) == true)
        {
            NextSequence();
        }
        if (Input.GetKeyDown(KeyCode.B) == true)
        {
            ShowBeer(true, 0);
        }
        if (Input.GetKeyDown(KeyCode.H) == true)
        {
            ShowBeer(false, 0);
        }
        if (Input.GetKeyDown(KeyCode.A) == true)
        {
            Show(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Show(false);
            ShowBeer(false, 0);
            ShowAngry(false);
            StopVoice();
        }
        if (Input.GetKeyDown(KeyCode.L) == true)
        {
            ShowStars();
        }
        */

        VoiceControl();
    }

    public void ShowStars()
    {
        speechBubble.SetLastState();
        if (speechBubble.GetNumberOfStars() > 0)
        {
            ShowBeer(true, 1);
        }
        else
        {
            ShowAngry(true);
        }
        TalkAnimation(false);
    }

    public void Show(bool show)
    {
        // Update animator variables.
        animator.SetBool("Appear", show);
        animator.SetBool("Disappear", !show);
        if (show)
            TalkAnimation(false);

        if (show && !blink)
        {
            StartCoroutine(EyeBlinkAfterTime(5));
        }
    }

    public void ShowAngry(bool show)
    {
        // Update animator variables.
        animator.SetBool("AppearAngry", show);
        animator.SetBool("Disappear", !show);
        if (show)
            TalkAnimation(false);

        if (show && !blink)
        {
            StartCoroutine(EyeBlinkAfterTime(5));
        }
    }

    public void ShowMoney(bool show)
    {
        // Update animator variables.
        animator.SetBool("AppearMoney", show);
        animator.SetBool("Disappear", !show);
        //animator.SetBool("DisappearMoney", !show);
        if (show)
            TalkAnimation(false);

        if (show && !blink)
        {
            StartCoroutine(EyeBlinkAfterTime(5));
        }
        Money(true);
    }

    public void Money(bool show)
    {
        if (show)
        {
            int random = Random.Range(0, 3);
            if (random == 0)
                animator.SetBool("Money", true);
            else if (random == 1)
                animator.SetBool("Money2", true);
            else
                animator.SetBool("Money3", true);
        }
        else
        {
            animator.SetBool("Money", false);
            animator.SetBool("Money2", false);
            animator.SetBool("Money3", false);
        }
    }

    public void SetSequenceAndState(int sequenceId, int stateId)
    {
        sequence = sequences.GetSequence(sequenceId);
        speechBubble.SetStates(sequence.states);
        speechBubble.SetState(stateId);
    }

    public void NextSequence()
    {
        if (sequence.id < sequences.Size() - 1)
        {
            sequence = sequences.GetSequence(sequence.id + 1);
            speechBubble.SetStates(sequence.states);
        }
    }

    IEnumerator EyeBlinkAfterTime(float time)
    {
        blink = true;
        yield return new WaitForSeconds(time);

        // Repeat this function as long as the professor is visible.
        if (visible)
        {
            // Play the professors eye blink animation once.
            animator.Play("Blink");
            // Create a random number between 5 and 15.
            time = rnd.Next(5, 16);
            StartCoroutine(EyeBlinkAfterTime(time));
        }
        else
        {
            blink = false;
        }
    }

    public void ShowBeer(bool show, int version)
    {
        // Do nothing if method already running.
        showBeer = show;
        if (professorButton != null)
            professorButton.SetVisible(false);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Invisible") && showBeer)
        {
            TalkAnimation(false);
            //Debug.Log("version: " + version);
            switch (version)
            {
                case 0:
                    // Play long get beer sound.
                    audioGetBeer[version].Play();
                    // Show professor with beer after audio is finished.
                    StartCoroutine(ShowBeerAfterTime(version));
                    break;
                case 1:
                    // Play short get beer sound.
                    audioGetBeer[version].Play();
                    // Show professor with beer after audio is finished.
                    StartCoroutine(ShowBeerAfterTime(version));
                    break;
                case 2:
                    // Show professor with beer but no sound.
                    animator.SetBool("AppearBeer", showBeer);
                    animator.SetBool("DisappearBeer", !showBeer);
                    // Reset flag.
                    showBeer = false;
                    break;
                case 3:
                    // Play get beer glass sound.
                    audioGetBeer[version].Play();
                    // Show professor with beer glass after audio is finished.
                    StartCoroutine(ShowBeerAfterTime(version));
                    break;
                case 4:
                    // Show professor with beer glass immediately.
                    animator.SetBool("AppearBeerGlass", showBeer);
                    animator.SetBool("Disappear", !showBeer);
                    break;
            }
            // Activate eye blinking.
            if (show && !blink)
            {
                StartCoroutine(EyeBlinkAfterTime(5));
            }
        }
        else
        {
            // Hide visible professor.
            animator.SetBool("Appear", false);
            animator.SetBool("Disappear", true);
            // Hide visible professor with beer.
            animator.SetBool("AppearBeer", false);
            animator.SetBool("Disappear", true);
            // Hide visible professor with beer glass.
            animator.SetBool("AppearBeerGlass", false);
            animator.SetBool("Disappear", true);
        }
    }

    IEnumerator ShowBeerAfterTime(int version)
    {
        // Wait until get beer sound is over.
        yield return new WaitForSeconds(audioGetBeer[version].clip.length - 0.5f);
        if (version != 3)
        {
            // Show professor with beer.
            animator.SetBool("AppearBeer", showBeer);
            animator.SetBool("Disappear", !showBeer);
        }
        else
        {
            // Show professor with beer glas.
            animator.SetBool("AppearBeerGlass", showBeer);
            animator.SetBool("Disappear", !showBeer);
        }
        // Reset flag.
        showBeer = false;
    }

    public bool ShouldShowBeer()
    {
        return showBeer;
    }

    public int GetCurrentSequenceId()
    {
        return sequence.id;
    }

    public int GetCurrentStateId()
    {
        return speechBubble.GetState().id;
    }

    public void StopVoice()
    {
        if (audioVoice.isPlaying)
        {
            TalkAnimation(false);
            StartCoroutine(AudioFadeOut.FadeOut(audioVoice, 1));
        }
    }

    /// <summary>
    /// Causes the professor to start speaking.
    /// </summary>
    /// <param name="speakingTime">The speaking time defines how long the professor will speak.</param>
    public void PlayVoice(int speakingTime)
    {
        audioSequence = new ArrayList();

        switch (speakingTime)
        {
            case 0:
                // Short sequence length.
                audioSequence.Add(0);
                break;
            case 1:
                // Middle short sequence length.
                if (1 == Random.Range(0, 2))
                {
                    audioSequence.Add(3);
                }
                else
                {
                    audioSequence.Add(4);
                }
                break;
            case 2:
                // Middle sequence length.
                if (1 == Random.Range(0, 2))
                {
                    audioSequence.Add(4);
                    audioSequence.Add(1);
                }
                else
                {
                    audioSequence.Add(5);
                    audioSequence.Add(1);
                }
                break;
            case 3:
                // Middle long sequence length 1.
                if (1 == Random.Range(0, 2))
                {
                    audioSequence.Add(2);
                    audioSequence.Add(7);
                }
                else
                {
                    audioSequence.Add(4);
                    audioSequence.Add(7);
                }
                break;
            case 4:
                // Middle long sequence length 2.
                if (1 == Random.Range(0, 2))
                {
                    audioSequence.Add(5);
                    audioSequence.Add(0);
                    audioSequence.Add(7);
                }
                else
                {
                    audioSequence.Add(4);
                    audioSequence.Add(0);
                    audioSequence.Add(7);
                }
                break;
            case 5:
                // Long sequence length 1.
                if (1 == Random.Range(0, 2))
                {
                    audioSequence.Add(2);
                    audioSequence.Add(5);
                    audioSequence.Add(1);
                }
                else
                {
                    audioSequence.Add(3);
                    audioSequence.Add(6);
                    audioSequence.Add(1);
                }
                break;
            case 6:
                // Long sequence length 2.
                if (1 == Random.Range(0, 2))
                {
                    audioSequence.Add(2);
                    audioSequence.Add(5);
                    audioSequence.Add(0);
                    audioSequence.Add(1);
                }
                else
                {
                    audioSequence.Add(3);
                    audioSequence.Add(4);
                    audioSequence.Add(0);
                    audioSequence.Add(1);
                }
                break;
            case 7:
                // Very long sequence length.
                if (1 == Random.Range(0, 2))
                {
                    audioSequence.Add(6);
                    audioSequence.Add(4);
                    audioSequence.Add(1);
                    audioSequence.Add(7);
                }
                else
                {
                    audioSequence.Add(5);
                    audioSequence.Add(6);
                    audioSequence.Add(1);
                    audioSequence.Add(7);
                }
                break;
        }

        SetVoice((int)audioSequence[0]);
        // Start talking audio.
        audioVoice.Play();
        // Start talking animation.
        TalkAnimation(true);
    }

    private void SetVoice(int id)
    {
        // Number of samples: 777600
        switch (id)
        {
            case 0:
                // Fast. Very short length.
                audioVoice.timeSamples = 296000;
                stopAtSample = 342000;
                break;
            case 1:
                // Slow. Short length 1.
                audioVoice.timeSamples = 112000;
                stopAtSample = 202000;
                break;
            case 2:
                // Fast. Short length 2.
                audioVoice.timeSamples = 202000;
                stopAtSample = 296000;
                break;
            case 3:
                // Slow. Middle length 1.
                audioVoice.timeSamples = 0;
                stopAtSample = 112000;
                break;
            case 4:
                // Fast. Middle length 2.
                audioVoice.timeSamples = 354000;
                stopAtSample = 458000;
                break;
            case 5:
                // Fast. Middle length 3.
                audioVoice.timeSamples = 458000;
                stopAtSample = 568000;
                break;
            case 6:
                // Fast. Long length 1.
                audioVoice.timeSamples = 568000;
                stopAtSample = 659000;
                break;
            case 7:
                // Slow. Long length 2.
                audioVoice.timeSamples = 659000;
                stopAtSample = 760000;
                break;
        }
    }

    private void VoiceControl()
    {
        // Check if audio part is about to be completely played.
        if (audioVoice.isPlaying && audioVoice.timeSamples > stopAtSample - 12000)
        {
            // Stop talking animation.
            TalkAnimation(false);
        }

        // Check if audio part has been played completely.
        if (audioVoice.isPlaying && audioVoice.timeSamples > stopAtSample)
        {
            if (audioSequence.Count <= 1)
            {
                // If audio sequence is empty, stop audio.
                audioVoice.Stop();
                audioSequence.RemoveAt(0);
            }
            else
            {
                // If audio sequence isn't empty, play next audio part.
                SetVoice((int)audioSequence[1]);
                // Remove current audio part from sequence list.
                audioSequence.RemoveAt(0);
                // Start talking animation again.
                TalkAnimation(true);
            }
        }
    }

    public void TalkAnimation(bool talk)
    {
        if (talk)
        {
            // Get current audio sequence.
            int cas = (int)audioSequence[0];
            // Start talking animation.
            animator.SetBool("StopTalking", false);
            // Check if professor is angry.
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleAngry"))
            {
                // Lower pitch.
                audioVoice.pitch = 0.93f;
                if (cas == 1 || cas == 3 || cas == 7)
                {
                    // Talk: angry, normal.
                    animator.Play("TalkAngryNormal");
                }
                else
                {
                    // Talk: angry, fast.
                    animator.Play("TalkAngryFast");
                }
            }
            else
            {
                // Normal pitch.
                audioVoice.pitch = 1f;
                if (cas == 1 || cas == 3 || cas == 7)
                {
                    // Talk happy, normal.
                    animator.Play("TalkHappyNormal");
                }
                else
                {
                    // Talk happy, fast.
                    animator.Play("TalkHappyFast");
                }
            }
        }
        else
        {
            // Stop talking animation.
            animator.SetBool("StopTalking", true);
        }
    }

    public void SetVisible(bool visible)
    {
        this.visible = visible;
    }

    public bool IsVisible()
    {
        return visible;
    }
}
