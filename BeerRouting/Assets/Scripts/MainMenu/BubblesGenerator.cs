using UnityEngine;
using System.Collections.Generic;

public class BubblesGenerator : MonoBehaviour
{

    public GameObject prefabBubble;
    public int numBubbles;
    public int bubbleDelay;
    public int countBubbles;
    private List<GameObject> gameObjectList;
    public float xAnimationOffset = 6.5f;

    // Use this for initialization
    void Start()
    {
        gameObjectList = new List<GameObject>();
        InvokeRepeating("SpawnBubble", .1f, .1f);
        // Reset for fun level professor.
        ApplicationState.levelRestarted = false;
    }
	
    // Update is called once per frame
    void  SpawnBubble()
    {
        float scale = Random.Range(0.2f, 1.1f);
        if (gameObjectList.Count < numBubbles)
        {
            CameraBounds camScript = FindObjectOfType<CameraBounds>();

            GameObject gameObject = (GameObject)Instantiate(prefabBubble);
            gameObject.transform.parent = this.transform.parent;

            gameObject.transform.parent = this.transform;

            gameObject.name = "Bubble " + gameObjectList.Count;
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
            //Debug.Log("Camerabounds=" + camScript.OrthographicBounds());
            RandomizeBubblePosition bubblepos = gameObject.GetComponent<RandomizeBubblePosition>();
            //orthographic size is equal to half the camera width
            float upperXBound = camScript.OrthographicBounds().size.x * 2;
            bubblepos.xOffset = Random.Range(xAnimationOffset, upperXBound);
            bubblepos.yOffset = Random.Range(-2f, 8f);
            gameObjectList.Add(gameObject);
        }
        else
        {
            CancelInvoke("SpawnBubble");
        }

    }

}
