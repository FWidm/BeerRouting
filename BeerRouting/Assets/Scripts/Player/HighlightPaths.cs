using UnityEngine;
using System.Collections;

public class HighlightPaths : MonoBehaviour
{

    private bool isHighlighted;

    // Use this for initialization
    void Start()
    {
        isHighlighted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            if (!isHighlighted)
            {
                Debug.Log("Highlighting of paths requested.");
                GameObject[] pathHighlights = GameObject.FindGameObjectsWithTag("PathHighlight");
                foreach (GameObject pathHighlight in pathHighlights)
                {
                    pathHighlight.GetComponent<SpriteRenderer>().enabled = true;
                }
                Debug.Log("PathHighlights list has " + pathHighlights.Length + " objects.");
                isHighlighted = true;
            }
        }
        else
        {
            if (isHighlighted)
            {
                GameObject[] pathHighlights = GameObject.FindGameObjectsWithTag("PathHighlight");
                foreach (GameObject pathHighlight in pathHighlights)
                {
                    pathHighlight.GetComponent<SpriteRenderer>().enabled = false;
                }
                isHighlighted = false;
            }
        }
    }
}
