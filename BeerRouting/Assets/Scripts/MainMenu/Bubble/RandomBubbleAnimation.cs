using UnityEngine;
using System.Collections;

public class RandomBubbleAnimation : MonoBehaviour
{

    public Animator animator;

    // Use this for initialization
    void Start()
    {
        SetRandomAnimation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetRandomAnimation()
    {
        // Start idle animation at random frame.
        float startPoint = Random.Range(0f, 1f);
        int r = Random.Range(0, 3);
        if (r == 0)
        {
            animator.Play("AnimateBeerBubble", 0, startPoint);
        }
        else if (r == 1)
        {
            animator.Play("AnimateBeerBubble2", 0, startPoint);
        }
        else
        {
            animator.Play("AnimateBeerBubble3", 0, startPoint);
        }
    }
}
