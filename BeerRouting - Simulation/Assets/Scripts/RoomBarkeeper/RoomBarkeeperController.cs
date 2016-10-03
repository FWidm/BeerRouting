using UnityEngine;
using System.Collections;

public class RoomBarkeeperController : MonoBehaviour
{
    public bool isLogEnabled = false;

    private GameObject[] mouths;
    private Animator animator;

    // Indicates whether the barkeeper is currently in the waving state.
    private bool isWaving = false;

    // Use this for initialization
    void Awake()
    {
        if (isLogEnabled)
            Debug.Log("Awaking the room barkeeper controller.");

        animator = GetComponent<Animator>();
        IdleNoBeer();

        // Find "Head" element.
        GameObject playerHead = null;
        Transform parentTransform = this.transform;
        foreach (Transform childTransform in parentTransform)
        {
            if (childTransform.name == "head")
            {
                if (isLogEnabled)
                    Debug.Log("Found a head element for room barkeeper: " + this.name);

                playerHead = childTransform.gameObject;
            }
            else
            {
                foreach (Transform secondLayerChild in childTransform)
                {
                    if (secondLayerChild.name == "head")
                    {
                        if (isLogEnabled)
                            Debug.Log("Found a head element for room barkeeper: " + this.name);

                        playerHead = secondLayerChild.gameObject;
                    }
                }
            }
        }

        if (playerHead != null)
        {
            int numChildren = playerHead.transform.childCount;
            mouths = new GameObject[numChildren];

            if (isLogEnabled)
                Debug.Log("Found " + numChildren + " mouth game objects for room barkeeper: " + this.name);

            int index = 0;
            foreach (Transform child in playerHead.transform)
            {
                mouths[index] = child.gameObject;
                mouths[index].SetActive(false);
                index++;
            }

            // Set normal mouth as init state.
            mouths[1].SetActive(true);
        }
    }

    public void SetMouth(int id)
    {
        if (id < mouths.Length)
        {
            for (int i = 0; i < mouths.Length; i++)
            {
                mouths[i].SetActive(false);
            }
            mouths[id].SetActive(true);
        }
    }

    public void Wave()
    {
        animator.SetBool("Wave", true);
        animator.SetBool("IdleNoBeer", false);
        animator.SetBool("IdleWithBeer", false);
        isWaving = true;
    }

    public void IdleNoBeer()
    {
        animator.SetBool("Wave", false);
        animator.SetBool("IdleNoBeer", true);
        animator.SetBool("IdleWithBeer", false);
        isWaving = false;
    }

    public void IdleWithBeer()
    {
        animator.SetBool("Wave", false);
        animator.SetBool("IdleNoBeer", false);
        animator.SetBool("IdleWithBeer", true);
        isWaving = false;
    }

    /// <summary>
    /// Indicates whether the barkeeper game object is currently in the waving state.
    /// </summary>
    /// <returns>Returns true, if the barkeeper is waving, otherwise false.</returns>
    public bool IsWaving()
    {
        return isWaving;
    }
}
