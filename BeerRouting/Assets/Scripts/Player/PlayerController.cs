using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Sprite headNormal;
    public Sprite headSunglasses;

    private LevelController levelController;
    private GameObject[] mouths;
    private Animator animator;
    private int mouthId;
    private bool drinking, throwDisabled;
    private AudioSource jumpSound;
    private SpriteRenderer spriteRendererHead;

    // Use this for initialization
    void Awake()
    {
        levelController = LevelController.GetCurrentLevelController();
        animator = GetComponent<Animator>();
        drinking = false;
        throwDisabled = false;
        jumpSound = GetComponent<AudioSource>();

        // Find "Head" element.
        GameObject playerHead = null;
        Transform parentTransform = this.transform;
        foreach (Transform childTransform in parentTransform)
        {
            if (childTransform.name == "head")
            {
                playerHead = childTransform.gameObject;
            }
            else
            {
                foreach (Transform secondLayerChild in childTransform)
                {
                    if (secondLayerChild.name == "head")
                    {
                        playerHead = secondLayerChild.gameObject;
                    }
                }
            }
        }

        if (playerHead != null)
        {
            int numChildren = playerHead.transform.childCount;
            mouths = new GameObject[numChildren];

            int index = 0;
            foreach (Transform child in playerHead.transform)
            {
                mouths[index] = child.gameObject;
                mouths[index].SetActive(false);
                index++;
            }

            // Set normal mouth as init state.
            mouthId = 1;

            spriteRendererHead = playerHead.GetComponent<SpriteRenderer>();
            spriteRendererHead.sprite = headNormal;
        }

        StartCoroutine(ThrowBeerAfterTime());
        // SetSunglasses(true);
    }

    public void SetMouth(int id)
    {
        mouthId = id;
    }

    public void SetSunglasses(bool sunglasses)
    {
        if (sunglasses)
        {
            spriteRendererHead.sprite = headSunglasses;
        }
        else
        {
            spriteRendererHead.sprite = headNormal;
        }
    }

    public void ChangeMouth()
    {
        //        Debug.Log("SetMouth id:" + mouthId);
        if (mouthId < mouths.Length)
        {
            for (int i = 0; i < mouths.Length; i++)
            {
                mouths[i].SetActive(false);
            }
            mouths[mouthId].SetActive(true);
        }
    }

    public void Drink()
    {
        animator.SetBool("Drink", true);
    }

    public void Jump()
    {
        if (levelController != null && levelController.IsPlayerWalking() == true)
        {
            animator.SetBool("Jump", true);
            jumpSound.Play();
        }
    }

    public void StopJump()
    {
        animator.SetBool("Jump", false);
    }

    public void ThrowBeer()
    {
        animator.SetBool("Throw", true);
    }

    public void StopThrowBeer()
    {
        animator.SetBool("Throw", false);
    }

    public void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.B))
        {
            Drink();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ThrowBeer();
        }
        */
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void LateUpdate()
    {
        if (!drinking)
        {
            ChangeMouth();
        }
    }

    public void SetDrinking(bool drinking)
    {
        this.drinking = drinking;
    }

    public void SetThrowDisabled(bool throwDisabled)
    {
        this.throwDisabled = throwDisabled;
    }

    IEnumerator ThrowBeerAfterTime()
    {
        // Wait a random time.
        int time = Random.Range(5, 20);
        yield return new WaitForSeconds(time);

        // Do nothing after drink animation was played.
        if (!drinking && !throwDisabled)
        {
            // Throw beer and restart this corutine.
            ThrowBeer();
            StartCoroutine(ThrowBeerAfterTime());
        }
    }
}
