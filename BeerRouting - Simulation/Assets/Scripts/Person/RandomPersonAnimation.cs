using UnityEngine;
using System.Collections;

public class RandomPersonAnimation : MonoBehaviour {

    public Animator animator; 

	// Use this for initialization
	void Start () {
        SetRandomAnimation();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void SetRandomAnimation()
    {
        // Start idle animation at random frame.
        float startPoint = Random.Range(0f, 1f);
        int r = Random.Range(0, 3);
        if (r == 0)
        {
            animator.Play("Idle1", -1, startPoint);
        }
        else if(r == 1)
        {
            animator.Play("Idle2", -1, startPoint);
        }
        else {
            animator.Play("Idle3", -1, startPoint);
        }
    }
}
