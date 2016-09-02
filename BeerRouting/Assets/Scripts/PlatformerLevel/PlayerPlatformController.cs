using UnityEngine;
using System.Collections;

public class PlayerPlatformController : MonoBehaviour {

    public Sprite headNormal;
    public Sprite headSunglasses;
    private SpriteRenderer spriteRendererHead;
    private Animator animator;
    private bool drinking = false;

    // Use this for initialization
    void Awake()
    {
        // Find "Head" element.
        animator = GetComponent<Animator>();
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
            spriteRendererHead = playerHead.GetComponent<SpriteRenderer>();
            spriteRendererHead.sprite = headNormal;
        }
        StartBeerThrowing();
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

    public void Kill()
    {
        this.gameObject.SetActive(false);
    }

    public void Drink()
    {
        animator.SetBool("Drink", true);
    }

    public void ThrowBeer()
    {
        animator.SetBool("Throw", true);
    }

    IEnumerator ThrowBeerAfterTime()
    {
        // Wait a random time.
        int time = Random.Range(5, 20);
        yield return new WaitForSeconds(time);

        // Do nothing after drink animation was played.
        if (!drinking)
        {
            // Throw beer and restart this corutine.
            ThrowBeer();
            StartCoroutine(ThrowBeerAfterTime());
        }
    }

    public void SetDrinking(bool drinking)
    {
        this.drinking = drinking;
    }

    public void StartBeerThrowing()
    {
        StartCoroutine(ThrowBeerAfterTime());
    }
}
